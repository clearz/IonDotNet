using System.Runtime.CompilerServices;

namespace IonLang
{
    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;

    public unsafe partial class Ion {
        MemArena ast_arena;

        void* ast_alloc(int size) {
            assert(size != 0);
            var ptr = ast_arena.Alloc(size);
            Unsafe.InitBlock(ptr, 0, (uint)size);
            return ptr;
        }

        void* ast_dup(void* src, int size) {
            if (size == 0) return null;

            var ptr = ast_arena.Alloc(size);
            Unsafe.CopyBlock(ptr, src, (uint)size);
            return ptr;
        }
        Note new_note(SrcPos pos, char* name, NoteArg* args, int num_args) {
            return new Note { pos = pos, name = name, args = (NoteArg*)ast_dup(args, num_args * sizeof(NoteArg)), num_args = num_args };
        }

        Notes new_notes(Note* notes, int num_notes) {
            return new Notes {notes = (Note*)ast_dup(notes, num_notes * sizeof(Note)), num_notes = num_notes};
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
        StmtList stmt_list(SrcPos pos, Stmt** stmts, int num_stmts)
        {
            return new StmtList
                {pos = pos, stmts = (Stmt**) ast_dup(stmts, num_stmts * sizeof(Stmt*)), num_stmts = num_stmts};
        }

        Typespec* typespec_new(TypespecKind kind, SrcPos pos) {
            var t = (Typespec*) ast_alloc(sizeof(Typespec));
            t->pos = pos;
            t->kind = kind;
            return t;
        }

        Typespec* new_typespec_name(SrcPos pos, char* name)
        {
            var t = typespec_new(TYPESPEC_NAME, pos);
            t->name = name;
            return t;
        }

        Typespec* new_typespec_ptr(SrcPos pos, Typespec* @base)
        {
            var t = typespec_new(TYPESPEC_PTR, pos);
            t->@base = @base;
            t->@base = @base;
            return t;
        }
        Typespec* new_typespec_const(SrcPos pos, Typespec* @base) {
            Typespec *t = typespec_new(TYPESPEC_CONST, pos);
            t->@base = @base;
            return t;
        }

        Typespec* new_typespec_array(SrcPos pos, Typespec* elem, Expr* size)
        {
            var t = typespec_new(TYPESPEC_ARRAY, pos);
            t->@base = elem;
            t->num_elems = size;
            return t;
        }

        Typespec* new_typespec_func(SrcPos pos, Typespec** args, int num_args, Typespec* ret, bool has_varargs)
        {
            var t = typespec_new(TYPESPEC_FUNC, pos);
            t->func.args = (Typespec**) ast_dup(args, num_args * sizeof(Typespec*));
            t->func.num_args = num_args;
            t->func.ret = ret;
            t->func.has_varargs = has_varargs;
            return t;
        }

        Decl* new_decl(DeclKind kind, SrcPos pos, char* name)
        {
            var d = (Decl*) ast_alloc(sizeof(Decl));
            d->pos = pos;
            d->kind = kind;
            d->name = name;
            return d;
        }

        Decls* new_decls(Decl** decls, int num_items)
        {
            var d = (Decls*) ast_alloc(sizeof(Decls));
            d->decls = (Decl**) ast_dup(decls, num_items * sizeof(Decl*));
            d->num_decls = num_items;
            return d;
        }

        Decl* new_decl_enum(SrcPos pos, char* name, Typespec* type, EnumItem* items, int num_items)
        {
            var d = new_decl(DECL_ENUM, pos, name);
            d->enum_decl.items = (EnumItem*) ast_dup(items, num_items * sizeof(EnumItem));
            d->enum_decl.type = type;
            d->enum_decl.num_items = num_items;
            return d;
        }

        Decl* new_decl_aggregate(SrcPos pos, DeclKind kind, char* name, AggregateItem* items, int num_items)
        {
            assert(kind == DECL_STRUCT || kind == DECL_UNION);
            var d = new_decl(kind, pos, name);
            d->aggregate.items = (AggregateItem*) ast_dup(items, num_items * sizeof(AggregateItem));
            d->aggregate.num_items = num_items;
            return d;
        }


        Decl* new_decl_var(SrcPos pos, char* name, Typespec* type, Expr* expr)
        {
            var d = new_decl(DECL_VAR, pos, name);
            d->var.type = type;
            d->var.expr = expr;
            return d;
        }

        Decl* new_decl_func(SrcPos pos, char* name, FuncParam* @params, int num_params, Typespec* ret_type, bool has_varargs, StmtList block)
        {
            var d = new_decl(DECL_FUNC, pos, name);
            d->func.@params = (FuncParam*) ast_dup(@params, num_params * sizeof(FuncParam));
            d->func.num_params = num_params;
            d->func.ret_type = ret_type;
            d->func.has_varargs = has_varargs;
            d->func.block = block;
            return d;
        }

        Decl* new_decl_const(SrcPos pos, char* name, Typespec* type, Expr* expr)
        {
            var d = new_decl(DECL_CONST, pos, name);
            d->const_decl.type = type;
            d->const_decl.expr = expr;
            return d;
        }

        Decl* new_decl_typedef(SrcPos pos, char* name, Typespec* type)
        {
            var d = new_decl(DECL_TYPEDEF, pos, name);
            d->typedef_decl.type = type;
            return d;
        }

        Decl* new_decl_note(SrcPos pos, Note note) {
            Decl *d = new_decl(DECL_NOTE, pos, null);
            d->note = note;
            return d;
        }

        Decl* new_decl_import(SrcPos pos, bool is_relative, char** names, int num_names, bool import_all, ImportItem* items, int num_items) {
            Decl *d = new_decl(DECL_IMPORT, pos, null);
            d->import.is_relative = is_relative;
            d->import.names = (char**)ast_dup(names, sizeof(char**) * num_names);
            d->import.num_names = num_names;
            d->import.import_all = import_all;
            d->import.items = (ImportItem*)ast_dup(items, sizeof(ImportItem) * num_items);
            d->import.num_items = num_items;
            return d;
        }



        Expr* new_expr(ExprKind kind, SrcPos pos)
        {
            var e = (Expr*) ast_alloc(sizeof(Expr));
            e->pos = pos;
            e->kind = kind;
            return e;
        }

        Expr* new_expr_paren(SrcPos pos, Expr* expr) {
            Expr *e = new_expr(EXPR_PAREN, pos);
            e->paren.expr = expr;
            return e;
        }

        Expr* new_expr_sizeof_expr(SrcPos pos, Expr* expr)
        {
            var e = new_expr(EXPR_SIZEOF_EXPR, pos);
            e->sizeof_expr = expr;
            return e;
        }

        Expr* new_expr_sizeof_type(SrcPos pos, Typespec* type)
        {
            var e = new_expr(EXPR_SIZEOF_TYPE, pos);
            e->sizeof_type = type;
            return e;
        }

        Expr* new_expr_typeof_expr(SrcPos pos, Expr* expr) {
            Expr *e = new_expr(EXPR_TYPEOF_EXPR, pos);
            e->typeof_expr = expr;
            return e;
        }

        Expr* new_expr_typeof_type(SrcPos pos, Typespec* type) {
            Expr *e = new_expr(EXPR_TYPEOF_TYPE, pos);
            e->typeof_type = type;
            return e;
        }

        Expr* new_expr_alignof_expr(SrcPos pos, Expr* expr) {
            Expr *e = new_expr(EXPR_ALIGNOF_EXPR, pos);
            e->alignof_expr = expr;
            return e;
        }

        Expr* new_expr_alignof_type(SrcPos pos, Typespec* type) {
            Expr *e = new_expr(EXPR_ALIGNOF_TYPE, pos);
            e->alignof_type = type;
            return e;
        }
        Expr* new_expr_modify(SrcPos pos, TokenKind op, bool post, Expr* expr) {
            Expr *e = new_expr(EXPR_MODIFY, pos);
            e->modify.op = op;
            e->modify.post = post;
            e->modify.expr = expr;
            return e;
        }

        Expr* new_expr_offsetof(SrcPos pos, Typespec* type, char* name) {
            Expr *e = new_expr(EXPR_OFFSETOF, pos);
            e->offsetof_field.type = type;
            e->offsetof_field.name = name;
            return e;
        }

        Expr* new_expr_int(SrcPos pos, ulong val, TokenMod mod, TokenSuffix suffix)
        {
            var e = new_expr(EXPR_INT, pos);
            e->int_lit.val = val;
            e->int_lit.mod = mod;
            e->int_lit.suffix = suffix;
            return e;
        }

        Expr* new_expr_float(SrcPos pos, double val, TokenSuffix suffix)
        {
            var e = new_expr(EXPR_FLOAT, pos);
            e->float_lit.val = val;
            e->float_lit.suffix = suffix;
            return e;
        }

        Expr* new_expr_str(SrcPos pos, char* val, TokenMod mod)
        {
            var e = new_expr(EXPR_STR, pos);
            e->str_lit.val = val;
            e->str_lit.mod = mod;
            return e;
        }

        Expr* new_expr_name(SrcPos pos, char* name) {
            var e = new_expr(EXPR_NAME, pos);
            e->name = name;
            return e;
        }

        Expr* new_expr_compound(SrcPos pos, Typespec* type, CompoundField* fields, int num_fields)
        {
            var e = new_expr(EXPR_COMPOUND, pos);
            e->compound.type = type;
            e->compound.fields = (CompoundField*) ast_dup(fields, num_fields * sizeof(CompoundField));
            e->compound.num_fields = num_fields;
            return e;
        }

        Expr* new_expr_cast(SrcPos pos, Typespec* type, Expr* expr)
        {
            var e = new_expr(EXPR_CAST, pos);
            e->cast.type = type;
            e->cast.expr = expr;
            return e;
        }

        Expr* new_expr_call(SrcPos pos, Expr* expr, Expr** args, int num_args)
        {
            var e = new_expr(EXPR_CALL, pos);
            e->call.expr = expr;
            e->call.args = (Expr**) ast_dup(args, num_args * sizeof(Expr*));
            e->call.num_args = num_args;
            return e;
        }

        Expr* new_expr_index(SrcPos pos, Expr* expr, Expr* index)
        {
            var e = new_expr(EXPR_INDEX, pos);
            e->index.expr = expr;
            e->index.index = index;
            return e;
        }

        Expr* new_expr_field(SrcPos pos, Expr* expr, char* name)
        {
            var e = new_expr(EXPR_FIELD, pos);
            e->field.expr = expr;
            e->field.name = name;
            return e;
        }

        Expr* new_expr_unary(SrcPos pos, TokenKind op, Expr* expr)
        {
            var e = new_expr(EXPR_UNARY, pos);
            e->unary.op = op;
            e->unary.expr = expr;
            return e;
        }

        Expr* new_expr_binary(SrcPos pos, TokenKind op, Expr* left, Expr* right)
        {
            var e = new_expr(EXPR_BINARY, pos);
            e->binary.op = op;
            e->binary.left = left;
            e->binary.right = right;
            return e;
        }

        Expr* new_expr_ternary(SrcPos pos, Expr* cond, Expr* then_expr, Expr* else_expr)
        {
            var e = new_expr(EXPR_TERNARY, pos);
            e->ternary.cond = cond;
            e->ternary.then_expr = then_expr;
            e->ternary.else_expr = else_expr;
            return e;
        }
        Note* get_stmt_note(Stmt* stmt, char* name) {
            for (var i = 0; i < stmt->notes.num_notes; i++) {
                Note *note = stmt->notes.notes + i;
                if (note->name == name) {
                    return note;
                }
            }
            return null;
        }
        Stmt* new_stmt_note(SrcPos pos, Note note) {
            Stmt *s = new_stmt(STMT_NOTE, pos);
            s->note = note;
            return s;
        }



        Stmt* new_stmt(StmtKind kind, SrcPos pos)
        {
            var s = (Stmt*) ast_alloc(sizeof(Stmt));
            s->pos = pos;
            s->kind = kind;
            return s;
        }

        Stmt* new_stmt_decl(SrcPos pos, Decl* decl)
        {
            var s = new_stmt(STMT_DECL, pos);
            s->decl = decl;
            return s;
        }

        Stmt* new_stmt_return(SrcPos pos, Expr* expr)
        {
            var s = new_stmt(STMT_RETURN, pos);
            s->expr = expr;
            return s;
        }

        Stmt* new_stmt_break(SrcPos pos)
        {
            return new_stmt(STMT_BREAK, pos);
        }

        Stmt* new_stmt_continue(SrcPos pos)
        {
            return new_stmt(STMT_CONTINUE, pos);
        }

        Stmt* new_stmt_block(SrcPos pos, StmtList block)
        {
            var s = new_stmt(STMT_BLOCK, pos);
            s->block = block;
            return s;
        }

        Stmt* new_stmt_if(SrcPos pos, Stmt* init, Expr* cond, StmtList then_block, ElseIf** elseifs, int num_elseifs,
            StmtList else_block)
        {
            var s = new_stmt(STMT_IF, pos);
            s->if_stmt.cond = cond;
            s->if_stmt.init = init;
            s->if_stmt.then_block = then_block;
            s->if_stmt.elseifs = (ElseIf**) ast_dup(elseifs, num_elseifs * sizeof(ElseIf*));
            s->if_stmt.num_elseifs = num_elseifs;
            s->if_stmt.else_block = else_block;
            return s;
        }

        Stmt* new_stmt_while(SrcPos pos, Expr* cond, StmtList block)
        {
            var s = new_stmt(STMT_WHILE, pos);
            s->while_stmt.cond = cond;
            s->while_stmt.block = block;
            return s;
        }

        Stmt* new_stmt_do_while(SrcPos pos, Expr* cond, StmtList block)
        {
            var s = new_stmt(STMT_DO_WHILE, pos);
            s->while_stmt.cond = cond;
            s->while_stmt.block = block;
            return s;
        }

        Stmt* new_stmt_for(SrcPos pos, Stmt* init, Expr* cond, Stmt* next, StmtList block)
        {
            var s = new_stmt(STMT_FOR, pos);
            s->for_stmt.init = init;
            s->for_stmt.cond = cond;
            s->for_stmt.next = next;
            s->for_stmt.block = block;
            return s;
        }

        Stmt* new_stmt_switch(SrcPos pos, Expr* expr, SwitchCase* cases, int num_cases)
        {
            var s = new_stmt(STMT_SWITCH, pos);
            s->switch_stmt.expr = expr;
            s->switch_stmt.cases = (SwitchCase*) ast_dup(cases, num_cases * sizeof(SwitchCase));
            s->switch_stmt.num_cases = num_cases;
            return s;
        }

        Stmt* new_stmt_assign(SrcPos pos, TokenKind op, Expr* left, Expr* right)
        {
            var s = new_stmt(STMT_ASSIGN, pos);
            s->assign.op = op;
            s->assign.left = left;
            s->assign.right = right;
            return s;
        }

        Stmt* new_stmt_init(SrcPos pos, char* name, Typespec* type, Expr* expr)
        {
            var s = new_stmt(STMT_INIT, pos);
            s->init.name = name;
            s->init.type = type;
            s->init.expr = expr;
            return s;
        }

        Stmt* new_stmt_expr(SrcPos pos, Expr* expr)
        {
            var s = new_stmt(STMT_EXPR, pos);
            s->expr = expr;
            return s;
        }
    }
}