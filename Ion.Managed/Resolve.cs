using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MLang
{
    using static Common;
    using static TypeKind;
    using static SymState;
    using static DeclKind;
    using static SymKind;
    using static TypespecKind;
    using static ExprKind;
    using static TokenKind;
    using static StmtKind;
    using static CompoundFieldKind;
    class Resolve
    {
        static Type type_alloc(TypeKind kind) {
            Type t = new Type();
            t.kind = kind;
            return t;
        }

        //private Type type_int_val = new Type {kind = TYPE_INT};
        //private Type type_float_val = new Type { kind = TYPE_FLOAT };

        private readonly Type type_int = type_alloc(TYPE_INT);
        private readonly Type type_float = type_alloc(TYPE_FLOAT);
        private readonly Type type_void = type_alloc(TYPE_VOID);
        private readonly Type type_char = type_alloc(TYPE_CHAR);
        internal const int PTR_SIZE = 4;
        internal const int PTR_ALIGN = 8;
        readonly IList<CachedPtrType> cached_ptr_types = new List<CachedPtrType>();
        long type_sizeof(Type type)
        {
            return type.size;
        }

        long type_alignof(Type type)
        {
            return type.align;
        }
        Type type_ptr(Type elem) {
            foreach (var it in cached_ptr_types)
            {
                if (it.elem == elem) {
                    return it.ptr;
                }
            }

            Type t = type_alloc(TypeKind.TYPE_PTR);
            t.ptr = new Type._ptr();
            t.size = PTR_SIZE;
            t.align = PTR_ALIGN;
            t.base = elem;

            cached_ptr_types.Add(new CachedPtrType { elem = elem, ptr = t});
            return t;
        }

        readonly IList<CachedArrayType> cached_array_types = new List<CachedArrayType>();

        Type type_array(Type elem, long size) {
            foreach (var it in cached_array_types)
            {
                if (it.elem == elem && it.size == size) {
                    return it.array;
                }
            }

            complete_type(elem);
            Type t = type_alloc(TypeKind.TYPE_ARRAY);
            t.array = new Type._array();
            t.size = size * type_sizeof(elem);
            t.align = type_alignof(elem);
            t.array.elem = elem;
            t.array.size = size;
            cached_array_types.Add(new CachedArrayType{elem = elem, array = t, size = size});
            return t;
        }

        readonly IList<CachedFuncType> cached_func_types = new List<CachedFuncType>();

        Type type_func(Type[] @params, long num_params, Type ret) {
            foreach (var it in cached_func_types)
            {
                if (it.num_params == num_params && it.ret == ret) {
                    bool match = true;
                    for (long i = 0; i < num_params; i++) {
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
            t.func.@params = Ast.ast_dup(@params);
            t.size = PTR_SIZE;
            t.align = PTR_ALIGN;
            t.func.num_params = num_params;
            t.func.ret = ret;
            cached_func_types.Add(new CachedFuncType{func = t, num_params =num_params, ret = ret, @params = @params});
            return t;
        }
        bool duplicate_fields(TypeField[] fields, long num_fields)
        {
            for (long i = 0; i < num_fields; i++)
            for (long j = i + 1; j < num_fields; j++)
                if (fields[i].name == fields[j].name)
                    return true;
            return false;
        }

        Type type_complete_struct(Type type, TypeField[] fields, long num_fields) {
            Debug.Assert(type.kind == TYPE_COMPLETING);
            type.kind = TYPE_STRUCT;
            type.size = 0;
            type.align = 0;
            foreach (var field in fields) {
                type.size = type_sizeof(field.type) + ALIGN_UP(type.size, type_alignof(field.type));
                type.align = MAX(type.align, type_alignof(field.type));
            }
            type.aggregate = new Type._aggregate();
            type.aggregate.fields = Ast.ast_dup(fields);
            type.aggregate.num_fields = num_fields;
            return type;
        }

        Type type_complete_union(Type type, TypeField[] fields, long num_fields){
            Debug.Assert(type.kind == TYPE_COMPLETING);
            type.kind = TYPE_UNION;
            type.size = 0;
            foreach (var field in fields)
            {
                type.size = MAX(type.size, field.type.size);
            }
            type.aggregate = new Type._aggregate();
            type.aggregate.fields = Ast.ast_dup(fields);
            type.aggregate.num_fields = num_fields;
            return type;
        }
        Type type_incomplete(Sym sym)
        {
            Type type = type_alloc(TYPE_INCOMPLETE);
            type.sym = sym;
            return type;
        } 
        public const int MAX_LOCAL_SYMS = 1024;
        readonly IList<Sym> global_syms = new List<Sym>(MAX_LOCAL_SYMS);
        readonly IList<Sym> local_syms = new List<Sym>(MAX_LOCAL_SYMS);
        Sym sym_new(SymKind kind, string name, Decl decl)
        {
            Sym sym = new Sym();
            sym.kind = kind;
            sym.name = name;
            sym.decl = decl;
            return sym;
        }

        Sym sym_decl(Decl decl) {
            SymKind kind = SYM_NONE;
            switch (decl.kind) {
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
                    Debug.Assert(false);
                    break;
            }
            Sym sym = sym_new(kind, decl.name, decl);
            if (decl.kind == DECL_STRUCT || decl.kind == DECL_UNION)
            {
                sym.state = SYM_RESOLVED;
                sym.type = type_incomplete(sym);
            }
            return sym;
        }

        Sym sym_enum_const(string name, Decl decl)
        {
            return sym_new(SYM_ENUM_CONST, name, decl);
        }
        Sym sym_get(string name) {
            foreach (Sym sym in local_syms)
            {
                if (sym.name == name)
                {
                    return sym;
                }
            }
            foreach (Sym sym in global_syms)
            {
                if (sym.name == name)
                {
                    return sym;
                }
            }
            return null;
        }

        void sym_push_var(string name, Type type)
        {
            if (local_syms.Count > MAX_LOCAL_SYMS)
            {
                throw new Exception("Too many local symbols");
            }

            local_syms.Add(new Sym
            {
                name = name,
                kind = SYM_VAR,
                state = SYM_RESOLVED,
                type = type,
            });

        }
        Sym sym_enter()
        {
            return local_syms.Last();
        }

        void sym_leave(Sym sym)
        {
            local_syms.Add(sym);
        }

        Sym sym_global_decl(Decl decl)
        {
            Sym sym = sym_decl(decl);
            global_syms.Add(sym);
            if (decl.kind == DECL_ENUM)
            {
                for (long i = 0; i < decl.enum_decl.num_items; i++)
                {
                    global_syms.Add(sym_enum_const(decl.enum_decl.items[i].name, decl));
                }
            }
            return sym;
        }
        Sym sym_global_type(string name, Type type)
        {
            Sym sym = sym_new(SYM_TYPE, name, null);
            sym.state = SYM_RESOLVED;
            sym.type = type;
            global_syms.Add(sym);
            return sym;
        }
        
        void sym_put(Decl decl) {
            Error.assert(decl.name != null);
            Error.assert(sym_get(decl.name) == null);

            local_syms.Add(new Sym{decl = decl, name = decl.name, state = SymState.SYM_UNRESOLVED});
        }

        void resolve_decl(Decl decl) {
            switch (decl.kind) {
                case DeclKind.DECL_CONST:
                    break;
            }
        }


        private readonly ResolvedExpr resolved_null;

        ResolvedExpr resolved_rvalue(Type type)
        {
            return new ResolvedExpr { type = type };
        }

        ResolvedExpr resolved_lvalue(Type type)
        {
            return new ResolvedExpr { type = type, is_lvalue = true };
        }

        ResolvedExpr resolved_const(long val)
        {
            return new ResolvedExpr { type = type_int, is_const = true, val = val, };
        }
        Type resolve_typespec(Typespec typespec)
        {
            if (typespec == null)
                return type_void;

            switch (typespec.kind)
            {
                case TYPESPEC_NAME:
                {
                    Sym sym = resolve_name(typespec.name);
                    if (sym.kind != SYM_TYPE)
                    {
                        throw new Exception($"{typespec.name} must denote a type");
                        return null;
                    }
                    return sym.type;
                }
                case TYPESPEC_PTR:
                    return type_ptr(resolve_typespec(typespec.base));
                case TYPESPEC_ARRAY:
                    var size = resolve_const_expr(typespec.array.size);
                    if (size < 0)
                        new Exception("Negative array size");

                    return type_array(resolve_typespec(typespec.array.elem), size);
                case TYPESPEC_FUNC:
                {
                    //Type** args = null;
                    var args = new List<Type>();
                    for (long i = 0; i < typespec.func.num_args; i++)
                    {
                        args.Add(resolve_typespec(typespec.func.args[i]));
                    }
                    Type ret = type_void;
                    if (typespec.func.ret != null)
                    {
                        ret = resolve_typespec(typespec.func.ret);
                    }
                    return type_func(args.ToArray(), args.Count, ret);
                }
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        private List<Sym> ordered_syms = new List<Sym>();

        void complete_type(Type type)
        {
            if (type.kind == TYPE_COMPLETING)
            {
                throw new Exception("Type completion cycle");
                return;
            }
            else if (type.kind != TYPE_INCOMPLETE)
            {
                return;
            }
            type.kind = TYPE_COMPLETING;
            Decl decl = type.sym.decl;
            Debug.Assert(decl.kind == DECL_STRUCT || decl.kind == DECL_UNION);
            var fields = new List<TypeField>();
            for (long i = 0; i < decl.aggregate.num_items; i++)
            {
                AggregateItem item = decl.aggregate.items[i];
                Type item_type = resolve_typespec(item.type);
                complete_type(item_type);
                for (long j = 0; j < item.num_names; j++)
                {
                    fields.Add(new TypeField { name = item.names[j], type = item_type });
                }
            }
            if (fields.Count == 0)
                throw new Exception("No fields");
            if (duplicate_fields(fields.ToArray(), fields.Count))
                throw new Exception("Duplicate fields");
            if (decl.kind == DECL_STRUCT)
            {
                type_complete_struct(type, fields.ToArray(), fields.Count);
            }
            else
            {
                Debug.Assert(decl.kind == DECL_UNION);
                type_complete_union(type, fields.ToArray(), fields.Count);
            }
            ordered_syms.Add(type.sym);
        }

        Type resolve_decl_type(Decl decl)
        {
            Debug.Assert(decl.kind == DECL_TYPEDEF);
            return resolve_typespec(decl.typedef_decl.type);
        }

        Type resolve_decl_var(Decl decl)
        {
            Debug.Assert(decl.kind == DECL_VAR);
            Type type = null;
            if (decl.var.type != null)
            {
                type = resolve_typespec(decl.var.type);
            }
            if (decl.var.expr != null)
            {
                ResolvedExpr result = resolve_expected_expr(decl.var.expr, type);
                if (type != null && result.type != type)
                {
                    throw new Exception("Declared var type does not match inferred type");
                }
                type = result.type;
            }
            complete_type(type);
            return type;
        }

        Type resolve_decl_const(Decl decl, long val)
        {
            Debug.Assert(decl.kind == DECL_CONST);
            ResolvedExpr result = resolve_expr(decl.const_decl.expr);
            if (!result.is_const)
                throw new Exception("Initializer for const is not a constant expression");
            val = result.val;
            return result.type;
        }

        Type resolve_decl_func(Decl decl)
        {
            Debug.Assert(decl.kind == DECL_FUNC);
            var @params = new List<Type>();
            for (long i = 0; i < decl.func.num_params; i++)
                @params.Add(resolve_typespec(decl.func.@params[i].type));
            Type ret_type = type_void;
            if (decl.func.ret_type != null)
                ret_type = resolve_typespec(decl.func.ret_type);

            return type_func(@params.ToArray(), @params.Count, ret_type);
        }


        void resolve_cond_expr(Expr expr)
        {
            ResolvedExpr cond = resolve_expr(expr);
            if (cond.type != type_int)
            {
                throw new Exception("Conditional expression must have type long");
            }
        }

        void resolve_stmt_block(StmtBlock block, Type ret_type)
        {
            Sym start = sym_enter();
            for (long i = 0; i < block.num_stmts; i++)
            {
                resolve_stmt(block.stmts[i], ret_type);
            }
            sym_leave(start);
        }

        void resolve_stmt(Stmt stmt, Type ret_type)
        {
            switch (stmt.kind)
            {
                case STMT_RETURN:
                    if (stmt.expr != null)
                    {
                        ResolvedExpr result = resolve_expected_expr(stmt.expr, ret_type);
                        if (result.type != ret_type)
                        {
                            throw new Exception("Return type mismatch");
                        }
                    }
                    else
                    {
                        if (ret_type != type_void)
                        {
                            throw new Exception("Empty return expression for function with non-void return type");
                        }
                    }
                    break;
                case STMT_BREAK:
                case STMT_CONTINUE:
                    // Do nothing
                    break;
                case STMT_BLOCK:
                    resolve_stmt_block(stmt.block, ret_type);
                    break;
                case STMT_IF:
                    resolve_cond_expr(stmt.if_stmt.cond);
                    resolve_stmt_block(stmt.if_stmt.then_block, ret_type);
                    for (long i = 0; i < stmt.if_stmt.num_elseifs; i++)
                    {
                        ElseIf elseif = stmt.if_stmt.elseifs[i];
                        resolve_cond_expr(elseif.cond);
                        resolve_stmt_block(elseif.block, ret_type);
                    }
                    if (stmt.if_stmt.else_block?.stmts != null)
                    {
                        resolve_stmt_block(stmt.if_stmt.else_block, ret_type);
                    }
                    break;
                case STMT_WHILE:
                case STMT_DO_WHILE:
                    resolve_cond_expr(stmt.while_stmt.cond);
                    resolve_stmt_block(stmt.while_stmt.block, ret_type);
                    break;
                case STMT_FOR:
                    {
                        Sym sym = sym_enter();
                        resolve_stmt(stmt.for_stmt.init, ret_type);
                        resolve_cond_expr(stmt.for_stmt.cond);
                        resolve_stmt_block(stmt.for_stmt.block, ret_type);
                        resolve_stmt(stmt.for_stmt.next, ret_type);
                        sym_leave(sym);
                        break;
                    }
                case STMT_SWITCH:
                    {
                        ResolvedExpr result = resolve_expr(stmt.switch_stmt.expr);
                        for (long i = 0; i < stmt.switch_stmt.num_cases; i++)
                        {
                            SwitchCase switch_case = stmt.switch_stmt.cases[i];
                            for (long j = 0; j < switch_case.num_exprs; j++)
                            {
                                ResolvedExpr case_result = resolve_expr(switch_case.exprs[j]);
                                if (case_result.type != result.type)
                                {
                                    throw new Exception("Switch case expression type mismatch");
                                }
                                resolve_stmt_block(switch_case.block, ret_type);
                            }
                        }
                        break;
                    }
                case STMT_ASSIGN:
                    {
                        ResolvedExpr left = resolve_expr(stmt.assign.left);
                        if (stmt.assign.right != null)
                        {
                            ResolvedExpr right = resolve_expected_expr(stmt.assign.right, left.type);
                            if (left.type != right.type)
                            {
                                throw new Exception("Left/right types do not match in assignment statement");
                            }
                        }
                        if (!left.is_lvalue)
                        {
                            throw new Exception("Cannot assign to non-lvalue");
                        }
                        if (stmt.assign.op != TOKEN_ASSIGN && left.type != type_int)
                        {
                            throw new Exception("Can only use assignment operators with type long");
                        }
                        break;
                    }
                case STMT_INIT:
                    sym_push_var(stmt.init.name, resolve_expr(stmt.init.expr).type);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        void resolve_func(Sym sym)
        {
            Decl decl = sym.decl;
            Debug.Assert(decl.kind == DECL_FUNC);
            Debug.Assert(sym.state == SYM_RESOLVED);
            Sym start = sym_enter();
            for (long i = 0; i < decl.func.num_params; i++)
            {
                FuncParam param = decl.func.@params[i];
                sym_push_var(param.name, resolve_typespec(param.type));
            }

            resolve_stmt_block(decl.func.block, resolve_typespec(decl.func.ret_type));
            sym_leave(start);
        }

        void resolve_sym(Sym sym)
        {
            if (sym.state == SYM_RESOLVED)
            {
                return;
            }
            else if (sym.state == SYM_RESOLVING)
            {
                throw new Exception("Cyclic dependency");
            }
            Debug.Assert(sym.state == SYM_UNRESOLVED);
            sym.state = SYM_RESOLVING;
            switch (sym.kind)
            {
                case SYM_TYPE:
                    sym.type = resolve_decl_type(sym.decl);
                    break;
                case SYM_VAR:
                    sym.type = resolve_decl_var(sym.decl);
                    break;
                case SYM_CONST:
                    sym.type = resolve_decl_const(sym.decl, sym.val);
                    break;
                case SYM_FUNC:
                    sym.type = resolve_decl_func(sym.decl);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            sym.state = SYM_RESOLVED;
            ordered_syms.Add(sym);
        }

        void complete_entity(Sym sym)
        {
            resolve_sym(sym);
            if (sym.kind == SYM_TYPE)
                complete_type(sym.type);
            else if (sym.kind == SYM_FUNC)
            {
                resolve_func(sym);
            }
        }

        Sym resolve_name(string name)
        {
            Sym sym = sym_get(name);
            if (sym == null)
            {
                throw new Exception("Non-existent name");
            }
            resolve_sym(sym);
            return sym;
        }


        ResolvedExpr resolve_expr_field(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_FIELD);
            ResolvedExpr left = resolve_expr(expr.field.expr);
            Type type = left.type;
            complete_type(type);
            if (type.kind != TYPE_STRUCT && type.kind != TYPE_UNION)
            {
                throw new Exception("Can only access fields on aggregate types");
                return resolved_null;
            }
            for (long i = 0; i < type.aggregate.num_fields; i++)
            {
                TypeField field = type.aggregate.fields[i];
                if (field.name == expr.field.name)
                {
                    return left.is_lvalue ? resolved_lvalue(field.type) : resolved_rvalue(field.type);
                }
            }
            throw new Exception($"No field named '{expr.field.name}'");
            return resolved_null;
        }

        ResolvedExpr ptr_decay(ResolvedExpr expr)
        {
            if (expr.type.kind == TYPE_ARRAY)
            {
                return resolved_rvalue(type_ptr(expr.type.array.elem));
            }
            else
            {
                return expr;
            }
        }


        ResolvedExpr resolve_expr_name(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_NAME);
            Sym sym = resolve_name(expr.name);
            if (sym.kind == SYM_VAR)
                return resolved_lvalue(sym.type);
            else if (sym.kind == SYM_CONST)
                return resolved_const(sym.val);
            else if (sym.kind == SYM_FUNC)
            {
                return resolved_rvalue(sym.type);
            }
            else
            {
                throw new Exception($"{expr.name} must denote a var func or const");
                return resolved_null;
            }
        }

        long eval_int_unary(TokenKind op, long val)
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
                    Debug.Assert(false);
                    return 0;
            }
        }

        long eval_int_binary(TokenKind op, long left, long right)
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
                    Debug.Assert(false);
                    return 0;
            }
        }

        ResolvedExpr resolve_expr_unary(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_UNARY);
            ResolvedExpr operand = resolve_expr(expr.unary.expr);
            Type type = operand.type;
            switch (expr.unary.op)
            {
                case TOKEN_MUL:
                    operand = ptr_decay(operand);
                    if (type.kind != TYPE_PTR)
                        throw new Exception("Cannot deref non-ptr type");

                    return resolved_lvalue(type.base);
                case TOKEN_AND:
                    if (!operand.is_lvalue)
                        throw new Exception("Cannot take address of non-lvalue");

                    return resolved_rvalue(type_ptr(type));
                default:
                    if (type.kind != TYPE_INT)
                        throw new Exception($"Can only use unary {Lexer.token_kind_name(expr.unary.op)} with ints");

                    if (operand.is_const)
                    {
                        return resolved_const(eval_int_unary(expr.unary.op, operand.val));
                    }
                    else
                        return resolved_rvalue(type);
            }
        }

        ResolvedExpr resolve_expr_binary(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_BINARY);
            ResolvedExpr left = resolve_expr(expr.binary.left);
            ResolvedExpr right = resolve_expr(expr.binary.right);
            if (left.type != type_int)
            {
                throw new Exception("left operand of + must be long");
            }
            if (right.type != left.type)
            {
                throw new Exception("left and right operand of + must have same type");
            }

            if (left.is_const && right.is_const)
                return resolved_const(eval_int_binary(expr.binary.op, left.val, right.val));
            else
                return resolved_rvalue(left.type);
        }
        long aggregate_field_index(Type type, string name)
        {
            Debug.Assert(type.kind == TYPE_STRUCT || type.kind == TYPE_UNION);
            for (long i = 0; i < type.aggregate.num_fields; i++)
            {
                if (type.aggregate.fields[i].name == name)
                {
                    return i;
                }
            }
            throw new Exception($"Field '{name}' in compound literal not found in struct/union");
            return long.MaxValue;
        }

        ResolvedExpr resolve_expr_compound(Expr expr, Type expected_type)
        {
            Debug.Assert(expr.kind == EXPR_COMPOUND);
            if (expected_type == null && expr.compound.type == null)
            {
                throw new Exception("Implicitly typed compound literals used in context without expected type");
            }
            Type type = null;
            if (expr.compound.type != null)
            {
                type = resolve_typespec(expr.compound.type);
            }
            else
            {
                type = expected_type;
            }
            complete_type(type);
            if (type.kind != TYPE_STRUCT && type.kind != TYPE_UNION && type.kind != TYPE_ARRAY)
            {
                throw new Exception("Compound literals can only be used with struct and array types");
            }

            if (type.kind == TYPE_STRUCT || type.kind == TYPE_UNION)
            {
                long index = 0;
                for (long i = 0; i < expr.compound.num_fields; i++)
                {
                    CompoundField field = expr.compound.fields[i];
                    if (field.kind == FIELD_INDEX)
                    {
                        throw new Exception("Index field initializer not allowed for struct/union compound literal");
                    }
                    else if (field.kind == FIELD_NAME)
                    {
                        index = aggregate_field_index(type, field.name);
                    }
                    if (index >= type.aggregate.num_fields)
                    {
                        throw new Exception("Field initializer in struct/union compound literal out of range");
                    }
                    ResolvedExpr init = resolve_expected_expr(expr.compound.fields[i].init, type.aggregate.fields[index].type);
                    if (init.type != type.aggregate.fields[index].type)
                    {
                        throw new Exception("Compound literal field type mismatch");
                    }
                    index++;
                }
            }
            else
            {
                Debug.Assert(type.kind == TYPE_ARRAY);
                long index = 0;
                for (long i = 0; i < expr.compound.num_fields; i++)
                {
                    CompoundField field = expr.compound.fields[i];
                    if (field.kind == FIELD_NAME)
                    {
                        throw new Exception("Named field initializer not allowed for array compound literals");
                    }
                    else if (field.kind == FIELD_INDEX)
                    {
                        long result = resolve_const_expr(field.index);
                        if (result < 0)
                        {
                            throw new Exception("Field initializer index cannot be negative");
                        }
                        index = result;
                    }
                    if (index >= type.array.size)
                    {
                        throw new Exception("Field initializer in array compound literal out of range");
                    }
                    ResolvedExpr init = resolve_expected_expr(expr.compound.fields[i].init, type.array.elem);
                    if (init.type != type.array.elem)
                    {
                        throw new Exception("Compound literal element type mismatch");
                    }
                    index++;
                }
            }
            return resolved_rvalue(type);
        }

        ResolvedExpr resolve_expr_call(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_CALL);
            ResolvedExpr func = resolve_expr(expr.call.expr);
            if (func.type.kind != TYPE_FUNC)
            {
                throw new Exception("Trying to call non-function value");
            }
            if (expr.call.num_args != func.type.func.num_params)
            {
                throw new Exception("Tried to call function with wrong number of arguments");
            }
            for (long i = 0; i < expr.call.num_args; i++)
            {
                Type param_type = func.type.func.@params[i];
                ResolvedExpr arg = resolve_expected_expr(expr.call.args[i], param_type);
                if (arg.type != param_type)
                {
                    throw new Exception("Call argument expression type doesn't match expected param type");
                }
            }
            return resolved_rvalue(func.type.func.ret);
        }

        ResolvedExpr resolve_expr_ternary(Expr expr, Type expected_type)
        {
            Debug.Assert(expr.kind == EXPR_TERNARY);
            ResolvedExpr cond = ptr_decay(resolve_expr(expr.ternary.cond));
            if (cond.type.kind != TYPE_INT && cond.type.kind != TYPE_PTR)
            {
                throw new Exception("Ternary cond expression must have type long or ptr");
            }
            ResolvedExpr then_expr = ptr_decay(resolve_expected_expr(expr.ternary.then_expr, expected_type));
            ResolvedExpr else_expr = ptr_decay(resolve_expected_expr(expr.ternary.else_expr, expected_type));
            if (then_expr.type != else_expr.type)
            {
                throw new Exception("Ternary then/else expressions must have matching types");
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

        ResolvedExpr resolve_expr_index(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_INDEX);
            ResolvedExpr operand = ptr_decay(resolve_expr(expr.index.expr));
            if (operand.type.kind != TYPE_PTR)
            {
                throw new Exception("Can only index arrays or pointers");
            }
            ResolvedExpr index = resolve_expr(expr.index.index);
            if (index.type.kind != TYPE_INT)
            {
                throw new Exception("Index expression must have type long");
            }
            return resolved_lvalue(operand.type.base);
        }

        ResolvedExpr resolve_expr_cast(Expr expr)
        {
            Debug.Assert(expr.kind == EXPR_CAST);
            Type type = resolve_typespec(expr.cast.type);
            ResolvedExpr result = ptr_decay(resolve_expr(expr.cast.expr));
            if (type.kind == TYPE_PTR)
            {
                if (result.type.kind != TYPE_PTR && result.type.kind != TYPE_INT)
                {
                    throw new Exception("Invalid cast to pointer type");
                }
            }
            else if (type.kind == TYPE_INT)
            {
                if (result.type.kind != TYPE_PTR && result.type.kind != TYPE_INT)
                {
                    throw new Exception("Invalid cast to long type");
                }
            }
            else
            {
                throw new Exception("Invalid target cast type");
            }
            return resolved_rvalue(type);
        }

        ResolvedExpr resolve_expected_expr(Expr expr, Type expected_type)
        {
            switch (expr.kind)
            {
                case EXPR_INT:
                    return resolved_const((long)expr.int_val);
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
                        Type type = resolve_expr(expr.sizeof_expr).type;
                        complete_type(type);
                        return resolved_const(type_sizeof(type));
                    }
                case EXPR_SIZEOF_TYPE:
                    {
                        Type type = resolve_typespec(expr.sizeof_type);
                        complete_type(type);
                        return resolved_const(type_sizeof(type));
                    }
                default:
                    Debug.Assert(false);
                    return resolved_null;
            }
        }

        ResolvedExpr resolve_expr(Expr expr)
        {
            return resolve_expected_expr(expr, null);
        }

        long resolve_const_expr(Expr expr)
        {
            ResolvedExpr result = resolve_expr(expr);
            if (!result.is_const)
                throw new Exception("Expected constant expression");

            return result.val;
        }

        internal static void resolve_test() {
            var r = new Resolve();
            r.type_int.align = r.type_float.align = r.type_int.size = r.type_float.size = 4;
            r.type_void.size = 0;
            r.type_char.size = r.type_char.align = 2;
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


            r.sym_global_type("void", r.type_void);
            r.sym_global_type("char", r.type_char);
            r.sym_global_type("int", r.type_int);
            r.sym_global_type("float", r.type_float);

            string[] code = {
                "struct Vector { x, y: int; }",
                "var i: int",
                "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }",
                "func f2(n: int): int { return 2*n; }",
                "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }",
                "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }",
                "func f5(x: int): int { switch(x) { case 0: case 1: return 42; case 3: default: return -1; } }",
                "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }",
                "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }",
                "func add(v: Vector, w: Vector): Vector { return {v.x + w.x, v.y + w.y}; }",

                "union IntOrPtr { i: int; p: int*; }",
                "var u1 = IntOrPtr{i = 42}",
                "var u2 = IntOrPtr{p = cast(int*, 42)}",
                "var a: int[256] = {1, 2, ['a'] = 42, [255] = 123}",
                "var v: Vector = 0 ? {1,2} : {3,4}",
                "var vs: Vector[2][2] = {{{1,2},{3,4}}, {{5,6},{7,8}}}",
                /*       "struct A { c: char; }",
                       "struct B { i: int; }",
                       "struct C { c: char; a: A; }",
                       "struct D { c: char; b: B; }",
                       "struct Vector { x, y: int; }",
                       "func print(v: Vector) { printf(\"{%d, %d}\", v.x, v.y); }",
                       "func add(v: Vector, w: Vector): Vector { return {v.x + w.x, v.y + w.y}; }",
                       "var x = add({1,2}, {3,4})",
                       "var v: Vector = {1,2}",
                       "var w = Vector{3,4}",
                       "var p: void*",
                       "var i = cast(int, p) + 1",
                       "var fp: func(Vector)",
                       //  "struct Dup { x: int; x: int; }",
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
                       "var j = cast(int, p)",
                       //  "var q = cast(int*, j)",
                       "const i = 42",
                       // "const j = +i",
                       //  "const k = -i",
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
                       "struct S { t: T[n]; }"*/

            };
            var lex = new Lexer();
            var parser = new Parser(lex);
            var p = new Print();
            foreach (var s in code) {

                lex.init_stream(s);
                Decl decl2 = parser.parse_decl();
                r.sym_global_decl(decl2);
            }

            foreach (var sym2 in r.global_syms) {
                r.complete_entity(sym2);
            }
            Console.WriteLine();
            foreach (var sym2 in r.ordered_syms)
            {
                if (sym2.decl != null)
                    p.print_decl(sym2.decl);
                else
                    p.printf("{0}", sym2.name);

                p.printf("\n");
            }
        }

    }
    class ResolvedExpr
    {
        public Type type;
        public bool is_lvalue;
        public bool is_const;
        public long val;
    }
    class CachedPtrType
    {
        public Type elem;
        public Type ptr;
    }

    class CachedArrayType
    {
        public Type elem;
        public long size;
        public Type array;
    }

    class CachedFuncType
    {
        public Type[] @params;
        public long num_params;
        public Type ret;
        public Type func;
    }


    class ConstEntity
    {
        public Type type;
        public ulong int_val;
        public double float_val;
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


    class Sym
    {
        public string name;
        public SymKind kind;
        public SymState state;
        public Decl decl;
        public Type type;
        public long val;
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



    class TypeField : ICloneable
    {
        public string name;
        public Type type;
        public object Clone() => MemberwiseClone();
    }


    class Type : ICloneable
    {
        public TypeKind kind;
        public long size;
        public long align;
        public Sym sym;
        public _ptr ptr;
        public _array array;
        public _aggregate aggregate;
        public _func func;

        internal class _ptr
        {
            public Type elem;
        }

        internal class _array
        {
            public Type elem;
            public long size;
        }

        internal class _aggregate
        {
            public TypeField[] fields;
            public long num_fields;
        }

        internal class _func
        {
            public Type[] @params;
            public long num_params;
            public Type ret;
        }

        public object Clone() => MemberwiseClone();
    }
}