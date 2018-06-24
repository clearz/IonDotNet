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
    using static EntityState;
    using static DeclKind;
    using static EntityKind;
    using static TypespecKind;
    using static ExprKind;

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
            Type* type = (Type*) Marshal.AllocHGlobal(sizeof(Type));// Marshal.AllocHGlobal(sizeof(Type)); //(1, sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint)sizeof(Type));
            type->kind = kind;
            return type;
        }

        //private Type type_int_val = new Type {kind = TYPE_INT};
        //private Type type_float_val = new Type { kind = TYPE_FLOAT };

        private Type* type_int = type_alloc(TypeKind.TYPE_INT);
        private readonly Type* type_float = type_alloc(TypeKind.TYPE_FLOAT);

        Buffer<CachedPtrType> cached_ptr_types = Buffer<CachedPtrType>.Create();

        Type* type_ptr(Type* elem) {
            for (CachedPtrType* it = (CachedPtrType*)cached_ptr_types._begin; it != cached_ptr_types._top; it++) {
                if (it->elem == elem) {
                    return it->ptr;
                }
            }

            Type* type = type_alloc(TYPE_PTR);
            type->size = PTR_SIZE;
            type->ptr.elem = elem;

            cached_ptr_types.Add(new CachedPtrType { elem = elem, ptr = type});
            return type;
        }

        Buffer<CachedArrayType> cached_array_types = Buffer<CachedArrayType>.Create();

        Type* type_array(Type* elem, size_t size) {
            for (CachedArrayType* it = (CachedArrayType*)cached_array_types._begin; it != cached_array_types._top; it++) {
                if (it->elem == elem && it->size == size) {
                    return it->array;
                }
            }

            complete_type(elem);
            Type* type = type_alloc(TYPE_ARRAY);
            type->size = size * elem->size;
            type->array.elem = elem;
            type->array.size = size;
            cached_array_types.Add(new CachedArrayType{elem = elem, array = type, size = size});
            return type;
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

            Type* type = type_alloc(TYPE_FUNC);
            type->size = PTR_SIZE;
            type->func.@params = (Type**)Marshal.AllocHGlobal(sizeof(Type*));
            Unsafe.CopyBlock(type->func.@params, @params, (uint) (num_params * sizeof(Type*)));//memcpy(t->func.@params, @params, num_params * sizeof(Type*));
            type->func.num_params = num_params;
             type->func.ret = ret;
            cached_func_types.Add(new CachedFuncType{func = type, num_params =num_params, ret = ret, @params = @params});
            return type;
        }

        Type* type_complete_struct(Type* type, TypeField* fields, size_t num_fields) {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_STRUCT;
            type->size = 0;
            for (TypeField* it = fields; it != fields + num_fields; it++)
            {
                // TODO: Alignment, etc.
                type->size += it->type->size;
            }
            type->aggregate.fields = (TypeField*)  Marshal.AllocHGlobal(sizeof(Type)); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->func.@params, fields, (uint)(num_fields * sizeof(TypeField)));//mmemcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        Type* type_complete_union(Type* type, TypeField* fields, size_t num_fields) {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_UNION;
            type->size = 0;
            for (TypeField* it = fields; it != fields + num_fields; it++)
            {
                type->size = MAX(type->size, it->type->size);
            }
            type->aggregate.fields = (TypeField*)  Marshal.AllocHGlobal(sizeof(Type)); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields, (uint)(num_fields * sizeof(TypeField))); // memcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        Type* type_incomplete(Entity* entity) {
            Type* type = type_alloc(TYPE_INCOMPLETE);
            type->entity = entity;
            return type;
        }


        readonly PtrBuffer* entities = PtrBuffer.Create();

        Entity* entity_new(EntityKind kind, char* name, Decl* decl) {
            Entity* entity = (Entity*) Marshal.AllocHGlobal(sizeof(Entity));
            Unsafe.InitBlock(entity,0, (uint)sizeof(Entity));
            entity->kind = kind;
            entity->name = name;
            entity->decl = decl;
            return entity;
        }

        Entity* entity_decl(Decl* decl)
        {
            EntityKind kind = ENTITY_NONE;
            switch (decl->kind)
            {
                case DECL_STRUCT:
                case DECL_UNION:
                case DECL_TYPEDEF:
                case DECL_ENUM:
                    kind = ENTITY_TYPE;
                    break;
                case DECL_VAR:
                    kind = ENTITY_VAR;
                    break;
                case DECL_CONST:
                    kind = ENTITY_CONST;
                    break;
                case DECL_FUNC:
                    kind = ENTITY_FUNC;
                    break;
                default:
                    assert(false);
                    break;
            }
            Entity* entity = entity_new(kind, decl->name, decl);
            if (decl->kind == DECL_STRUCT || decl->kind == DECL_UNION)
            {
                entity->state = ENTITY_RESOLVED;
                entity->type = type_incomplete(entity);
            }
            return entity;
        }

        Entity* entity_enum_const(char* name, Decl *decl) {
            return entity_new(ENTITY_ENUM_CONST, name, decl);
        }

        Entity* entity_get(char* name)
        {
            for (Entity** it = (Entity**)entities->_begin; it != entities->_top; it++)
            {
                Entity* entity = *it;
                if (entity->name == name)
                {
                    return entity;
                }
            }
            return null;
        }

        Entity* entity_install_decl(Decl* decl)
        {
            Entity* entity = entity_decl(decl);
            entities->Add(entity);
            if (decl->kind == DECL_ENUM)
            {
                for (size_t i = 0; i < decl->enum_decl.num_items; i++)
                {
                    entities->Add(entity_enum_const(decl->enum_decl.items[i].name, decl));
                }
            }
            return entity;
        }

        Entity* entity_install_type(char* name, Type* type)
        {
            Entity* entity = entity_new(ENTITY_TYPE, name, null);
            entity->state = ENTITY_RESOLVED;
            entity->type = type;
            entities->Add(entity);
            return entity;
        }

    void resolve_decl(Decl* decl) {
            switch (decl->kind) {
                case DECL_CONST:
                    break;
            }
        }

        void resolve_sym(Entity* entity) {
            if (entity->state == ENTITY_RESOLVED) {
                return;
            }

            if (entity->state == ENTITY_RESOLVING) {
                fatal("Cyclic dependency");
                return;
            }

            resolve_decl(entity->decl);
        }
        Type* resolve_typespec(Typespec* typespec)
        {
            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                {
                    Entity* entity = resolve_name(typespec->name);
                    if (entity->kind != ENTITY_TYPE)
                    {
                        fatal("%s must denote a type", new string(typespec->name));
                        return null;
                    }
                    return entity->type;
                }
                case TYPESPEC_PTR:
                    return type_ptr(resolve_typespec(typespec->ptr.elem));
                case TYPESPEC_ARRAY:
                    return type_array(resolve_typespec(typespec->array.elem), resolve_const_expr(typespec->array.size).val);
                case TYPESPEC_FUNC:
                {
                    //Type** args = null;
                    var args = PtrBuffer.Create();
                    for (size_t i = 0; i < typespec->func.num_args; i++)
                    {
                        args->Add(resolve_typespec(typespec->func.args[i]));
                    }
                    return type_func((Type**)args->_begin, args->count, resolve_typespec(typespec->func.ret));
                }
                default:
                    assert(false);
                    return null;
            }
        }

        private PtrBuffer* ordered_entities = PtrBuffer.Create();

        void complete_type(Type* type)
        {
            if (type->kind == TYPE_COMPLETING)
            {
                fatal("Type completion cycle");
                return;
            }
            else if (type->kind != TYPE_INCOMPLETE)
            {
                return;
            }
            type->kind = TYPE_COMPLETING;
            Decl* decl = type->entity->decl;
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            var fields = Buffer<TypeField>.Create();
            for (size_t i = 0; i < decl->aggregate.num_items; i++)
            {
                AggregateItem item = decl->aggregate.items[i];
                Type* item_type = resolve_typespec(item.type);
                complete_type(item_type);
                for (size_t j = 0; j < item.num_names; j++)
                {
                    fields.Add( new TypeField{ name = item.names[j], type = item_type});
                }
            }
            if (decl->kind == DECL_STRUCT) {
                type_complete_struct(type, (TypeField*)fields._begin, fields.count);
            } else {
                assert(decl->kind == DECL_UNION);
                type_complete_union(type, (TypeField*)fields._begin, fields.count);
            }
            ordered_entities->Add(type->entity);
        }

        Type* resolve_decl_type(Decl* decl)
        {
            assert(decl->kind == DECL_TYPEDEF);
            return resolve_typespec(decl->typedef_decl.type);
        }

        Type* resolve_decl_var(Decl* decl)
        {
            assert(decl->kind == DECL_VAR);
            Type* type = null;
            if (decl->var.type != null)
            {
                type = resolve_typespec(decl->var.type);
            }
            if (decl->var.expr != null)
            {
                ResolvedExpr result = resolve_expr(decl->var.expr);
                if (type != null && result.type != type)
                {
                    fatal("Declared var type does not match inferred type");
                }
                type = result.type;
            }
            complete_type(type);
            return type;
        }

        Type* resolve_decl_const(Decl* decl, long* val)
        {
            assert(decl->kind == DECL_CONST);
            ResolvedExpr result = resolve_expr(decl->const_decl.expr);
            *val = result.val;
            return result.type;
        }

        void resolve_entity(Entity* entity)
        {
            if (entity->state == ENTITY_RESOLVED)
            {
                return;
            }
            else if (entity->state == ENTITY_RESOLVING)
            {
                fatal("Cyclic dependency");
                return;
            }
            assert(entity->state == ENTITY_UNRESOLVED);
            entity->state = ENTITY_RESOLVING;
            switch (entity->kind)
            {
                case ENTITY_TYPE:
                    entity->type = resolve_decl_type(entity->decl);
                    break;
                case ENTITY_VAR:
                    entity->type = resolve_decl_var(entity->decl);
                    break;
                case ENTITY_CONST:
                    entity->type = resolve_decl_const(entity->decl, &entity->val);
                    break;
                default:
                    assert(false);
                    break;
            }
            entity->state = ENTITY_RESOLVED;
            ordered_entities->Add(entity);
        }

        Entity* resolve_name(char* name) {
            Entity* entity = entity_get(name);
            if (entity == null) {
                fatal("Non-existent name");
                return null;
            }
            resolve_entity(entity);
            return entity;
        }

        ResolvedExpr resolve_expr_name(Expr* expr)
        {
            assert(expr->kind == EXPR_NAME);
            Entity* entity = resolve_name(expr->name);
            if (entity->kind == ENTITY_VAR)
            {
                return new ResolvedExpr{ type = entity->type};
            }
            else if (entity->kind == ENTITY_CONST)
            {
                return new ResolvedExpr{ type = entity->type, is_const = true, val = entity->val};
            }
            else
            {
                fatal("%s must denote a var or const", new String(expr->name));
                return new ResolvedExpr();
            }
        }

        ResolvedExpr resolve_expr_unary(Expr* expr)
        {
            assert(expr->kind == EXPR_UNARY);
            assert(expr->unary.op == TokenKind.TOKEN_MUL);
            ResolvedExpr operand = resolve_expr(expr->unary.expr);
            if (operand.type->kind != TYPE_PTR)
            {
                fatal("Cannot deref non-ptr type");
            }
            return new ResolvedExpr{ type = operand.type->ptr.elem};
        }

        ResolvedExpr resolve_expr_binary(Expr* expr)
        {
            assert(expr->kind == EXPR_BINARY);
            assert(expr->binary.op == TokenKind.TOKEN_ADD);
            ResolvedExpr left = resolve_expr(expr->binary.left);
            ResolvedExpr right = resolve_expr(expr->binary.right);
            if (left.type != type_int)
            {
                fatal("left operand of + is not int");
            }
            if (right.type != left.type)
            {
                fatal("left and right operand of + must have same type");
            }
            ResolvedExpr result = new ResolvedExpr{ type = left.type };
            if (left.is_const && right.is_const)
            {
                result.is_const = true;
                result.val = left.val + right.val;
            }
            return result;
        }

        ResolvedExpr resolve_expr(Expr* expr)
        {
            switch (expr->kind)
            {
                case EXPR_INT:
                    return new ResolvedExpr{ type = type_int, is_const = true, val = expr->int_val};
                case EXPR_NAME:
                    return resolve_expr_name(expr);
                case EXPR_UNARY:
                    return resolve_expr_unary(expr);
                case EXPR_BINARY:
                    return resolve_expr_binary(expr);
                case EXPR_SIZEOF_EXPR:
                    {
                        ResolvedExpr result = resolve_expr(expr->sizeof_expr);
                        Type* type = result.type;
                        complete_type(type);
                        return new ResolvedExpr{ type = type_int, is_const = true,val = type->size};
                    }
                case EXPR_SIZEOF_TYPE:
                    {
                        Type* type = resolve_typespec(expr->sizeof_type);
                        complete_type(type);
                        return new ResolvedExpr{type = type_int, is_const = true, val = type->size};
                    }
                default:
                    assert(false);
                    return new ResolvedExpr();
            }
        }

        ResolvedExpr resolve_const_expr(Expr* expr)
        {
            ResolvedExpr result = resolve_expr(expr);
            if (!result.is_const)
            {
                fatal("Expected constant expression");
            }
            return result;
        }



        void resolve_test() {
            type_int->size = type_float->size = 4;

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

            char* int_name = _I("int");
            entity_install_type(int_name, type_int);
            char*[] code = {
                "const n = 1+sizeof(*p)".ToPtr(),
                "var p: T*".ToPtr(),
                "struct T { i: int[sizeof(p)]; }".ToPtr(),
                "const n = sizeof(x)".ToPtr(),
                "var x: T".ToPtr(),
                "struct T { s: S*; }".ToPtr(),
                "struct S { t: T[n]; }".ToPtr(),
            };
            for (size_t i = 0; i < code.Length ; i++) {
                init_stream(code[i]);
                Decl* decl = parse_decl();
                //print_decl(decl);
                //Console.WriteLine();
                entity_install_decl(decl);
            }
            for (Entity** it = (Entity**)entities->_begin; it != entities->_top; it++)
            {
                Entity* entity = *it;
                resolve_entity(entity);
                if (entity->kind == ENTITY_TYPE)
                {
                    complete_type(entity->type);
                }
            }
            for (Entity** it = (Entity**)ordered_entities->_begin; it != ordered_entities->_top; it++)
            {
                Entity* entity = *it;
                //printf("{0}, ", entity->name);
            }

           // Console.WriteLine();
        }

    }
    unsafe struct ResolvedExpr
    {
        public Type* type;
        public bool is_const;
        public long val;
    }
    unsafe struct CachedPtrType
    {
        public Type* elem;
        public Type* ptr;
    }

    unsafe struct CachedArrayType
    {
        public Type* elem;
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

    enum EntityKind
    {
        ENTITY_NONE,
        ENTITY_VAR,
        ENTITY_CONST,
        ENTITY_FUNC,
        ENTITY_TYPE,
        ENTITY_ENUM_CONST,
    }

    enum EntityState
    {
        ENTITY_UNRESOLVED,
        ENTITY_RESOLVING,
        ENTITY_RESOLVED,
    }


    unsafe struct Entity
    {
        public char* name;
        public EntityKind kind;
        public EntityState state;
        public Decl* decl;
        public Type* type;
        public long val;
    }

    enum TypeKind
    {
        TYPE_NONE,
        TYPE_INCOMPLETE,
        TYPE_COMPLETING,
        TYPE_INT,
        TYPE_FLOAT,
        TYPE_PTR,
        TYPE_ARRAY,
        TYPE_STRUCT,
        TYPE_UNION,
        TYPE_ENUM,
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
        [FieldOffset(4)] public size_t size;
        [FieldOffset(Ion.PTR_SIZE + 4)] public Entity* entity;
        [FieldOffset(Ion.PTR_SIZE * 2 + 4)] public _ptr ptr;
        [FieldOffset(Ion.PTR_SIZE * 2 + 4)] public _array array;
        [FieldOffset(Ion.PTR_SIZE * 2 + 4)] public _aggregate aggregate;
        [FieldOffset(Ion.PTR_SIZE * 2 + 4)] public _func func;


        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct _ptr
        {
            public Type* elem;
        }

        internal struct _array
        {
            public Type* elem;
            public size_t size;
        }

        internal struct _aggregate
        {
            public TypeField* fields;
            public size_t num_fields;
        }

        internal struct _func
        {
            public Type** @params;
            public size_t num_params;
            public Type* ret;
        }

    }

}