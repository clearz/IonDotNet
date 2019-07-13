using DotNetCross.Memory;
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

        private readonly Type* type_void = basic_type_alloc(TYPE_VOID, 0, 1);
        private readonly Type* type_int = basic_type_alloc(TYPE_INT, 4, 4);
        private readonly Type* type_float = basic_type_alloc(TYPE_FLOAT, 4, 4);
        private readonly Type* type_char = basic_type_alloc(TYPE_CHAR, 2, 2);
#if X64
        internal const int PTR_SIZE = 8;
#else
        internal const int PTR_SIZE = 8;
#endif
        private const long PTR_ALIGN = 8;

        private static Type* type_alloc(TypeKind kind)
        {
            var type = (Type*) xmalloc(sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint) sizeof(Type));
            type->kind = kind;
            return type;
        }

        private static Type* basic_type_alloc(TypeKind kind, long size = 0, long align = 0)
        {
            var type = type_alloc(kind);
            type->size = size;
            type->align = align;
            return type;
        }

        private long type_sizeof(Type* type)
        {
            assert(type->kind > TYPE_COMPLETING);
            assert(type->size != 0);
            return type->size;
        }

        private long type_alignof(Type* type)
        {
            assert(type->kind > TYPE_COMPLETING);
            assert(IS_POW2(type->align));
            return type->align;
        }


        private Type* type_ptr(Type* elem)
        {
            var type = (Type*) cached_ptr_types.map_get(elem);
            if (type == null)
            {
                type = type_alloc(TYPE_PTR);
                type->size = PTR_SIZE;
                type->align = PTR_ALIGN;
                type->ptr.elem = elem;

                cached_ptr_types.map_put(elem, type);
            }

            return type;
        }


        private Type* type_array(Type* elem, long size)
        {
            for (var it = cached_array_types._begin; it != cached_array_types._top; it++)
                if (it->elem == elem && it->size == size)
                    return it->array;

            complete_type(elem);
            var type = type_alloc(TYPE_ARRAY);
            type->size = size * type_sizeof(elem);
            type->align = type_alignof(elem);
            type->array.elem = elem;
            type->array.size = size;
            cached_array_types.Add(new CachedArrayType {elem = elem, array = type, size = size});
            return type;
        }

        private Type* type_func(Type** @params, int num_params, Type* ret)
        {
            for (var it = cached_func_types._begin; it != cached_func_types._top; it++)
                if (it->num_params == num_params && it->ret == ret)
                {
                    var match = true;
                    for (var i = 0; i < num_params; i++)
                        if (it->@params[i] != @params[i])
                        {
                            match = false;
                            break;
                        }

                    if (match) return it->func;
                }

            var type = type_alloc(TYPE_FUNC);
            type->size = PTR_SIZE;
            type->align = PTR_ALIGN;
            if (num_params > 0)
            {
                type->func.@params = (Type**) xmalloc(sizeof(Type*) * num_params);
                Unsafe.CopyBlock(type->func.@params, @params,
                    (uint) (num_params *
                            sizeof(Type*))); //memcpy(t->func.@params, @params, num_params * sizeof(Type*));
                type->func.num_params = num_params;
            }

            type->func.ret = ret;
            cached_func_types.Add(new CachedFuncType
                {func = type, num_params = num_params, ret = ret, @params = type->func.@params});
            return type;
        }

        private Type* type_func(Type*[] params_a, int num_params, Type* ret)
        {
            fixed (Type** @params = params_a)
            {
                for (var it = cached_func_types._begin; it != cached_func_types._top; it++)
                    if (it->num_params == num_params && it->ret == ret)
                    {
                        var match = true;
                        for (var i = 0; i < num_params; i++)
                            if (it->@params[i] != @params[i])
                            {
                                match = false;
                                break;
                            }

                        if (match) return it->func;
                    }

                var type = type_alloc(TYPE_FUNC);
                type->size = PTR_SIZE;
                type->align = PTR_ALIGN;
                if (num_params > 0)
                {
                    type->func.@params = (Type**) xmalloc(sizeof(Type*) * num_params);
                    Unsafe.CopyBlock(type->func.@params, @params,
                        (uint) (num_params *
                                (long) sizeof(Type*))); //memcpy(t->func.@params, @params, num_params * sizeof(Type*));
                    type->func.num_params = num_params;
                }

                type->func.ret = ret;
                cached_func_types.Add(new CachedFuncType
                    {func = type, num_params = num_params, ret = ret, @params = type->func.@params});
                return type;
            }
        }

        // TODO: This probably shouldn't use an O(n^2) algorithm
        private bool duplicate_fields(TypeField* fields, long num_fields)
        {
            for (var i = 0; i < num_fields; i++)
            for (var j = i + 1; j < num_fields; j++)
                if (fields[i].name == fields[j].name)
                    return true;
            return false;
        }

        private Type* type_complete_struct(Type* type, TypeField* fields, int num_fields)
        {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_STRUCT;
            type->size = 0;
            type->align = 0;

            for (var it = fields; it != fields + num_fields; it++)
            {
                assert(IS_POW2(type_alignof(it->type)));
                type->size = type_sizeof(it->type) + ALIGN_UP(type->size, type_alignof(it->type));
                type->align = MAX(type->align, type_alignof(it->type));
            }

            type->aggregate.fields =
                (TypeField*) xmalloc(num_fields * sizeof(TypeField)); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint) (num_fields *
                        sizeof(TypeField))); // memcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        private Type* type_complete_union(Type* type, TypeField* fields, int num_fields)
        {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_UNION;
            type->size = 0;
            for (var it = fields; it != fields + num_fields; it++)
            {
                assert(it->type->kind > TYPE_COMPLETING);
                type->size = MAX(type->size, it->type->size);
                type->align = MAX(type->align, type_alignof(it->type));
            }

            type->aggregate.fields =
                (TypeField*) xmalloc(sizeof(TypeField) * num_fields); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields,
                (uint) (num_fields *
                        sizeof(TypeField))); // memcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        private Type* type_incomplete(Sym* sym)
        {
            var type = type_alloc(TYPE_INCOMPLETE);
            type->sym = sym;
            return type;
        }


        private Sym* sym_new(SymKind kind, char* name, Decl* decl)
        {
            var sym = xmalloc<Sym>(); //(Sym*)xmalloc(sizeof(Sym));

            sym->kind = kind;
            sym->name = name;
            sym->decl = decl;
            sym->state = 0;
            return sym;
        }

        private Sym* sym_decl(Decl* decl)
        {
            var kind = SYM_NONE;
            switch (decl->kind)
            {
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
            if (decl->kind == DECL_STRUCT || decl->kind == DECL_UNION)
            {
                sym->state = SYM_RESOLVED;
                sym->type = type_incomplete(sym);
            }

            return sym;
        }

        private Sym* sym_enum_const(char* name, Decl* decl)
        {
            return sym_new(SYM_ENUM_CONST, name, decl);
        }

        private Sym* sym_get(char* name)
        {
            for (var sym = local_syms._top - 1; sym >= local_syms._begin; sym--)
                if (sym->name == name)
                    return sym;

            return (Sym*) global_syms_map.map_get(name);
        }

        private void sym_push_var(char* name, Type* type)
        {
            if (local_syms._top == local_syms._begin + MAX_LOCAL_SYMS) fatal("Too many local symbols");

            var sym = xmalloc<Sym>();
            sym->name = name;
            sym->kind = SYM_VAR;
            sym->state = SYM_RESOLVED;
            sym->type = type;
            *local_syms._top++ = *sym;
        }

        private Sym* sym_enter()
        {
            return local_syms._top;
        }

        private void sym_leave(Sym* sym)
        {
            // Console.Write((Sym*)local_syms._top - sym);
            local_syms._top = sym;
        }

        private void sym_global_put(Sym* sym)
        {
            //ulong l = (ulong)sym->name << 32 | (ulong)sym->decl;
            global_syms_map.map_put(sym->name, sym);
            global_syms_buf->Add(sym);
        }

        private Sym* sym_global_decl(Decl* decl)
        {
            var sym = sym_decl(decl);
            sym_global_put(sym);
            decl->sym = sym;
            if (decl->kind == DECL_ENUM)
                for (var i = 0; i < decl->enum_decl.num_items; i++)
                    sym_global_put(sym_enum_const(decl->enum_decl.items[i].name, decl));
            return sym;
        }

        private void sym_global_type(char* name, Type* type)
        {
            var sym = sym_new(SYM_TYPE, _I(name), null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            sym_global_put(sym);
        }

        private void sym_global_func(char* name, Type* type)
        {
            assert(type->kind == TYPE_FUNC);
            var sym = sym_new(SYM_FUNC, _I(name), null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            sym_global_put(sym);
        }

        private Operand operand_rvalue(Type* type)
        {
            return new Operand {type = type};
        }

        private Operand operand_lvalue(Type* type)
        {
            return new Operand {type = type, is_lvalue = true};
        }

        private Operand operand_const(long val)
        {
            return new Operand {type = type_int, is_const = true, val = val};
        }


        private Type* resolve_typespec(Typespec* typespec)
        {
            if (typespec == null)
                return type_void;

            Type* result = null;
            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                {
                    var sym = resolve_name(typespec->name);

                    if (sym->kind != SYM_TYPE)
                    {
                        fatal("{0} must denote a type", new string(typespec->name));
                        return null;
                    }

                    result = sym->type;
                }
                    break;
                case TYPESPEC_PTR:
                    result = type_ptr(resolve_typespec(typespec->ptr.elem));
                    break;
                case TYPESPEC_ARRAY:
                    var size = resolve_const_expr(typespec->array.size);
                    if (size < 0)
                        fatal("Negative array size");

                    result = type_array(resolve_typespec(typespec->array.elem), size);
                    break;
                case TYPESPEC_FUNC:
                {
                    var args = PtrBuffer.GetPooledBuffer();
                    try
                    {
                        for (var i = 0; i < typespec->func.num_args; i++)
                            args->Add(resolve_typespec(typespec->func.args[i]));
                        var ret = type_void;
                        if (typespec->func.ret != null) ret = resolve_typespec(typespec->func.ret);
                        result = type_func((Type**) args->_begin, args->count, ret);
                    }
                    finally
                    {
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

        private readonly PtrBuffer* ordered_syms = PtrBuffer.Create(1 << 16);

        private void complete_type(Type* type)
        {
            if (type->kind == TYPE_COMPLETING)
            {
                fatal("Type completion cycle");
                return;
            }

            if (type->kind != TYPE_INCOMPLETE) return;
            type->kind = TYPE_COMPLETING;
            var decl = type->sym->decl;
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            var fields = Buffer<TypeField>.Create();
            for (var i = 0; i < decl->aggregate.num_items; i++)
            {
                var item = decl->aggregate.items[i];
                var item_type = resolve_typespec(item.type);
                complete_type(item_type);
                for (var j = 0; j < item.num_names; j++)
                    fields.Add(new TypeField {name = item.names[j], type = item_type});
            }

            if (fields.count == 0)
                fatal("No fields");
            if (duplicate_fields(fields._begin, fields.count))
                fatal("Duplicate fields");
            if (decl->kind == DECL_STRUCT)
            {
                type_complete_struct(type, fields._begin, fields.count);
            }
            else
            {
                assert(decl->kind == DECL_UNION);
                type_complete_union(type, fields._begin, fields.count);
            }

            ordered_syms->Add(type->sym);
        }

        private Type* resolve_decl_type(Decl* decl)
        {
            assert(decl->kind == DECL_TYPEDEF);
            return resolve_typespec(decl->typedef_decl.type);
        }

        private Type* resolve_decl_var(Decl* decl)
        {
            assert(decl->kind == DECL_VAR);
            Type* type = null;
            if (decl->var.type != null) type = resolve_typespec(decl->var.type);
            if (decl->var.expr != null)
            {
                var result = resolve_expected_expr(decl->var.expr, type);
                if (type != null && result.type != type) fatal("Declared var type does not match inferred type");
                type = result.type;
            }

            complete_type(type);
            return type;
        }

        private Type* resolve_decl_const(Decl* decl, long* val)
        {
            assert(decl->kind == DECL_CONST);
            var result = resolve_expr(decl->const_decl.expr);
            if (!result.is_const)
                fatal("Initializer for const is not a constant expression");
            *val = result.val;
            return result.type;
        }

        private Type* resolve_decl_func(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            var @params = PtrBuffer.GetPooledBuffer();
            try
            {
                for (var i = 0; i < decl->func.num_params; i++)
                    @params->Add(resolve_typespec(decl->func.@params[i].type));
                var ret_type = type_void;
                if (decl->func.ret_type != null)
                    ret_type = resolve_typespec(decl->func.ret_type);

                return type_func((Type**) @params->_begin, @params->count, ret_type);
            }
            finally
            {
                @params->Release();
            }
        }


        private void resolve_cond_expr(Expr* expr)
        {
            var cond = resolve_expr(expr);
            if (cond.type != type_int) fatal("Conditional expression must have type long");
        }

        private void resolve_stmt_block(StmtList block, Type* ret_type)
        {
            var start = sym_enter();
            for (var i = 0; i < block.num_stmts; i++) resolve_stmt(block.stmts[i], ret_type);
            sym_leave(start);
        }

        private void resolve_stmt(Stmt* stmt, Type* ret_type)
        {
            switch (stmt->kind)
            {
                case STMT_RETURN:
                    if (stmt->expr != null)
                    {
                        var result = resolve_expected_expr(stmt->expr, ret_type);
                        if (result.type != ret_type) fatal("Return type mismatch");
                    }
                    else
                    {
                        if (ret_type != type_void)
                            fatal("Empty return expression for function with non-void return type");
                    }

                    break;
                case STMT_BREAK:
                case STMT_CONTINUE:
                    // Do nothing
                    break;
                case STMT_BLOCK:
                    resolve_stmt_block(stmt->block, ret_type);
                    break;
                case STMT_IF:
                    resolve_cond_expr(stmt->if_stmt.cond);
                    resolve_stmt_block(stmt->if_stmt.then_block, ret_type);
                    for (var i = 0; i < stmt->if_stmt.num_elseifs; i++)
                    {
                        var elseif = stmt->if_stmt.elseifs[i];
                        resolve_cond_expr(elseif->cond);
                        resolve_stmt_block(elseif->block, ret_type);
                    }

                    if (stmt->if_stmt.else_block.stmts != null) resolve_stmt_block(stmt->if_stmt.else_block, ret_type);
                    break;
                case STMT_WHILE:
                case STMT_DO_WHILE:
                    resolve_cond_expr(stmt->while_stmt.cond);
                    resolve_stmt_block(stmt->while_stmt.block, ret_type);
                    break;
                case STMT_FOR:
                {
                    var sym = sym_enter();
                    resolve_stmt(stmt->for_stmt.init, ret_type);
                    resolve_cond_expr(stmt->for_stmt.cond);
                    resolve_stmt_block(stmt->for_stmt.block, ret_type);
                    resolve_stmt(stmt->for_stmt.next, ret_type);
                    sym_leave(sym);
                    break;
                }

                case STMT_SWITCH:
                {
                    var result = resolve_expr(stmt->switch_stmt.expr);
                    for (var i = 0; i < stmt->switch_stmt.num_cases; i++)
                    {
                        var switch_case = stmt->switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_exprs; j++)
                        {
                            var case_result = resolve_expr(switch_case.exprs[j]);
                            if (case_result.type != result.type) fatal("Switch case expression type mismatch");
                            resolve_stmt_block(switch_case.block, ret_type);
                        }
                    }

                    break;
                }

                case STMT_ASSIGN:
                {
                    var left = resolve_expr(stmt->assign.left);
                    if (stmt->assign.right != null)
                    {
                        var right = resolve_expected_expr(stmt->assign.right, left.type);
                        if (left.type != right.type) fatal("Left/right types do not match in assignment statement");
                    }

                    if (!left.is_lvalue) fatal("Cannot assign to non-lvalue");
                    if (stmt->assign.op != TOKEN_ASSIGN && left.type != type_int)
                        fatal("Can only use assignment operators with type long");
                    break;
                }

                case STMT_INIT:
                    sym_push_var(stmt->init.name, resolve_expr(stmt->init.expr).type);
                    break;
                case STMT_EXPR:
                    resolve_expr(stmt->expr);
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void resolve_func_body(Sym* sym)
        {
            var decl = sym->decl;
            assert(decl->kind == DECL_FUNC);
            assert(sym->state == SYM_RESOLVED);
            var scope = sym_enter();
            for (var i = 0; i < decl->func.num_params; i++)
            {
                var param = decl->func.@params[i];
                sym_push_var(param.name, resolve_typespec(param.type));
            }

            resolve_stmt_block(decl->func.block, resolve_typespec(decl->func.ret_type));
            sym_leave(scope);
        }

        private void resolve_sym(Sym* sym)
        {
            if (sym->state == SYM_RESOLVED) return;

            if (sym->state == SYM_RESOLVING)
            {
                fatal("Cyclic dependency");
                return;
            }

            assert(sym->state == SYM_UNRESOLVED);
            sym->state = SYM_RESOLVING;
            switch (sym->kind)
            {
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
            ordered_syms->Add(sym);
        }

        private void finalize_sym(Sym* sym)
        {
            resolve_sym(sym);
            if (sym->kind == SYM_TYPE)
                complete_type(sym->type);
            else if (sym->kind == SYM_FUNC) resolve_func_body(sym);
        }

        private Sym* resolve_name(char* name)
        {
            var sym = sym_get(name);
            if (sym == null)
            {
                fatal("Non-existent name");
                return null;
            }

            resolve_sym(sym);
            return sym;
        }


        private Operand resolve_expr_field(Expr* expr)
        {
            assert(expr->kind == EXPR_FIELD);
            var left = resolve_expr(expr->field.expr);
            var type = left.type;
            complete_type(type);
            if (type->kind != TYPE_STRUCT && type->kind != TYPE_UNION)
            {
                fatal("Can only access fields on aggregate types");
                return default;
            }

            for (var i = 0; i < type->aggregate.num_fields; i++)
            {
                var field = type->aggregate.fields[i];
                if (field.name == expr->field.name)
                    return left.is_lvalue ? operand_lvalue(field.type) : operand_rvalue(field.type);
            }

            fatal("No field named '{0}'", new string(expr->field.name));
            return default;
        }

        private Operand ptr_decay(Operand expr)
        {
            if (expr.type->kind == TYPE_ARRAY)
                return operand_rvalue(type_ptr(expr.type->array.elem));
            return expr;
        }


        private long eval_int_unary(TokenKind op, long val)
        {
            switch (op)
            {
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
                    return 0;
            }
        }

        private long eval_int_binary(TokenKind op, long left, long right)
        {
            switch (op)
            {
                case TOKEN_MUL:
                    return left * right;
                case TOKEN_DIV:
                    return right != 0 ? left / right : 0;
                case TOKEN_MOD:
                    return right != 0 ? left % right : 0;
                case TOKEN_AND:
                    return left & right;
                // TODO: Don't allow UB in shifts, etc
                case TOKEN_LSHIFT:
                    return left << (int) right;
                case TOKEN_RSHIFT:
                    return left >> (int) right;
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
                // TODO: Probably handle logical AND/OR separately
                case TOKEN_AND_AND:
                    return left & right;
                case TOKEN_OR_OR:
                    return left | right;
                default:
                    assert(false);
                    return 0;
            }
        }

        private Operand resolve_expr_name(Expr* expr)
        {
            assert(expr->kind == EXPR_NAME);
            var sym = resolve_name(expr->name);
            if (sym->kind == SYM_VAR) return operand_lvalue(sym->type);

            if (sym->kind == SYM_CONST) return operand_const(sym->val);

            if (sym->kind == SYM_FUNC) return operand_rvalue(sym->type);

            fatal("{0} must denote a var func or const", new string(expr->name));
            return default;
        }

        private Operand resolve_expr_unary(Expr* expr)
        {
            assert(expr->kind == EXPR_UNARY);
            var operand = resolve_expr(expr->unary.expr);
            var type = operand.type;
            switch (expr->unary.op)
            {
                case TOKEN_MUL:
                    operand = ptr_decay(operand);
                    if (type->kind != TYPE_PTR)
                        fatal("Cannot deref non-ptr type");

                    return operand_lvalue(type->ptr.elem);
                case TOKEN_AND:
                    if (!operand.is_lvalue)
                        fatal("Cannot take address of non-lvalue");

                    return operand_rvalue(type_ptr(type));
                default:
                    if (type->kind != TYPE_INT)
                        fatal("Can only use unary {0} with ints", token_kind_name(expr->unary.op));

                    if (operand.is_const)
                        return operand_const(eval_int_unary(expr->unary.op, operand.val));
                    else
                        return operand_rvalue(type);
            }
        }

        private Operand resolve_expr_binary(Expr* expr)
        {
            assert(expr->kind == EXPR_BINARY);
            var left = resolve_expr(expr->binary.left);
            var right = resolve_expr(expr->binary.right);
            if (left.type != type_int) fatal("left operand of + must be int");
            if (right.type != left.type) fatal("left and right operand of + must have same type");

            if (left.is_const && right.is_const)
                return operand_const(eval_int_binary(expr->binary.op, left.val, right.val));
            return operand_rvalue(left.type);
        }

        private int aggregate_field_index(Type* type, char* name)
        {
            assert(type->kind == TYPE_STRUCT || type->kind == TYPE_UNION);
            for (var i = 0; i < type->aggregate.num_fields; i++)
                if (type->aggregate.fields[i].name == name)
                    return i;
            fatal("Field '{0}' in compound literal not found in struct/union", new string(name));
            return int.MaxValue;
        }

        private Operand resolve_expr_compound(Expr* expr, Type* expected_type)
        {
            assert(expr->kind == EXPR_COMPOUND);
            if (expected_type == null && expr->compound.type == null)
                fatal("Implicitly typed compound literals used in context without expected type");
            Type* type = null;
            if (expr->compound.type != null)
                type = resolve_typespec(expr->compound.type);
            else
                type = expected_type;
            complete_type(type);
            if (type->kind != TYPE_STRUCT && type->kind != TYPE_UNION && type->kind != TYPE_ARRAY)
                fatal("Compound literals can only be used with struct and array types");

            if (type->kind == TYPE_STRUCT || type->kind == TYPE_UNION)
            {
                var index = 0;
                for (var i = 0; i < expr->compound.num_fields; i++)
                {
                    var field = expr->compound.fields[i];
                    if (field.kind == FIELD_INDEX)
                        fatal("Index field initializer not allowed for struct/union compound literal");
                    else if (field.kind == FIELD_NAME) index = aggregate_field_index(type, field.name);
                    if (index >= type->aggregate.num_fields)
                        fatal("Field initializer in struct/union compound literal out of range");
                    var init = resolve_expected_expr(expr->compound.fields[i].init, type->aggregate.fields[index].type);
                    if (init.type != type->aggregate.fields[index].type) fatal("Compound literal field type mismatch");
                    index++;
                }
            }
            else
            {
                assert(type->kind == TYPE_ARRAY);
                long index = 0;
                for (var i = 0; i < expr->compound.num_fields; i++)
                {
                    var field = expr->compound.fields[i];
                    if (field.kind == FIELD_NAME)
                    {
                        fatal("Named field initializer not allowed for array compound literals");
                    }
                    else if (field.kind == FIELD_INDEX)
                    {
                        var result = resolve_const_expr(field.index);
                        if (result < 0) fatal("Field initializer index cannot be negative");
                        index = result;
                    }

                    if (index >= type->array.size) fatal("Field initializer in array compound literal out of range");
                    var init = resolve_expected_expr(expr->compound.fields[i].init, type->array.elem);
                    if (init.type != type->array.elem) fatal("Compound literal element type mismatch");
                    index++;
                }
            }

            return operand_rvalue(type);
        }

        private Operand resolve_expr_call(Expr* expr)
        {
            assert(expr->kind == EXPR_CALL);
            var func = resolve_expr(expr->call.expr);
            if (func.type->kind != TYPE_FUNC) fatal("Trying to call non-function value");
            if (expr->call.num_args != func.type->func.num_params)
                fatal("Tried to call function with wrong number of arguments");
            for (var i = 0; i < expr->call.num_args; i++)
            {
                var param_type = func.type->func.@params[i];
                var arg = resolve_expected_expr(expr->call.args[i], param_type);
                if (arg.type != param_type) fatal("Call argument expression type doesn't match expected param type");
            }

            return operand_rvalue(func.type->func.ret);
        }

        private Operand resolve_expr_ternary(Expr* expr, Type* expected_type)
        {
            assert(expr->kind == EXPR_TERNARY);
            var cond = ptr_decay(resolve_expr(expr->ternary.cond));
            if (cond.type->kind != TYPE_INT && cond.type->kind != TYPE_PTR)
                fatal("Ternary cond expression must have type long or ptr");
            var then_expr = ptr_decay(resolve_expected_expr(expr->ternary.then_expr, expected_type));
            var else_expr = ptr_decay(resolve_expected_expr(expr->ternary.else_expr, expected_type));
            if (then_expr.type != else_expr.type) fatal("Ternary then/else expressions must have matching types");
            if (cond.is_const && then_expr.is_const && else_expr.is_const)
                return operand_const(cond.val != 0 ? then_expr.val : else_expr.val);
            return operand_rvalue(then_expr.type);
        }

        private Operand resolve_expr_index(Expr* expr)
        {
            assert(expr->kind == EXPR_INDEX);
            var operand = ptr_decay(resolve_expr(expr->index.expr));
            if (operand.type->kind != TYPE_PTR) fatal("Can only index arrays or pointers");
            var index = resolve_expr(expr->index.index);
            if (index.type->kind != TYPE_INT) fatal("Index expression must have type long");
            return operand_lvalue(operand.type->ptr.elem);
        }

        private Operand resolve_expr_cast(Expr* expr)
        {
            assert(expr->kind == EXPR_CAST);
            var type = resolve_typespec(expr->cast.type);
            var result = ptr_decay(resolve_expr(expr->cast.expr));
            if (type->kind == TYPE_PTR)
            {
                if (result.type->kind != TYPE_PTR && result.type->kind != TYPE_INT)
                    fatal("Invalid cast to pointer type");
            }
            else if (type->kind == TYPE_INT)
            {
                if (result.type->kind != TYPE_PTR && result.type->kind != TYPE_INT) fatal("Invalid cast to long type");
            }
            else
            {
                fatal("Invalid target cast type");
            }

            return operand_rvalue(type);
        }

        private Operand resolve_expected_expr(Expr* expr, Type* expected_type)
        {
            Operand result;
            switch (expr->kind)
            {
                case EXPR_INT:
                    result = operand_const(expr->int_val);
                    break;
                case EXPR_FLOAT:
                    result = operand_rvalue(type_float);
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
                case EXPR_SIZEOF_EXPR:
                {
                    var type = resolve_expr(expr->sizeof_expr).type;
                    complete_type(type);
                    result = operand_const(type_sizeof(type));
                    break;
                }

                case EXPR_SIZEOF_TYPE:
                {
                    var type = resolve_typespec(expr->sizeof_type);
                    complete_type(type);
                    result = operand_const(type_sizeof(type));
                    break;
                }

                default:
                    assert(false);
                    result = default;
                    break;
            }

            if (result.type != null)
            {
                assert(expr->type == null || expr->type == result.type);
                expr->type = result.type;
            }

            return result;
        }

        private Operand resolve_expr(Expr* expr)
        {
            return resolve_expected_expr(expr, null);
        }

        private long resolve_const_expr(Expr* expr)
        {
            var result = resolve_expr(expr);
            if (!result.is_const)
                fatal("Expected constant expression");

            return result.val;
        }

        internal void sym_global_decls(DeclSet* declset)
        {
            for (var i = 0; i < declset->num_decls; i++) sym_global_decl(declset->decls[i]);
        }

        private readonly string[] code =
        {
            "union IntOrPtr { i: int; p: int*; }",
            "var u1 = IntOrPtr{i = 42}",
            "var u2 = IntOrPtr{p = (:int*)42}",
            "var i: int",
            "struct Vector { x, y: int; }",
            "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }",
            "func f2(n: int): int { return 2*n; }",
            "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }",
            "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }",
            "func f5(x: int): int { switch(x) { case 0: case 1: return 42; case 3: default: return -1; } }",
            "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }",
            "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }"
            /*
            "var i: int",
            "func add(v: Vector, w: Vector): Vector { return {v.x + w.x, v.y + w.y}; }",
            "var a: int[256] = {1, 2, ['a'] = 42, [255] = 123}",
            "var v: Vector = 0 ? {1,2} : {3,4}",
            "var vs: Vector[2][2] = {{{1,2},{3,4}}, {{5,6},{7,8}}}",
            "struct A { c: char; }",
            "struct B { i: int; }",
            "struct C { c: char; a: A; }",
            "struct D { c: char; b: B; }",
            "func print(v: Vector) { printf(\"{%d, %d}\", v.x, v.y); }",
            "var x = add({1,2}, {3,4})",
            "var v: Vector = {1,2}",
            "var w = Vector{3,4}",
            "var p: void*",
            "var i = (:int)p + 1",
            "var fp: func(Vector)",
            "struct Dup { x: int; x: int; }",
            "var a: int[3] = {1,2,3}",
            "var b: int[4]",
            "var p = &a[1]",
            "var i = p[1]",
            "var j = *p",
            "const n = sizeof(a)",
            "const m = sizeof(&a[0])",
            "const l = sizeof(1 ? a : b)",
            "var pi = 3.14",
            "var name = \"Per\"",
            "var v = Vector{1,2}",
            "var j = (:int)p",
            "var q = (:int*)j",
            "const i = 42",
            "const j = +i",
            "const k = -i",
            "const a = 1000/((2*3-5) << 1)",
            "const b = !0",
            "const c = ~100 + 1 == -100",
            "const k = 1 ? 2 : 3",
            "union IntOrPtr { i: int; p: int*; }",
            "var i = 42",
            "var u = IntOrPtr{i, &i}",
            "const n = 1+sizeof(p)",
            "var p: T*",
            "var u = *p",
            "struct T { a: int[n]; }",
            "var r = &t.a",
            "var t: T",
            "typedef S = int[n+m]",
            "const m = sizeof(t.a)",
            "var i = n+m",
            "var q = &i",
            "const n = sizeof(x)",
            "var x: T",
            "struct T { s: S*; }",
            "struct S { t: T[n]; }",
    */
        };

        private void init_global_syms()
        {
            sym_global_type(_I("void"), type_void);
            sym_global_type(_I("char"), type_char);
            sym_global_type(_I("int"), type_int);
            sym_global_type(_I("float"), type_float);

            sym_global_func(_I("puts"), type_func(new[] {type_ptr(type_char)}, 1, type_int));
        }

        private void finalize_syms()
        {
            for (var it = (Sym**) global_syms_buf->_begin; it != global_syms_buf->_top; it++)
            {
                var sym = *it;
                if (sym->decl != null) finalize_sym(sym);
            }
        }

        private void resolve_test()
        {
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
            fixed (Type** t = &type_int)
            {
                var int_int_func = type_func(t, 1, type_int);
                assert(type_func(t, 1, type_int) == int_int_func);

                var int_func = type_func((Type**) null, 0, type_int);
                assert(int_int_func != int_func);
                assert(int_func == type_func((Type**) null, 0, type_int));
            }

            init_global_syms();
            for (var i = 0; i < code.Length; i++)
            {
                init_stream(code[i].ToPtr(), null);
                var decl = parse_decl();
                sym_global_decl(decl);
            }

            finalize_syms();
            Console.WriteLine();
            for (var sym = (Sym**) ordered_syms->_begin; sym != ordered_syms->_top; sym++)
            {
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
        public long val;
    }

    internal unsafe struct CachedPtrType
    {
        public Type* elem;
        public Type* ptr;
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
        public Type* ret;
        public Type* func;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct ConstEntity
    {
        [FieldOffset(0)] public Type* type;
        [FieldOffset(Ion.PTR_SIZE)] public long int_val;
        [FieldOffset(Ion.PTR_SIZE)] public double float_val;
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
        public long val;
    }

    internal enum TypeKind
    {
        TYPE_NONE,
        TYPE_INCOMPLETE,
        TYPE_COMPLETING,
        TYPE_VOID,
        TYPE_CHAR,
        TYPE_INT,
        TYPE_FLOAT,
        TYPE_PTR,
        TYPE_ARRAY,
        TYPE_STRUCT,
        TYPE_UNION,
        TYPE_ENUM,
        TYPE_FUNC
    }


    internal unsafe struct TypeField
    {
        public char* name;
        public Type* type;
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
            public Type* ret;
        }
    }
}