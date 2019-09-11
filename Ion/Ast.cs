using System.Runtime.CompilerServices;

namespace Lang
{
    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;

    public unsafe partial class Ion
    {
        private readonly MemArena* ast_arena = MemArena.Create();

        private void* ast_alloc(int size)
        {
            assert(size != 0);
            var ptr = ast_arena->Alloc(size);
            Unsafe.InitBlock(ptr, 0, (uint) size);
            return ptr;
        }

        private void* ast_dup(void* src, int size)
        {
            if (size == 0) return null;

            var ptr = ast_arena->Alloc(size);
            Unsafe.CopyBlock(ptr, src, (uint) size);
            return ptr;
        }

        NoteList note_list(Note* notes, int num_notes) {
            return new NoteList {notes = (Note*)ast_dup(notes, num_notes * sizeof(Note)), num_notes = num_notes};
        }

        Note* get_decl_note(Decl* decl, char* name) {
            for (int i = 0; i < decl->notes.num_notes; i++) {
                Note *note = decl->notes.notes + i;
                if (note->name == name) {
                    return note;
                }
            }
            return null;
        }

        bool is_decl_foreign(Decl* decl) {
            return get_decl_note(decl, foreign_name) != null;
        }
        private StmtList stmt_list(SrcPos pos, Stmt** stmts, int num_stmts)
        {
            return new StmtList
                {pos = pos, stmts = (Stmt**) ast_dup(stmts, num_stmts * sizeof(Stmt*)), num_stmts = num_stmts};
        }

        private Typespec* typespec_new(SrcPos pos, TypespecKind kind)
        {
            var t = (Typespec*) ast_alloc(sizeof(Typespec));
            t->pos = pos;
            t->kind = kind;
            return t;
        }

        private Typespec* typespec_name(SrcPos pos, char* name)
        {
            var t = typespec_new(pos, TYPESPEC_NAME);
            t->name = name;
            return t;
        }

        private Typespec* typespec_ptr(SrcPos pos, Typespec* elem)
        {
            var t = typespec_new(pos, TYPESPEC_PTR);
            t->ptr.elem = elem;
            return t;
        }

        private Typespec* typespec_array(SrcPos pos, Typespec* elem, Expr* size)
        {
            var t = typespec_new(pos, TYPESPEC_ARRAY);
            t->array.elem = elem;
            t->array.size = size;
            return t;
        }

        private Typespec* typespec_func(SrcPos pos, Typespec** args, int num_args, Typespec* ret, bool variadic)
        {
            var t = typespec_new(pos, TYPESPEC_FUNC);
            t->func.args = (Typespec**) ast_dup(args, num_args * sizeof(Typespec*));
            t->func.num_args = num_args;
            t->func.ret = ret;
            t->func.variadic = variadic;
            return t;
        }

        private Decl* decl_new(DeclKind kind, SrcPos pos, char* name)
        {
            var d = (Decl*) ast_alloc(sizeof(Decl));
            d->pos = pos;
            d->kind = kind;
            d->name = name;
            return d;
        }

        private DeclSet* declset_new(Decl** decls, int num_items)
        {
            var d = (DeclSet*) ast_alloc(sizeof(DeclSet));
            d->decls = (Decl**) ast_dup(decls, num_items * sizeof(Decl*));
            d->num_decls = num_items;
            return d;
        }

        private Decl* decl_enum(SrcPos pos, char* name, EnumItem* items, int num_items)
        {
            var d = decl_new(DECL_ENUM, pos, name);
            d->enum_decl.items = (EnumItem*) ast_dup(items, num_items * sizeof(EnumItem));
            ;
            d->enum_decl.num_items = num_items;
            return d;
        }

        private Decl* decl_aggregate(SrcPos pos, DeclKind kind, char* name, AggregateItem* items, int num_items)
        {
            assert(kind == DECL_STRUCT || kind == DECL_UNION);
            var d = decl_new(kind, pos, name);
            d->aggregate.items = (AggregateItem*) ast_dup(items, num_items * sizeof(AggregateItem));
            d->aggregate.num_items = num_items;
            return d;
        }


