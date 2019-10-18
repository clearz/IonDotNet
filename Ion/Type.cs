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
        TYPE_ENUM,
        TYPE_FLOAT,
        TYPE_DOUBLE,

        TYPE_PTR,
        TYPE_FUNC,
        TYPE_ARRAY,
        TYPE_STRUCT,
        TYPE_UNION,
        TYPE_CONST,
        NUM_TYPE_KINDS,
    }

    internal unsafe struct TypeField
    {
        public char* name;
        public Type* type;
        public long offset;
    }

    struct TypeMetrics
    {
        public long size;
        public long align;
        public bool sign;
        public ulong max;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Type
    {
        [FieldOffset(0)] public TypeKind kind;
        [FieldOffset(4)] public long size;
        [FieldOffset(12)] public long align;
        [FieldOffset(20)] public bool nonmodifiable;
        [FieldOffset(24)] public uint typeid;
        [FieldOffset(28)] public Sym* sym;
        [FieldOffset(    Ion.PTR_SIZE + 28)] public Type* @base;
        [FieldOffset(2 * Ion.PTR_SIZE + 28)] public _aggregate aggregate;
        [FieldOffset(2 * Ion.PTR_SIZE + 28)] public _func func;
        [FieldOffset(2 * Ion.PTR_SIZE + 28)] public int num_elems;

        internal struct _aggregate
        {
            public TypeField* fields;
            public long num_fields;
        }

        internal struct _func
        {
            public Type** @params;
            public long num_params;
            public bool has_varargs;
            public Type* ret;
        }
    }

    unsafe partial class Ion
    {
        private Map cached_ptr_types;
        private Map cached_array_types;
        private Map cached_func_types;
        private Map cached_const_types;
        private static Map typeid_map;

        private static readonly Type* type_void    = basic_type_alloc(TYPE_VOID);
        private static readonly Type* type_bool    = basic_type_alloc(TYPE_BOOL);
        private static readonly Type* type_char    = basic_type_alloc(TYPE_CHAR);
        private static readonly Type* type_uchar   = basic_type_alloc(TYPE_UCHAR);
        private static readonly Type* type_schar   = basic_type_alloc(TYPE_SCHAR);
        private static readonly Type* type_short   = basic_type_alloc(TYPE_SHORT);
        private static readonly Type* type_ushort  = basic_type_alloc(TYPE_USHORT);
        private static readonly Type* type_int     = basic_type_alloc(TYPE_INT);
        private static readonly Type* type_uint    = basic_type_alloc(TYPE_UINT);
        private static readonly Type* type_long    = basic_type_alloc(TYPE_LONG); // 4 on 64-bit windows, 8 on 64-bit linux, probably factor this out to the backend
        private static readonly Type* type_ulong   = basic_type_alloc(TYPE_ULONG);
        private static readonly Type* type_llong   = basic_type_alloc(TYPE_LLONG);
        private static readonly Type* type_ullong  = basic_type_alloc(TYPE_ULLONG);
        private static readonly Type* type_float   = basic_type_alloc(TYPE_FLOAT);
        private static readonly Type* type_double  = basic_type_alloc(TYPE_DOUBLE);

        private static Type* type_usize, type_ssize, type_uintptr;

        static readonly int[] type_ranks = new int[(int)NUM_TYPE_KINDS];
        static readonly char*[] type_names = new char*[(int)NUM_TYPE_KINDS];
        static readonly char*[] typeid_kind_names = new char*[(int)NUM_TYPE_KINDS];
        bool is_const_type(Type* type) {
            return type->kind == TYPE_CONST;
        }

        bool is_integer_type(Type* type) {
            return TYPE_BOOL <= type->kind && type->kind <= TYPE_ENUM;
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

        bool is_aggregate_type(Type* type) {
            return type->kind == TYPE_STRUCT || type->kind == TYPE_UNION;
        }

        bool is_ptr_type(Type* type) {
            return type->kind == TYPE_PTR;
        }

        bool is_array_type(Type* type) {
            return type->kind == TYPE_ARRAY;
        }


        bool is_signed_type(Type* type) {
            switch (type->kind) {
                case TYPE_CHAR:
                    return type_metrics[(int)TYPE_CHAR].sign;
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
        int aggregate_field_index(Type* type, char* name) {
            assert(is_aggregate_type(type));
            for (int i = 0; i < type->aggregate.num_fields; i++) {
                if (type->aggregate.fields[i].name == name) {
                    return i;
                }
            }
            return -1;
        }

        Type* aggregate_field_type_from_index(Type* type, int index) {
            assert(is_aggregate_type(type));
            assert(0 <= index && index < type->aggregate.num_fields);
            return type->aggregate.fields[index].type;
        }

        Type* aggregate_field_type_from_name(Type* type, char* name) {
            assert(is_aggregate_type(type));
            int index = aggregate_field_index(type, name);
            if (index < 0) {
                return null;
            }
            return aggregate_field_type_from_index(type, index);
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
        void init_builtin_type(Type* type) {
            type->typeid = next_typeid++;
            register_typeid(type);
            type->size = type_metrics[(int)type->kind].size;
            type->align = type_metrics[(int)type->kind].align;
        }

        void init_builtin_types() {
            init_builtin_type(type_void);
            init_builtin_type(type_bool);
            init_builtin_type(type_char);
            init_builtin_type(type_uchar);
            init_builtin_type(type_schar);
            init_builtin_type(type_short);
            init_builtin_type(type_ushort);
            init_builtin_type(type_int);
            init_builtin_type(type_uint);
            init_builtin_type(type_long);
            init_builtin_type(type_ulong);
            init_builtin_type(type_llong);
            init_builtin_type(type_ullong);
            init_builtin_type(type_float);
            init_builtin_type(type_double);
        }

        private static Type* basic_type_alloc(TypeKind kind, long size = 0, long align = 0, uint typeid = 0) {
            var type = (Type*) xmalloc(sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint)sizeof(Type));
            type->kind = kind;
            type->typeid = typeid;
            type->size = size;
            type->align = align;
            type->typeid = typeid;
            return type;
        }

        private long type_sizeof(Type* type) {
            assert(type->kind > TYPE_COMPLETING);
            return type->size;
        }

        private long type_alignof(Type* type) {
            assert(type->kind > TYPE_COMPLETING);
            assert(IS_POW2(type->align));
            return type->align;
        }

        Type* type_enum(Sym* sym) {
            Type *type = type_alloc(TYPE_ENUM);
            type->sym = sym;
            type->size = type_int->size;
            type->align = type_int->align;
            return type;
        }

        private Type* type_ptr(Type* @base) {
            var type = cached_ptr_types.map_get<Type>(@base);
            if (type == null) {
                type = type_alloc(TYPE_PTR);
                type->size = type_metrics[(int)TYPE_PTR].size;
                type->align = type_metrics[(int)TYPE_PTR].align;
                type->@base = @base;

                cached_ptr_types.map_put(@base, type);
            }

            return type;
        }


        Type* get_type_from_typeid(int typeid) {
            if (typeid == 0) {
                return null;
            }
            return typeid_map.map_get<Type>((void*)typeid);
        }

        void register_typeid(Type* type) {
            typeid_map.map_put((void*)type->typeid, type);
        }

        private Type* type_alloc(TypeKind kind) {
            var type = (Type*) xmalloc(sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint)sizeof(Type));
            type->kind = kind;
            type->typeid = next_typeid++;
            register_typeid(type);
            return type;
        }


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

        bool is_incomplete_array_type(Type* type) {
            return is_array_type(type) && type->num_elems == 0;
        }

        private Type* type_array(Type* @base, int num_elems) {
            var hash = Map.hash_mix(Map.hash_ptr(@base), Map.hash_uint64((ulong)num_elems));
            void *key = (void *)(hash != 0 ? hash : 1);
            CachedArrayType *cached = cached_array_types.map_get<CachedArrayType>(key);
            for (CachedArrayType* it = cached; it != null; it = it->next) {
                Type *t = it->type;
                if (t->@base == @base && t->num_elems == num_elems) {
                    return t;
                }
            }

            complete_type(@base);
            Type *type = type_alloc(TYPE_ARRAY);
            type->nonmodifiable = @base->nonmodifiable;
            type->size = num_elems * type_sizeof(@base);
            type->align = type_alignof(@base);
            type->@base = @base;
            type->num_elems = num_elems;
            CachedArrayType *new_cached = xmalloc<CachedArrayType>();
            new_cached->type = type;
            new_cached->next = cached;
            cached_array_types.map_put(key, new_cached);
            return type;
        }

        [DllImport("msvcrt.dll")]
        private static extern unsafe int memcmp(void* b1, void* b2, int count);

        private Type* type_func(Type** @params, int num_params, Type* ret, bool has_varargs = false) {
            var params_size = num_params * PTR_SIZE;
            ulong hash = Map.hash_mix(Map.hash_bytes(@params, params_size), Map.hash_ptr(ret));
            void *key = (void *)(hash != 0 ? hash : 1);
            CachedFuncType *cached = cached_func_types.map_get<CachedFuncType>(key);
            for (CachedFuncType* it = cached; it != null; it = it->next) {
                Type *type1 = it->type;
                if (type1->func.num_params == num_params && type1->func.ret == ret && type1->func.has_varargs == has_varargs) {
                    if (memcmp(type1->func.@params, @params, params_size) == 0) {
                        return type1;
                    }
                }
            }
            Type *type = type_alloc(TYPE_FUNC);
            type->size = type_metrics[(int)TYPE_PTR].size;
            type->align = type_metrics[(int)TYPE_PTR].align;
            type->func.@params = (Type**)memdup(@params, params_size);
            type->func.num_params = num_params;
            type->func.has_varargs = has_varargs;
            type->func.ret = ret;
            CachedFuncType *new_cached = xmalloc<CachedFuncType>();
            new_cached->type = type;
            new_cached->next = cached;
            cached_func_types.map_put(key, new_cached);
            return type;
        }

        private Type* type_func(Type*[] params_a, int num_params, Type* ret, bool has_varargs = false) {
            fixed (Type** @params = params_a) {
                return type_func(@params, num_params, ret, has_varargs);
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
                nonmodifiable = it->type->nonmodifiable || nonmodifiable;
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
    }

    internal unsafe struct CachedArrayType
    {
        public  Type *type;
        public CachedArrayType *next;
    }

    internal unsafe struct CachedFuncType
    {
        public Type *type;
        public CachedFuncType *next;
    }
}
