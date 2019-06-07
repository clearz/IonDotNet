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
    using static SymKind;
    using static TypespecKind;
    using static ExprKind;
    using static TokenKind;
    using static StmtKind;
    using static CompoundFieldKind;

    #region Typedefs

#if X64
    using size_t = Int64;
#else
    using size_t = System.Int32;
#endif

    #endregion

    unsafe partial class Ion
    {
        Buffer<CachedPtrType> cached_ptr_types = Buffer<CachedPtrType>.Create();
        Buffer<CachedArrayType> cached_array_types = Buffer<CachedArrayType>.Create();
        Buffer<CachedFuncType> cached_func_types = Buffer<CachedFuncType>.Create();
        public const int MAX_LOCAL_SYMS = 1024;

        private readonly PtrBuffer* global_syms = PtrBuffer.Create();
        private Buffer<Sym> local_syms = Buffer<Sym>.Create(MAX_LOCAL_SYMS);
        static Type* type_alloc(TypeKind kind)
        {
            Type* type = (Type*)Marshal.AllocHGlobal(sizeof(Type));// Marshal.AllocHGlobal(sizeof(Type)); //(1, sizeof(Type));
            Unsafe.InitBlock(type, 0, (uint)sizeof(Type));
            type->kind = kind;
            return type;
        }

        private readonly Type* type_int = type_alloc(TYPE_INT);
        private readonly Type* type_float = type_alloc(TYPE_FLOAT);
        private readonly Type* type_void = type_alloc(TYPE_VOID);
        private readonly Type* type_char = type_alloc(TYPE_CHAR);
#if X64
        internal const int PTR_SIZE = 8;
#else
        internal const int PTR_SIZE = 4;
#endif
        const size_t PTR_ALIGN = 8;

        size_t type_sizeof(Type* type)
        {
            assert(type->kind > TYPE_COMPLETING);
            assert(type->size != 0);
            return type->size;
        }

        size_t type_alignof(Type* type)
        {
            assert(type->kind > TYPE_COMPLETING);
            assert(IS_POW2(type->align));
            return type->align;
        }


        Type* type_ptr(Type* elem)
        {
            for (CachedPtrType* it = (CachedPtrType*)cached_ptr_types._begin; it != cached_ptr_types._top; it++)
            {
                if (it->elem == elem)
                {
                    return it->ptr;
                }
            }

            Type* type = type_alloc(TYPE_PTR);
            type->size = PTR_SIZE;
            type->align = PTR_ALIGN;
            type->ptr.elem = elem;

            cached_ptr_types.Add(new CachedPtrType { elem = elem, ptr = type });
            return type;
        }


        Type* type_array(Type* elem, size_t size)
        {
            for (CachedArrayType* it = (CachedArrayType*)cached_array_types._begin; it != cached_array_types._top; it++)
            {
                if (it->elem == elem && it->size == size)
                {
                    return it->array;
                }
            }

            complete_type(elem);
            Type* type = type_alloc(TYPE_ARRAY);
            type->size = size * type_sizeof(elem);
            type->align = type_alignof(elem);
            type->array.elem = elem;
            type->array.size = size;
            cached_array_types.Add(new CachedArrayType { elem = elem, array = type, size = size });
            return type;
        }


        Type* type_func(Type** @params, int num_params, Type* ret)
        {
            for (CachedFuncType* it = cached_func_types._begin; it != cached_func_types._top; it++)
            {
                if (it->num_params == num_params && it->ret == ret)
                {
                    bool match = true;
                    for (size_t i = 0; i < num_params; i++)
                    {
                        if (it->@params[i] != @params[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        return it->func;
                    }
                }
            }

            Type* type = type_alloc(TYPE_FUNC);
            type->size = PTR_SIZE;
            type->align = PTR_ALIGN;
            if (num_params > 0)
            {
                type->func.@params = (Type**)Marshal.AllocHGlobal(sizeof(Type*) * num_params);
                Unsafe.CopyBlock(type->func.@params, @params,
                    (uint)(num_params *
                            sizeof(Type*))); //memcpy(t->func.@params, @params, num_params * sizeof(Type*));
                type->func.num_params = num_params;
            }

            type->func.ret = ret;
            cached_func_types.Add(new CachedFuncType { func = type, num_params = num_params, ret = ret, @params = @params });
            return type;
        }

        // TODO: This probably shouldn't use an O(n^2) algorithm
        bool duplicate_fields(TypeField* fields, size_t num_fields)
        {
            for (size_t i = 0; i < num_fields; i++)
                for (size_t j = i + 1; j < num_fields; j++)
                    if (fields[i].name == fields[j].name)
                        return true;
            return false;
        }

        Type* type_complete_struct(Type* type, TypeField* fields, int num_fields)
        {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_STRUCT;
            type->size = 0;
            type->align = 0;

            for (TypeField* it = fields; it != fields + num_fields; it++)
            {
                type->size = type_sizeof(it->type) + ALIGN_UP(type->size, type_alignof(it->type));
                type->align = MAX(type->align, type_alignof(it->type));
            }
            type->aggregate.fields = (TypeField*)Marshal.AllocHGlobal(num_fields * sizeof(TypeField)); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->func.@params, fields, (uint)(num_fields * sizeof(TypeField)));//mmemcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        Type* type_complete_union(Type* type, TypeField* fields, int num_fields)
        {
            assert(type->kind == TYPE_COMPLETING);
            type->kind = TYPE_UNION;
            type->size = 0;
            for (TypeField* it = fields; it != fields + num_fields; it++)
            {
                type->size = MAX(type->size, it->type->size);
            }
            type->aggregate.fields = (TypeField*)Marshal.AllocHGlobal(sizeof(TypeField) * num_fields); //(num_fields, sizeof(TypeField));
            Unsafe.CopyBlock(type->aggregate.fields, fields, (uint)(num_fields * sizeof(TypeField))); // memcpy(t->aggregate.fields, fields, num_fields * sizeof(TypeField));
            type->aggregate.num_fields = num_fields;
            return type;
        }

        Type* type_incomplete(Sym* sym)
        {
            Type* type = type_alloc(TYPE_INCOMPLETE);
            type->sym = sym;
            return type;
        }



        Sym* sym_new(SymKind kind, char* name, Decl* decl)
        {
            Sym* sym = (Sym*)Marshal.AllocHGlobal(sizeof(Sym));
            Unsafe.InitBlock(sym, 0, (uint)sizeof(Sym));
            sym->kind = kind;
            sym->name = name;
            sym->decl = decl;
            return sym;
        }

        Sym* sym_decl(Decl* decl)
        {
            SymKind kind = SYM_NONE;
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
            Sym* sym = sym_new(kind, decl->name, decl);
            if (decl->kind == DECL_STRUCT || decl->kind == DECL_UNION)
            {
                sym->state = SYM_RESOLVED;
                sym->type = type_incomplete(sym);
            }
            return sym;
        }

        Sym* sym_enum_const(char* name, Decl* decl)
        {
            return sym_new(SYM_ENUM_CONST, name, decl);
        }

        Sym* sym_get(char* name)
        {
            for (Sym* it = local_syms._top; it >= local_syms._begin; it--)
            {
                Sym* sym = it - 1;
                if (sym->name == name)
                {
                    return sym;
                }
            }
            for (Sym** it = (Sym**)global_syms->_begin; it != global_syms->_top; it++)
            {
                Sym* sym = *it;
                if (sym->name == name)
                {
                    return sym;
                }
            }
            return null;
        }

        void sym_push_var(char* name, Type* type)
        {
            if (local_syms._top == local_syms._begin + MAX_LOCAL_SYMS)
            {
                fatal("Too many local symbols");
            }

            local_syms.Add(new Sym
            {
                name = name,
                kind = SYM_VAR,
                state = SYM_RESOLVED,
                type = type,
            });

        }

        Sym* sym_enter()
        {
            return local_syms._top;
        }

        void sym_leave(Sym* sym)
        {
            local_syms._top = sym;
        }

        Sym* sym_global_decl(Decl* decl)
        {
            Sym* sym = sym_decl(decl);
            global_syms->Add(sym);
            if (decl->kind == DECL_ENUM)
            {
                for (size_t i = 0; i < decl->enum_decl.num_items; i++)
                {
                    global_syms->Add(sym_enum_const(decl->enum_decl.items[i].name, decl));
                }
            }
            return sym;
        }

        Sym* sym_global_type(char* name, Type* type)
        {
            Sym* sym = sym_new(SYM_TYPE, name, null);
            sym->state = SYM_RESOLVED;
            sym->type = type;
            global_syms->Add(sym);
            return sym;
        }

        void resolve_decl(Decl* decl)
        {
            switch (decl->kind)
            {
                case DECL_CONST:
                    break;
            }
        }

        private readonly ResolvedExpr resolved_null;

        ResolvedExpr resolved_rvalue(Type* type)
        {
            return new ResolvedExpr { type = type };
        }

        ResolvedExpr resolved_lvalue(Type* type)
        {
            return new ResolvedExpr { type = type, is_lvalue = true };
        }

        ResolvedExpr resolved_const(size_t val)
        {
            return new ResolvedExpr { type = type_int, is_const = true, val = val, };
        }

        Type* resolve_typespec(Typespec* typespec)
        {
            if (typespec == null)
                return type_void;

            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                    {
                        Sym* sym = resolve_name(typespec->name);
                        if (sym->kind != SYM_TYPE)
                        {
                            fatal("{0} must denote a type", new string(typespec->name));
                            return null;
                        }
                        return sym->type;
                    }
                case TYPESPEC_PTR:
                    return type_ptr(resolve_typespec(typespec->ptr.elem));
                case TYPESPEC_ARRAY:
                    size_t size = resolve_const_expr(typespec->array.size);
                    if (size < 0)
                        fatal("Negative array size");

                    return type_array(resolve_typespec(typespec->array.elem), size);
                case TYPESPEC_FUNC:
                    {
                        //Type** args = null;
                        var args = PtrBuffer.Create();
                        for (size_t i = 0; i < typespec->func.num_args; i++)
                        {
                            args->Add(resolve_typespec(typespec->func.args[i]));
                        }
                        Type* ret = type_void;
                        if (typespec->func.ret != null)
                        {
                            ret = resolve_typespec(typespec->func.ret);
                        }
                        return type_func((Type**)args->_begin, args->count, ret);
                    }
                default:
                    assert(false);
                    return null;
            }
        }

        private PtrBuffer* ordered_syms = PtrBuffer.Create();

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
            Decl* decl = type->sym->decl;
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            var fields = Buffer<TypeField>.Create();
            for (size_t i = 0; i < decl->aggregate.num_items; i++)
            {
                AggregateItem item = decl->aggregate.items[i];
                Type* item_type = resolve_typespec(item.type);
                complete_type(item_type);
                for (size_t j = 0; j < item.num_names; j++)
                {
                    fields.Add(new TypeField { name = item.names[j], type = item_type });
                }
            }
            if (fields.count == 0)
                fatal("No fields");
            if (duplicate_fields((TypeField*)fields._begin, fields.count))
                fatal("Duplicate fields");
            if (decl->kind == DECL_STRUCT)
            {
                type_complete_struct(type, (TypeField*)fields._begin, fields.count);
            }
            else
            {
                assert(decl->kind == DECL_UNION);
                type_complete_union(type, (TypeField*)fields._begin, fields.count);
            }
            ordered_syms->Add(type->sym);
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
                ResolvedExpr result = resolve_expected_expr(decl->var.expr, type);
                if (type != null && result.type != type)
                {
                    fatal("Declared var type does not match inferred type");
                }
                type = result.type;
            }
            complete_type(type);
            return type;
        }

        Type* resolve_decl_const(Decl* decl, size_t* val)
        {
            assert(decl->kind == DECL_CONST);
            ResolvedExpr result = resolve_expr(decl->const_decl.expr);
            if (!result.is_const)
                fatal("Initializer for const is not a constant expression");
            *val = result.val;
            return result.type;
        }

        Type* resolve_decl_func(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            var @params = PtrBuffer.Create();
            for (size_t i = 0; i < decl->func.num_params; i++)
                @params->Add(resolve_typespec(decl->func.@params[i].type));
            Type* ret_type = type_void;
            if (decl->func.ret_type != null)
                ret_type = resolve_typespec(decl->func.ret_type);

            return type_func((Type**)@params->_begin, @params->count, ret_type);
        }


        void resolve_cond_expr(Expr* expr)
        {
            ResolvedExpr cond = resolve_expr(expr);
            if (cond.type != type_int)
            {
                fatal("Conditional expression must have type int");
            }
        }

        void resolve_stmt_block(StmtBlock block, Type* ret_type)
        {
            Sym* start = sym_enter();
            for (size_t i = 0; i < block.num_stmts; i++)
            {
                resolve_stmt(block.stmts[i], ret_type);
            }
            sym_leave(start);
        }

        void resolve_stmt(Stmt* stmt, Type* ret_type)
        {
            switch (stmt->kind)
            {
                case STMT_RETURN:
                    if (stmt->expr != null)
                    {
                        ResolvedExpr result = resolve_expected_expr(stmt->expr, ret_type);
                        if (result.type != ret_type)
                        {
                            fatal("Return type mismatch");
                        }
                    }
                    else
                    {
                        if (ret_type != type_void)
                        {
                            fatal("Empty return expression for function with non-void return type");
                        }
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
                    for (size_t i = 0; i < stmt->if_stmt.num_elseifs; i++)
                    {
                        ElseIf elseif = stmt->if_stmt.elseifs[i];
                        resolve_cond_expr(elseif.cond);
                        resolve_stmt_block(elseif.block, ret_type);
                    }
                    if (stmt->if_stmt.else_block.stmts != null)
                    {
                        resolve_stmt_block(stmt->if_stmt.else_block, ret_type);
                    }
                    break;
                case STMT_WHILE:
                case STMT_DO_WHILE:
                    resolve_cond_expr(stmt->while_stmt.cond);
                    resolve_stmt_block(stmt->while_stmt.block, ret_type);
                    break;
                case STMT_FOR:
                    {
                        Sym* sym = sym_enter();
                        resolve_stmt(stmt->for_stmt.init, ret_type);
                        resolve_cond_expr(stmt->for_stmt.cond);
                        resolve_stmt_block(stmt->for_stmt.block, ret_type);
                        resolve_stmt(stmt->for_stmt.next, ret_type);
                        sym_leave(sym);
                        break;
                    }
                case STMT_SWITCH:
                    {
                        ResolvedExpr result = resolve_expr(stmt->switch_stmt.expr);
                        for (size_t i = 0; i < stmt->switch_stmt.num_cases; i++)
                        {
                            SwitchCase switch_case = stmt->switch_stmt.cases[i];
                            for (size_t j = 0; j < switch_case.num_exprs; j++)
                            {
                                ResolvedExpr case_result = resolve_expr(switch_case.exprs[j]);
                                if (case_result.type != result.type)
                                {
                                    fatal("Switch case expression type mismatch");
                                }
                                resolve_stmt_block(switch_case.block, ret_type);
                            }
                        }
                        break;
                    }
                case STMT_ASSIGN:
                    {
                        ResolvedExpr left = resolve_expr(stmt->assign.left);
                        if (stmt->assign.right != null)
                        {
                            ResolvedExpr right = resolve_expected_expr(stmt->assign.right, left.type);
                            if (left.type != right.type)
                            {
                                fatal("Left/right types do not match in assignment statement");
                            }
                        }
                        if (!left.is_lvalue)
                        {
                            fatal("Cannot assign to non-lvalue");
                        }
                        if (stmt->assign.op != TOKEN_ASSIGN && left.type != type_int)
                        {
                            fatal("Can only use assignment operators with type int");
                        }
                        break;
                    }
                case STMT_INIT:
                    sym_push_var(stmt->init.name, resolve_expr(stmt->init.expr).type);
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void resolve_func(Sym* sym)
        {
            Decl* decl = sym->decl;
            assert(decl->kind == DECL_FUNC);
            assert(sym->state == SYM_RESOLVED);
            Sym* start = sym_enter();
            for (size_t i = 0; i < decl->func.num_params; i++)
            {
                FuncParam param = decl->func.@params[i];
                sym_push_var(param.name, resolve_typespec(param.type));
            }

            resolve_stmt_block(decl->func.block, resolve_typespec(decl->func.ret_type));
            sym_leave(start);
        }

        void resolve_sym(Sym* sym)
        {
            if (sym->state == SYM_RESOLVED)
            {
                return;
            }
            else if (sym->state == SYM_RESOLVING)
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

        void complete_entity(Sym* sym)
        {
            resolve_sym(sym);
            if (sym->kind == SYM_TYPE)
                complete_type(sym->type);
            else if (sym->kind == SYM_FUNC)
            {
                resolve_func(sym);
            }
        }

        Sym* resolve_name(char* name)
        {
            Sym* sym = sym_get(name);
            if (sym == null)
            {
                fatal("Non-existent name");
                return null;
            }
            resolve_sym(sym);
            return sym;
        }


        ResolvedExpr resolve_expr_field(Expr* expr)
        {
            assert(expr->kind == EXPR_FIELD);
            ResolvedExpr left = resolve_expr(expr->field.expr);
            Type* type = left.type;
            complete_type(type);
            if (type->kind != TYPE_STRUCT && type->kind != TYPE_UNION)
            {
                fatal("Can only access fields on aggregate types");
                return resolved_null;
            }
            for (size_t i = 0; i < type->aggregate.num_fields; i++)
            {
                TypeField field = type->aggregate.fields[i];
                if (field.name == expr->field.name)
                {
                    return left.is_lvalue ? resolved_lvalue(field.type) : resolved_rvalue(field.type);
                }
            }
            fatal("No field named '{0}'", new String(expr->field.name));
            return resolved_null;
        }

        ResolvedExpr ptr_decay(ResolvedExpr expr)
        {
            if (expr.type->kind == TYPE_ARRAY)
            {
                return resolved_rvalue(type_ptr(expr.type->array.elem));
            }
            else
            {
                return expr;
            }
        }


        ResolvedExpr resolve_expr_name(Expr* expr)
        {
            assert(expr->kind == EXPR_NAME);
            Sym* sym = resolve_name(expr->name);
            if (sym->kind == SYM_VAR)
                return resolved_lvalue(sym->type);
            else if (sym->kind == SYM_CONST)
                return resolved_const(sym->val);
            else if (sym->kind == SYM_FUNC)
            {
                return resolved_rvalue(sym->type);
            }
            else
            {
                fatal("{0} must denote a var func or const", new String(expr->name));
                return resolved_null;
            }
        }

        size_t eval_int_unary(TokenKind op, size_t val)
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

        size_t eval_int_binary(TokenKind op, size_t left, size_t right)
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

        ResolvedExpr resolve_expr_unary(Expr* expr)
        {
            assert(expr->kind == EXPR_UNARY);
            ResolvedExpr operand = resolve_expr(expr->unary.expr);
            Type* type = operand.type;
            switch (expr->unary.op)
            {
                case TOKEN_MUL:
                    operand = ptr_decay(operand);
                    if (type->kind != TYPE_PTR)
                        fatal("Cannot deref non-ptr type");

                    return resolved_lvalue(type->ptr.elem);
                case TOKEN_AND:
                    if (!operand.is_lvalue)
                        fatal("Cannot take address of non-lvalue");

                    return resolved_rvalue(type_ptr(type));
                default:
                    if (type->kind != TYPE_INT)
                        fatal("Can only use unary {0} with ints", new String(token_kind_name(expr->unary.op)));

                    if (operand.is_const)
                    {
                        return resolved_const(eval_int_unary(expr->unary.op, operand.val));
                    }
                    else
                        return resolved_rvalue(type);
            }
        }

        ResolvedExpr resolve_expr_binary(Expr* expr)
        {
            assert(expr->kind == EXPR_BINARY);
            ResolvedExpr left = resolve_expr(expr->binary.left);
            ResolvedExpr right = resolve_expr(expr->binary.right);
            if (left.type != type_int)
            {
                fatal("left operand of + must be int");
            }
            if (right.type != left.type)
            {
                fatal("left and right operand of + must have same type");
            }

            if (left.is_const && right.is_const)
                return resolved_const(eval_int_binary(expr->binary.op, left.val, right.val));
            else
                return resolved_rvalue(left.type);
        }
        size_t aggregate_field_index(Type* type, char* name)
        {
            assert(type->kind == TYPE_STRUCT || type->kind == TYPE_UNION);
            for (size_t i = 0; i < type->aggregate.num_fields; i++)
            {
                if (type->aggregate.fields[i].name == name)
                {
                    return i;
                }
            }
            fatal("Field '{0}' in compound literal not found in struct/union", new String(name));
            return size_t.MaxValue;
        }

        ResolvedExpr resolve_expr_compound(Expr* expr, Type* expected_type)
        {
            assert(expr->kind == EXPR_COMPOUND);
            if (expected_type == null && expr->compound.type == null)
            {
                fatal("Implicitly typed compound literals used in context without expected type");
            }
            Type* type = null;
            if (expr->compound.type != null)
            {
                type = resolve_typespec(expr->compound.type);
            }
            else
            {
                type = expected_type;
            }
            complete_type(type);
            if (type->kind != TYPE_STRUCT && type->kind != TYPE_UNION && type->kind != TYPE_ARRAY)
            {
                fatal("Compound literals can only be used with struct and array types");
            }

            if (type->kind == TYPE_STRUCT || type->kind == TYPE_UNION)
            {
                size_t index = 0;
                for (size_t i = 0; i < expr->compound.num_fields; i++)
                {
                    CompoundField field = expr->compound.fields[i];
                    if (field.kind == FIELD_INDEX)
                    {
                        fatal("Index field initializer not allowed for struct/union compound literal");
                    }
                    else if (field.kind == FIELD_NAME)
                    {
                        index = aggregate_field_index(type, field.name);
                    }
                    if (index >= type->aggregate.num_fields)
                    {
                        fatal("Field initializer in struct/union compound literal out of range");
                    }
                    ResolvedExpr init = resolve_expected_expr(expr->compound.fields[i].init, type->aggregate.fields[index].type);
                    if (init.type != type->aggregate.fields[index].type)
                    {
                        fatal("Compound literal field type mismatch");
                    }
                    index++;
                }
            }
            else
            {
                assert(type->kind == TYPE_ARRAY);
                size_t index = 0;
                for (size_t i = 0; i < expr->compound.num_fields; i++)
                {
                    CompoundField field = expr->compound.fields[i];
                    if (field.kind == FIELD_NAME)
                    {
                        fatal("Named field initializer not allowed for array compound literals");
                    }
                    else if (field.kind == FIELD_INDEX)
                    {
                        size_t result = resolve_const_expr(field.index);
                        if (result < 0)
                        {
                            fatal("Field initializer index cannot be negative");
                        }
                        index = result;
                    }
                    if (index >= type->array.size)
                    {
                        fatal("Field initializer in array compound literal out of range");
                    }
                    ResolvedExpr init = resolve_expected_expr(expr->compound.fields[i].init, type->array.elem);
                    if (init.type != type->array.elem)
                    {
                        fatal("Compound literal element type mismatch");
                    }
                    index++;
                }
            }
            return resolved_rvalue(type);
        }

        ResolvedExpr resolve_expr_call(Expr* expr)
        {
            assert(expr->kind == EXPR_CALL);
            ResolvedExpr func = resolve_expr(expr->call.expr);
            if (func.type->kind != TYPE_FUNC)
            {
                fatal("Trying to call non-function value");
            }
            if (expr->call.num_args != func.type->func.num_params)
            {
                fatal("Tried to call function with wrong number of arguments");
            }
            for (size_t i = 0; i < expr->call.num_args; i++)
            {
                Type* param_type = func.type->func.@params[i];
                ResolvedExpr arg = resolve_expected_expr(expr->call.args[i], param_type);
                if (arg.type != param_type)
                {
                    fatal("Call argument expression type doesn't match expected param type");
                }
            }
            return resolved_rvalue(func.type->func.ret);
        }

        ResolvedExpr resolve_expr_ternary(Expr* expr, Type* expected_type)
        {
            assert(expr->kind == EXPR_TERNARY);
            ResolvedExpr cond = ptr_decay(resolve_expr(expr->ternary.cond));
            if (cond.type->kind != TYPE_INT && cond.type->kind != TYPE_PTR)
            {
                fatal("Ternary cond expression must have type int or ptr");
            }
            ResolvedExpr then_expr = ptr_decay(resolve_expected_expr(expr->ternary.then_expr, expected_type));
            ResolvedExpr else_expr = ptr_decay(resolve_expected_expr(expr->ternary.else_expr, expected_type));
            if (then_expr.type != else_expr.type)
            {
                fatal("Ternary then/else expressions must have matching types");
            }
            if (cond.is_const && then_expr.is_const && else_expr.is_const)
            {
                return resolved_const(cond.val != 0 ? then_expr.val : else_expr.val);
            }
            else
            {
                return resolved_rvalue(then_expr.type);
            }
        }

        ResolvedExpr resolve_expr_index(Expr* expr)
        {
            assert(expr->kind == EXPR_INDEX);
            ResolvedExpr operand = ptr_decay(resolve_expr(expr->index.expr));
            if (operand.type->kind != TYPE_PTR)
            {
                fatal("Can only index arrays or pointers");
            }
            ResolvedExpr index = resolve_expr(expr->index.index);
            if (index.type->kind != TYPE_INT)
            {
                fatal("Index expression must have type int");
            }
            return resolved_lvalue(operand.type->ptr.elem);
        }

        ResolvedExpr resolve_expr_cast(Expr* expr)
        {
            assert(expr->kind == EXPR_CAST);
            Type* type = resolve_typespec(expr->cast.type);
            ResolvedExpr result = ptr_decay(resolve_expr(expr->cast.expr));
            if (type->kind == TYPE_PTR)
            {
                if (result.type->kind != TYPE_PTR && result.type->kind != TYPE_INT)
                {
                    fatal("Invalid cast to pointer type");
                }
            }
            else if (type->kind == TYPE_INT)
            {
                if (result.type->kind != TYPE_PTR && result.type->kind != TYPE_INT)
                {
                    fatal("Invalid cast to int type");
                }
            }
            else
            {
                fatal("Invalid target cast type");
            }
            return resolved_rvalue(type);
        }

        ResolvedExpr resolve_expected_expr(Expr* expr, Type* expected_type)
        {
            switch (expr->kind)
            {
                case EXPR_INT:
                    return resolved_const(expr->int_val);
                case EXPR_FLOAT:
                    return resolved_rvalue(type_float);
                case EXPR_STR:
                    return resolved_rvalue(type_ptr(type_char));
                case EXPR_NAME:
                    return resolve_expr_name(expr);
                case EXPR_CAST:
                    return resolve_expr_cast(expr);
                case EXPR_CALL:
                    return resolve_expr_call(expr);
                case EXPR_INDEX:
                    return resolve_expr_index(expr);
                case EXPR_FIELD:
                    return resolve_expr_field(expr);
                case EXPR_COMPOUND:
                    return resolve_expr_compound(expr, expected_type);
                case EXPR_UNARY:
                    return resolve_expr_unary(expr);
                case EXPR_BINARY:
                    return resolve_expr_binary(expr);
                case EXPR_TERNARY:
                    return resolve_expr_ternary(expr, expected_type);
                case EXPR_SIZEOF_EXPR:
                    {
                        Type* type = resolve_expr(expr->sizeof_expr).type;
                        complete_type(type);
                        return resolved_const(type_sizeof(type));
                    }
                case EXPR_SIZEOF_TYPE:
                    {
                        Type* type = resolve_typespec(expr->sizeof_type);
                        complete_type(type);
                        return resolved_const(type_sizeof(type));
                    }
                default:
                    assert(false);
                    return resolved_null;
            }
        }

        ResolvedExpr resolve_expr(Expr* expr)
        {
            return resolve_expected_expr(expr, null);
        }

        size_t resolve_const_expr(Expr* expr)
        {
            ResolvedExpr result = resolve_expr(expr);
            if (!result.is_const)
                fatal("Expected constant expression");

            return result.val;
        }

        internal void sym_global_decls(DeclSet* declset)
        {
            for (int i = 0; i < declset->num_decls; i++)
            {
                sym_global_decl(declset->decls[i]);
            }
        }

        void resolve_test()
        {
            type_int->align = type_float->align = type_int->size = type_float->size = 4;
            type_void->size = 0;
            type_char->size = type_char->align = 2;

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
            fixed (Type** t = &type_int)
            {
                Type* int_int_func = type_func(t, 1, type_int);
                assert(type_func(t, 1, type_int) == int_int_func);

                Type* int_func = type_func(null, 0, type_int);
                assert(int_int_func != int_func);
                assert(int_func == type_func(null, 0, type_int));
            }

            sym_global_type(_I("void"), type_void);
            sym_global_type(_I("char"), type_char);
            sym_global_type(_I("int"), type_int);
            sym_global_type(_I("float"), type_float);

            char*[] code = {
                "struct Vector { x, y: int; }".ToPtr(),
        "var i: int".ToPtr(),
        "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }".ToPtr(),
          "func f2(n: int): int { return 2*n; }".ToPtr(),
        "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }".ToPtr(),
        "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }".ToPtr(),
        "func f5(x: int): int { switch(x) { case 0: case 1: return 42; case 3: default: return -1; } }".ToPtr(),
        "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }".ToPtr(),
        "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }".ToPtr(),
        "func add(v: Vector, w: Vector): Vector { return {v.x + w.x, v.y + w.y}; }".ToPtr(),

            "union IntOrPtr { i: int; p: int*; }".ToPtr(),
        "var u1 = IntOrPtr{i = 42}".ToPtr(),
        "var u2 = IntOrPtr{p = cast(int*, 42)}".ToPtr(),
        "var a: int[256] = {1, 2, ['a'] = 42, [255] = 123}".ToPtr(),
        "var v: Vector = 0 ? {1,2} : {3,4}".ToPtr(),
        "var vs: Vector[2][2] = {{{1,2},{3,4}}, {{5,6},{7,8}}}".ToPtr(),
          "struct A { c: char; }".ToPtr(),
        "struct B { i: int; }".ToPtr(),
        "struct C { c: char; a: A; }".ToPtr(),
        "struct D { c: char; b: B; }".ToPtr(),
		// "func print(v: Vector) { printf(\"{%d, %d}\", v.x, v.y); }".ToPtr(),
		   "var x = add({1,2}, {3,4})".ToPtr(),
        "var v1: Vector = {1,2}".ToPtr(),
        "var w = Vector{3,4}".ToPtr(),
        "var p: void*".ToPtr(),
        "var r = cast(int, p) + 1".ToPtr(),
        "var fp: func(Vector)".ToPtr(),
		//  "struct Dup { x: int; x: int; }".ToPtr(),
        "var a2: int[3] = {1,2,3}".ToPtr(),
        "var b1: int[4]".ToPtr(),
        "var p1 = &a2[1]".ToPtr(),
        "var s1 = p1[1]".ToPtr(),
        "var j1 = *p1".ToPtr(),
        "const n2 = sizeof(a2)".ToPtr(),
      "const m1 = sizeof(&a2[0])".ToPtr(),
        "const l1 = sizeof(1 ? a2 : b1)".ToPtr(),
            "var pi = 3.14".ToPtr(),
        "var name = \"Per\"".ToPtr(),
            "var v3 = Vector{1,2}".ToPtr(),
        "var j2 = cast(int, p1)".ToPtr(),
        "var q6 = cast(int*, j2)".ToPtr(),
                  "const i = 42".ToPtr(),
                    // "const j = +i".ToPtr(),
                    //  "const k = -i".ToPtr(),
                    "const a = 1000/((2*3-5) << 1)".ToPtr(),
                    "const b = !0".ToPtr(),
                    "const c = ~100 + 1 == -100".ToPtr(),
                    "const k = 1 ? 2 : 3".ToPtr(),
                    "union IntOrPtr { i: int; p: int*; }".ToPtr(),
                    "var i = 42".ToPtr(),
                    "var u = IntOrPtr{i, &i}".ToPtr(),
                    "const n = 1+sizeof(p)".ToPtr(),
                    "var p: T*".ToPtr(),
                    "var u = *p".ToPtr(),
                    "struct T { a: int[n]; }".ToPtr(),
                    "var r = &t.a".ToPtr(),
                    "var t: T".ToPtr(),
                    "typedef S = int[n+m]".ToPtr(),
                    "const m = sizeof(t.a)".ToPtr(),
                    "var i = n+m".ToPtr(),
                    "var q = &i".ToPtr(),

                    "const n = sizeof(x)".ToPtr(),
                    "var x: T".ToPtr(),
                    "struct T { s: S*; }".ToPtr(),
                    "struct S { t: T[n]; }".ToPtr()


            };

            for (size_t i = 0; i < code.Length; i++)
            {
                init_stream(code[i]);
                Decl* decl = parse_decl();
                sym_global_decl(decl);
            }

            for (Sym** it = (Sym**)global_syms->_begin; it != global_syms->_top; it++)
            {
                Sym* sym = *it;
                complete_entity(sym);
            }

            Console.WriteLine();
            for (Sym** it = (Sym**)ordered_syms->_begin; it != ordered_syms->_top; it++)
            {
                Sym* sym = *it;
                if (sym->decl != null)
                    print_decl(sym->decl);
                else
                    printf("{0}", sym->name);

                printf("\n");
            }

            Console.WriteLine();
        }

    }
    unsafe struct ResolvedExpr
    {
        public Type* type;
        public bool is_lvalue;
        public bool is_const;
        public size_t val;
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

    enum SymKind
    {
        SYM_NONE,
        SYM_VAR,
        SYM_CONST,
        SYM_FUNC,
        SYM_TYPE,
        SYM_ENUM_CONST,
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
        public SymKind kind;
        public SymState state;
        public Decl* decl;
        public Type* type;
        public size_t val;
    }

    enum TypeKind
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
        [FieldOffset(Ion.PTR_SIZE + 4)] public size_t align;
        [FieldOffset(Ion.PTR_SIZE * 2 + 4)] public Sym* sym;
        [FieldOffset(Ion.PTR_SIZE * 3 + 4)] public _ptr ptr;
        [FieldOffset(Ion.PTR_SIZE * 3 + 4)] public _array array;
        [FieldOffset(Ion.PTR_SIZE * 3 + 4)] public _aggregate aggregate;
        [FieldOffset(Ion.PTR_SIZE * 3 + 4)] public _func func;


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
            public int num_params;
            public Type* ret;
        }

    }

}