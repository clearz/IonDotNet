﻿using System;

namespace IonLang
{
    using static TokenKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static CompoundFieldKind;

    public unsafe partial class Ion {
        internal Decls* parse_decls() {
            var buf = PtrBuffer.GetPooledBuffer();
            try {
                while (!is_token(TOKEN_EOF)) {
                    buf->Add(parse_decl());
                }

                return new_decls((Decl**)buf->_begin, buf->count);
            }
            finally {
                buf->Release();
            }
        }

        private Typespec* parse_type_func() {
            var buf = PtrBuffer.GetPooledBuffer();
            var pos = token.pos;
            bool has_varargs = false;
            try {
                expect_token(TOKEN_LPAREN);
                if (!is_token(TOKEN_RPAREN)) {
                    buf->Add(parse_type_func_param());
                    while (match_token(TOKEN_COMMA)) {
                        if (match_token(TOKEN_ELLIPSIS)) {
                            if (has_varargs) {
                                error_here("Multiple ellipsis instances in function type");
                            }
                            has_varargs = true;
                        }
                        else {
                            if (has_varargs) {
                                error_here("Ellipsis must be last parameter in function type");
                            }
                            buf->Add(parse_type_func_param());
                        }
                    }
                }

                expect_token(TOKEN_RPAREN);
                Typespec* ret = null;
                if (match_token(TOKEN_COLON))
                    ret = parse_type();

                return new_typespec_func(pos, (Typespec**)buf->_begin, buf->count, ret, has_varargs);
            }
            finally {
                buf->Release();
            }
        }

        private Typespec* parse_type_base() {
            var pos = token.pos;
            if (is_token(TOKEN_NAME)) {
                var name = token.name;
                next_token();
                return new_typespec_name(pos, name);
            }

            if (match_keyword(func_keyword))
                return parse_type_func();

            if (match_token(TOKEN_LPAREN)) {
                var type = parse_type();
                expect_token(TOKEN_RPAREN);
                return type;
            }

            fatal_error_here("Unexpected token {0} in type", token_info());
            return null;
        }

        private Typespec* parse_type() {
            var pos = token.pos;
            var type = parse_type_base();
            while (is_token(TOKEN_LBRACKET) || is_token(TOKEN_MUL) || is_keyword(const_keyword))
                if (match_token(TOKEN_LBRACKET)) {
                    Expr* expr = null;
                    if (!is_token(TOKEN_RBRACKET))
                        expr = parse_expr();

                    expect_token(TOKEN_RBRACKET);
                    type = typespec_array(pos, type, expr);

                }
                else if (match_keyword(const_keyword)) {
                    type = new_typespec_const(pos, type);
                }
                else {
                    assert(is_token(TOKEN_MUL));
                    next_token();
                    type = new_typespec_ptr(pos, type);
                }

            return type;
        }

        Typespec* parse_type_func_param() {
            Typespec *type = parse_type();
            if (match_token(TOKEN_COLON)) {
                if (type->kind != TypespecKind.TYPESPEC_NAME) {
                    error_here("Colons in parameters of func types must be preceded by names.");
                }
                type = parse_type();
            }
            return type;
        }

        private CompoundField parse_expr_compound_field() {
            var pos = token.pos;
            if (match_token(TOKEN_LBRACKET)) {
                var index = parse_expr();
                expect_token(TOKEN_RBRACKET);
                expect_token(TOKEN_ASSIGN);
                return new CompoundField { pos = pos, kind = FIELD_INDEX, init = parse_expr(), index = index };
            }

            var expr = parse_expr();
            if (match_token(TOKEN_ASSIGN)) {
                if (expr->kind != EXPR_NAME)
                    fatal_error_here("Named initializer in compound literal must be preceded by field name");
                return new CompoundField { pos = pos, kind = FIELD_NAME, init = parse_expr(), name = expr->name };
            }

            return new CompoundField { pos = pos, kind = FIELD_DEFAULT, init = expr };
        }

