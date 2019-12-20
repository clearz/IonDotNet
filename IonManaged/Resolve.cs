using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IonLangManaged
{
    using static TypeKind;
    using static SymState;
    using static DeclKind;
    using static SymKind;
    using static TypespecKind;
    using static ExprKind;
    using static TokenKind;
    using static AggregateKind;
    using static AggregateItemKind;
    using static SymReachable;
    using static CompoundFieldKind;
    using static TokenSuffix;
    using static StmtKind;

    unsafe partial class Ion
    {
        const int MAX_LOCAL_SYMS = 1024;
        const int MAX_PATH = 512;

        Package current_package;
        Package builtin_package;

        Dictionary<string, Package> package_map;
        Dictionary<object, Sym> resolved_sym_map;
        Dictionary<Expr, Type> resolved_expected_type_map;
        Dictionary<object, Type> resolved_type_map;
        Dictionary<string, Label> labels_map;
        Dictionary<Expr, Type> pointer_promo_map;
        Dictionary<Expr, Val> resolved_val_map;
        Dictionary<Type, SymReachable> reachable_map;
        Dictionary<Expr, Type> type_conv_map;

        List<string>  decl_note_names;
        List<Expr> implicit_any_list;
        List<Label> labels;
        List<Package> package_list;
        List<Sym> sorted_syms;
        List<Sym> reachable_syms;

        Type type_allocator;
        Type type_allocator_ptr;

        Sym[] local_syms;
        int local_sym_pos;

        SymReachable reachable_phase = REACHABLE_NATURAL;

        Label get_label(SrcPos pos, string name) {
            var label = labels_map.ContainsKey(name) ? labels_map[name] : null;
            if (label == null) {
                label = new Label { name = name, pos = pos };
                labels.Add(label);
                labels_map[name] = label;
            }

            return label;
        }

        void reference_label(SrcPos pos, string name) {
            Label label = get_label(pos, name);
            label.referenced = true;
        }

        void define_label(SrcPos pos, string name) {
            Label label = get_label(pos, name);
            if (label.defined) {
                fatal_error(pos, "Multiple definitions of label '{0}'", name);
            }
            label.defined = true;
        }

        void resolve_labels() {
            foreach (Label label in labels) {
                if (label.referenced && !label.defined) {
                    fatal_error(label.pos, "Label '{0}' referenced but not defined", label.name);
                }
                if (label.defined && !label.referenced) {
                    warning(label.pos, "Label '{0}' defined but not referenced", label.name);
                }
            }
            labels.Clear();
        }

        Sym get_package_sym(Package package, string name) {
            if (package.symbol_dict.ContainsKey(name))
                return package.symbol_dict[name];
            //foreach(var pkg in package.ref_pkg) {
                // TODO: maybe go recursive here
                //if (pkg.symbol_dict.ContainsKey(name))
                 //   return pkg.symbol_dict[name];
            //}
            return null;
        }

        void add_package(Package package) {
            Package old_package = package_map.ContainsKey(package.path) ? package_map[package.path] : null;
            if (old_package != package) {
                assert(old_package == null);
                package_map[package.path] = package;
                package_list.Add(package);
            }
        }

        Package enter_package(Package new_package) {
            Package old_package = current_package;
            current_package = new_package;
            return old_package;
        }

        void leave_package(Package old_package) {
            current_package = old_package;
        }

        bool is_local_sym(Sym sym) {
            return local_syms.Contains(sym);
        }

        internal Sym sym_new(SymKind kind, string name, Decl decl) {
            var sym = new Sym {kind = kind, name = name, decl = decl, home_package = current_package};

            set_resolved_sym(sym, sym);
            return sym;
        }

        void process_decl_notes(Decl decl, Sym sym) {
            Note? foreign_note = get_decl_note(decl, foreign_name);
            if (foreign_note != null) {
                if (foreign_note.Value.num_args > 1) {
                    fatal_error(decl.pos, "@foreign takes 0 or 1 argument");
                }
                string external_name;
                if (foreign_note.Value.num_args == 0) {
                    external_name = sym.name;
                }
                else {
                    Expr arg = foreign_note.Value.args[0].expr;
                    if (arg.kind != EXPR_STR) {
                        fatal_error(decl.pos, "@foreign argument 1 must be a string literal");
                    }
                    external_name = arg.str_lit.val;
                }
                sym.external_name = external_name;
            }
        }

        Sym sym_decl(Decl decl) {
            var kind = SYM_NONE;
            switch (decl.kind) {
                case DECL_STRUCT:
                case DECL_UNION:
                case DECL_TYPEDEF:
                case DECL_ENUM:
                    kind = SYM_TYPE;
                    break;
                case DECL_VAR:
                    kind = SYM_VAR;
                    break;
                case DECL_CONST:
                    kind = SYM_CONST;
                    break;
                case DECL_FUNC:
                    kind = SYM_FUNC;
                    break;
                default:
                    assert(false);
                    break;
            }

            var sym = sym_new(kind, decl.name, decl);
            set_resolved_sym(decl, sym);
            process_decl_notes(decl, sym);
            return sym;
        }

        Sym sym_get_local(string name) {
            for (var i = local_sym_pos; i >= 0; --i) {
                var sym = local_syms[i];
                if (sym != null && sym.name == name)
                    return sym;
            }

            return null;
        }

        Sym sym_get(string name) {
            Sym sym = sym_get_local(name);
            return sym ?? get_package_sym(current_package, name);
        }

        bool sym_push_var(string name, Type type) {
            if (sym_get_local(name) != null) {
                return false;
            }
            if (local_sym_pos == MAX_LOCAL_SYMS)
                fatal("Too many local symbols");


            var sym = new Sym {name = name, kind = SYM_VAR, state = SYM_RESOLVED, type = type};
            local_syms[++local_sym_pos] = sym;

            return true;
        }

        int sym_enter() {
            return local_sym_pos;
        }

        void sym_leave(int i) {
            local_sym_pos = i;
        }

        void sym_global_put(string name, Sym sym) {
            Sym old_sym = current_package.symbol_dict.ContainsKey(name) ? current_package.symbol_dict[name] : null;
            if (old_sym != null && !(sym.home_package == current_package && old_sym.home_package != current_package)) {
                if (sym == old_sym) {
                    return;
                }
                if (sym.kind == SYM_PACKAGE && old_sym.kind == SYM_PACKAGE && sym.package == old_sym.package) {
                    return;
                }
                SrcPos pos = sym.decl != null ? sym.decl.pos : pos_builtin;
                if (old_sym.decl != null) {
                    warning(old_sym.decl.pos, "Previous definition of '{0}'", name);
                }
                else if (sym.home_package == current_package) {
                    fatal_error(pos, "Duplicate definition of symbol '{0}'.", name);
                }
                else {
                    fatal_error(pos, "Conflicting import of symbol {0} into {1} from {2} and {3}.", name, current_package.path, sym.home_package.path, old_sym.home_package.path);
                }
            }
            current_package.symbol_dict[name] = sym;
            current_package.symbols.Add(sym);
        }

        Sym sym_global_decl(Decl decl) {
            Sym sym = null;
            if (decl.name != null) {
                sym = sym_decl(decl);
                sym_global_put(sym.name, sym);
            }
            if (decl.kind == DECL_ENUM) {
                string name = sym != null ? sym.name : "int";
                Typespec enum_typespec = enum_typespec = new_typespec_name(decl.pos, new []{name}, 1);
                string prev_item_name = null;
                for (int i = 0; i < decl.enum_decl.num_items; i++) {
                    EnumItem item = decl.enum_decl.items[i];
                    Expr init;
                    if (item.init != null) {
                        init = item.init;
                    }
                    else if (prev_item_name != null) {
                        init = new_expr_binary(item.pos, TOKEN_ADD, new_expr_name(item.pos, prev_item_name), new_expr_int(item.pos, 1, 0, 0));
                    }
                    else {
                        init = new_expr_int(item.pos, 0, 0, 0);
                    }
                    Decl item_decl = new_decl_const(item.pos, item.name, enum_typespec, init);
                    item_decl.notes = decl.notes;
                    sym_global_decl(item_decl);
                    prev_item_name = item.name;
                }
            }
            return sym;
        }

        Sym sym_global_type(string name, Type type) {
            Sym sym = sym_new(SYM_TYPE, name, null);
            sym.state = SYM_RESOLVED;
            sym.type = type;
            sym.external_name = name;
            sym_global_put(name, sym);
            return sym;
        }

        Sym sym_global_tuple(string name, Type type) {
            Sym sym = sym_new(SYM_TYPE, name, null);
            sym.state = SYM_RESOLVED;
            sym.type = type;
            sym.external_name = name;
            Package old_package = enter_package(builtin_package);
            sym_global_put(name, sym);
            leave_package(old_package);
            sorted_syms.Add(sym);
            reachable_syms.Add(sym);
            sym.reachable = REACHABLE_NATURAL;
            return sym;
        }


        Type get_resolved_type(object obj) {
            assert(resolved_type_map.ContainsKey(obj));
            return resolved_type_map.ContainsKey(obj) ? resolved_type_map[obj] : null;
        }

        void set_resolved_type(object obj, Type type) {
            resolved_type_map[obj] = type;
        }

        Sym get_resolved_sym(object obj) {
            if (!resolved_sym_map.ContainsKey(obj))
                return null;
            return resolved_sym_map[obj];
        }

        void set_resolved_sym(object obj, Sym sym) {
            if (!is_local_sym(sym)) {
                resolved_sym_map[obj] = sym;
            }
        }

        Type get_resolved_expected_type(Expr expr) {
            return resolved_expected_type_map.ContainsKey(expr) ? resolved_expected_type_map[expr] : null;
        }

        void set_resolved_expected_type(Expr expr, Type type) {
            if (expr != null && type != null)
                resolved_expected_type_map[expr] = type;
        }

        Operand operand_rvalue(Type type) {
            return new Operand { type = unqualify_type(type) };
        }

        Operand operand_lvalue(Type type) {
            return new Operand { type = type, is_lvalue = true };
        }

        Operand operand_const(Type type, Val val) {
            return new Operand { type = unqualify_type(type), is_const = true, val = val };
        }

        Type type_decay(Type type) {
            type = unqualify_type(type);
            if (type.kind == TYPE_ARRAY) {
                return type_ptr(type.@base);
            }
            return type;
        }

        Type incomplete_decay(Type type) {
            if (is_incomplete_array_type(type) || is_ptr_type(type)) {
                return type_ptr(incomplete_decay(type.@base));
            }
            return type;
        }

        Operand operand_decay(Operand operand) {
            operand.type = type_decay(operand.type);
            operand.is_lvalue = false;
            return operand;
        }

        bool is_convertible(ref Operand operand, Type dest) {
            dest = unqualify_type(dest);
            Type src = unqualify_type(operand.type);
            if (dest == src) {
                return true;
            }
            else if (is_func_type(src) && src.func.intrinsic) {
                return false;
            }
            else if (dest == type_any || dest == type_void) {
                return true;
            }
            else if (is_arithmetic_type(dest) && is_arithmetic_type(src)) {
                return true;
            }
            else if (is_ptr_like_type(dest) && is_null_ptr(ref operand)) {
                return true;
            }
            else if (is_ptr_type(dest) && is_ptr_type(src)) {
                if (is_const_type(dest.@base) && is_const_type(src.@base)) {
                    return dest.@base.@base == src.@base.@base || dest.@base.@base == type_void || src.@base.@base == type_void;
                }
                else if (is_aggregate_type(dest.@base) && is_aggregate_type(src.@base) && dest.@base == src.@base.aggregate.fields[0].type) {
                    return true;
                }
                else {
                    Type unqual_dest_base = unqualify_type(dest.@base);
                    if (unqual_dest_base == src.@base) {
                        return true;
                    }
                    else if (unqual_dest_base == type_void) {
                        return is_const_type(dest.@base) || !is_const_type(src.@base);
                    }
                    else {
                        return src.@base == type_void;
                    }
                }
            }
            else {
                return false;
            }
        }

        void put_type_name(StringBuilder buf, Type type) {
            string type_name = type_names[(int)type.kind];
            if (type_name != null) {
                buf.Append(type_name);
            }
            else {
                switch (type.kind) {
                    case TYPE_STRUCT:
                    case TYPE_UNION:
                    case TYPE_ENUM:
                    case TYPE_INCOMPLETE:
                        assert(type.sym != null);
                        buf.Append(type.sym.name);
                        break;
                    case TYPE_CONST:
                        put_type_name(buf, type.@base);
                        buf.Append(" const");
                        break;
                    case TYPE_PTR:
                        put_type_name(buf, type.@base);
                        break;
                    case TYPE_ARRAY:
                        put_type_name(buf, type.@base);
                        buf.Append(type.num_elems);
                        break;
                    case TYPE_FUNC:
                        buf.Append("func(");
                        for (var i = 0; i < type.func.num_params; i++) {
                            if (i != 0) {
                                buf.Append(", ");
                            }
                            put_type_name(buf, type.func.@params[i]);
                        }
                        if (type.func.has_varargs) {
                            buf.Append("...");
                        }
                        buf.Append(')');
                        if (type.func.ret != type_void) {
                            buf.Append(": ");
                            put_type_name(buf, type.func.ret);
                        }
                        break;
                    case TYPE_TUPLE:
                        buf.Append('{');
                        for (int i = 0; i < type.aggregate.num_fields; i++) {
                            if (i != 0) {
                                buf.Append(", ");
                            }
                            put_type_name(buf, type.aggregate.fields[i].type);
                        }
                        buf.Append('}');
                        break;
                    default:
                        assert(0);
                        break;
                }
            }
        }

        string get_type_name(Type type) {
            var buf = new StringBuilder();
            put_type_name(buf, type);
            return buf.ToString();
        }

        bool is_castable(ref Operand operand, Type dest) {
            Type src = operand.type;
            if (is_convertible(ref operand, dest)) {
                return true;
            }
            else if (is_integer_type(dest)) {
                return is_ptr_like_type(src);
            }
            else if (is_integer_type(src)) {
                return is_ptr_like_type(dest);
            }
            else if (is_ptr_like_type(dest) && is_ptr_like_type(src)) {
                return true;
            }
            else {
                return false;
            }
        }

        bool convert_operand(ref Operand operand, Type type) {
            if (is_convertible(ref operand, type)) {
                cast_operand(ref operand, type);
                operand.type = unqualify_type(operand.type);
                operand.is_lvalue = false;
                return true;
            }
            return false;
        }

        bool cast_operand(ref Operand operand, Type type) {
            Type qual_type = type;
            type = unqualify_type(type);
            operand.type = unqualify_type(operand.type);

            if (operand.type != type) {

                if (!is_castable(ref operand, type)) {
                    return false;
                }
                if (operand.is_const) {
                    if (is_floating_type(operand.type)) {
                        operand.is_const = !is_integer_type(type);
                    }
                    else {
                        if (type.kind == TYPE_ENUM) {
                            type = type.@base;
                        }
                        Type operand_type = operand.type;
                        if (operand_type.kind == TYPE_ENUM) {
                            operand_type = operand_type.@base;
                        }
                        switch (operand_type.kind) {
                            case TYPE_BOOL: {
                                var p = operand.val.b ? 1 : 0;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p == 1;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_CHAR: {
                                var p = operand.val.c;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)(int)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_UCHAR: {
                                var p = operand.val.uc;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_SCHAR: {
                                var p = operand.val.sc;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_SHORT: {
                                var p = operand.val.s;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_USHORT: {
                                var p = operand.val.us;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_INT: {
                                var p = operand.val.i;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_UINT: {
                                var p = operand.val.u;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_LONG: {
                                var p = operand.val.l;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_LLONG: {
                                var p = operand.val.ll;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            case TYPE_ULLONG: {
                                var p = operand.val.ull;
                                switch (type.kind) {
                                    case TYPE_BOOL:
                                        operand.val.b = p != 0;
                                        break;
                                    case TYPE_CHAR:
                                        operand.val.c = (char)p;
                                        break;
                                    case TYPE_UCHAR:
                                        operand.val.uc = (byte)p;
                                        break;
                                    case TYPE_SCHAR:
                                        operand.val.sc = (sbyte)p;
                                        break;
                                    case TYPE_SHORT:
                                        operand.val.s = (short)p;
                                        break;
                                    case TYPE_USHORT:
                                        operand.val.us = (ushort)p;
                                        break;
                                    case TYPE_INT:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_UINT:
                                        operand.val.u = (uint)p;
                                        break;
                                    case TYPE_LONG:
                                        operand.val.l = (int)p;
                                        break;
                                    case TYPE_ULONG:
                                        operand.val.ul = (uint)p;
                                        break;
                                    case TYPE_LLONG:
                                        operand.val.ll = (long)p;
                                        break;
                                    case TYPE_ULLONG:
                                        operand.val.ull = (ulong)p;
                                        break;
                                    case TYPE_ENUM:
                                        operand.val.i = (int)p;
                                        break;
                                    case TYPE_FLOAT:
                                    case TYPE_DOUBLE:
                                        break;
                                    case TYPE_PTR:
                                        operand.val.p = (void*)p;
                                        break;
                                    default:
                                        operand.is_const = false;
                                        break;
                                }

                                break;
                            }
                            default:
                                operand.is_const = false;
                                break;
                        }
                    }
                }
            }
            operand.type = qual_type;
            return true;
        }

        bool is_null_ptr(ref Operand operand) {
            if (operand.is_const && (is_ptr_type(operand.type) || is_integer_type(operand.type))) {
                cast_operand(ref operand, type_ullong);
                return operand.val.ull == 0;
            }
            else {
                return false;
            }
        }
        Val convert_const(Type dest_type, Type src_type, Val src_val) {
            Operand operand = operand_const(src_type, src_val);
            cast_operand(ref operand, dest_type);
            return operand.val;
        }

        void promote_operand(ref Operand operand) {
            switch (operand.type.kind) {
                case TYPE_BOOL:
                case TYPE_CHAR:
                case TYPE_SCHAR:
                case TYPE_UCHAR:
                case TYPE_SHORT:
                case TYPE_USHORT:
                case TYPE_ENUM:
                    cast_operand(ref operand, type_int);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        void unify_arithmetic_operands(ref Operand left, ref Operand right) {
            if (left.type == type_double) {
                cast_operand(ref right, type_double);
            }
            else if (right.type == type_double) {
                cast_operand(ref left, type_double);
            }
            else if (left.type == type_float) {
                cast_operand(ref right, type_float);
            }
            else if (right.type == type_float) {
                cast_operand(ref left, type_float);
            }
            else {
                assert(is_integer_type(left.type));
                assert(is_integer_type(right.type));
                promote_operand(ref left);
                promote_operand(ref right);
                if (left.type != right.type) {
                    if (is_signed_type(left.type) == is_signed_type(right.type)) {
                        if (type_rank(left.type) <= type_rank(right.type)) {
                            cast_operand(ref left, right.type);
                        }
                        else {
                            cast_operand(ref right, left.type);
                        }
                    }
                    else if (is_signed_type(left.type) && type_rank(right.type) >= type_rank(left.type)) {
                        cast_operand(ref left, right.type);
                    }
                    else if (is_signed_type(right.type) && type_rank(left.type) >= type_rank(right.type)) {
                        cast_operand(ref right, left.type);
                    }
                    else if (is_signed_type(left.type) && type_sizeof(left.type) > type_sizeof(right.type)) {
                        cast_operand(ref right, left.type);
                    }
                    else if (is_signed_type(right.type) && type_sizeof(right.type) > type_sizeof(left.type)) {
                        cast_operand(ref left, right.type);
                    }
                    else {
                        Type type = unsigned_type(is_signed_type(left.type) ? left.type : right.type);
                        cast_operand(ref left, type);
                        cast_operand(ref right, type);
                    }
                }
            }
            assert(left.type == right.type);
        }

        Val get_resolved_val(Expr e) {
            Val val = resolved_val_map[e];
            return val;
        }

        void set_resolved_val(Expr e, Val val) {
            resolved_val_map[e] = val;
        }

        void set_reachable(Type type) {
            if (reachable_map.ContainsKey(type))
                return;

            reachable_map[type] = reachable_phase;
        }

        SymReachable get_reachable(Type type) {
            return (SymReachable)reachable_map[type];
        }

        Type unify_arithmetic_types(Type left_type, Type right_type) {
            Operand left = operand_rvalue(left_type);
            Operand right = operand_rvalue(right_type);
            unify_arithmetic_operands(ref left, ref right);
            assert(left.type == right.type);
            return left.type;
        }

        Type promote_type(Type type) {
            Operand operand = operand_rvalue(type);
            promote_operand(ref operand);
            return operand.type;
        }

        Type resolve_typespec_strict(Typespec typespec, bool with_const) {
            if (typespec == null)
                return type_void;

            Type result = null;
            switch (typespec.kind) {
                case TYPESPEC_NAME: {
                    Package package = current_package;
                    for (var i = 0; i < typespec.num_names - 1; i++) {
                        string name = typespec.names[i];
                        Sym sym2 = get_package_sym(package, name);
                        if (sym2 == null) {
                            fatal_error(typespec.pos, "Unresolved package '{0}'", name);
                        }
                        if (sym2.kind != SYM_PACKAGE) {
                            fatal_error(typespec.pos, "{0} must denote a package", name);
                        }
                        package = sym2.package;
                    }
                    {
                        string name = typespec.names[typespec.num_names - 1];
                        Sym sym = get_package_sym(package, name);
                        if (sym == null) {
                            fatal_error(typespec.pos, "Unresolved type name '{0}'", name);
                        }
                        if (sym.kind != SYM_TYPE) {
                            fatal_error(typespec.pos, "{0} must denote a type", name);
                        }
                        resolve_sym(sym);
                        set_resolved_sym(typespec, sym);
                        result = sym.type;
                    }
                }
                break;
                case TYPESPEC_CONST:
                    result = resolve_typespec_strict(typespec.@base, with_const);
                    if (with_const) {
                        result = type_const(result);
                    }
                    break;
                case TYPESPEC_PTR:
                    result = type_ptr(resolve_typespec_strict(typespec.@base, with_const));
                    break;
                case TYPESPEC_ARRAY:
                    int size = 0;
                    Type  @base = resolve_typespec_strict(typespec.@base, with_const);
                    if (typespec.num_elems != null) {
                        Operand operand = resolve_const_expr(typespec.num_elems);
                        if (!is_integer_type(operand.type)) {
                            fatal_error(typespec.pos, "Array size constant expression must have integer type");
                        }
                        cast_operand(ref operand, type_int);
                        size = operand.val.i;
                        if (size < 0) {
                            fatal_error(typespec.num_elems.pos, "Negative array size");
                        }
                    }

                    result = type_array(@base, size, typespec.num_elems == null);
                    break;
                case TYPESPEC_FUNC:
                    var args = new List<Type>();
                    for (var i = 0; i < typespec.func.num_args; i++) {
                        Type arg = resolve_typespec_strict(typespec.func.args[i], with_const);
                        if (arg == type_void) {
                            fatal_error(typespec.pos, "Function parameter type cannot be void");
                        }
                        arg = incomplete_decay(arg);
                        args.Add(arg);
                    }
                    var ret = type_void;
                    if (typespec.func.ret != null)
                        ret = incomplete_decay(resolve_typespec_strict(typespec.func.ret, with_const));
                    if (is_array_type(ret)) {
                        fatal_error(typespec.pos, "Function return type cannot be array");
                    }
                    // TODO: func pointers should be able to support varargs (including typed)
                    result = type_func(args.ToArray(), args.Count, ret, false, false, type_void);
                    break;
                case TYPESPEC_TUPLE:
                    var fields = new List<Type>();
                    for (int i = 0; i < typespec.tuple.num_fields; i++) {
                        Type field = resolve_typespec_strict(typespec.tuple.fields[i], with_const);
                        if (field == type_void) {
                            fatal_error(typespec.pos, "Tuple element types cannot be void");
                        }
                        fields.Add(field);
                    }
                    result = type_tuple(fields.ToArray(), fields.Count);
                    set_reachable(result);
                    break;

                default:
                    assert(false);
                    return null;
            }
            set_resolved_type(typespec, result);
            return result;
        }

        Type resolve_typespec(Typespec typespec) {
            return resolve_typespec_strict(typespec, false);
        }

        Type complete_aggregate_strict(Type type, Aggregate aggregate, bool with_const) {
            var fields = new List<TypeField>();
            for (var i = 0; i < aggregate.num_items; i++) {
                AggregateItem item = aggregate.items[i];
                if (item.kind == AGGREGATE_ITEM_FIELD) {
                    Type item_type = resolve_typespec_strict(item.type, with_const);
                    item_type = incomplete_decay(item_type);
                    complete_type(item_type);
                    if (type_sizeof(item_type) == 0) {
                        if (!is_array_type(item_type) || type_sizeof(item_type.@base) == 0) {
                            fatal_error(item.pos, "Field type of size 0 is not allowed");
                        }
                    }
                    for (var j = 0; j < item.num_names; j++) {
                        fields.Add(new TypeField { name = item.names[j], type = item_type });
                    }
                }
                else {
                    assert(item.kind == AGGREGATE_ITEM_SUBAGGREGATE);
                    Type item_type = complete_aggregate_strict(null, item.subaggregate, with_const);
                    fields.Add(new TypeField { type = item_type });
                }
            }
            if (type == null) {
                type = type_incomplete(null);
                type.kind = TYPE_COMPLETING;
            }
            if (aggregate.kind == AGGREGATE_STRUCT) {
                type_complete_struct(type, fields.ToArray(), fields.Count);
            }
            else {
                assert(aggregate.kind == AGGREGATE_UNION);
                type_complete_union(type, fields.ToArray(), fields.Count);
            }
            if (type.aggregate.num_fields == 0) {
                fatal_error(aggregate.pos, "No fields");
            }
            if (has_duplicate_fields(type)) {
                fatal_error(aggregate.pos, "Duplicate fields");
            }
            return type;
        }

        Type complete_aggregate(Type type, Aggregate aggregate) {
            return complete_aggregate_strict(type, aggregate, type.sym != null && is_decl_foreign(type.sym.decl));
        }

        void complete_type(Type type) {
            if (type.kind == TYPE_COMPLETING) {
                fatal_error(type.sym.decl.pos, "Type completion cycle");
                return;
            }

            if (type.kind != TYPE_INCOMPLETE)
                return;

            Sym sym = type.sym;
            Package old_package = enter_package(sym.home_package);
            Decl decl = sym.decl;

            if (decl.is_incomplete) {
                fatal_error(decl.pos, "Trying to use incomplete type as complete type");
            }
            type.kind = TYPE_COMPLETING;
            assert(decl.kind == DECL_STRUCT || decl.kind == DECL_UNION);
            complete_aggregate(type, decl.aggregate);
            sorted_syms.Add(type.sym);
            leave_package(old_package);
        }

        Type resolve_typed_init(SrcPos pos, Type type, Expr expr) {
            Type expected_type = unqualify_type(type);
            Operand operand = resolve_expected_expr(expr, expected_type);
            if (is_incomplete_array_type(type)) {
                if (is_array_type(operand.type) && type.@base == operand.type.@base) {
                    // Incomplete array size, so infer the size from the initializer expression's type.
                    type.num_elems = operand.type.num_elems;
                    type.size = operand.type.size;
                    type.incomplete_elems = false;
                    set_resolved_expected_type(expr, type);
                    return type;
                }
                else if (is_ptr_type(operand.type) && type.@base == operand.type.@base) {
                    set_resolved_expected_type(expr, operand.type);
                    return operand.type;
                }
            }
            if (type != null && is_ptr_type(type)) {
                operand = operand_decay(operand);
            }
            if (!convert_operand(ref operand, expected_type)) {
                return null;
            }
            set_resolved_expected_type(expr, operand.type);
            return operand.type;
        }

        Type resolve_init(SrcPos pos, Typespec typespec, Expr expr, bool was_const, bool is_undef) {
            Type type = null;
            Type inferred_type = null;
            Type declared_type = null;
            if (is_undef) {
                if (typespec != null) {
                    declared_type = type = resolve_typespec_strict(typespec, was_const);
                }
                if (type == null) {
                    fatal_error(pos, "Cannot use undef initializer without declared type");
                }
            }
            else if (typespec != null) {
                declared_type = type = resolve_typespec_strict(typespec, was_const);
                if (expr != null) {
                    inferred_type = type = resolve_typed_init(pos, declared_type, expr);
                    if (inferred_type == null) {
                        fatal_error(pos, "Invalid type in initialization. Expected %s", get_type_name(declared_type));
                    }
                }
            }
            else {
                assert(expr != null);
                inferred_type = type = unqualify_type(resolve_expr(expr).type);
                if (is_array_type(type) && expr.kind != EXPR_COMPOUND) {
                    type = type_decay(type);
                    set_resolved_type(expr, type);
                }
                else
                    set_resolved_expected_type(expr, type);

                expr.type = type;
            }
            complete_type(type);
            if (expr == null || is_ptr_type(inferred_type)) {
                type = incomplete_decay(type);
            }
            if (type.size == 0) {
                fatal_error(pos, "Cannot declare variable of size 0");
            }
            return type;
        }

        Type resolve_decl_var(Decl decl) {
            assert(decl.kind == DECL_VAR);
            return resolve_init(decl.pos, decl.var.type, decl.var.expr, is_decl_foreign(decl), false);
        }

        Type resolve_decl_const(Decl decl, ref Val val) {
            assert(decl.kind == DECL_CONST);
            Operand result = resolve_const_expr(decl.const_decl.expr);
            if (!is_scalar_type(result.type))
                fatal_error(decl.pos, "Const declarations must have scalar type");
            if (decl.const_decl.type != null) {
                Type type = resolve_typespec(decl.const_decl.type);
                if (!convert_operand(ref result, type)) {
                    fatal_error(decl.pos, "Invalid type in constant declaration. Expected {0}, got {1}", get_type_name(type), get_type_name(result.type));
                }
            }
            val = result.val;
            return result.type;
        }

        Type resolve_decl_func(Decl decl) {
            assert(decl.kind == DECL_FUNC);
            bool foreign = get_decl_note(decl, foreign_name) != null;
            bool intrinsic = get_decl_note(decl, intrinsic_name) != null;
            bool with_const = foreign;
            var @params = new List<Type>();
            for (var i = 0; i < decl.func.num_params; i++) {
                Type param = resolve_typespec_strict(decl.func.@params[i].type, with_const);
                param = incomplete_decay(param);
                complete_type(param);
                if (param == type_void && !foreign) {
                    fatal_error(decl.pos, "Function parameter type cannot be void");
                }
                @params.Add(param);
            }
            var ret_type = type_void;
            if (decl.func.ret_type != null) {
                ret_type = incomplete_decay(resolve_typespec_strict(decl.func.ret_type, with_const));
                complete_type(ret_type);
            }
            if (is_array_type(ret_type)) {
                fatal_error(decl.pos, "Function return type cannot be array");
            }
            Type varargs_type = type_void;
            if (decl.func.varargs_type != null) {
                varargs_type = incomplete_decay(resolve_typespec_strict(decl.func.varargs_type, with_const));
                complete_type(varargs_type);
                if (is_integer_type(varargs_type) && type_rank(varargs_type) < type_rank(type_int)) {
                    fatal_error(decl.pos, "Integer varargs type must have same or higher rank than int");
                }
                else if (varargs_type == type_float) {
                    fatal_error(decl.pos, "Floating varargs type must be double, not float");
                }
            }
            return type_func(@params.ToArray(), @params.Count, ret_type, intrinsic, decl.func.has_varargs, varargs_type);
        }

        bool is_cond_operand(Operand operand) {
            operand = operand_decay(operand);
            return is_scalar_type(operand.type);
        }

        void resolve_cond_expr(Expr expr) {
            var cond = resolve_expr_rvalue(expr);
            if (!is_cond_operand(cond)) {
                fatal_error(expr.pos, "Conditional expression must have arithmetic or scalar type");
            }
        }

        bool resolve_stmt_block(StmtList block, Type ret_type, StmtCtx ctx) {
            var scope = sym_enter();
            bool returns = false;
            for (var i = 0; i < block.num_stmts; i++) {
                returns = resolve_stmt(block.stmts[i], ret_type, ctx) || returns;
            }
            sym_leave(scope);
            return returns;
        }

        void resolve_stmt_assign(Stmt stmt) {
            assert(stmt.kind == STMT_ASSIGN);
            Expr left_expr = stmt.assign.left;
            Operand left = resolve_expr(left_expr);
            if (!left.is_lvalue) {
                fatal_error(stmt.pos, "Cannot assign to non-lvalue");
            }
            if (is_array_type(left.type)) {
                fatal_error(stmt.pos, "Cannot assign to array");
            }
            if (left.type.nonmodifiable) {
                fatal_error(stmt.pos, "Left-hand side of assignment has non-modifiable type");
            }
            string assign_op_name = token_kind_name(stmt.assign.op);
            TokenKind binary_op = assign_token_to_binary_token[(int)stmt.assign.op];
            Expr right_expr = stmt.assign.right;
            Operand right = resolve_expected_expr_rvalue(right_expr, left.type);
            Operand result = default;
            if (stmt.assign.op == TOKEN_ASSIGN) {
                result = right;
            }
            else if (stmt.assign.op == TOKEN_ADD_ASSIGN || stmt.assign.op == TOKEN_SUB_ASSIGN) {
                if (left.type.kind == TYPE_PTR && is_integer_type(right.type)) {
                    if (unqualify_type(left.type.@base) == type_void) {
                        set_pointer_promo_type(left_expr, type_ptr(qualify_type(type_char, left.type.@base)));
                    }
                    result = operand_rvalue(left.type);
                }
                else if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                    result = resolve_expr_binary_op(binary_op, assign_op_name, stmt.pos, left, right, left_expr, right_expr);
                }
                else {
                    fatal_error(stmt.pos, "Invalid operand types for {0}", assign_op_name);
                }
            }
            else {
                result = resolve_expr_binary_op(binary_op, assign_op_name, stmt.pos, left, right, left_expr, right_expr);
            }
            if (!convert_operand(ref result, left.type)) {
                fatal_error(stmt.pos, "Invalid type in assignment. Expected {0}, got {1}", get_type_name(left.type), get_type_name(result.type));
            }
        }

        void resolve_stmt_init(Stmt stmt) {
            assert(stmt.kind == STMT_INIT);
            Type type = resolve_init(stmt.pos, stmt.init.type, stmt.init.expr, false, stmt.init.is_undef);
            if (!sym_push_var(stmt.init.name, type))
                fatal_error(stmt.pos, "Shadowed definition of local symbol");
        }

        void resolve_static_assert(Note note) {
            if (note.num_args != 1) {
                fatal_error(note.pos, "#static_assert takes 1 argument");
            }
            Operand operand = resolve_const_expr(note.args[0].expr);
            if (operand.val.ull == 0) {
                fatal_error(note.pos, "#static_assert failed");
            }
        }

        bool resolve_stmt(Stmt stmt, Type ret_type, StmtCtx ctx) {
            switch (stmt.kind) {
                case STMT_RETURN:
                    if (stmt.expr != null) {
                        Operand operand = resolve_expected_expr_rvalue(stmt.expr, ret_type);
                        if (!convert_operand(ref operand, ret_type)) {
                            fatal_error(stmt.pos, "Invalid type in return expression. Expected {0}, got {1}", get_type_name(ret_type), get_type_name(operand.type));
                        }
                    }
                    else {
                        if (ret_type != type_void)
                            fatal_error(stmt.pos, "Empty return expression for function with non-void return type");
                    }

                    return true;
                case STMT_BREAK:
                    if (!ctx.is_break_legal) {
                        fatal_error(stmt.pos, "Break statement outside loop");
                    }
                    return false;
                case STMT_CONTINUE:
                    if (!ctx.is_continue_legal) {
                        fatal_error(stmt.pos, "Continue statement outside loop");
                    }
                    return false;
                case STMT_BLOCK:
                    return resolve_stmt_block(stmt.block, ret_type, ctx);
                case STMT_NOTE:
                    if (stmt.note.name == assert_name) {
                        if (stmt.note.num_args != 1) {
                            fatal_error(stmt.pos, "#assert takes 1 argument");
                        }
                        resolve_cond_expr(stmt.note.args[0].expr);
                    }
                    else if (stmt.note.name == foreign_name) {
                        // TODO: check args
                    }
                    else if (stmt.note.name == static_assert_name) {
                        resolve_static_assert(stmt.note);
                    }
                    else {
                        warning(stmt.pos, "Unknown statement #directive '{0}'", stmt.note.name);
                    }
                    return false;
                case STMT_IF: {
                    var scope = sym_enter();
                    if (stmt.if_stmt.init != null) {
                        resolve_stmt_init(stmt.if_stmt.init);
                    }
                    if (stmt.if_stmt.cond != null) {
                        resolve_cond_expr(stmt.if_stmt.cond);
                    }
                    else if (!is_cond_operand(resolve_name_operand(stmt.pos, stmt.if_stmt.init.init.name))) {
                        fatal_error(stmt.pos, "Conditional expression must have scalar type");
                    }
                    bool returns = resolve_stmt_block(stmt.if_stmt.then_block, ret_type, ctx);
                    for (var i = 0; i < stmt.if_stmt.num_elseifs; i++) {
                        var elseif = stmt.if_stmt.elseifs[i];
                        resolve_cond_expr(elseif.cond);
                        returns = resolve_stmt_block(elseif.block, ret_type, ctx) && returns;
                    }

                    if (stmt.if_stmt.else_block.stmts != null) {
                        returns = resolve_stmt_block(stmt.if_stmt.else_block, ret_type, ctx) && returns;
                    }
                    else
                        returns = false;

                    sym_leave(scope);
                    return returns;
                }
                case STMT_WHILE:
                case STMT_DO_WHILE:
                    resolve_cond_expr(stmt.while_stmt.cond);
                    ctx.is_break_legal = true;
                    ctx.is_continue_legal = true;
                    resolve_stmt_block(stmt.while_stmt.block, ret_type, ctx);
                    return false;
                case STMT_FOR: {
                    var sym = sym_enter();
                    if (stmt.for_stmt.init != null)
                        resolve_stmt(stmt.for_stmt.init, ret_type, ctx);
                    if (stmt.for_stmt.cond != null)
                        resolve_cond_expr(stmt.for_stmt.cond);
                    if (stmt.for_stmt.next != null)
                        resolve_stmt(stmt.for_stmt.next, ret_type, ctx);

                    ctx.is_break_legal = true;
                    ctx.is_continue_legal = true;
                    resolve_stmt_block(stmt.for_stmt.block, ret_type, ctx);
                    sym_leave(sym);
                    return false;
                }

                case STMT_SWITCH: {
                    Operand operand = resolve_expr_rvalue(stmt.switch_stmt.expr);
                    if (!is_integer_type(operand.type)) {
                        fatal_error(stmt.pos, "Switch expression must have integer type");
                    }
                    ctx.is_break_legal = true;
                    bool returns = true;
                    bool has_default = false;
                    for (var i = 0; i < stmt.switch_stmt.num_cases; i++) {
                        var switch_case = stmt.switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_patterns; j++) {
                            SwitchCasePattern pattern = switch_case.patterns[j];
                            Expr start_expr = pattern.start;
                            Operand start_operand = resolve_const_expr(start_expr);
                            if (!convert_operand(ref start_operand, operand.type)) {
                                fatal_error(start_expr.pos, "Invalid type in switch case expression. Expected {0}, got {0}", get_type_name(operand.type), get_type_name(start_operand.type));
                            }
                            Expr end_expr = pattern.end;
                            if (end_expr != null) {
                                Operand end_operand = resolve_const_expr(end_expr);
                                if (!convert_operand(ref end_operand, operand.type)) {
                                    fatal_error(end_expr.pos, "Invalid type in switch case expression. Expected {0}, got {0}", get_type_name(operand.type), get_type_name(end_operand.type));
                                }
                                convert_operand(ref start_operand, type_llong);
                                set_resolved_val(start_expr, start_operand.val);
                                convert_operand(ref end_operand, type_llong);
                                set_resolved_val(end_expr, end_operand.val);
                                if (end_operand.val.ll < start_operand.val.ll) {
                                    fatal_error(start_expr.pos, "Case range end value cannot be less thn start value");
                                }
                                if (end_operand.val.ll - start_operand.val.ll >= 256) {
                                    fatal_error(start_expr.pos, "Case range cannot span more than 256 values");
                                }
                            }
                        }
                        if (switch_case.is_default) {
                            if (has_default) {
                                fatal_error(stmt.pos, "Switch statement has multiple default clauses");
                            }
                            has_default = true;
                        }
                        if (switch_case.block.num_stmts > 1) {
                            Stmt last_stmt = switch_case.block.stmts[switch_case.block.num_stmts - 1];
                            if (last_stmt.kind == STMT_BREAK) {
                                warning(last_stmt.pos, "Case blocks already end with an implicit break");
                            }
                        }
                        returns = resolve_stmt_block(switch_case.block, ret_type, ctx) && returns;
                    }
                    return returns && has_default;
                }

                case STMT_ASSIGN:
                    resolve_stmt_assign(stmt);
                    return false;

                case STMT_INIT:
                    resolve_stmt_init(stmt);
                    return false;
                case STMT_EXPR:
                    resolve_expr(stmt.expr);
                    return false;
                case STMT_LABEL:
                    define_label(stmt.pos, stmt.label);
                    return false;
                case STMT_GOTO:
                    reference_label(stmt.pos, stmt.label);
                    return false;
                default:
                    assert(false);
                    return false;
            }
        }

        void resolve_func_body(Sym sym) {
            var decl = sym.decl;
            assert(decl.kind == DECL_FUNC);
            assert(sym.state == SYM_RESOLVED);
            if (decl.is_incomplete) {
                return;
            }

            Package old_package = enter_package(sym.home_package);
            var scope = sym_enter();
            for (var i = 0; i < decl.func.num_params; i++) {
                FuncParam param = decl.func.@params[i];
                Type param_type = resolve_typespec(param.type);
                param_type = incomplete_decay(param_type);
                if (is_array_type(param_type)) {
                    param_type = type_ptr(param_type.@base);
                }
                sym_push_var(param.name, resolve_typespec(param.type));
            }
            Type ret_type = incomplete_decay(resolve_typespec(decl.func.ret_type));
            bool returns = resolve_stmt_block(decl.func.block, ret_type, default);
            resolve_labels();
            sym_leave(scope);
            if (ret_type != type_void && !returns) {
                fatal_error(decl.pos, "Not all control paths return values");
            }
            leave_package(old_package);
        }

        void resolve_sym(Sym sym) {
            if (sym.state == SYM_RESOLVED)
                return;

            if (sym.state == SYM_RESOLVING) {
                fatal_error(sym.decl.pos, "Cyclic dependency");
                return;
            }

            assert(sym.state == SYM_UNRESOLVED);
            assert(sym.reachable == 0);

            reachable_syms.Add(sym);
            sym.reachable = reachable_phase;
            sym.state = SYM_RESOLVING;
            Decl decl = sym.decl;
            Package old_package = enter_package(sym.home_package);
            switch (sym.kind) {
                case SYM_TYPE:
                    if (decl != null && decl.kind == DECL_TYPEDEF) {
                        sym.type = resolve_typespec_strict(decl.typedef_decl.type, is_decl_foreign(decl));
                    }
                    else if (decl.kind == DECL_ENUM) {
                        Type  @base = decl.enum_decl.type != null ? resolve_typespec(decl.enum_decl.type) : type_int;
                        if (!is_integer_type(@base)) {
                            fatal_error(decl.pos, "Base type of enum must be integer type");
                        }
                        sym.type = type_enum(sym, @base);
                    }
                    else {
                        sym.type = type_incomplete(sym);
                    }
                    break;
                case SYM_VAR:
                    sym.type = resolve_decl_var(decl);
                    break;
                case SYM_CONST:
                    sym.type = resolve_decl_const(decl, ref sym.val);
                    break;
                case SYM_FUNC:
                    sym.type = resolve_decl_func(decl);
                    break;
                case SYM_PACKAGE:
                    // Do nothing
                    break;
                default:
                    assert(false);
                    break;
            }

            leave_package(old_package);
            sym.state = SYM_RESOLVED;
            if (decl.is_incomplete || (decl.kind != DECL_STRUCT && decl.kind != DECL_UNION)) {
                sorted_syms.Add(sym);
            }
        }

        void finalize_sym(Sym sym) {
            assert(sym.state == SYM_RESOLVED);
            if (sym.decl != null && !sym.decl.is_incomplete) {
                if (sym.kind == SYM_TYPE) {
                    complete_type(sym.type);
                }
                else if (sym.kind == SYM_FUNC) {
                    resolve_func_body(sym);
                }
            }
        }

        Sym resolve_name(string name) {
            var sym = sym_get(name);
            if (sym == null) {
                return null;
            }

            resolve_sym(sym);
            return sym;
        }

        Package try_resolve_package(Expr expr) {
            if (expr.kind == EXPR_NAME) {
                Sym sym = resolve_name(expr.name);
                if (sym != null && sym.kind == SYM_PACKAGE) {
                    return sym.package;
                }
            }
            else if (expr.kind == EXPR_FIELD) {
                Package package = try_resolve_package(expr.field.expr);
                if (package != null) {
                    Sym sym = get_package_sym(package, expr.field.name);
                    if (sym != null && sym.kind == SYM_PACKAGE) {
                        return sym.package;
                    }
                }
            }
            return null;
        }

        Operand resolve_expr_field(Expr expr) {
            assert(expr.kind == EXPR_FIELD);
            Package package = try_resolve_package(expr.field.expr);
            if (package != null) {
                Package old_package = enter_package(package);
                Sym sym = resolve_name(expr.field.name);
                Operand o = resolve_name_operand(expr.pos, expr.field.name);
                leave_package(old_package);
                set_resolved_sym(expr, sym);
                return o;
            }
            Operand operand = resolve_expr(expr.field.expr);
            bool was_const_type = is_const_type(operand.type);
            Type type = unqualify_type(operand.type);
            complete_type(type);
            if (is_ptr_type(type)) {
                operand = operand_lvalue(type.@base);
                was_const_type = is_const_type(operand.type);
                type = unqualify_type(operand.type);
                complete_type(type);
            }
            if (!is_aggregate_type(type)) {
                fatal_error(expr.pos, "Can only access fields on aggregates or pointers to aggregates");
                return default;
            }

            for (var i = 0; i < type.aggregate.num_fields; i++) {
                var field = type.aggregate.fields[i];
                if (field.name == expr.field.name) {
                    Operand field_operand = operand.is_lvalue ? operand_lvalue(field.type) : operand_rvalue(field.type);
                    if (was_const_type) {
                        field_operand.type = type_const(field_operand.type);
                    }
                    return field_operand;
                }
            }

            fatal_error(expr.pos, "No field named '{0}'", expr.field.name);
            return default;
        }

        long eval_unary_op_ll(TokenKind op, long val) {
            switch (op) {
                case TOKEN_ADD:
                    return +val;
                case TOKEN_SUB:
                    return -val;
                case TOKEN_NEG:
                    return ~val;
                case TOKEN_NOT:
                    return val == 0 ? 1 : 0;
                default:
                    assert(false);
                    break;
            }
            return 0;
        }

        ulong eval_unary_op_ull(TokenKind op, ulong val) {
            switch (op) {
                case TOKEN_ADD:
                    return +val;
                case TOKEN_SUB:
                    return 0ul - val;
                case TOKEN_NEG:
                    return ~val;
                case TOKEN_NOT:
                    return val == 0 ? 1ul : 0;
                default:
                    assert(false);
                    break;
            }
            return 0;
        }

        Val eval_unary_op(TokenKind op, Type type, Val val) {
            if (is_integer_type(type)) {
                Operand operand = operand_const(type, val);
                if (is_signed_type(type)) {
                    cast_operand(ref operand, type_llong);
                    operand.val.ll = eval_unary_op_ll(op, operand.val.ll);
                }
                else {
                    cast_operand(ref operand, type_ullong);
                    operand.val.ull = eval_unary_op_ull(op, operand.val.ull);
                }
                cast_operand(ref operand, type);
                return operand.val;
            }
            return default;
        }

        long eval_binary_op_ll(TokenKind op, long left, long right) {
            switch (op) {
                case TOKEN_MUL:
                    return left * right;
                case TOKEN_DIV:
                    return right != 0 ? left / right : 0;
                case TOKEN_MOD:
                    return right != 0 ? left % right : 0;
                case TOKEN_AND:
                    return left & right;
                case TOKEN_LSHIFT:
                    return left << (int)right;
                case TOKEN_RSHIFT:
                    return left >> (int)right;
                case TOKEN_ADD:
                    return left + right;
                case TOKEN_SUB:
                    return left - right;
                case TOKEN_OR:
                    return left | right;
                case TOKEN_XOR:
                    return left ^ right;
                case TOKEN_EQ:
                    return left == right ? 1 : 0;
                case TOKEN_NOTEQ:
                    return left != right ? 1 : 0;
                case TOKEN_LT:
                    return left < right ? 1 : 0;
                case TOKEN_LTEQ:
                    return left <= right ? 1 : 0;
                case TOKEN_GT:
                    return left > right ? 1 : 0;
                case TOKEN_GTEQ:
                    return left >= right ? 1 : 0;
                default:
                    assert(false);
                    break;
            }
            return 0;
        }

        ulong eval_binary_op_ull(TokenKind op, ulong left, ulong right) {
            switch (op) {
                case TOKEN_MUL:
                    return left * right;
                case TOKEN_DIV:
                    return right != 0 ? left / right : 0;
                case TOKEN_MOD:
                    return right != 0 ? left % right : 0;
                case TOKEN_AND:
                    return left & right;
                case TOKEN_LSHIFT:
                    return left << (int)right;
                case TOKEN_RSHIFT:
                    return left >> (int)right;
                case TOKEN_ADD:
                    return left + right;
                case TOKEN_SUB:
                    return left - right;
                case TOKEN_OR:
                    return left | right;
                case TOKEN_XOR:
                    return left ^ right;
                case TOKEN_EQ:
                    return left == right ? 1ul : 0;
                case TOKEN_NOTEQ:
                    return left != right ? 1ul : 0;
                case TOKEN_LT:
                    return left < right ? 1ul : 0;
                case TOKEN_LTEQ:
                    return left <= right ? 1ul : 0;
                case TOKEN_GT:
                    return left > right ? 1ul : 0;
                case TOKEN_GTEQ:
                    return left >= right ? 1ul : 0;
                default:
                    assert(false);
                    break;
            }
            return 0;
        }

        Val eval_binary_op(TokenKind op, Type type, Val left, Val right) {
            if (is_integer_type(type)) {
                Operand left_operand = operand_const(type, left);
                Operand right_operand = operand_const(type, right);
                Operand result_operand;
                if (is_signed_type(type)) {
                    cast_operand(ref left_operand, type_llong);
                    cast_operand(ref right_operand, type_llong);
                    result_operand = operand_const(type_llong, new Val { ll = eval_binary_op_ll(op, left_operand.val.ll, right_operand.val.ll) });
                }
                else {
                    cast_operand(ref left_operand, type_ullong);
                    cast_operand(ref right_operand, type_ullong);
                    result_operand = operand_const(type_ullong, new Val { ull = eval_binary_op_ull(op, left_operand.val.ull, right_operand.val.ull) });
                }
                cast_operand(ref result_operand, type);
                return result_operand.val;
            }

            return default;
        }

        Operand resolve_name_operand(SrcPos pos, string name) {
            Sym sym = resolve_name(name);
            if (sym == null) {
                fatal_error(pos, "Unresolved name '{0}'", name);
            }

            if (sym.kind == SYM_VAR) {
                Operand operand = operand_lvalue(sym.type);
                if (is_array_type(operand.type) && !is_incomplete_array_type(operand.type)) {
                    operand = operand_decay(operand);
                }
                return operand;
            }

            if (sym.kind == SYM_CONST)
                return operand_const(sym.type, sym.val);

            if (sym.kind == SYM_FUNC)
                return operand_rvalue(sym.type);

            fatal_error(pos, "{0} must denote a var func or const", name);
            return default;
        }

        Operand resolve_expr_name(Expr expr) {
            assert(expr.kind == EXPR_NAME);
            return resolve_name_operand(expr.pos, expr.name);
        }


        Operand resolve_unary_op(TokenKind op, Operand operand) {
            promote_operand(ref operand);
            if (operand.is_const) {
                return operand_const(operand.type, eval_unary_op(op, operand.type, operand.val));
            }
            else {
                return operand;
            }
        }

        Operand resolve_expr_unary(Expr expr) {
            Operand operand = resolve_expr_rvalue(expr.unary.expr);
            Type type = operand.type;
            switch (expr.unary.op) {
                case TOKEN_MUL:
                    if (!is_ptr_type(type)) {
                        fatal_error(expr.pos, "Cannot deref non-ptr type");
                    }
                    return operand_lvalue(type.@base);
                case TOKEN_ADD:
                case TOKEN_SUB:
                    if (!is_arithmetic_type(type)) {
                        fatal_error(expr.pos, "Can only use unary {0} with arithmetic types", token_kind_name(expr.unary.op));
                    }
                    return resolve_unary_op(expr.unary.op, operand);
                case TOKEN_NEG:
                    if (!is_integer_type(type)) {
                        fatal_error(expr.pos, "Can only use ~ with integer types");
                    }
                    return resolve_unary_op(expr.unary.op, operand);
                case TOKEN_NOT:
                    if (!is_scalar_type(type)) {
                        fatal_error(expr.pos, " Can only use ! with scalar types");
                    }
                    return resolve_unary_op(expr.unary.op, operand);
            }
            return default;
        }

        Operand resolve_binary_op(TokenKind op, Operand left, Operand right) {
            if (left.is_const && right.is_const) {
                return operand_const(left.type, eval_binary_op(op, left.type, left.val, right.val));
            }
            else {
                return operand_rvalue(left.type);
            }
        }

        Operand resolve_binary_arithmetic_op(TokenKind op, Operand left, Operand right) {
            unify_arithmetic_operands(ref left, ref right);
            return resolve_binary_op(op, left, right);
        }

        bool compatible_pointer_arith(Type left, Type right, Expr left_expr, Expr right_expr) {
            if (is_ptr_type(left) && is_ptr_type(right)) {
                Type left_base = unqualify_type(left.@base);
                Type right_base = unqualify_type(right.@base);
                if (left_base == right_base) {
                    return true;
                }
                if (left_base == type_void && right_base == type_char) {
                    set_pointer_promo_type(left_expr, type_ptr(qualify_type(type_char, left.@base)));
                    return true;
                }
                if (left_base == type_char && right_base == type_void) {
                    set_pointer_promo_type(right_expr, type_ptr(qualify_type(type_char, right.@base)));
                    return true;
                }
            }
            return false;
        }

        Operand resolve_expr_binary_op(TokenKind op, string op_name, SrcPos pos, Operand left, Operand right, Expr left_expr, Expr right_expr) {
            switch (op) {
                case TOKEN_MUL:
                case TOKEN_DIV:
                    if (!is_arithmetic_type(left.type)) {
                        fatal_error(pos, "Left operand of {0} must have arithmetic type", op_name);
                    }
                    if (!is_arithmetic_type(right.type)) {
                        fatal_error(pos, "Right operand of {0} must have arithmetic type", op_name);
                    }
                    return resolve_binary_arithmetic_op(op, left, right);
                case TOKEN_MOD:
                    if (!is_integer_type(left.type)) {
                        fatal_error(pos, "Left operand of % must have integer type");
                    }
                    if (!is_integer_type(right.type)) {
                        fatal_error(pos, "Right operand of % must have integer type");
                    }
                    return resolve_binary_arithmetic_op(op, left, right);
                case TOKEN_ADD:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else if (is_ptr_type(left.type) && is_integer_type(right.type)) {
                        complete_type(left.type.@base);
                        if (unqualify_type(left.type.@base) == type_void) {
                            Type promo_type = type_ptr(qualify_type(type_char, left.type.@base));
                            set_pointer_promo_type(left_expr, promo_type);
                            left.type = promo_type;
                        }
                        else if (type_sizeof(left.type.@base) == 0) {
                            fatal_error(pos, "Cannot do pointer arithmetic with size 0 base type");
                        }
                        return operand_rvalue(left.type);
                    }
                    else if (is_ptr_type(right.type) && is_integer_type(left.type)) {
                        complete_type(right.type.@base);
                        if (unqualify_type(right.type.@base) == type_void) {
                            Type promo_type = type_ptr(qualify_type(type_char, right.type.@base));
                            set_pointer_promo_type(right_expr, promo_type);
                            right.type = promo_type;
                        }
                        else if (type_sizeof(right.type.@base) == 0) {
                            fatal_error(pos, "Cannot do pointer arithmetic with size 0 base type");
                        }
                        return operand_rvalue(right.type);
                    }
                    else {
                        fatal_error(pos, "Operands of + must both have arithmetic type, or pointer and integer type");
                    }
                    break;
                case TOKEN_SUB:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else if (is_ptr_type(left.type) && is_integer_type(right.type)) {
                        Type left_base = unqualify_type(left.type.@base);
                        if (left_base == type_void) {
                            Type promo_type = type_ptr(qualify_type(type_char, left_base));
                            set_pointer_promo_type(left_expr, promo_type);
                            left.type = promo_type;
                        }
                        complete_type(left.type.@base);
                        if (type_sizeof(left.type.@base) == 0) {
                            fatal_error(pos, "Cannot do pointer arithmetic with size 0 base type");
                        }
                        return operand_rvalue(left.type);
                    }
                    else if (is_ptr_type(left.type) && is_ptr_type(right.type)) {
                        if (!compatible_pointer_arith(left.type, right.type, left_expr, right_expr)) {
                            fatal_error(pos, "Cannot subtract pointers to different types");
                        }
                        Type left_base = left.type.@base;
                        Type right_base = right.type.@base;
                        if (unqualify_type(left_base) == type_void && unqualify_type(right_base) == type_void) {
                            set_pointer_promo_type(left_expr, type_ptr(type_char));
                            set_pointer_promo_type(right_expr, type_ptr(type_char));
                        }
                        return operand_rvalue(type_ssize);
                    }
                    else {
                        fatal_error(pos, "Operands of - must both have arithmetic type, pointer and integer type, or compatible pointer types");
                    }
                    break;
                case TOKEN_LSHIFT:
                case TOKEN_RSHIFT:
                    if (is_integer_type(left.type) && is_integer_type(right.type)) {
                        promote_operand(ref left);
                        promote_operand(ref right);
                        Type result_type = left.type;
                        Operand result;
                        if (is_signed_type(left.type)) {
                            cast_operand(ref left, type_llong);
                            cast_operand(ref right, type_llong);
                        }
                        else {
                            cast_operand(ref left, type_ullong);
                            cast_operand(ref right, type_ullong);
                        }
                        result = resolve_binary_op(op, left, right);
                        cast_operand(ref result, result_type);
                        return result;
                    }
                    else {
                        fatal_error(pos, "Operands of {0} must both have integer type", op_name);
                    }
                    break;
                case TOKEN_EQ:
                case TOKEN_NOTEQ:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        Operand result = resolve_binary_arithmetic_op(op, left, right);
                        cast_operand(ref result, type_int);
                        return result;
                    }
                    else if (is_ptr_type(left.type) && is_ptr_type(right.type)) {
                        Type unqual_left_base = unqualify_type(left.type.@base);
                        Type unqual_right_base = unqualify_type(right.type.@base);
                        if (unqual_left_base != unqual_right_base && unqual_left_base != type_void && unqual_right_base != type_void) {
                            fatal_error(pos, "Cannot compare pointers to different types");
                        }
                        return operand_rvalue(type_int);
                    }
                    else if ((is_null_ptr(ref left) && is_ptr_type(right.type)) || (is_null_ptr(ref right) && is_ptr_type(left.type))) {
                        return operand_rvalue(type_int);
                    }
                    else {
                        fatal_error(pos, "Operands of {0} must be arithmetic types or compatible pointer types", op_name);
                    }
                    break;
                case TOKEN_LT:
                case TOKEN_LTEQ:
                case TOKEN_GT:
                case TOKEN_GTEQ:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        Operand result = resolve_binary_arithmetic_op(op, left, right);
                        cast_operand(ref result, type_int);
                        return result;
                    }
                    else if (is_ptr_type(left.type) && is_ptr_type(right.type)) {
                        if (unqualify_type(left.type.@base) != unqualify_type(right.type.@base)) {
                            Type left_base = unqualify_type(left.type.@base);
                            Type right_base = unqualify_type(right.type.@base);
                            if (left_base != right_base) {
                                set_pointer_promo_type(right_expr, type_ptr(qualify_type(type_char, left.type.@base)));
                                set_pointer_promo_type(left_expr, type_ptr(qualify_type(type_char, left.type.@base)));
                            }
                        }
                        return operand_rvalue(type_int);
                    }
                    else if ((is_null_ptr(ref left) && is_ptr_type(right.type)) || (is_null_ptr(ref right) && is_ptr_type(left.type))) {
                        return operand_rvalue(type_int);
                    }
                    else {
                        fatal_error(pos, "Operands of {0} must be arithmetic types or compatible pointer types", op_name);
                    }
                    break;
                case TOKEN_AND:
                case TOKEN_XOR:
                case TOKEN_OR:
                    if (is_integer_type(left.type) && is_integer_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else {
                        fatal_error(pos, "Operands of {0} must have arithmetic types", op_name);
                    }
                    break;
                case TOKEN_AND_AND:
                case TOKEN_OR_OR:
                    if (is_scalar_type(left.type) && is_scalar_type(right.type)) {
                        if (left.is_const && right.is_const) {
                            cast_operand(ref left, type_bool);
                            cast_operand(ref right, type_bool);
                            bool b;
                            if (op == TOKEN_AND_AND) {
                                b = left.val.b && right.val.b;
                            }
                            else {
                                assert(op == TOKEN_OR_OR);
                                b = left.val.b || right.val.b;
                            }
                            return operand_const(type_int, new Val { b = b });
                        }
                        else {
                            return operand_rvalue(type_int);
                        }
                    }
                    else {
                        fatal_error(pos, "Operands of {0} must have scalar types", op_name);
                    }
                    break;
                default:
                    assert(0);
                    break;
            }

            return default;
        }

        Operand resolve_expr_binary(Expr expr) {
            assert(expr.kind == EXPR_BINARY);
            Operand left = resolve_expr_rvalue(expr.binary.left);
            Operand right = resolve_expr_rvalue(expr.binary.right);
            TokenKind op = expr.binary.op;
            var op_name = token_kind_name(op);
            return resolve_expr_binary_op(op, op_name, expr.pos, left, right, expr.binary.left, expr.binary.right);
        }

        Operand resolve_expr_compound(Expr expr, Type expected_type) {
            assert(expr.kind == EXPR_COMPOUND);
            if (expected_type == null && expr.compound.type == null)
                fatal_error(expr.pos, "Implicitly typed compound literals used in context without expected type");
            Type type = null;
            if (expr.compound.type != null)
                type = resolve_typespec(expr.compound.type);
            else
                type = expected_type;
            complete_type(type);
            bool is_const = is_const_type(type);
            type = unqualify_type(type);

            if (type.kind == TYPE_STRUCT || type.kind == TYPE_UNION || type.kind == TYPE_TUPLE) {
                var index = 0;
                for (var i = 0; i < expr.compound.num_fields; i++) {
                    var field = expr.compound.fields[i];
                    if (field.kind == FIELD_INDEX)
                        fatal_error(field.pos, "Index field initializer not allowed for struct/union compound literal");
                    else if (field.kind == FIELD_NAME) {
                        index = aggregate_item_field_index(type, field.name);
                        if (index == -1) {
                            fatal_error(field.pos, "Named field in compound literal does not exist");
                        }
                    }
                    if (index >= type.aggregate.num_fields)
                        fatal_error(field.pos, "Field initializer in struct/union compound literal out of range");
                    Type field_type = type.aggregate.fields[index].type;
                    if (resolve_typed_init(field.pos, field_type, field.init) == null) {
                        fatal_error(field.pos, "Invalid type in compound literal initializer for aggregate type. Expected {0}.", get_type_name(field_type));
                    }
                    index++;
                }
            }
            else if (type.kind == TYPE_ARRAY || type.kind == TYPE_PTR) {
                int index = 0, max_index = 0;
                for (var i = 0; i < expr.compound.num_fields; i++) {
                    var field = expr.compound.fields[i];
                    if (field.kind == FIELD_NAME) {
                        fatal_error(field.pos, "Named field initializer not allowed for array compound literals");
                    }
                    else if (field.kind == FIELD_INDEX) {
                        Operand operand = resolve_const_expr(field.index);
                        if (!is_integer_type(operand.type)) {
                            fatal_error(field.pos, "Field initializer index expression must have type int");
                        }
                        if (!cast_operand(ref operand, type_int)) {
                            fatal_error(field.pos, "Invalid type in field initializer index. Expected integer type");
                        }
                        if (operand.val.i < 0) {
                            fatal_error(field.pos, "Field initializer index cannot be negative");
                        }
                        index = operand.val.i;
                    }

                    if (type.num_elems != 0 && index >= type.num_elems)
                        fatal_error(field.pos, "Field initializer in array compound literal out of range");
                    if (resolve_typed_init(field.pos, type.@base, field.init) == null) {
                        fatal_error(field.pos, "Invalid type in compound literal initializer for array type. Expected {0}", get_type_name(type.@base));
                    }
                    max_index = (int)MAX(max_index, index);
                    index++;
                }
                if (type.incomplete_elems) {
                    type = type_array(type.@base, max_index + 1, false);
                }
            }
            else {
                if (type == type_void) {
                    fatal_error(expr.pos, "Anonymous compound literal in context expecting void type");
                }
                assert(is_scalar_type(type));
                if (expr.compound.num_fields > 1) {
                    fatal_error(expr.pos, "Compound literal for scalar type cannot have more than one operand");
                }
                else if (expr.compound.num_fields == 1) {
                    CompoundField field = expr.compound.fields[0];
                    Operand init = resolve_expected_expr_rvalue(field.init, type);
                    if (!convert_operand(ref init, type)) {
                        fatal_error(field.pos, "Invalid type in compound literal initializer. Expected {0}", get_type_name(type.@base));
                    }
                }
            }

            return operand_lvalue(is_const ? type_const(type) : type);
        }

        Operand resolve_expr_call_intrinsic(Operand func, Expr expr, Type expected_type) {
            Sym sym = get_resolved_sym(expr.call.expr);
            assert(sym != null);
            if (sym.name == "va_arg") {
                Operand args = resolve_expr(expr.call.args[0]);
                if (!args.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of va_arg must be lvalue");
                }
                Operand arg = resolve_expr(expr.call.args[1]);
                if (!arg.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 2 of va_arg must be lvalue");
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "aput") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                Type base_type = unqualify_type(array.type.@base);
                if (base_type == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!is_aggregate_type(base_type) && base_type.aggregate.num_fields != 2) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have aggregate base type with 2 fields", sym.name);
                }
                Type base_key_type = base_type.aggregate.fields[0].type;
                if (type_padding(base_key_type) != 0) {
                    fatal_error(expr.call.args[1].pos, "Key type of {0} must contain no padding", sym.name);
                }
                Operand key = resolve_expected_expr_rvalue(expr.call.args[1], base_key_type);
                if (!convert_operand(ref key, base_key_type)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1's key type", sym.name);
                }
                Type base_value_type = base_type.aggregate.fields[1].type;
                Operand value = resolve_expected_expr_rvalue(expr.call.args[2], base_value_type);
                if (!is_convertible(ref value, base_value_type)) {
                    fatal_error(expr.call.args[2].pos, "Argument 3 of {0} not convertible to argument 1's value type", sym.name);
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "ageti" || sym.name == "adel") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                Type base_type = unqualify_type(array.type.@base);
                if (base_type == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!is_aggregate_type(base_type) && base_type.aggregate.num_fields != 2) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have aggregate base type with 2 fields", sym.name);
                }
                Type base_key_type = base_type.aggregate.fields[0].type;
                if (type_padding(base_key_type) != 0) {
                    fatal_error(expr.call.args[1].pos, "Key type of {0} must contain no padding", sym.name);
                }
                Operand key = resolve_expected_expr_rvalue(expr.call.args[1], base_key_type);
                if (!convert_operand(ref key, base_key_type)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1's key type", sym.name);
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "agetp") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                Type base_type = unqualify_type(array.type.@base);
                if (base_type == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!is_aggregate_type(base_type) && base_type.aggregate.num_fields != 2) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have aggregate base type with 2 fields", sym.name);
                }
                Type base_key_type = base_type.aggregate.fields[0].type;
                if (type_padding(base_key_type) != 0) {
                    fatal_error(expr.call.args[1].pos, "Key type of {0} must contain no padding", sym.name);
                }
                Operand key = resolve_expected_expr_rvalue(expr.call.args[1], base_key_type);
                if (!convert_operand(ref key, base_key_type)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1's key type", sym.name);
                }
                return operand_rvalue(type_ptr(array.type.@base.aggregate.fields[1].type));
            }
            else if (sym.name == "aget") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                Type base_type = unqualify_type(array.type.@base);
                if (base_type == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!is_aggregate_type(base_type) && base_type.aggregate.num_fields != 2) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have aggregate base type with 2 fields", sym.name);
                }
                Type base_key_type = base_type.aggregate.fields[0].type;
                if (type_padding(base_key_type) != 0) {
                    fatal_error(expr.call.args[1].pos, "Key type of {0} must contain no padding", sym.name);
                }
                Operand key = resolve_expected_expr_rvalue(expr.call.args[1], base_key_type);
                if (!convert_operand(ref key, base_key_type)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1's key type", sym.name);
                }
                return operand_rvalue(array.type.@base.aggregate.fields[1].type);
            }
            else if (sym.name == "apush" || sym.name == "aputv" || sym.name == "agetvi" || sym.name == "adelv") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                if (sym.name != "apush" && type_padding(array.type.@base) != 0) {
                    fatal_error(expr.call.args[1].pos, "Base type of {0} must contain no padding", sym.name);
                }
                Operand elem = resolve_expected_expr_rvalue(expr.call.args[1], array.type.@base);
                if (!convert_operand(ref elem, array.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1 base type", sym.name);
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "agetvp") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                if (type_padding(array.type.@base) != 0) {
                    fatal_error(expr.call.args[1].pos, "Base type of {0} must contain no padding", sym.name);
                }
                Operand elem = resolve_expected_expr_rvalue(expr.call.args[1], array.type.@base);
                if (!convert_operand(ref elem, array.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1 base type", sym.name);
                }
                return operand_rvalue(array.type);
            }
            else if (sym.name == "agetv") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                if (type_padding(array.type.@base) != 0) {
                    fatal_error(expr.call.args[1].pos, "Base type of {0} must contain no padding", sym.name);
                }
                Operand elem = resolve_expected_expr_rvalue(expr.call.args[1], array.type.@base);
                if (!convert_operand(ref elem, array.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1 base type", sym.name);
                }
                return operand_rvalue(array.type.@base);
            }
            else if (sym.name == "adefault") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                Type base_type = unqualify_type(array.type.@base);
                if (base_type == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!is_aggregate_type(base_type) && base_type.aggregate.num_fields != 2) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have aggregate base type with 2 fields", sym.name);
                }
                Type base_val_type = base_type.aggregate.fields[1].type;
                Operand key = resolve_expected_expr_rvalue(expr.call.args[1], base_val_type);
                if (!convert_operand(ref key, base_val_type)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1's value type", sym.name);
                }
                return operand_rvalue(type_void);
            }
            else if (sym.name == "afill") {
                Operand array = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(array.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(array.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!array.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must be lvalue", sym.name);
                }
                Operand elem = resolve_expected_expr_rvalue(expr.call.args[1], array.type.@base);
                if (!convert_operand(ref elem, array.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 2 of {0} not convertible to argument 1 base type", sym.name);
                }
                Operand Count = resolve_expected_expr_rvalue(expr.call.args[2], type_usize);
                if (!convert_operand(ref Count, type_usize)) {
                    fatal_error(expr.call.args[2].pos, "Argument 3 of {0} not convertible to usize", sym.name);
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "acat") {
                assert(expr.call.num_args == 2);
                Operand dest = resolve_expr(expr.call.args[0]);
                if (!is_ptr_type(dest.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(dest.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of {0} must have non-void base type", sym.name);
                }
                if (!dest.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of acat must be lvalue");
                }
                Operand src = resolve_expr_rvalue(expr.call.args[1]);
                if (!is_ptr_type(src.type)) {
                    fatal_error(expr.call.args[0].pos, "Argument 2 of {0} must have pointer type", sym.name);
                }
                if (unqualify_type(src.type.@base) == type_void) {
                    fatal_error(expr.call.args[0].pos, "Argument 2 of {0} must have non-void base type", sym.name);
                }
                if (dest.type.@base != unqualify_type(src.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 1 and 2 of acat don't have identical base types");
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "acatn") {
                assert(expr.call.num_args == 3);
                Operand dest = resolve_expr(expr.call.args[0]);
                Operand src = resolve_expr_rvalue(expr.call.args[1]);
                if (!dest.is_lvalue) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of acat must be lvalue");
                }
                if (dest.type.@base != unqualify_type(src.type.@base)) {
                    fatal_error(expr.call.args[1].pos, "Argument 1 and 2 of acatn don't have identical base types");
                }
                Operand len = resolve_expr_rvalue(expr.call.args[2]);
                if (!convert_operand(ref len, type_usize)) {
                    fatal_error(expr.call.args[2].pos, "Argument 3 of acatn not convertible to usize");
                }
                return operand_rvalue(func.type.func.ret);
            }
            else if (sym.name == "anew") {
                assert(expr.call.num_args == 1);
                Operand allocator = resolve_expr_rvalue(expr.call.args[0]);
                if (!convert_operand(ref allocator, type_allocator_ptr)) {
                    fatal_error(expr.call.args[0].pos, "Argument 1 of %s must have type Allocator", sym.name);
                }
                if (expected_type == null || (!is_ptr_type(expected_type) && !is_array_type(expected_type))) {
                    fatal_error(expr.pos, "anew can only be used when its inferred type is array or pointer type");
                }
                complete_type(expected_type.@base);
                if (type_sizeof(expected_type.@base) == 0) {
                    fatal_error(expr.pos, "anew base type cannot be incomplete or have size 0");
                }
                return operand_rvalue(type_ptr(expected_type.@base));
            }
            else {
                return resolve_expr_call_default(func, expr);
            }
        }

        Operand resolve_expr_call_default(Operand func, Expr expr) {
            var num_params = func.type.func.num_params;
            for (var i = 0; i < expr.call.num_args; i++) {
                Type param_type = i < num_params ? func.type.func.@params[i] : func.type.func.varargs_type;
                Operand arg = resolve_expected_expr_rvalue(expr.call.args[i], param_type);
                if (is_array_type(param_type)) {
                    param_type = type_ptr(param_type.@base);
                }
                if (!convert_operand(ref arg, param_type)) {
                    fatal_error(expr.call.args[i].pos, "Invalid type in function call argument. Expected {0}, got {1}", get_type_name(param_type), get_type_name(arg.type));
                }
            }
            return operand_rvalue(func.type.func.ret);
        }

        Operand resolve_expr_call(Expr expr, Type expected_type) {
            assert(expr.kind == EXPR_CALL);
            if (expr.call.expr.kind == EXPR_NAME) {
                Sym sym = resolve_name(expr.call.expr.name);

                if (sym != null && sym.kind == SYM_TYPE) {
                    if (expr.call.num_args != 1) {
                        fatal_error(expr.pos, "Type conversion operator takes 1 argument");
                    }
                    Operand operand = resolve_expr_rvalue(expr.call.args[0]);
                    if (!cast_operand(ref operand, sym.type)) {
                        fatal_error(expr.pos, "Invalid type cast from {0} to {1}", get_type_name(operand.type), get_type_name(sym.type));
                    }
                    set_resolved_sym(expr.call.expr, sym);
                    return operand;
                }
            }
            var func = resolve_expr_rvalue(expr.call.expr);
            if (func.type.kind != TYPE_FUNC)
                fatal_error(expr.pos, "Cannot call non-function value");
            var num_params = func.type.func.num_params;

            if (expr.call.num_args < num_params) {
                fatal_error(expr.pos, "Function call with too few arguments");
            }
            if (expr.call.num_args > num_params && !func.type.func.has_varargs) {
                fatal_error(expr.pos, "Function call with too many arguments");
            }
            if (func.type.func.intrinsic) {
                return resolve_expr_call_intrinsic(func, expr, expected_type);
            }
            else {
                return resolve_expr_call_default(func, expr);
            }
        }

        Operand resolve_expr_ternary(Expr expr, Type expected_type) {
            assert(expr.kind == EXPR_TERNARY);
            var cond = resolve_expr_rvalue(expr.ternary.cond);
            if (!is_scalar_type(cond.type)) {
                fatal_error(expr.pos, "Ternary conditional must have scalar type");
            }
            Operand left = resolve_expected_expr_rvalue(expr.ternary.then_expr, expected_type);
            Operand right = resolve_expected_expr_rvalue(expr.ternary.else_expr, expected_type);
            if (left.type == right.type) {
                return operand_rvalue(left.type);
            }
            else if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                unify_arithmetic_operands(ref left, ref right);
                if (cond.is_const && left.is_const && right.is_const) {
                    return operand_const(left.type, cond.val.i != 0 ? left.val : right.val);
                }
                else {
                    return operand_rvalue(left.type);
                }
            }
            else if (is_ptr_type(left.type) && is_null_ptr(ref right)) {
                return operand_rvalue(left.type);
            }
            else if (is_ptr_type(right.type) && is_null_ptr(ref left)) {
                return operand_rvalue(right.type);
            }
            else {
                if (is_ptr_type(left.type) && is_ptr_type(right.type)) {
                    if (left.type.@base == type_void && right.type.@base == type_char) {
                        return operand_rvalue(right.type);
                    }
                    else if (left.type.@base == type_char && right.type.@base == type_void) {
                        return operand_rvalue(left.type);
                    }
                }
            }
            fatal_error(expr.pos, "Left and right operands of ternary expression must have arithmetic types or identical types");
            return default;
        }

        Operand resolve_expr_index(Expr expr) {
            assert(expr.kind == EXPR_INDEX);
            var index = resolve_expr(expr.index.index);
            if (!is_integer_type(index.type))
                fatal_error(expr.pos, "Index expression must have integer type");
            Operand operand = resolve_expr(expr.index.expr);
            if (is_aggregate_type(operand.type)) {
                if (!index.is_const) {
                    fatal_error(expr.pos, "Aggregate field index must be an integer constant");
                }
                convert_operand(ref index, type_llong);
                set_resolved_val(expr.index.index, index.val);
                long i = index.val.u;
                if (!(0 <= i && i < (long)operand.type.aggregate.num_fields)) {
                    fatal_error(expr.pos, "Aggregate field index out of range");
                }
                operand.type = operand.type.aggregate.fields[i].type;
                return operand;
            }
            operand = operand_decay(operand);
            if (!is_ptr_type(operand.type)) {
                fatal_error(expr.pos, "Can only index aggregates, arrays and pointers");
            }
            return operand_lvalue(operand.type.@base);
        }

        Operand resolve_expr_cast(Expr expr) {
            assert(expr.kind == EXPR_CAST);
            var type = resolve_typespec(expr.cast.type);
            var operand = resolve_expected_expr_rvalue(expr.cast.expr, type);
            if (!cast_operand(ref operand, type)) {
                fatal_error(expr.pos, "Invalid type cast from {0} to {0}", get_type_name(operand.type), get_type_name(type));
            }
            return operand;
        }

        Operand resolve_expr_int(Expr expr) {
            assert(expr.kind == EXPR_INT);
            ulong int_max = type_metrics[(int)TYPE_INT].max;
            ulong uint_max = type_metrics[(int)TYPE_UINT].max;
            ulong long_max = type_metrics[(int)TYPE_LONG].max;
            ulong ulong_max = type_metrics[(int)TYPE_ULONG].max;
            ulong llong_max = type_metrics[(int)TYPE_LLONG].max;
            ulong val = expr.int_lit.val;
            Operand operand = operand_const(type_ullong, new Val{ull = val});
            Type type = type_ullong;
            if (expr.int_lit.mod == TokenMod.MOD_NONE) {
                bool overflow = false;
                switch (expr.int_lit.suffix) {
                    case SUFFIX_NONE:
                        type = type_int;
                        // TODO: MAX constants should be sourced from the backend target table, not from the host compiler's header files.
                        if (val > int_max) {
                            type = type_long;
                            if (val > long_max) {
                                type = type_llong;
                                overflow = val > llong_max;
                            }
                        }
                        break;
                    case SUFFIX_U:
                        type = type_uint;
                        if (val > uint_max) {
                            type = type_ulong;
                            if (val > ulong_max) {
                                type = type_ullong;
                            }
                        }
                        break;
                    case SUFFIX_L:
                        type = type_long;
                        if (val > long_max) {
                            type = type_llong;
                            overflow = val > llong_max;
                        }
                        break;
                    case SUFFIX_UL:
                        type = type_ulong;
                        if (val > ulong_max) {
                            type = type_ullong;
                        }
                        break;
                    case SUFFIX_LL:
                        type = type_llong;
                        overflow = val > llong_max;
                        break;
                    case SUFFIX_ULL:
                        type = type_ullong;
                        break;
                    default:
                        assert(0);
                        break;
                }
                if (overflow) {
                    fatal_error(expr.pos, "Integer literal overflow");
                }
            }
            else {
                switch (expr.int_lit.suffix) {
                    case SUFFIX_NONE:
                        type = type_int;
                        if (val > int_max) {
                            type = type_uint;
                            if (val > uint_max) {
                                type = type_long;
                                if (val > long_max) {
                                    type = type_ulong;
                                    if (val > ulong_max) {
                                        type = type_llong;
                                        if (val > llong_max) {
                                            type = type_ullong;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SUFFIX_U:
                        type = type_uint;
                        if (val > uint_max) {
                            type = type_ulong;
                            if (val > ulong_max) {
                                type = type_ullong;
                            }
                        }
                        break;
                    case SUFFIX_L:
                        type = type_long;
                        if (val > long_max) {
                            type = type_ulong;
                            if (val > ulong_max) {
                                type = type_llong;
                                if (val > llong_max) {
                                    type = type_ullong;
                                }
                            }
                        }
                        break;
                    case SUFFIX_UL:
                        type = type_ulong;
                        if (val > ulong_max) {
                            type = type_ullong;
                        }
                        break;
                    case SUFFIX_LL:
                        type = type_llong;
                        if (val > llong_max) {
                            type = type_ullong;
                        }
                        break;
                    case SUFFIX_ULL:
                        type = type_ullong;
                        break;
                    default:
                        assert(0);
                        break;
                }
            }
            cast_operand(ref operand, type);
            return operand;
        }

        Operand resolve_expr_modify(Expr expr) {
            Operand operand = resolve_expr(expr.modify.expr);
            Type type = operand.type;
            complete_type(type);
            if (!operand.is_lvalue) {
                fatal_error(expr.pos, "Cannot modify non-lvalue");
            }
            if (type.nonmodifiable) {
                fatal_error(expr.pos, "Cannot modify non-modifiable type");
            }
            if (!(is_integer_type(type) || is_ptr_type(type))) {
                fatal_error(expr.pos, "{0} only valid for integer and pointer types", token_kind_name(expr.modify.op));
            }
            if (is_ptr_type(type) && type_sizeof(type.@base) == 0) {
                fatal_error(expr.pos, "Cannot do pointer arithmetic with size 0 base type");
            }
            return operand_rvalue(type);
        }

        void try_const_cast(ref Operand operand, Expr expr) {
            Type unqual = unqualify_ptr_type(operand.type);
            if (!operand.is_lvalue && unqual != operand.type) {
                set_type_conv(expr, unqual);
                operand.type = unqual;
            }
        }

        Operand resolve_expr_new(Expr expr, Type expected_type) {
            if (expr.new_expr.alloc != null) {
                Operand alloc = resolve_expr(expr.new_expr.alloc);
                if (!convert_operand(ref alloc, type_allocator_ptr)) {
                    fatal_error(expr.new_expr.alloc.pos, "Allocator of new must have type Allocator or be pointer to struct with leading field of type Allocator");
                }
            }
            if (expr.new_expr.len != null) {
                Operand len = resolve_expr_rvalue(expr.new_expr.len);
                if (!is_integer_type(len.type)) {
                    fatal_error(expr.new_expr.len.pos, "Length argument of new must have integer type");
                }
            }
            Type expected_base = null;
            if (is_ptr_type(expected_type)) {
                expected_base = expected_type.@base;
            }
            if (expr.new_expr.arg == null) {
                expected_type = type_decay(expected_type);
                if (!is_ptr_type(expected_type)) {
                    fatal_error(expr.pos, "New with void argument must have expected pointer type");
                }
                return operand_rvalue(expected_type);
            }
            else {
                Operand arg = resolve_expected_expr(expr.new_expr.arg, expr.new_expr.len != null ? expected_type : expected_base);
                if (expr.new_expr.len != null) {
                    if (!is_ptr_type(arg.type)) {
                        fatal_error(expr.new_expr.arg.pos, "Argument to new[] must have pointer type");
                    }
                }
                else {
                    if (!arg.is_lvalue) {
                        fatal_error(expr.new_expr.arg.pos, "Argument to new must be lvalue");
                    }
                }
                complete_type(arg.type);
                if (type_sizeof(arg.type) == 0) {
                    fatal_error(expr.new_expr.arg.pos, "Type of argument to new has zero size");
                }
                return operand_rvalue(expr.new_expr.len != null ? arg.type : type_ptr(arg.type));
            }
        }

        Operand resolve_expected_expr(Expr expr, Type expected_type) {
            Operand result = default;
            switch (expr.kind) {
                case EXPR_PAREN:
                    result = resolve_expected_expr(expr.paren.expr, expected_type);
                    break;
                case EXPR_INT:
                    result = resolve_expr_int(expr);
                    break;
                case EXPR_FLOAT:
                    result = operand_const(expr.float_lit.suffix == SUFFIX_D ? type_double : type_float, default);
                    break;
                case EXPR_STR:
                    result = operand_rvalue(type_array(type_char, expr.str_lit.val.Length + 1, false));
                    break;
                case EXPR_NAME:
                    // HACK
                    result = resolve_expr_name(expr);
                    set_resolved_sym(expr, resolve_name(expr.name));
                    break;
                case EXPR_CAST:
                    result = resolve_expr_cast(expr);
                    break;
                case EXPR_CALL:
                    result = resolve_expr_call(expr, expected_type);
                    break;
                case EXPR_INDEX:
                    result = resolve_expr_index(expr);
                    break;
                case EXPR_FIELD:
                    result = resolve_expr_field(expr);
                    break;
                case EXPR_COMPOUND:
                    result = resolve_expr_compound(expr, expected_type);
                    break;
                case EXPR_UNARY:
                    if (expr.unary.op == TOKEN_AND) {
                        Operand operand;
                        if (expected_type != null && is_ptr_type(expected_type)) {
                            operand = resolve_expected_expr(expr.unary.expr, expected_type.@base);
                        }
                        else {
                            operand = resolve_expr(expr.unary.expr);
                        }
                        if (!operand.is_lvalue) {
                            fatal_error(expr.pos, "Cannot take address of non-lvalue");
                        }
                        result = operand_rvalue(type_ptr(operand.type));
                    }
                    else {
                        result = resolve_expr_unary(expr);
                    }
                    break;
                case EXPR_BINARY:
                    result = resolve_expr_binary(expr);
                    break;
                case EXPR_TERNARY:
                    result = resolve_expr_ternary(expr, expected_type);
                    break;
                case EXPR_SIZEOF_EXPR: {
                    if (expr.sizeof_expr.kind == EXPR_NAME) {
                        Sym sym = resolve_name(expr.sizeof_expr.name);
                        if (sym != null && sym.kind == SYM_TYPE) {
                            complete_type(sym.type);
                            result = operand_const(type_usize, new Val { ll = type_sizeof(sym.type) });
                            set_resolved_type(expr.sizeof_expr, sym.type);
                            set_resolved_sym(expr.sizeof_expr, sym);
                            break;
                        }
                    }
                    var type = resolve_expr(expr.sizeof_expr).type;
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_sizeof(type) });
                    break;
                }

                case EXPR_SIZEOF_TYPE: {
                    var type = resolve_typespec(expr.sizeof_type);
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_sizeof(type) });
                    break;
                }
                case EXPR_TYPEOF_TYPE: {
                    Type type = resolve_typespec_strict(expr.typeof_type, true);
                    result = operand_const(type_ullong, new Val { ull = type.typeid });
                    break;
                }
                case EXPR_TYPEOF_EXPR: {
                    if (expr.typeof_expr.kind == EXPR_NAME) {
                        Sym sym = resolve_name(expr.typeof_expr.name);
                        if (sym != null && sym.kind == SYM_TYPE) {
                            result = operand_const(type_ullong, new Val { ull = sym.type.typeid });
                            set_resolved_type(expr.typeof_expr, sym.type);
                            break;
                        }
                    }
                    Type type = resolve_expr(expr.typeof_expr).type;
                    result = operand_const(type_ullong, new Val { ull = type.typeid });
                    break;
                }
                case EXPR_ALIGNOF_EXPR: {
                    if (expr.alignof_expr.kind == EXPR_NAME) {
                        Sym sym = resolve_name(expr.alignof_expr.name);
                        if (sym != null && sym.kind == SYM_TYPE) {
                            complete_type(sym.type);
                            result = operand_const(type_usize, new Val { ll = type_alignof(sym.type) });
                            set_resolved_type(expr.alignof_expr, sym.type);
                            set_resolved_sym(expr.alignof_expr, sym);
                            break;
                        }
                    }
                    Type type = resolve_expr(expr.alignof_expr).type;
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_alignof(type) });
                    break;
                }
                case EXPR_ALIGNOF_TYPE: {
                    Type type = resolve_typespec(expr.alignof_type);
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_alignof(type) });
                    break;
                }
                case EXPR_OFFSETOF: {
                    Type type = resolve_typespec(expr.offsetof_field.type);
                    complete_type(type);
                    if (type.kind != TYPE_STRUCT && type.kind != TYPE_UNION) {
                        fatal_error(expr.pos, "offsetof can only be used with struct/union types");
                    }
                    int field = aggregate_item_field_index(type, expr.offsetof_field.name);
                    if (field < 0) {
                        fatal_error(expr.pos, "No field '{0}' in type", expr.offsetof_field.name);
                    }
                    result = operand_const(type_usize, new Val { ll = type.aggregate.fields[field].offset });
                    break;
                }
                case EXPR_MODIFY:
                    result = resolve_expr_modify(expr);
                    break;
                case EXPR_NEW:
                    result = resolve_expr_new(expr, expected_type);
                    break;
                default:
                    assert(false);
                    result = default;
                    break;
            }
            try_const_cast(ref result, expr);
            if (expected_type != null && unqualify_type(expected_type) == type_any && unqualify_type(result.type) != type_any) {
                set_implicit_any(expr);
                set_resolved_type(expr, type_decay(result.type));
            }
            else {
                set_resolved_type(expr, result.type);
            }

            return result;
        }

        Operand resolve_expr(Expr expr) {
            return resolve_expected_expr(expr, null);
        }

        Operand resolve_const_expr(Expr expr) {
            Operand operand = resolve_expr(expr);
            if (!operand.is_const) {
                fatal_error(expr.pos, "Expected constant expression");
            }
            return operand;
        }

        Operand resolve_expr_rvalue(Expr expr) {
            return operand_decay(resolve_expr(expr));
        }

        Operand resolve_expected_expr_rvalue(Expr expr, Type expected_type) {
            return operand_decay(resolve_expected_expr(expr, expected_type));
        }

        internal void add_package_decls(Package package) {
            foreach (Decl decl in package.decls) {
                if (decl.kind == DECL_NOTE) {
                    if (!decl_note_names.Contains(decl.note.name)) {
                        warning(decl.pos, "Unknown declaration #directive '{0}'", decl.note.name);
                    }
                    if (decl.note.name == declare_note_name) {
                        if (decl.note.num_args != 1) {
                            fatal_error(decl.pos, "#declare_note takes 1 argument");
                        }
                        Expr arg = decl.note.args[0].expr;
                        if (arg.kind != EXPR_NAME) {
                            fatal_error(decl.pos, "#declare_note argument must be name");
                        }
                        decl_note_names.Add(arg.name);
                    }
                    else if (decl.note.name == static_assert_name) {
                        // TODO: decide how to handle top-level  asserts wrt laziness/tree shaking
                        if (!flag_lazy) {
                            resolve_static_assert(decl.note);
                        }
                    }
                }
                else if (decl.kind == DECL_IMPORT) {
                    // Add to list of imports
                }
                else {
                    sym_global_decl(decl);
                }
            }
        }

        bool is_implicit_any(Expr expr) {
            return implicit_any_list.Contains(expr);
        }

        void set_implicit_any(Expr expr) {
            implicit_any_list.Add(expr);
        }

        Type type_conv(Expr expr) {
            if (!type_conv_map.ContainsKey(expr))
                return null;
            Type type = type_conv_map[expr];
            return type;
        }

        void set_type_conv(Expr expr, Type type) {
            type_conv_map[expr] = type;
        }

        Type pointer_promo_type(Expr expr) {
            if (!pointer_promo_map.ContainsKey(expr))
                return null;
            Type type = pointer_promo_map[expr];
            return type;
        }

        void set_pointer_promo_type(Expr expr, Type type) {
            pointer_promo_map[expr] = type;
        }

        bool is_package_dir(ReadOnlySpan<char> search_path, ReadOnlySpan<char> package_path, Span<char> dest) {
            var str = Path.Join(search_path, package_path);
            if (Directory.Exists(str) && Directory.EnumerateFiles(str, "*.ion").Any()) {
                ((ReadOnlySpan<char>)str).CopyTo(dest);
                return true;
            }
            return false;
        }

        bool copy_package_full_path(ReadOnlySpan<char> package_path, Span<char> dest) {
            for (int i = 0; i < package_search_paths.Count; i++) {
                string path = package_search_paths[i];
                if (is_package_dir(path, package_path, dest)) {
                    return true;
                }
            }
            return false;
        }

        unsafe Package import_package(string package_path) {
            Package package = package_map.ContainsKey(package_path) ? package_map[package_path] : null;
            if (package == null) {
                package = new Package { path = package_path };
                if (flag_verbose)
                    print("Importing Package '{0}'", package_path);
                Span<char> full_path = stackalloc char[MAX_PATH];
                if (!copy_package_full_path(package_path, full_path)) {
                    return null;
                }
                Normalize(ref full_path);
                package.full_path = full_path.ToString();
                package.symbol_dict = new Dictionary<string, Sym>();
                package.symbols = new List<Sym>();
                //package.ref_pkg = new List<Package>();
                package.decls = new List<Decl>();
                add_package(package);
                compile_package(package);
            }
            return package;
        }

        void import_all_package_symbols(Package package) {
            for (int i = 0; i < package.symbols.Count; i++) {
                Sym sym = package.symbols[i];
                if (sym.home_package == package && sym.name != MAIN_NAME) {
                    sym_global_put(sym.name, sym);
                }
            }
        }

        void import_package_symbols(Decl decl, Package package) {
            for (int i = 0; i < decl.import.num_items; i++) {
                ImportItem item = decl.import.items[i];
                Sym sym = get_package_sym(package, item.name);
                if (sym == null) {
                    fatal_error(decl.pos, "Symbol '{0}' does not exist in package '{1}'", item.name, package.path);
                }
                sym_global_put(item.rename != null ? item.rename : item.name, sym);
            }
        }

        void process_package_imports(Package package) {
            foreach (Decl decl in package.decls) {
                if (decl.kind == DECL_NOTE) {
                    if (decl.note.name == always_name) {
                        package.always_reachable = true;
                    }
                }
                else if (decl.kind == DECL_IMPORT) {
                    var len = package.path.Length;
                    var sb = new StringBuilder();
                    if (decl.import.is_relative) {
                        sb.Append(package.path).Append("/");
                    }

                    for (int k = 0; k < decl.import.num_names; k++) {
                        if (!str_islower(decl.import.names[k])) {
                            fatal_error(decl.pos, "Import name must be lower case: '{0}'", decl.import.names[k]);
                        }
                        if (k > 0)
                            sb.Append("/");
                        sb.Append(decl.import.names[k]);
                    }
                    var path = sb.ToString();
                    Package imported_package = import_package(path);
                    //package.ref_pkg.Add(package);
                    if (imported_package == null) {
                        fatal_error(decl.pos, "Failed to import package '{0}'", path);
                    }

                    if (decl.import.import_all) {
                        import_all_package_symbols(imported_package);
                    }
                    else {
                        import_package_symbols(decl, imported_package);
                    }
                    string sym_name = decl.name ?? decl.import.names[decl.import.num_names - 1];
                    Sym sym = sym_new(SYM_PACKAGE, sym_name, decl);
                    sym.package = imported_package;
                    sym_global_put(sym_name, sym);
                }
            }
        }

        bool parse_package(Package package) {
            foreach (var path in Directory.EnumerateFiles(package.full_path, "*.ion")) {
                if (is_excluded_target_filename(path)) {
                    continue;
                }

                string code = File.ReadAllText(path, Encoding.ASCII);
                fixed (char* c = code)
                    init_stream(c, path);
                parse_decls(package.decls);
            }

            return package.decls.Count > 0;
        }

        bool compile_package(Package package) {
            if (!parse_package(package)) {
                return false;
            }
            Package old_package = enter_package(package);
            if (package_list.Count == 1) {
                init_builtin_syms();
            }
            if (builtin_package != null) {
                //package.ref_pkg.Add(builtin_package);
                import_all_package_symbols(builtin_package);
            }
            add_package_decls(package);
            process_package_imports(package);
            leave_package(old_package);
            return true;
        }

        void resolve_package_syms(Package package) {
            //Console.WriteLine("ENTER: resolving package syms for: '{0}'\n", package.path);
            Package old_package = enter_package(package);
            for (int i = 0; i < package.symbols.Count; i++) {
                Sym sym = package.symbols[i];
                if (sym.home_package == package) {
                    resolve_sym(sym);
                }
            }
            leave_package(old_package);
        }

        void finalize_reachable_syms() {
            if (flag_verbose)
                print("Finalizing reachable symbols");
            int prev_num_reachable = 0;
            var num_reachable = reachable_syms.Count;
            int i;
            for (i = 0; i < num_reachable; i++) {
                var sym = reachable_syms[i];
                finalize_sym(sym);
                if (i == num_reachable - 1) {
                    if (flag_verbose && false) {
                        // printf("New reachable symbols:");
                        //printf("\n");
                        for (var k = prev_num_reachable; k < num_reachable; k++) {
                            //var s = reachable_syms[k];
                            //printf(" {0}: {1}/{2}",k, s.home_package.path, s.name);
                        }
                        //System.Console.ReadKey();
                        //printf("\n");
                    }
                    prev_num_reachable = num_reachable;
                    num_reachable = reachable_syms.Count;
                }
            }
        }

        bool is_intrinsic(Sym sym) {
            if (sym == null || sym.kind != SYM_FUNC) {
                return false;
            }
            assert(is_func_type(sym.type));
            return sym.type.func.intrinsic;
        }

        void init_builtin_syms() {

            assert(current_package != null);

            type_ranks[(int)TYPE_BOOL] = 1;
            type_ranks[(int)TYPE_CHAR] = 2;
            type_ranks[(int)TYPE_SCHAR] = 2;
            type_ranks[(int)TYPE_UCHAR] = 2;
            type_ranks[(int)TYPE_SHORT] = 3;
            type_ranks[(int)TYPE_USHORT] = 3;
            type_ranks[(int)TYPE_INT] = 4;
            type_ranks[(int)TYPE_UINT] = 4;
            type_ranks[(int)TYPE_LONG] = 5;
            type_ranks[(int)TYPE_ULONG] = 5;
            type_ranks[(int)TYPE_LLONG] = 6;
            type_ranks[(int)TYPE_ULLONG] = 6;

            type_names[(int)TYPE_VOID] = "void";
            type_names[(int)TYPE_BOOL] = "bool";
            type_names[(int)TYPE_CHAR] = "char";
            type_names[(int)TYPE_SCHAR] = "schar";
            type_names[(int)TYPE_UCHAR] = "uchar";
            type_names[(int)TYPE_SHORT] = "short";
            type_names[(int)TYPE_USHORT] = "ushort";
            type_names[(int)TYPE_INT] = "int";
            type_names[(int)TYPE_UINT] = "uint";
            type_names[(int)TYPE_LONG] = "long";
            type_names[(int)TYPE_ULONG] = "ulong";
            type_names[(int)TYPE_LLONG] = "llong";
            type_names[(int)TYPE_ULLONG] = "ullong";
            type_names[(int)TYPE_FLOAT] = "float";
            type_names[(int)TYPE_DOUBLE] = "double";

            typeid_kind_names[(int)TYPE_NONE] = "TYPE_NONE";
            typeid_kind_names[(int)TYPE_VOID] = "TYPE_VOID";
            typeid_kind_names[(int)TYPE_BOOL] = "TYPE_BOOL";
            typeid_kind_names[(int)TYPE_CHAR] = "TYPE_CHAR";
            typeid_kind_names[(int)TYPE_SCHAR] = "TYPE_SCHAR";
            typeid_kind_names[(int)TYPE_UCHAR] = "TYPE_UCHAR";
            typeid_kind_names[(int)TYPE_SHORT] = "TYPE_SHORT";
            typeid_kind_names[(int)TYPE_USHORT] = "TYPE_USHORT";
            typeid_kind_names[(int)TYPE_INT] = "TYPE_INT";
            typeid_kind_names[(int)TYPE_UINT] = "TYPE_UINT";
            typeid_kind_names[(int)TYPE_LONG] = "TYPE_LONG";
            typeid_kind_names[(int)TYPE_ULONG] = "TYPE_ULONG";
            typeid_kind_names[(int)TYPE_LLONG] = "TYPE_LLONG";
            typeid_kind_names[(int)TYPE_ULLONG] = "TYPE_ULLONG";
            typeid_kind_names[(int)TYPE_FLOAT] = "TYPE_FLOAT";
            typeid_kind_names[(int)TYPE_DOUBLE] = "TYPE_DOUBLE";

            typeid_kind_names[(int)TYPE_CONST] = "TYPE_CONST";
            typeid_kind_names[(int)TYPE_PTR] = "TYPE_PTR";
            typeid_kind_names[(int)TYPE_ARRAY] = "TYPE_ARRAY";
            typeid_kind_names[(int)TYPE_STRUCT] = "TYPE_STRUCT";
            typeid_kind_names[(int)TYPE_UNION] = "TYPE_UNION";
            typeid_kind_names[(int)TYPE_FUNC] = "TYPE_FUNC";
            typeid_kind_names[(int)TYPE_TUPLE] = "TYPE_TUPLE";

            sym_global_type("void", type_void);
            sym_global_type("bool", type_bool);
            sym_global_type("char", type_char);

            sym_global_type("schar", type_schar);
            sym_global_type("uchar", type_uchar);
            sym_global_type("short", type_short);
            sym_global_type("ushort", type_ushort);
            sym_global_type("int", type_int);
            sym_global_type("uint", type_uint);
            sym_global_type("long", type_long);
            sym_global_type("ulong", type_ulong);
            sym_global_type("llong", type_llong);
            sym_global_type("ullong", type_ullong);
            sym_global_type("float", type_float);
            sym_global_type("double", type_double);
        }

        void postinit_builtin() {
            assert(current_package == builtin_package);
            Sym sym = resolve_name("Allocator");
            if (sym != null)
                assert(sym.kind == SYM_TYPE);
            type_allocator = sym.type;
            type_allocator_ptr = type_ptr(type_allocator);
        }
    }


    internal unsafe struct Operand
    {
        public Type type;
        public bool is_lvalue;
        public bool is_const;
        public Val val;
    }

    internal enum SymReachable : byte
    {
        REACHABLE_NONE,
        REACHABLE_NATURAL,
        REACHABLE_FORCED
    }

    internal enum SymKind
    {
        SYM_NONE,
        SYM_VAR,
        SYM_CONST,
        SYM_FUNC,
        SYM_TYPE,
        SYM_PACKAGE,
    }

    internal enum SymState
    {
        SYM_UNRESOLVED,
        SYM_RESOLVING,
        SYM_RESOLVED
    }

    internal unsafe class Package
    {
        public string path;
        public string full_path;
        public List<Decl> decls;
        public Dictionary<string, Sym> symbol_dict;
        public List<Sym> symbols;
        public string external_name;
        public bool always_reachable;

        public override string ToString() => $"PKG: '{path}' Decls: {decls.Count}, Syms: {symbols.Count}";
    }

    internal unsafe class Sym {
        public string name;
        public Package home_package;
        public SymKind kind;
        public SymState state;
        public SymReachable reachable;
        public string external_name;
        public Decl decl;
        public Type type;
        public Val val;
        public Package package;

        public override string ToString() => $"SYM: '{name}'{(type == null ? "" : $", Type: {type}")}{(home_package == null ? "" : $", HomePkg: {home_package.path}")}{(package == null ? "" : $", Pkg: {package.path}")}";
    }

}