using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MLang
{
    class Print
    {
        int indent;

        //string print_buf;
        private StringBuilder sb = new StringBuilder(256);
        bool use_print_buf;

        internal void printf(string format, params object[] @params) {
            if (use_print_buf)
                sb.AppendFormat(format, @params);
            else
                Console.Write(format, @params);
        }

        internal void printf(string format, string str) => printf(format, (object)str);

        void flush_print_buf(StreamWriter file) {
            if (sb.Length > 0) {
                file?.Write(sb.ToString());
                file?.Flush();
            }
        }

        void print_newline() {
            printf("\n{0}", "".PadLeft(2 * indent));
        }

        void print_typespec(Typespec type) {
            Typespec t = type;
            switch (t.kind) {
                case TypespecKind.TYPESPEC_NAME:
                    printf("{0}", t.name);
                    break;
                case TypespecKind.TYPESPEC_FUNC:
                    printf("(func (");
                    foreach (Typespec it in t.func.args) {
                        printf(" ");
                        print_typespec(it);
                    }

                    printf(" ) ");
                    print_typespec(t.func.ret);
                    printf(")");
                    break;
                case TypespecKind.TYPESPEC_ARRAY:
                    printf("(array ");
                    print_typespec(t.array.elem);
                    printf(" ");
                    print_expr(t.array.size);
                    printf(")");
                    break;
                case TypespecKind.TYPESPEC_PTR:
                    printf("(ptr ");
                    print_typespec(t.ptr.elem);
                    printf(")");
                    break;
                default:
                    Error.assert(false);
                    break;
            }
        }

        void print_expr(Expr expr) {
            if(expr == null) return;
            Expr e = expr;
            switch (e.kind) {
                case ExprKind.EXPR_INT:
                    printf("{0}", e.int_val);
                    break;
                case ExprKind.EXPR_FLOAT:
                    printf("{0}", e.float_val);
                    break;
                case ExprKind.EXPR_STR:
                    printf("\"{0}\"", e.str_val);
                    break;
                case ExprKind.EXPR_NAME:
                    printf("{0}", e.name);
                    break;
                case ExprKind.EXPR_SIZEOF_EXPR:
                    printf("(sizeof-expr ");
                    print_expr(e.sizeof_expr);
                    printf(")");
                    break;
                case ExprKind.EXPR_SIZEOF_TYPE:
                    printf("(sizeof-type ");
                    print_typespec(e.sizeof_type);
                    printf(")");
                    break;
                case ExprKind.EXPR_CAST:
                    printf("(cast ");
                    print_typespec(e.cast.type);
                    printf(" ");
                    print_expr(e.cast.expr);
                    printf(")");
                    break;
                case ExprKind.EXPR_CALL:
                    printf("(");
                    print_expr(e.call.expr);
                    foreach(var it in e.call.args) {
                        printf(" ");
                        print_expr(it);
                    }

                    printf(")");
                    break;
                case ExprKind.EXPR_INDEX:
                    printf("(index ");
                    print_expr(e.index.expr);
                    printf(" ");
                    print_expr(e.index.index);
                    printf(")");
                    break;
                case ExprKind.EXPR_FIELD:
                    printf("(field ");
                    print_expr(e.field.expr);
                    printf(" {0})", e.field.name);
                    break;
                case ExprKind.EXPR_COMPOUND:
                    printf("(compound ");
                    if (e.compound.type != null) {
                        print_typespec(e.compound.type);
                    }
                    else {
                        printf("nil");
                    }

                    foreach (var it in e.compound.fields){
                        printf(" ");
                        if (it.kind == CompoundFieldKind.FIELD_DEFAULT)
                        {
                            printf("(nil ");
                        }
                        else if (it.kind == CompoundFieldKind.FIELD_NAME)
                        {
                            printf("(name {0} ", it.name);
                        }
                        else
                        {
                            Debug.Assert(it.kind == CompoundFieldKind.FIELD_INDEX);
                            printf("(index ");
                            print_expr(it.index);
                            printf(" ");
                        }
                        print_expr(it.init);
                        printf(")");
                    }

                    printf(")");
                    break;
                case ExprKind.EXPR_UNARY:
                    printf("({0} ", Lexer.token_kind_name(e.unary.op));
                    print_expr(e.unary.expr);
                    printf(")");
                    break;
                case ExprKind.EXPR_BINARY:
                    printf("({0} ", Lexer.token_kind_name(e.binary.op));
                    print_expr(e.binary.left);
                    printf(" ");
                    print_expr(e.binary.right);
                    printf(")");
                    break;
                case ExprKind.EXPR_TERNARY:
                    printf("(? ");
                    print_expr(e.ternary.cond);
                    printf(" ");
                    print_expr(e.ternary.then_expr);
                    printf(" ");
                    print_expr(e.ternary.else_expr);
                    printf(")");
                    break;
                default:
                    Error.assert(false);
                    break;
            }
        }

        void print_stmt_block(StmtBlock block) {
            printf("(block");
            indent++;
            foreach(var it in block.stmts) { 
                print_newline();
                print_stmt(it);
            }

            indent--;
            printf(")");
        }

        void print_stmt(Stmt stmt) {
            Stmt s = stmt;
            switch (s.kind) {
                case StmtKind.STMT_DECL:
                    print_decl(s.decl);
                    break;
                case StmtKind.STMT_RETURN:
                    printf("(return");
                    if (s.expr != null) {
                        printf(" ");
                        print_expr(s.expr);
                    }

                    printf(")");
                    break;
                case StmtKind.STMT_BREAK:
                    printf("(break)");
                    break;
                case StmtKind.STMT_CONTINUE:
                    printf("(continue)");
                    break;
                case StmtKind.STMT_BLOCK:
                    print_stmt_block(s.block);
                    break;
                case StmtKind.STMT_IF:
                    printf("(if ");
                    print_expr(s.if_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s.if_stmt.then_block);
                    foreach(var it in s.if_stmt.elseifs) {
                        print_newline();
                        printf("elseif ");
                        print_expr(it.cond);
                        print_newline();
                        print_stmt_block(it.block);
                    }

                    if (s.if_stmt.else_block != null && s.if_stmt.else_block.num_stmts != 0) {
                        print_newline();
                        printf("else ");
                        print_newline();
                        print_stmt_block(s.if_stmt.else_block);
                    }

                    indent--;
                    printf(")");
                    break;
                case StmtKind.STMT_WHILE:
                    printf("(while ");
                    print_expr(s.while_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s.while_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case StmtKind.STMT_DO_WHILE:
                    printf("(do-while ");
                    print_expr(s.while_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s.while_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case StmtKind.STMT_FOR:
                    printf("(for ");
                    print_stmt(s.for_stmt.init);
                    print_expr(s.for_stmt.cond);
                    print_stmt(s.for_stmt.next);
                    indent++;
                    print_newline();
                    print_stmt_block(s.for_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case StmtKind.STMT_SWITCH:
                    printf("(switch ");
                    print_expr(s.switch_stmt.expr);
                    indent++;
                    foreach(var it in s.switch_stmt.cases) {
                        print_newline();
                        printf("(case ({0}", it.is_default ? " default" : "");
                        foreach(var expr in it.exprs) {
                            printf(" ");
                            print_expr(expr);
                        }

                        printf(" ) ");
                        indent++;
                        print_newline();
                        print_stmt_block(it.block);
                        indent--;
                    }

                    indent--;
                    printf(")");
                    break;
                case StmtKind.STMT_ASSIGN:
                    printf("({0} ", Lexer.token_kind_name(s.assign.op));
                    print_expr(s.assign.left);
                    if (s.assign.right != null) {
                        printf(" ");
                        print_expr(s.assign.right);
                    }

                    printf(")");
                    break;
                case StmtKind.STMT_INIT:
                    printf("(:= {0} ", s.init.name);
                    print_expr(s.init.expr);
                    printf(")");
                    break;
                case StmtKind.STMT_EXPR:
                    print_expr(s.expr);
                    break;
                default:
                    Error.assert(false);
                    break;
            }
        }

        void print_aggregate_decl(Decl decl) {
            Decl d = decl;
            foreach(var it in d.aggregate.items) {
                print_newline();
                printf("(");
                print_typespec(it.type);
                foreach(var name in it.names){
                    printf(" {0}", name);
                }

                printf(")");
            }
        }

        internal void print_decl(Decl decl) {
            Decl d = decl;
            switch (d.kind) {
                case DeclKind.DECL_ENUM:
                    printf("(enum {0}", d.name);
                    indent++;
                    foreach(var it in d.enum_decl.items) {
                        print_newline();
                        printf("({0} ", it.name);
                        if (it.init != null) {
                            print_expr(it.init);
                        }
                        else {
                            printf("nil");
                        }

                        printf(")");
                    }

                    indent--;
                    printf(")");
                    break;
                case DeclKind.DECL_STRUCT:
                    printf("(struct {0}", d.name);
                    indent++;
                    print_aggregate_decl(d);
                    indent--;
                    printf(")");
                    break;
                case DeclKind.DECL_UNION:
                    printf("(union {0}", d.name);
                    indent++;
                    print_aggregate_decl(d);
                    indent--;
                    printf(")");
                    break;
                case DeclKind.DECL_VAR:
                    printf("(var {0} ", d.name);
                    if (d.var.type != null) {
                        print_typespec(d.var.type);
                    }
                    else {
                        printf("nil");
                    }

                    printf(" ");
                    print_expr(d.var.expr);
                    printf(")");
                    break;
                case DeclKind.DECL_CONST:
                    printf("({0} ", d.name);
                    print_expr(d.const_decl.expr);
                    printf(")");
                    break;
                case DeclKind.DECL_TYPEDEF:
                    printf("(typedef {0} ", d.name);
                    print_typespec(d.typedef_decl.type);
                    printf(")");
                    break;
                case DeclKind.DECL_FUNC:
                    printf("(func {0} ", d.name);
                    printf("(");
                    foreach(var it in d.func.@params) {
                        printf(" {0} ", it.name);
                        print_typespec(it.type);
                    }

                    printf(" ) ");
                    if (d.func.ret_type != null) {
                        print_typespec(d.func.ret_type);
                    }
                    else {
                        printf("nil");
                    }

                    indent++;
                    print_newline();
                    print_stmt_block(d.func.block);
                    indent--;
                    printf(")");
                    break;
                default:
                    Error.assert(false);
                    break;
            }
        }

        internal void print_test() {
            Ast ast = new Ast();
            use_print_buf = true;
            // Expressions 
            Expr[] exprs = new []{
                ast.expr_binary(TokenKind.TOKEN_ADD, ast.expr_int(1), ast.expr_int(2)),
                ast.expr_unary(TokenKind.TOKEN_SUB, ast.expr_float(3.14)),
                ast.expr_ternary(ast.expr_name("flag"), ast.expr_str("true"), ast.expr_str("false")),
                ast.expr_field(ast.expr_name("person"), "name"),
                ast.expr_call(ast.expr_name("fact"), new []{ ast.expr_int(42)}, 1),
                ast.expr_index(ast.expr_field(ast.expr_name("person"), "siblings"), ast.expr_int(3)),
                Ast.expr_cast(ast.typespec_ptr(ast.typespec_name("int")), ast.expr_name("void_ptr")),
            };
            foreach (Expr it in exprs) {
                print_expr(it);
                printf("\n\n");
            }

            printf("\n\n");
            var elif = new ElseIf {
                cond = ast.expr_name("flag2"),
                block = new StmtBlock {
                    stmts = new Stmt[] {
                        ast.stmt_return(ast.expr_int(2))
                        },
                    num_stmts = 1,
                }
            };

            // Statements
            Stmt[] stmts = new[]{
                ast.stmt_return(ast.expr_int(42)),
                ast.stmt_break(),
                ast.stmt_continue(),
                ast.stmt_block(
                    new StmtBlock {
                        stmts = new Stmt[]{
                                ast.stmt_break(),
                                ast.stmt_continue()
                            },
                        num_stmts = 2,
                    }
                ),
                ast.stmt_expr(ast.expr_call(ast.expr_name("print"), new []{ast.expr_int(1), ast.expr_int(2)}, 2)),
                ast.stmt_init("x", ast.expr_int(42)),
                ast.stmt_if(
                    ast.expr_name("flag1"),
                    new StmtBlock {
                        stmts = new Stmt[]{
                                ast.stmt_return(ast.expr_int(1))
                            },
                        num_stmts = 1,
                    },
                    new []{elif},
                    1,
                    new StmtBlock {
                        stmts = new Stmt[]{
                            ast.stmt_return(ast.expr_int(3))
                            },
                        num_stmts = 1,
                    }
                ),
                ast.stmt_while(
                    ast.expr_name("running"),
                    new StmtBlock {
                        stmts = new Stmt[]{
                            ast.stmt_assign(TokenKind.TOKEN_ADD_ASSIGN, ast.expr_name("i"), ast.expr_int(16))
                        },
                        num_stmts = 1,
                    }
                ),
                ast.stmt_switch(
                    ast.expr_name("val"),
                    new SwitchCase[] {
                        new SwitchCase {
                            exprs = new []{ast.expr_int(3), ast.expr_int(4)},
                            num_exprs = 2,
                            is_default = false,
                            block = new StmtBlock {
                                stmts = new Stmt[]{
                                    ast.stmt_return(ast.expr_name("val"))
                                },
                                num_stmts = 1,
                            },
                        },
                        new SwitchCase {
                            exprs = new []{ast.expr_int(1)},
                            num_exprs = 1,
                            is_default = true,
                            block = new StmtBlock {
                                stmts = new Stmt[]{
                                    ast.stmt_return(ast.expr_int(0))},
                                num_stmts = 1,
                            },
                        }
                    },
                    2
                ),
            };
            foreach (Stmt it in stmts) {
                print_stmt(it);
                printf("\n\n");
            }

            flush_print_buf(new StreamWriter(Console.OpenStandardOutput()));
            use_print_buf = false;
        }

    }
}