        private Expr* parse_expr_compound(Typespec* type) {
            var pos = token.pos;
            expect_token(TOKEN_LBRACE);
            var buf = Buffer<CompoundField>.Create();
            ; // Expr**
            while (!is_token(TOKEN_RBRACE)) {
                buf.Add(parse_expr_compound_field());
                if (!match_token(TOKEN_COMMA)) {
                    break;
                }
            }

            expect_token(TOKEN_RBRACE);
            return new_expr_compound(pos, type, buf, buf.count);
        }

        private Expr* parse_expr_operand() {
            var pos = token.pos;
            if (is_token(TOKEN_INT)) {
                var val = token.int_val;
                var suffix = token.suffix;
                var mod = token.mod;
                next_token();
                return new_expr_int(pos, val, mod, suffix);
            }

            if (is_token(TOKEN_FLOAT)) {
                double val = token.float_val;
                TokenSuffix suffix = token.suffix;
                next_token();
                return new_expr_float(pos, val, suffix);
            }

            if (is_token(TOKEN_STR)) {
                var mod = token.mod;
                var val = token.str_val;
                next_token();
                return new_expr_str(pos, val, mod);
            }

            if (is_token(TOKEN_NAME)) {
                var name = token.name;
                next_token();
                if (is_token(TOKEN_LBRACE))
                    return parse_expr_compound(new_typespec_name(pos, name));
                return new_expr_name(pos, name);
            }


            if (match_keyword(sizeof_keyword)) {
                expect_token(TOKEN_LPAREN);
                if (match_token(TOKEN_COLON)) {
                    var type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    return new_expr_sizeof_type(pos, type);
                }

                var expr = parse_expr();
                expect_token(TOKEN_RPAREN);
                return new_expr_sizeof_expr(pos, expr);
            }

            if (is_token(TOKEN_LBRACE))
                return parse_expr_compound(null);

            if (match_token(TOKEN_LPAREN)) {
                if (match_token(TOKEN_COLON)) {
                    var type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    if (is_token(TOKEN_LBRACE))
                        return parse_expr_compound(type);
                    return new_expr_cast(pos, type, parse_expr_unary());
                }

                var expr = parse_expr();
                expect_token(TOKEN_RPAREN);
                return expr;
            }

            fatal_error_here("Unexpected token {0} in expression", token_info());
            return null;
        }

        private Expr* parse_expr_base() {
            var pos = token.pos;
            var expr = parse_expr_operand();
            while (is_token(TOKEN_LPAREN) || is_token(TOKEN_LBRACKET) || is_token(TOKEN_DOT))
                if (match_token(TOKEN_LPAREN)) {
                    var buf = PtrBuffer.GetPooledBuffer();

                    try {
                        if (!is_token(TOKEN_RPAREN)) {
                            buf->Add(parse_expr());
                            while (match_token(TOKEN_COMMA))
                                buf->Add(parse_expr());
                        }

                        expect_token(TOKEN_RPAREN);
                        expr = new_expr_call(pos, expr, (Expr**)buf->_begin, buf->count);
                    }
                    finally {
                        buf->Release();
                    }
                }
                else if (match_token(TOKEN_LBRACKET)) {
                    var index = parse_expr();
                    expect_token(TOKEN_RBRACKET);
                    expr = new_expr_index(pos, expr, index);
                }
                else {
                    assert(is_token(TOKEN_DOT));
                    next_token();
                    var field = token.name;
                    expect_token(TOKEN_NAME);
                    expr = new_expr_field(pos, expr, field);
                }

            return expr;
        }

        private bool is_unary_op() {
            return is_token(TOKEN_ADD) || is_token(TOKEN_SUB) || is_token(TOKEN_MUL) || is_token(TOKEN_AND) ||
                   is_token(TOKEN_NEG) || is_token(TOKEN_NOT);
        }

        private Expr* parse_expr_unary() {
            var pos = token.pos;
            if (is_unary_op()) {
                var op = token.kind;
                next_token();
                return new_expr_unary(pos, op, parse_expr_unary());
            }

            return parse_expr_base();
        }

        private bool is_mul_op() {
            return TOKEN_FIRST_MUL <= token.kind && token.kind <= TOKEN_LAST_MUL;
        }

        private Expr* parse_expr_mul() {
            var pos = token.pos;
            var expr = parse_expr_unary();
            while (is_mul_op()) {
                var op = token.kind;
                next_token();
                expr = new_expr_binary(pos, op, expr, parse_expr_unary());
            }

            return expr;
        }

