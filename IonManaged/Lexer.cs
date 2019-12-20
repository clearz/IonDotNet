using System.Collections.Generic;
using System.Text;

namespace IonLangManaged
{
    using static TokenKind;
    using static TokenMod;
    using static TokenSuffix;

    public unsafe partial class Ion
    {
        IList<string> keywords;

        bool inited;

        readonly byte[] char_to_digit = new byte[256];
        readonly char[] escape_to_char = new char[256];
        static SrcPos pos_builtin = new SrcPos{name = "<builtin>"};
        readonly StringBuilder str_buf = new StringBuilder();

        string[] token_kind_names;
        string[] token_suffix_names;

        string break_keyword;
        string case_keyword;
        string const_keyword;
        string continue_keyword;
        string default_keyword;
        string do_keyword;
        string else_keyword;
        string enum_keyword;

        string first_keyword;
        string for_keyword;
        string func_keyword;
        string if_keyword;
        string last_keyword;
        string return_keyword;
        string sizeof_keyword;
        string typeof_keyword;
        string alignof_keyword;
        string offsetof_keyword;
        string import_keyword;
        string goto_keyword;
        string new_keyword;
        string undef_keyword;

        string struct_keyword;
        string switch_keyword;

        string typedef_keyword;
        string union_keyword;
        string var_keyword;
        string while_keyword;

        string always_name;
        string inline_name;
        string intrinsic_name;
        string foreign_name;
        string complete_name;
        string assert_name;
        string declare_note_name;
        string static_assert_name;

        Token token;
        char* line_start;
        char* stream;

        TokenKind[] assign_token_to_binary_token = new TokenKind[(int)NUM_TOKEN_KINDS];

        public void lex_init() {

            if (inited)
                return;

            keywords = new List<string>();

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
            token_kind_names = new string[(int)NUM_TOKEN_KINDS];
            token_suffix_names = new string[7];

            token_suffix_names[(int)SUFFIX_NONE] = "";
            token_suffix_names[(int)SUFFIX_D] = "d";
            token_suffix_names[(int)SUFFIX_U] = "u";
            token_suffix_names[(int)SUFFIX_L] = "l";
            token_suffix_names[(int)SUFFIX_UL] = "ul";
            token_suffix_names[(int)SUFFIX_LL] = "ll";
            token_suffix_names[(int)SUFFIX_ULL] = "ull";

            token_kind_names[(int)TOKEN_EOF] = "EOF";
            token_kind_names[(int)TOKEN_COLON] = ":";
            token_kind_names[(int)TOKEN_LPAREN] = "(";
            token_kind_names[(int)TOKEN_RPAREN] = ")";
            token_kind_names[(int)TOKEN_LBRACE] = "{";
            token_kind_names[(int)TOKEN_RBRACE] = "}";
            token_kind_names[(int)TOKEN_LBRACKET] = "[";
            token_kind_names[(int)TOKEN_RBRACKET] = "]";
            token_kind_names[(int)TOKEN_COMMA] = ";";
            token_kind_names[(int)TOKEN_DOT] = ".";
            token_kind_names[(int)TOKEN_AT] = "@";
            token_kind_names[(int)TOKEN_POUND] = "#";
            token_kind_names[(int)TOKEN_QUESTION] = "?";
            token_kind_names[(int)TOKEN_ELLIPSIS] = "...";
            token_kind_names[(int)TOKEN_SEMICOLON] = ";";
            token_kind_names[(int)TOKEN_NEG] = "~";
            token_kind_names[(int)TOKEN_NOT] = "!";
            token_kind_names[(int)TOKEN_KEYWORD] = "keyword";
            token_kind_names[(int)TOKEN_INT] = "int";
            token_kind_names[(int)TOKEN_FLOAT] = "float";
            token_kind_names[(int)TOKEN_STR] = "string";
            token_kind_names[(int)TOKEN_NAME] = "name";
            token_kind_names[(int)TOKEN_MUL] = "*";
            token_kind_names[(int)TOKEN_DIV] = "/";
            token_kind_names[(int)TOKEN_MOD] = "%";
            token_kind_names[(int)TOKEN_AND] = "&";
            token_kind_names[(int)TOKEN_LSHIFT] = "<<";
            token_kind_names[(int)TOKEN_RSHIFT] = ">>";
            token_kind_names[(int)TOKEN_ADD] = "+";
            token_kind_names[(int)TOKEN_SUB] = "-";
            token_kind_names[(int)TOKEN_OR] = "|";
            token_kind_names[(int)TOKEN_XOR] = "^";
            token_kind_names[(int)TOKEN_EQ] = "==";
            token_kind_names[(int)TOKEN_NOTEQ] = "!=";
            token_kind_names[(int)TOKEN_LT] = "<";
            token_kind_names[(int)TOKEN_GT] = ">";
            token_kind_names[(int)TOKEN_LTEQ] = "<=";
            token_kind_names[(int)TOKEN_GTEQ] = ">=";
            token_kind_names[(int)TOKEN_AND_AND] = "&&";
            token_kind_names[(int)TOKEN_OR_OR] = "||";
            token_kind_names[(int)TOKEN_ASSIGN] = "=";
            token_kind_names[(int)TOKEN_ADD_ASSIGN] = "+=";
            token_kind_names[(int)TOKEN_SUB_ASSIGN] = "-=";
            token_kind_names[(int)TOKEN_OR_ASSIGN] = "|=";
            token_kind_names[(int)TOKEN_AND_ASSIGN] = "&=";
            token_kind_names[(int)TOKEN_XOR_ASSIGN] = "^=";
            token_kind_names[(int)TOKEN_MUL_ASSIGN] = "=";
            token_kind_names[(int)TOKEN_DIV_ASSIGN] = "/=";
            token_kind_names[(int)TOKEN_MOD_ASSIGN] = "%=";
            token_kind_names[(int)TOKEN_LSHIFT_ASSIGN] = "<<=";
            token_kind_names[(int)TOKEN_RSHIFT_ASSIGN] = ">>=";
            token_kind_names[(int)TOKEN_INC] = "++";
            token_kind_names[(int)TOKEN_DEC] = "--";
            token_kind_names[(int)TOKEN_COLON_ASSIGN] = ":=";
        }

