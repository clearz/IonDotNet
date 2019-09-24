using System;
using System.Linq;
using System.Net.Mail;

namespace MLang
{
    using static TokenKind;
    using static TokenMod;
    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;


    public class Ast
    {
        object ast_alloc(int size) {
            throw new NotImplementedException();
        }

        internal static T[] ast_dup<T>(T[] src) where T : ICloneable
        {
            
            if (src == null)
                return null;
            if (src.Length == 0) return src;

            return src.Select(s => s.Clone()).Cast<T>().ToArray();
        }
        StmtBlock stmt_list(Stmt[] stmts, int num_stmts)
        {
            return new StmtBlock { stmts = ast_dup(stmts), num_stmts = num_stmts };
        }
        Typespec typespec_new(TypespecKind kind) {
            Typespec t = new Typespec();
            t.kind = kind;
            return t;
        }

        internal Typespec typespec_name(string name) {
            Typespec t = typespec_new(TypespecKind.TYPESPEC_NAME);
            t.name = name;
            return t;
        }

        internal Typespec typespec_ptr(Typespec elem) {
            Typespec t = typespec_new(TypespecKind.TYPESPEC_PTR);
            t.ptr = new PtrTypespec();
            t.base = elem;
            return t;
        }

        internal Typespec typespec_array(Typespec elem, Expr size) {
            Typespec t = typespec_new(TypespecKind.TYPESPEC_ARRAY);
            t.array = new ArrayTypespec();
            t.array.elem = elem;
            t.array.size = size;
            return t;
        }

        internal Typespec typespec_func(Typespec[] args, int num_args, Typespec ret) {
            Typespec t = typespec_new(TypespecKind.TYPESPEC_FUNC);
            t.func = new FuncTypespec();
            t.func.args = args;
            t.func.num_args = num_args;
            t.func.ret = ret;
            return t;
        }

        Decl decl_new(DeclKind kind, string name) {
            Decl d = new Decl();
            d.kind = kind;
            d.name = name;
            return d;
        }

        internal Decl decl_enum(string name, EnumItem[] items, int num_items) {
            Decl d = decl_new(DeclKind.DECL_ENUM, name);
            d.enum_decl = new EnumDecl();
            d.enum_decl.items = items;
            d.enum_decl.num_items = num_items;
            return d;
        }

        internal Decl decl_aggregate(DeclKind kind, string name, AggregateItem[] items, int num_items) {
            Error.assert(kind == DeclKind.DECL_STRUCT || kind == DeclKind.DECL_UNION);
            Decl d = decl_new(kind, name);
            d.aggregate = new AggregateDecl();
            d.aggregate.items = ast_dup(items);
            d.aggregate.num_items = num_items;
            return d;
        }

        internal Decl decl_union(string name, AggregateItem[] items, int num_items) {
            Decl d = decl_new(DeclKind.DECL_UNION, name);
            d.aggregate = new AggregateDecl();
            d.aggregate.items = ast_dup(items);
            d.aggregate.num_items = num_items;
            return d;
        }

        internal Decl decl_var(string name, Typespec type, Expr expr) {
            Decl d = decl_new(DeclKind.DECL_VAR, name);
            d.var = new VarDecl();
            d.var.type = type;
            d.var.expr = expr;
            return d;
        }

        internal Decl decl_func(string name, FuncParam[] @params, int num_params, Typespec ret_type, StmtBlock block) {
            Decl d = decl_new(DeclKind.DECL_FUNC, name);
            d.func = new FuncDecl();
            d.func.@params = ast_dup(@params);
            d.func.num_params = num_params;
            d.func.ret_type = ret_type;
            d.func.block = block;
            return d;
        }

        internal Decl decl_const(string name, Expr expr) {
            Decl d = decl_new(DeclKind.DECL_CONST, name);
            d.const_decl = new ConstDecl();
            d.const_decl.expr = expr;
            return d;
        }

        internal Decl decl_typedef(string name, Typespec type) {
            Decl d = decl_new(DeclKind.DECL_TYPEDEF, name);
            d.typedef_decl = new TypedefDecl();
            d.typedef_decl.type = type;
            return d;
        }

        static Expr expr_new(ExprKind kind) {
            Expr e = new Expr();
            e.kind = kind;
            return e;
        }

        internal Expr expr_sizeof_expr(Expr expr) {
            Expr e = expr_new(ExprKind.EXPR_SIZEOF_EXPR);
            e.sizeof_expr = expr;
            return e;
        }

        internal Expr expr_sizeof_type(Typespec type) {
            Expr e = expr_new(ExprKind.EXPR_SIZEOF_TYPE);
            e.sizeof_type = type;
            return e;
        }

        internal Expr expr_int(ulong int_val) {
            Expr e = expr_new(ExprKind.EXPR_INT);
            e.int_val = int_val;
            return e;
        }

        internal Expr expr_float(double float_val) {
            Expr e = expr_new(ExprKind.EXPR_FLOAT);
            e.float_val = float_val;
            return e;
        }

        internal Expr expr_str(string str_val) {
            Expr e = expr_new(ExprKind.EXPR_STR);
            e.str_val = str_val;
            return e;
        }

        internal Expr expr_name(string name) {
            Expr e = expr_new(ExprKind.EXPR_NAME);
            e.name = name;
            return e;
        }

        internal Expr expr_compound(Typespec type, CompoundField[] fields, int num_args) {
            Expr e = expr_new(ExprKind.EXPR_COMPOUND);
            e.compound = new CompoundExpr();
            e.compound.type = type;
            e.compound.fields = ast_dup(fields);
            e.compound.num_fields = num_args;
            return e;
        }

