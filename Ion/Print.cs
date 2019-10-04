using System;
using System.IO;
using System.Text;

namespace IonLang
{
    #region Header

#if X64
    using size_t = System.Int64;
#else
    using size_t = System.Int32;
#endif

    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static CompoundFieldKind;

    #endregion
    unsafe partial class Ion
    {
        private int _indent;

        //char* print_buf;
        private readonly StringBuilder sb = new StringBuilder(256);
        private bool use_print_buf;

        private void printf(string format, params object[] @params)
        {
            if (use_print_buf)
                sb.AppendFormat(format, @params);
            else
                Console.Write(format, @params);
        }

        private void printf(string format, char* str)
        {
            printf(format, new string(str));
        }

        private void flush_print_buf(StreamWriter file)
        {
            if (sb.Length > 0)
            {
                file?.Write(sb.ToString());
                file?.Flush();
            }
        }

        private void print_newline()
        {
            printf("\n{0}", "".PadLeft(2 * _indent));
        }

        private void print_typespec(Typespec* type)
        {
            var t = type;
            switch (t->kind)
            {
                case TYPESPEC_NAME:
                    printf("{0}", t->name);
                    break;
                case TYPESPEC_FUNC:
                    printf("(func (");
                    for (var it = t->func.args; it != t->func.args + t->func.num_args; it++)
                    {
                        printf(" ");
                        print_typespec(*it);
                    }

                    printf(" ) ");
                    if (t->func.ret != null)
                        print_typespec(t->func.ret);
                    else
                        printf("void");
                    printf(")");
                    break;
                case TYPESPEC_ARRAY:
                    printf("(array ");
                    print_typespec(t->@base);
                    printf(" ");
                    if (t->num_elems != null)
                        print_expr(t->num_elems);
                    else
                        printf("nil");
                    printf(")");
                    break;
                case TYPESPEC_PTR:
                    printf("(ptr ");
                    print_typespec(t->@base);
                    printf(")");
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void print_expr(Expr* expr)
        {
            var e = expr;
            switch (e->kind)
            {
                case EXPR_INT:
                    printf("{0}", e->int_lit.val);
                    break;
                case EXPR_FLOAT:
                    printf("{0}", e->float_lit.val);
                    break;
                case EXPR_STR:
                    printf("\"{0}\"", e->str_lit.val);
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
                    for (var it = e->call.args; it != e->call.args + e->call.num_args; it++)
                    {
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
                    if (e->compound.type != null)
                        print_typespec(e->compound.type);
                    else
                        printf("nil");

                    for (var it = e->compound.fields; it != e->compound.fields + e->compound.num_fields; it++)
                    {
                        printf(" ");
                        if (it->kind == FIELD_DEFAULT)
                        {
                            printf("(nil ");
                        }
                        else if (it->kind == FIELD_NAME)
                        {
                            printf("(name {0} ", it->name);
                        }
                        else
                        {
                            assert(it->kind == FIELD_INDEX);
                            printf("(index ");
                            print_expr(it->index);
                            printf(" ");
                        }

                        print_expr(it->init);
                        printf(")");
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

        private void print_stmt_block(StmtList block)
        {
            printf("(block");
            _indent++;
            for (var it = block.stmts; it != block.stmts + block.num_stmts; it++)
            {
                print_newline();
                print_stmt(*it);
            }

            _indent--;
            printf(")");
        }

        private void print_stmt(Stmt* stmt)
        {
            var s = stmt;
            switch (s->kind)
            {
                case STMT_DECL:
                    print_decl(s->decl);
                    break;
                case STMT_RETURN:
                    printf("(return");
                    if (s->expr != null)
                    {
                        printf(" ");
                        print_expr(s->expr);
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
                    _indent++;
                    print_newline();
                    print_stmt_block(s->if_stmt.then_block);
                    for (var it = s->if_stmt.elseifs; it != s->if_stmt.elseifs + s->if_stmt.num_elseifs; it++)
                    {
                        print_newline();
                        printf("elseif ");
                        print_expr((*it)->cond);
                        print_newline();
                        print_stmt_block((*it)->block);
                    }

                    if (s->if_stmt.else_block.num_stmts != 0)
                    {
                        print_newline();
                        printf("else ");
                        print_newline();
                        print_stmt_block(s->if_stmt.else_block);
                    }

                    _indent--;
                    printf(")");
                    break;
                case STMT_WHILE:
                    printf("(while ");
                    print_expr(s->while_stmt.cond);
                    _indent++;
                    print_newline();
                    print_stmt_block(s->while_stmt.block);
                    _indent--;
                    printf(")");
                    break;
                case STMT_DO_WHILE:
                    printf("(do-while ");
                    print_expr(s->while_stmt.cond);
                    _indent++;
                    print_newline();
                    print_stmt_block(s->while_stmt.block);
                    _indent--;
                    printf(")");
                    break;
                case STMT_FOR:
                    printf("(for ");
                    print_stmt(s->for_stmt.init);
                    print_expr(s->for_stmt.cond);
                    print_stmt(s->for_stmt.next);
                    _indent++;
                    print_newline();
                    print_stmt_block(s->for_stmt.block);
                    _indent--;
                    printf(")");
                    break;
                case STMT_SWITCH:
                    printf("(switch ");
                    print_expr(s->switch_stmt.expr);
                    _indent++;
                    for (var it = s->switch_stmt.cases;
                        it != s->switch_stmt.cases + s->switch_stmt.num_cases;
                        it++)
                    {
                        print_newline();
                        printf("(case ({0}", it->is_default ? " default" : "");
                        for (var expr = it->exprs; expr != it->exprs + it->num_exprs; expr++)
                        {
                            printf(" ");
                            print_expr(*expr);
                        }

                        printf(" ) ");
                        _indent++;
                        print_newline();
                        print_stmt_block(it->block);
                        _indent--;
                    }

                    _indent--;
                    printf(")");
                    break;
                case STMT_ASSIGN:
                    printf("({0} ", token_kind_names[(size_t) s->assign.op]);
                    print_expr(s->assign.left);
                    if (s->assign.right != null)
                    {
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

        private void print_aggregate_decl(Decl* decl)
        {
            var d = decl;
            for (var it = d->aggregate.items; it != d->aggregate.items + d->aggregate.num_items; it++)
            {
                print_newline();
                printf("(");
                for (var name = it->names; name != it->names + it->num_names; name++) printf("{0} ", *name);
                print_typespec(it->type);
                printf(")");
            }
        }

        private void print_decl(Decl* decl)
        {
            var d = decl;
            switch (d->kind)
            {
                case DECL_ENUM:
                    printf("(enum {0}", d->name);
                    _indent++;
                    for (var it = d->enum_decl.items; it != d->enum_decl.items + d->enum_decl.num_items; it++)
                    {
                        print_newline();
                        printf("({0} ", it->name);
                        if (it->init != null)
                            print_expr(it->init);
                        else
                            printf("nil");

                        printf(")");
                    }

                    _indent--;
                    printf(")");
                    break;
                case DECL_STRUCT:
                    printf("(struct {0}", d->name);
                    _indent++;
                    print_aggregate_decl(d);
                    _indent--;
                    printf(")");
                    break;
                case DECL_UNION:
                    printf("(union {0}", d->name);
                    _indent++;
                    print_aggregate_decl(d);
                    _indent--;
                    printf(")");
                    break;
                case DECL_VAR:
                    printf("(var {0} ", d->name);
                    if (d->var.type != null)
                        print_typespec(d->var.type);
                    else
                        printf("nil");

                    printf(" ");
                    if (d->var.expr != null)
                        print_expr(d->var.expr);
                    else
                        printf("nil");

                    printf(")");
                    break;
                case DECL_CONST:
                    printf("(const {0} ", d->name);
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
                    for (var it = d->func.@params; it != d->func.@params + d->func.num_params; it++)
                    {
                        printf(" {0} ", it->name);
                        print_typespec(it->type);
                    }

                    printf(" ) ");
                    if (d->func.ret_type != null)
                        print_typespec(d->func.ret_type);
                    else
                        printf("nil");

                    _indent++;
                    print_newline();
                    print_stmt_block(d->func.block);
                    _indent--;
                    printf(")");
                    break;
                default:
                    assert(false);
                    break;
            }
        }
    }
}