using System.Runtime.InteropServices;

namespace IonLang
{
    using static TokenKind;
    using static TokenMod;
    using static TokenSuffix;

    public unsafe partial class Ion
    {
        static PtrBuffer* keywords;


        static bool inited;

        readonly byte[] char_to_digit = new byte[256];
        readonly char[] escape_to_char = new char[256];
        static SrcPos pos_builtin = new SrcPos{name = "<builtin>".ToPtr()};

        char** token_kind_names;
        char*[] token_suffix_names;

        char* break_keyword;
        char* case_keyword;
        char* const_keyword;
        char* continue_keyword;
        char* default_keyword;
        char* do_keyword;
        char* else_keyword;
        char* enum_keyword;

        char* first_keyword;
        char* for_keyword;
        char* func_keyword;
        char* if_keyword;
        char* last_keyword;
        char* line_start;
        char* return_keyword;
        char* sizeof_keyword;
        char *typeof_keyword;
        char *alignof_keyword;
        char *offsetof_keyword;
        char *import_keyword;

        Buffer<char> str_buf;
        char* stream;
        char* struct_keyword;
        char* switch_keyword;

        Token token;

        char* typedef_keyword;
        char* union_keyword;
        char* var_keyword;
        char* while_keyword;

        char *always_name;
        char *foreign_name;
        char *complete_name;
        char *assert_name;
        char *declare_note_name;
        char *static_assert_name;

        TokenKind[] assign_token_to_binary_token = new TokenKind[(int)NUM_TOKEN_KINDS];

