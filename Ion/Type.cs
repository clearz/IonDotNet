using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IonLang
{
    using static TypeKind;
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
        TYPE_CONST,
        NUM_TYPE_KINDS,
    }


    internal unsafe struct TypeField
    {
        public char* name;
        public Type* type;
        public long offset;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Type
    {
        [FieldOffset(0)] public TypeKind kind;
        [FieldOffset(4)] public long size;
        [FieldOffset(12)] public long align;
        [FieldOffset(20)] public bool nonmodifiable;
        [FieldOffset(24)] public Sym* sym;
        [FieldOffset(    Ion.PTR_SIZE + 24)] public Type* @base;
        [FieldOffset(2 * Ion.PTR_SIZE + 24)] public _aggregate aggregate;
        [FieldOffset(2 * Ion.PTR_SIZE + 24)] public _func func;
        [FieldOffset(2 * Ion.PTR_SIZE + 24)] public int num_elems;

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

    unsafe partial class Ion
    {

        private Map cached_ptr_types;
        private Buffer<CachedArrayType> cached_array_types = Buffer<CachedArrayType>.Create();
        private Buffer<CachedFuncType> cached_func_types = Buffer<CachedFuncType>.Create();

        private static readonly Type* type_void    = basic_type_alloc(TYPE_VOID, 0, 1);
        private static readonly Type* type_bool    = basic_type_alloc(TYPE_BOOL, 1, 1);
        private static readonly Type* type_char    = basic_type_alloc(TYPE_CHAR, 2, 2);
        private static readonly Type* type_uchar   = basic_type_alloc(TYPE_UCHAR, 1, 1);
        private static readonly Type* type_schar   = basic_type_alloc(TYPE_SCHAR, 1, 1);
        private static readonly Type* type_short   = basic_type_alloc(TYPE_SHORT, 2, 2);
        private static readonly Type* type_ushort  = basic_type_alloc(TYPE_USHORT, 2, 2);
        private static readonly Type* type_int     = basic_type_alloc(TYPE_INT, 4, 4);
        private static readonly Type* type_uint    = basic_type_alloc(TYPE_UINT, 4, 4);
        private static readonly Type* type_long    = basic_type_alloc(TYPE_LONG, 4, 4); // 4 on 64-bit windows, 8 on 64-bit linux, probably factor this out to the backend
        private static readonly Type* type_ulong   = basic_type_alloc(TYPE_ULONG, 4, 4);
        private static readonly Type* type_llong   = basic_type_alloc(TYPE_LLONG, 8, 8);
        private static readonly Type* type_ullong  = basic_type_alloc(TYPE_ULLONG, 8, 8);
        private static readonly Type* type_float   = basic_type_alloc(TYPE_FLOAT, 4, 4);
        private static readonly Type* type_double  = basic_type_alloc(TYPE_DOUBLE, 8, 8);
        private static readonly Type* type_usize   = type_ullong;
        private static readonly Type* type_ssize   = type_llong;

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
        bool is_ptr_type(Type* type) {
            return type->kind == TYPE_PTR;
        }

        bool is_array_type(Type* type) {
            return type->kind == TYPE_ARRAY;
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


        private Type* type_ptr(Type* @base) {
            var type = cached_ptr_types.map_get<Type>(@base);
            if (type == null) {
                type = type_alloc(TYPE_PTR);
                type->size = PTR_SIZE;
                type->align = PTR_ALIGN;
                type->@base = @base;

                cached_ptr_types.map_put(@base, type);
            }

            return type;
        }

        Map cached_const_types;

        Type* type_const(Type* @base) {
            if (@base->kind == TYPE_CONST) {
                return @base;
            }
            Type *type = cached_const_types.map_get<Type>(@base);
            if (type == null) {
                complete_type(@base);
                type = type_alloc(TYPE_CONST);
                type->nonmodifiable = true;
                type->size = @base->size;
                type->align = @base->align;
                type->@base = @base;
                cached_const_types.map_put(@base, type);
            }
            return type;
        }

        Type* unqualify_type(Type* type) {
            if (type->kind == TYPE_CONST) {
                return type->@base;
            }
            else {
                return type;
            }
        }

        private Type* type_array(Type* elem, int num_elems) {
            for (var it = cached_array_types._begin; it != cached_array_types._top; it++)
                if (it->elem == elem && it->num_elems == num_elems)
                    return it->array;

            complete_type(elem);
            var type = type_alloc(TYPE_ARRAY);
            type->nonmodifiable = elem->nonmodifiable;
            type->size = num_elems * type_sizeof(elem);
            type->align = type_alignof(elem);
            type->@base = elem;
            type->num_elems = num_elems;
            cached_array_types.Add(new CachedArrayType { elem = elem, array = type, num_elems = num_elems });
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
        private bool has_duplicate_fields(TypeField* fields, long num_fields) {
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
            bool nonmodifiable = false;

            for (var it = fields; it != fields + num_fields; it++) {
                assert(IS_POW2(type_alignof(it->type)));
                it->offset = type->size;
                type->size = type_sizeof(it->type) + ALIGN_UP(type->size, type_alignof(it->type));
                type->align = MAX(type->align, type_alignof(it->type));
                nonmodifiable = type->nonmodifiable || nonmodifiable;
            }

            type->aggregate.fields =
                (TypeField*)xmalloc(num_fields * sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint)(num_fields *
                        sizeof(TypeField)));
            type->aggregate.num_fields = num_fields;
            type->nonmodifiable = nonmodifiable;
            return type;
        }

        private Type* type_complete_union(Type* type, TypeField* fields, int num_fields) {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_UNION;
            type->size = 0;
            bool nonmodifiable = false;
            for (var it = fields; it != fields + num_fields; it++) {
                assert(it->type->kind > TYPE_COMPLETING);
                it->offset = 0;
                type->size = MAX(type->size, it->type->size);
                type->align = MAX(type->align, type_alignof(it->type));
                nonmodifiable = type->nonmodifiable || nonmodifiable;
            }

            type->aggregate.fields =
                (TypeField*)xmalloc(sizeof(TypeField) * num_fields);
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint)(num_fields *
                        sizeof(TypeField)));
            type->aggregate.num_fields = num_fields;
            type->nonmodifiable = nonmodifiable;
            return type;
        }

        private Type* type_incomplete(Sym* sym) {
            var type = type_alloc(TYPE_INCOMPLETE);
            type->sym = sym;
            return type;
        }
        Type* type_enum(Sym* sym) {
            Type *type = type_alloc(TYPE_ENUM);
            type->sym = sym;
            type->size = type_int->size;
            type->align = type_int->align;
            return type;
        }
    }

    internal unsafe struct CachedArrayType
    {
        public Type* elem;
        public long num_elems;
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
}
