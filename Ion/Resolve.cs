using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DotNetCross.Memory;

namespace Lang
{
    using static TypeKind;
    using static SymState;
    using static DeclKind;

    #region Typedefs

#if X64
    using size_t = System.Int64;
#else
    using size_t = System.Int32;
#endif

    #endregion

    unsafe partial class Ion
    {
        static Type* type_alloc(TypeKind kind) {
            Type* t = (Type*) Marshal.AllocHGlobal(sizeof(Type));// Marshal.AllocHGlobal(sizeof(Type)); //(1, sizeof(Type));
            Unsafe.InitBlock(t, 1, (uint)sizeof(Type));
            t->kind = kind;
            return t;
        }

        //private Type type_int_val = new Type {kind = TYPE_INT};
        //private Type type_float_val = new Type { kind = TYPE_FLOAT };

        private Type* type_int = type_alloc(TypeKind.TYPE_INT);
        private readonly Type* type_float = type_alloc(TypeKind.TYPE_FLOAT);

        Buffer<CachedPtrType> cached_ptr_types = Buffer<CachedPtrType>.Create();

        Type* type_ptr(Type* @base) {
            for (CachedPtrType* it = (CachedPtrType*)cached_ptr_types._begin; it != cached_ptr_types._top; it++) {
                if (it->@base == @base) {
                    return it->ptr;
                }
            }

            Type* t = type_alloc(TYPE_PTR);
            t->ptr.@base = @base;

            cached_ptr_types.Add(new CachedPtrType { @base = @base, ptr = t});
            return t;
        }

        Buffer<CachedArrayType> cached_array_types = Buffer<CachedArrayType>.Create();

        Type* type_array(Type* @base, size_t size) {
            for (CachedArrayType* it = (CachedArrayType*)cached_array_types._begin; it != cached_array_types._top; it++) {
                if (it->@base == @base && it->size == size) {
                    return it->array;
                }
            }

            Type* t = type_alloc(TYPE_ARRAY);
            t->array.@base = @base;
            t->array.size = size;
            cached_array_types.Add(new CachedArrayType{@base = @base, array = t, size = size});
            return t;
        }

        Buffer<CachedFuncType> cached_func_types = Buffer<CachedFuncType>.Create();

        Type* type_func(Type** @params, size_t num_params, Type* ret) {
            for (CachedFuncType* it = (CachedFuncType*)cached_func_types._begin; it != cached_func_types._top; it++) {
                if (it->num_params == num_params && it->ret == ret) {
                    bool match = true;
                    for (size_t i = 0; i < num_params; i++) {
                        if (it->@params[i] != @params[i]) {
                            match = false;
                            break;
                        }
                    }

                    if (match) {
                        return it->func;
                    }
                }
            }

            Type* t = type_alloc(TYPE_FUNC);
            t->func.@params = (Type**)Marshal.AllocHGlobal(sizeof(Type*));
            Unsafe.InitBlock(t->func.@params, (byte)num_params, (uint)sizeof(Type*));
            Unsafe.CopyBlock(t->func.@params, @params, (uint) (num_params * sizeof(Type*)));//memcpy(t->func.@params, @params, num_params * sizeof(Type*));
            t->func.num_params = num_params;
             t->func.ret = ret;
            cached_func_types.Add(new CachedFuncType{func = t, num_params =num_params, ret = ret, @params = @params});
            return t;
        }

        Type* type_struct(TypeField* fields, size_t num_fields) {
            Type* t = type_alloc(TYPE_STRUCT);
            t->aggregate.fields = (TypeField*)  Marshal.AllocHGlobal(sizeof(Type)); //(num_fields, sizeof(TypeField));
            Unsafe.InitBlock(t->aggregate.fields, (byte)num_fields, (uint)sizeof(TypeField));
            Unsafe.CopyBlock(t->func.@params, fields, (uint)(num_fields * sizeof(TypeField)));//mmemcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            t->aggregate.num_fields = num_fields;
            return t;
        }

        Type* type_union(TypeField* fields, size_t num_fields) {
            Type* t = type_alloc(TYPE_UNION);
            t->aggregate.fields = (TypeField*)  Marshal.AllocHGlobal(sizeof(Type)); //(num_fields, sizeof(TypeField));
            Unsafe.InitBlock(t->aggregate.fields, (byte)num_fields, (uint)sizeof(TypeField));
            Unsafe.CopyBlock(t->aggregate.fields, fields, (uint)(num_fields * sizeof(TypeField))); // memcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            t->aggregate.num_fields = num_fields;
            return t;
        }