        public void lex_init() {

            if (inited)
                return;

            keywords = PtrBuffer.Create();

            assign_token_to_binary_token[(int)TOKEN_ADD_ASSIGN] = TOKEN_ADD;
            assign_token_to_binary_token[(int)TOKEN_SUB_ASSIGN] = TOKEN_SUB;
            assign_token_to_binary_token[(int)TOKEN_OR_ASSIGN] = TOKEN_OR;
            assign_token_to_binary_token[(int)TOKEN_AND_ASSIGN] = TOKEN_AND;
            assign_token_to_binary_token[(int)TOKEN_XOR_ASSIGN] = TOKEN_XOR;
            assign_token_to_binary_token[(int)TOKEN_LSHIFT_ASSIGN] = TOKEN_LSHIFT;
            assign_token_to_binary_token[(int)TOKEN_RSHIFT_ASSIGN] = TOKEN_RSHIFT;
            assign_token_to_binary_token[(int)TOKEN_MUL_ASSIGN] = TOKEN_MUL;
            assign_token_to_binary_token[(int)TOKEN_DIV_ASSIGN] = TOKEN_DIV;
            assign_token_to_binary_token[(int)TOKEN_MOD_ASSIGN] = TOKEN_MOD;

            init_tokens();
            init_keywords();

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
            escape_to_char['\\'] = '\\';
            escape_to_char['\''] = '\'';
            escape_to_char['\"'] = '\"';

        }
        void init_tokens() {
            token_kind_names = (char**)xmalloc((int)NUM_TOKEN_KINDS * sizeof(char**));
            token_suffix_names = new char*[7];

            token_suffix_names[(int)SUFFIX_NONE] = "".ToPtr();
            token_suffix_names[(int)SUFFIX_D] = "d".ToPtr();
            token_suffix_names[(int)SUFFIX_U] = "u".ToPtr();
            token_suffix_names[(int)SUFFIX_L] = "l".ToPtr();
            token_suffix_names[(int)SUFFIX_UL] = "ul".ToPtr();
            token_suffix_names[(int)SUFFIX_LL] = "ll".ToPtr();
            token_suffix_names[(int)SUFFIX_ULL] = "ull".ToPtr();

            token_kind_names[(int)TOKEN_EOF] = "EOF".ToPtr();
            token_kind_names[(int)TOKEN_COLON] = ":".ToPtr();
            token_kind_names[(int)TOKEN_LPAREN] = "(".ToPtr();
            token_kind_names[(int)TOKEN_RPAREN] = ")".ToPtr();
            token_kind_names[(int)TOKEN_LBRACE] = "{".ToPtr();
            token_kind_names[(int)TOKEN_RBRACE] = "}".ToPtr();
            token_kind_names[(int)TOKEN_LBRACKET] = "[".ToPtr();
            token_kind_names[(int)TOKEN_RBRACKET] = "]".ToPtr();
            token_kind_names[(int)TOKEN_COMMA] = ";".ToPtr();
            token_kind_names[(int)TOKEN_DOT] = ".".ToPtr();
            token_kind_names[(int)TOKEN_AT] = "@".ToPtr();
            token_kind_names[(int)TOKEN_POUND] = "#".ToPtr();
            token_kind_names[(int)TOKEN_QUESTION] = "?".ToPtr();
            token_kind_names[(int)TOKEN_ELLIPSIS] = "...".ToPtr();
            token_kind_names[(int)TOKEN_SEMICOLON] = ";".ToPtr();
            token_kind_names[(int)TOKEN_NEG] = "~".ToPtr();
            token_kind_names[(int)TOKEN_NOT] = "!".ToPtr();
            token_kind_names[(int)TOKEN_KEYWORD] = "keyword".ToPtr();
            token_kind_names[(int)TOKEN_INT] = "int".ToPtr();
            token_kind_names[(int)TOKEN_FLOAT] = "float".ToPtr();
            token_kind_names[(int)TOKEN_STR] = "string".ToPtr();
            token_kind_names[(int)TOKEN_NAME] = "name".ToPtr();
            token_kind_names[(int)TOKEN_MUL] = "*".ToPtr();
            token_kind_names[(int)TOKEN_DIV] = "/".ToPtr();
            token_kind_names[(int)TOKEN_MOD] = "%".ToPtr();
            token_kind_names[(int)TOKEN_AND] = "&".ToPtr();
            token_kind_names[(int)TOKEN_LSHIFT] = "<<".ToPtr();
            token_kind_names[(int)TOKEN_RSHIFT] = ">>".ToPtr();
            token_kind_names[(int)TOKEN_ADD] = "+".ToPtr();
            token_kind_names[(int)TOKEN_SUB] = "-".ToPtr();
            token_kind_names[(int)TOKEN_OR] = "|".ToPtr();
            token_kind_names[(int)TOKEN_XOR] = "^".ToPtr();
            token_kind_names[(int)TOKEN_EQ] = "==".ToPtr();
            token_kind_names[(int)TOKEN_NOTEQ] = "!=".ToPtr();
            token_kind_names[(int)TOKEN_LT] = "<".ToPtr();
            token_kind_names[(int)TOKEN_GT] = ">".ToPtr();
            token_kind_names[(int)TOKEN_LTEQ] = "<=".ToPtr();
            token_kind_names[(int)TOKEN_GTEQ] = ">=".ToPtr();
            token_kind_names[(int)TOKEN_AND_AND] = "&&".ToPtr();
            token_kind_names[(int)TOKEN_OR_OR] = "||".ToPtr();
            token_kind_names[(int)TOKEN_ASSIGN] = "=".ToPtr();
            token_kind_names[(int)TOKEN_ADD_ASSIGN] = "+=".ToPtr();
            token_kind_names[(int)TOKEN_SUB_ASSIGN] = "-=".ToPtr();
            token_kind_names[(int)TOKEN_OR_ASSIGN] = "|=".ToPtr();
            token_kind_names[(int)TOKEN_AND_ASSIGN] = "&=".ToPtr();
            token_kind_names[(int)TOKEN_XOR_ASSIGN] = "^=".ToPtr();
            token_kind_names[(int)TOKEN_MUL_ASSIGN] = "*=".ToPtr();
            token_kind_names[(int)TOKEN_DIV_ASSIGN] = "/=".ToPtr();
            token_kind_names[(int)TOKEN_MOD_ASSIGN] = "%=".ToPtr();
            token_kind_names[(int)TOKEN_LSHIFT_ASSIGN] = "<<=".ToPtr();
            token_kind_names[(int)TOKEN_RSHIFT_ASSIGN] = ">>=".ToPtr();
            token_kind_names[(int)TOKEN_INC] = "++".ToPtr();
            token_kind_names[(int)TOKEN_DEC] = "--".ToPtr();
            token_kind_names[(int)TOKEN_COLON_ASSIGN] = ":=".ToPtr();
        }

        void init_keywords() {
            typedef_keyword = _I("typedef");
            keywords->Add(typedef_keyword);

            var arena_end = intern_arena.end;

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

            typeof_keyword = _I("typeof");
            keywords->Add(typeof_keyword);

            alignof_keyword = _I("alignof");
            keywords->Add(alignof_keyword);

            offsetof_keyword = _I("offsetof");
            keywords->Add(offsetof_keyword);

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

            import_keyword = _I("import");
            keywords->Add(import_keyword);

            default_keyword = _I("default");
            keywords->Add(default_keyword);

            always_name = _I("always");
            foreign_name  = _I("foreign");
            complete_name = _I("complete");
            assert_name   = _I("assert");
            declare_note_name = _I("declare_note");
            static_assert_name = _I("static_assert");

            assert(intern_arena.end == arena_end);

            first_keyword = typedef_keyword;
            last_keyword = default_keyword;
        }

