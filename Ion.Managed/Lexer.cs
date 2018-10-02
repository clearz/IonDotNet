using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MLang
{
    public class Lexer
    {
        internal static string typedef_keyword;
        internal static string enum_keyword;
        internal static string struct_keyword;
        internal static string union_keyword;
        internal static string var_keyword;
        internal static string const_keyword;
        internal static string func_keyword;
        internal static string sizeof_keyword;
        internal static string cast_keyword;
        internal static string break_keyword;
        internal static string continue_keyword;
        internal static string return_keyword;
        internal static string if_keyword;
        internal static string else_keyword;
        internal static string while_keyword;
        internal static string do_keyword;
        internal static string for_keyword;
        internal static string switch_keyword;
        internal static string case_keyword;
        internal static string default_keyword;
        
        IList<string> keywords;


        bool inited = false;
        private int idx = 0;

        static readonly Dictionary<TokenKind, string> tokenKindNames = new Dictionary<TokenKind, string> {
            [TokenKind.TOKEN_EOF] = "EOF",
            [TokenKind.TOKEN_COLON] = ":",
            [TokenKind.TOKEN_LPAREN] = "(",
            [TokenKind.TOKEN_RPAREN] = ")",
            [TokenKind.TOKEN_LBRACE] = "{",
            [TokenKind.TOKEN_RBRACE] = "}",
            [TokenKind.TOKEN_LBRACKET] = "[",
            [TokenKind.TOKEN_RBRACKET] = "]",
            [TokenKind.TOKEN_COMMA] = ",",
            [TokenKind.TOKEN_DOT] = ".",
            [TokenKind.TOKEN_QUESTION] = "?",
            [TokenKind.TOKEN_SEMICOLON] = ";",
            [TokenKind.TOKEN_NEG] = "~",
            [TokenKind.TOKEN_NOT] = "!",
            [TokenKind.TOKEN_KEYWORD] = "keyword",
            [TokenKind.TOKEN_INT] = "int",
            [TokenKind.TOKEN_FLOAT] = "float",
            [TokenKind.TOKEN_STR] = "string",
            [TokenKind.TOKEN_NAME] = "name",
            [TokenKind.TOKEN_MUL] = "*",
            [TokenKind.TOKEN_DIV] = "/",
            [TokenKind.TOKEN_MOD] = "%",
            [TokenKind.TOKEN_AND] = "&",
            [TokenKind.TOKEN_LSHIFT] = "<<",
            [TokenKind.TOKEN_RSHIFT] = ">>",
            [TokenKind.TOKEN_ADD] = "+",
            [TokenKind.TOKEN_SUB] = "-",
            [TokenKind.TOKEN_OR] = "|",
            [TokenKind.TOKEN_XOR] = "^",
            [TokenKind.TOKEN_EQ] = "==",
            [TokenKind.TOKEN_NOTEQ] = "!=",
            [TokenKind.TOKEN_LT] = "<",
            [TokenKind.TOKEN_GT] = ">",
            [TokenKind.TOKEN_LTEQ] = "<=",
            [TokenKind.TOKEN_GTEQ] = ">=",
            [TokenKind.TOKEN_AND_AND] = "&&",
            [TokenKind.TOKEN_OR_OR] = "||",
            [TokenKind.TOKEN_ASSIGN] = "=",
            [TokenKind.TOKEN_ADD_ASSIGN] = "+=",
            [TokenKind.TOKEN_SUB_ASSIGN] = "-=",
            [TokenKind.TOKEN_OR_ASSIGN] = "|=",
            [TokenKind.TOKEN_AND_ASSIGN] = "&=",
            [TokenKind.TOKEN_XOR_ASSIGN] = "^=",
            [TokenKind.TOKEN_MUL_ASSIGN] = "*=",
            [TokenKind.TOKEN_DIV_ASSIGN] = "/=",
            [TokenKind.TOKEN_MOD_ASSIGN] = "%=",
            [TokenKind.TOKEN_LSHIFT_ASSIGN] = "<<=",
            [TokenKind.TOKEN_RSHIFT_ASSIGN] = ">>=",
            [TokenKind.TOKEN_INC] = "++",
            [TokenKind.TOKEN_DEC] = "--",
            [TokenKind.TOKEN_COLON_ASSIGN] = ":=",
        };

        private readonly byte[] char_to_digit = new byte[256];
        private readonly char[] escape_to_char = new char[256];

        internal Token token = new Token();
        string stream;

        public void lex_init() {
            if (inited)
                return;
            keywords = new List<string>();

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

            cast_keyword = "cast";
            keywords.Add(cast_keyword);

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

            default_keyword = "default";
            keywords.Add(default_keyword);
            
            
        }

        bool is_keyword_name(string name) => keywords.Contains(name);

        internal static string token_kind_name(TokenKind kind) {
            if (kind < TokenKind.TOKEN_SIZE) {
                return tokenKindNames[kind];
            }
            else {
                return "<unknown>";
            }
        }

        internal string token_info() {
            if (token.kind == TokenKind.TOKEN_NAME || token.kind == TokenKind.TOKEN_KEYWORD)
                return token.name;

            return token_kind_name(token.kind);
        }

        void scan_int() {
            ulong @base = 10;
            if (stream[idx] == '0' && idx+1 < stream.Length) { 
                idx++;
                if (Char.ToLower(stream[idx]) == 'x') {
                    idx++;
                    token.mod = TokenMod.TOKENMOD_HEX;
                    @base = 16;
                }
                else if (Char.ToLower(stream[idx]) == 'b') {
                    idx++;
                    token.mod = TokenMod.TOKENMOD_BIN;
                    @base = 2;
                }
                else if (Char.IsDigit(stream[idx])) {
                    token.mod = TokenMod.TOKENMOD_OCT;
                    @base = 8;
                }
            }

            ulong val = 0;
            for (; idx < stream.Length;) {
                ulong digit = char_to_digit[stream[idx]];
                if (digit == 0 && stream[idx] != '0') {
                    break;
                }

                if (digit >= @base) {
                    Error.syntax_error("Digit '{0}' out of range for base {1}", stream[idx], @base);
                    digit = 0;
                }

                if (val > (18446744073709551615ul - digit) / @base) {
                    Error.syntax_error("Integer literal overflow");
                    while (Char.IsDigit(stream[idx])) {
                        idx++;
                    }

                    val = 0;
                    break;
                }

                val = val * @base + digit;
                idx++;
            }

            token.kind = TokenKind.TOKEN_INT;
            token.int_val = (ulong) val;
        }

        void scan_float() {
            string start = stream;
            while (Char.IsDigit(stream[idx])) {
                idx++;
            }

            if (stream[idx] == '.') {
                idx++;
            }

            while (Char.IsDigit(stream[idx])) {
                if (idx + 1 < stream.Length)
                    idx++;
                else break;
            }

            if (Char.ToLower(stream[idx]) == 'e') {
                idx++;
                if (stream[idx] == '+' || stream[idx] == '-') {
                    idx++;
                }

                if (!Char.IsDigit(stream[idx])) {
                    Error.syntax_error("Expected digit after float literal exponent, found '{0}'.", stream[idx]);
                }

                while (idx < stream.Length && Char.IsDigit(stream[idx])) {
                    idx++;
                }
            }

            double val = double.Parse(start.Substring(token.start, idx-token.start));
            if (double.IsPositiveInfinity(val)) {
                Error.syntax_error("Float literal overflow");
            }

            token.kind = TokenKind.TOKEN_FLOAT;
            token.float_val = val;
        }

        void scan_char() {
            Error.assert(stream[idx] == '\'');
            idx++;
            char val = '\0';
            if (stream[idx] == '\'') {
                Error.syntax_error("Char literal cannot be empty");
                idx++;
            }
            else if (stream[idx] == '\n') {
                Error.syntax_error("Char literal cannot contain newline");
            }
            else if (stream[idx] == '\\') {
                idx++;
                val = escape_to_char[stream[idx]];
                if (val == 0 && stream[idx] != '0') {
                    Error.syntax_error("Invalid char literal escape '\\%c'", stream[idx]);
                }

                idx++;
            }
            else {
                val = stream[idx];
                idx++;
            }

            if (stream[idx] != '\'') {
                Error.syntax_error("Expected closing char quote, got '%c'", stream[idx]);
            }
            else {
                idx++;
            }

            token.kind = TokenKind.TOKEN_INT;
            token.int_val = val;
            token.mod = TokenMod.TOKENMOD_CHAR;
        }

        private readonly StringBuilder str_buf = new StringBuilder();

        void scan_str() {
            Error.assert(stream[idx] == '"');
            idx++;
            string start = stream;
            string str = null;
            str_buf.Clear();
            while (stream[idx] != 0 && stream[idx] != '"') {
                char val = stream[idx];
                if (val == '\n') {
                    Error.syntax_error("String literal cannot contain newline");
                    break;
                }
                else if (val == '\\') {
                    idx++;
                    val = escape_to_char[stream[idx]];
                    if (val == 0 && stream[idx] != '0') {
                        Error.syntax_error("Invalid string literal escape '\\%c'", stream[idx]);
                    }
                }

                str_buf.Append(val);
                idx++;
            }

            if (stream[idx] != 0) {
                Error.assert(stream[idx] == '"');
                idx++;
            }
            else {
                Error.syntax_error("Unexpected end of file within string literal");
            }

            token.kind = TokenKind.TOKEN_STR;
            token.str_val = str_buf.ToString();
        }

        internal void next_token() {
            repeat:
            token.start = idx;
            token.mod = 0;
            if (idx == stream.Length) {
                token.kind = TokenKind.TOKEN_EOF;

            }
            else switch (stream[idx]) {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                case '\v':
                    while (Char.IsWhiteSpace(stream[idx])) {
                        idx++;
                    }
                    goto repeat;
                case '\'':
                    scan_char();
                    break;
                case '"':
                    scan_str();
                    break;
                case '.':
                    if (Char.IsDigit(stream[idx+1])) {
                        scan_float();
                    }
                    else {
                        token.kind = TokenKind.TOKEN_DOT;
                        idx++;
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
                    while (idx < stream.Length && Char.IsDigit(stream[idx])) {
                        idx++;
                    }

                    char c = idx == stream.Length ? '\0' : stream[idx];
                    idx = token.start;
                    if (c == '.' || Char.ToLower(c) == 'e') {
                        scan_float();
                    }
                    else {
                        scan_int();
                    }

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
                    while (idx != stream.Length && (Char.IsLetterOrDigit(stream[idx]) || stream[idx] == '_')) {
                        idx++;
                    }

                    token.name = stream.Substring(token.start, idx - token.start);
                    token.kind = is_keyword_name(token.name) ? TokenKind.TOKEN_KEYWORD : TokenKind.TOKEN_NAME;
                    break;
                case '<':
                    token.kind = TokenKind.TOKEN_LT;
                    idx++;
                    if (stream[idx] == '<') {
                        token.kind = TokenKind.TOKEN_LSHIFT;
                        idx++;
                        if (stream[idx] == '=') {
                            token.kind = TokenKind.TOKEN_LSHIFT_ASSIGN;
                            idx++;
                        }
                    }
                    else if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_LTEQ;
                        idx++;
                    }

                    break;
                case '>':
                    token.kind = TokenKind.TOKEN_GT;
                    idx++;
                    if (stream[idx] == '>') {
                        token.kind = TokenKind.TOKEN_RSHIFT;
                        idx++;
                        if (stream[idx] == '=') {
                            token.kind = TokenKind.TOKEN_RSHIFT_ASSIGN;
                            idx++;
                        }
                    }
                    else if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_GTEQ;
                        idx++;
                    }

                    break;
                case '\0':
                    token.kind = TokenKind.TOKEN_EOF;
                    idx++;
                    break;
                case '(':
                    token.kind = TokenKind.TOKEN_LPAREN;
                    idx++;
                    break;
                case ')':
                    token.kind = TokenKind.TOKEN_RPAREN;
                    idx++;
                    break;
                case '{':
                    token.kind = TokenKind.TOKEN_LBRACE;
                    idx++;
                    break;
                case '}':
                    token.kind = TokenKind.TOKEN_RBRACE;
                    idx++;
                    break;
                case '[':
                    token.kind = TokenKind.TOKEN_LBRACKET;
                    idx++;
                    break;
                case ']':
                    token.kind = TokenKind.TOKEN_RBRACKET;
                    idx++;
                    break;
                case ',':
                    token.kind = TokenKind.TOKEN_COMMA;
                    idx++;
                    break;
                case '?':
                    token.kind = TokenKind.TOKEN_QUESTION;
                    idx++;
                    break;
                case ';':
                    token.kind = TokenKind.TOKEN_SEMICOLON;
                    idx++;
                    break;
                case '~':
                    token.kind = TokenKind.TOKEN_NEG;
                    idx++;
                    break;
                case '!':
                    token.kind = TokenKind.TOKEN_NOT;
                    idx++;
                    break;

                case ':':
                    token.kind = TokenKind.TOKEN_COLON;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_COLON_ASSIGN;
                        idx++;
                    }

                    break;
                case '=':
                    token.kind = TokenKind.TOKEN_ASSIGN;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_EQ;
                        idx++;
                    }

                    break;
                case '^':
                    token.kind = TokenKind.TOKEN_XOR;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_XOR_ASSIGN;
                        idx++;
                    }

                    break;
                case '*':
                    token.kind = TokenKind.TOKEN_MUL;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_MUL_ASSIGN;
                        idx++;
                    }

                    break;
                case '/':
                    token.kind = TokenKind.TOKEN_DIV;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_DIV_ASSIGN;
                        idx++;
                    }

                    break;
                case '%':
                    token.kind = TokenKind.TOKEN_MOD;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_MOD_ASSIGN;
                        idx++;
                    }

                    break;
                case '+':
                    token.kind = TokenKind.TOKEN_ADD;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_ADD_ASSIGN;
                        idx++;
                    }
                    else if (stream[idx] == '+') {
                        token.kind = TokenKind.TOKEN_INC;
                        idx++;
                    }

                    break;
                case '-':
                    token.kind = TokenKind.TOKEN_SUB;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_SUB_ASSIGN;
                        idx++;
                    }
                    else if (stream[idx] == '-') {
                        token.kind = TokenKind.TOKEN_DEC;
                        idx++;
                    }

                    break;
                case '&':
                    token.kind = TokenKind.TOKEN_AND;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_AND_ASSIGN;
                        idx++;
                    }
                    else if (stream[idx] == '&') {
                        token.kind = TokenKind.TOKEN_AND_AND;
                        idx++;
                    }

                    break;
                case '|':
                    token.kind = TokenKind.TOKEN_OR;
                    idx++;
                    if (stream[idx] == '=') {
                        token.kind = TokenKind.TOKEN_OR_ASSIGN;
                        idx++;
                    }
                    else if (stream[idx] == '|') {
                        token.kind = TokenKind.TOKEN_OR_OR;
                        idx++;
                    }

                    break;
                default:
                    Error.syntax_error("Invalid '{0}' token, skipping", stream[idx]);
                    idx++;
                    goto repeat;
            }

            token.end = idx;
        }

        internal void init_stream(string str) {
            idx = 0;
            stream = str;
            next_token(); 
        }

        internal bool is_token(TokenKind kind) {
            return token.kind == kind;
        }


        internal bool is_token_eof() {
            return token.kind == TokenKind.TOKEN_EOF;
        }

        bool is_token_name(string name) {
            return token.kind == TokenKind.TOKEN_NAME && token.name == name;
        }

        internal bool is_keyword(string name) {
            return is_token(TokenKind.TOKEN_KEYWORD) && token.name == name;
        }

        internal bool match_keyword(string name) {
            if (is_keyword(name)) {
                next_token();
                return true;
            }
            else {
                return false;
            }
        }

        internal bool match_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }
            else {
                return false;
            }
        }

        internal bool expect_token(TokenKind kind) {
            if (is_token(kind)) {
                next_token();
                return true;
            }
            else { 
                Error.fatal("expected token {0}, got {1}", tokenKindNames[kind], token_info());
                return false;
            }
        }

        void keyword_test() {
            init_keywords();
            Error.assert(is_keyword_name(keywords.First()));
            Error.assert(is_keyword_name(keywords.Last()));
            foreach(var it in keywords)
                Error.assert(is_keyword_name(it));
            

            Error.assert(!is_keyword_name("foo"));
        }

        private void assert_token(TokenKind x) => Error.assert(match_token(x));
        private void assert_token_name(string x) => Error.assert(token.name == x && match_token(TokenKind.TOKEN_NAME));
        private void assert_token_int(ulong x) => Error.assert(token.int_val == x && match_token(TokenKind.TOKEN_INT));
        private void assert_token_float(double x) => Error.assert(token.float_val == x && match_token(TokenKind.TOKEN_FLOAT));

        private void assert_token_str(string x) => Error.assert(token.str_val == x && match_token(TokenKind.TOKEN_STR));
        private void assert_token_eof() => Error.assert(is_token(0));

        public void lex_test() {
            keyword_test();

            // Integer literal tests
            init_stream("0 18446744073709551615 0xffffffffffffffff 042 0b1111");
            assert_token_int(0);
            assert_token_int(18446744073709551615L);
            Error.assert(token.mod == TokenMod.TOKENMOD_HEX);
            assert_token_int(0xffffffffffffffffL);
            Error.assert(token.mod == TokenMod.TOKENMOD_OCT);
            assert_token_int(34);
            Error.assert(token.mod == TokenMod.TOKENMOD_BIN);
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
            assert_token_str("foo");
            assert_token_str("a\nb");
            assert_token_eof();

            // Operator tests
            init_stream(": := + += ++ < <= << <<=");
            assert_token(TokenKind.TOKEN_COLON);
            assert_token(TokenKind.TOKEN_COLON_ASSIGN);
            assert_token(TokenKind.TOKEN_ADD);
            assert_token(TokenKind.TOKEN_ADD_ASSIGN);
            assert_token(TokenKind.TOKEN_INC);
            assert_token(TokenKind.TOKEN_LT);
            assert_token(TokenKind.TOKEN_LTEQ);
            assert_token(TokenKind.TOKEN_LSHIFT);
            assert_token(TokenKind.TOKEN_LSHIFT_ASSIGN);
            assert_token_eof();

            // Misc tests
            init_stream("XY+(XY)_HELLO1,234+994");
            assert_token_name("XY");
            assert_token(TokenKind.TOKEN_ADD);
            assert_token(TokenKind.TOKEN_LPAREN);
            assert_token_name("XY");
            assert_token(TokenKind.TOKEN_RPAREN);
            assert_token_name("_HELLO1");
            assert_token(TokenKind.TOKEN_COMMA);
            assert_token_int(234);
            assert_token(TokenKind.TOKEN_ADD);
            assert_token_int(994);
            assert_token_eof();
        }


        internal class Token
        {
            public ulong int_val;
            public double float_val;
            public string str_val;
            public string name;

            public TokenKind kind;
            public TokenMod mod;
            public int start;
            public int end;
        }
    }

    enum TokenKind : byte
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

    enum TokenMod : byte
    {
        TOKENMOD_NONE,
        TOKENMOD_HEX,
        TOKENMOD_BIN,
        TOKENMOD_OCT,
        TOKENMOD_CHAR,
    }
}