        // Sort out static or not static? 
        internal static Expr expr_cast(Typespec type, Expr expr) {
            Expr e = expr_new(ExprKind.EXPR_CAST);
            e.cast = new CastExpr();
            e.cast.type = type;
            e.cast.expr = expr;
            return e;
        }

        internal Expr expr_call(Expr expr, Expr[] args, int num_args) {
            Expr e = expr_new(ExprKind.EXPR_CALL);
            e.call = new CallExpr();
            e.call.expr = expr;
            e.call.args = ast_dup(args);
            e.call.num_args = num_args;
            return e;
        }

        internal Expr expr_index(Expr expr, Expr index) {
            Expr e = expr_new(ExprKind.EXPR_INDEX);
            e.index = new IndexExpr();
            e.index.expr = expr;
            e.index.index = index;
            return e;
        }

        internal Expr expr_field(Expr expr, string name) {
            Expr e = expr_new(ExprKind.EXPR_FIELD);
            e.field = new FieldExpr();
            e.field.expr = expr;
            e.field.name = name;
            return e;
        }

        internal Expr expr_unary(TokenKind op, Expr expr) {
            Expr e = expr_new(ExprKind.EXPR_UNARY);
            e.unary = new UnaryExpr();
            e.unary.op = op;
            e.unary.expr = expr;
            return e;
        }

        internal Expr expr_binary(TokenKind op, Expr left, Expr right) {
            Expr e = expr_new(ExprKind.EXPR_BINARY);
            e.binary = new BinaryExpr();
            e.binary.op = op;
            e.binary.left = left;
            e.binary.right = right;
            return e;
        }

        internal Expr expr_ternary(Expr cond, Expr then_expr, Expr else_expr) {
            Expr e = expr_new(ExprKind.EXPR_TERNARY);
            e.ternary = new TernaryExpr();
            e.ternary.cond = cond;
            e.ternary.then_expr = then_expr;
            e.ternary.else_expr = else_expr;
            return e;
        }

        Stmt stmt_new(StmtKind kind) {
            Stmt s = new Stmt();
            s.kind = kind;
            return s;
        }

        internal Stmt stmt_decl(Decl decl) {
            Stmt s = stmt_new(StmtKind.STMT_DECL);
            s.decl = decl;
            return s;
        }

        internal Stmt stmt_return(Expr expr) {
            Stmt s = stmt_new(StmtKind.STMT_RETURN);
            s.expr = expr;
            return s;
        }

        internal Stmt stmt_break() {
            return stmt_new(StmtKind.STMT_BREAK);
        }

        internal Stmt stmt_continue() {
            return stmt_new(StmtKind.STMT_CONTINUE);
        }

        internal Stmt stmt_block(StmtBlock block) {
            Stmt s = stmt_new(StmtKind.STMT_BLOCK);
            s.block = block;
            return s;
        }

        internal Stmt stmt_if(Expr cond, StmtBlock then_block, ElseIf[] elseifs, int num_elseifs, StmtBlock else_block) {
            Stmt s = stmt_new(StmtKind.STMT_IF);
            s.if_stmt = new IfStmt();
            s.if_stmt.cond = cond;
            s.if_stmt.then_block = then_block;
            s.if_stmt.elseifs = ast_dup(elseifs);
            s.if_stmt.num_elseifs = num_elseifs;
            s.if_stmt.else_block = else_block;
            return s;
        }

        internal Stmt stmt_while(Expr cond, StmtBlock block) {
            Stmt s = stmt_new(StmtKind.STMT_WHILE);
            s.while_stmt = new WhileStmt();
            s.while_stmt.cond = cond;
            s.while_stmt.block = block;
            return s;
        }

        internal Stmt stmt_do_while(Expr cond, StmtBlock block) {
            Stmt s = stmt_new(StmtKind.STMT_DO_WHILE);
            s.while_stmt = new WhileStmt();
            s.while_stmt.cond = cond;
            s.while_stmt.block = block;
            return s;
        }

        internal Stmt stmt_for(Stmt init, Expr cond, Stmt next, StmtBlock block) {
            Stmt s = stmt_new(StmtKind.STMT_FOR);
            s.for_stmt = new ForStmt();
            s.for_stmt.init = init;
            s.for_stmt.cond = cond;
            s.for_stmt.next = next;
            s.for_stmt.block = block;
            return s;
        }

        internal Stmt stmt_switch(Expr expr, SwitchCase[] cases, int num_cases) {
            Stmt s = stmt_new(StmtKind.STMT_SWITCH);
            s.switch_stmt = new SwitchStmt();
            s.switch_stmt.expr = expr;
            s.switch_stmt.cases = ast_dup(cases);
            s.switch_stmt.num_cases = num_cases;
            return s;
        }

        internal Stmt stmt_assign(TokenKind op, Expr left, Expr right) {
            Stmt s = stmt_new(StmtKind.STMT_ASSIGN);
            s.assign = new AssignStmt();
            s.assign.op = op;
            s.assign.left = left;
            s.assign.right = right;
            return s;
        }

        internal Stmt stmt_init(string name, Expr expr) {
            Stmt s = stmt_new(StmtKind.STMT_INIT);
            s.init = new InitStmt();
            s.init.name = name;
            s.init.expr = expr;
            return s;
        }

        internal Stmt stmt_expr(Expr expr) {
            Stmt s = stmt_new(StmtKind.STMT_EXPR);
            s.expr = expr;
            return s;
        }

    }
}