        bool is_keyword_name(char* name) {
            return first_keyword <= name && name <= last_keyword;
        }

        string token_kind_name(TokenKind kind) {
            if (kind < NUM_TOKEN_KINDS)
                return _S(token_kind_names[(int)kind]);
            return "<unknown>";
        }

        char* _token_kind_name(TokenKind kind) {
            if (kind < NUM_TOKEN_KINDS)
                return *(token_kind_names + (long)kind);
            return _I("<unknown>");
        }

        string token_info() {
            if (token.kind == TOKEN_NAME || token.kind == TOKEN_KEYWORD)
                return _S(token.name);

            return token_kind_name(token.kind);
        }

        void scan_int() {
            byte @base = 10;
            char *start_digits = stream;
            if (*stream == '0') {
                stream++;
                if (char.ToLower(*stream) == 'x') {
                    stream++;
                    token.mod = MOD_HEX;
                    @base = 16;
                    start_digits = stream;
                }
                else if (char.ToLower(*stream) == 'b') {
                    stream++;
                    token.mod = MOD_BIN;
                    @base = 2;
                    start_digits = stream;
                }
                else if (char.IsDigit(*stream)) {
                    token.mod = MOD_OCT;
                    @base = 8;
                    start_digits = stream;
                }
            }

            ulong val = 0;
            for (;;)
            {
                if (*stream == '_') {
                    stream++;
                    continue;
                }
                byte digit = char_to_digit[*stream];
                if (digit == 0 && *stream != '0')
                    break;

                if (digit >= @base) {
                    error_here("Digit '{0}' out of range for base {1}", *stream, @base);
                    digit = 0;
                }

                if (val > (ulong.MaxValue - (digit) / (ulong)@base)) {
                    error_here("Integer literal overflow");
                    while (char.IsDigit(*stream))
                        stream++;

                    val = 0;
                    break;
                }

                val = val * @base + digit;
                stream++;
            }
            if (stream == start_digits) {
                error_here("Expected base {0} digit, got '{1}'", @base, *stream);
            }
            token.kind = TOKEN_INT;
            token.int_val = val;

            scan_sufffix();
        }

        void scan_float() {
            var start = stream;
            while (char.IsDigit(*stream))
                stream++;

            if (*stream == '.')
                stream++;

            while (char.IsDigit(*stream))
                stream++;

            if (char.ToLower(*stream) == 'e') {
                stream++;
                if (*stream == '+' || *stream == '-')
                    stream++;

                if (!char.IsDigit(*stream))
                    error_here("Expected digit after float literal exponent, found '{0}'.", *stream);

                while (char.IsDigit(*stream))
                    stream++;
            }

            var val = double.Parse(new string(start, 0, (int) (stream - start)));
            if (double.IsPositiveInfinity(val))
                error_here("Float literal overflow");

            token.kind = TOKEN_FLOAT;
            token.float_val = val;

            scan_sufffix();
        }

        void scan_sufffix() {
            if (tolower(*stream) == 'u') {
                if (tolower(*++stream) == 'l') {
                    if (tolower(*++stream) == 'l') {
                        token.suffix = SUFFIX_ULL;
                        stream++;
                    }
                    else
                        token.suffix = SUFFIX_UL;
                }
                else
                    token.suffix = SUFFIX_U;
            }
            else if (tolower(*stream) == 'l') {
                if (tolower(*++stream) == 'l') {
                    token.suffix = SUFFIX_LL;
                    stream++;
                }
                else
                    token.suffix = SUFFIX_L;
            }
            else if (tolower(*stream) == 'd') {
                token.suffix = SUFFIX_D;
                stream++;
            }
        }
        int scan_hex_escape() {
            assert(*stream == 'x');
            stream++;
            int val = char_to_digit[*stream];
            if (val == 0) {
                error_here("\\x needs at least 1 hex digit");
            }
            stream++;
            int digit = char_to_digit[*stream];
            if (digit > 0 || *stream == 48) {
                val *= 16;
                val += digit;
                if (val > 0xFF) {
                    error_here("\\x argument out of range");
                    val = 0xFF;
                }
                stream++;
            }
            return val;
        }


