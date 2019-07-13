using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Lang
{
    using static TokenKind;
    using static TokenMod;

    public unsafe partial class Ion
    {
        private static PtrBuffer* keywords;


        private static bool inited;

        private readonly byte[] char_to_digit = new byte[256];
        private readonly char[] escape_to_char = new char[256];

        private readonly Dictionary<TokenKind, string> tokenKindNames = new Dictionary<TokenKind, string>
        {
            [TOKEN_EOF] = string.Intern("EOF"),
            [TOKEN_COLON] = string.Intern(":"),
            [TOKEN_LPAREN] = string.Intern("("),
            [TOKEN_RPAREN] = string.Intern(")"),
            [TOKEN_LBRACE] = string.Intern("{"),
            [TOKEN_RBRACE] = string.Intern("}"),
            [TOKEN_LBRACKET] = string.Intern("["),
            [TOKEN_RBRACKET] = string.Intern("]"),
            [TOKEN_COMMA] = string.Intern("),"),
            [TOKEN_DOT] = string.Intern("."),
            [TOKEN_QUESTION] = string.Intern("?"),
            [TOKEN_SEMICOLON] = string.Intern(";"),
            [TOKEN_NEG] = string.Intern("~"),
            [TOKEN_NOT] = string.Intern("!"),
            [TOKEN_KEYWORD] = string.Intern("keyword"),
            [TOKEN_INT] = string.Intern("int"),
            [TOKEN_FLOAT] = string.Intern("float"),
            [TOKEN_STR] = string.Intern("string"),
            [TOKEN_NAME] = string.Intern("name"),
            [TOKEN_MUL] = string.Intern("*"),
            [TOKEN_DIV] = string.Intern("/"),
            [TOKEN_MOD] = string.Intern("%"),
            [TOKEN_AND] = string.Intern("&"),
            [TOKEN_LSHIFT] = string.Intern("<<"),
            [TOKEN_RSHIFT] = string.Intern(">>"),
            [TOKEN_ADD] = string.Intern("+"),
            [TOKEN_SUB] = string.Intern("-"),
            [TOKEN_OR] = string.Intern("|"),
            [TOKEN_XOR] = string.Intern("^"),
            [TOKEN_EQ] = string.Intern("=="),
            [TOKEN_NOTEQ] = string.Intern("!="),
            [TOKEN_LT] = string.Intern("<"),
            [TOKEN_GT] = string.Intern(">"),
            [TOKEN_LTEQ] = string.Intern("<="),
            [TOKEN_GTEQ] = string.Intern(">="),
            [TOKEN_AND_AND] = string.Intern("&&"),
            [TOKEN_OR_OR] = string.Intern("||"),
            [TOKEN_ASSIGN] = string.Intern("="),
            [TOKEN_ADD_ASSIGN] = string.Intern("+="),
            [TOKEN_SUB_ASSIGN] = string.Intern("-="),
            [TOKEN_OR_ASSIGN] = string.Intern("|="),
            [TOKEN_AND_ASSIGN] = string.Intern("&="),
            [TOKEN_XOR_ASSIGN] = string.Intern("^="),
            [TOKEN_MUL_ASSIGN] = string.Intern("*="),
            [TOKEN_DIV_ASSIGN] = string.Intern("/="),
            [TOKEN_MOD_ASSIGN] = string.Intern("%="),
            [TOKEN_LSHIFT_ASSIGN] = string.Intern("<<="),
            [TOKEN_RSHIFT_ASSIGN] = string.Intern(">>="),
            [TOKEN_INC] = string.Intern("++"),
            [TOKEN_DEC] = string.Intern("--"),
            [TOKEN_COLON_ASSIGN] = string.Intern(":=")
        };

        private char* break_keyword;
        private char* case_keyword;
        private char* const_keyword;
        private char* continue_keyword;
        private char* default_keyword;
        private char* do_keyword;
        private char* else_keyword;
        private char* enum_keyword;

        private char* first_keyword;
        private char* for_keyword;
        private char* func_keyword;
        private char* if_keyword;
        private char* last_keyword;
        private char* line_start;
        private char* return_keyword;
        private char* sizeof_keyword;

        private Buffer<char> str_buf = Buffer<char>.Create(256);
        private char* stream;
        private char* struct_keyword;
        private char* switch_keyword;

        private Token token;

        private char** token_kind_names;
        private char* typedef_keyword;
        private char* union_keyword;
        private char* var_keyword;
        private char* while_keyword;

        public void lex_init()
        {
            if (inited)
                return;
            keywords = PtrBuffer.Create();
            fixed (char*** clPtr = &token_kind_names)
            {
                tokenKindNames.ToCharArrayPointer(clPtr);
            }

            init_keywords();

            char_to_escape['\n'] = 'n';
            char_to_escape['\\'] = '\\';
            char_to_escape['"'] = '"';
            char_to_escape['\''] = '\'';

            char_to_digit['0'] = 0;
            char_to_digit['1'] = 1;
            char_to_digit['2'] = 2;
            char_to_digit['3'] = 3;
            char_to_digit['4'] = 4;
            char_to_digit['5'] = 5;
            char_to_digit['6'] = 6;
            char_to_digit['7'] = 7;
            char_to_digit['8'] = 8;
            char_to_digit['9'] = 9;
            char_to_digit['a'] = 10;
            char_to_digit['b'] = 11;
            char_to_digit['c'] = 12;
            char_to_digit['d'] = 13;
            char_to_digit['e'] = 14;
            char_to_digit['f'] = 15;
            char_to_digit['A'] = 10;
            char_to_digit['B'] = 11;
            char_to_digit['C'] = 12;
            char_to_digit['D'] = 13;
            char_to_digit['E'] = 14;
            char_to_digit['F'] = 15;

            escape_to_char['n'] = '\n';
            escape_to_char['r'] = '\r';
            escape_to_char['t'] = '\t';
            escape_to_char['v'] = '\v';
            escape_to_char['b'] = '\b';
            escape_to_char['a'] = '\a';
            escape_to_char['0'] = '\0';

            inited = true;
        }

        private void init_keywords()
        {
            typedef_keyword = _I("typedef");
            keywords->Add(typedef_keyword);

            var arena_end = intern_arena->end;

            enum_keyword = _I("enum");
            keywords->Add(enum_keyword);

            struct_keyword = _I("struct");
            keywords->Add(struct_keyword);

            union_keyword = _I("union");
            keywords->Add(union_keyword);

            var_keyword = _I("var");
            keywords->Add(var_keyword);

            const_keyword = _I("const");
            keywords->Add(const_keyword);

            func_keyword = _I("func");
            keywords->Add(func_keyword);

            sizeof_keyword = _I("sizeof");
            keywords->Add(sizeof_keyword);

            break_keyword = _I("break");
            keywords->Add(break_keyword);

            continue_keyword = _I("continue");
            keywords->Add(continue_keyword);

            return_keyword = _I("return");
            keywords->Add(return_keyword);

            if_keyword = _I("if");
            keywords->Add(if_keyword);

            else_keyword = _I("else");
            keywords->Add(else_keyword);

            while_keyword = _I("while");
            keywords->Add(while_keyword);

            do_keyword = _I("do");
            keywords->Add(do_keyword);

            for_keyword = _I("for");
            keywords->Add(for_keyword);

            switch_keyword = _I("switch");
            keywords->Add(switch_keyword);

            case_keyword = _I("case");
            keywords->Add(case_keyword);

            default_keyword = _I("default");
            keywords->Add(default_keyword);

            assert(intern_arena->end == arena_end);

            first_keyword = typedef_keyword;
            last_keyword = default_keyword;
        }

        private bool is_keyword_name(char* name)
        {
            return first_keyword <= name && name <= last_keyword;
        }

        private string token_kind_name(TokenKind kind)
        {
            if (kind < TOKEN_SIZE)
                return tokenKindNames[kind];
            return "<unknown>";
        }

        private char* _token_kind_name(TokenKind kind)
        {
            if (kind < TOKEN_SIZE)
                return *(token_kind_names + (long) kind);
            return "<unknown>".ToPtr();
        }

        private string token_info()
        {
            if (token.kind == TOKEN_NAME || token.kind == TOKEN_KEYWORD)
                return new string(token.name);

            return token_kind_name(token.kind);
        }

        private void scan_int()
        {
            long @base = 10;
            if (*stream == '0')
            {
                stream++;
                if (char.ToLower(*stream) == 'x')
                {
                    stream++;
                    token.mod = TOKENMOD_HEX;
                    @base = 16;
                }
                else if (char.ToLower(*stream) == 'b')
                {
                    stream++;
                    token.mod = TOKENMOD_BIN;
                    @base = 2;
                }
                else if (char.IsDigit(*stream))
                {
                    token.mod = TOKENMOD_OCT;
                    @base = 8;
                }
            }

            long val = 0;
            for (;;)
            {
                long digit = char_to_digit[*stream];
                if (digit == 0 && *stream != '0') break;

                if (digit >= @base)
                {
                    syntax_error("Digit '{0}' out of range for base {1}", *stream, @base);
                    digit = 0;
                }

                if (val > (long.MaxValue - digit) / @base)
                {
                    syntax_error("Integer literal overflow");
                    while (char.IsDigit(*stream)) stream++;

                    val = 0;
                    break;
                }

                val = val * @base + digit;
                stream++;
            }

            token.kind = TOKEN_INT;
            token.int_val = val;
        }

        private void scan_float()
        {
            var start = stream;
            while (char.IsDigit(*stream)) stream++;

            if (*stream == '.') stream++;

            while (char.IsDigit(*stream)) stream++;

            if (char.ToLower(*stream) == 'e')
            {
                stream++;
                if (*stream == '+' || *stream == '-') stream++;

                if (!char.IsDigit(*stream))
                    syntax_error("Expected digit after float literal exponent, found '%c'.", *stream);

                while (char.IsDigit(*stream)) stream++;
            }

            var val = double.Parse(new string(start, 0, (int) (stream - start)));
            if (double.IsPositiveInfinity(val)) syntax_error("Float literal overflow");

            token.kind = TOKEN_FLOAT;
            token.float_val = val;
        }

        private void scan_char()
        {
            assert(*stream == '\'');
            stream++;
            var val = '\0';
            if (*stream == '\'')
            {
                syntax_error("Char literal cannot be empty");
                stream++;
            }
            else if (*stream == '\n')
            {
                syntax_error("Char literal cannot contain newline");
            }
            else if (*stream == '\\')
            {
                stream++;
                val = escape_to_char[*stream];
                if (val == 0 && *stream != '0') syntax_error("Invalid char literal escape '\\%c'", *stream);

                stream++;
            }
            else
            {
                val = *stream;
                stream++;
            }

            if (*stream != '\'')
                syntax_error("Expected closing char quote, got '%c'", *stream);
            else
                stream++;

            token.kind = TOKEN_INT;
            token.int_val = val;
            token.mod = TOKENMOD_CHAR;
        }

        private void scan_str()
        {
            assert(*stream == '"');
            stream++;
            var start = stream;
            str_buf.clear();
            while (*stream != 0 && *stream != '"')
            {
                var val = *stream;
                if (val == '\n')
                {
                    syntax_error("String literal cannot contain newline");
                    break;
                }

                if (val == '\\')
                {
                    stream++;
                    val = escape_to_char[*stream];
                    if (val == 0 && *stream != '0') syntax_error("Invalid string literal escape '\\%c'", *stream);
                }

                str_buf.Add(val);
                stream++;
            }

            if (*stream != 0)
            {
                assert(*stream == '"');
                stream++;
            }
            else
            {
                syntax_error("Unexpected end of file within string literal");
            }

            str_buf.Add('\0');

            token.kind = TOKEN_STR;
            token.str_val = str_buf._begin;
        }

        private void next_token()
        {
            repeat:
            token.start = stream;
            token.mod = 0;
            switch (*stream)
            {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                case '\v':
                    while (char.IsWhiteSpace(*stream))
                        if (*stream++ == '\n')
                        {
                            line_start = stream;
                            token.pos.line++;
                        }

                    goto repeat;
                case '\'':
                    scan_char();
                    break;
                case '"':
                    scan_str();
                    break;
                case '.':
                    if (char.IsDigit(stream[1]))
                    {
                        scan_float();
                    }
                    else
                    {
                        token.kind = TOKEN_DOT;
                        stream++;
                    }

                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    while (char.IsDigit(*stream)) stream++;

                    var c = *stream;
                    stream = token.start;
                    if (c == '.' || char.ToLower(c) == 'e')
                        scan_float();
                    else
                        scan_int();

                    break;

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                    while (char.IsLetterOrDigit(*stream) || *stream == '_') stream++;

                    token.name = Intern.InternRange(token.start, stream);
                    token.kind = is_keyword_name(token.name) ? TOKEN_KEYWORD : TOKEN_NAME;
                    break;
                case '<':
                    stream++;
                    if (*stream == '<')
                    {
                        token.kind = TOKEN_LSHIFT;
                        stream++;
                        if (*stream == '=')
                        {
                            token.kind = TOKEN_LSHIFT_ASSIGN;
                            stream++;
                        }
                    }
                    else if (*stream == '=')
                    {
                        token.kind = TOKEN_LTEQ;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_LT;
                    }

                    break;
                case '>':
                    stream++;
                    if (*stream == '>')
                    {
                        token.kind = TOKEN_RSHIFT;
                        stream++;
                        if (*stream == '=')
                        {
                            token.kind = TOKEN_RSHIFT_ASSIGN;
                            stream++;
                        }
                    }
                    else if (*stream == '=')
                    {
                        token.kind = TOKEN_GTEQ;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_GT;
                    }

                    break;
                case '\0':
                    token.kind = TOKEN_EOF;
                    stream++;
                    break;
                case '(':
                    token.kind = TOKEN_LPAREN;
                    stream++;
                    break;
                case ')':
                    token.kind = TOKEN_RPAREN;
                    stream++;
                    break;
                case '{':
                    token.kind = TOKEN_LBRACE;
                    stream++;
                    break;
                case '}':
                    token.kind = TOKEN_RBRACE;
                    stream++;
                    break;
                case '[':
                    token.kind = TOKEN_LBRACKET;
                    stream++;
                    break;
                case ']':
                    token.kind = TOKEN_RBRACKET;
                    stream++;
                    break;
                case ',':
                    token.kind = TOKEN_COMMA;
                    stream++;
                    break;
                case '?':
                    token.kind = TOKEN_QUESTION;
                    stream++;
                    break;
                case ';':
                    token.kind = TOKEN_SEMICOLON;
                    stream++;
                    break;
                case '~':
                    token.kind = TOKEN_NEG;
                    stream++;
                    break;
                case '!':
                    token.kind = TOKEN_NOT;
                    stream++;
                    break;


                // CASE2
                case ':':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_COLON_ASSIGN;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_COLON;
                    }

                    break;
                case '=':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_EQ;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_ASSIGN;
                    }

                    break;
                case '^':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_XOR_ASSIGN;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_XOR;
                    }

                    break;
                case '*':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_MUL_ASSIGN;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_MUL;
                    }

                    break;
                case '/':
                    if (*++stream == '=')
                    {
                        token.kind = TOKEN_DIV_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '/')
                    {
                        stream++;
                        while (*stream != 0 && *stream != '\n') stream++;
                        goto repeat;
                    }
                    else
                    {
                        token.kind = TOKEN_DIV;
                    }

                    break;
                case '%':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_MOD_ASSIGN;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_MOD;
                    }

                    break;

                // CASE3 Types
                case '+':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_ADD_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '+')
                    {
                        token.kind = TOKEN_INC;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_ADD;
                    }

                    break;
                case '-':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_SUB_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '-')
                    {
                        token.kind = TOKEN_DEC;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_SUB;
                    }

                    break;
                case '&':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_AND_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '&')
                    {
                        token.kind = TOKEN_AND_AND;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_AND;
                    }

                    break;
                case '|':
                    stream++;
                    if (*stream == '=')
                    {
                        token.kind = TOKEN_OR_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '|')
                    {
                        token.kind = TOKEN_OR_OR;
                        stream++;
                    }
                    else
                    {
                        token.kind = TOKEN_OR;
                    }

                    break;
                default:
                    syntax_error("Invalid '{0}' token, skipping", *stream);
                    stream++;
                    goto repeat;
            }

            token.end = stream;
        }

        private void init_stream(string buf, string name = "<anonymous>")
        {
            init_stream(buf.ToPtr(), $"\"{name}\"".ToPtr2());
        }

        private void init_stream(char* str, char* name = null)
        {
            token.pos.name = name != null ? name : "<string>".ToPtr2();
            token.pos.line = 1;
            stream = str;
            next_token();
        }

        private bool is_token(TokenKind kind)
        {
            return token.kind == kind;
        }


        private bool is_token_eof()
        {
            return token.kind == TOKEN_EOF;
        }

        private bool is_token_name(char* name)
        {
            return token.kind == TOKEN_NAME && token.name == name;
        }

        private bool is_keyword(char* name)
        {
            return is_token(TOKEN_KEYWORD) && token.name == name;
        }

        private bool match_keyword(char* name)
        {
            if (is_keyword(name))
            {
                next_token();
                return true;
            }

            return false;
        }

        private bool match_token(TokenKind kind)
        {
            if (is_token(kind))
            {
                next_token();
                return true;
            }

            return false;
        }

        private bool expect_token(TokenKind kind)
        {
            if (is_token(kind))
            {
                next_token();
                return true;
            }

            fatal_syntax_error("expected token {0}, got {1}", tokenKindNames[kind], token_info());
            return false;
        }

        private void keyword_test()
        {
            lex_init();
            assert(is_keyword_name(first_keyword));
            assert(is_keyword_name(last_keyword));
            for (var it = (char**) keywords->_begin; it != keywords->_top; it++) assert(is_keyword_name(*it));

            assert(!is_keyword_name(_I("foo")));
        }

        private void assert_token(TokenKind x)
        {
            assert(match_token(x));
        }

        private void assert_token_name(string x)
        {
            assert(token.name == _I(x) && match_token(TOKEN_NAME));
        }

        private void assert_token_int(ulong x)
        {
            assert((ulong) token.int_val == x && match_token(TOKEN_INT));
        }

        private void assert_token_float(double x)
        {
            assert(token.float_val == x && match_token(TOKEN_FLOAT));
        }

        private void assert_token_str(char* x)
        {
            assert(strcmp(token.str_val, x) == 0 && match_token(TOKEN_STR));
        }

        private void assert_token_eof()
        {
            assert(is_token(0));
        }

        public void lex_test()
        {
            keyword_test();

            // Integer literal tests
            init_stream("0 18446744073709551615 0xffffffffffffffff 042 0b1111");
            assert_token_int(0);
            assert_token_int(18446744073709551615L);
            assert(token.mod == TOKENMOD_HEX);
            assert_token_int(0xffffffffffffffffL);
            assert(token.mod == TOKENMOD_OCT);
            assert_token_int(34);
            assert(token.mod == TOKENMOD_BIN);
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


        [StructLayout(LayoutKind.Explicit)]
        private struct Token
        {
            [FieldOffset(0)] public long int_val;
            [FieldOffset(0)] public double float_val;
            [FieldOffset(0)] public char* str_val;
            [FieldOffset(0)] public char* name;

            [FieldOffset(8)] public TokenKind kind;
            [FieldOffset(9)] public TokenMod mod;
            [FieldOffset(10)] public char* start;
            [FieldOffset(12 + PTR_SIZE)] public char* end;
            [FieldOffset(12 + 2 * PTR_SIZE)] public SrcPos pos;
        }
    }

    internal enum TokenKind : byte
    {
        TOKEN_EOF,
        TOKEN_COLON,
        TOKEN_LPAREN,
        TOKEN_RPAREN,
        TOKEN_LBRACE,
        TOKEN_RBRACE,
        TOKEN_LBRACKET,
        TOKEN_RBRACKET,
        TOKEN_COMMA,
        TOKEN_DOT,
        TOKEN_QUESTION,
        TOKEN_SEMICOLON,
        TOKEN_KEYWORD,
        TOKEN_INT,
        TOKEN_FLOAT,
        TOKEN_STR,
        TOKEN_NAME,
        TOKEN_NEG,
        TOKEN_NOT,

        // Multiplicative precedence
        TOKEN_FIRST_MUL,
        TOKEN_MUL = TOKEN_FIRST_MUL,
        TOKEN_DIV,
        TOKEN_MOD,
        TOKEN_AND,
        TOKEN_LSHIFT,
        TOKEN_RSHIFT,
        TOKEN_LAST_MUL = TOKEN_RSHIFT,

        // Additive precedence
        TOKEN_FIRST_ADD,
        TOKEN_ADD = TOKEN_FIRST_ADD,
        TOKEN_SUB,
        TOKEN_XOR,
        TOKEN_OR,
        TOKEN_LAST_ADD = TOKEN_OR,

        // Comparative precedence
        TOKEN_FIRST_CMP,
        TOKEN_EQ = TOKEN_FIRST_CMP,
        TOKEN_NOTEQ,
        TOKEN_LT,
        TOKEN_GT,
        TOKEN_LTEQ,
        TOKEN_GTEQ,
        TOKEN_LAST_CMP = TOKEN_GTEQ,
        TOKEN_AND_AND,
        TOKEN_OR_OR,

        // Assignment operators
        TOKEN_FIRST_ASSIGN,
        TOKEN_ASSIGN = TOKEN_FIRST_ASSIGN,
        TOKEN_ADD_ASSIGN,
        TOKEN_SUB_ASSIGN,
        TOKEN_OR_ASSIGN,
        TOKEN_AND_ASSIGN,
        TOKEN_XOR_ASSIGN,
        TOKEN_LSHIFT_ASSIGN,
        TOKEN_RSHIFT_ASSIGN,
        TOKEN_MUL_ASSIGN,
        TOKEN_DIV_ASSIGN,
        TOKEN_MOD_ASSIGN,
        TOKEN_LAST_ASSIGN = TOKEN_MOD_ASSIGN,
        TOKEN_INC,
        TOKEN_DEC,
        TOKEN_COLON_ASSIGN,
        TOKEN_SIZE
    }

    internal enum TokenMod : byte
    {
        TOKENMOD_NONE,
        TOKENMOD_HEX,
        TOKENMOD_BIN,
        TOKENMOD_OCT,
        TOKENMOD_CHAR
    }
}