        private Decl* decl_var(SrcPos pos, char* name, Typespec* type, Expr* expr)
        {
            var d = decl_new(DECL_VAR, pos, name);
            d->var.type = type;
            d->var.expr = expr;
            return d;
        }

        private Decl* decl_func(SrcPos pos, char* name, FuncParam* @params, int num_params, Typespec* ret_type, bool variadic, StmtList block)
        {
            var d = decl_new(DECL_FUNC, pos, name);
            d->func.@params = (FuncParam*) ast_dup(@params, num_params * sizeof(FuncParam));
            d->func.num_params = num_params;
            d->func.ret_type = ret_type;
            d->func.variadic = variadic;
            d->func.block = block;
            return d;
        }

        private Decl* decl_const(SrcPos pos, char* name, Expr* expr)
        {
            var d = decl_new(DECL_CONST, pos, name);
            d->const_decl.expr = expr;
            return d;
        }

        private Decl* decl_typedef(SrcPos pos, char* name, Typespec* type)
        {
            var d = decl_new(DECL_TYPEDEF, pos, name);
            d->typedef_decl.type = type;
            return d;
        }

        private Expr* expr_new(ExprKind kind, SrcPos pos)
        {
            var e = (Expr*) ast_alloc(sizeof(Expr));
            e->pos = pos;
            e->kind = kind;
            return e;
        }

        private Expr* expr_sizeof_expr(SrcPos pos, Expr* expr)
        {
            var e = expr_new(EXPR_SIZEOF_EXPR, pos);
            e->sizeof_expr = expr;
            return e;
        }

        private Expr* expr_sizeof_type(SrcPos pos, Typespec* type)
        {
            var e = expr_new(EXPR_SIZEOF_TYPE, pos);
            e->sizeof_type = type;
            return e;
        }

        private Expr* expr_int(SrcPos pos, int int_val, char* name)
        {
            var e = expr_new(EXPR_INT, pos);
            e->name = name;
            e->int_val = int_val;
            return e;
        }

        private Expr* expr_float(SrcPos pos, float float_val, char* name)
        {
            var e = expr_new(EXPR_FLOAT, pos);
            e->float_val = float_val;
            e->name = name;
            return e;
        }

        private Expr* expr_str(SrcPos pos, char* str_val)
        {
            var e = expr_new(EXPR_STR, pos);
            e->str_val = str_val;
            return e;
        }

        private Expr* expr_name(SrcPos pos, char* name)
        {
            var e = expr_new(EXPR_NAME, pos);
            e->name = name;
            return e;
        }

        private Expr* expr_compound(SrcPos pos, Typespec* type, CompoundField* fields, int num_fields)
        {
            var e = expr_new(EXPR_COMPOUND, pos);
            e->compound.type = type;
            e->compound.fields = (CompoundField*) ast_dup(fields, num_fields * sizeof(CompoundField));
            e->compound.num_fields = num_fields;
            return e;
        }

        private Expr* expr_cast(SrcPos pos, Typespec* type, Expr* expr)
        {
            var e = expr_new(EXPR_CAST, pos);
            e->cast.type = type;
            e->cast.expr = expr;
            return e;
        }

        private Expr* expr_call(SrcPos pos, Expr* expr, Expr** args, int num_args)
        {
            var e = expr_new(EXPR_CALL, pos);
            e->call.expr = expr;
            e->call.args = (Expr**) ast_dup(args, num_args * sizeof(Expr*));
            e->call.num_args = num_args;
            return e;
        }

        private Expr* expr_index(SrcPos pos, Expr* expr, Expr* index)
        {
            var e = expr_new(EXPR_INDEX, pos);
            e->index.expr = expr;
            e->index.index = index;
            return e;
        }

        private Expr* expr_field(SrcPos pos, Expr* expr, char* name)
        {
            var e = expr_new(EXPR_FIELD, pos);
            e->field.expr = expr;
            e->field.name = name;
            return e;
        }

        private Expr* expr_unary(SrcPos pos, TokenKind op, Expr* expr)
        {
            var e = expr_new(EXPR_UNARY, pos);
            e->unary.op = op;
            e->unary.expr = expr;
            return e;
        }