        void init_keywords() {
            typedef_keyword = "typedef";
            keywords.Add(typedef_keyword);

            enum_keyword = "enum";
            keywords.Add(enum_keyword);

            struct_keyword = "struct";
            keywords.Add(struct_keyword);

            union_keyword = "union";
            keywords.Add(union_keyword);

            var_keyword = "var";
            keywords.Add(var_keyword);

            const_keyword = "const";
            keywords.Add(const_keyword);

            func_keyword = "func";
            keywords.Add(func_keyword);

            sizeof_keyword = "sizeof";
            keywords.Add(sizeof_keyword);

            typeof_keyword = "typeof";
            keywords.Add(typeof_keyword);

            alignof_keyword = "alignof";
            keywords.Add(alignof_keyword);

            offsetof_keyword = "offsetof";
            keywords.Add(offsetof_keyword);

            break_keyword = "break";
            keywords.Add(break_keyword);

            continue_keyword = "continue";
            keywords.Add(continue_keyword);

            return_keyword = "return";
            keywords.Add(return_keyword);

            if_keyword = "if";
            keywords.Add(if_keyword);

            else_keyword = "else";
            keywords.Add(else_keyword);

            while_keyword = "while";
            keywords.Add(while_keyword);

            do_keyword = "do";
            keywords.Add(do_keyword);

            for_keyword = "for";
            keywords.Add(for_keyword);

            switch_keyword = "switch";
            keywords.Add(switch_keyword);

            case_keyword = "case";
            keywords.Add(case_keyword);

            import_keyword = "import";
            keywords.Add(import_keyword);

            goto_keyword = "goto";
            keywords.Add(goto_keyword);

            new_keyword = "new";
            keywords.Add(new_keyword);

            undef_keyword = "undef";
            keywords.Add(undef_keyword);

            default_keyword = "default";
            keywords.Add(default_keyword);

            always_name = "always";
            inline_name = "inline";
            intrinsic_name = "intrinsic";
            foreign_name  = "foreign";
            complete_name = "complete";
            assert_name   = "assert";
            declare_note_name = "declare_note";
            static_assert_name = "static_assert";

            first_keyword = typedef_keyword;
            last_keyword = default_keyword;
        }

        bool is_keyword_name(string name) => keywords.Contains(name);

        string token_kind_name(TokenKind kind) {
            if (kind < NUM_TOKEN_KINDS)
                return token_kind_names[(int)kind];
            return "<unknown>";
        }


        string token_info() {
            if (token.kind == TOKEN_NAME || token.kind == TOKEN_KEYWORD)
                return token.name;

            return token_kind_name(token.kind);
        }

        void scan_int() {
            byte @base = 10;
            char* start_digits = stream;
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
            if (val == 0 && *stream != '0') {
                error_here("\\x needs at least 1 hex digit");
            }
            stream++;
            int digit = char_to_digit[*stream];
            if (digit > 0 || *stream == '0') {
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
            str_buf.Clear();
            if (stream[0] == '"' && stream[1] == '"') {
                stream += 2;
                while (*stream != 0) {
                    if (stream[0] == '"' && stream[1] == '"' && stream[2] == '"') {
                        stream += 3;
                        break;
                    }
                    if (*stream != '\r') {
                        str_buf.Append(*stream);
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
                                error_here("Invalid string literal escape '\\{0}'", *stream);
                            }
                            stream++;
                        }
                    }
                    else {
                        stream++;
                    }
                    str_buf.Append(val);
                }
                if (*stream != 0) {
                    //assert(*stream == '"');
                    stream++;
                    token.name = new string(start - 1, 0, (int)(stream - start + 1));
                }
                else {
                    error_here("Unexpected end of file within string literal");
                }

            }

            token.kind = TOKEN_STR;
            token.str_val = str_buf.ToString();
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

                    var c = stream;
                    stream = token.start;
                    if (*c == '.' || char.ToLower(*c) == 'e')
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

                    token.name = new string(token.start, 0, (int)(stream - token.start));
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

        void init_stream(char* str, string name = null) {
            token.pos.name = name != null ? name : "<string>";
            fixed(char* c = token.pos.name)
                path_normalize(c);
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

        bool is_token_name(string name) {
            return token.kind == TOKEN_NAME && token.name == name;
        }

        bool is_keyword(string name) {
            return is_token(TOKEN_KEYWORD) && token.name == name;
        }

        bool match_keyword(string name) {
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

            error_here("Expected token {0}, got {1}", token_kind_names[(int)kind], token_info());
            return false;
        }

        struct Token
        {
            public ulong int_val;
            public double float_val;
            public string str_val;
            public string name;

            public TokenKind kind;
            public TokenMod mod;
            public TokenSuffix suffix;
            public char* start;
            public char* end;
            public SrcPos pos;
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