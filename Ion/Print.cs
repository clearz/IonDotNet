using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCross.Memory;

namespace Lang
{
    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static TokenKind;

#if X64
    using size_t = Int64;
#else
    using size_t = Int32;
#endif
    unsafe partial class Ion
    {
        int indent;

        //char* print_buf;
        private StringBuilder sb = new StringBuilder(256);
        bool use_print_buf;

        void printf(string format, params object[] @params) {
            if (use_print_buf)
                sb.AppendFormat(format, @params);
            else
                Console.Write(format, @params);
        }

        void printf(string format, char* str) => printf(format, new String(str));

        void flush_print_buf(StreamWriter file) {
            if (sb.Length > 0) {
                file?.Write(sb.ToString());
                file?.Flush();
            }
        }

        void print_newline() {
            printf("\n{0}", "".PadLeft(2 * indent));
        }

        void print_typespec(Typespec* type) {
            Typespec* t = type;
            switch (t->kind) {
                case TYPESPEC_NAME:
                    printf("{0}", t->name);
                    break;
                case TYPESPEC_FUNC:
                    printf("(func (");
                    for (Typespec** it = t->func.args; it != t->func.args + t->func.num_args; it++) {
                        printf(" ");
                        print_typespec(*it);
                    }

                    printf(" ) ");
                    print_typespec(t->func.ret);
                    printf(")");
                    break;
                case TYPESPEC_ARRAY:
                    printf("(array ");
                    print_typespec(t->array.elem);
                    printf(" ");
                    print_expr(t->array.size);
                    printf(")");
                    break;
                case TYPESPEC_PTR:
                    printf("(ptr ");
                    print_typespec(t->ptr.elem);
                    printf(")");
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void print_expr(Expr* expr) {
            Expr* e = expr;
            switch (e->kind) {
                case EXPR_INT:
                    printf("{0}", e->int_val);
                    break;
                case EXPR_FLOAT:
                    printf("{0}", e->float_val);
                    break;
                case EXPR_STR:
                    printf("\"{0}\"", e->str_val);
                    break;
                case EXPR_NAME:
                    printf("{0}", e->name);
                    break;
                case EXPR_SIZEOF_EXPR:
                    printf("(sizeof-expr ");
                    print_expr(e->sizeof_expr);
                    printf(")");
                    break;
                case EXPR_SIZEOF_TYPE:
                    printf("(sizeof-type ");
                    print_typespec(e->sizeof_type);
                    printf(")");
                    break;
                case EXPR_CAST:
                    printf("(cast ");
                    print_typespec(e->cast.type);
                    printf(" ");
                    print_expr(e->cast.expr);
                    printf(")");
                    break;
                case EXPR_CALL:
                    printf("(");
                    print_expr(e->call.expr);
                    for (Expr** it = e->call.args; it != e->call.args + e->call.num_args; it++) {
                        printf(" ");
                        print_expr(*it);
                    }

                    printf(")");
                    break;
                case EXPR_INDEX:
                    printf("(index ");
                    print_expr(e->index.expr);
                    printf(" ");
                    print_expr(e->index.index);
                    printf(")");
                    break;
                case EXPR_FIELD:
                    printf("(field ");
                    print_expr(e->field.expr);
                    printf(" {0})", e->field.name);
                    break;
                case EXPR_COMPOUND:
                    printf("(compound ");
                    if (e->compound.type != null) {
                        print_typespec(e->compound.type);
                    }
                    else {
                        printf("nil");
                    }

                    for (Expr** it = e->compound.args; it != e->compound.args + e->compound.num_args; it++) {
                        printf(" ");
                        print_expr(*it);
                    }

                    printf(")");
                    break;
                case EXPR_UNARY:
                    printf("({0} ", token_kind_name(e->unary.op));
                    print_expr(e->unary.expr);
                    printf(")");
                    break;
                case EXPR_BINARY:
                    printf("({0} ", token_kind_name(e->binary.op));
                    print_expr(e->binary.left);
                    printf(" ");
                    print_expr(e->binary.right);
                    printf(")");
                    break;
                case EXPR_TERNARY:
                    printf("(? ");
                    print_expr(e->ternary.cond);
                    printf(" ");
                    print_expr(e->ternary.then_expr);
                    printf(" ");
                    print_expr(e->ternary.else_expr);
                    printf(")");
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void print_stmt_block(StmtBlock block) {
            printf("(block");
            indent++;
            for (Stmt** it = block.stmts; it != block.stmts + block.num_stmts; it++) {
                print_newline();
                print_stmt(*it);
            }

            indent--;
            printf(")");
        }

        void print_stmt(Stmt* stmt) {
            Stmt* s = stmt;
            switch (s->kind) {
                case STMT_DECL:
                    print_decl(s->decl);
                    break;
                case STMT_RETURN:
                    printf("(return");
                    if (s->return_stmt.expr != null) {
                        printf(" ");
                        print_expr(s->return_stmt.expr);
                    }

                    printf(")");
                    break;
                case STMT_BREAK:
                    printf("(break)");
                    break;
                case STMT_CONTINUE:
                    printf("(continue)");
                    break;
                case STMT_BLOCK:
                    print_stmt_block(s->block);
                    break;
                case STMT_IF:
                    printf("(if ");
                    print_expr(s->if_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s->if_stmt.then_block);
                    for (ElseIf* it = s->if_stmt.elseifs; it != s->if_stmt.elseifs + s->if_stmt.num_elseifs; it++) {
                        print_newline();
                        printf("elseif ");
                        print_expr(it->cond);
                        print_newline();
                        print_stmt_block(it->block);
                    }

                    if (s->if_stmt.else_block.num_stmts != 0) {
                        print_newline();
                        printf("else ");
                        print_newline();
                        print_stmt_block(s->if_stmt.else_block);
                    }

                    indent--;
                    printf(")");
                    break;
                case STMT_WHILE:
                    printf("(while ");
                    print_expr(s->while_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s->while_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case STMT_DO_WHILE:
                    printf("(do-while ");
                    print_expr(s->while_stmt.cond);
                    indent++;
                    print_newline();
                    print_stmt_block(s->while_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case STMT_FOR:
                    printf("(for ");
                    print_stmt(s->for_stmt.init);
                    print_expr(s->for_stmt.cond);
                    print_stmt(s->for_stmt.next);
                    indent++;
                    print_newline();
                    print_stmt_block(s->for_stmt.block);
                    indent--;
                    printf(")");
                    break;
                case STMT_SWITCH:
                    printf("(switch ");
                    print_expr(s->switch_stmt.expr);
                    indent++;
                    for (SwitchCase* it = s->switch_stmt.cases;
                        it != s->switch_stmt.cases + s->switch_stmt.num_cases;
                        it++) {
                        print_newline();
                        printf("(case ({0}", it->is_default ? " default" : "");
                        for (Expr** expr = it->exprs; expr != it->exprs + it->num_exprs; expr++) {
                            printf(" ");
                            print_expr(*expr);
                        }

                        printf(" ) ");
                        indent++;
                        print_newline();
                        print_stmt_block(it->block);
                        indent--;
                    }

                    indent--;
                    printf(")");
                    break;
                case STMT_ASSIGN:
                    printf("({0} ", token_kind_names[(int) s->assign.op]);
                    print_expr(s->assign.left);
                    if (s->assign.right != null) {
                        printf(" ");
                        print_expr(s->assign.right);
                    }

                    printf(")");
                    break;
                case STMT_INIT:
                    printf("(:= {0} ", s->init.name);
                    print_expr(s->init.expr);
                    printf(")");
                    break;
                case STMT_EXPR:
                    print_expr(s->expr);
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void print_aggregate_decl(Decl* decl) {
            Decl* d = decl;
            for (AggregateItem* it = d->aggregate.items; it != d->aggregate.items + d->aggregate.num_items; it++) {
                print_newline();
                printf("(");
                print_typespec(it->type);
                for (char** name = it->names; name != it->names + it->num_names; name++) {
                    printf(" {0}", *name);
                }

                printf(")");
            }
        }

        void print_decl(Decl* decl) {
            Decl* d = decl;
            switch (d->kind) {
                case DECL_ENUM:
                    printf("(enum {0}", d->name);
                    indent++;
                    for (EnumItem* it = d->enum_decl.items; it != d->enum_decl.items + d->enum_decl.num_items; it++) {
                        print_newline();
                        printf("({0} ", it->name);
                        if (it->init != null) {
                            print_expr(it->init);
                        }
                        else {
                            printf("nil");
                        }

                        printf(")");
                    }

                    indent--;
                    printf(")");
                    break;
                case DECL_STRUCT:
                    printf("(struct {0}", d->name);
                    indent++;
                    print_aggregate_decl(d);
                    indent--;
                    printf(")");
                    break;
                case DECL_UNION:
                    printf("(union {0}", d->name);
                    indent++;
                    print_aggregate_decl(d);
                    indent--;
                    printf(")");
                    break;
                case DECL_VAR:
                    printf("(var {0} ", d->name);
                    if (d->var.type != null) {
                        print_typespec(d->var.type);
                    }
                    else {
                        printf("nil");
                    }

                    printf(" ");
                    print_expr(d->var.expr);
                    printf(")");
                    break;
                case DECL_CONST:
                    printf("({0} ", d->name);
                    print_expr(d->const_decl.expr);
                    printf(")");
                    break;
                case DECL_TYPEDEF:
                    printf("(typedef {0} ", d->name);
                    print_typespec(d->typedef_decl.type);
                    printf(")");
                    break;
                case DECL_FUNC:
                    printf("(func {0} ", d->name);
                    printf("(");
                    for (FuncParam* it = d->func.@params; it != d->func.@params + d->func.num_params; it++) {
                        printf(" {0} ", it->name);
                        print_typespec(it->type);
                    }

                    printf(" ) ");
                    if (d->func.ret_type != null) {
                        print_typespec(d->func.ret_type);
                    }
                    else {
                        printf("nil");
                    }

                    indent++;
                    print_newline();
                    print_stmt_block(d->func.block);
                    indent--;
                    printf(")");
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        internal void print_test() {
            use_print_buf = true;
            // Expressions
            Expr*[] exprs = {
                expr_binary(TOKEN_ADD, expr_int(1), expr_int(2)),
                expr_unary(TOKEN_SUB, expr_float(3.14)),
                expr_ternary(expr_name("flag".ToPtr()), expr_str("true".ToPtr()), expr_str("false".ToPtr())),
                expr_field(expr_name("person".ToPtr()), "name".ToPtr()),
                expr_call(expr_name("fact".ToPtr()), (Expr**) PtrBuffer.Array(expr_int(42)), 1),
                expr_index(expr_field(expr_name("person".ToPtr()), "siblings".ToPtr()), expr_int(3)),
                expr_cast(typespec_ptr(typespec_name("int".ToPtr())), expr_name("void_ptr".ToPtr())),
                expr_compound(typespec_name("Vector".ToPtr()), (Expr**) PtrBuffer.Array(expr_int(1), expr_int(2)), 2),
            };
            foreach (Expr* it in exprs) {
                print_expr(it);
                printf("\n\n");
            }

            printf("\n\n");
            var elif = new ElseIf {
                cond = expr_name("flag2".ToPtr()),
                block = new StmtBlock {
                    stmts = (Stmt**)
                        PtrBuffer.Array(stmt_return(expr_int(2))
                        ),
                    num_stmts = 1,
                }
            };

            // Statements
            Stmt*[] stmts = {
                stmt_return(expr_int(42)),
                stmt_break(),
                stmt_continue(),
                stmt_block(
                    new StmtBlock {
                        stmts = (Stmt**)
                            PtrBuffer.Array(
                                stmt_break(),
                                stmt_continue()
                            ),
                        num_stmts = 2,
                    }
                ),
                stmt_expr(expr_call(expr_name("print".ToPtr()), (Expr**) PtrBuffer.Array(expr_int(1), expr_int(2)), 2)),
                stmt_init("x".ToPtr(), expr_int(42)),
                stmt_if(
                    expr_name("flag1".ToPtr()),
                    new StmtBlock {
                        stmts = (Stmt**)
                            PtrBuffer.Array(
                                stmt_return(expr_int(1))
                            ),
                        num_stmts = 1,
                    },
                    &elif,
                    1,
                    new StmtBlock {
                        stmts = (Stmt**)
                            PtrBuffer.Array(stmt_return(expr_int(3))
                            ),
                        num_stmts = 1,
                    }
                ),
                stmt_while(
                    expr_name("running".ToPtr()),
                    new StmtBlock {
                        stmts = (Stmt**)
                            PtrBuffer.Array(stmt_assign(TOKEN_ADD_ASSIGN, expr_name("i".ToPtr()), expr_int(16))),
                        num_stmts = 1,
                    }
                ),
                stmt_switch(
                    expr_name("val".ToPtr()),
                    (SwitchCase*) new[] {
                        new SwitchCase {
                            exprs = (Expr**) PtrBuffer.Array(expr_int(3), expr_int(4)),
                            num_exprs = 2,
                            is_default = false,
                            block = new StmtBlock {
                                stmts = (Stmt**)
                                    PtrBuffer.Array(stmt_return(expr_name("val".ToPtr()))),
                                num_stmts = 1,
                            },
                        },
                        new SwitchCase {
                            exprs = (Expr**) PtrBuffer.Array(expr_int(1)),
                            num_exprs = 1,
                            is_default = true,
                            block = new StmtBlock {
                                stmts = (Stmt**)
                                    PtrBuffer.Array(stmt_return(expr_int(0))),
                                num_stmts = 1,
                            },
                        }
                    }.ToArrayPtr(),
                    2
                ),
            };
            foreach (Stmt* it in stmts) {
                print_stmt(it);
                printf("\n\n");
            }

            Console.WriteLine();
            flush_print_buf(new StreamWriter(Console.OpenStandardOutput()));
            use_print_buf = false;
        }

    }
}