        Buffer<Sym> syms = Buffer<Sym>.Create();

        Sym* sym_get(char* name) {
            for (Sym* it = (Sym*)syms._begin; it != syms._top; it++) {
                if (it->name == name) {
                    return it;
                }
            }

            return null;
        }

        void sym_put(Decl* decl) {
            assert(decl->name != null);
            assert(sym_get(decl->name) == null);

            syms.Add(new Sym{decl = decl, name = decl->name, state = SYM_UNRESOLVED});
        }

        void resolve_decl(Decl* decl) {
            switch (decl->kind) {
                case DECL_CONST:
                    break;
            }
        }

        void resolve_sym(Sym* sym) {
            if (sym->state == SYM_RESOLVED) {
                return;
            }

            if (sym->state == SYM_RESOLVING) {
                fatal("Cyclic dependency");
                return;
            }

            resolve_decl(sym->decl);
        }

        Sym* resolve_name(char* name) {
            Sym* sym = sym_get(name);
            if (sym == null) {
                fatal("Unknown name");
                return null;
            }

            resolve_sym(sym);
            return sym;
        }

        void resolve_syms() {
            for (Sym* it = (Sym*)syms._begin; it != syms._end; it++) {
                resolve_sym(it);
            }
        }

        void resolve_test() {
            char* foo = _I("foo");
            assert(sym_get(foo) == null);
            Decl* decl = decl_const(foo, expr_int(42));
            sym_put(decl);
            Sym* sym = sym_get(foo);
            assert(sym != null && sym->decl == decl);

            Type* int_ptr = type_ptr(type_int);
            assert(type_ptr(type_int) == int_ptr);
            Type* float_ptr = type_ptr(type_float);
            assert(type_ptr(type_float) == float_ptr);
            assert(int_ptr != float_ptr);
            Type* int_ptr_ptr = type_ptr(type_ptr(type_int));
            assert(type_ptr(type_ptr(type_int)) == int_ptr_ptr);
            Type* float4_array = type_array(type_float, 4);
            assert(type_array(type_float, 4) == float4_array);
            Type* float3_array = type_array(type_float, 3);
            assert(type_array(type_float, 3) == float3_array);
            assert(float4_array != float3_array);
            fixed (Type** t = &type_int) {
                Type* int_int_func = type_func(t, 1, type_int);
                assert(type_func(t, 1, type_int) == int_int_func);

                Type* int_func = type_func(null, 0, type_int);
                assert(int_int_func != int_func);
                assert(int_func == type_func(null, 0, type_int));
            }
        }

    }

    unsafe struct CachedPtrType
    {
        public Type* @base;
        public Type* ptr;
    }

    unsafe struct CachedArrayType
    {
        public Type* @base;
        public size_t size;
        public Type* array;
    }

    unsafe struct CachedFuncType
    {
        public Type** @params;
        public size_t num_params;
        public Type* ret;
        public Type* func;
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct ConstEntity
    {
        [FieldOffset(0)] public Type* type;
        [FieldOffset(Ion.PTR_SIZE)] public ulong int_val;
        [FieldOffset(Ion.PTR_SIZE)] public double float_val;
    }


    struct Entity
    {
        public int foo;
    }


    enum SymState
    {
        SYM_UNRESOLVED,
        SYM_RESOLVING,
        SYM_RESOLVED,
    }


    unsafe struct Sym
    {
        public char* name;
        public Decl* decl;
        public SymState state;
        public Entity* ent;
    }

    enum TypeKind
    {
        TYPE_INT = 1,
        TYPE_FLOAT,
        TYPE_PTR,
        TYPE_ARRAY,
        TYPE_STRUCT,
        TYPE_UNION,
        TYPE_FUNC,
    }



    unsafe struct TypeField
    {
        public char* name;
        public Type* type;
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Type
    {
        [FieldOffset(0)] public TypeKind kind;
        [FieldOffset(4)] public _ptr ptr;
        [FieldOffset(4)] public _array array;
        [FieldOffset(4)] public _aggregate aggregate;
        [FieldOffset(4)] public _func func;


        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal unsafe struct _ptr
        {
            public Type* @base;
        }

        internal unsafe struct _array
        {
            public Type* @base;
            public size_t size;
        }

        internal unsafe struct _aggregate
        {
            public TypeField* fields;
            public size_t num_fields;
        }

        internal unsafe struct _func
        {
            public Type** @params;
            public size_t num_params;
            public Type* ret;
        }

    }

}