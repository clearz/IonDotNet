using System.Runtime.InteropServices;

namespace IonLang
{
    using static TokenKind;
    using static TokenMod;
    using static TokenSuffix;

    public unsafe partial class Ion
    {
        private static PtrBuffer* keywords;


        private static bool inited;

        private readonly byte[] char_to_digit = new byte[256];
        private readonly char[] escape_to_char = new char[256];
        static SrcPos pos_builtin = new SrcPos{name = "<builtin>".ToPtr()};

        private char** token_kind_names;
        char*[] token_suffix_names;
        void init_tokens() {
            token_kind_names = (char**)xmalloc((int)TOKEN_SIZE * sizeof(char**));
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

        private Buffer<char> str_buf;
        private char* stream;
        private char* struct_keyword;
        private char* switch_keyword;

        private Token token;

        private char* typedef_keyword;
        private char* union_keyword;
        private char* var_keyword;
        private char* while_keyword;

        char *foreign_name;


        public void lex_init() {
            if (inited)
                return;
            keywords = PtrBuffer.Create();


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

            inited = true;
        }

        private void init_keywords() {
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

            foreign_name = _I("foreign");


            assert(intern_arena->end == arena_end);

            first_keyword = typedef_keyword;
            last_keyword = default_keyword;
        }

        private bool is_keyword_name(char* name) {
            return first_keyword <= name && name <= last_keyword;
        }

        private string token_kind_name(TokenKind kind) {
            if (kind < TOKEN_SIZE)
                return new string(token_kind_names[(int)kind]);
            return "<unknown>";
        }

        private char* _token_kind_name(TokenKind kind) {
            if (kind < TOKEN_SIZE)
                return *(token_kind_names + (long)kind);
            return _I("<unknown>");
        }

        private string token_info() {
            if (token.kind == TOKEN_NAME || token.kind == TOKEN_KEYWORD)
                return new string(token.name);

            return token_kind_name(token.kind);
        }

        private void scan_int() {
            ulong @base = 10;
            if (*stream == '0') {
                stream++;
                if (char.ToLower(*stream) == 'x') {
                    stream++;
                    token.mod = MOD_HEX;
                    @base = 16;
                }
                else if (char.ToLower(*stream) == 'b') {
                    stream++;
                    token.mod = MOD_BIN;
                    @base = 2;
                }
                else if (char.IsDigit(*stream)) {
                    token.mod = MOD_OCT;
                    @base = 8;
                }
            }

            ulong val = 0;
            for (; ; )
            {
                ulong digit = (ulong)char_to_digit[*stream];
                if (digit == 0 && *stream != '0')
                    break;

                if (digit >= @base) {
                    error_here("Digit '{0}' out of range for base {1}", *stream, @base);
                    digit = 0;
                }

                if (val > (ulong.MaxValue - (digit) / @base)) {
                    error_here("Integer literal overflow");
                    while (char.IsDigit(*stream))
                        stream++;

                    val = 0;
                    break;
                }

                val = val * @base + digit;
                stream++;
            }

            token.kind = TOKEN_INT;
            token.int_val = val;

            scan_sufffix();
        }

        private void scan_float() {
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
                    error_here("Expected digit after float literal exponent, found '%c'.", *stream);

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

        private void scan_char() {
            assert(*stream == '\'');
            stream++;
            var val = '\0';
            if (*stream == '\'') {
                error_here("Char literal cannot be empty");
                stream++;
            }
            else if (*stream == '\n') {
                error_here("Char literal cannot contain newline");
            }
            else if (*stream == '\\') {
                stream++;
                val = escape_to_char[*stream];
                if (val == 0 && *stream != '0')
                    error_here("Invalid char literal escape '\\%c'", *stream);

                stream++;
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
            token.int_val = val;
            token.mod = MOD_CHAR;
        }

        private void scan_str() {
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
                        val = escape_to_char[*stream];
                        if (val == 0 && *stream != '0')
                            error_here("Invalid string literal escape '\\%c'", *stream);
                    }

                    str_buf.Add(val);
                    stream++;
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

        private void next_token() {
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
                            line_start = stream + 1;
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
            token.pos.col = stream - line_start;
        }

        private void init_stream(string buf, string name = "<anonymous>") {
            init_stream(buf.ToPtr(), $"\"{name}\"".ToPtr());
        }

        private void init_stream(char* str, char* name = null) {
            token.pos.name = name != null ? name : "<string>".ToPtr();
            token.pos.line = token.pos.col = 1;
            stream = str;
            next_token();
        }

        private bool is_token(TokenKind kind) {
            return token.kind == kind;
        }


        private bool is_token_eof() {
            return token.kind == TOKEN_EOF;
        }

        private bool is_token_name(char* name) {
            return token.kind == TOKEN_NAME && token.name == name;
        }

        private bool is_keyword(char* name) {
            return is_token(TOKEN_KEYWORD) && token.name == name;
        }

        private bool match_keyword(char* name) {
            if (is_keyword(name)) {
                next_token();
                return true;
            }

            return false;
        }

        private bool match_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }

            return false;
        }

        private bool expect_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }

            error_here("Expected token {0}, got {1}", new string(token_kind_names[(int)kind]), token_info());
            return false;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Token
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
        TOKEN_SIZE,

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