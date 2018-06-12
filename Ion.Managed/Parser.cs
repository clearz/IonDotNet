using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MLang
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private readonly Ast _ast;

        public Parser() {
            _lexer = new Lexer();
            _lexer.lex_init();
            _ast = new Ast();
        }
        Typespec parse_type_func()
        {
            var specs = new List<Typespec>();
            _lexer.expect_token(TokenKind.TOKEN_LPAREN);
            if (!_lexer.is_token(TokenKind.TOKEN_RPAREN)) {
                specs.Add(parse_type());
                while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                    specs.Add(parse_type());
                }
            }

            _lexer.expect_token(TokenKind.TOKEN_RPAREN);
            Typespec ret = null;
            if (_lexer.match_token(TokenKind.TOKEN_COLON)) {
                ret = parse_type();
            }

            return _ast.typespec_func(specs.ToArray(), specs.Count, ret);
        }

        Typespec parse_type_base() {
            if (_lexer.is_token(TokenKind.TOKEN_NAME)) {
                string name = _lexer.token.name;
                _lexer.next_token();
                return _ast.typespec_name(name);
            }
            else if (_lexer.match_keyword(Lexer.func_keyword)) {
                return parse_type_func();
            }
            else if (_lexer.match_token(TokenKind.TOKEN_LPAREN)) {
                Typespec type = parse_type();
                _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                return type;
            }
            else {
                Error.syntax_error("Unexpected token %s in type", _lexer.token_info());
                return null;
            }
        }

        Typespec parse_type() {
            Typespec type = parse_type_base();
            while (_lexer.is_token(TokenKind.TOKEN_LBRACKET) || _lexer.is_token(TokenKind.TOKEN_MUL)) {
                if (_lexer.match_token(TokenKind.TOKEN_LBRACKET)) {
                    Expr expr = null;
                    if (!_lexer.is_token(TokenKind.TOKEN_RBRACKET)) {
                        expr = parse_expr();
                    }

                    _lexer.expect_token(TokenKind.TOKEN_RBRACKET);
                    type = _ast.typespec_array(type, expr);
                }
                else {
                    Error.assert(_lexer.is_token(TokenKind.TOKEN_MUL));
                    _lexer.next_token();
                    type = _ast.typespec_ptr(type);
                }
            }

            return type;
        }

        Expr parse_expr_compound(Typespec type) {
            _lexer.expect_token(TokenKind.TOKEN_LBRACE);
            var exprs = new List<Expr>();
            if (!_lexer.is_token(TokenKind.TOKEN_RBRACE)) {
                exprs.Add(parse_expr());
                while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                    exprs.Add(parse_expr());
                }
            }

            _lexer.expect_token(TokenKind.TOKEN_RBRACE);
            return _ast.expr_compound(type, exprs.ToArray(), exprs.Count);
        }

        Expr parse_expr_operand() {
            if (_lexer.is_token(TokenKind.TOKEN_INT)) {
                ulong val = _lexer.token.int_val;
                _lexer.next_token();
                return _ast.expr_int(val);
            }
            else if (_lexer.is_token(TokenKind.TOKEN_FLOAT)) {
                double val = _lexer.token.float_val;
                _lexer.next_token();
                return _ast.expr_float(val);
            }
            else if (_lexer.is_token(TokenKind.TOKEN_STR)) {
                string val = _lexer.token.str_val;
                _lexer.next_token();
                return _ast.expr_str(val);
            }
            else if (_lexer.is_token(TokenKind.TOKEN_NAME)) {
                string name = _lexer.token.name;
                _lexer.next_token();
                if (_lexer.is_token(TokenKind.TOKEN_LBRACE)) {
                    return parse_expr_compound(_ast.typespec_name(name));
                }
                else {
                    return _ast.expr_name(name);
                }
            }
            else if (_lexer.match_keyword(Lexer.sizeof_keyword)) {
                _lexer.expect_token(TokenKind.TOKEN_LPAREN);
                if (_lexer.match_token(TokenKind.TOKEN_COLON)) {
                    Typespec type = parse_type();
                    _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                    return _ast.expr_sizeof_type(type);
                }
                else {
                    Expr expr = parse_expr();
                    _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                    return _ast.expr_sizeof_expr(expr);
                }
            }
            else if (_lexer.is_token(TokenKind.TOKEN_LBRACE)) {
                return parse_expr_compound(null);
            }
            else if (_lexer.match_token(TokenKind.TOKEN_LPAREN)) {
                if (_lexer.match_token(TokenKind.TOKEN_COLON)) {
                    Typespec type = parse_type();
                    _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                    return parse_expr_compound(type);
                }
                else {
                    Expr expr = parse_expr();
                    _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                    return expr;
                }
            }
            else {
                Error.syntax_error("Unexpected token %s in expression", _lexer.token_info());
                return null;
            }
        }

        Expr parse_expr_base() {
            Expr expr = parse_expr_operand();
            while (_lexer.is_token(TokenKind.TOKEN_LPAREN) || _lexer.is_token(TokenKind.TOKEN_LBRACKET) || _lexer.is_token(TokenKind.TOKEN_DOT)) {
                if (_lexer.match_token(TokenKind.TOKEN_LPAREN)) {
                    var exprs = new List<Expr>();
                    if (!_lexer.is_token(TokenKind.TOKEN_RPAREN)) {
                        exprs.Add(parse_expr());
                        while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                            exprs.Add(parse_expr());
                        }
                    }

                    _lexer.expect_token(TokenKind.TOKEN_RPAREN);
                    expr = _ast.expr_call(expr, exprs.ToArray(), exprs.Count);
                }
                else if (_lexer.match_token(TokenKind.TOKEN_LBRACKET)) {
                    Expr index = parse_expr();
                    _lexer.expect_token(TokenKind.TOKEN_RBRACKET);
                    expr = _ast.expr_index(expr, index);
                }
                else {
                    Error.assert(_lexer.is_token(TokenKind.TOKEN_DOT));
                    _lexer.next_token();
                    string field = _lexer.token.name;
                    _lexer.expect_token(TokenKind.TOKEN_NAME);
                    expr = _ast.expr_field(expr, field);
                }
            }

            return expr;
        }

        bool is_unary_op() {
            return _lexer.is_token(TokenKind.TOKEN_ADD) || _lexer.is_token(TokenKind.TOKEN_SUB) || _lexer.is_token(TokenKind.TOKEN_MUL) || _lexer.is_token(TokenKind.TOKEN_AND);
        }

        Expr parse_expr_unary() {
            if (is_unary_op()) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                return _ast.expr_unary(op, parse_expr_unary());
            }
            else {
                return parse_expr_base();
            }
        }

        bool is_mul_op() {
            return TokenKind.TOKEN_FIRST_MUL <= _lexer.token.kind && _lexer.token.kind <= TokenKind.TOKEN_LAST_MUL;
        }

        Expr parse_expr_mul() {
            Expr expr = parse_expr_unary();
            while (is_mul_op()) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                expr = _ast.expr_binary(op, expr, parse_expr_unary());
            }

            return expr;
        }

        bool is_add_op() {
            return TokenKind.TOKEN_FIRST_ADD <= _lexer.token.kind && _lexer.token.kind <= TokenKind.TOKEN_LAST_ADD;
        }

        Expr parse_expr_add() {
            Expr expr = parse_expr_mul();
            while (is_add_op()) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                expr = _ast.expr_binary(op, expr, parse_expr_mul());
            }

            return expr;
        }

        bool is_cmp_op() {
            return TokenKind.TOKEN_FIRST_CMP <= _lexer.token.kind && _lexer.token.kind <= TokenKind.TOKEN_LAST_CMP;
        }

        Expr parse_expr_cmp() {
            Expr expr = parse_expr_add();
            while (is_cmp_op()) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                expr = _ast.expr_binary(op, expr, parse_expr_add());
            }

            return expr;
        }

        Expr parse_expr_and() {
            Expr expr = parse_expr_cmp();
            while (_lexer.match_token(TokenKind.TOKEN_AND_AND)) {
                expr = _ast.expr_binary(TokenKind.TOKEN_AND_AND, expr, parse_expr_cmp());
            }

            return expr;
        }

        Expr parse_expr_or() {
            Expr expr = parse_expr_and();
            while (_lexer.match_token(TokenKind.TOKEN_OR_OR)) {
                expr = _ast.expr_binary(TokenKind.TOKEN_OR_OR, expr, parse_expr_and());
            }

            return expr;
        }

        Expr parse_expr_ternary() {
            Expr expr = parse_expr_or();
            if (_lexer.match_token(TokenKind.TOKEN_QUESTION)) {
                Expr then_expr = parse_expr_ternary();
                _lexer.expect_token(TokenKind.TOKEN_COLON);
                Expr else_expr = parse_expr_ternary();
                expr = _ast.expr_ternary(expr, then_expr, else_expr);
            }

            return expr;
        }

        Expr parse_expr() {
            return parse_expr_ternary();
        }

        Expr parse_paren_expr() {
            _lexer.expect_token(TokenKind.TOKEN_LPAREN);
            Expr expr = parse_expr();
            _lexer.expect_token(TokenKind.TOKEN_RPAREN);
            return expr;
        }

        StmtBlock parse_stmt_block() {
            _lexer.expect_token(TokenKind.TOKEN_LBRACE);
            var stmts = new List<Stmt>();
            while (!_lexer.is_token_eof() && !_lexer.is_token(TokenKind.TOKEN_RBRACE)) {
                stmts.Add(parse_stmt());
            }

            _lexer.expect_token(TokenKind.TOKEN_RBRACE);
            return new StmtBlock() {
                stmts = stmts.ToArray(),
                num_stmts = stmts.Count
            };
        }

        Stmt parse_stmt_if() {
            Expr cond = parse_paren_expr();
            StmtBlock then_block = parse_stmt_block();
            StmtBlock else_block = default(StmtBlock);
            var elseifs = new List<ElseIf>();
            while (_lexer.match_keyword(Lexer.else_keyword)) {
                if (!_lexer.match_keyword(Lexer.if_keyword)) {
                    else_block = parse_stmt_block();
                    break;
                }

                Expr elseif_cond = parse_paren_expr();
                StmtBlock elseif_block = parse_stmt_block();
                elseifs.Add(new ElseIf {cond = elseif_cond, block = elseif_block});
            }

            return _ast.stmt_if(cond, then_block, elseifs.ToArray(), elseifs.Count, else_block);
        }

        Stmt parse_stmt_while() {
            Expr cond = parse_paren_expr();
            return _ast.stmt_while(cond, parse_stmt_block());
        }

        Stmt parse_stmt_do_while() {
            StmtBlock block = parse_stmt_block();
            if (!_lexer.match_keyword(Lexer.while_keyword)) {
                Error.syntax_error("Expected 'while' after 'do' block");
                return null;
            }

            Stmt stmt = _ast.stmt_do_while(parse_paren_expr(), block);
            _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
            return stmt;
        }

        bool is_assign_op() {
            return TokenKind.TOKEN_FIRST_ASSIGN <= _lexer.token.kind && _lexer.token.kind <= TokenKind.TOKEN_LAST_ASSIGN;
        }

        Stmt parse_simple_stmt() {
            Expr expr = parse_expr();
            Stmt stmt;
            if (_lexer.match_token(TokenKind.TOKEN_COLON_ASSIGN)) {
                if (expr.kind != ExprKind.EXPR_NAME) {
                    Error.syntax_error(":= must be preceded by a name");
                    return null;
                }

                stmt = _ast.stmt_init(expr.name, parse_expr());
            }
            else if (is_assign_op()) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                stmt = _ast.stmt_assign(op, expr, parse_expr());
            }
            else if (_lexer.is_token(TokenKind.TOKEN_INC) || _lexer.is_token(TokenKind.TOKEN_DEC)) {
                TokenKind op = _lexer.token.kind;
                _lexer.next_token();
                stmt = _ast.stmt_assign(op, expr, null);
            }
            else {
                stmt = _ast.stmt_expr(expr);
            }

            return stmt;
        }

        Stmt parse_stmt_for() {
            _lexer.expect_token(TokenKind.TOKEN_LPAREN);
            Stmt init = null;
            if (!_lexer.is_token(TokenKind.TOKEN_SEMICOLON)) {
                init = parse_simple_stmt();
            }

            _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
            Expr cond = null;
            if (!_lexer.is_token(TokenKind.TOKEN_SEMICOLON)) {
                cond = parse_expr();
            }

            _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
            Stmt next = null;
            if (!_lexer.is_token(TokenKind.TOKEN_RPAREN)) {
                next = parse_simple_stmt();
                if (next.kind == StmtKind.STMT_INIT) {
                    Error.syntax_error("Init statements not allowed in for-statement's next clause");
                }
            }

            _lexer.expect_token(TokenKind.TOKEN_RPAREN);
            return _ast.stmt_for(init, cond, next, parse_stmt_block());
        }

        SwitchCase parse_stmt_switch_case() {
            var exprs = new List<Expr>();
            bool is_default = false;
            while (_lexer.is_keyword(Lexer.case_keyword) || _lexer.is_keyword(Lexer.default_keyword)) {
                if (_lexer.match_keyword(Lexer.case_keyword)) {
                    exprs.Add(parse_expr());
                }
                else {
                    Error.assert(_lexer.is_keyword(Lexer.default_keyword));
                    _lexer.next_token();
                    if (is_default) {
                        Error.syntax_error("Duplicate default labels in same switch clause");
                    }

                    is_default = true;
                }

                _lexer.expect_token(TokenKind.TOKEN_COLON);
            }

            var stmts = new List<Stmt>();
            while (!_lexer.is_token_eof() && !_lexer.is_token(TokenKind.TOKEN_RBRACE) && !_lexer.is_keyword(Lexer.case_keyword) &&
                   !_lexer.is_keyword(Lexer.default_keyword)) {
                stmts.Add(parse_stmt());
            }

            StmtBlock block = new StmtBlock {
                stmts = stmts.ToArray(),
                num_stmts = stmts.Count
            };
            return new SwitchCase {
                exprs = exprs.ToArray(),
                num_exprs = exprs.Count,
                is_default = is_default,
                block = block
            };
        }

        Stmt parse_stmt_switch() {
            Expr expr = parse_paren_expr();
            var cases = new List<SwitchCase>();
            _lexer.expect_token(TokenKind.TOKEN_LBRACE);
            while (!_lexer.is_token_eof() && !_lexer.is_token(TokenKind.TOKEN_RBRACE)) {
                cases.Add(parse_stmt_switch_case());
            }

            _lexer.expect_token(TokenKind.TOKEN_RBRACE);
            return _ast.stmt_switch(expr, cases.ToArray(), cases.Count);
        }

        Stmt parse_stmt() {
            if (_lexer.match_keyword(Lexer.if_keyword)) {
                return parse_stmt_if();
            }
            else if (_lexer.match_keyword(Lexer.while_keyword)) {
                return parse_stmt_while();
            }
            else if (_lexer.match_keyword(Lexer.do_keyword)) {
                return parse_stmt_do_while();
            }
            else if (_lexer.match_keyword(Lexer.for_keyword)) {
                return parse_stmt_for();
            }
            else if (_lexer.match_keyword(Lexer.switch_keyword)) {
                return parse_stmt_switch();
            }
            else if (_lexer.is_token(TokenKind.TOKEN_LBRACE)) {
                return _ast.stmt_block(parse_stmt_block());
            }
            else if (_lexer.match_keyword(Lexer.break_keyword)) {
                _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
                return _ast.stmt_break();
            }
            else if (_lexer.match_keyword(Lexer.continue_keyword)) {
                _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
                return _ast.stmt_continue();
            }
            else if (_lexer.match_keyword(Lexer.return_keyword)) {
                Expr expr = null;
                if (!_lexer.is_token(TokenKind.TOKEN_SEMICOLON)) {
                    expr = parse_expr();
                }

                _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
                return _ast.stmt_return(expr);
            }
            else {
                Decl decl = parse_decl_opt();
                if (decl != null) {
                    return _ast.stmt_decl(decl);
                }

                Stmt stmt = parse_simple_stmt();
                _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
                return stmt;
            }
        }

        string parse_name() {
            string name = _lexer.token.name;
            _lexer.expect_token(TokenKind.TOKEN_NAME);
            return name;
        }

        EnumItem parse_decl_enum_item() {
            string name = parse_name();
            Expr init = null;
            if (_lexer.match_token(TokenKind.TOKEN_ASSIGN)) {
                init = parse_expr();
            }

            return new EnumItem {name = name, init = init};
        }

        Decl parse_decl_enum() {
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_LBRACE);
            var items = new List<EnumItem>();
            if (!_lexer.is_token(TokenKind.TOKEN_RBRACE)) {
                items.Add(parse_decl_enum_item());
                while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                    items.Add(parse_decl_enum_item());
                }
            }

            _lexer.expect_token(TokenKind.TOKEN_RBRACE);
            return _ast.decl_enum(name, items.ToArray(), items.Count);
        }

        AggregateItem parse_decl_aggregate_item() {
            var names = new List<string>();
            names.Add(parse_name());
            while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                names.Add(parse_name());
            }

            _lexer.expect_token(TokenKind.TOKEN_COLON);
            Typespec type = parse_type();
            _lexer.expect_token(TokenKind.TOKEN_SEMICOLON);
            return new AggregateItem {
                names = names.ToArray(),
                num_names = names.Count,
                type = type
            };
        }

        Decl parse_decl_aggregate(DeclKind kind) {
            Error.assert(kind == DeclKind.DECL_STRUCT || kind == DeclKind.DECL_UNION);
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_LBRACE);
            var items = new List<AggregateItem>();
            while (!_lexer.is_token_eof() && !_lexer.is_token(TokenKind.TOKEN_RBRACE)) {
                items.Add(parse_decl_aggregate_item());
            }

            _lexer.expect_token(TokenKind.TOKEN_RBRACE);
            return _ast.decl_aggregate(kind, name, items.ToArray(), items.Count);
        }

        Decl parse_decl_var() {
            string name = parse_name();
            if (_lexer.match_token(TokenKind.TOKEN_ASSIGN)) {
                return _ast.decl_var(name, null, parse_expr());
            }
            else if (_lexer.match_token(TokenKind.TOKEN_COLON)) {
                Typespec type = parse_type();
                Expr expr = null;
                if (_lexer.match_token(TokenKind.TOKEN_ASSIGN)) {
                    expr = parse_expr();
                }

                return _ast.decl_var(name, type, expr);
            }
            else {
                Error.syntax_error("Expected : or = after var, got %s", _lexer.token_info());
                return null;
            }
        }

        Decl parse_decl_const() {
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_ASSIGN);
            return _ast.decl_const(name, parse_expr());
        }

        Decl parse_decl_typedef() {
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_ASSIGN);
            return _ast.decl_typedef(name, parse_type());
        }

        FuncParam parse_decl_func_param() {
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_COLON);
            Typespec type = parse_type();
            return new FuncParam {name = name, type = type};
        }

        Decl parse_decl_func() {
            string name = parse_name();
            _lexer.expect_token(TokenKind.TOKEN_LPAREN);
            var @params = new List<FuncParam>();
            if (!_lexer.is_token(TokenKind.TOKEN_RPAREN)) {
                @params.Add(parse_decl_func_param());
                while (_lexer.match_token(TokenKind.TOKEN_COMMA)) {
                    @params.Add(parse_decl_func_param());
                }
            }

            _lexer.expect_token(TokenKind.TOKEN_RPAREN);
            Typespec ret_type = null;
            if (_lexer.match_token(TokenKind.TOKEN_COLON)) {
                ret_type = parse_type();
            }

            StmtBlock block = parse_stmt_block();
            return _ast.decl_func(name, @params.ToArray(), @params.Count, ret_type, block);
        }

        Decl parse_decl_opt() {
            if (_lexer.match_keyword(Lexer.enum_keyword)) {
                return parse_decl_enum();
            }
            else if (_lexer.match_keyword(Lexer.struct_keyword)) {
                return parse_decl_aggregate(DeclKind.DECL_STRUCT);
            }
            else if (_lexer.match_keyword(Lexer.union_keyword)) {
                return parse_decl_aggregate(DeclKind.DECL_UNION);
            }
            else if (_lexer.match_keyword(Lexer.var_keyword)) {
                return parse_decl_var();
            }
            else if (_lexer.match_keyword(Lexer.const_keyword)) {
                return parse_decl_const();
            }
            else if (_lexer.match_keyword(Lexer.typedef_keyword)) {
                return parse_decl_typedef();
            }
            else if (_lexer.match_keyword(Lexer.func_keyword)) {
                return parse_decl_func();
            }
            else {
                return null;
            }
        }

        Decl parse_decl() {
            Decl decl = parse_decl_opt();
            if (decl == null) {
                Error.syntax_error("Expected declaration keyword, got %s", _lexer.token_info());
            }

            return decl;
        }

        static readonly string[] decls = {
            "func fact(): int { trace(\"fact\"); }",
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void parse_test() {
            var printer = new Print();
            foreach (var it in decls) {
                _lexer.init_stream(it);
                Decl decl = parse_decl();
                printer.print_decl(decl);
                Console.WriteLine();
            }
        }
    }
}
