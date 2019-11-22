//#define DEBUG_VERBOSE
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace IonLang
{
    using static CompoundFieldKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static TypeKind;
    using static TypespecKind;
    using static TokenMod;
    using static SymKind;
    using static SymReachable;
    using static AggregateItemKind;
    using static AggregateKind;
    using static TokenSuffix;

    unsafe partial class Ion
    {
        StringBuilder gen_buf = new StringBuilder();
        StringBuilder gen_preamble_buf = new StringBuilder();
        StringBuilder gen_postamble_buf = new StringBuilder();
        readonly char[] char_to_escape  = new char[256];

        readonly char* defineStr = "#define ".ToPtr();
        readonly char* includeStr = "#include ".ToPtr();
        readonly char* lineStr = "#line ".ToPtr();
        readonly char* void_str = "void".ToPtr();
        readonly char* externStr = "extern".ToPtr();

        SrcPos gen_pos;
        int gen_indent;

        void indent() {
            var size = gen_indent * 4;

            gen_buf.Append(string.Empty.PadRight(size));
            gen_pos.line++;
        }

        void writeln() {
#if DEBUG_VERBOSE
            System.Console.WriteLine();
#endif
            gen_buf.AppendLine();
        }

        void c_write(char c) {
#if DEBUG_VERBOSE
            System.Console.Write(c);
#endif
            gen_buf.Append(c);
        }

        void c_write(char* c) {
#if DEBUG_VERBOSE
            System.Console.Write(new string(c));
#endif
            gen_buf.Append(c, strlen(c));
            //while ((*(gen_buf + gen_pos) = *(c++)) != 0) gen_pos++;
        }

        void c_write(char* c, int len) {
#if DEBUG_VERBOSE
            System.Console.Write(new string(c));
#endif
            gen_buf.Append(c, len);
            //Unsafe.CopyBlock(gen_buf + gen_pos, c, (uint)len << 1);
            //gen_pos += len;
        }
        void c_write(ReadOnlySpan<char> s) {
#if DEBUG_VERBOSE
            System.Console.Write(s.ToString());
#endif
            gen_buf.Append(s);
            //Unsafe.CopyBlock(gen_buf + gen_pos, c, (uint)len << 1);
            //gen_pos += len;
        }
        void c_write(StringBuilder s) {
#if DEBUG_VERBOSE
            System.Console.Write(s);
#endif
            gen_buf.Append(s);
        }
        void genlnf(char c) {
            genln();
            c_write(c);
        }
        void genlnf(StringBuilder sb) {
            genln();
            c_write(sb);
        }

        void genlnf(ReadOnlySpan<char> c) {
            genln();
            c_write(c);
        }

        void genlnf(char* fmt) {
            genln();
            c_write(fmt);
        }
        void genlnf(char* fmt, int len) {
            genln();
            c_write(fmt, len);
        }

        void genln() {
            writeln();
            indent();
            gen_pos.line++;
        }

        bool is_reachable(SymReachable reachable) {
            return flag_fullgen || reachable == REACHABLE_NATURAL;
        }

        bool is_sym_reachable(Sym* sym) {
            return is_reachable(sym->reachable);
        }

        bool is_tuple_reachable(Type* type) {
            return is_reachable(get_reachable(type));
        }

        bool is_excluded_typeinfo(Type* type) {
            while (type->kind == TYPE_ARRAY || type->kind == TYPE_CONST || type->kind == TYPE_PTR) {
                type = type->@base;
            }
            if (type->sym != null) {
                if (get_decl_note(type->sym->decl, _I("notypeinfo")) != null) {
                    return true;
                }
                else {
                    return !is_sym_reachable(type->sym);
                }
            }
            else if (type->kind == TYPE_TUPLE) {
                return !is_tuple_reachable(type);
            }
            else {
                assert(type->sym == null);
                return type->kind == TYPE_STRUCT || type->kind == TYPE_UNION;
            }
        }

        Map gen_name_map;
        char* get_gen_name_or_default(void* ptr, char* default_name) {
            char *name = gen_name_map.map_get<char>(ptr);
            if (name == null) {
                Sym *sym = get_resolved_sym(ptr);
                if (sym != null) {
                    if (sym->external_name != null) {
                        name = sym->external_name;
                    }
                    else if (sym->home_package->external_name != null) {
                        char *external_name = sym->home_package->external_name;
                        const int SIZE = 256;
                        char* buf = stackalloc char[SIZE];
                        if (sym->kind == SYM_CONST) {
                            char *cptr = buf;
                            for (char *str = external_name; *str != '\0' && cptr < buf + SIZE - 1; str++, cptr++) {
                                *cptr = char.ToUpper(*str);
                            }
                            *cptr = '\0';
                            if (cptr < buf + SIZE) {
                                external_name = buf;
                            }
                        }
                        name = strcat2(external_name, sym->name);
                    }
                    else {
                        name = sym->name;
                    }
                }
                else {
                    assert(default_name);
                    name = default_name;
                }
                gen_name_map.map_put(ptr, (void*)name);
            }
            return name;
        }

        static readonly char *errorStr = "ERROR".ToPtr();
        char* get_gen_name(void* ptr) {
            char *name = get_gen_name_or_default(ptr, errorStr);
            assert(name != errorStr);
            return name;
        }

        char* cdecl_name(Type* type) {
            char* type_name = type_names[(int)type->kind];
            if (type_name != null) {
                return type_name;
            }
            else if (type->kind == TYPE_TUPLE) {
                char* id = type->typeid.itoa();
                return strcat2("tuple".ToPtr(), id);
            }
            else {
                assert(type->sym != null);
                return get_gen_name(type->sym);
            }
        }

        void type_to_cdecl(Type* type, char* str) {
            switch (type->kind) {
                case TYPE_PTR:
                    if (str != null) {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        //buf_write(cdecl_name(type->@base));
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPE_CONST:
                    if (str != null) {
                        type_to_cdecl(type->@base, const_keyword);
                        c_write(' ');
                        c_write('(');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        c_write(const_keyword, 5);
                        c_write(' ');
                        type_to_cdecl(type->@base, null);
                    }
                    break;
                case TYPE_ARRAY:
                    if (str != null) {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write(str);
                        c_write('[');
                        c_write(type->num_elems.itoa());
                        c_write(']');
                        c_write(')');
                    }
                    else {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('[');
                        c_write(type->num_elems.itoa());
                        c_write(']');
                    }

                    break;
                case TYPE_FUNC: {
                    if (type->func.ret != null)
                        type_to_cdecl(type->func.ret, null);
                    else
                        c_write(void_str, 4);
                    c_write('(');
                    c_write('*');
                    if (str != null)
                        c_write(str);
                    c_write(')');

                    c_write('(');
                    if (type->func.num_params == 0)
                        c_write(void_str, 4);
                    else
                        for (var i = 0; i < type->func.num_params; i++) {
                            if (i > 0) {
                                c_write(',');
                                c_write(' ');
                            }

                            type_to_cdecl(type->func.@params[i], null);
                        }
                    if (type->func.has_varargs) {
                        c_write(',');
                        c_write(' ');
                        c_write('.');
                        c_write('.');
                        c_write('.');
                    }
                    c_write(')');
                }
                break;
                default:
                    if (str != null) {
                        c_write(cdecl_name(type));
                        c_write(' ');
                        c_write(str);
                    }
                    else {
                        c_write(cdecl_name(type));
                    }
                    break;
            }
        }

        void gen_str(char* str, bool multiline) {
            if (multiline) {
                gen_indent++;
                genln();
            }
            c_write('\"');
            while (*str != 0) {
                var start = str;
                while (*str != 0 && isprint(*str) && char_to_escape[*str] == 0)
                    str++;
                if (start != str)
                    c_write(start, (int)(str - start));
                if (*str != 0 && *str < 256) {
                    if (char_to_escape[*str] != 0) {
                        c_write('\\');
                        c_write(char_to_escape[*str]);
                        if (multiline && str[0] == '\n' && str[1] != 0) {
                            c_write('\"');
                            genlnf('\"');
                        }
                    }
                    else {
                        assert(!isprint(*str));

                        c_write('\\');
                        c_write('x');
                        c_write(((int)*str).itoa(16));
                    }
                    str++;
                }
            }
            c_write('\"');
            if (multiline) {
                gen_indent--;
            }
        }

        void gen_buf_pos(ref StringBuilder pbuf, SrcPos pos) {
            if (flag_nosync) {
                return;
            }
            var buf = pbuf;
            buf.AppendLine().Append(lineStr, 6).Append(pos.line).Append(' ');
            var old_gen_buf = gen_buf;
            gen_buf = buf;
            gen_str(pos.name, false);
            buf = gen_buf;
            gen_buf = old_gen_buf;
            buf.AppendLine();
            pbuf = buf;
        }

        void gen_sync_pos(SrcPos pos) {
            if (flag_nosync)
                return;

            assert(pos.name != null && pos.line != 0);
            if (gen_pos.line != pos.line || gen_pos.name != pos.name) {
                genln();
                c_write(lineStr, 6);
                c_write(pos.line.itoa());
                if (gen_pos.name != pos.name) {
                    c_write(' ');
                    gen_str(_I(pos.name), false);
                }
                gen_pos = pos;
            }
        }

        void typespec_to_cdecl(Typespec* typespec, char* str) {
            if (typespec == null) {
                c_write(void_str);
                if (*str != 0)
                    c_write(' ');
            }
            switch (typespec->kind) {
                case TYPESPEC_NAME:
                    if (str != null) {
                        c_write(get_gen_name(typespec));
                        c_write(' ');
                        c_write(str);
                    }
                    else {
                        c_write(get_gen_name(typespec));
                    }

                    break;
                case TYPESPEC_CONST:
                    typespec_to_cdecl(typespec->@base, str);
                    break;
                case TYPESPEC_PTR:
                    if (str != null) {
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        if (typespec->@base != null)
                            typespec_to_cdecl(typespec->@base, null);
                        c_write('*');
                    }

                    break;
                case TYPESPEC_ARRAY:
                    if (typespec->@base != null) {
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');
                    }
                    if (str != null) {
                        c_write('(');
                        c_write(str);
                    }
                    c_write('[');
                    if (typespec->num_elems != null)
                        gen_expr(typespec->num_elems);
                    c_write(']');

                    if (str != null)
                        c_write(')');

                    break;
                case TYPESPEC_TUPLE:
                        Type *type = get_resolved_type(typespec);
                        c_write("tuple");
                        c_write(type->typeid.itoa());
                    if (str != null) {
                        c_write(' ');
                        c_write(str);
                    }
                    break;
                case TYPESPEC_FUNC: {
                    if (typespec->func.ret != null)
                        typespec_to_cdecl(typespec->func.ret, null);
                    else
                        c_write(void_str, 4);

                    c_write(' ');
                    c_write('(');
                    c_write('*');
                    if (str != null)
                        c_write(str);
                    c_write(')');


                    c_write('(');
                    if (typespec->func.num_args == 0) {
                        c_write(void_str, 4);
                    }
                    else {
                        for (var i = 0; i < typespec->func.num_args; i++) {
                            if (i > 0) {
                                c_write(',');
                                c_write(' ');
                            }

                            typespec_to_cdecl(typespec->func.args[i], null);
                        }

                        if (typespec->func.has_varargs) {
                            c_write(',');
                            c_write(' ');
                            c_write('.');
                            c_write('.');
                            c_write('.');
                        }
                    }
                    c_write(')');

                    break;
                }

                default:
                    assert(false);
                    break;
            }
        }

        void gen_defs() {
            for (Sym** it = (Sym**)sorted_syms->_begin; it < sorted_syms->_top; it++) {
                Sym* sym = *it;
                Decl* decl = sym->decl;
                if (sym->state != SymState.SYM_RESOLVED || decl == null || decl->is_incomplete || sym->reachable != REACHABLE_NATURAL) {
                    continue;
                }
                if (decl->kind == DECL_FUNC) {
                    bool foreign = is_decl_foreign(decl);
                    var buf = gen_buf;
                    if (foreign) {
                        gen_buf = new StringBuilder();
                    }
                    if (get_decl_note(decl, inline_name) != null) {
                        genlnf("INLINE");
                    }
                    if (get_decl_note(decl, _I("noinline")) != null) {
                        genlnf("NOINLINE");
                    }
                    gen_func_decl(decl);
                    c_write(' ');
                    gen_stmt_block(decl->func.block);
                    genln();
                    if (foreign) {
                        gen_buf = buf;
                    }

                }
                else if (decl->kind == DECL_VAR) {
                    if (is_decl_threadlocal(decl)) {
                        genlnf("THREADLOCAL");
                    }
                    genln();
                    if (decl->var.type != null && !is_incomplete_array_typespec(decl->var.type)) {
                        typespec_to_cdecl(decl->var.type, get_gen_name(sym));
                    }
                    else {
                        type_to_cdecl(sym->type, get_gen_name(sym));
                    }
                    if (decl->var.expr != null) {
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_expr(decl->var.expr);
                    }
                    c_write(';');
                    genln();
                }
            }
        }

        void gen_func_decl(Decl* decl) {
            assert(decl->kind == DECL_FUNC);
            gen_sync_pos(decl->pos);
            genln();

            if (decl->func.ret_type != null) {
                typespec_to_cdecl(decl->func.ret_type, null);
                c_write(' ');
                c_write(get_gen_name(decl));
                c_write('(');
            }
            else {
                c_write(void_str);
                c_write(' ');
                c_write(get_gen_name(decl));
                c_write('(');
            }

            if (decl->func.num_params == 0)
                c_write(void_str);
            else
                for (var i = 0; i < decl->func.num_params; i++) {
                    var param = decl->func.@params + i;
                    if (i != 0) {
                        c_write(',');
                        c_write(' ');
                    }
                    typespec_to_cdecl(param->type, param->name);
                }
            if (decl->func.has_varargs) {
                c_write(',');
                c_write(' ');
                c_write('.');
                c_write('.');
                c_write('.');
            }
            c_write(')');
        }

        void gen_forward_decls() {
            genln();
            for (int i = 0; i < tuple_types->count; i++) {
                Type *type = tuple_types->Get<Type>(i);
                if (is_tuple_reachable(type)) {
                    char* id = type->typeid.itoa();
                    genlnf("typedef struct tuple");
                    c_write(id);
                    c_write(" tuple");
                    c_write(id);
                    c_write(';');
                }
            }
            for (var it = (Sym**)sorted_syms->_begin; it != sorted_syms->_top; it++) {
                var sym = *it;         
                var decl = sym->decl;
                if (decl == null || !is_sym_reachable(sym))
                    continue;
                if (is_decl_foreign(decl))
                    continue;

                var name = get_gen_name(sym);
                switch (decl->kind) {
                    case DECL_STRUCT:
                        genln();
                        c_write(typedef_keyword);
                        c_write(' ');
                        c_write(struct_keyword);
                        c_write(' ');
                        c_write(name);
                        c_write(' ');
                        c_write(name);
                        c_write(';');
                        break;
                    case DECL_UNION:
                        genln();
                        c_write(typedef_keyword);
                        c_write(' ');
                        c_write(union_keyword);
                        c_write(' ');
                        c_write(name);
                        c_write(' ');
                        c_write(name);
                        c_write(';');
                        break;
                }
            }
        }

        void gen_tuple_decl(Type* type) {
            var name = get_gen_name(type->sym);
            genln();
            c_write(struct_keyword);
            c_write(' ');
            c_write(name);
            c_write(' ');
            c_write('{');
            gen_indent++;
            for (var i = 0; i < type->aggregate.num_fields; i++) {
                TypeField field = type->aggregate.fields[i];
                genln();
                type_to_cdecl(field.type, field.name);
                c_write(';');
            }
            gen_indent--;
            genln();
            c_write('}');
            c_write(';');
        }

        void gen_aggregate(Decl* decl) {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            if (decl->is_incomplete) {
                return;
            }
            genln();
            if (decl->kind == DECL_STRUCT)
                c_write(struct_keyword);
            else
                c_write(union_keyword);
            c_write(' ');
            c_write(get_gen_name(decl));
            c_write(' ');
            c_write('{');

            gen_aggregate_items(decl->aggregate);

            genln();
            c_write('}');
            c_write(';');

            void gen_aggregate_items(Aggregate* aggregate) {
                gen_indent++;
                for (var i = 0; i < aggregate->num_items; i++) {
                    AggregateItem item = aggregate->items[i];
                    if (item.kind == AGGREGATE_ITEM_FIELD) {
                        for (var j = 0; j < item.num_names; j++) {
                            gen_sync_pos(item.pos);
                            genln();
                            if (item.type->kind == TYPESPEC_ARRAY && item.type->num_elems == null) {
                                typespec_to_cdecl(new_typespec_ptr(item.pos, item.type->@base), item.names[j]);
                                c_write(';');
                            }
                            else {
                                typespec_to_cdecl(item.type, item.names[j]);
                                c_write(';');
                            }
                        }
                    }
                    else if (item.kind == AGGREGATE_ITEM_SUBAGGREGATE) {
                        genln();
                        if (item.subaggregate->kind == AGGREGATE_STRUCT)
                            c_write(struct_keyword, 6);
                        else
                            c_write(union_keyword, 5);
                        c_write(' ');
                        c_write('{');
                        gen_aggregate_items(item.subaggregate);
                        genln();
                        c_write('}');
                        c_write(';');
                    }
                    else {
                        assert(0);
                    }
                }
                gen_indent--;
            }
        }

        void gen_typeid(Type* type) {
            if (type->size == 0 || is_excluded_typeinfo(type)) {
                c_write("TYPEID0(".ToPtr(), 8);
                c_write(type->typeid.itoa());
                c_write(',');
                c_write(' ');
                c_write(typeid_kind_name(type));
                c_write(')');
            }
            else {
                c_write("TYPEID(".ToPtr(), 7);
                c_write(type->typeid.itoa());
                c_write(',');
                c_write(' ');
                c_write(typeid_kind_name(type));
                c_write(',');
                c_write(' ');
                type_to_cdecl(type, null);
                c_write(')');
            }

            char* typeid_kind_name(Type* type) {
                if (type->kind < NUM_TYPE_KINDS) {
                    char *name = typeid_kind_names[(int)type->kind];
                    if (name != null) {
                        return name;
                    }
                }
                return typeid_kind_names[(int)TYPE_NONE];
            }
        }

        void gen_intrinsic(Sym* sym, Expr* expr) {
            Type *type = get_resolved_type(expr->call.args[0]);
            Type * @base = is_ptr_type(type) ? unqualify_type(type->@base) : null;
            Type *key = @base != null && is_aggregate_type(@base) && @base->aggregate.num_fields == 2 ? @base->aggregate.fields[0].type : null;
            Type *val = @base != null  && is_aggregate_type(@base) && @base->aggregate.num_fields == 2 ? @base->aggregate.fields[1].type : null;
            if (sym->name == _I("va_copy") || sym->name == _I("va_start") || sym->name == _I("va_end")) {
                c_write(sym->name);
                c_write('(');
                for (int i = 0; i < expr->call.num_args; i++) {
                    if (i != 0) {
                        c_write(',');
                        c_write(' ');
                    }
                    gen_expr(expr->call.args[i]);
                }
                c_write(')');
            }
            else if (sym->name == _I("va_arg")) {
                assert(expr->call.num_args == 2);
                gen_expr(expr->call.args[1]);
                c_write(' ');
                c_write('=');
                c_write(' ');
                c_write(sym->name);
                c_write('(');
                gen_expr(expr->call.args[0]);
                type = get_resolved_type(expr->call.args[1]);
                c_write(',');
                c_write(' ');
                type_to_cdecl(type, null);
                c_write(')');
            }
            else if (sym->name == _I("apush") || sym->name == _I("aputv") || sym->name == _I("adelv") ||
              sym->name == _I("agetvi") || sym->name == _I("agetvp") || sym->name == _I("agetv") ||
              sym->name == _I("asetcap") || sym->name == _I("afit") || sym->name == _I("acat") ||
              sym->name == _I("adeli") || sym->name == _I("aindexv") || sym->name == _I("asetlen") ||
              sym->name == _I("ainit")) {
                // (t, a, v)
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("adefault")) {
                // (t, tv, a, v)
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                type_to_cdecl(val, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("afill")) {
                // (t, a, v, n)
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[2]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("acatn") || sym->name == _I("adeln")) {
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[2]);
                c_write(')');c_write(')');
            }
            else if (sym->name == _I("aindex") || sym->name == _I("ageti") || sym->name == _I("adel")) {
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                type_to_cdecl(key, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("agetp") || sym->name == _I("aget")) {
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                type_to_cdecl(key, null);
                c_write(',');
                c_write(' ');
                type_to_cdecl(val, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("aput")) {
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                type_to_cdecl(key, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[1]);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[2]);
                c_write(')');
                c_write(')');
            }
            else if (sym->name == _I("ahdrsize") || sym->name == _I("ahdralign") || sym->name == _I("ahdr") ||
              sym->name == _I("alen") || sym->name == _I("acap") || sym->name == _I("afree") ||
              sym->name == _I("aclear") || sym->name == _I("apop")) {
                c_write(sym->name);
                c_write('(');
                type_to_cdecl(@base, null);
                c_write(',');
                c_write(' ');
                c_write('(');
                gen_expr(expr->call.args[0]);
                c_write(')');
                c_write(')');
            }
            else {
                fatal_error(expr->pos, "Call to unimplemented intrinsic {0}", _S(sym->name));
            }
        }

        static  readonly char* generic_alloc_copy = "generic_alloc_copy".ToPtr();
        readonly char* alloc_copy = generic_alloc_copy + 8;
         readonly char* allocator_cast = "(Allocator *)".ToPtr();
        void gen_expr_new(Expr* expr) {
            assert(expr->kind == EXPR_NEW);
            Type *type = get_resolved_type(expr);
            assert(is_ptr_type(type));
            if (expr->new_expr.alloc != null) {
                c_write('(');
                c_write('(');
                type_to_cdecl(type, null);
                c_write(')');
                c_write(generic_alloc_copy, 18);
                if (expr->new_expr.len != null) {
                    c_write('n');
                    c_write('(');
                    c_write(allocator_cast, 13);
                    c_write('(');
                    gen_expr(expr->new_expr.alloc);
                    c_write(')');
                    c_write(',');
                    c_write(' ');
                    gen_expr(expr->new_expr.len);
      
                }
                else {
                    c_write('(');
                    c_write(allocator_cast, 13);
                    c_write('(');
                    gen_expr(expr->new_expr.alloc);
                    c_write(')');
                }
                c_write(',');
                c_write(' ');
                c_write(sizeof_keyword, 6);
                c_write('(');
                type_to_cdecl(type->@base, null);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write(alignof_keyword, 7);
                c_write('(');
                type_to_cdecl(type->@base, null);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('&');
                c_write('(');
            }
            else {
                c_write('(');
                c_write('(');
                type_to_cdecl(type, null);
                c_write(')');
                c_write(alloc_copy, 10);
                if (expr->new_expr.len != null) {
                    c_write('n');
                    c_write('(');
                    gen_expr(expr->new_expr.len);
                    c_write(',');
                    c_write(' ');
                }
                else {
                    c_write('(');
                }
                c_write(sizeof_keyword, 6);
                c_write('(');
                type_to_cdecl(type->@base, null);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write(alignof_keyword, 7);
                c_write('(');
                type_to_cdecl(type->@base, null);
                c_write(')');
                c_write(',');
                c_write(' ');
                c_write('&');
                c_write('(');
            }
            gen_expr(expr->new_expr.arg);
            c_write(')');
            c_write(')');
            c_write(')');
        }


        void gen_expr_compound(Expr* expr) {
            Type *expected_type = get_resolved_expected_type(expr);

            if (expected_type != null && !is_ptr_type(expected_type)) {
                c_write('{');
            }
            else if (expr->compound.type != null) {
                c_write('(');
                typespec_to_cdecl(expr->compound.type, null);
                c_write(')');
                c_write('{');
            }
            else {
                c_write('(');
                type_to_cdecl(get_resolved_type(expr), null);
                c_write(')');
                c_write('{');
            }
            for (int i = 0; i < expr->compound.num_fields; i++) {
                if (i != 0) {
                    c_write(',');
                    c_write(' ');
                }
                CompoundField field = expr->compound.fields[i];
                if (field.kind == FIELD_NAME) {
                    c_write('.');
                    c_write(field.name);
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                }
                else if (field.kind == FIELD_INDEX) {
                    c_write('[');
                    gen_expr(field.index);
                    c_write(']');
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                }

                gen_expr(field.init);
            }
            if (expr->compound.num_fields == 0) {
                c_write('0');
            }
            c_write('}');
        }

        void gen_char(char c) {
            c_write('\'');
            if (char_to_escape[c] != 0) {
                c_write('\\');
                c_write(char_to_escape[c]);
            }
            else if (isprint(c)) {
                c_write(c);
            }
            else {
                c_write('\\');
                c_write('x');
                c_write(((int)c).itoa(16));
            }
            c_write('\'');
        }

        void gen_expr(Expr* expr) {
            Type *type = null;
            Type *conv = type_conv(expr);
            if (conv != null) {
                c_write('(');
                type_to_cdecl(conv, null);
                c_write(')');
                c_write('(');
            }
            bool gen_any = is_implicit_any(expr);
            if (gen_any) {
                type = get_resolved_type(expr);
                c_write("(any){(");
                type_to_cdecl(type, null);
                c_write("[]){");
            }

            switch (expr->kind) {
                case EXPR_PAREN:
                    c_write('(');
                    gen_expr(expr->paren.expr);
                    c_write(')');
                    break;
                case EXPR_INT: {
                    char *suffix_name = token_suffix_names[(int)expr->int_lit.suffix];
                    switch (expr->int_lit.mod) {
                        case MOD_BIN:
                        case MOD_HEX:
                            c_write('0');
                            c_write('x');
                            c_write(expr->int_lit.val.itoa(16));
                            c_write(suffix_name);
                            break;
                        case MOD_OCT:
                            c_write('0');
                            c_write(expr->int_lit.val.itoa(8));
                            c_write(suffix_name);
                            break;
                        case MOD_CHAR:
                            gen_char((char)expr->int_lit.val);
                            break;
                        default:
                            c_write(expr->int_lit.val.itoa());
                            c_write(suffix_name);
                            break;
                    }
                }
                break;
                case EXPR_FLOAT:
                    bool is_double = expr->type != null && expr->type->kind == TYPE_DOUBLE;
                    int len = (int)(expr->float_lit.end - expr->float_lit.start);
                    if ((is_double && expr->float_lit.suffix == SUFFIX_D) || expr->float_lit.suffix == SUFFIX_D) len--; // remove prefix 'd' from literal
                    c_write(expr->float_lit.start, len);
                    if (!is_double)
                        c_write('f');
                    break;
                case EXPR_STR:
                    gen_str(expr->str_lit.val, expr->str_lit.mod == MOD_MULTILINE);
                    break;
                case EXPR_NAME:
                    c_write(get_gen_name_or_default(expr, expr->name));
                    break;
                case EXPR_CAST:
                    c_write('(');
                    typespec_to_cdecl(expr->cast.type, null);
                    c_write(')');
                    c_write('(');
                    gen_expr(expr->cast.expr);
                    c_write(')');
                    break;
                case EXPR_CALL:
                    Sym *sym = get_resolved_sym(expr->call.expr);
                    if (is_intrinsic(sym)) {
                        gen_intrinsic(sym, expr);
                    }
                    else {
                        if (sym != null && sym->kind == SymKind.SYM_TYPE) {
                            c_write('(');
                            c_write(get_gen_name(sym));
                            c_write(')');
                        }
                        else
                            gen_expr(expr->call.expr);
                        c_write('(');
                        for (var i = 0; i < expr->call.num_args; i++) {
                            if (i != 0) {
                                c_write(',');
                                c_write(' ');
                            }

                            gen_expr(expr->call.args[i]);
                        }

                        c_write(')');
                    }
                    break;
                case EXPR_INDEX: {
                    type = unqualify_type(get_resolved_type(expr->index.expr));
                    if (is_aggregate_type(type)) {
                        gen_expr(expr->index.expr);
                        c_write('.');
                        long i = get_resolved_val(expr->index.index).ll;
                        c_write(type->aggregate.fields[i].name);
                    }
                    else {
                        gen_expr(expr->index.expr);
                        c_write('[');
                        gen_expr(expr->index.index);
                        c_write(']');
                    }
                    break;
                }
                case EXPR_FIELD: {
                    Sym* sym1 = get_resolved_sym(expr);
                    if (sym1 != null) {
                        c_write('(');
                        c_write(get_gen_name(sym1));
                        c_write(')');
                    }
                    else {
                        char *name = expr->field.name;
                        type = unqualify_type(get_resolved_type(expr->field.expr));
                        gen_expr(expr->field.expr);
                        if (type->kind == TYPE_PTR) {

                            c_write('-');
                            c_write('>');
                        }
                        else
                            c_write('.');

                        c_write(name);
                    }
              
                    break;
                }
                case EXPR_COMPOUND:
                    gen_expr_compound(expr);
                    break;
                case EXPR_UNARY:
                    c_write(_token_kind_name(expr->unary.op));
                    c_write('(');
                    gen_expr(expr->unary.expr);
                    c_write(')');
                    break;
                case EXPR_BINARY:
                    c_write('(');
                    Type *left_promo = pointer_promo_type(expr->binary.left);
                    if (left_promo != null) {
                        c_write('(');
                        type_to_cdecl(left_promo, null);
                        c_write(')');
                    }
                    gen_expr(expr->binary.left);
                    c_write(')');
                    c_write(' ');
                    c_write(_token_kind_name(expr->binary.op));
                    c_write(' ');
                    c_write('(');
                    Type *right_promo = pointer_promo_type(expr->binary.right);
                    if (right_promo != null) {
                        c_write('(');
                        type_to_cdecl(right_promo, null);
                        c_write(')');
                    }
                    gen_expr(expr->binary.right);
                    c_write(')');
                    break;
                case EXPR_TERNARY:
                    c_write('(');
                    gen_expr(expr->ternary.cond);
                    c_write(' ');
                    c_write('?');
                    c_write(' ');
                    gen_expr(expr->ternary.then_expr);
                    c_write(' ');
                    c_write(':');
                    c_write(' ');
                    gen_expr(expr->ternary.else_expr);
                    c_write(')');
                    break;
                case EXPR_SIZEOF_EXPR:
                    c_write(sizeof_keyword);
                    c_write('(');
                    gen_expr(expr->sizeof_expr);
                    c_write(')');
                    break;
                case EXPR_SIZEOF_TYPE:
                    c_write(sizeof_keyword);
                    c_write('(');
                    typespec_to_cdecl(expr->sizeof_type, null);
                    c_write(')');
                    break;
                case EXPR_ALIGNOF_EXPR:
                    c_write(alignof_keyword);
                    c_write('(');
                    type_to_cdecl(get_resolved_type(expr->alignof_expr), null);
                    c_write(')');
                    break;
                case EXPR_ALIGNOF_TYPE:
                    c_write(alignof_keyword);
                    c_write('(');
                    typespec_to_cdecl(expr->alignof_type, null);
                    c_write(')');
                    break;
                case EXPR_TYPEOF_EXPR: {
                    type = get_resolved_type(expr->typeof_expr);
                    assert(type->typeid);
                    gen_typeid(type);
                    break;
                }
                case EXPR_TYPEOF_TYPE: {
                    type = get_resolved_type(expr->typeof_type);
                    assert(type->typeid);
                    gen_typeid(type);
                    break;
                }
                case EXPR_OFFSETOF: {
                    c_write(offsetof_keyword);
                    c_write('(');
                    typespec_to_cdecl(expr->offsetof_field.type, null);
                    c_write(',');
                    c_write(' ');
                    c_write(expr->offsetof_field.name);
                    c_write(')');
                    break;
                }
                case EXPR_MODIFY:
                    if (!expr->modify.post) {
                        c_write(_token_kind_name(expr->modify.op));
                    }
                    gen_paren_expr(expr->modify.expr);
                    if (expr->modify.post) {
                        c_write(_token_kind_name(expr->modify.op));
                    }
                    break;
                case EXPR_NEW:
                    gen_expr_new(expr);
                    break;
                default:
                    assert(false);
                    break;
            }
            if (gen_any) {
                c_write('}');
                c_write(',');
                c_write(' ');
                gen_typeid(type);
                c_write('}');
            }
            if (conv != null) {
                c_write(')');
            }
        }

        bool is_incomplete_array_typespec(Typespec* typespec) {
            return typespec->kind == TYPESPEC_ARRAY && typespec->num_elems == null;
        }

        void gen_stmt_block(StmtList block) {
            c_write('{');
            gen_indent++;
            for (var i = 0; i < block.num_stmts; i++)
                gen_stmt(block.stmts[i]);
            gen_indent--;
            genln();
            c_write('}');
        }

        void gen_paren_expr(Expr* expr) {
            c_write('(');
            gen_expr(expr);
            c_write(')');
        }

        void gen_simple_stmt(Stmt* stmt) {
            switch (stmt->kind) {
                case STMT_EXPR:
                    gen_expr(stmt->expr);
                    break;
                case STMT_INIT:
                    if (stmt->init.type != null) {
                        Typespec *init_typespec = stmt->init.type;
                        bool incomplete = is_incomplete_array_typespec(stmt->init.type);
                        if (incomplete && stmt->init.expr == null) {
                            Type *init_type = get_resolved_type(stmt->init.type);
                            type_to_cdecl(type_decay(init_type), stmt->init.name);
                            c_write(" = 0");
                        }
                        else {
                            if (incomplete) {
                                Expr *size = new_expr_int(init_typespec->pos, (ulong)get_resolved_type(stmt->init.expr)->num_elems, 0, 0);
                                init_typespec = new_typespec_array(init_typespec->pos, init_typespec->@base, size);
                            }

                            typespec_to_cdecl(stmt->init.type, stmt->init.name);
                            c_write(' ');
                            c_write('=');
                            c_write(' ');
                            if (stmt->init.expr != null) {
                                gen_expr(stmt->init.expr);
                            }
                            else {
                                c_write('{');
                                c_write('0');
                                c_write('}');
                            }
                        }
                    }
                    else {
                        type_to_cdecl(unqualify_type(get_resolved_type(stmt->init.expr)), stmt->init.name);
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_expr(stmt->init.expr);
                    }
                    break;
                case STMT_ASSIGN:
                    Type *promo_type = pointer_promo_type(stmt->assign.left);
                    if (promo_type != null) {
                        assert(stmt->assign.op == TokenKind.TOKEN_ADD_ASSIGN);
                        Type *left_type = get_resolved_type(stmt->assign.left);
                        if (stmt->assign.left->kind == EXPR_NAME) {
                            char *name = get_gen_name_or_default(stmt->assign.left, stmt->assign.left->name);
                            c_write(name);
                            c_write(" = (char *)(");
                            c_write(name);
                            c_write(") + ");
                            gen_expr(stmt->assign.right);
                        }
                        else {
                            // TODO: this is an ugly codegen template that needs to avoid both illegal aliasing and multiple evaluation.
                            // However, 99.9% of use cases will use a name on the left-hand side, and we handle that cleanly.
                            c_write("do { ");
                            type_to_cdecl(type_ptr(left_type), "__pp".ToPtr());
                            c_write(" = (");
                            type_to_cdecl(type_ptr(left_type), null);
                            c_write(")&(");
                            gen_expr(stmt->assign.left);
                            c_write("); *__pp = (");
                            type_to_cdecl(left_type, null);
                            c_write(")(*(char **)__pp + ");
                            gen_expr(stmt->assign.right);
                            c_write("); } while(0)");
                        }
                    }
                    else {
                        gen_expr(stmt->assign.left);
                        c_write(' ');
                        c_write(_token_kind_name(stmt->assign.op));
                        c_write(' ');
                        gen_expr(stmt->assign.right);
                    }
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void gen_stmt(Stmt* stmt) {
            gen_sync_pos(stmt->pos);
            switch (stmt->kind) {
                case STMT_RETURN:
                    genlnf(return_keyword);
                    if (stmt->expr != null) {
                        c_write(' ');
                        gen_expr(stmt->expr);
                    }

                    c_write(';');
                    break;
                case STMT_BREAK:
                    genlnf(break_keyword);
                    c_write(';');
                    break;
                case STMT_CONTINUE:
                    genlnf(continue_keyword);
                    c_write(';');
                    break;
                case STMT_BLOCK:
                    genln();
                    gen_stmt_block(stmt->block);
                    break;
                case STMT_NOTE: {
                    Note note = stmt->note;
                    if (note.name == assert_name) {
                        genlnf(assert_name);
                        c_write('(');
                        assert(note.num_args == 1);
                        gen_expr(note.args[0].expr);
                        c_write(')');
                        c_write(';');
                    }
                    else if (note.name == foreign_name) {
                        char *preamble_name = _I("preamble");
                        char *postamble_name = _I("postamble");
                        for (var i = 0; i < note.num_args; i++) {
                            char *name = note.args[i].name;
                            Expr *expr = note.args[i].expr;
                            if (expr->kind != EXPR_STR) {
                                fatal_error(expr->pos, "#foreign argument must be a string");
                            }
                            char *str = expr->str_lit.val;
                            if (name == preamble_name) {
                                gen_buf_pos(ref gen_preamble_buf, note.args[i].pos);
                                gen_preamble_buf.Append(str, strlen(str));
                                gen_preamble_buf.Append('\n');
                            }
                            else if (name == postamble_name) {
                                gen_buf_pos(ref gen_postamble_buf, note.args[i].pos);
                                gen_postamble_buf.Append(str, strlen(str));
                                gen_postamble_buf.Append('\n');
                            }
                        }
                    }
                    break;
                }
                case STMT_LABEL: {
                    var indent = gen_indent;
                    gen_indent = 0;
                    genlnf(stmt->label);
                    c_write(':');
                    gen_indent = indent;
                    break;
                }
                case STMT_GOTO:
                    genlnf(goto_keyword, 4);
                    c_write(' ');
                    c_write(stmt->label);
                    c_write(';');
                    break;
                case STMT_IF:
                    if (stmt->if_stmt.init != null) {
                        genlnf('{');
                        gen_indent++;
                        gen_stmt(stmt->if_stmt.init);
                    }
                    genlnf(if_keyword);
                    c_write(' ');
                    c_write('(');
                    if (stmt->if_stmt.cond != null) {
                        gen_expr(stmt->if_stmt.cond);
                    }
                    else {
                        c_write(stmt->if_stmt.init->init.name);
                    }
                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->if_stmt.then_block);
                    for (var i = 0; i < stmt->if_stmt.num_elseifs; i++) {
                        var elseif = stmt->if_stmt.elseifs[i];
                        c_write(' ');
                        c_write(else_keyword);
                        c_write(' ');
                        c_write(if_keyword);
                        c_write(' ');
                        c_write('(');
                        gen_expr(elseif->cond);
                        c_write(')');
                        c_write(' ');
                        gen_stmt_block(elseif->block);
                    }

                    if (stmt->if_stmt.else_block.stmts != null) {
                        c_write(' ');
                        c_write(else_keyword);
                        c_write(' ');
                        gen_stmt_block(stmt->if_stmt.else_block);
                    }
                    else {
                        Note *complete_note = get_stmt_note(stmt, complete_name);
                        if (complete_note != null) {
                            c_write(' ');
                            c_write(else_keyword);
                            c_write(' ');
                            c_write('{');
                            gen_indent++;
                            gen_sync_pos(complete_note->pos);
                            genlnf("assert(\"@complete if/elseif chain failed to handle case\" && 0);".ToPtr());
                            gen_indent--;
                            genlnf('}');
                        }
                    }

                    if (stmt->if_stmt.init != null) {
                        gen_indent--;
                        genlnf('}');
                    }
                    break;
                case STMT_WHILE:
                    genlnf(while_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->while_stmt.cond);
                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->while_stmt.block);
                    break;
                case STMT_DO_WHILE:
                    genlnf(do_keyword);
                    c_write(' ');
                    gen_stmt_block(stmt->while_stmt.block);
                    c_write(' ');
                    c_write(while_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->while_stmt.cond);
                    c_write(')');
                    c_write(';');
                    break;
                case STMT_FOR:
                    genlnf(for_keyword);
                    c_write(' ');
                    c_write('(');
                    if (stmt->for_stmt.init != null)
                        gen_simple_stmt(stmt->for_stmt.init);
                    c_write(';');
                    if (stmt->for_stmt.cond != null) {
                        c_write(' ');
                        gen_expr(stmt->for_stmt.cond);
                    }

                    c_write(';');
                    if (stmt->for_stmt.next != null) {
                        c_write(' ');
                        gen_simple_stmt(stmt->for_stmt.next);
                    }

                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->for_stmt.block);
                    break;
                case STMT_SWITCH: {
                    genlnf(switch_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->switch_stmt.expr);
                    c_write(')');
                    c_write(' ');
                    c_write('{');
                    bool has_default = false;
                    gen_indent++;
                    for (var i = 0; i < stmt->switch_stmt.num_cases; i++) {
                        var switch_case = stmt->switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_patterns; j++) {
                            SwitchCasePattern pattern = switch_case.patterns[j];
                            if (pattern.end != null) {
                                Val start_val = get_resolved_val(pattern.start);
                                Val end_val = get_resolved_val(pattern.end);
                                if (is_char_lit(pattern.start) && is_char_lit(pattern.end)) {
                                    genln();
                                    for (int c = (int)start_val.ll; c <= (int)end_val.ll; c++) {
                                        genlnf(case_keyword);
                                        c_write(' ');
                                        gen_char((char)c);
                                        c_write(':');
                                        c_write(' ');
                                    }
                                }
                                else {
                                    c_write('/');
                                    c_write('/');
                                    c_write(' ');
                                    gen_expr(pattern.start);
                                    c_write('.');
                                    c_write('.');
                                    c_write('.');
                                    gen_expr(pattern.end);
                                    genln();
                                    for (long ll = start_val.ll; ll <= end_val.ll; ll++) {
                                        genlnf(case_keyword);
                                        c_write(' ');
                                        c_write(ll.itoa());
                                        c_write(':');
                                        c_write(' ');
                                    }
                                }
                            }
                            else {
                                genlnf(case_keyword);
                                c_write(' ');
                                gen_expr(pattern.start);
                                c_write(':');
                            }
                        }
                        if (switch_case.is_default) {
                            genln();
                            has_default = true;
                            c_write(default_keyword);
                            c_write(':');
                        }

                        c_write(' ');
                        c_write('{');
                        gen_indent++;
                        {
                            StmtList block = switch_case.block;
                            int j;
                            for (j = 0; j < block.num_stmts; j++) {
                                gen_stmt(block.stmts[j]);
                            }
                            if (block.num_stmts == 0 || block.stmts[j-1]->kind != STMT_BREAK) {
                                genlnf(break_keyword, 5);
                                c_write(';');
                            }
                        }
                        gen_indent--;
                        genlnf('}');
                    }
                    if (!has_default) {
                        Note *note = get_stmt_note(stmt, complete_name);
                        if (note != null) {
                            c_write(default_keyword);
                            c_write(':');
                            gen_indent++;
                            genlnf("assert(\"@complete switch failed to handle case\" && 0);".ToPtr());
                            genln();
                            c_write(break_keyword);
                            c_write(';');
                            gen_indent--;
                        }
                    }
                    gen_indent--;
                    genln();
                    c_write('}');
                    break;
                }
                default:
                    genln();
                    gen_simple_stmt(stmt);
                    c_write(';');
                    break;
            }
        }

         bool is_char_lit(Expr* expr) {
            return expr->kind == EXPR_INT && expr->int_lit.mod == MOD_CHAR;
        }

        void gen_decl(Sym* sym) {
            var decl = sym->decl;
            if (decl == null || is_decl_foreign(decl))
                return;
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                gen_sync_pos(decl->pos);
            switch (decl->kind) {
                case DECL_CONST:
                    genln();
                    c_write(defineStr, 8);
                    c_write(get_gen_name(sym));
                    c_write(' ');
                    c_write('(');
                    if (decl->const_decl.type != null) {
                        c_write('(');
                        typespec_to_cdecl(decl->const_decl.type, null);
                        c_write(')');
                        c_write('(');
                    }
                    gen_expr(decl->const_decl.expr);
                    if (decl->const_decl.type != null)
                        c_write(')');

                    c_write(')');
                    break;
                case DECL_VAR:
                    if (is_decl_threadlocal(decl)) {
                        genlnf("THREADLOCAL");
                    }
                    genlnf(externStr, 6);
                    c_write(' ');
                    if (decl->var.type != null && !is_incomplete_array_typespec(decl->var.type)) {
                        typespec_to_cdecl(decl->var.type, get_gen_name(sym));
                    }
                    else {
                        type_to_cdecl(sym->type, get_gen_name(sym));
                    }
                    c_write(';');
                    break;
                case DECL_FUNC:
                    gen_func_decl(decl);
                    c_write(';');
                    break;
                case DECL_STRUCT:
                case DECL_UNION:
                    gen_aggregate(decl);
                    break;
                case DECL_ENUM:
                    genln();
                    c_write(typedef_keyword, 7);
                    c_write(' ');
                    if (decl->enum_decl.type != null) {
                        typespec_to_cdecl(decl->enum_decl.type, get_gen_name(decl));
                    }
                    else {
                        c_write(type_names[(int)TYPE_INT], 3);
                        c_write(' ');
                        c_write(get_gen_name(sym));
                    }
                    c_write(';');
                    break;
                case DECL_TYPEDEF:
                    genln();
                    c_write(typedef_keyword);
                    c_write(' ');
                    typespec_to_cdecl(decl->typedef_decl.type, get_gen_name(sym));
                    c_write(';');
                    break;
                case DECL_IMPORT:
                    // Do nothing
                    break;
                default:
                    assert(false);
                    break;
            }
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                genln();
        }

        void gen_sorted_decls() {
            genln();
            for (int i = 0; i < tuple_types->count; i++) {
                Type *type = tuple_types->Get<Type>(i);
                if (!is_tuple_reachable(type))
                    continue;
                
                char* id = type->typeid.itoa();
                genlnf("struct tuple");
                c_write(id);
                c_write(' ');
                c_write('{');
                gen_indent++;
                for (var j = 0; j < type->aggregate.num_fields; j++) {
                    TypeField field = type->aggregate.fields[j];
                    type_to_cdecl(field.type, field.name);
                    c_write(';');
                }
                gen_indent--;
                c_write('}');
                c_write(';');
            }
            genln();
            var sym_cnt = sorted_syms->count;

            for (var i = 0; i < sym_cnt; i++) {
                var sym = *(Sym**) (sorted_syms->_begin + i);
                if (sym->reachable == REACHABLE_NATURAL) {
                    gen_decl(sym);
                }
            }
        }
         Map gen_foreign_headers_map;
         PtrBuffer* gen_foreign_headers_buf = PtrBuffer.Create();
         PtrBuffer* gen_foreign_sources_buf = PtrBuffer.Create();
         PtrBuffer* gen_sources_buf = PtrBuffer.Create();


         void add_foreign_header(char* name) {
            name = _I(name);
            if (!gen_foreign_headers_map.exists(name)) {
                gen_foreign_headers_map.map_put(name, (void*)1);
                gen_foreign_headers_buf->Add(name);
            }
        }


         void add_foreign_source(char* name) {
            gen_foreign_sources_buf->Add(_I(name));
        }

        void gen_include(char* path) {
            genlnf(includeStr);
            if (*path == '<') {
                c_write(path);
            }
            else {
                gen_str(path, false);
            }
        }

        void gen_foreign_headers() {
            if (gen_foreign_headers_buf->count > 0) {
                for (var i = 0; i < gen_foreign_headers_buf->count; i++) {
                    gen_include(gen_foreign_headers_buf->Get<char>(i));
                }
            }
        }

         readonly char *header_name = _I("header");
         readonly char *source_name = _I("source");
         readonly char *preamble_name = _I("preamble");
         readonly char *postamble_name = _I("postamble");

        void preprocess_package(Package* package) {
            if (package->external_name == null) {
                char *external_name = stackalloc char[256];
                int i = 0;
                for (char *ptr = package->path; *ptr != '\0'; ptr++, i++) {
                    if (*ptr == '/')
                        external_name[i] = '_';
                    else
                        external_name[i] = *ptr;
                }
                external_name[i] = '_';
                package->external_name = _I(external_name);
            }
            for (var i = 0; i < package->num_decls; i++) {
                Decl *decl = package->decls[i];
                if (decl->kind != DECL_NOTE) {
                    continue;
                }
                Note note = decl->note;
                if (note.name == foreign_name) {
                    for (var j = 0; j < note.num_args; j++) {
                        NoteArg arg = note.args[j];
                        Expr *expr = note.args[j].expr;
                        if (expr->kind != EXPR_STR) {
                            fatal_error(decl->pos, "#foreign argument must be a string");
                        }
                        char *str = expr->str_lit.val;
                        if (arg.name == header_name) {
                            char* path = stackalloc char[256];
                            put_include_path(path, package, str);
                            add_foreign_header(path);
                        }
                        else if (arg.name == source_name) {
                            char* path = stackalloc char[256];
                            put_include_path(path, package, str);
                            add_foreign_source(path);
                        }
                        else if (arg.name == preamble_name) {
                            gen_buf_pos(ref gen_preamble_buf, arg.pos);
                            gen_preamble_buf.Append(str, strlen(str));
                            gen_preamble_buf.AppendLine();
                        }
                        else if (arg.name == postamble_name) {
                            gen_buf_pos(ref gen_postamble_buf, arg.pos);
                            gen_postamble_buf.Append(str, strlen(str));
                            gen_postamble_buf.AppendLine();
                        }
                        else {
                            fatal_error(decl->pos, "Unknown #foreign named argument '{0}'", _S(arg.name));
                        }
                    }
                }
            }
        }

        void preprocess_packages() {
            for (var i = 0; i < package_list->count; i++) {
                preprocess_package(package_list->Get<Package>(i));
            }
        }
        void gen_foreign_sources() {
            for (var i = 0; i < gen_foreign_sources_buf->count; i++) {
                gen_include(gen_foreign_sources_buf->Get<char>(i));
            }
        }

        void gen_preamble() {
            genln();
            if (gen_preamble_buf.Length > 0) {
                genlnf(gen_preamble_buf);
            }
        }

        void gen_postamble() {
            genln();
            if (gen_postamble_buf.Length > 0) {
                genlnf(gen_postamble_buf);
            }
        }

        void put_include_path(char* path, Package* package, char* filename) {
            if (*filename == '<') {
                strcpy(path, filename);
            } else {
                strcpy(path, package->full_path);
                path_join(path, filename);
                path_absolute(ref path);
            }
        }

        void path_absolute(ref char* path) {
            path = Path.GetFullPath(_S(path)).ToPtr();
        }


        char*[] tiInfo;
        void gen_typeinfos() {
            tiInfo = new[] {"TypeInfo *typeinfo_table[".ToPtr(out int ti0),
                          "] = {".ToPtr(out int ti1),
                          "int num_typeinfos = ".ToPtr(out int ti2),
                          "TypeInfo **typeinfos = (TypeInfo **)typeinfo_table;".ToPtr(out int ti3),
                          "NULL, // No associated type".ToPtr(out int ti4),
                          "&(TypeInfo){TYPE_VOID, .name = \"void\", .size = 0, .align = 0},".ToPtr(out int ti5),
                          "&(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = ".ToPtr(out int ti6),
                          ", .base = ".ToPtr(out int ti7),
                          "NULL, // Incomplete array type".ToPtr(out int ti8),
                          ", .base = ".ToPtr(out int ti9),
                          ", .count = ".ToPtr(out int ti10),
                          ", .name = ".ToPtr(out int ti11),
                          ", .num_fields = ".ToPtr(out int ti12),
                          ".fields = (TypeFieldInfo[]) {".ToPtr(out int ti13),
                          "NULL, // Func".ToPtr(out int ti14),
                          "NULL, // Enum".ToPtr(out int ti15),
                          "NULL, // Incomplete: ".ToPtr(out int ti16),
                          "NULL, // Unhandled".ToPtr(out int ti17),

                          ", .type = ".ToPtr(out int ti18),
                          ", .offset = offsetof(".ToPtr(out int ti19),

                          "&(TypeInfo){".ToPtr(out int ti20),
                          ", .size = sizeof(".ToPtr(out int ti21),
                          "), .align = alignof(".ToPtr(out int ti22),
                          ", .size = 0, .align = 0".ToPtr(out int ti23),
                          "#define TYPEID0(index, kind) ((ullong)(index) | ((ullong)(kind) << 24))".ToPtr(out int ti24),
                          "#define TYPEID(index, kind, ...) ((ullong)(index) | ((ullong)sizeof(__VA_ARGS__) << 32) | ((ullong)(kind) << 24))".ToPtr(out int ti25),
            };
            genlnf(tiInfo[24], ti24);
            genlnf(tiInfo[25], ti25);
            genln();

            if (flag_notypeinfo) {
                genln();
                c_write("int num_typeinfos;");
                genln();
                c_write("const TypeInfo **typeinfos;");
            }
            else {
                uint num_typeinfos = next_typeid;
                genln();
                c_write(tiInfo[0], ti0);
                c_write(num_typeinfos.itoa());
                c_write(tiInfo[1], ti1);
                gen_indent++;
                for (int typeid = 0; typeid < num_typeinfos; typeid++) {
                    genln();
                    c_write('[');
                    c_write(typeid.itoa());
                    c_write(']');
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                    Type *type = get_type_from_typeid(typeid);
                    if (type != null && !is_excluded_typeinfo(type)) {
                        gen_typeinfo(type);
                    }
                    else {
                        c_write(tiInfo[4], ti4);
                    }
                }
                gen_indent--;
                genln();
                c_write('}');
                c_write(';');
                genln();
                c_write(tiInfo[2], ti2);
                c_write(num_typeinfos.itoa());
                c_write(';');
                genln();
                c_write(tiInfo[3], ti3);
                genln();
            }
            void gen_typeinfo_header(char* kind, Type* type) {
                if (type_sizeof(type) == 0) {
                    c_write(tiInfo[20], ti20);
                    c_write(kind);
                    c_write(tiInfo[23], ti23);
                }
                else {
                    c_write(tiInfo[20], ti20);
                    c_write(kind);
                    c_write(tiInfo[21], ti21);
                    type_to_cdecl(type, null);
                    c_write(tiInfo[22], ti22);
                    type_to_cdecl(type, null);
                    c_write(')');
                }
            }

            void gen_typeinfo_fields(Type* type) {
                gen_indent++;

                for (var i = 0; i < type->aggregate.num_fields; i++) {
                    TypeField field = type->aggregate.fields[i];
                    genln();
                    c_write('{');
                    gen_str(field.name, false);

                    c_write(tiInfo[18], ti18);
                    gen_typeid(field.type);
                    c_write(tiInfo[19], ti19);
                    c_write(get_gen_name(type->sym));
                    c_write(',');
                    c_write(' ');
                    c_write(field.name);
                    c_write(')');
                    c_write('}');
                    c_write(',');
                }
                gen_indent--;
            }

            void gen_type(TypeKind tk) {
                char* c = type_names[(int)tk];
                c_write(tiInfo[20], ti20);
                c_write(typeid_kind_names[(int)tk]);
                c_write(tiInfo[21], ti21);
                c_write(c);
                c_write(tiInfo[22], ti22);
                c_write(c);
                c_write(')');
                c_write(tiInfo[11], ti11);
                gen_str(c, false);
            }

            void gen_typeinfo(Type* type) {
                switch (type->kind) {
                    case TYPE_BOOL:
                        gen_type(TYPE_BOOL);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_CHAR:
                        gen_type(TYPE_CHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UCHAR:
                        gen_type(TYPE_UCHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SCHAR:
                        gen_type(TYPE_SCHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SHORT:
                        gen_type(TYPE_SHORT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_USHORT:
                        gen_type(TYPE_USHORT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_INT:
                        gen_type(TYPE_INT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UINT:
                        gen_type(TYPE_UINT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LONG:
                        gen_type(TYPE_LONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULONG:
                        gen_type(TYPE_ULONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LLONG:
                        gen_type(TYPE_LLONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULLONG:
                        gen_type(TYPE_ULLONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_FLOAT:
                        gen_type(TYPE_FLOAT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_DOUBLE:
                        gen_type(TYPE_DOUBLE);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_VOID:
                        c_write(tiInfo[5], ti5);
                        break;
                    case TYPE_PTR:
                        c_write(tiInfo[6], ti6);
                        gen_typeid(type->@base);
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_CONST:
                        gen_typeinfo_header(typeid_kind_names[(int)TYPE_CONST], type);
                        c_write(tiInfo[7], ti7);
                        gen_typeid(type->@base);
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_ARRAY:
                        if (is_incomplete_array_type(type)) {
                            c_write(tiInfo[8], ti8);
                        }
                        else {
                            gen_typeinfo_header(typeid_kind_names[(int)TYPE_ARRAY], type);
                            c_write(tiInfo[9], ti9);
                            gen_typeid(type->@base);
                            c_write(tiInfo[10], ti10);
                            c_write(type->num_elems.itoa());
                            c_write('}');
                            c_write(',');
                        }
                        break;
                    case TYPE_STRUCT:
                    case TYPE_UNION:
                        gen_typeinfo_header(typeid_kind_names[(int)type->kind], type);
                        c_write(tiInfo[11], ti11);
                        gen_str(get_gen_name(type->sym), false);
                        c_write(tiInfo[12], ti12);
                        c_write(type->aggregate.num_fields.itoa());
                        c_write(',');
                        gen_indent++;
                        genlnf(tiInfo[13], ti13);
                        gen_typeinfo_fields(type);
                        genln();
                        c_write('}');
                        gen_indent--;
                        genln();
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_FUNC:
                        c_write(tiInfo[14], ti14);
                        break;
                    case TYPE_ENUM:
                        c_write(tiInfo[15], ti15);
                        break;
                    case TYPE_INCOMPLETE:
                        c_write(tiInfo[16], ti16);
                        c_write(get_gen_name(type->sym));
                        break;
                    default:
                        c_write(tiInfo[17], ti17);
                        break;
                }
            }

        }

        void gen_all() {
            preprocess_packages();
            gen_foreign_headers();
            gen_forward_decls();
            gen_sorted_decls();
            gen_typeinfos();
            gen_defs();
            gen_foreign_sources();
            gen_postamble();

            var buf = gen_buf;
            gen_buf = new StringBuilder();
            gen_preamble();
            gen_buf.Append(buf);
        }

        void init_chars() {
            // TODO: Need to expand this and also deal with non-printable chars via \x
            char_to_escape['\0'] = '0';
            char_to_escape['\n'] = 'n';
            char_to_escape['\r'] = 'r';
            char_to_escape['\t'] = 't';
            char_to_escape['\v'] = 'v';
            char_to_escape['\b'] = 'b';
            char_to_escape['\a'] = 'a';
            char_to_escape['\\'] = '\\';
            char_to_escape['"'] = '"';
            char_to_escape['\''] = '\'';
        }
    }
}