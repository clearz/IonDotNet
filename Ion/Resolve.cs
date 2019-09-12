using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lang
{
    using static TypeKind;
    using static SymState;
    using static DeclKind;
    using static SymKind;
    using static TypespecKind;
    using static ExprKind;
    using static TokenKind;
    using static StmtKind;
    using static CompoundFieldKind;

    unsafe partial class Ion
    {
        public const int MAX_LOCAL_SYMS = 1024;
        private Map cached_ptr_types;
        private Map global_syms_map;
        private Buffer<CachedArrayType> cached_array_types = Buffer<CachedArrayType>.Create();
        private Buffer<CachedFuncType> cached_func_types = Buffer<CachedFuncType>.Create();
        private readonly PtrBuffer* global_syms_buf = PtrBuffer.Create();
        private Buffer<Sym> local_syms = Buffer<Sym>.Create(MAX_LOCAL_SYMS);

        private static readonly Type* type_void = basic_type_alloc(TYPE_VOID, 0, 1);
        private static readonly Type* type_bool = basic_type_alloc(TYPE_BOOL, 1, 1);
        private static readonly Type* type_char = basic_type_alloc(TYPE_CHAR, 2, 2);
        private static readonly Type* type_uchar = basic_type_alloc(TYPE_UCHAR, 1, 1);
        private static readonly Type* type_schar = basic_type_alloc(TYPE_SCHAR, 1, 1);
        private static readonly Type* type_short = basic_type_alloc(TYPE_SHORT, 2, 2);
        private static readonly Type* type_ushort = basic_type_alloc(TYPE_USHORT, 2, 2);
        private static readonly Type* type_int = basic_type_alloc(TYPE_INT, 4, 4);
        private static readonly Type* type_uint = basic_type_alloc(TYPE_UINT, 4, 4);
        private static readonly Type* type_long = basic_type_alloc(TYPE_LONG, 4, 4); // 4 on 64-bit windows, 8 on 64-bit linux, probably factor this out to the backend
        private static readonly Type* type_ulong = basic_type_alloc(TYPE_ULONG, 4, 4);
        private static readonly Type* type_llong = basic_type_alloc(TYPE_LLONG, 8, 8);
        private static readonly Type* type_ullong = basic_type_alloc(TYPE_ULLONG, 8, 8);
        private static readonly Type* type_float = basic_type_alloc(TYPE_FLOAT, 4, 4);
        private static readonly Type* type_double =basic_type_alloc(TYPE_DOUBLE, 8, 8);
        private static readonly Type* type_usize = type_ullong;
        private static readonly Type* type_ssize = type_llong;

#if X64
        internal const int PTR_SIZE = 8;
#else
        internal const int PTR_SIZE = 4;
#endif
        private const long PTR_ALIGN = 8;

        readonly int[] type_ranks = new int[(int)NUM_TYPE_KINDS];
        readonly char*[] type_names = new char*[(int)NUM_TYPE_KINDS];

        bool is_integer_type(Type* type) {
            return TYPE_BOOL <= type->kind && type->kind <= TYPE_ULLONG;
        }

        bool is_floating_type(Type* type) {
            return TYPE_FLOAT <= type->kind && type->kind <= TYPE_DOUBLE;
        }

        bool is_arithmetic_type(Type* type) {
            return TYPE_BOOL <= type->kind && type->kind <= TYPE_DOUBLE;
        }

        bool is_scalar_type(Type* type) {
            return TYPE_BOOL <= type->kind && type->kind <= TYPE_FUNC;
        }

        bool is_signed_type(Type* type) {
            switch (type->kind) {
                // TODO: TYPE_CHAR signedness is platform independent, needs to factor into backend
                case TYPE_SCHAR:
                case TYPE_SHORT:
                case TYPE_INT:
                case TYPE_LONG:
                case TYPE_LLONG:
                    return true;
                default:
                    return false;
            }
        }

        int type_rank(Type* type) {
            int rank = type_ranks[(int)type->kind];
            assert(rank != 0);
            return rank;
        }

        Type* unsigned_type(Type* type) {
            switch (type->kind) {
                case TYPE_BOOL:
                    return type_bool;
                case TYPE_CHAR:
                case TYPE_SCHAR:
                case TYPE_UCHAR:
                    return type_uchar;
                case TYPE_SHORT:
                case TYPE_USHORT:
                    return type_ushort;
                case TYPE_INT:
                case TYPE_UINT:
                    return type_uint;
                case TYPE_LONG:
                case TYPE_ULONG:
                    return type_ulong;
                case TYPE_LLONG:
                case TYPE_ULLONG:
                    return type_ullong;
                default:
                    assert(false);
                    return null;
            }
        }
        private static Type* type_alloc(TypeKind kind) {
            var type = (Type*) xmalloc(sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint)sizeof(Type));
            type->kind = kind;
            return type;
        }

        private static Type* basic_type_alloc(TypeKind kind, long size = 0, long align = 0) {
            var type = type_alloc(kind);
            type->size = size;
            type->align = align;
            return type;
        }

        private long type_sizeof(Type* type) {
            assert(type->kind > TYPE_COMPLETING);
            assert(type->size != 0);
            return type->size;
        }

        private long type_alignof(Type* type) {
            assert(type->kind > TYPE_COMPLETING);
            assert(IS_POW2(type->align));
            return type->align;
        }


        private Type* type_ptr(Type* elem) {
            var type = (Type*) cached_ptr_types.map_get(elem);
            if (type == null) {
                type = type_alloc(TYPE_PTR);
                type->size = PTR_SIZE;
                type->align = PTR_ALIGN;
                type->ptr.elem = elem;

                cached_ptr_types.map_put(elem, type);
            }

            return type;
        }


        private Type* type_array(Type* elem, long size) {
            for (var it = cached_array_types._begin; it != cached_array_types._top; it++)
                if (it->elem == elem && it->size == size)
                    return it->array;

            complete_type(elem);
            var type = type_alloc(TYPE_ARRAY);
            type->size = size * type_sizeof(elem);
            type->align = type_alignof(elem);
            type->array.elem = elem;
            type->array.size = size;
            cached_array_types.Add(new CachedArrayType { elem = elem, array = type, size = size });
            return type;
        }

        private Type* type_func(Type** @params, int num_params, Type* ret, bool variadic = false) {
            for (var it = cached_func_types._begin; it != cached_func_types._top; it++)
                if (it->num_params == num_params && it->ret == ret && it->variadic == variadic) {
                    var match = true;
                    for (var i = 0; i < num_params; i++)
                        if (it->@params[i] != @params[i]) {
                            match = false;
                            break;
                        }

                    if (match)
                        return it->func;
                }

            var type = type_alloc(TYPE_FUNC);
            type->size = PTR_SIZE;
            type->align = PTR_ALIGN;
            type->func.variadic = variadic;
            if (num_params > 0) {
                type->func.@params = (Type**)xmalloc(sizeof(Type*) * num_params);
                Unsafe.CopyBlock(type->func.@params, @params,
                    (uint)(num_params *
                            sizeof(Type*)));
                type->func.num_params = num_params;
            }

            type->func.ret = ret;
            cached_func_types.Add(new CachedFuncType { func = type, num_params = num_params, ret = ret, @params = type->func.@params, variadic = variadic });
            return type;
        }

        private Type* type_func(Type*[] params_a, int num_params, Type* ret, bool variadic = false) {
            fixed (Type** @params = params_a) {
                return type_func(@params, num_params, ret, variadic);
            }
        }

        // TODO: This probably shouldn't use an O(n^2) algorithm
        private bool duplicate_fields(TypeField* fields, long num_fields) {
            for (var i = 0; i < num_fields; i++)
                for (var j = i + 1; j < num_fields; j++)
                    if (fields[i].name == fields[j].name)
                        return true;
            return false;
        }

        private Type* type_complete_struct(Type* type, TypeField* fields, int num_fields) {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_STRUCT;
            type->size = 0;
            type->align = 0;

            for (var it = fields; it != fields + num_fields; it++) {
                assert(IS_POW2(type_alignof(it->type)));
                it->offset = type->size;
                type->size = type_sizeof(it->type) + ALIGN_UP(type->size, type_alignof(it->type));
                type->align = MAX(type->align, type_alignof(it->type));
            }

            type->aggregate.fields =
                (TypeField*)xmalloc(num_fields * sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint)(num_fields *
                        sizeof(TypeField)));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        private Type* type_complete_union(Type* type, TypeField* fields, int num_fields) {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_UNION;
            type->size = 0;
            for (var it = fields; it != fields + num_fields; it++) {
                assert(it->type->kind > TYPE_COMPLETING);
                it->offset = 0;
                type->size = MAX(type->size, it->type->size);
                type->align = MAX(type->align, type_alignof(it->type));
            }

            type->aggregate.fields =
                (TypeField*)xmalloc(sizeof(TypeField) * num_fields);
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint)(num_fields *
                        sizeof(TypeField)));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        private Type* type_incomplete(Sym* sym) {
            var type = type_alloc(TYPE_INCOMPLETE);
            type->sym = sym;
            return type;
        }


        private Sym* sym_new(SymKind kind, char* name, Decl* decl) {
            var sym = xmalloc<Sym>();

            sym->kind = kind;
            sym->name = name;
            sym->decl = decl;
            sym->state = 0;
            sym->val = default;
            return sym;
        }

        private Sym* sym_decl(Decl* decl) {
            var kind = SYM_NONE;
            switch (decl->kind) {
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

            var sym = sym_new(kind, decl->name, decl);
            if (decl->kind == DECL_STRUCT || decl->kind == DECL_UNION) {
                sym->state = SYM_RESOLVED;
                sym->type = type_incomplete(sym);
            }

            return sym;
        }

        private Sym* sym_enum_const(char* name, Decl* decl) {
            return sym_new(SYM_ENUM_CONST, name, decl);
        }

        private Sym* sym_get(char* name) {
            for (var sym = local_syms._top - 1; sym >= local_syms._begin; sym--)
                if (sym->name == name)
                    return sym;

            return (Sym*)global_syms_map.map_get(name);
        }

        private void sym_push_var(char* name, Type* type) {
            if (local_syms._top == local_syms._begin + MAX_LOCAL_SYMS)
                fatal("Too many local symbols");


            var sym = xmalloc<Sym>();
            sym->name = name;
            sym->kind = SYM_VAR;
            sym->state = SYM_RESOLVED;
            sym->type = type;
            *local_syms._top++ = *sym;
        }

        private Sym* sym_enter() {
            return local_syms._top;
        }

        private void sym_leave(Sym* sym) {
            local_syms._top = sym;
        }

        private void sym_global_put(Sym* sym) {
            global_syms_map.map_put(sym->name, sym);
            global_syms_buf->Add(sym);
        }

        private Sym* sym_global_decl(Decl* decl) {
            var sym = sym_decl(decl);
            sym_global_put(sym);
            decl->sym = sym;
            if (decl->kind == DECL_ENUM)
                for (var i = 0; i < decl->enum_decl.num_items; i++)
                    sym_global_put(sym_enum_const(decl->enum_decl.items[i].name, decl));
            return sym;
        }

        private void sym_global_type(char* name, Type* type) {
            var sym = sym_new(SYM_TYPE, _I(name), null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            sym_global_put(sym);
        }

        void sym_global_const(char* name, Type* type, Val val) {
            Sym *sym = sym_new(SYM_CONST, _I(name), null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            sym->val = val;
            sym_global_put(sym);
        }


        private void sym_global_func(char* name, Type* type) {
            assert(type->kind == TYPE_FUNC);
            var sym = sym_new(SYM_FUNC, _I(name), null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            sym_global_put(sym);
        }

        private Operand operand_rvalue(Type* type) {
            return new Operand { type = type };
        }

        private Operand operand_lvalue(Type* type) {
            return new Operand { type = type, is_lvalue = true };
        }

        private Operand operand_const(Type* type, Val val) {
            return new Operand { type = type, is_const = true, val = val };
        }

        bool is_convertible(Type* dest, Type* src) {
            if (dest == src) {
                return true;
            }
            else if (is_arithmetic_type(dest) && is_arithmetic_type(src)) {
                return true;
            }
            else if (dest->kind == TYPE_PTR && src->kind == TYPE_PTR) {
                return dest->ptr.elem == type_void || src->ptr.elem == type_void;
            }
            else {
                return false;
            }
        }




        int TKind(TypeKind kind) => 5;
        bool convert_operand(Operand* operand, Type* type) {
            // TODO: check for legal conversion

            if (operand->type == type) {
                return true;
            }
            if (!is_convertible(operand->type, type)) {
                return false;
            }
            if (operand->is_const) {
                if (is_floating_type(operand->type)) {
                    operand->is_const = !is_integer_type(type);
                }
                else {
                    switch (operand->type->kind) {
                        case TYPE_BOOL: {
                            var p = operand->val.b;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)(p ? 1 : 0);
                                    ;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)(p ? 1 : 0);
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)(p ? 1 : 0);
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)(p ? 1 : 0);
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)(p ? 1 : 0);
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)(p ? 1 : 0);
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)(p ? 1 : 0);
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)(p ? 1 : 0);
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)(p ? 1 : 0);
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)(p ? 1 : 0);
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)(p ? 1 : 0);
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_CHAR: {
                            var p = operand->val.c;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_UCHAR: {
                            var p = operand->val.uc;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_SCHAR: {
                            var p = operand->val.sc;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_SHORT: {
                            var p = operand->val.s;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_USHORT: {
                            var p = operand->val.us;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_INT: {
                            var p = operand->val.i;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_UINT: {
                            var p = operand->val.u;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_LONG: {
                            var p = operand->val.l;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_LLONG: {
                            var p = operand->val.ll;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        case TYPE_ULLONG: {
                            var p = operand->val.ull;
                            switch (type->kind) {
                                case TYPE_BOOL:
                                    operand->val.b = p != 0;
                                    break;
                                case TYPE_CHAR:
                                    operand->val.c = (char)p;
                                    break;
                                case TYPE_UCHAR:
                                    operand->val.uc = (byte)p;
                                    break;
                                case TYPE_SCHAR:
                                    operand->val.sc = (sbyte)p;
                                    break;
                                case TYPE_SHORT:
                                    operand->val.s = (short)p;
                                    break;
                                case TYPE_USHORT:
                                    operand->val.us = (ushort)p;
                                    break;
                                case TYPE_INT:
                                    operand->val.i = (int)p;
                                    break;
                                case TYPE_UINT:
                                    operand->val.u = (uint)p;
                                    break;
                                case TYPE_LONG:
                                    operand->val.l = (int)p;
                                    break;
                                case TYPE_ULONG:
                                    operand->val.ul = (uint)p;
                                    break;
                                case TYPE_LLONG:
                                    operand->val.ll = (long)p;
                                    break;
                                case TYPE_ULLONG:
                                    operand->val.ull = (ulong)p;
                                    break;
                                default:
                                    operand->is_const = false;
                                    break;
                            }

                            break;
                        }
                        default:
                            operand->is_const = false;
                            break;
                    }
                }
            }
            operand->type = type;
            return true;
        }

        Val convert_const(Type* dest_type, Type* src_type, Val src_val) {
            Operand operand = operand_const(src_type, src_val);
            convert_operand(&operand, dest_type);
            return operand.val;
        }

        void promote_operand(Operand* operand) {
            switch (operand->type->kind) {
                case TYPE_BOOL:
                case TYPE_CHAR:
                case TYPE_SCHAR:
                case TYPE_UCHAR:
                case TYPE_SHORT:
                case TYPE_USHORT:
                    convert_operand(operand, type_int);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        void unify_arithmetic_operands(Operand* left, Operand* right) {
            if (left->type == type_double) {
                convert_operand(right, type_double);
            }
            else if (right->type == type_double) {
                convert_operand(left, type_double);
            }
            else if (left->type == type_float) {
                convert_operand(right, type_float);
            }
            else if (right->type == type_float) {
                convert_operand(left, type_float);
            }
            else {
                assert(is_integer_type(left->type));
                assert(is_integer_type(right->type));
                promote_operand(left);
                promote_operand(right);
                if (left->type != right->type) {
                    if (is_signed_type(left->type) == is_signed_type(right->type)) {
                        if (type_rank(left->type) <= type_rank(right->type)) {
                            convert_operand(left, right->type);
                        }
                        else {
                            convert_operand(right, left->type);
                        }
                    }
                    else if (is_signed_type(left->type) && type_rank(right->type) >= type_rank(left->type)) {
                        convert_operand(left, right->type);
                    }
                    else if (is_signed_type(right->type) && type_rank(left->type) >= type_rank(right->type)) {
                        convert_operand(right, left->type);
                    }
                    else if (is_signed_type(left->type) && type_sizeof(left->type) > type_sizeof(right->type)) {
                        convert_operand(right, left->type);
                    }
                    else if (is_signed_type(right->type) && type_sizeof(right->type) > type_sizeof(left->type)) {
                        convert_operand(left, right->type);
                    }
                    else {
                        Type *type = unsigned_type(is_signed_type(left->type) ? left->type : right->type);
                        convert_operand(left, type);
                        convert_operand(right, type);
                    }
                }
            }
            assert(left->type == right->type);
        }

        Type* unify_arithmetic_types(Type* left_type, Type* right_type) {
            Operand left = operand_rvalue(left_type);
            Operand right = operand_rvalue(right_type);
            unify_arithmetic_operands(&left, &right);
            assert(left.type == right.type);
            return left.type;
        }

        Type* promote_type(Type* type) {
            Operand operand = operand_rvalue(type);
            promote_operand(&operand);
            return operand.type;
        }

        private Type* resolve_typespec(Typespec* typespec) {
            if (typespec == null)
                return type_void;

            Type* result = null;
            switch (typespec->kind) {
                case TYPESPEC_NAME: {
                    var sym = resolve_name(typespec->name);

                    if (sym->kind != SYM_TYPE) {
                        fatal_error(typespec->pos, "{0} must denote a type", new string(typespec->name));
                        return null;
                    }

                    result = sym->type;
                }
                break;
                case TYPESPEC_PTR:
                    result = type_ptr(resolve_typespec(typespec->ptr.elem));
                    break;
                case TYPESPEC_ARRAY:
                    int size = 0;
                    if (typespec->array.size != null) {
                        Operand operand = resolve_const_expr(typespec->array.size);
                        if (!is_integer_type(operand.type)) {
                            fatal_error(typespec->pos, "Array size constant expression must have integer type");
                        }
                        convert_operand(&operand, type_int);
                        size = operand.val.i;
                        if (size <= 0) {
                            fatal_error(typespec->array.size->pos, "Non-positive array size");
                        }
                    }

                    result = type_array(resolve_typespec(typespec->array.elem), size);
                    break;
                case TYPESPEC_FUNC: {
                    var args = PtrBuffer.GetPooledBuffer();
                    try {
                        for (var i = 0; i < typespec->func.num_args; i++)
                            args->Add(resolve_typespec(typespec->func.args[i]));
                        var ret = type_void;
                        if (typespec->func.ret != null)
                            ret = resolve_typespec(typespec->func.ret);
                        result = type_func((Type**)args->_begin, args->count, ret, false);
                    }
                    finally {
                        args->Release();
                    }
                }
                break;
                default:
                    assert(false);
                    return null;
            }

            assert(typespec->type == null || typespec->type == result);
            typespec->type = result;
            return result;
        }

        private readonly PtrBuffer* sorted_syms = PtrBuffer.Create(capacity: 256);

        private void complete_type(Type* type) {
            if (type->kind == TYPE_COMPLETING) {
                fatal_error(type->sym->decl->pos, "Type completion cycle");
                return;
            }

            if (type->kind != TYPE_INCOMPLETE)
                return;

            var decl = type->sym->decl;
            type->kind = TYPE_COMPLETING;
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            var fields = Buffer<TypeField>.Create();
            for (var i = 0; i < decl->aggregate.num_items; i++) {
                var item = decl->aggregate.items[i];
                var item_type = resolve_typespec(item.type);
                complete_type(item_type);
                for (var j = 0; j < item.num_names; j++)
                    fields.Add(new TypeField { name = item.names[j], type = item_type });
            }

            if (fields.count == 0)
                fatal_error(decl->pos, "No fields");
            if (duplicate_fields(fields._begin, fields.count))
                fatal_error(decl->pos, "Duplicate fields");
            if (decl->kind == DECL_STRUCT) {
                type_complete_struct(type, fields._begin, fields.count);
            }
            else {
                assert(decl->kind == DECL_UNION);
                type_complete_union(type, fields._begin, fields.count);
            }

            sorted_syms->Add(type->sym);
        }

        private Type* resolve_decl_type(Decl* decl) {
            assert(decl->kind == DECL_TYPEDEF);
            return resolve_typespec(decl->typedef_decl.type);
        }

        private Type* resolve_decl_var(Decl* decl) {
            assert(decl->kind == DECL_VAR);
            Type* type = null;
            if (decl->var.type != null)
                type = resolve_typespec(decl->var.type);
            if (decl->var.expr != null) {
                Operand operand = resolve_expected_expr(decl->var.expr, type);
                if (type != null) {
                    if (type->kind == TYPE_ARRAY && operand.type->kind == TYPE_ARRAY && type->array.elem == operand.type->array.elem && type->array.size == 0) {
                        // Incomplete array size, so infer the size from the initializer expression's type.
                    }
                    else {
                        if (!convert_operand(&operand, type)) {
                            fatal_error(decl->pos, "Illegal conversion in variable initializer");
                        }
                    }
                }
                type = operand.type;
            }

            complete_type(type);
            return type;
        }

        private Type* resolve_decl_const(Decl* decl, Val* val) {
            assert(decl->kind == DECL_CONST);
            Operand result = resolve_const_expr(decl->const_decl.expr);
            if (!is_arithmetic_type(result.type))
                fatal_error(decl->pos, "Const must have arithmetic type");
            *val = result.val;
            return result.type;
        }

        private Type* resolve_decl_func(Decl* decl) {
            assert(decl->kind == DECL_FUNC);
            var @params = PtrBuffer.GetPooledBuffer();
            try {
                for (var i = 0; i < decl->func.num_params; i++)
                    @params->Add(resolve_typespec(decl->func.@params[i].type));
                var ret_type = type_void;
                if (decl->func.ret_type != null)
                    ret_type = resolve_typespec(decl->func.ret_type);

                return type_func((Type**)@params->_begin, @params->count, ret_type, decl->func.variadic);
            }
            finally {
                @params->Release();
            }
        }


        private void resolve_cond_expr(Expr* expr) {
            var cond = resolve_expr(expr);
            if (!is_arithmetic_type(cond.type) && cond.type->kind != TYPE_PTR) {
                fatal_error(expr->pos, "Conditional expression must have arithmetic or pointer type");
            }
        }

        private bool resolve_stmt_block(StmtList block, Type* ret_type) {
            var scope = sym_enter();
            bool returns = false;
            for (var i = 0; i < block.num_stmts; i++) {
                returns = resolve_stmt(block.stmts[i], ret_type) || returns;
            }
            sym_leave(scope);
            return returns;
        }

        void resolve_stmt_assign(Stmt* stmt) {
            assert(stmt->kind == STMT_ASSIGN);
            Operand left = resolve_expr(stmt->assign.left);
            if (!left.is_lvalue) {
                fatal_error(stmt->pos, "Cannot assign to non-lvalue");
            }
            if (stmt->assign.right != null) {
                Operand right = resolve_expected_expr(stmt->assign.right, left.type);
                if (!convert_operand(&right, left.type)) {
                    fatal_error(stmt->pos, "Illegal conversion in assignment statement");
                }
            }
        }

        bool resolve_stmt(Stmt* stmt, Type* ret_type) {
            switch (stmt->kind) {
                case STMT_RETURN:
                    if (stmt->expr != null) {
                        Operand operand = resolve_expected_expr(stmt->expr, ret_type);
                        if (!convert_operand(&operand, ret_type)) {
                            fatal_error(stmt->pos, "Illegal conversion in return expression");
                        }
                    }
                    else {
                        if (ret_type != type_void)
                            fatal_error(stmt->pos, "Empty return expression for function with non-void return type");
                    }

                    return true;
                case STMT_BREAK:
                case STMT_CONTINUE:
                    return false;
                case STMT_BLOCK:
                    return resolve_stmt_block(stmt->block, ret_type);
                case STMT_IF: {
                    resolve_cond_expr(stmt->if_stmt.cond);
                    bool returns = resolve_stmt_block(stmt->if_stmt.then_block, ret_type);
                    for (var i = 0; i < stmt->if_stmt.num_elseifs; i++) {
                        var elseif = stmt->if_stmt.elseifs[i];
                        resolve_cond_expr(elseif->cond);
                        returns = resolve_stmt_block(elseif->block, ret_type) && returns;
                    }

                    if (stmt->if_stmt.else_block.stmts != null) {
                        returns = resolve_stmt_block(stmt->if_stmt.else_block, ret_type) && returns;
                    }
                    else
                        returns = false;
                    return returns;
                }
                case STMT_WHILE:
                case STMT_DO_WHILE:
                    resolve_cond_expr(stmt->while_stmt.cond);
                    resolve_stmt_block(stmt->while_stmt.block, ret_type);
                    return false;
                case STMT_FOR: {
                    var sym = sym_enter();
                    resolve_stmt(stmt->for_stmt.init, ret_type);
                    resolve_cond_expr(stmt->for_stmt.cond);
                    resolve_stmt_block(stmt->for_stmt.block, ret_type);
                    resolve_stmt(stmt->for_stmt.next, ret_type);
                    sym_leave(sym);
                    return false;
                }

                case STMT_SWITCH: {
                    var result = resolve_expr(stmt->switch_stmt.expr);
                    bool returns = true;
                    bool has_default = false;
                    for (var i = 0; i < stmt->switch_stmt.num_cases; i++) {
                        var switch_case = stmt->switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_exprs; j++) {
                            Expr *case_expr = switch_case.exprs[j];
                            Operand case_operand = resolve_expr(case_expr);
                            if (!convert_operand(&case_operand, case_expr->type)) {
                                fatal_error(case_expr->pos, "Illegal conversion in switch case expression");
                            }
                            returns = resolve_stmt_block(switch_case.block, ret_type) && returns;
                        }
                        if (switch_case.is_default) {
                            if (has_default) {
                                fatal_error(stmt->pos, "Switch statement has multiple default clauses");
                            }
                            has_default = true;
                        }
                    }
                    if (!has_default) {
                        returns = false;
                    }
                    return returns;
                }

                case STMT_ASSIGN:
                    resolve_stmt_assign(stmt);
                    return false;

                case STMT_INIT:
                    sym_push_var(stmt->init.name, resolve_expr(stmt->init.expr).type);
                    return false;
                case STMT_EXPR:
                    resolve_expr(stmt->expr);
                    return false;
                default:
                    assert(false);
                    return false;
            }
        }

        private void resolve_func_body(Sym* sym) {
            var decl = sym->decl;
            assert(decl->kind == DECL_FUNC);
            assert(sym->state == SYM_RESOLVED);
            var scope = sym_enter();
            for (var i = 0; i < decl->func.num_params; i++) {
                var param = decl->func.@params[i];
                sym_push_var(param.name, resolve_typespec(param.type));
            }
            Type *ret_type = resolve_typespec(decl->func.ret_type);
            bool returns = resolve_stmt_block(decl->func.block, ret_type);
            sym_leave(scope);
            if (ret_type != type_void && !returns) {
                fatal_error(decl->pos, "Not all control paths return values");
            }
            sym_leave(scope);
        }

        private void resolve_sym(Sym* sym) {
            if (sym->state == SYM_RESOLVED)
                return;

            if (sym->state == SYM_RESOLVING) {
                fatal_error(sym->decl->pos, "Cyclic dependency");
                return;
            }

            assert(sym->state == SYM_UNRESOLVED);
            sym->state = SYM_RESOLVING;
            switch (sym->kind) {
                case SYM_TYPE:
                    sym->type = resolve_decl_type(sym->decl);
                    break;
                case SYM_VAR:
                    sym->type = resolve_decl_var(sym->decl);
                    break;
                case SYM_CONST:
                    sym->type = resolve_decl_const(sym->decl, &sym->val);
                    break;
                case SYM_FUNC:
                    sym->type = resolve_decl_func(sym->decl);
                    break;
                default:
                    assert(false);
                    break;
            }

            sym->state = SYM_RESOLVED;
            sorted_syms->Add(sym);
        }

        private void finalize_sym(Sym* sym) {
            resolve_sym(sym);
            if (sym->kind == SYM_TYPE)
                complete_type(sym->type);
            else if (sym->kind == SYM_FUNC)
                resolve_func_body(sym);
        }

        private Sym* resolve_name(char* name) {
            var sym = sym_get(name);
            if (sym == null) {
                return null;
            }

            resolve_sym(sym);
            return sym;
        }


        private Operand resolve_expr_field(Expr* expr) {
            assert(expr->kind == EXPR_FIELD);
            var left = resolve_expr(expr->field.expr);
            var type = left.type;
            complete_type(type);
            if (type->kind != TYPE_STRUCT && type->kind != TYPE_UNION) {
                fatal_error(expr->pos, "Can only access fields on aggregate types");
                return default;
            }

            for (var i = 0; i < type->aggregate.num_fields; i++) {
                var field = type->aggregate.fields[i];
                if (field.name == expr->field.name)
                    return left.is_lvalue ? operand_lvalue(field.type) : operand_rvalue(field.type);
            }

            fatal_error(expr->pos, "No field named '{0}'", new string(expr->field.name));
            return default;
        }

        private Operand ptr_decay(Operand expr) {
            if (expr.type->kind == TYPE_ARRAY)
                return operand_rvalue(type_ptr(expr.type->array.elem));
            return expr;
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
                    return 0ul - val; // Shut up MSVC's unary minus warning
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

        Val eval_unary_op(TokenKind op, Type* type, Val val) {
            if (is_integer_type(type)) {
                Operand operand = operand_const(type, val);
                if (is_signed_type(type)) {
                    convert_operand(&operand, type_llong);
                    operand.val.ll = eval_unary_op_ll(op, operand.val.ll);
                }
                else {
                    convert_operand(&operand, type_ullong);
                    operand.val.ull = eval_unary_op_ull(op, operand.val.ull);
                }
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
                case TOKEN_AND_AND:
                    return (left != 0 && right != 0) ? 1 : 0;
                case TOKEN_OR_OR:
                    return (left != 0 || right != 0) ? 1 : 0;
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
                case TOKEN_AND_AND:
                    return (left != 0 && right != 0) ? 1ul : 0;
                case TOKEN_OR_OR:
                    return (left != 0 || right != 0) ? 1ul : 0;
                default:
                    assert(false);
                    break;
            }
            return 0;
        }
        Val eval_binary_op(TokenKind op, Type* type, Val left, Val right) {
            if (is_integer_type(type)) {
                Operand left_operand = operand_const(type, left);
                Operand right_operand = operand_const(type, right);
                Operand result_operand;
                if (is_signed_type(type)) {
                    convert_operand(&left_operand, type_llong);
                    convert_operand(&right_operand, type_llong);
                    result_operand = operand_const(type_llong, new Val { ll = eval_binary_op_ll(op, left_operand.val.ll, right_operand.val.ll) });
                }
                else {
                    convert_operand(&left_operand, type_ullong);
                    convert_operand(&right_operand, type_ullong);
                    result_operand = operand_const(type_ullong, new Val { ull = eval_binary_op_ull(op, left_operand.val.ull, right_operand.val.ull) });
                }
                convert_operand(&result_operand, type);
                return result_operand.val;
            }

            return default;
        }

        private Operand resolve_expr_name(Expr* expr) {
            assert(expr->kind == EXPR_NAME);
            var sym = resolve_name(expr->name);
            if (sym->kind == SYM_VAR)
                return operand_lvalue(sym->type);

            if (sym->kind == SYM_CONST)
                return operand_const(sym->type, sym->val);

            if (sym->kind == SYM_FUNC)
                return operand_rvalue(sym->type);

            fatal_error(expr->pos, "{0} must denote a var func or const", new string(expr->name));
            return default;
        }

        Operand resolve_unary_op(TokenKind op, Operand operand) {
            promote_operand(&operand);
            if (operand.is_const) {
                return operand_const(operand.type, eval_unary_op(op, operand.type, operand.val));
            }
            else {
                return operand;
            }
        }
        private Operand resolve_expr_unary(Expr* expr) {
            assert(expr->kind == EXPR_UNARY);
            var operand = resolve_expr(expr->unary.expr);
            var type = operand.type;
            switch (expr->unary.op) {
                case TOKEN_MUL:
                    operand = ptr_decay(operand);
                    if (type->kind != TYPE_PTR)
                        fatal_error(expr->pos, "Cannot deref non-ptr type");

                    return operand_lvalue(type->ptr.elem);
                case TOKEN_AND:
                    if (!operand.is_lvalue)
                        fatal_error(expr->pos, "Cannot take address of non-lvalue");

                    return operand_rvalue(type_ptr(type));
                case TOKEN_ADD:
                case TOKEN_SUB:
                    if (!is_arithmetic_type(type)) {
                        fatal_error(expr->pos, "Can only use unary %s with arithmetic types", token_kind_name(expr->unary.op));
                    }
                    return resolve_unary_op(expr->unary.op, operand);
                case TOKEN_NEG:
                    if (!is_integer_type(type)) {
                        fatal_error(expr->pos, "Can only use ~ with integer types", token_kind_name(expr->unary.op));
                    }
                    return resolve_unary_op(expr->unary.op, operand);
                default:
                    assert(false);
                    break;
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
            unify_arithmetic_operands(&left, &right);
            return resolve_binary_op(op, left, right);
        }

        private Operand resolve_expr_binary(Expr* expr) {
            assert(expr->kind == EXPR_BINARY);
            var left = resolve_expr(expr->binary.left);
            var right = resolve_expr(expr->binary.right);
            TokenKind op = expr->binary.op;
            var op_name = token_kind_name(op);
            switch (op) {
                case TOKEN_MUL:
                case TOKEN_DIV:
                    if (!is_arithmetic_type(left.type)) {
                        fatal_error(expr->binary.left->pos, "Left operand of {0} must have arithmetic type", op_name);
                    }
                    if (!is_arithmetic_type(right.type)) {
                        fatal_error(expr->binary.right->pos, "Right operand of {0} must have arithmetic type", op_name);
                    }
                    return resolve_binary_arithmetic_op(op, left, right);
                case TOKEN_MOD:
                    if (!is_integer_type(left.type)) {
                        fatal_error(expr->binary.left->pos, "Left operand of %% must have integer type");
                    }
                    if (!is_integer_type(right.type)) {
                        fatal_error(expr->binary.right->pos, "Right operand of %% must have integer type");
                    }
                    return resolve_binary_arithmetic_op(op, left, right);
                case TOKEN_ADD:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else if (left.type->kind == TYPE_PTR && is_integer_type(right.type)) {
                        return operand_rvalue(left.type);
                    }
                    else if (right.type->kind == TYPE_PTR && is_integer_type(left.type)) {
                        return operand_rvalue(right.type);
                    }
                    else {
                        fatal_error(expr->pos, "Operands of + must both have arithmetic type, or pointer and integer type");
                    }
                    break;
                case TOKEN_SUB:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else if (left.type->kind == TYPE_PTR && is_integer_type(right.type)) {
                        return operand_rvalue(left.type);
                    }
                    else if (left.type->kind == TYPE_PTR && right.type->kind == TYPE_PTR) {
                        if (left.type->ptr.elem != right.type->ptr.elem) {
                            fatal_error(expr->pos, "Cannot subtract pointers to different types");
                        }
                        return operand_rvalue(type_ssize);
                    }
                    else {
                        fatal_error(expr->pos, "Operands of - must both have arithmetic type, pointer and integer type, or compatible pointer types");
                    }
                    break;
                case TOKEN_LSHIFT:
                case TOKEN_RSHIFT:
                    if (is_integer_type(left.type) && is_integer_type(right.type)) {
                        promote_operand(&left);
                        promote_operand(&right);
                        Type *result_type = left.type;
                        Operand result;
                        if (is_signed_type(left.type)) {
                            convert_operand(&left, type_llong);
                            convert_operand(&right, type_llong);
                        }
                        else {
                            convert_operand(&left, type_ullong);
                            convert_operand(&right, type_ullong);
                        }
                        result = resolve_binary_op(op, left, right);
                        convert_operand(&result, result_type);
                        return result;
                    }
                    else {
                        fatal_error(expr->pos, "Operands of {0} must both have integer type", op_name);
                    }
                    break;
                case TOKEN_LT:
                case TOKEN_LTEQ:
                case TOKEN_GT:
                case TOKEN_GTEQ:
                case TOKEN_EQ:
                case TOKEN_NOTEQ:
                    if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                        Operand result = resolve_binary_arithmetic_op(op, left, right);
                        convert_operand(&result, type_int);
                        return result;
                    }
                    else if (left.type->kind == TYPE_PTR && right.type->kind == TYPE_PTR) {
                        if (left.type->ptr.elem != right.type->ptr.elem) {
                            fatal_error(expr->pos, "Cannot compare pointers to different types");
                        }
                        return operand_rvalue(type_int);
                    }
                    else {
                        // TODO: handle null pointer constants
                        fatal_error(expr->pos, "Operands of {0} must be arithmetic types or compatible pointer types", op_name);
                    }
                    break;
                case TOKEN_AND:
                case TOKEN_XOR:
                case TOKEN_OR:
                    if (is_integer_type(left.type) && is_integer_type(right.type)) {
                        return resolve_binary_arithmetic_op(op, left, right);
                    }
                    else {
                        fatal_error(expr->pos, "Operands of {0} must have arithmetic types", op_name);
                    }
                    break;
                case TOKEN_AND_AND:
                case TOKEN_OR_OR:
                    // TODO: const expr evaluation
                    if (is_scalar_type(left.type) && is_scalar_type(right.type)) {
                        return operand_rvalue(type_int);
                    }
                    else {
                        fatal_error(expr->pos, "Operands of {0} must have scalar types", op_name);
                    }
                    break;
                default:
                    assert(false);
                    break;
            }

            return default;
        }
        private int aggregate_field_index(Type* type, char* name) {
            assert(type->kind == TYPE_STRUCT || type->kind == TYPE_UNION);
            for (var i = 0; i < type->aggregate.num_fields; i++)
                if (type->aggregate.fields[i].name == name)
                    return i;
            return -1;
        }

        private Operand resolve_expr_compound(Expr* expr, Type* expected_type) {
            assert(expr->kind == EXPR_COMPOUND);
            if (expected_type == null && expr->compound.type == null)
                fatal_error(expr->pos, "Implicitly typed compound literals used in context without expected type");
            Type* type = null;
            if (expr->compound.type != null)
                type = resolve_typespec(expr->compound.type);
            else
                type = expected_type;
            complete_type(type);

            if (type->kind == TYPE_STRUCT || type->kind == TYPE_UNION) {
                var index = 0;
                for (var i = 0; i < expr->compound.num_fields; i++) {
                    var field = expr->compound.fields[i];
                    if (field.kind == FIELD_INDEX)
                        fatal_error(field.pos, "Index field initializer not allowed for struct/union compound literal");
                    else if (field.kind == FIELD_NAME) {
                        index = aggregate_field_index(type, field.name);
                        if (index == -1) {
                            fatal_error(field.pos, "Named field in compound literal does not exist");
                        }
                    }
                    if (index >= type->aggregate.num_fields)
                        fatal_error(field.pos, "Field initializer in struct/union compound literal out of range");
                    Type *field_type = type->aggregate.fields[index].type;
                    Operand init = resolve_expected_expr(field.init, field_type);
                    if (!convert_operand(&init, field_type)) {
                        fatal_error(field.pos, "Illegal conversion in compound literal initializer");
                    }
                    index++;
                }
            }
            else if (type->kind == TYPE_ARRAY) {
                int index = 0, max_index = 0;
                for (var i = 0; i < expr->compound.num_fields; i++) {
                    var field = expr->compound.fields[i];
                    if (field.kind == FIELD_NAME) {
                        fatal_error(field.pos, "Named field initializer not allowed for array compound literals");
                    }
                    else if (field.kind == FIELD_INDEX) {
                        Operand operand = resolve_const_expr(field.index);
                        if (!is_integer_type(operand.type)) {
                            fatal_error(field.pos, "Field initializer index expression must have type int");
                        }
                        if (!convert_operand(&operand, type_int)) {
                            fatal_error(field.pos, "Illegal conversion in field initializer index");
                        }
                        if (operand.val.i < 0) {
                            fatal_error(field.pos, "Field initializer index cannot be negative");
                        }
                        index = operand.val.i;
                    }

                    if (type->array.size != 0 && index >= type->array.size)
                        fatal_error(field.pos, "Field initializer in array compound literal out of range");
                    Operand init = resolve_expected_expr(field.init, type->array.elem);
                    if (!convert_operand(&init, type->array.elem)) {
                        fatal_error(field.pos, "Illegal conversion in compound literal initializer");
                    }
                    max_index = (int)MAX(max_index, index);
                    index++;
                }
                if (type->array.size == 0) {
                    type = type_array(type->array.elem, max_index + 1);
                }
            }
            else {
                if (expr->compound.num_fields > 1) {
                    fatal_error(expr->pos, "Compound literal for scalar type cannot have more than one operand");
                }
                else if (expr->compound.num_fields == 1) {
                    CompoundField field = expr->compound.fields[0];
                    Operand init = resolve_expected_expr(field.init, type);
                    if (!convert_operand(&init, type)) {
                        fatal_error(field.pos, "Illegal conversion in compound literal initializer");
                    }
                }
            }

            return operand_lvalue(type);
        }

        private Operand resolve_expr_call(Expr* expr) {
            assert(expr->kind == EXPR_CALL);
            var func = resolve_expr(expr->call.expr);
            if (func.type->kind != TYPE_FUNC)
                fatal_error(expr->pos, "Trying to call non-function value");
            var num_params = func.type->func.num_params;

            if (expr->call.num_args < num_params) {
                fatal_error(expr->pos, "Tried to call function with too few arguments");
            }
            if (expr->call.num_args > num_params && !func.type->func.variadic) {
                fatal_error(expr->pos, "Tried to call function with too many arguments");
            }

            for (var i = 0; i < num_params; i++) {
                var param_type = func.type->func.@params[i];
                var arg = resolve_expected_expr(expr->call.args[i], param_type);
                if (!convert_operand(&arg, param_type)) {
                    fatal_error(expr->call.args[i]->pos, "Illegal conversion in call argument expression");
                }
            }
            for (var i = num_params; i < expr->call.num_args; i++) {
                resolve_expr(expr->call.args[i]);
            }

            return operand_rvalue(func.type->func.ret);
        }

        private Operand resolve_expr_ternary(Expr* expr, Type* expected_type) {
            assert(expr->kind == EXPR_TERNARY);
            var cond = ptr_decay(resolve_expr(expr->ternary.cond));
            if (!is_scalar_type(cond.type)) {
                fatal_error(expr->pos, "Ternary conditional must have scalar type");
            }
            Operand left = ptr_decay(resolve_expected_expr(expr->ternary.then_expr, expected_type));
            Operand right = ptr_decay(resolve_expected_expr(expr->ternary.else_expr, expected_type));
            if (is_arithmetic_type(left.type) && is_arithmetic_type(right.type)) {
                unify_arithmetic_operands(&left, &right);
                if (cond.is_const && left.is_const && right.is_const) {
                    return operand_const(left.type, cond.val.i != 0 ? left.val : right.val);
                }
                else {
                    return operand_rvalue(left.type);
                }
            }
            else if (left.type == right.type)
                return operand_rvalue(left.type);

            fatal_error(expr->pos, "Left and right operands of ternary expression must have arithmetic types or identical types");
            return default;
        }

        private Operand resolve_expr_index(Expr* expr) {
            assert(expr->kind == EXPR_INDEX);
            var operand = ptr_decay(resolve_expr(expr->index.expr));
            if (operand.type->kind != TYPE_PTR)
                fatal_error(expr->pos, "Can only index arrays or pointers");
            var index = resolve_expr(expr->index.index);
            if (index.type->kind != TYPE_INT)
                fatal_error(expr->pos, "Index expression must have type long");
            return operand_lvalue(operand.type->ptr.elem);
        }

        private Operand resolve_expr_cast(Expr* expr) {
            assert(expr->kind == EXPR_CAST);
            var type = resolve_typespec(expr->cast.type);
            var operand = ptr_decay(resolve_expr(expr->cast.expr));
            if (!convert_operand(&operand, type)) {
                fatal_error(expr->pos, "Illegal conversion in cast");
            }
            return operand;
        }

        private Operand resolve_expected_expr(Expr* expr, Type* expected_type) {
            Operand result;
            switch (expr->kind) {
                case EXPR_INT:
                    result = operand_const(type_int, new Val { i = expr->int_val });
                    break;
                case EXPR_FLOAT:
                    result = operand_const(type_float, default);
                    break;
                case EXPR_STR:
                    result = operand_rvalue(type_ptr(type_char));
                    break;
                case EXPR_NAME:
                    result = resolve_expr_name(expr);
                    break;
                case EXPR_CAST:
                    result = resolve_expr_cast(expr);
                    break;
                case EXPR_CALL:
                    result = resolve_expr_call(expr);
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
                    result = resolve_expr_unary(expr);
                    break;
                case EXPR_BINARY:
                    result = resolve_expr_binary(expr);
                    break;
                case EXPR_TERNARY:
                    result = resolve_expr_ternary(expr, expected_type);
                    break;
                case EXPR_SIZEOF_EXPR: {
                    var type = resolve_expr(expr->sizeof_expr).type;
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_sizeof(type) });
                    break;
                }

                case EXPR_SIZEOF_TYPE: {
                    var type = resolve_typespec(expr->sizeof_type);
                    complete_type(type);
                    result = operand_const(type_usize, new Val { ll = type_sizeof(type) });
                    break;
                }

                default:
                    assert(false);
                    result = default;
                    break;
            }

            if (result.type != null) {
                assert(expr->type == null || expr->type == result.type);
                expr->type = result.type;
            }

            return result;
        }

        private Operand resolve_expr(Expr* expr) {
            return resolve_expected_expr(expr, null);
        }

        private Operand resolve_const_expr(Expr* expr) {
            var result = resolve_expr(expr);
            if (!result.is_const)
                fatal_error(expr->pos, "Expected constant expression");

            return result;
        }

        internal void sym_global_decls(DeclSet* declset) {
            for (var i = 0; i < declset->num_decls; i++)
                sym_global_decl(declset->decls[i]);
        }

        private readonly string[] code =
        {
            "var u2 = (:int*)42",
           /* "union IntOrPtr { i: int; p: int*; }",
        "var u1 = IntOrPtr{i = 42}",
        "var u2 = IntOrPtr{p = (:int*)42}",
        "var i: int",
        "struct Vector { x, y: int; }",
        "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }",
        "func f2(n: int): int { return 2*n; }",
        "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }",
        "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }",
        "func f5(x: int): int { switch(x) { case 0, 1: return 42; case 3: default: return -1; } }",
        "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }",
        "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }",*/
        };

        private void init_global_syms() {
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


            type_names[(int)TYPE_VOID] = "void".ToPtr();
            type_names[(int)TYPE_BOOL] = "bool".ToPtr();
            type_names[(int)TYPE_CHAR] = "char".ToPtr();
            type_names[(int)TYPE_SCHAR] = "schar".ToPtr();
            type_names[(int)TYPE_UCHAR] = "uchar".ToPtr();
            type_names[(int)TYPE_SHORT] = "short".ToPtr();
            type_names[(int)TYPE_USHORT] = "ushort".ToPtr();
            type_names[(int)TYPE_INT] = "int".ToPtr();
            type_names[(int)TYPE_UINT] = "uint".ToPtr();
            type_names[(int)TYPE_LONG] = "long".ToPtr();
            type_names[(int)TYPE_ULONG] = "ulong".ToPtr();
            type_names[(int)TYPE_LLONG] = "llong".ToPtr();
            type_names[(int)TYPE_ULLONG] = "ullong".ToPtr();
            type_names[(int)TYPE_FLOAT] = "float".ToPtr();
            type_names[(int)TYPE_DOUBLE] = "double".ToPtr();

            sym_global_type("void".ToPtr(), type_void);
            sym_global_type("bool".ToPtr(), type_bool);
            sym_global_type("char".ToPtr(), type_char);

            sym_global_type("schar".ToPtr(), type_schar);
            sym_global_type("uchar".ToPtr(), type_uchar);
            sym_global_type("short".ToPtr(), type_short);
            sym_global_type("ushort".ToPtr(), type_ushort);
            sym_global_type("int".ToPtr(), type_int);
            sym_global_type("uint".ToPtr(), type_uint);
            sym_global_type("long".ToPtr(), type_long);
            sym_global_type("ulong".ToPtr(), type_ulong);
            sym_global_type("llong".ToPtr(), type_llong);
            sym_global_type("ullong".ToPtr(), type_ullong);
            sym_global_type("float".ToPtr(), type_float);


            sym_global_const("true".ToPtr(), type_bool, new Val { b = true });
            sym_global_const("false".ToPtr(), type_bool, new Val { b = false });
        }

        private void finalize_syms() {
            for (var it = (Sym**)global_syms_buf->_begin; it != global_syms_buf->_top; it++) {
                var sym = *it;
                if (sym->decl != null)
                    finalize_sym(sym);
            }
        }

        private void resolve_test() {
            init_global_syms();
            assert(promote_type(type_char) == type_int);
            assert(promote_type(type_schar) == type_int);
            assert(promote_type(type_uchar) == type_int);
            assert(promote_type(type_short) == type_int);
            assert(promote_type(type_ushort) == type_int);
            assert(promote_type(type_int) == type_int);
            assert(promote_type(type_uint) == type_uint);
            assert(promote_type(type_long) == type_long);
            assert(promote_type(type_ulong) == type_ulong);
            assert(promote_type(type_llong) == type_llong);
            assert(promote_type(type_ullong) == type_ullong);

            assert(unify_arithmetic_types(type_char, type_char) == type_int);
            assert(unify_arithmetic_types(type_char, type_ushort) == type_int);
            assert(unify_arithmetic_types(type_int, type_uint) == type_uint);
            assert(unify_arithmetic_types(type_int, type_long) == type_long);
            assert(unify_arithmetic_types(type_ulong, type_long) == type_ulong);
            assert(unify_arithmetic_types(type_long, type_uint) == type_ulong);
            assert(unify_arithmetic_types(type_llong, type_ulong) == type_llong);

            assert(convert_const(type_int, type_char, new Val { c = (char)100 }).i == 100);
            assert(convert_const(type_uint, type_int, new Val { i = -1 }).u == uint.MaxValue);
            assert(convert_const(type_uint, type_ullong, new Val { ull = ulong.MaxValue }).u == uint.MaxValue);
            assert(convert_const(type_int, type_schar, new Val { sc = -1 }).i == -1);

            type_int->align = type_float->align = type_int->size = type_float->size = 4;
            type_void->size = 0;
            type_char->size = type_char->align = 2;

            var int_ptr = type_ptr(type_int);
            assert(type_ptr(type_int) == int_ptr);
            var float_ptr = type_ptr(type_float);
            assert(type_ptr(type_float) == float_ptr);
            assert(int_ptr != float_ptr);
            var int_ptr_ptr = type_ptr(type_ptr(type_int));
            assert(type_ptr(type_ptr(type_int)) == int_ptr_ptr);
            var float4_array = type_array(type_float, 4);
            assert(type_array(type_float, 4) == float4_array);
            var float3_array = type_array(type_float, 3);
            assert(type_array(type_float, 3) == float3_array);
            assert(float4_array != float3_array);
            fixed (Type** t = &type_int) {
                var int_int_func = type_func(t, 1, type_int);
                assert(type_func(t, 1, type_int) == int_int_func);

                var int_func = type_func((Type**) null, 0, type_int);
                assert(int_int_func != int_func);
                assert(int_func == type_func((Type**)null, 0, type_int));
            }

            for (var i = 0; i < code.Length; i++) {
                init_stream(code[i].ToPtr(), null);
                var decl = parse_decl();
                sym_global_decl(decl);
            }

            finalize_syms();
            Console.WriteLine();
            for (var sym = (Sym**)sorted_syms->_begin; sym != sorted_syms->_top; sym++) {
                if ((*sym)->decl != null)
                    print_decl((*sym)->decl);
                else
                    printf("{0}", (*sym)->name);

                printf("\n");
            }

            Console.WriteLine();
        }
    }

    internal unsafe struct Operand
    {
        public Type* type;
        public bool is_lvalue;
        public bool is_const;
        public Val val;
    }

    internal unsafe struct CachedArrayType
    {
        public Type* elem;
        public long size;
        public Type* array;
    }

    internal unsafe struct CachedFuncType
    {
        public Type** @params;
        public long num_params;
        public bool variadic;
        public Type* ret;
        public Type* func;
    }

    internal enum SymKind
    {
        SYM_NONE,
        SYM_VAR,
        SYM_CONST,
        SYM_FUNC,
        SYM_TYPE,
        SYM_ENUM_CONST
    }

    internal enum SymState
    {
        SYM_UNRESOLVED,
        SYM_RESOLVING,
        SYM_RESOLVED
    }


    internal unsafe struct Sym
    {
        public char* name;
        public SymKind kind;
        public SymState state;
        public Decl* decl;
        public Type* type;
        public Val val;
    }

    internal enum TypeKind
    {
        TYPE_NONE,

        TYPE_INCOMPLETE,
        TYPE_COMPLETING,

        TYPE_VOID,
        TYPE_BOOL,

        TYPE_CHAR,
        TYPE_SCHAR,
        TYPE_UCHAR,
        TYPE_SHORT,
        TYPE_USHORT,
        TYPE_INT,
        TYPE_UINT,
        TYPE_LONG,
        TYPE_ULONG,
        TYPE_LLONG,
        TYPE_ULLONG,
        TYPE_FLOAT,
        TYPE_DOUBLE,

        TYPE_PTR,
        TYPE_FUNC,
        TYPE_ARRAY,
        TYPE_STRUCT,
        TYPE_UNION,
        TYPE_ENUM,

        NUM_TYPE_KINDS,
    }


    internal unsafe struct TypeField
    {
        public char* name;
        public Type* type;
        public long offset;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct Val
    {
        [FieldOffset(0)] public bool b;
        [FieldOffset(0)] public char c;
        [FieldOffset(0)] public byte uc;
        [FieldOffset(0)] public sbyte sc;
        [FieldOffset(0)] public short s;
        [FieldOffset(0)] public ushort us;
        [FieldOffset(0)] public int i;
        [FieldOffset(0)] public uint u;
        [FieldOffset(0)] public int l;
        [FieldOffset(0)] public uint ul;
        [FieldOffset(0)] public long ll;
        [FieldOffset(0)] public ulong ull;

        public override string ToString() {
            String str;

            if ((ull >> 32) > 0)
                str = Convert.ToString((int)(ull >> 32), 2) + Convert.ToString(l, 2).PadLeft(8, '0');
            else
                str = Convert.ToString(l, 2);
            return $"[{str.Length}:{str}]";
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Type
    {
        [FieldOffset(0)] public TypeKind kind;
        [FieldOffset(4)] public long size;
        [FieldOffset(12)] public long align;
        [FieldOffset(20)] public Sym* sym;
        [FieldOffset(Ion.PTR_SIZE + 20)] public _ptr ptr;
        [FieldOffset(Ion.PTR_SIZE + 20)] public _array array;
        [FieldOffset(Ion.PTR_SIZE + 20)] public _aggregate aggregate;
        [FieldOffset(Ion.PTR_SIZE + 20)] public _func func;


        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct _ptr
        {
            public Type* elem;
        }

        internal struct _array
        {
            public Type* elem;
            public long size;
        }

        internal struct _aggregate
        {
            public TypeField* fields;
            public long num_fields;
        }

        internal struct _func
        {
            public Type** @params;
            public long num_params;
            public bool variadic;
            public Type* ret;
        }
    }
}