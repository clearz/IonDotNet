using System.Collections.Generic;

namespace MLang
{
    class Resolve
    {
        static Type type_alloc(TypeKind kind) {
            Type t = new Type();
            t.kind = kind;
            return t;
        }

        //private Type type_int_val = new Type {kind = TYPE_INT};
        //private Type type_float_val = new Type { kind = TYPE_FLOAT };

        private Type type_int = type_alloc(TypeKind.TYPE_INT);
        private readonly Type type_float = type_alloc(TypeKind.TYPE_FLOAT);

        IList<CachedPtrType> cached_ptr_types = new List<CachedPtrType>();

        Type type_ptr(Type base_t) {
            foreach (var it in cached_ptr_types)
            {
                if (it.@base == base_t) {
                    return it.ptr;
                }
            }

            Type t = type_alloc(TypeKind.TYPE_PTR);
            t.ptr = new Type._ptr();
            t.ptr.@base = base_t;

            cached_ptr_types.Add(new CachedPtrType { @base = base_t, ptr = t});
            return t;
        }

        IList<CachedArrayType> cached_array_types = new List<CachedArrayType>();

        Type type_array(Type @base, int size) {
            foreach (var it in cached_array_types)
            {
                if (it.@base == @base && it.size == size) {
                    return it.array;
                }
            }

            Type t = type_alloc(TypeKind.TYPE_ARRAY);
            t.array = new Type._array();
            t.array.@base = @base;
            t.array.size = size;
            cached_array_types.Add(new CachedArrayType{@base = @base, array = t, size = size});
            return t;
        }

        IList<CachedFuncType> cached_func_types = new List<CachedFuncType>();

        Type type_func(Type[] @params, int num_params, Type ret) {
            foreach (var it in cached_func_types)
            {
                if (it.num_params == num_params && it.ret == ret) {
                    bool match = true;
                    for (int i = 0; i < num_params; i++) {
                        if (it.@params[i] != @params[i]) {
                            match = false;
                            break;
                        }
                    }

                    if (match) {
                        return it.func;
                    }
                }
            }

            Type t = type_alloc(TypeKind.TYPE_FUNC);
            t.func = new Type._func();
            t.func.@params = @params;
            t.func.num_params = num_params;
             t.func.ret = ret;
            cached_func_types.Add(new CachedFuncType{func = t, num_params =num_params, ret = ret, @params = @params});
            return t;
        }

        Type type_struct(TypeField[] fields, int num_fields) {
            Type t = type_alloc(TypeKind.TYPE_STRUCT);
            t.aggregate = new Type._aggregate();
            t.aggregate.fields = fields;
            t.aggregate.num_fields = num_fields;
            return t;
        }

        Type type_union(TypeField[] fields, int num_fields) {
            Type t = type_alloc(TypeKind.TYPE_UNION);
            t.aggregate = new Type._aggregate();
            t.aggregate.fields = fields;
            t.aggregate.num_fields = num_fields;
            return t;
        }

        IList<Sym> syms = new List<Sym>();

        Sym sym_get(string name) {
            foreach(var it in syms)
            { 
                if (it.name == name) {
                    return it;
                }
            }

            return null;
        }

        void sym_put(Decl decl) {
            Error.assert(decl.name != null);
            Error.assert(sym_get(decl.name) == null);

            syms.Add(new Sym{decl = decl, name = decl.name, state = SymState.SYM_UNRESOLVED});
        }

        void resolve_decl(Decl decl) {
            switch (decl.kind) {
                case DeclKind.DECL_CONST:
                    break;
            }
        }

        void resolve_sym(Sym sym) {
            if (sym.state == SymState.SYM_RESOLVED) {
                return;
            }

            if (sym.state == SymState.SYM_RESOLVING) {
                Error.fatal("Cyclic dependency");
                return;
            }

            resolve_decl(sym.decl);
        }

        Sym resolve_name(string name) {
            Sym sym = sym_get(name);
            if (sym == null) {
                Error.fatal("Unknown name");
            }

            resolve_sym(sym);
            return sym;
        }

        void resolve_syms() {
            foreach (var it in syms)
            {
                resolve_sym(it);
            }
        }

        internal static void resolve_test() {
            var r = new Resolve();
            
            string foo = "foo";
            var ast = new Ast();
            Error.assert(r.sym_get(foo) == null);
            Decl decl = ast.decl_const(foo, ast.expr_int(42));
            r.sym_put(decl);
            Sym sym = r.sym_get(foo);
            Error.assert(sym != null && sym.decl == decl);

            Type int_ptr = r.type_ptr(r.type_int);
            Error.assert(r.type_ptr(r.type_int) == int_ptr);
            Type float_ptr = r.type_ptr(r.type_float);
            Error.assert(r.type_ptr(r.type_float) == float_ptr);
            Error.assert(int_ptr != float_ptr);
            Type int_ptr_ptr = r.type_ptr(r.type_ptr(r.type_int));
            Error.assert(r.type_ptr(r.type_ptr(r.type_int)) == int_ptr_ptr);
            Type float4_array = r.type_array(r.type_float, 4);
            Error.assert(r.type_array(r.type_float, 4) == float4_array);
            Type float3_array = r.type_array(r.type_float, 3);
            Error.assert(r.type_array(r.type_float, 3) == float3_array);
            Error.assert(float4_array != float3_array);
            Type t = r.type_int;
            Type int_int_func = r.type_func(new []{t}, 1, r.type_int);
            Error.assert(r.type_func(new[] { t }, 1, r.type_int) == int_int_func);

            Type int_func = r.type_func(null, 0, r.type_int);
            Error.assert(int_int_func != int_func);
            Error.assert(int_func == r.type_func(null, 0, r.type_int));
        }

    }

    unsafe struct CachedPtrType
    {
        public Type @base;
        public Type ptr;
    }

    class CachedArrayType
    {
        public Type @base;
        public int size;
        public Type array;
    }

    class CachedFuncType
    {
        public Type[] @params;
        public int num_params;
        public Type ret;
        public Type func;
    }


    class ConstEntity
    {
        public Type type;
        public ulong int_val;
        public double float_val;
    }


    class Entity
    {
        public int foo;
    }


    enum SymState
    {
        SYM_UNRESOLVED,
        SYM_RESOLVING,
        SYM_RESOLVED,
    }


    class Sym
    {
        public string name;
        public Decl decl;
        public SymState state;
        public Entity[] ent;
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



    class TypeField
    {
        public string name;
        public Type type;
    }


    class Type
    {
        public TypeKind kind;
        public _ptr ptr;
        public _array array;
        public _aggregate aggregate;
        public _func func;

        internal class _ptr
        {
            public Type @base;
        }

        internal class _array
        {
            public Type @base;
            public int size;
        }

        internal class _aggregate
        {
            public TypeField[] fields;
            public int num_fields;
        }

        internal class _func
        {
            public Type[] @params;
            public int num_params;
            public Type ret;
        }
    }
}