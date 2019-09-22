using System;
using System.Collections.Generic;
using System.Text;

namespace IonLang
{
    using static TokenKind;
    using static TokenMod;
    using static TokenSuffix;

    unsafe partial class Ion
    {
        void main_test() {
            lex_init();
            var tests = new Action[] {resolve_test, common_test, lex_test, print_test, parse_test, ion_test };
            foreach(var test in tests) {
                Console.Write($"\nPress a key to preform '{test.Method.Name}', Press Space to skip");
                if (Console.ReadKey().KeyChar != ' ')
                    test();
            }
            Console.Write($"All Tests Passed. Press a key to exit");
        }

        #region Common Tests

        void intern_test() {
            char *a = "hello".ToPtr();
            assert(strcmp(a, _I(a)) == 0);
            assert(_I(a) == _I(a));
            assert(_I(_I(a)) == _I(a));
            char *b = "hello".ToPtr();
            assert(a != b);
            assert(_I(a) == _I(b));
            char *c = "hello!".ToPtr();
            assert(_I(a) != _I(c));
            char *d = "hell".ToPtr();
            assert(_I(a) != _I(d));
        }

        void buf_test() {
            var buf = Buffer<int>.Create();
            assert(buf.count == 0);
            int n = 1024;
            for (int i = 0; i < n; i++) {
                buf.Add(i);
            }
            assert(buf.count == n);
            for (var i = 0; i < buf.count; i++) {
                assert(*(buf._begin + i) == i);
            }
            buf.free();
            assert(buf._begin == null);
            assert(buf.count == 0);
        }

        void map_test() {
            Map map = default;
            const int  N = 1024;
            for (int i = 1; i < N; i++) {
                map.map_put((void*)i, (void*)(i + 1));
            }
            for (int i = 1; i < N; i++) {
                int *val = map.map_get<int>((int*)i);
                assert(val == (int*)(i + 1));
            }
        }

        void common_test() {
            buf_test();
            intern_test();
            map_test();
        }
        #endregion

        #region Lexer Tests
        private void keyword_test() {
            lex_init();
            assert(is_keyword_name(first_keyword));
            assert(is_keyword_name(last_keyword));
            for (var it = (char**)keywords->_begin; it != keywords->_top; it++)
                assert(is_keyword_name(*it));

            assert(!is_keyword_name(_I("foo")));
        }

        private void assert_token(TokenKind x) {
            assert(match_token(x));
        }

        private void assert_token_name(string x) {
            assert(token.name == _I(x) && match_token(TOKEN_NAME));
        }

        private void assert_token_int(ulong x) {
            assert((ulong)token.int_val == x && match_token(TOKEN_INT));
        }

        private void assert_token_float(double x) {
            assert(token.float_val == x && match_token(TOKEN_FLOAT));
        }

        private void assert_token_str(char* x) {
            assert(strcmp(token.str_val, x) == 0 && match_token(TOKEN_STR));
        }

        private void assert_token_eof() {
            assert(is_token(0));
        }