        private bool is_add_op() {
            return TOKEN_FIRST_ADD <= token.kind && token.kind <= TOKEN_LAST_ADD;
        }

        private Expr* parse_expr_add() {
            var pos = token.pos;
            var expr = parse_expr_mul();
            while (is_add_op()) {
                var op = token.kind;
                next_token();
                expr = new_expr_binary(pos, op, expr, parse_expr_mul());
            }

            return expr;
        }

        private bool is_cmp_op() {
            return TOKEN_FIRST_CMP <= token.kind && token.kind <= TOKEN_LAST_CMP;
        }

        private Expr* parse_expr_cmp() {
            var pos = token.pos;
            var expr = parse_expr_add();
            while (is_cmp_op()) {
                var op = token.kind;
                next_token();
                expr = new_expr_binary(pos, op, expr, parse_expr_add());
            }

            return expr;
        }

        private Expr* parse_expr_and() {
            var pos = token.pos;
            var expr = parse_expr_cmp();
            while (match_token(TOKEN_AND_AND))
                expr = new_expr_binary(pos, TOKEN_AND_AND, expr, parse_expr_cmp());

            return expr;
        }

        private Expr* parse_expr_or() {
            var pos = token.pos;
            var expr = parse_expr_and();
            while (match_token(TOKEN_OR_OR))
                expr = new_expr_binary(pos, TOKEN_OR_OR, expr, parse_expr_and());

            return expr;
        }

        private Expr* parse_expr_ternary() {
            var pos = token.pos;
            var expr = parse_expr_or();
            if (match_token(TOKEN_QUESTION)) {
                var then_expr = parse_expr_ternary();
                expect_token(TOKEN_COLON);
                var else_expr = parse_expr_ternary();
                expr = new_expr_ternary(pos, expr, then_expr, else_expr);
            }

            return expr;
        }

        private Expr* parse_expr() {
            return parse_expr_ternary();
        }

        private Expr* parse_paren_expr() {
            expect_token(TOKEN_LPAREN);
            var expr = parse_expr();
            expect_token(TOKEN_RPAREN);
            return expr;
        }

