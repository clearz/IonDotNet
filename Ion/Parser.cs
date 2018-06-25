using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lang
{
    using static TokenKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;

#if X64
    using size_t = Int64;
    using System.IO;
    using System.Diagnostics;
#else
    using size_t = Int32;
#endif
    public unsafe partial class Ion
    {
        Typespec* parse_type_func() {
            var buf = PtrBuffer.Create();;
            expect_token(TOKEN_LPAREN);
            if (!is_token(TOKEN_RPAREN)) {
                buf->Add(parse_type());
                while (match_token(TOKEN_COMMA)) {
                    buf->Add(parse_type());
                }
            }

            expect_token(TOKEN_RPAREN);
            Typespec* ret = null;
            if (match_token(TOKEN_COLON)) {
                ret = parse_type();
            }

            return typespec_func((Typespec**)buf->_begin , buf->count, ret); // ast_dup(buf->_begin, buf->count * PTR_SIZE)
        }

        Typespec* parse_type_base() {
            if (is_token(TOKEN_NAME)) {
                char* name = token.name;
                next_token();
                return typespec_name(name);
            }
            else if (match_keyword(func_keyword)) {
                return parse_type_func();
            }
            else if (match_token(TOKEN_LPAREN)) {
                Typespec* type = parse_type();
                expect_token(TOKEN_RPAREN);
                return type;
            }
            else {
                fatal_syntax_error("Unexpected token {0} in type", new String(token_info()));
                return null;
            }
        }

        Typespec* parse_type() {
            Typespec* type = parse_type_base();
            while (is_token(TOKEN_LBRACKET) || is_token(TOKEN_MUL)) {
                if (match_token(TOKEN_LBRACKET)) {
                    Expr* expr = null;
                    if (!is_token(TOKEN_RBRACKET)) {
                        expr = parse_expr();
                    }

                    expect_token(TOKEN_RBRACKET);
                    type = typespec_array(type, expr);
                }
                else {
                    assert(is_token(TOKEN_MUL));
                    next_token();
                    type = typespec_ptr(type);
                }
            }

            return type;
        }

        Expr* parse_expr_compound(Typespec* type) {
            expect_token(TOKEN_LBRACE);
            var buf = PtrBuffer.Create();; // Expr**
            if (!is_token(TOKEN_RBRACE)) {
                buf->Add(parse_expr());
                while (match_token(TOKEN_COMMA)) {
                    buf->Add(parse_expr());
                }
            }

            expect_token(TOKEN_RBRACE);
            return expr_compound(type, (Expr**) buf->_begin, buf->count);
        }

        Expr* parse_expr_operand() {
            if (is_token(TOKEN_INT)) {
                long val = token.int_val;
                next_token();
                return expr_int(val);
            }
            else if (is_token(TOKEN_FLOAT)) {
                double val = token.float_val;
                next_token();
                return expr_float(val);
            }
            else if (is_token(TOKEN_STR)) {
                char* val = token.str_val;
                next_token();
                return expr_str(val);
            }
            else if (is_token(TOKEN_NAME)) {
                char* name = token.name;
                next_token();
                if (is_token(TOKEN_LBRACE)) {
                    return parse_expr_compound(typespec_name(name));
                }
                else {
                    return expr_name(name);
                }
            }else if (match_keyword(cast_keyword)) {
            expect_token(TOKEN_LPAREN);
            Typespec* type = parse_type();
            expect_token(TOKEN_COMMA);
            Expr* expr = parse_expr();
            expect_token(TOKEN_RPAREN);
            return expr_cast(type, expr);
        }
        else if (match_keyword(sizeof_keyword)) {
                expect_token(TOKEN_LPAREN);
                if (match_token(TOKEN_COLON)) {
                    Typespec* type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    return expr_sizeof_type(type);
                }
                else {
                    Expr* expr = parse_expr();
                    expect_token(TOKEN_RPAREN);
                    return expr_sizeof_expr(expr);
                }
            }
            else if (is_token(TOKEN_LBRACE)) {
                return parse_expr_compound(null);
            }
            else if (match_token(TOKEN_LPAREN)) {
                if (match_token(TOKEN_COLON)) {
                    Typespec* type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    return parse_expr_compound(type);
                }
                else {
                    Expr* expr = parse_expr();
                    expect_token(TOKEN_RPAREN);
                    return expr;
                }
            }
            else {
                fatal_syntax_error("Unexpected token {0} in expression", new String(token_info()));
                return null;
            }
        }

        Expr* parse_expr_base() {
            Expr* expr = parse_expr_operand();
            while (is_token(TOKEN_LPAREN) || is_token(TOKEN_LBRACKET) || is_token(TOKEN_DOT)) {
                if (match_token(TOKEN_LPAREN)) {
                    var buf = PtrBuffer.Create();;
                    if (!is_token(TOKEN_RPAREN)) {
                        buf->Add(parse_expr());
                        while (match_token(TOKEN_COMMA)) {
                            buf->Add(parse_expr());
                        }
                    }

                    expect_token(TOKEN_RPAREN);
                    expr = expr_call(expr, (Expr**) buf->_begin, buf->count);
                }
                else if (match_token(TOKEN_LBRACKET)) {
                    Expr* index = parse_expr();
                    expect_token(TOKEN_RBRACKET);
                    expr = expr_index(expr, index);
                }
                else {
                    assert(is_token(TOKEN_DOT));
                    next_token();
                    char* field = token.name;
                    expect_token(TOKEN_NAME);
                    expr = expr_field(expr, field);
                }
            }

            return expr;
        }

        bool is_unary_op() {
            return is_token(TOKEN_ADD) || is_token(TOKEN_SUB) || is_token(TOKEN_MUL) || is_token(TOKEN_AND) || is_token(TOKEN_NEG) || is_token(TOKEN_NOT);
        }

        Expr* parse_expr_unary() {
            if (is_unary_op()) {
                TokenKind op = token.kind;
                next_token();
                return expr_unary(op, parse_expr_unary());
            }
            else {
                return parse_expr_base();
            }
        }

        bool is_mul_op() {
            return TOKEN_FIRST_MUL <= token.kind && token.kind <= TOKEN_LAST_MUL;
        }

        Expr* parse_expr_mul() {
            Expr* expr = parse_expr_unary();
            while (is_mul_op()) {
                TokenKind op = token.kind;
                next_token();
                expr = expr_binary(op, expr, parse_expr_unary());
            }

            return expr;
        }

        bool is_add_op() {
            return TOKEN_FIRST_ADD <= token.kind && token.kind <= TOKEN_LAST_ADD;
        }

        Expr* parse_expr_add() {
            Expr* expr = parse_expr_mul();
            while (is_add_op()) {
                TokenKind op = token.kind;
                next_token();
                expr = expr_binary(op, expr, parse_expr_mul());
            }

            return expr;
        }

        bool is_cmp_op() {
            return TOKEN_FIRST_CMP <= token.kind && token.kind <= TOKEN_LAST_CMP;
        }

        Expr* parse_expr_cmp() {
            Expr* expr = parse_expr_add();
            while (is_cmp_op()) {
                TokenKind op = token.kind;
                next_token();
                expr = expr_binary(op, expr, parse_expr_add());
            }

            return expr;
        }

        Expr* parse_expr_and() {
            Expr* expr = parse_expr_cmp();
            while (match_token(TOKEN_AND_AND)) {
                expr = expr_binary(TOKEN_AND_AND, expr, parse_expr_cmp());
            }

            return expr;
        }

        Expr* parse_expr_or() {
            Expr* expr = parse_expr_and();
            while (match_token(TOKEN_OR_OR)) {
                expr = expr_binary(TOKEN_OR_OR, expr, parse_expr_and());
            }

            return expr;
        }

        Expr* parse_expr_ternary() {
            Expr* expr = parse_expr_or();
            if (match_token(TOKEN_QUESTION)) {
                Expr* then_expr = parse_expr_ternary();
                expect_token(TOKEN_COLON);
                Expr* else_expr = parse_expr_ternary();
                expr = expr_ternary(expr, then_expr, else_expr);
            }

            return expr;
        }

        Expr* parse_expr() {
            return parse_expr_ternary();
        }

        Expr* parse_paren_expr() {
            expect_token(TOKEN_LPAREN);
            Expr* expr = parse_expr();
            expect_token(TOKEN_RPAREN);
            return expr;
        }

        StmtBlock parse_stmt_block() {
            expect_token(TOKEN_LBRACE);
            Stmt** stmts = null;
            var buf = PtrBuffer.Create();;
            while (!is_token_eof() && !is_token(TOKEN_RBRACE)) {
                buf->Add(parse_stmt());
            }

            expect_token(TOKEN_RBRACE);
            return stmt_list((Stmt**)buf->_begin, buf->count);
            
        }

        Stmt* parse_stmt_if() {
            Expr* cond = parse_paren_expr();
            StmtBlock then_block = parse_stmt_block();
            StmtBlock else_block = default(StmtBlock);
            ElseIf* elseifs = null;
            var buf = Buffer<ElseIf>.Create();
            while (match_keyword(else_keyword)) {
                if (!match_keyword(if_keyword)) {
                    else_block = parse_stmt_block();
                    break;
                }

                Expr* elseif_cond = parse_paren_expr();
                StmtBlock elseif_block = parse_stmt_block();
                //var elif = (ElseIf*) Marshal.AllocHGlobal(sizeof(ElseIf));
                //elif->cond = elseif_cond;
                //elif->block = elseif_block;
                buf.Add(new ElseIf {cond = elseif_cond, block = elseif_block});
            }

            return stmt_if(cond, then_block, (ElseIf*) buf._begin, buf.count,
                else_block);
        }

        Stmt* parse_stmt_while() {
            Expr* cond = parse_paren_expr();
            return stmt_while(cond, parse_stmt_block());
        }

        Stmt* parse_stmt_do_while() {
            StmtBlock block = parse_stmt_block();
            if (!match_keyword(while_keyword)) {
                fatal_syntax_error("Expected 'while' after 'do' block");
                return null;
            }

            Stmt* stmt = stmt_do_while(parse_paren_expr(), block);
            expect_token(TOKEN_SEMICOLON);
            return stmt;
        }

        bool is_assign_op() {
            return TOKEN_FIRST_ASSIGN <= token.kind && token.kind <= TOKEN_LAST_ASSIGN;
        }

        Stmt* parse_simple_stmt() {
            Expr* expr = parse_expr();
            Stmt* stmt;
            if (match_token(TOKEN_COLON_ASSIGN)) {
                if (expr->kind != EXPR_NAME) {
                    fatal_syntax_error(":= must be preceded by a name");
                    return null;
                }

                stmt = stmt_init(expr->name, parse_expr());
            }
            else if (is_assign_op()) {
                TokenKind op = token.kind;
                next_token();
                stmt = stmt_assign(op, expr, parse_expr());
            }
            else if (is_token(TOKEN_INC) || is_token(TOKEN_DEC)) {
                TokenKind op = token.kind;
                next_token();
                stmt = stmt_assign(op, expr, null);
            }
            else {
                stmt = stmt_expr(expr);
            }

            return stmt;
        }

        Stmt* parse_stmt_for() {
            expect_token(TOKEN_LPAREN);
            Stmt* init = null;
            if (!is_token(TOKEN_SEMICOLON)) {
                init = parse_simple_stmt();
            }

            expect_token(TOKEN_SEMICOLON);
            Expr* cond = null;
            if (!is_token(TOKEN_SEMICOLON)) {
                cond = parse_expr();
            }

            expect_token(TOKEN_SEMICOLON);
            Stmt* next = null;
            if (!is_token(TOKEN_RPAREN)) {
                next = parse_simple_stmt();
                if (next->kind == STMT_INIT) {
                    syntax_error("Init statements not allowed in for-statement's next clause");
                }
            }

            expect_token(TOKEN_RPAREN);
            return stmt_for(init, cond, next, parse_stmt_block());
        }

        SwitchCase parse_stmt_switch_case() {
            Expr** exprs = null;
            var buf = PtrBuffer.Create();;
            bool is_default = false;
            while (is_keyword(case_keyword) || is_keyword(default_keyword)) {
                if (match_keyword(case_keyword)) {
                    buf->Add(parse_expr());
                }
                else {
                    assert(is_keyword(default_keyword));
                    next_token();
                    if (is_default) {
                        syntax_error("Duplicate default labels in same switch clause");
                    }

                    is_default = true;
                }

                expect_token(TOKEN_COLON);
            }

            Stmt** stmts = null;
            var buf2 = PtrBuffer.Create();;
            while (!is_token_eof() && !is_token(TOKEN_RBRACE) && !is_keyword(case_keyword) &&
                   !is_keyword(default_keyword)) {
                buf2->Add(parse_stmt());
            }

            StmtBlock block = new StmtBlock {
                stmts = (Stmt**) buf2->_begin,
                num_stmts = buf2->count
            };
            return new SwitchCase {
                exprs = (Expr**) buf->_begin,
                num_exprs = buf->count,
                is_default = is_default,
                block = block
            };
        }

        Stmt* parse_stmt_switch() {
            Expr* expr = parse_paren_expr();
            SwitchCase* cases = null;
            var buf = Buffer<SwitchCase>.Create();
            expect_token(TOKEN_LBRACE);
            while (!is_token_eof() && !is_token(TOKEN_RBRACE)) {
                buf.Add(parse_stmt_switch_case());
            }

            expect_token(TOKEN_RBRACE);
            return stmt_switch(expr, (SwitchCase*) buf._begin, buf.count);
        }

        Stmt* parse_stmt() {
            if (match_keyword(if_keyword)) {
                return parse_stmt_if();
            }
            else if (match_keyword(while_keyword)) {
                return parse_stmt_while();
            }
            else if (match_keyword(do_keyword)) {
                return parse_stmt_do_while();
            }
            else if (match_keyword(for_keyword)) {
                return parse_stmt_for();
            }
            else if (match_keyword(switch_keyword)) {
                return parse_stmt_switch();
            }
            else if (is_token(TOKEN_LBRACE)) {
                return stmt_block(parse_stmt_block());
            }
            else if (match_keyword(break_keyword)) {
                expect_token(TOKEN_SEMICOLON);
                return stmt_break();
            }
            else if (match_keyword(continue_keyword)) {
                expect_token(TOKEN_SEMICOLON);
                return stmt_continue();
            }
            else if (match_keyword(return_keyword)) {
                Expr* expr = null;
                if (!is_token(TOKEN_SEMICOLON)) {
                    expr = parse_expr();
                }

                expect_token(TOKEN_SEMICOLON);
                return stmt_return(expr);
            }
            else {
                Decl* decl = parse_decl_opt();
                if (decl != null) {
                    return stmt_decl(decl);
                }

                Stmt* stmt = parse_simple_stmt();
                expect_token(TOKEN_SEMICOLON);
                return stmt;
            }
        }

        char* parse_name() {
            char* name = token.name;
            expect_token(TOKEN_NAME);
            return name;
        }

        EnumItem parse_decl_enum_item() {
            char* name = parse_name();
            Expr* init = null;
            if (match_token(TOKEN_ASSIGN)) {
                init = parse_expr();
            }

            return new EnumItem {name = name, init = init};
        }

        Decl* parse_decl_enum() {
            char* name = parse_name();
            expect_token(TOKEN_LBRACE);
            EnumItem* items = null;
            var buf = Buffer<EnumItem>.Create();
            if (!is_token(TOKEN_RBRACE)) {
                buf.Add(parse_decl_enum_item());
                while (match_token(TOKEN_COMMA)) {
                    buf.Add(parse_decl_enum_item());
                }
            }

            expect_token(TOKEN_RBRACE);
            return decl_enum(name, (EnumItem*) buf._begin, buf.count);
        }

        AggregateItem parse_decl_aggregate_item() {
            char** names = null;
            var buf = PtrBuffer.Create();;
            buf->Add(parse_name());
            while (match_token(TOKEN_COMMA)) {
                buf->Add(parse_name());
            }

            expect_token(TOKEN_COLON);
            Typespec* type = parse_type();
            expect_token(TOKEN_SEMICOLON);
            return new AggregateItem {
                names = (char**) buf->_begin,
                num_names = buf->count,
                type = type
            };
        }

        Decl* parse_decl_aggregate(DeclKind kind) {
            assert(kind == DECL_STRUCT || kind == DECL_UNION);
            char* name = parse_name();
            expect_token(TOKEN_LBRACE);
            AggregateItem* items = null;
            var buf = Buffer<AggregateItem>.Create();
            while (!is_token_eof() && !is_token(TOKEN_RBRACE)) {
                buf.Add(parse_decl_aggregate_item());
            }

            expect_token(TOKEN_RBRACE);
            return decl_aggregate(kind, name, (AggregateItem*) buf._begin, buf.count);
        }

        Decl* parse_decl_var() {
            char* name = parse_name();
            if (match_token(TOKEN_ASSIGN)) {
                return decl_var(name, null, parse_expr());
            }
            else if (match_token(TOKEN_COLON)) {
                Typespec* type = parse_type();
                Expr* expr = null;
                if (match_token(TOKEN_ASSIGN)) {
                    expr = parse_expr();
                }

                return decl_var(name, type, expr);
            }
            else {
                fatal_syntax_error("Expected : or = after var, got {0}", new String(token_info()));
                return null;
            }
        }

        Decl* parse_decl_const() {
            char* name = parse_name();
            expect_token(TOKEN_ASSIGN);
            return decl_const(name, parse_expr());
        }

        Decl* parse_decl_typedef() {
            char* name = parse_name();
            expect_token(TOKEN_ASSIGN);
            return decl_typedef(name, parse_type());
        }

        FuncParam parse_decl_func_param() {
            char* name = parse_name();
            expect_token(TOKEN_COLON);
            Typespec* type = parse_type();
            return new FuncParam {name = name, type = type};
        }

        Decl* parse_decl_func() {
            char* name = parse_name();
            expect_token(TOKEN_LPAREN);
            FuncParam* @params = null;
            var buf = Buffer<FuncParam>.Create();
            if (!is_token(TOKEN_RPAREN)) {
                buf.Add(parse_decl_func_param());
                while (match_token(TOKEN_COMMA)) {
                    buf.Add(parse_decl_func_param());
                }
            }

            expect_token(TOKEN_RPAREN);
            Typespec* ret_type = null;
            if (match_token(TOKEN_COLON)) {
                ret_type = parse_type();
            }

            StmtBlock block = parse_stmt_block();
            return decl_func(name, (FuncParam*) buf._begin, buf.count, ret_type, block);
        }

        Decl* parse_decl_opt() {
            if (match_keyword(enum_keyword)) {
                return parse_decl_enum();
            }
            else if (match_keyword(struct_keyword)) {
                return parse_decl_aggregate(DECL_STRUCT);
            }
            else if (match_keyword(union_keyword)) {
                return parse_decl_aggregate(DECL_UNION);
            }
            else if (match_keyword(var_keyword)) {
                return parse_decl_var();
            }
            else if (match_keyword(const_keyword)) {
                return parse_decl_const();
            }
            else if (match_keyword(typedef_keyword)) {
                return parse_decl_typedef();
            }
            else if (match_keyword(func_keyword)) {
                return parse_decl_func();
            }
            else {
                return null;
            }
        }

        Decl* parse_decl() {
            Decl* decl = parse_decl_opt();
            if (decl == null) {
                fatal_syntax_error("Expected declaration keyword, got {0}", new String(token_info()));
            }

            return decl;
        }

        static readonly string[] decls = {
            "const n = sizeof(:int*[16])",
            "const n = sizeof(1+2)",
            "var x = b == 1 ? 1+2 : 3-4",
            "func fact(n: int): int { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }",
            "func fact(n: int): int { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }",
            "var foo = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0",
            "func f(x: int): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }",
            "enum Color { RED = 3, GREEN, BLUE = 0 }",
            "const pi = 3.14",
            "struct Vector { x, y: float; }",
            "var v = Vector{1.0, -1.0}",
            "var v: Vector = {1.0, -1.0}",
            "union IntOrFloat { i: int; f: float; }",
            "typedef Vectors = Vector[1+2]",
            "func f() { do { print(42); } while(1); }",
            "typedef T = (func(int):int)[16]",
            "func f() { enum E { A, B, C } return; }",
            "func f() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }",
        };
        
        private char** _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void init_parse_test() {
            _ptr = (char**) Marshal.AllocHGlobal(PTR_SIZE * decls.Length);
            for (var i = 0; i < decls.Length; i++) {
                var it = decls[i].ToPtr();
                *(_ptr + i) = it;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void parse_test_and_print()
        {
            init_parse_test();
            Console.WriteLine();
            for (var i = 0; i < 18; i++)
            {
                var it = *(_ptr + i);
                init_stream(it);
                Decl* decl = parse_decl();
                print_decl(decl);
                Console.WriteLine();
            }

            //var txt = File.ReadAllText("parser.output.txt");
            //var txt2 = sb.ToString();
            //assert(txt == txt2);
            //use_print_buf = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void parse_test()
        {
            for (var i = 0; i < 18; i++)
            {
                var it = *(_ptr + i);
                init_stream(it);
                Decl* decl = parse_decl();
            }
        }
    }
}