        public void lex_test() {
            keyword_test();

            // Integer literal tests
            init_stream("0 18446744073709551615 0xffffffffffffffff 042 0b1111");
            assert_token_int(0);
            assert_token_int(18446744073709551615L);
            assert(token.mod == MOD_HEX);
            assert_token_int(0xffffffffffffffffL);
            assert(token.mod == MOD_OCT);
            assert_token_int(34);
            assert(token.mod == MOD_BIN);
            assert_token_int(0b1111);
            assert_token_eof();

            // Float literal tests
            init_stream("3.14 .123 42. 3e10");
            assert_token_float(3.14);
            assert_token_float(.123);
            assert_token_float(42.0);
            assert_token_float(3e10);
            assert_token_eof();

            // Char literal tests
            init_stream("'a' '\\n'");
            assert_token_int('a');
            assert_token_int('\n');
            assert_token_eof();

            // String literal tests
            init_stream("\"foo\" \"a\\nb\"");
            assert_token_str("foo".ToPtr());
            assert_token_str("a\nb".ToPtr());
            assert_token_eof();

            // Operator tests
            init_stream(": := + += ++ < <= << <<=");
            assert_token(TOKEN_COLON);
            assert_token(TOKEN_COLON_ASSIGN);
            assert_token(TOKEN_ADD);
            assert_token(TOKEN_ADD_ASSIGN);
            assert_token(TOKEN_INC);
            assert_token(TOKEN_LT);
            assert_token(TOKEN_LTEQ);
            assert_token(TOKEN_LSHIFT);
            assert_token(TOKEN_LSHIFT_ASSIGN);
            assert_token_eof();

            // Misc tests
            init_stream("XY+(XY)_HELLO1,234+994");
            assert_token_name("XY");
            assert_token(TOKEN_ADD);
            assert_token(TOKEN_LPAREN);
            assert_token_name("XY");
            assert_token(TOKEN_RPAREN);
            assert_token_name("_HELLO1");
            assert_token(TOKEN_COMMA);
            assert_token_int(234);
            assert_token(TOKEN_ADD);
            assert_token_int(994);
            assert_token_eof();
        }

        #endregion

        #region Parser Tests

        private static readonly string[] decls =
        {
       "var x: char[256] = {1, 2, 3, ['a'] = 4}",
        "struct Vector { x, y: float; }",
        "var v = Vector{x = 1.0, y = -1.0};",
        "var v: Vector = {1.0, -1.0};",
        "const n = sizeof(:int*[16]);",
        "const n = sizeof(1+2);",
        "var x = b == 1 ? 1+2 : 3-4;",
        "func fact(n: int): int { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }",
        "func fact(n: int): int { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }",
        "var foo = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0;",
        "func f(x: int): bool { switch (x) { case 0: case 1: return true; case 2: default: return false; } }",
        "enum Color { RED = 3, GREEN, BLUE = 0 }",
        "const pi = 3.14;",
        "union IntOrFloat { i: int; f: float; }",
        "typedef Vectors = Vector[1+2];",
        "func f() { do { print(42); } while(1); }",
        "typedef T = (func(int):int)[16];",
        //"func f() { enum E { A, B, C } return; }",
        "func f() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }",
        };


        internal void parse_test() {
            Console.WriteLine();
            var ds = parse_file();
            for (var i = 0; i < decls.Length; i++) {
                var it = decls[i].ToPtr();
                init_stream(it);
                var decl = parse_decl();
                print_decl(decl);
            }

        }

        #endregion

        #region Resolve Test

        private readonly string[] code =
        {
            "var u2 = (:int*)42;",
            "union IntOrPtr { i: int; p: int*; }",
            "var u1 = IntOrPtr{i = 42};",
            "var u2 = IntOrPtr{p = (:int*)42}",
            "var i: int;",
            "struct Vector { x, y: int; }",
            "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }",
            "func f2(n: int): int { return 2*n; }",
            "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }",
            "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }",
            "func f5(x: int): int { switch(x) { case 0, 1: return 42; case 3: default: return -1; } }",
            "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }",
            "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }",
        };



        private void cdecl_test() {
            var c = 'a';
            init_builtins();
            type_to_cdecl(type_int, &c);
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_ptr(type_int), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_array(type_int, 10), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_func(new[] { type_int }, 1, type_int), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_array(type_func(new[] { type_int }, 1, type_int), 10), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_func(new[] { type_ptr(type_int) }, 1, type_int), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            var type1 = type_func(new[] {type_array(type_int, 10)}, 1, type_int);
            type_to_cdecl(type1, &c);
            Console.WriteLine(new string(cdecl_buffer) + ";");
            cdecl_buffer[_pos] = (char)0;
            _pos = 0;
            c++;
            type_to_cdecl(type_func((Type**)null, 0, type1), &c);
            c++;
            cdecl_buffer[_pos] = (char)0;
            Console.WriteLine(new string(cdecl_buffer) + ";");
            _pos = 0;
            type_to_cdecl(type_func((Type**)null, 0, type_array(type_func((Type**)null, 0, type_int), 10)), &c);
            _pos = 0;
        }