        void scan_char() {
            assert(*stream == '\'');
            stream++;
            var val = 0;
            if (*stream == '\'') {
                error_here("Char literal cannot be empty");
                stream++;
            }
            else if (*stream == '\n') {
                error_here("Char literal cannot contain newline");
            }
            else if (*stream == '\\') {
                stream++;
                if (*stream == 'x') {
                    val = scan_hex_escape();
                }
                else {
                    val = escape_to_char[*stream];
                    if (val == 0 && *stream != '0') {
                        error_here("Invalid char literal escape '\\%c'", *stream);
                    }
                    stream++;
                }
            }
            else {
                val = *stream;
                stream++;
            }

            if (*stream != '\'')
                error_here("Expected closing char quote, got '%c'", *stream);
            else
                stream++;

            token.kind = TOKEN_INT;
            token.int_val = (ulong)val;
            token.mod = MOD_CHAR;
        }

        void scan_str() {
            assert(*stream == '"');
            stream++;
            var start = stream;
            str_buf = Buffer<char>.Create(256);
            if (stream[0] == '"' && stream[1] == '"') {
                stream += 2;
                while (*stream != 0) {
                    if (stream[0] == '"' && stream[1] == '"' && stream[2] == '"') {
                        stream += 3;
                        break;
                    }
                    if (*stream != '\r') {
                        // TODO: Should probably just read files in text mode instead.
                        str_buf.Add(*stream);
                    }
                    if (*stream == '\n') {
                        token.pos.line++;
                    }
                    stream++;
                }
                if (*stream == 0) {
                    error_here("Unexpected end of file within multi-line string literal");
                }
                token.mod = MOD_MULTILINE;
            }
            else {
                while (*stream != 0 && *stream != '"') {
                    var val = *stream;
                    if (val == '\n') {
                        error_here("String literal cannot contain newline");
                        break;
                    }

                    if (val == '\\') {
                        stream++;
                        if (*stream == 'x') {
                            val = (char)scan_hex_escape();
                        }
                        else {
                            val = escape_to_char[*stream];
                            if (val == 0 && *stream != '0') {
                                error_here("Invalid string literal escape '\\%c'", *stream);
                            }
                            stream++;
                        }
                    }
                    else {
                        stream++;
                    }
                    str_buf.Add(val);
                }
                if (*stream != 0) {
                    assert(*stream == '"');
                    stream++;
                }
                else {
                    error_here("Unexpected end of file within string literal");
                }

            }
            str_buf.Add('\0');

            token.kind = TOKEN_STR;
            token.str_val = str_buf._begin;
        }

        void next_token() {
repeat:
            token.start = stream;
            token.suffix = 0;
            token.mod = 0;
            switch (*stream) {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                case '\v':
                    while (char.IsWhiteSpace(*stream))
                        if (*stream++ == '\n') {
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
                    if (char.IsDigit(stream[1])) {
                        scan_float();
                    }
                    else if (stream[1] == '.' && stream[2] == '.') {
                        token.kind = TOKEN_ELLIPSIS;
                        stream += 3;
                    }
                    else {
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
                    while (char.IsDigit(*stream))
                        stream++;

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
                    while (char.IsLetterOrDigit(*stream) || *stream == '_')
                        stream++;

                    token.name = Intern.InternRange(token.start, stream);
                    token.kind = is_keyword_name(token.name) ? TOKEN_KEYWORD : TOKEN_NAME;
                    break;
                case '<':
                    stream++;
                    if (*stream == '<') {
                        token.kind = TOKEN_LSHIFT;
                        stream++;
                        if (*stream == '=') {
                            token.kind = TOKEN_LSHIFT_ASSIGN;
                            stream++;
                        }
                    }
                    else if (*stream == '=') {
                        token.kind = TOKEN_LTEQ;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_LT;
                    }

                    break;
                case '>':
                    stream++;
                    if (*stream == '>') {
                        token.kind = TOKEN_RSHIFT;
                        stream++;
                        if (*stream == '=') {
                            token.kind = TOKEN_RSHIFT_ASSIGN;
                            stream++;
                        }
                    }
                    else if (*stream == '=') {
                        token.kind = TOKEN_GTEQ;
                        stream++;
                    }
                    else {
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
                case '@':
                    token.kind = TOKEN_AT;
                    stream++;
                    break;
                case '#':
                    token.kind = TOKEN_POUND;
                    stream++;
                    break;


                // CASE2
                case ':':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_COLON_ASSIGN;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_COLON;
                    }

                    break;
                case '!':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_NOTEQ;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_NOT;
                    }

                    break;
                case '=':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_EQ;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_ASSIGN;
                    }

                    break;
                case '^':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_XOR_ASSIGN;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_XOR;
                    }

                    break;
                case '*':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_MUL_ASSIGN;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_MUL;
                    }