        private StmtList parse_stmt_block() {
            expect_token(TOKEN_LBRACE);
            var pos = token.pos;

            var buf = PtrBuffer.GetPooledBuffer();
            try {
                while (!is_token_eof() && !is_token(TOKEN_RBRACE))
                    buf->Add(parse_stmt());

                expect_token(TOKEN_RBRACE);
                return stmt_list(pos, (Stmt**)buf->_begin, buf->count);
            }
            finally {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_if(SrcPos pos) {
            var cond = parse_paren_expr();
            var then_block = parse_stmt_block();
            var else_block = default(StmtList);

            var buf = PtrBuffer.GetPooledBuffer();
            try {
                while (match_keyword(else_keyword)) {
                    if (!match_keyword(if_keyword)) {
                        else_block = parse_stmt_block();
                        break;
                    }

                    var elseif_cond = parse_paren_expr();
                    var elseif_block = parse_stmt_block();
                    var elif = (ElseIf*) xmalloc(sizeof(ElseIf));
                    elif->cond = elseif_cond;
                    elif->block = elseif_block;
                    buf->Add(elif);
                }

                return new_stmt_if(pos, cond, then_block, (ElseIf**)buf->_begin, buf->count,
                    else_block);
            }
            finally {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_while(SrcPos pos) {
            var cond = parse_paren_expr();
            return new_stmt_while(pos, cond, parse_stmt_block());
        }

        private Stmt* parse_stmt_do_while(SrcPos pos) {
            var block = parse_stmt_block();
            if (!match_keyword(while_keyword)) {
                fatal_error_here("Expected 'while' after 'do' block");
                return null;
            }

            var stmt = new_stmt_do_while(pos, parse_paren_expr(), block);
            expect_token(TOKEN_SEMICOLON);
            return stmt;
        }

        private bool is_assign_op() {
            return TOKEN_FIRST_ASSIGN <= token.kind && token.kind <= TOKEN_LAST_ASSIGN;
        }

        private Stmt* parse_simple_stmt() {
            var pos = token.pos;
            var expr = parse_expr();
            Stmt* stmt;
            if (match_token(TOKEN_COLON_ASSIGN)) {
                if (expr->kind != EXPR_NAME) {
                    fatal_error_here(":= must be preceded by a name");
                    return null;
                }

                stmt = new_stmt_init(pos, expr->name, null, parse_expr());
            }
            else if (match_token(TOKEN_COLON)) {
                if (expr->kind != EXPR_NAME) {
                    fatal_error_here(": must be preceded by a name");
                    return null;
                }
                char *name = expr->name;
                Typespec *type = parse_type();
                if (match_token(TOKEN_ASSIGN)) {
                    expr = parse_expr();
                }
                else
                    expr = null;

                stmt = new_stmt_init(pos, name, type, expr);
            }
            else if (is_assign_op()) {
                var op = token.kind;
                next_token();
                stmt = new_stmt_assign(pos, op, expr, parse_expr());
            }
            else if (is_token(TOKEN_INC) || is_token(TOKEN_DEC)) {
                var op = token.kind;
                next_token();
                stmt = new_stmt_assign(pos, op, expr, null);
            }
            else {
                stmt = new_stmt_expr(pos, expr);
            }

            return stmt;
        }

        private Stmt* parse_stmt_for(SrcPos pos) {
            expect_token(TOKEN_LPAREN);
            Stmt* init = null;
            if (!is_token(TOKEN_SEMICOLON))
                init = parse_simple_stmt();

            expect_token(TOKEN_SEMICOLON);
            Expr* cond = null;
            if (!is_token(TOKEN_SEMICOLON))
                cond = parse_expr();

            expect_token(TOKEN_SEMICOLON);
            Stmt* next = null;
            if (!is_token(TOKEN_RPAREN)) {
                next = parse_simple_stmt();
                if (next->kind == STMT_INIT)
                    error_here("Init statements not allowed in for-statement's next clause");
            }

            expect_token(TOKEN_RPAREN);
            return new_stmt_for(pos, init, cond, next, parse_stmt_block());
        }

        private SwitchCase parse_stmt_switch_case() {
            var buf = PtrBuffer.GetPooledBuffer();
            try {
                var is_default = false;
                while (is_keyword(case_keyword) || is_keyword(default_keyword)) {
                    if (match_keyword(case_keyword)) {
                        buf->Add(parse_expr());
                        while (match_token(TOKEN_COMMA)) {
                            buf->Add(parse_expr());
                        }
                    }
                    else {
                        assert(is_keyword(default_keyword));
                        next_token();
                        if (is_default)
                            error_here("Duplicate default labels in same switch clause");

                        is_default = true;
                    }

                    expect_token(TOKEN_COLON);
                }

                var pos = token.pos;

                var buf2 = PtrBuffer.GetPooledBuffer();
                ;
                while (!is_token_eof() && !is_token(TOKEN_RBRACE) && !is_keyword(case_keyword) &&
                       !is_keyword(default_keyword))
                    buf2->Add(parse_stmt());

                var block = new StmtList
                {
                    pos = pos,
                    stmts = (Stmt**) buf2->_begin,
                    num_stmts = buf2->count
                };
                return new SwitchCase {
                    exprs = (Expr**)ast_dup(buf->_begin, buf->buf_byte_size),
                    num_exprs = buf->count,
                    is_default = is_default,
                    block = block
                };
            }
            finally {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_switch(SrcPos pos) {
            var expr = parse_paren_expr();

            var buf = Buffer<SwitchCase>.Create();
            expect_token(TOKEN_LBRACE);
            while (!is_token_eof() && !is_token(TOKEN_RBRACE))
                buf.Add(parse_stmt_switch_case());

            expect_token(TOKEN_RBRACE);
            return new_stmt_switch(pos, expr, buf, buf.count);
        }

        private Stmt* parse_stmt() {
            var pos = token.pos;
            if (match_keyword(if_keyword))
                return parse_stmt_if(pos);

            else if (match_keyword(while_keyword))
                return parse_stmt_while(pos);

            else if (match_keyword(do_keyword))
                return parse_stmt_do_while(pos);

            else if (match_keyword(for_keyword))
                return parse_stmt_for(pos);

            else if (match_keyword(switch_keyword))
                return parse_stmt_switch(pos);

            else if(is_token(TOKEN_LBRACE))
                return new_stmt_block(pos, parse_stmt_block());

            else if(match_keyword(break_keyword)) {
                expect_token(TOKEN_SEMICOLON);
                return new_stmt_break(pos);
            }

            else if(match_keyword(continue_keyword)) {
                expect_token(TOKEN_SEMICOLON);
                return new_stmt_continue(pos);
            }

            else if(match_keyword(return_keyword)) {
                Expr* expr = null;
                if (!is_token(TOKEN_SEMICOLON))
                    expr = parse_expr();

                expect_token(TOKEN_SEMICOLON);
                return new_stmt_return(pos, expr);
            }
            else {
                Stmt *stmt = parse_simple_stmt();
                expect_token(TOKEN_SEMICOLON);
                return stmt;
            }
        }

        private char* parse_name() {
            var name = token.name;
            expect_token(TOKEN_NAME);
            return name;
        }

        private EnumItem parse_decl_enum_item() {
            var pos = token.pos;
            var name = parse_name();
            Expr* init = null;
            if (match_token(TOKEN_ASSIGN))
                init = parse_expr();

            return new EnumItem { pos = pos, name = name, init = init };
        }

        private Decl* parse_decl_enum(SrcPos pos) {
            var name = parse_name();
            expect_token(TOKEN_LBRACE);
            EnumItem* items = null;
            var buf = Buffer<EnumItem>.Create();
            while (!is_token(TOKEN_RBRACE)) {
                buf.Add(parse_decl_enum_item());
                if (!match_token(TOKEN_COMMA))
                    break;
            }

            expect_token(TOKEN_RBRACE);
            return new_decl_enum(pos, name, buf._begin, buf.count);
        }

        private AggregateItem parse_decl_aggregate_item() {
            var buf = PtrBuffer.GetPooledBuffer();
            var pos = token.pos;
            try {
                buf->Add(parse_name());
                while (match_token(TOKEN_COMMA))
                    buf->Add(parse_name());

                expect_token(TOKEN_COLON);
                var type = parse_type();
                expect_token(TOKEN_SEMICOLON);
                return new AggregateItem {
                    pos = pos,
                    names = (char**)ast_dup(buf->_begin, buf->count * sizeof(char*)),
                    num_names = buf->count,
                    type = type
                };
            }
            finally {
                buf->Release();
            }
        }

        private Decl* parse_decl_aggregate(SrcPos pos, DeclKind kind) {
            assert(kind == DECL_STRUCT || kind == DECL_UNION);
            var name = parse_name();
            expect_token(TOKEN_LBRACE);

            var buf = Buffer<AggregateItem>.Create();
            while (!is_token_eof() && !is_token(TOKEN_RBRACE))
                buf.Add(parse_decl_aggregate_item());

            expect_token(TOKEN_RBRACE);
            return new_decl_aggregate(pos, kind, name, buf, buf.count);
        }

        private Decl* parse_decl_var(SrcPos pos) {
            var name = parse_name();
            if (match_token(TOKEN_ASSIGN)) {
                Expr *expr = parse_expr();
                expect_token(TOKEN_SEMICOLON);
                return new_decl_var(pos, name, null, expr);
            }
            else if (match_token(TOKEN_COLON)) {
                Typespec *type = parse_type();
                Expr *expr = null;
                if (match_token(TOKEN_ASSIGN)) {
                    expr = parse_expr();
                }
                expect_token(TOKEN_SEMICOLON);
                return new_decl_var(pos, name, type, expr);
            }
            else { 
                fatal_error_here("Expected : or = after var, got {0}", token_info());
                return null;
            }
        }

        private Decl* parse_decl_const(SrcPos pos) {
            Typespec *type = null;
            var name = parse_name();
            if (match_token(TOKEN_COLON)) {
                type = parse_type();
            }
            expect_token(TOKEN_ASSIGN);
            Expr *expr = parse_expr();
            expect_token(TOKEN_SEMICOLON);
            return new_decl_const(pos, name, type, expr);
        }

        private Decl* parse_decl_typedef(SrcPos pos) {
            var name = parse_name();
            expect_token(TOKEN_ASSIGN);
            Typespec *type = parse_type();
            expect_token(TOKEN_SEMICOLON);
            return new_decl_typedef(pos, name, type);
        }

        private FuncParam parse_decl_func_param() {
            var pos = token.pos;
            var name = parse_name();
            expect_token(TOKEN_COLON);
            var type = parse_type();
            return new FuncParam { pos = pos, name = name, type = type };
        }

        private Decl* parse_decl_func(SrcPos pos) {
            var name = parse_name();
            bool has_varargs = false;
            expect_token(TOKEN_LPAREN);
            var buf = Buffer<FuncParam>.Create();
            if (!is_token(TOKEN_RPAREN)) {
                buf.Add(parse_decl_func_param());
                while (match_token(TOKEN_COMMA)) {
                    if (match_token(TOKEN_ELLIPSIS)) {
                        if (has_varargs) {
                            error_here("Multiple ellipsis in function declaration");
                        }
                        has_varargs = true;
                    }
                    else {
                        if (has_varargs) {
                            error_here("Ellipsis must be last parameter in function declaration");
                        }
                        buf.Add(parse_decl_func_param());
                    }
                }
            }

            expect_token(TOKEN_RPAREN);
            Typespec* ret_type = null;
            if (match_token(TOKEN_COLON))
                ret_type = parse_type();

            StmtList block = default;
            bool is_incomplete = false;
            if (is_token(TOKEN_LBRACE)) {
                block = parse_stmt_block();
            }
            else {
                expect_token(TOKEN_SEMICOLON);
                is_incomplete = true;
            }
            return new_decl_func(pos, name, buf, buf.count, ret_type, is_incomplete, has_varargs, block);
        }
        NoteArg parse_note_arg() {
            SrcPos pos = token.pos;
            Expr *expr = parse_expr();
            char *name = null;
            if (match_token(TOKEN_ASSIGN)) {
                if (expr->kind != EXPR_NAME) {
                    fatal_error_here("Left operand of = in note argument must be a name");
                }
                name = expr->name;
                expr = parse_expr();
            }
            return new NoteArg { pos = pos, name = name, expr = expr };
        }

        Note parse_note() {
            SrcPos pos = token.pos;
            char *name = parse_name();
            var args = Buffer<NoteArg>.Create();
            if (match_token(TOKEN_LPAREN)) {
                args.Add(parse_note_arg());
                while (match_token(TOKEN_COMMA)) {
                    args.Add(parse_note_arg());
                }
                expect_token(TOKEN_RPAREN);
            }
            return new_note(pos, name, args, args.count);
        }

        Notes parse_note_list() {
            var buf = Buffer<Note>.Create();
            while (match_token(TOKEN_AT)) {
                buf.Add(parse_note());
            }
            return new_note_list(buf, buf.count);
        }

        Decl* parse_decl_note(SrcPos pos) {
            return new_decl_note(pos, parse_note());
        }

        private Decl* parse_decl_opt() {
            var pos = token.pos;
            if (match_keyword(enum_keyword))
                return parse_decl_enum(pos);
            if (match_keyword(struct_keyword))
                return parse_decl_aggregate(pos, DECL_STRUCT);
            if (match_keyword(union_keyword))
                return parse_decl_aggregate(pos, DECL_UNION);
            if (match_keyword(var_keyword))
                return parse_decl_var(pos);
            if (match_keyword(const_keyword))
                return parse_decl_const(pos);
            if (match_keyword(typedef_keyword))
                return parse_decl_typedef(pos);
            if (match_keyword(func_keyword))
                return parse_decl_func(pos);
            if (match_token(TOKEN_POUND))
                return parse_decl_note(pos);
            return null;
        }

        private Decl* parse_decl() {
            Notes notes = parse_note_list();
            var decl = parse_decl_opt();
            if (decl == null)
                fatal_error_here("Expected declaration keyword, got {0}", token_info());

            decl->notes = notes;
            return decl;
        }
    }
}