        private Expr* expr_binary(SrcPos pos, TokenKind op, Expr* left, Expr* right)
        {
            var e = expr_new(EXPR_BINARY, pos);
            e->binary.op = op;
            e->binary.left = left;
            e->binary.right = right;
            return e;
        }

        private Expr* expr_ternary(SrcPos pos, Expr* cond, Expr* then_expr, Expr* else_expr)
        {
            var e = expr_new(EXPR_TERNARY, pos);
            e->ternary.cond = cond;
            e->ternary.then_expr = then_expr;
            e->ternary.else_expr = else_expr;
            return e;
        }

        private Stmt* stmt_new(StmtKind kind, SrcPos pos)
        {
            var s = (Stmt*) ast_alloc(sizeof(Stmt));
            s->pos = pos;
            s->kind = kind;
            return s;
        }

        private Stmt* stmt_decl(SrcPos pos, Decl* decl)
        {
            var s = stmt_new(STMT_DECL, pos);
            s->decl = decl;
            return s;
        }

        private Stmt* stmt_return(SrcPos pos, Expr* expr)
        {
            var s = stmt_new(STMT_RETURN, pos);
            s->expr = expr;
            return s;
        }

        private Stmt* stmt_break(SrcPos pos)
        {
            return stmt_new(STMT_BREAK, pos);
        }

        private Stmt* stmt_continue(SrcPos pos)
        {
            return stmt_new(STMT_CONTINUE, pos);
        }

        private Stmt* stmt_block(SrcPos pos, StmtList block)
        {
            var s = stmt_new(STMT_BLOCK, pos);
            s->block = block;
            return s;
        }

        private Stmt* stmt_if(SrcPos pos, Expr* cond, StmtList then_block, ElseIf** elseifs, int num_elseifs,
            StmtList else_block)
        {
            var s = stmt_new(STMT_IF, pos);
            s->if_stmt.cond = cond;
            s->if_stmt.then_block = then_block;
            s->if_stmt.elseifs = (ElseIf**) ast_dup(elseifs, num_elseifs * sizeof(ElseIf*));
            s->if_stmt.num_elseifs = num_elseifs;
            s->if_stmt.else_block = else_block;
            return s;
        }

        private Stmt* stmt_while(SrcPos pos, Expr* cond, StmtList block)
        {
            var s = stmt_new(STMT_WHILE, pos);
            s->while_stmt.cond = cond;
            s->while_stmt.block = block;
            return s;
        }

        private Stmt* stmt_do_while(SrcPos pos, Expr* cond, StmtList block)
        {
            var s = stmt_new(STMT_DO_WHILE, pos);
            s->while_stmt.cond = cond;
            s->while_stmt.block = block;
            return s;
        }

        private Stmt* stmt_for(SrcPos pos, Stmt* init, Expr* cond, Stmt* next, StmtList block)
        {
            var s = stmt_new(STMT_FOR, pos);
            s->for_stmt.init = init;
            s->for_stmt.cond = cond;
            s->for_stmt.next = next;
            s->for_stmt.block = block;
            return s;
        }

        private Stmt* stmt_switch(SrcPos pos, Expr* expr, SwitchCase* cases, int num_cases)
        {
            var s = stmt_new(STMT_SWITCH, pos);
            s->switch_stmt.expr = expr;
            s->switch_stmt.cases = (SwitchCase*) ast_dup(cases, num_cases * sizeof(SwitchCase));
            s->switch_stmt.num_cases = num_cases;
            return s;
        }

        private Stmt* stmt_assign(SrcPos pos, TokenKind op, Expr* left, Expr* right)
        {
            var s = stmt_new(STMT_ASSIGN, pos);
            s->assign.op = op;
            s->assign.left = left;
            s->assign.right = right;
            return s;
        }

        private Stmt* stmt_init(SrcPos pos, char* name, Expr* expr)
        {
            var s = stmt_new(STMT_INIT, pos);
            s->init.name = name;
            s->init.expr = expr;
            return s;
        }

        private Stmt* stmt_expr(SrcPos pos, Expr* expr)
        {
            var s = stmt_new(STMT_EXPR, pos);
            s->expr = expr;
            return s;
        }
    }
}