                    break;
                case '/':
                    if (*++stream == '=') {
                        token.kind = TOKEN_DIV_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '/') {
                        stream++;
                        while (*stream != 0 && *stream != '\n')
                            stream++;
                        goto repeat;
                    }
                    else if (*stream == '*') {
                        stream++;
                        int level = 1;
                        while (*stream != 0 && level > 0) {
                            if (stream[0] == '/' && stream[1] == '*') {
                                level++;
                                stream += 2;
                            }
                            else if (stream[0] == '*' && stream[1] == '/') {
                                level--;
                                stream += 2;
                            }
                            else {
                                if (*stream == '\n') {
                                    token.pos.line++;
                                }
                                stream++;
                            }
                        }
                        goto repeat;
                    }
                    else {
                        token.kind = TOKEN_DIV;
                    }

                    break;
                case '%':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_MOD_ASSIGN;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_MOD;
                    }

                    break;

                // CASE3 Types
                case '+':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_ADD_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '+') {
                        token.kind = TOKEN_INC;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_ADD;
                    }

                    break;
                case '-':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_SUB_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '-') {
                        token.kind = TOKEN_DEC;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_SUB;
                    }

                    break;
                case '&':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_AND_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '&') {
                        token.kind = TOKEN_AND_AND;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_AND;
                    }

                    break;
                case '|':
                    stream++;
                    if (*stream == '=') {
                        token.kind = TOKEN_OR_ASSIGN;
                        stream++;
                    }
                    else if (*stream == '|') {
                        token.kind = TOKEN_OR_OR;
                        stream++;
                    }
                    else {
                        token.kind = TOKEN_OR;
                    }

                    break;
                default:
                    error_here("Invalid '{0}' token, skipping", *stream);
                    stream++;
                    goto repeat;
            }

            token.end = stream;
            token.pos.col = token.start - line_start + 1;
        }

        void init_stream(string buf, string name = "<anonymous>") {
            init_stream(buf.ToPtr(), $"\"{name}\"".ToPtr());
        }

        void init_stream(char* str, char* name = null) {
            token.pos.name = name != null ? name : "<string>".ToPtr();
            path_normalize(token.pos.name);
            token.pos.line = token.pos.col = 1;
            line_start = stream = str;
            next_token();
        }

        bool is_token(TokenKind kind) {
            return token.kind == kind;
        }


        bool is_token_eof() {
            return token.kind == TOKEN_EOF;
        }

        bool is_token_name(char* name) {
            return token.kind == TOKEN_NAME && token.name == name;
        }

        bool is_keyword(char* name) {
            return is_token(TOKEN_KEYWORD) && token.name == name;
        }

        bool match_keyword(char* name) {
            if (is_keyword(name)) {
                next_token();
                return true;
            }

            return false;
        }

        bool match_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }

            return false;
        }

        bool expect_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }

            error_here("Expected token {0}, got {1}", _S(token_kind_names[(int)kind]), token_info());
            return false;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Token
        {
            [FieldOffset(0)] public ulong int_val;
            [FieldOffset(0)] public double float_val;
            [FieldOffset(0)] public char* str_val;
            [FieldOffset(0)] public char* name;

            [FieldOffset(8)] public TokenKind kind;
            [FieldOffset(9)] public TokenMod mod;
            [FieldOffset(10)] public TokenSuffix suffix;
            [FieldOffset(11)] public char* start;
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
        TOKEN_AT,
        TOKEN_POUND,
        TOKEN_ELLIPSIS,
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
        NUM_TOKEN_KINDS,

    }

    internal enum TokenMod : byte
    {
        MOD_NONE,
        MOD_HEX,
        MOD_BIN,
        MOD_OCT,
        MOD_CHAR,
        MOD_MULTILINE,
    }

    enum TokenSuffix : byte
    {
        SUFFIX_NONE,
        SUFFIX_D,
        SUFFIX_U,
        SUFFIX_L,
        SUFFIX_UL,
        SUFFIX_LL,
        SUFFIX_ULL,
    }
}