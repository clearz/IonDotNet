﻿using System.Collections.Generic;
using System.Linq;

namespace IonLangManaged
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
        TYPE_CONST,
        TYPE_STRUCT,
        TYPE_TUPLE,
        TYPE_UNION,
        NUM_TYPE_KINDS,
    }

    internal unsafe struct TypeField
    {
        public string name;
        public Type type;
        public long offset;
    }

    struct TypeMetrics
    {
        public long size;
        public long align;
        public bool sign;
        public ulong max;
    }


    internal unsafe class Type
    {
        public TypeKind kind;
        public bool nonmodifiable;
        public long size;
        public long align;
        public long padding;
        public uint typeid;
        public Sym sym;
        public Type @base;

        public _aggregate aggregate;
        public _func func;

        public bool incomplete_elems;
        public int num_elems;

        public override string ToString() => kind == TYPE_FUNC ? func.ToString() :
                                             kind > TYPE_CONST ? kind.ToString() + ": " + aggregate.ToString() :
                                             $"{kind} base=({@base}) [{GetHashCode()}]";


        internal struct _aggregate
        {
            public TypeField[] fields;
            public long num_fields;

            public override string ToString() => $"({string.Join(", ", fields.Select(t => t.type.kind.ToString().Substring(5)))})";

        }

        internal struct _func
        {
            public bool intrinsic;
            public Type[] @params;
            public int num_params;
            public bool has_varargs;
            public Type varargs_type;
            public Type ret;

            public override string ToString() => $"func ({string.Join(", ", @params.Select(t => t.kind.ToString().Substring(5)))}) => {ret.kind.ToString().Substring(5)}";

        }
    }

    unsafe partial class Ion
    {
        readonly Dictionary<Type,Type> cached_ptr_types = new Dictionary<Type,Type>();
        readonly Dictionary<(Type, int),Type> cached_array_types = new Dictionary<(Type, int),Type>();
        readonly Dictionary<int,Type> cached_func_types = new Dictionary<int,Type>();
        readonly Dictionary<Type,Type> cached_const_types = new Dictionary<Type,Type>();
        readonly Dictionary<uint,Type>  typeid_map = new Dictionary<uint,Type> ();
        readonly Dictionary<Type[],Type> cached_tuple_types = new Dictionary<Type[],Type>();
        IList<Type> tuple_types = new List<Type>();

        readonly Type type_void    = basic_type_alloc(TYPE_VOID);
        readonly Type type_bool    = basic_type_alloc(TYPE_BOOL);
        readonly Type type_char    = basic_type_alloc(TYPE_CHAR);
        readonly Type type_uchar   = basic_type_alloc(TYPE_UCHAR);
        readonly Type type_schar   = basic_type_alloc(TYPE_SCHAR);
        readonly Type type_short   = basic_type_alloc(TYPE_SHORT);
        readonly Type type_ushort  = basic_type_alloc(TYPE_USHORT);
        readonly Type type_int     = basic_type_alloc(TYPE_INT);
        readonly Type type_uint    = basic_type_alloc(TYPE_UINT);
        readonly Type type_long    = basic_type_alloc(TYPE_LONG);
        readonly Type type_ulong   = basic_type_alloc(TYPE_ULONG);
        readonly Type type_llong   = basic_type_alloc(TYPE_LLONG);
        readonly Type type_ullong  = basic_type_alloc(TYPE_ULLONG);
        readonly Type type_float   = basic_type_alloc(TYPE_FLOAT);
        readonly Type type_double  = basic_type_alloc(TYPE_DOUBLE);

        Type type_char_ptr;
        Type type_alloc_func;

        Type type_usize, type_ssize, type_uintptr;

        uint next_typeid = 1;

        Type type_any;

        readonly int[] type_ranks = new int[(int)NUM_TYPE_KINDS];
        readonly string[] type_names = new string[(int)NUM_TYPE_KINDS];
        readonly string[] typeid_kind_names = new string[(int)NUM_TYPE_KINDS];

        long type_padding(Type type) {
            assert(type.kind > TYPE_COMPLETING);
            return type.padding;
        }

        bool is_integer_type(Type type) {
            return TYPE_BOOL <= type.kind && type.kind <= TYPE_ENUM;
        }

        bool is_floating_type(Type type) {
            return TYPE_FLOAT <= type.kind && type.kind <= TYPE_DOUBLE;
        }

        bool is_arithmetic_type(Type type) {
            return TYPE_BOOL <= type.kind && type.kind <= TYPE_DOUBLE;
        }

        bool is_scalar_type(Type type) {
            return TYPE_BOOL <= type.kind && type.kind <= TYPE_FUNC;
        }

        bool is_aggregate_type(Type type) {
            return type.kind == TYPE_STRUCT || type.kind == TYPE_UNION || type.kind == TYPE_TUPLE;
        }

        bool is_ptr_type(Type type) {
            return type != null && type.kind == TYPE_PTR;
        }

        bool is_func_type(Type type) {
            return type != null && type.kind == TYPE_FUNC;
        }

        bool is_ptr_like_type(Type type) {
            return type != null && type.kind == TYPE_PTR || type.kind == TYPE_FUNC;
        }

        bool is_const_type(Type type) {
            return type != null && type.kind == TYPE_CONST;
        }

        bool is_array_type(Type type) {
            return type != null && type.kind == TYPE_ARRAY;
        }

        bool is_incomplete_array_type(Type type) {
            return type != null && is_array_type(type) && type.num_elems == 0;
        }

        bool is_signed_type(Type type) {
            switch (type.kind) {
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

        Type qualify_type(Type type, Type qual) {
            type = unqualify_type(type);
            while (qual.kind == TYPE_CONST) {
                type = type_const(type);
                qual = qual.@base;
            }
            return type;
        }

        int aggregate_item_field_index(Type type, string name) {
            assert(is_aggregate_type(type));
            for (int i = 0; i < type.aggregate.num_fields; i++) {
                if (type.aggregate.fields[i].name == name) {
                    return i;
                }
            }
            return -1;
        }

        Type aggregate_item_field_type_from_index(Type type, int index) {
            assert(is_aggregate_type(type));
            assert(0 <= index && index < (int)type.aggregate.num_fields);
            return type.aggregate.fields[index].type;
        }

        Type aggregate_item_field_type_from_name(Type type, string name) {
            assert(is_aggregate_type(type));
            int index = aggregate_item_field_index(type, name);
            if (index < 0) {
                return null;
            }
            return aggregate_item_field_type_from_index(type, index);
        }

        int type_rank(Type type) {
            int rank = type_ranks[(int)type.kind];
            assert(rank != 0);
            return rank;
        }

        Type unsigned_type(Type type) {
            switch (type.kind) {
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

        void init_builtin_type(Type type) {
            type.typeid = next_typeid++;
            register_typeid(type);
            type.size = type_metrics[(int)type.kind].size;
            type.align = type_metrics[(int)type.kind].align;
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


            type_char_ptr = type_ptr(type_char);
            type_alloc_func = type_func(new[] { type_usize, type_usize }, 2, type_ptr(type_void), false, false, type_void);
        }

        static Type basic_type_alloc(TypeKind kind, long size = 0, long align = 0, uint typeid = 0) {
            var type = new Type();
            type.kind = kind;
            type.typeid = typeid;
            type.size = size;
            type.align = align;
            type.typeid = typeid;
            return type;
        }

        long type_sizeof(Type type) {
            assert(type.kind > TYPE_COMPLETING);
            return type.size;
        }

        long type_alignof(Type type) {
            assert(type.kind > TYPE_COMPLETING);
            assert(IS_POW2(type.align));
            return type.align;
        }

        Type type_enum(Sym sym, Type @base) {
            Type type = type_alloc(TYPE_ENUM);
            type.sym = sym;
            type.@base = @base;
            type.size = type_int.size;
            type.align = type_int.align;
            return type;
        }

        Type type_ptr(Type @base) {
            var type = cached_ptr_types.ContainsKey(@base) ? cached_ptr_types[@base] : null;
            if (type == null) {
                type = type_alloc(TYPE_PTR);
                type.size = type_metrics[(int)TYPE_PTR].size;
                type.align = type_metrics[(int)TYPE_PTR].align;
                type.@base = @base;

                cached_ptr_types[@base] = type;
            }

            return type;
        }


        Type get_type_from_typeid(uint typeid) {
            if (typeid == 0) {
                return null;
            }
            return typeid_map.ContainsKey(typeid) ? typeid_map[typeid] : null;
        }

        void register_typeid(Type type) {
            typeid_map[type.typeid] = type;
        }

        Type type_alloc(TypeKind kind) {
            var type = new Type {kind = kind, typeid = next_typeid++};
            register_typeid(type);
            return type;
        }

        Type type_const(Type @base) {
            if (@base.kind == TYPE_CONST) {
                return @base;
            }
            var type = cached_const_types.ContainsKey(@base) ? cached_const_types[@base] : null;
            if (type == null) {
                complete_type(@base);
                type = type_alloc(TYPE_CONST);
                type.nonmodifiable = true;
                type.size = @base.size;
                type.align = @base.align;
                type.@base = @base;
                cached_const_types[@base] = type;
            }
            return type;
        }

        Type unqualify_type(Type type) {
            if (type.kind == TYPE_CONST) {
                return type.@base;
            }
            else {
                return type;
            }
        }
        Type type_array(Type @base, int num_elems, bool incomplete_elems) {
            var type = cached_array_types.ContainsKey((@base,num_elems)) ? cached_array_types[(@base,num_elems)] : null;
            if (type == null) {
                complete_type(@base);
                type = type_alloc(TYPE_ARRAY);
                type.nonmodifiable = @base.nonmodifiable;
                type.size = num_elems * type_sizeof(@base);
                type.align = type_alignof(@base);
                type.@base = @base;
                type.num_elems = num_elems;
                type.incomplete_elems = incomplete_elems;
                if (!incomplete_elems) {
                    cached_array_types[(@base, num_elems)] = type;
                }
            }
            return type;
        }

        Type type_func(Type[] @params, int num_params, Type ret, bool intrinsic, bool has_varargs, Type varargs_type) {
            //var hashstr = @params.Aggregate("", (i, field) => i + field.ToString()) + has_varargs + varargs_type + ret.ToString() + intrinsic;
            var hash = Extentions.Hash(@params, ret, intrinsic, has_varargs ? varargs_type : null);
            Type type = cached_func_types.ContainsKey(hash) ? cached_func_types[hash] : null;
            if (type == null) {
                type = type_alloc(TYPE_FUNC);
                type.size = type_metrics[(int)TYPE_PTR].size;
                type.align = type_metrics[(int)TYPE_PTR].align;
                type.func.@params = @params;
                type.func.num_params = num_params;
                type.func.intrinsic = intrinsic;
                type.func.has_varargs = has_varargs;
                type.func.varargs_type = varargs_type;
                type.func.ret = ret;
                cached_func_types[hash] = type;
            }

            //System.Console.WriteLine(type + ": " + type.typeid + ", hash = " + hash);
            return type;
        }

        // TODO: This probably shouldn't use an O(n^2) algorithm
        bool has_duplicate_fields(Type type) {
            for (var i = 0; i < type.aggregate.num_fields; i++)
                for (var j = i + 1; j < type.aggregate.num_fields; j++)
                    if (type.aggregate.fields[i].name == type.aggregate.fields[j].name)
                        return true;
            return false;
        }

        void add_type_fields(List<TypeField> fields, Type type, long offset) { //TypeField[]
            assert(type.kind == TYPE_STRUCT || type.kind == TYPE_UNION);
            for (var i = 0; i < type.aggregate.num_fields; i++) {
                TypeField field = type.aggregate.fields[i];
                fields.Add(new TypeField { name = field.name, type = field.type, offset = (field.offset + offset) });
            }
        }

        Type type_complete_struct(Type type, TypeField[] fields, int num_fields) {
            assert(type.kind == TYPE_COMPLETING);
            type.kind = TYPE_STRUCT;
            type.size = 0;
            type.align = 0;
            bool nonmodifiable = false;
            long field_sizes = 0;
            var new_fields = new List<TypeField>();

            for (var i = 0; i < fields.Length; i++) {
                var it = fields[i];
                assert(IS_POW2(type_alignof(it.type)));
                if (it.name != null) {
                    it.offset = type.size;
                    new_fields.Add(it);
                }
                else {
                    add_type_fields(new_fields, it.type, type.size);
                }
                field_sizes += type_sizeof(it.type);
                type.align = MAX(type.align, type_alignof(it.type));
                type.size = type_sizeof(it.type) + ALIGN_UP(type.size, type_alignof(it.type));
                nonmodifiable = it.type.nonmodifiable || nonmodifiable;
            }
            type.size = ALIGN_UP(type.size, type.align);
            type.padding = type.size - field_sizes;
            type.aggregate.fields = new_fields.ToArray();
            type.aggregate.num_fields = new_fields.Count;
            type.nonmodifiable = nonmodifiable;
            return type;
        }

        Type type_complete_union(Type type, TypeField[] fields, int num_fields) {
            assert(type.kind == TYPE_COMPLETING);
            type.kind = TYPE_UNION;
            type.size = 0;
            type.align = 0;
            bool nonmodifiable = false;
            var new_fields = new List<TypeField>();

            for (var i = 0; i < fields.Length; i++) {
                var it = fields[i];
                assert(it.type.kind > TYPE_COMPLETING);
                if (it.name != null) {
                    it.offset = 0;
                    new_fields.Add(it);
                }
                else {
                    add_type_fields(new_fields, it.type, 0);
                }

                type.align = MAX(type.align, type_alignof(it.type));
                type.size = MAX(type.size, type_sizeof(it.type));
                nonmodifiable = it.type.nonmodifiable || nonmodifiable;
            }

            type.size = ALIGN_UP(type.size, type.align);
            type.aggregate.fields = new_fields.ToArray();
            type.aggregate.num_fields = new_fields.Count;
            type.nonmodifiable = nonmodifiable;
            return type;
        }

        void type_complete_tuple(Type type, Type[] fields, int num_fields) {
            type.kind = TYPE_TUPLE;
            type.size = 0;
            type.align = 0;
            bool nonmodifiable = false;
            long elem_sizes = 0;
            var new_fields = new List<TypeField>();
            for (int i = 0; i < num_fields; i++) {
                Type field = fields[i];
                complete_type(fields[i]);
                assert(IS_POW2(type_alignof(field)));
                string name = ("_" + i);
                var new_field = new TypeField {

                    name = name,
                    type = fields[i],
                    offset = type.size,
                };
                new_fields.Add(new_field);
                elem_sizes += type_sizeof(field);
                type.align = MAX(type.align, type_alignof(field));
                type.size = type_sizeof(field) + ALIGN_UP(type.size, type_alignof(field));
                nonmodifiable = field.nonmodifiable || nonmodifiable;
            }
            type.size = ALIGN_UP(type.size, type.align);
            type.padding = type.size - elem_sizes;
            type.aggregate.fields = new_fields.ToArray();
            type.aggregate.num_fields = new_fields.Count;
            type.nonmodifiable = nonmodifiable;
        }

        Type type_tuple(Type[] fields, int num_fields) {
            Type type = cached_tuple_types.ContainsKey(fields) ? cached_tuple_types[fields] : null;
            if (type == null) {
                type = type_alloc(TYPE_TUPLE);
                type_complete_tuple(type, fields, num_fields);
                cached_tuple_types[fields] = type;
                tuple_types.Add(type);
            }
            return type;
        }

        Type unqualify_ptr_type(Type type) {
            if (type.kind == TYPE_PTR) {
                type = type_ptr(unqualify_type(type.@base));
            }
            return type;
        }

        Type type_incomplete(Sym sym) {
            var type = type_alloc(TYPE_INCOMPLETE);
            type.sym = sym;
            return type;
        }
    }
}