        private void resolve_test() {
            init_builtins();
            assert(promote_type(type_char) == type_int);
            assert(promote_type(type_schar) == type_int);
            assert(promote_type(type_uchar) == type_int);
            assert(promote_type(type_short) == type_int);
            assert(promote_type(type_ushort) == type_int);
            assert(promote_type(type_int) == type_int);
            assert(promote_type(type_uint) == type_uint);
            assert(promote_type(type_long) == type_long);
            assert(promote_type(type_ulong) == type_ulong);
            assert(promote_type(type_llong) == type_llong);
            assert(promote_type(type_ullong) == type_ullong);

            assert(unify_arithmetic_types(type_char, type_char) == type_int);
            assert(unify_arithmetic_types(type_char, type_ushort) == type_int);
            assert(unify_arithmetic_types(type_int, type_uint) == type_uint);
            assert(unify_arithmetic_types(type_int, type_long) == type_long);
            assert(unify_arithmetic_types(type_ulong, type_long) == type_ulong);
            assert(unify_arithmetic_types(type_long, type_uint) == type_ulong);
            assert(unify_arithmetic_types(type_llong, type_ulong) == type_llong);

            assert(convert_const(type_int, type_char, new Val { c = (char)100 }).i == 100);
            assert(convert_const(type_uint, type_int, new Val { i = -1 }).u == uint.MaxValue);
            assert(convert_const(type_uint, type_ullong, new Val { ull = ulong.MaxValue }).u == uint.MaxValue);
            assert(convert_const(type_int, type_schar, new Val { sc = -1 }).i == -1);

            type_int->align = type_float->align = type_int->size = type_float->size = 4;
            type_void->size = 0;
            type_char->size = type_char->align = 2;

            var int_ptr = type_ptr(type_int);
            assert(type_ptr(type_int) == int_ptr);
            var float_ptr = type_ptr(type_float);
            assert(type_ptr(type_float) == float_ptr);
            assert(int_ptr != float_ptr);
            var int_ptr_ptr = type_ptr(type_ptr(type_int));
            assert(type_ptr(type_ptr(type_int)) == int_ptr_ptr);
            var float4_array = type_array(type_float, 4);
            assert(type_array(type_float, 4) == float4_array);
            var float3_array = type_array(type_float, 3);
            assert(type_array(type_float, 3) == float3_array);
            assert(float4_array != float3_array);
            fixed (Type** t = &type_int) {
                var int_int_func = type_func(t, 1, type_int);
                assert(type_func(t, 1, type_int) == int_int_func);

                var int_func = type_func((Type**) null, 0, type_int);
                assert(int_int_func != int_func);
                assert(int_func == type_func((Type**)null, 0, type_int));
            }

            for (var i = 0; i < code.Length; i++) {
                init_stream(code[i].ToPtr(), null);
                var decl = parse_decl();
                sym_global_decl(decl);
            }

            finalize_syms();
            Console.WriteLine();
            for (var sym = (Sym**)sorted_syms->_begin; sym != sorted_syms->_top; sym++) {
                if ((*sym)->decl != null)
                    print_decl((*sym)->decl);
                else
                    printf("{0}", (*sym)->name);

                printf("\n");
            }

            Console.WriteLine();
        }
        #endregion

        #region Print Tests

        void** CreateArray(params void*[] arr) {
            var buf = PtrBuffer.GetPooledBuffer();
            foreach (var v in arr)
                buf->Add(v);

            return buf->_begin;
        }
        internal void print_test() {
            use_print_buf = true;
            // Expressions
            Expr*[] exprs = {
                expr_binary(default, TOKEN_ADD, expr_int(default, 1, 0, 0), expr_int(default, 2, 0, 0)),
                expr_unary(default, TOKEN_SUB, expr_float(default, 3.14, 0)),
                expr_ternary(default, expr_name(default, "flag".ToPtr()), expr_str(default, "true".ToPtr(), 0), expr_str(default, "false".ToPtr(), 0)),
                expr_field(default, expr_name(default, "person".ToPtr()), "name".ToPtr()),
                expr_call(default, expr_name(default, "fact".ToPtr()), (Expr**) CreateArray(expr_int(default, 42, 0, 0)), 1),
                expr_index(default, expr_field(default, expr_name(default, "person".ToPtr()), "siblings".ToPtr()), expr_int(default, 3, 0, 0)),
                expr_cast(default, typespec_ptr(default, typespec_name(default, "int".ToPtr())), expr_name(default, "void_ptr".ToPtr())),
            };
            foreach (Expr* it in exprs) {
                print_expr(it);
                printf("\n\n");
            }

            printf("\n\n");
            var elif = new ElseIf {
                cond = expr_name(default, "flag2".ToPtr()),
                block = new StmtList {
                    stmts = (Stmt**)
                        CreateArray(stmt_return(default, expr_int(default, 2, 0, 0))
                        ),
                    num_stmts = 1,
                }
            };

            // Statements
            Stmt*[] stmts = {
                stmt_return(default, expr_int(default, 42, 0, 0)),
                stmt_break(default),
                stmt_continue(default),
                stmt_block(
                    default,
                    new StmtList {
                        stmts = (Stmt**)
                            CreateArray(
                                stmt_break(default),
                                stmt_continue(default)
                            ),
                        num_stmts = 2,
                    }
                ),
                stmt_expr(default, expr_call(default, expr_name(default, "print".ToPtr()), (Expr**) CreateArray(expr_int(default, 1, 0, 0), expr_int(default, 2, 0, 0)), 2)),
                stmt_init(default, "x".ToPtr(), null, expr_int(default, 42, 0, 0)),
                stmt_if(default,
                    expr_name(default, "flag1".ToPtr()),
                    new StmtList {
                        stmts = (Stmt**)
                            CreateArray(
                                stmt_return(default, expr_int(default, 1, 0, 0))
                            ),
                        num_stmts = 1,
                    },
                    (ElseIf**)CreateArray(&elif),
                    1,
                    new StmtList {
                        stmts = (Stmt**)
                            CreateArray(stmt_return(default, expr_int(default, 3, 0, 0))
                            ),
                        num_stmts = 1,
                    }
                ),
                stmt_while(default, 
                    expr_name(default, "running".ToPtr()),
                    new StmtList {
                        stmts = (Stmt**)
                            CreateArray(stmt_assign(default, TOKEN_ADD_ASSIGN, expr_name(default, "i".ToPtr()), expr_int(default, 16, 0, 0))),
                        num_stmts = 1,
                    }
                ),
                stmt_switch(default,
                    expr_name(default, "val".ToPtr()),
                    (SwitchCase*) new[] {
                        new SwitchCase {
                            exprs = (Expr**) CreateArray(expr_int(default, 3, 0, 0), expr_int(default, 4, 0, 0)),
                            num_exprs = 2,
                            is_default = false,
                            block = new StmtList {
                                stmts = (Stmt**)
                                    CreateArray(stmt_return(default, expr_name(default, "val".ToPtr()))),
                                num_stmts = 1,
                            },
                        },
                        new SwitchCase {
                            exprs = (Expr**) CreateArray(expr_int(default, 1, 0, 0)),
                            num_exprs = 1,
                            is_default = true,
                            block = new StmtList {
                                stmts = (Stmt**)
                                    CreateArray(stmt_return(default, expr_int(default, 0, 0, 0))),
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
            flush_print_buf(new System.IO.StreamWriter(Console.OpenStandardOutput()));
            use_print_buf = false;
        }
        #endregion
    }
}
