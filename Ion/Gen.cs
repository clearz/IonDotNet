//#define DEBUG_VERBOSE
using System.Globalization;
using System.Text;

namespace IonLang
{
    using static CompoundFieldKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static TypeKind;
    using static TypespecKind;
    using static TokenMod;


    unsafe partial class Ion
    {
        const int _1MB = 1024 * 1024;

        readonly char* cdecl_buffer = xmalloc<char>(_1MB);
        readonly StringBuilder gen_buf = new StringBuilder();
        readonly char[] char_to_escape  = new char[256];
        readonly PtrBuffer* gen_headers_buf = PtrBuffer.Create();

        readonly string preamble =  "// Preamble\n" +
                                            "#ifndef _CRT_SECURE_NO_WARNINGS\n" +
                                            "#define _CRT_SECURE_NO_WARNINGS\n" +
                                            "#endif\n" +
                                            "#if _MSC_VER >= 1900 || __STDC_VERSION__ >= 201112L\n" +
                                            "// Visual Studio 2015 supports enough C99/C11 features for us.\n" +
                                            "#else\n" +
                                            "#error \"C11 support required or Visual Studio 2015 or later\"\n" +
                                            "#endif\n" +
                                            "\n" +
                                            "#include <stdbool.h>\n" +
                                            "#include <stdint.h>\n" +
                                            "#include <stdarg.h>\n" +
                                            "#include <assert.h>\n" +
                                            "#include <stddef.h>\n\n" +

                                            "typedef unsigned char uchar;\n" +
                                            "typedef signed char schar;\n" +
                                            "typedef unsigned short ushort;\n" +
                                            "typedef unsigned int uint;\n" +
                                            "typedef unsigned long ulong;\n" +
                                            "typedef long long llong;\n" +
                                            "typedef unsigned long long ullong;\n" +
                                            "\n" +
                                            "#ifdef _MSC_VER\n" +
                                            "#define alignof(x) __alignof(x)\n" +
                                            "#else\n" +
                                            "#define alignof(x) __alignof__(x)\n" +
                                            "#endif\n" +
                                            "\n"  +
                                            "#define va_start_ptr(args, arg) (va_start(*(args), *(arg)))\n"  +
                                            "#define va_copy_ptr(dest, src) (va_copy(*(dest), *(src)))\n"    +
                                            "#define va_end_ptr(args) (va_end(*(args)))\n"                   +
                                            "\n"      +
                                            "struct Any;\n"+
                                            "void va_arg_ptr(va_list *args, struct Any any);\n";


        string gen_postamble =              "\n// Postamble\n"                                           +
                                            "void va_arg_ptr(va_list *args, Any any) {\n"                +
                                            "    switch (typeid_kind(any.type)) {\n"                     +
                                            "    case TYPE_BOOL:\n"                                      +
                                            "        *(bool *)any.ptr = (bool)va_arg(*args, int);\n"           +
                                            "        break;\n"                                           +
                                            "    case TYPE_CHAR:\n"                                      +
                                            "        *(char *)any.ptr = (char)va_arg(*args, int);\n"           +
                                            "        break;\n"                                           +
                                            "    case TYPE_UCHAR:\n"                                     +
                                            "        *(uchar *)any.ptr = (uchar)va_arg(*args, int);\n"          +
                                            "        break;\n"                                           +
                                            "    case TYPE_SCHAR:\n"                                     +
                                            "        *(schar *)any.ptr = (schar)va_arg(*args, int);\n"          +
                                            "        break;\n"                                           +
                                            "    case TYPE_SHORT:\n"                                     +
                                            "        *(short *)any.ptr = (short)va_arg(*args, int);\n"          +
                                            "        break;\n"                                           +
                                            "    case TYPE_USHORT:\n"                                    +
                                            "        *(ushort *)any.ptr = (ushort)va_arg(*args, int);\n"         +
                                            "        break;\n"                                           +
                                            "    case TYPE_INT:\n"                                       +
                                            "        *(int *)any.ptr = va_arg(*args, int);\n"            +
                                            "        break;\n"                                           +
                                            "    case TYPE_UINT:\n"                                      +
                                            "        *(uint *)any.ptr = va_arg(*args, uint);\n"          +
                                            "        break;\n"                                           +
                                            "    case TYPE_LONG:\n"                                      +
                                            "        *(long *)any.ptr = va_arg(*args, long);\n"          +
                                            "        break;\n"                                           +
                                            "    case TYPE_ULONG:\n"                                     +
                                            "        *(ulong *)any.ptr = va_arg(*args, ulong);\n"        +
                                            "        break;\n"                                           +
                                            "    case TYPE_LLONG:\n"                                     +
                                            "        *(llong *)any.ptr = va_arg(*args, llong);\n"        +
                                            "        break;\n"                                           +
                                            "    case TYPE_ULLONG:\n"                                    +
                                            "        *(ullong *)any.ptr = va_arg(*args, ullong);\n"      +
                                            "        break;\n"                                           +
                                            "    case TYPE_FLOAT:\n"                                     +
                                            "        *(float *)any.ptr = (float)va_arg(*args, double);\n"+
                                            "        break;\n"                                           +
                                            "    case TYPE_DOUBLE:\n"                                    +
                                            "        *(double *)any.ptr = va_arg(*args, double);\n"      +
                                            "        break;\n"                                           +
                                            "    case TYPE_FUNC:\n"                                      +
                                            "    case TYPE_PTR:\n"                                       +
                                            "        *(void **)any.ptr = va_arg(*args, void *);\n"       +
                                            "        break;\n"                                           +
                                            "    default:\n"                                             +
                                            "        assert(0 && \"argument type not supported\");\n"    +
                                            "        break;\n"                                           +
                                            "    }\n"                                                    +
                                            "}\n";


        readonly char* defineStr = "#define ".ToPtr();
        readonly char* includeStr = "#include ".ToPtr();
        readonly char* lineStr = "#line ".ToPtr();
        readonly char* void_str = "void".ToPtr();
        readonly char* externStr = "extern".ToPtr();

        SrcPos gen_pos;
        int gen_indent;

        void indent() {
            var size = gen_indent * 4;

            gen_buf.Append(string.Empty.PadRight(size).ToPtr(out int s), s);
            gen_pos.line++;
        }

        void writeln() {
#if DEBUG_VERBOSE
            System.Console.WriteLine();
#endif
            gen_buf.Append('\n');
        }

        void c_write(char c) {
#if DEBUG_VERBOSE
            System.Console.Write(c);
#endif
            gen_buf.Append(c);
        }

        void c_write(char* c) {
#if DEBUG_VERBOSE
            System.Console.Write(new string(c));
#endif
            gen_buf.Append(c, strlen(c));
            //while ((*(gen_buf + gen_pos) = *(c++)) != 0) gen_pos++;
        }

        void c_write(char* c, int len) {
#if DEBUG_VERBOSE
            System.Console.Write(new string(c));
#endif
            gen_buf.Append(c, len);
            //Unsafe.CopyBlock(gen_buf + gen_pos, c, (uint)len << 1);
            //gen_pos += len;
        }
        void c_write(string s) {
#if DEBUG_VERBOSE
            System.Console.Write(s);
#endif
            gen_buf.Append(s);
            //Unsafe.CopyBlock(gen_buf + gen_pos, c, (uint)len << 1);
            //gen_pos += len;
        }
        void genlnf(char c) {
            genln();
            c_write(c);
        }
        void genlnf(char* fmt) {
            genln();
            c_write(fmt);
        }
        void genlnf(char* fmt, int len) {
            genln();
            c_write(fmt, len);
        }

        void genln() {
            writeln();
            indent();
            gen_pos.line++;
        }

        bool gen_reachable(Sym* sym) {
            return flag_fullgen || sym->reachable == REACHABLE_NATURAL;
        }

        bool is_excluded_typeinfo(Type* type) {
            return type->sym != null && !gen_reachable(type->sym);
        }

        Map gen_name_map;
        char* get_gen_name_or_default(void* ptr, char* default_name) {
            char *name = gen_name_map.map_get<char>(ptr);
            if (name == null) {
                Sym *sym = get_resolved_sym(ptr);
                if (sym != null) {
                    if (sym->external_name != null) {
                        name = sym->external_name;
                    }
                    else if (sym->package->external_name != null) {
                        name = strcat2(sym->package->external_name, sym->name);
                    }
                    else {
                        name = sym->name;
                    }
                }
                else {
                    assert(default_name);
                    name = default_name;
                }
                gen_name_map.map_put(ptr, (void*)name);
            }
            return name;
        }

        char* get_gen_name(void* ptr) {
            return get_gen_name_or_default(ptr, "ERROR".ToPtr());
        }
        char* cdecl_name(Type* type) {
            char* type_name = type_names[(int)type->kind];
            if (type_name != null) {
                return type_name;
            }
            else {
                assert(type->sym != null);
                return get_gen_name(type->sym);
            }
        }

        void type_to_cdecl(Type* type, char* str) {
            switch (type->kind) {
                case TYPE_PTR:
                    if (str != null) {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        //buf_write(cdecl_name(type->@base));
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPE_CONST:
                    if (str != null) {
                        type_to_cdecl(type->@base, const_keyword);
                        c_write(' ');
                        c_write('(');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        c_write(const_keyword, 5);
                        c_write(' ');
                        type_to_cdecl(type->@base, null);
                    }
                    break;
                case TYPE_ARRAY:
                    if (str != null) {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write(str);
                        c_write('[');
                        c_write(type->num_elems.itoa());
                        c_write(']');
                        c_write(')');
                    }
                    else {
                        type_to_cdecl(type->@base, null);
                        c_write(' ');
                        c_write('[');
                        c_write(type->num_elems.itoa());
                        c_write(']');
                    }

                    break;
                case TYPE_FUNC: {
                    if (type->func.ret != null)
                        type_to_cdecl(type->func.ret, null);
                    else
                        c_write(void_str, 4);
                    c_write('(');
                    c_write('*');
                    if (str != null)
                        c_write(str);
                    c_write(')');

                    c_write('(');
                    if (type->func.num_params == 0)
                        c_write(void_str, 4);
                    else
                        for (var i = 0; i < type->func.num_params; i++) {
                            if (i > 0) {
                                c_write(',');
                                c_write(' ');
                            }

                            type_to_cdecl(type->func.@params[i], null);
                        }
                    if (type->func.has_varargs) {
                        c_write(',');
                        c_write(' ');
                        c_write('.');
                        c_write('.');
                        c_write('.');
                    }
                    c_write(')');
                }
                break;
                default:
                    if (str != null) {
                        c_write(cdecl_name(type));
                        c_write(' ');
                        c_write(str);
                    }
                    else {
                        c_write(cdecl_name(type));
                    }
                    break;
            }
        }

        void gen_str(char* str, bool multiline) {
            if (multiline) {
                gen_indent++;
                genln();
            }
            c_write('\"');
            while (*str != 0) {
                var start = str;
                while (*str != 0 && isprint(*str) && char_to_escape[*str] == 0)
                    str++;
                if (start != str)
                    c_write(start, (int)(str - start));
                if (*str != 0 && *str < 256) {
                    if (char_to_escape[*str] != 0) {
                        c_write('\\');
                        c_write(char_to_escape[*str]);
                        if (multiline && str[0] == '\n' && str[1] != 0) {
                            c_write('\"');
                            genlnf('\"');
                        }
                    }
                    else {
                        assert(!isprint(*str));

                        c_write('\\');
                        c_write('x');
                        c_write(((int)*str).itoa(16));
                    }
                    str++;
                }
            }
            c_write('\"');
            if (multiline) {
                gen_indent--;
            }
        }

        void gen_sync_pos(SrcPos pos) {
            if (flag_skip_lines)
                return;
            assert(pos.name != null && pos.line != 0);
            if (gen_pos.line != pos.line || gen_pos.name != pos.name) {
                genln();
                c_write(lineStr, 6);
                c_write(pos.line.itoa());
                if (gen_pos.name != pos.name) {
                    c_write(' ');
                    gen_str(pos.name, false);
                }
                gen_pos = pos;
            }
        }

        void typespec_to_cdecl(Typespec* typespec, char* str) {
            if (typespec == null) {
                c_write(void_str);
                if (*str != 0)
                    c_write(' ');
            }
            switch (typespec->kind) {
                case TYPESPEC_NAME:
                    if (str != null) {
                        c_write(get_gen_name_or_default(typespec, typespec->name));
                        c_write(' ');
                        c_write(str);
                    }
                    else {
                        c_write(get_gen_name_or_default(typespec, typespec->name));
                    }

                    break;
                case TYPESPEC_CONST:
                    if (str != null) {
                        c_write(const_keyword);
                        c_write(' ');
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        c_write(const_keyword);
                        c_write(' ');
                        typespec_to_cdecl(typespec->@base, null);
                    }
                    break;
                case TYPESPEC_PTR:
                    if (str != null) {
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        if (typespec->@base != null)
                            typespec_to_cdecl(typespec->@base, null);
                        if (typespec->name != null)
                            c_write(typespec->name);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPESPEC_ARRAY:
                    if (typespec->@base != null) {
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');
                    }
                    if (str != null) {
                        c_write('(');
                        c_write(str);
                    }
                    c_write('[');
                    if (typespec->num_elems != null)
                        gen_expr(typespec->num_elems);
                    c_write(']');

                    if (str != null)
                        c_write(')');

                    break;
                case TYPESPEC_FUNC: {
                    if (typespec->func.ret != null)
                        typespec_to_cdecl(typespec->func.ret, null);
                    else
                        c_write(void_str, 4);
                    c_write(' ');


                    c_write('(');
                    c_write('*');
                    if (str != null)
                        c_write(str);
                    c_write(')');


                    c_write('(');
                    if (typespec->func.num_args == 0) {
                        c_write(void_str, 4);
                    }
                    else {
                        for (var i = 0; i < typespec->func.num_args; i++) {
                            if (i > 0) {
                                c_write(',');
                                c_write(' ');
                            }

                            typespec_to_cdecl(typespec->func.args[i], null);
                        }

                        if (typespec->func.has_varargs) {
                            c_write(',');
                            c_write(' ');
                            c_write('.');
                            c_write('.');
                            c_write('.');
                        }
                    }
                    c_write(')');

                    break;
                }

                default:
                    assert(false);
                    break;
            }
        }

        void gen_defs() {
            for (Sym** it = (Sym**)sorted_syms->_begin; it < sorted_syms->_top; it++) {
                Sym* sym = *it;
                Decl* decl = sym->decl;
                if (sym->state != SymState.SYM_RESOLVED || decl == null || is_decl_foreign(decl) || decl->is_incomplete || sym->reachable != REACHABLE_NATURAL) {
                    continue;
                }
                if (decl->kind == DECL_FUNC) {

                    gen_func_decl(decl);
                    c_write(' ');
                    gen_stmt_block(decl->func.block);
                    genln();

                }
                else if (decl->kind == DECL_VAR) {
                    genln();
                    if (decl->var.type != null && !is_incomplete_array_typespec(decl->var.type)) {
                        typespec_to_cdecl(decl->var.type, get_gen_name(sym));
                    }
                    else {
                        type_to_cdecl(sym->type, get_gen_name(sym));
                    }
                    if (decl->var.expr != null) {
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_expr(decl->var.expr);
                    }
                    c_write(';');
                    genln();
                }
            }
        }
        void gen_func_decl(Decl* decl) {
            assert(decl->kind == DECL_FUNC);

            gen_sync_pos(decl->pos);
            if (decl->func.ret_type != null) {
                genln();
                typespec_to_cdecl(decl->func.ret_type, null);
                c_write(' ');
                c_write(get_gen_name(decl));
                c_write('(');
            }
            else {
                genln();
                c_write(void_str);
                c_write(' ');
                c_write(get_gen_name(decl));
                c_write('(');
            }

            if (decl->func.num_params == 0)
                c_write(void_str);
            else
                for (var i = 0; i < decl->func.num_params; i++) {
                    var param = decl->func.@params + i;
                    if (i != 0) {
                        c_write(',');
                        c_write(' ');
                    }
                    typespec_to_cdecl(param->type, param->name);
                }
            if (decl->func.has_varargs) {
                c_write(',');
                c_write(' ');
                c_write('.');
                c_write('.');
                c_write('.');
            }
            c_write(')');
        }
        void gen_forward_decls() {
            for (var it = (Sym**)sorted_syms->_begin; it != sorted_syms->_top; it++) {
                var sym = *it;

                var decl = sym->decl;
                if (decl == null || !gen_reachable(sym))
                    continue;
                if (is_decl_foreign(decl))
                    continue;

                var name = get_gen_name(sym);
                switch (decl->kind) {
                    case DECL_STRUCT:
                        genln();
                        c_write(typedef_keyword);
                        c_write(' ');
                        c_write(struct_keyword);
                        c_write(' ');
                        c_write(name);
                        c_write(' ');
                        c_write(name);
                        c_write(';');
                        break;
                    case DECL_UNION:
                        genln();
                        c_write(typedef_keyword);
                        c_write(' ');
                        c_write(union_keyword);
                        c_write(' ');
                        c_write(name);
                        c_write(' ');
                        c_write(name);
                        c_write(';');
                        break;
                }
            }
        }

        void gen_aggregate(Decl* decl) {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            if (decl->is_incomplete) {
                return;
            }
            genln();
            if (decl->kind == DECL_STRUCT)
                c_write(struct_keyword);
            else
                c_write(union_keyword);
            c_write(' ');
            c_write(get_gen_name(decl));
            c_write(' ');
            c_write('{');

            gen_indent++;
            for (var i = 0; i < decl->aggregate.num_items; i++) {
                var item = decl->aggregate.items[i];
                for (var j = 0; j < item.num_names; j++) {
                    gen_sync_pos(item.pos);
                    genln();
                    typespec_to_cdecl(item.type, item.names[j]);
                    c_write(';');
                }
            }

            gen_indent--;
            genln();
            c_write('}');
            c_write(';');
        }

        char* typeid_kind_name(Type* type) {
            if (type->kind < NUM_TYPE_KINDS) {
                char *name = typeid_kind_names[(int)type->kind];
                if (name != null) {
                    return name;
                }
            }
            return typeid_kind_names[(int)TYPE_NONE];
        }

        void gen_typeid(Type* type) {
            if (type->size == 0 || is_excluded_typeinfo(type)) {
                c_write("TYPEID0(".ToPtr(), 8);
                c_write(type->typeid.itoa());
                c_write(',');
                c_write(' ');
                c_write(typeid_kind_name(type));
                c_write(')');
            }
            else {
                c_write("TYPEID(".ToPtr(), 7);
                c_write(type->typeid.itoa());
                c_write(',');
                c_write(' ');
                c_write(typeid_kind_name(type));
                c_write(',');
                c_write(' ');
                type_to_cdecl(type, null);
                c_write(')');
            }
        }

        void gen_expr_compound(Expr* expr) {
            Type *expected_type = get_resolved_expected_type(expr);

            if (expected_type != null && !is_ptr_type(expected_type)) {
                c_write('{');
            }
            else if (expr->compound.type != null) {
                c_write('(');
                typespec_to_cdecl(expr->compound.type, null);
                c_write(')');
                c_write('{');
            }
            else {
                c_write('(');
                type_to_cdecl(get_resolved_type(expr), null);
                c_write(')');
                c_write('{');
            }
            for (int i = 0; i < expr->compound.num_fields; i++) {
                if (i != 0) {
                    c_write(',');
                    c_write(' ');
                }
                CompoundField field = expr->compound.fields[i];
                if (field.kind == FIELD_NAME) {
                    c_write('.');
                    c_write(field.name);
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                }
                else if (field.kind == FIELD_INDEX) {
                    c_write('[');
                    gen_expr(field.index);
                    c_write(']');
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                }

                gen_expr(field.init);
            }
            if (expr->compound.num_fields == 0) {
                c_write('0');
            }
            c_write('}');
        }

        void gen_char(char c) {
            c_write('\'');
            if (char_to_escape[c] != 0) {
                c_write('\\');
                c_write(char_to_escape[c]);
            }
            else if (isprint(c)) {
                c_write(c);
            }
            else {
                c_write('\\');
                c_write('x');
                c_write(((int)c).itoa(16));
            }
            c_write('\'');
        }

        void gen_expr(Expr* expr) {
            switch (expr->kind) {
                case EXPR_PAREN:
                    c_write('(');
                    gen_expr(expr->paren.expr);
                    c_write(')');
                    break;
                case EXPR_INT: {
                    char *suffix_name = token_suffix_names[(int)expr->int_lit.suffix];
                    switch (expr->int_lit.mod) {
                        case MOD_BIN:
                        case MOD_HEX:
                            c_write('0');
                            c_write('x');
                            c_write(expr->int_lit.val.itoa(16));
                            c_write(suffix_name);
                            break;
                        case MOD_OCT:
                            c_write('0');
                            c_write(expr->int_lit.val.itoa(8));
                            c_write(suffix_name);
                            break;
                        case MOD_CHAR:
                            gen_char((char)expr->int_lit.val);
                            break;
                        default:
                            c_write(expr->int_lit.val.itoa());
                            c_write(suffix_name);
                            break;
                    }
                }
                break;
                case EXPR_FLOAT:
                    c_write(expr->float_lit.val.ToString("0.0###############", CultureInfo.InvariantCulture).ToPtr());
                    if (expr->float_lit.suffix != TokenSuffix.SUFFIX_D)
                        c_write('f');
                    break;
                case EXPR_STR:
                    gen_str(expr->str_lit.val, expr->str_lit.mod == MOD_MULTILINE);
                    break;
                case EXPR_NAME:
                    c_write(get_gen_name_or_default(expr, expr->name));
                    break;
                case EXPR_CAST:
                    c_write('(');
                    typespec_to_cdecl(expr->cast.type, null);
                    c_write(')');
                    c_write('(');
                    gen_expr(expr->cast.expr);
                    c_write(')');
                    break;
                case EXPR_CALL:
                    Sym *sym = get_resolved_sym(expr->call.expr);
                    if (sym != null && sym->kind == SymKind.SYM_TYPE) {
                        c_write('(');
                        c_write(get_gen_name(sym));
                        c_write(')');
                    }
                    else
                        gen_expr(expr->call.expr);
                    c_write('(');
                    for (var i = 0; i < expr->call.num_args; i++) {
                        if (i != 0) {
                            c_write(',');
                            c_write(' ');
                        }

                        gen_expr(expr->call.args[i]);
                    }

                    c_write(')');
                    break;
                case EXPR_INDEX:
                    gen_expr(expr->index.expr);
                    c_write('[');
                    gen_expr(expr->index.index);
                    c_write(']');
                    break;
                case EXPR_FIELD:
                    gen_expr(expr->field.expr);
                    if (get_resolved_type(expr->field.expr)->kind == TYPE_PTR) {

                        c_write('-');
                        c_write('>');
                    }
                    else
                        c_write('.');
                    c_write(expr->field.name);
                    break;
                case EXPR_COMPOUND:
                    gen_expr_compound(expr);
                    break;
                case EXPR_UNARY:
                    c_write(_token_kind_name(expr->unary.op));
                    c_write('(');
                    gen_expr(expr->unary.expr);
                    c_write(')');
                    break;
                case EXPR_BINARY:
                    c_write('(');
                    gen_expr(expr->binary.left);
                    c_write(')');
                    c_write(' ');
                    c_write(_token_kind_name(expr->binary.op));
                    c_write(' ');
                    c_write('(');
                    gen_expr(expr->binary.right);
                    c_write(')');
                    break;
                case EXPR_TERNARY:
                    c_write('(');
                    gen_expr(expr->ternary.cond);
                    c_write(' ');
                    c_write('?');
                    c_write(' ');
                    gen_expr(expr->ternary.then_expr);
                    c_write(' ');
                    c_write(':');
                    c_write(' ');
                    gen_expr(expr->ternary.else_expr);
                    c_write(')');
                    break;
                case EXPR_SIZEOF_EXPR:
                    c_write(sizeof_keyword);
                    c_write('(');
                    gen_expr(expr->sizeof_expr);
                    c_write(')');
                    break;
                case EXPR_SIZEOF_TYPE:
                    c_write(sizeof_keyword);
                    c_write('(');
                    typespec_to_cdecl(expr->sizeof_type, null);
                    c_write(')');
                    break;
                case EXPR_ALIGNOF_EXPR:
                    c_write(alignof_keyword);
                    c_write('(');
                    type_to_cdecl(get_resolved_type(expr->alignof_expr), null);
                    c_write(')');
                    break;
                case EXPR_ALIGNOF_TYPE:
                    c_write(alignof_keyword);
                    c_write('(');
                    typespec_to_cdecl(expr->alignof_type, null);
                    c_write(')');
                    break;
                case EXPR_TYPEOF_EXPR: {
                    Type *type = get_resolved_type(expr->typeof_expr);
                    assert(type->typeid);
                    gen_typeid(type);
                    break;
                }
                case EXPR_TYPEOF_TYPE: {
                    Type *type = get_resolved_type(expr->typeof_type);
                    assert(type->typeid);
                    gen_typeid(type);
                    break;
                }
                case EXPR_OFFSETOF: {
                    c_write(offsetof_keyword);
                    c_write('(');
                    type_to_cdecl(get_resolved_type(expr->alignof_type), null);
                    c_write(',');
                    c_write(' ');
                    c_write(expr->offsetof_field.name);
                    c_write(')');
                    break;
                }
                case EXPR_MODIFY:
                    if (!expr->modify.post) {
                        c_write(_token_kind_name(expr->modify.op));
                    }
                    gen_paren_expr(expr->modify.expr);
                    if (expr->modify.post) {
                        c_write(_token_kind_name(expr->modify.op));
                    }
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        bool is_incomplete_array_typespec(Typespec* typespec) {
            return typespec->kind == TYPESPEC_ARRAY && typespec->num_elems == null;
        }

        void gen_stmt_block(StmtList block) {
            c_write('{');
            gen_indent++;
            for (var i = 0; i < block.num_stmts; i++)
                gen_stmt(block.stmts[i]);
            gen_indent--;
            genln();
            c_write('}');
        }

        void gen_paren_expr(Expr* expr) {
            c_write('(');
            gen_expr(expr);
            c_write(')');
        }

        void gen_simple_stmt(Stmt* stmt) {
            switch (stmt->kind) {
                case STMT_EXPR:
                    gen_expr(stmt->expr);
                    break;
                case STMT_INIT:
                    if (stmt->init.type != null) {
                        Typespec *init_typespec = stmt->init.type;
                        if (is_incomplete_array_typespec(stmt->init.type)) {
                            Expr *size = new_expr_int(init_typespec->pos, (ulong)get_resolved_type(stmt->init.expr)->num_elems, 0, 0);
                            init_typespec = new_typespec_array(init_typespec->pos, init_typespec->@base, size);
                        }
                        typespec_to_cdecl(stmt->init.type, stmt->init.name);
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        if (stmt->init.expr != null) {
                            gen_expr(stmt->init.expr);
                        }
                        else {
                            c_write('{');
                            c_write('0');
                            c_write('}');
                        }
                    }
                    else {
                        type_to_cdecl(unqualify_type(get_resolved_type(stmt->init.expr)), stmt->init.name);
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_expr(stmt->init.expr);
                    }
                    break;
                case STMT_ASSIGN:
                    gen_expr(stmt->assign.left);
                    c_write(' ');
                    c_write(_token_kind_name(stmt->assign.op));
                    c_write(' ');
                    gen_expr(stmt->assign.right);
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void gen_stmt(Stmt* stmt) {
            gen_sync_pos(stmt->pos);
            switch (stmt->kind) {
                case STMT_RETURN:
                    genlnf(return_keyword);
                    if (stmt->expr != null) {
                        c_write(' ');
                        gen_expr(stmt->expr);
                    }

                    c_write(';');
                    break;
                case STMT_BREAK:
                    genlnf(break_keyword);
                    c_write(';');
                    break;
                case STMT_CONTINUE:
                    genlnf(continue_keyword);
                    c_write(';');
                    break;
                case STMT_BLOCK:
                    genln();
                    gen_stmt_block(stmt->block);
                    break;
                case STMT_NOTE:
                    if (stmt->note.name == assert_name) {
                        genlnf(assert_name);
                        c_write('(');
                        assert(stmt->note.num_args == 1);
                        gen_expr(stmt->note.args[0].expr);
                        c_write(')');
                        c_write(';');
                    }
                    break;
                case STMT_IF:
                    if (stmt->if_stmt.init != null) {
                        genlnf('{');
                        gen_indent++;
                        gen_stmt(stmt->if_stmt.init);
                    }
                    genlnf(if_keyword);
                    c_write(' ');
                    c_write('(');
                    if (stmt->if_stmt.cond != null) {
                        gen_expr(stmt->if_stmt.cond);
                    }
                    else {
                        c_write(stmt->if_stmt.init->init.name);
                    }
                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->if_stmt.then_block);
                    for (var i = 0; i < stmt->if_stmt.num_elseifs; i++) {
                        var elseif = stmt->if_stmt.elseifs[i];
                        c_write(' ');
                        c_write(else_keyword);
                        c_write(' ');
                        c_write(if_keyword);
                        c_write(' ');
                        c_write('(');
                        gen_expr(elseif->cond);
                        c_write(')');
                        c_write(' ');
                        gen_stmt_block(elseif->block);
                    }

                    if (stmt->if_stmt.else_block.stmts != null) {
                        c_write(' ');
                        c_write(else_keyword);
                        c_write(' ');
                        gen_stmt_block(stmt->if_stmt.else_block);
                    }
                    else {
                        Note *complete_note = get_stmt_note(stmt, complete_name);
                        if (complete_note != null) {
                            c_write(' ');
                            c_write(else_keyword);
                            c_write(' ');
                            c_write('{');
                            gen_indent++;
                            gen_sync_pos(complete_note->pos);
                            genlnf("assert(\"@complete if/elseif chain failed to handle case\" && 0);".ToPtr());
                            gen_indent--;
                            genlnf('}');
                        }
                    }

                    if (stmt->if_stmt.init != null) {
                        gen_indent--;
                        genlnf('}');
                    }
                    break;
                case STMT_WHILE:
                    genlnf(while_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->while_stmt.cond);
                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->while_stmt.block);
                    break;
                case STMT_DO_WHILE:
                    genlnf(do_keyword);
                    c_write(' ');
                    gen_stmt_block(stmt->while_stmt.block);
                    c_write(' ');
                    c_write(while_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->while_stmt.cond);
                    c_write(')');
                    c_write(';');
                    break;
                case STMT_FOR:
                    genlnf(for_keyword);
                    c_write(' ');
                    c_write('(');
                    if (stmt->for_stmt.init != null)
                        gen_simple_stmt(stmt->for_stmt.init);
                    c_write(';');
                    if (stmt->for_stmt.cond != null) {
                        c_write(' ');
                        gen_expr(stmt->for_stmt.cond);
                    }

                    c_write(';');
                    if (stmt->for_stmt.next != null) {
                        c_write(' ');
                        gen_simple_stmt(stmt->for_stmt.next);
                    }

                    c_write(')');
                    c_write(' ');
                    gen_stmt_block(stmt->for_stmt.block);
                    break;
                case STMT_SWITCH: {
                    genlnf(switch_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->switch_stmt.expr);
                    c_write(')');
                    c_write(' ');
                    c_write('{');
                    bool has_default = false;
                    gen_indent++;
                    for (var i = 0; i < stmt->switch_stmt.num_cases; i++) {
                        var switch_case = stmt->switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_exprs; j++) {
                            genlnf(case_keyword);
                            c_write(' ');
                            gen_expr(switch_case.exprs[j]);
                            c_write(':');
                        }

                        if (switch_case.is_default) {
                            has_default = true;
                            c_write(default_keyword);
                            c_write(':');
                        }

                        c_write(' ');
                        c_write('{');
                        gen_indent++;
                        StmtList block = switch_case.block;
                        for (int j = 0; j < block.num_stmts; j++) {
                            gen_stmt(block.stmts[j]);
                        }
                        genlnf(break_keyword, 5);
                        c_write(';');
                        gen_indent--;
                        genlnf('}');
                    }
                    if (!has_default) {
                        Note *note = get_stmt_note(stmt, complete_name);
                        if (note != null) {
                            c_write(default_keyword);
                            c_write(':');
                            gen_indent++;
                            genlnf("assert(\"@complete switch failed to handle case\" && 0);".ToPtr());
                            genln();
                            c_write(break_keyword);
                            c_write(';');
                            gen_indent--;
                        }
                    }
                    gen_indent--;
                    genln();
                    c_write('}');
                    break;
                }
                default:
                    genln();
                    gen_simple_stmt(stmt);
                    c_write(';');
                    break;
            }
        }

        void gen_enum(Decl* decl) {
            assert(decl->kind == DECL_ENUM);
            genln();
            c_write(typedef_keyword, 7);
            c_write(' ');
            c_write(enum_keyword, 4);
            c_write(' ');
            c_write(decl->name);
            c_write(' ');
            c_write('{');
            gen_indent++;
            for (var i = 0; i < decl->enum_decl.num_items; i++) {
                genln();
                c_write(decl->enum_decl.items[i].name);
                c_write(',');
            }
            gen_indent--;
            genln();
            c_write('}');
            c_write(decl->name);
            c_write(';');
        }

        void gen_decl(Sym* sym) {
            var decl = sym->decl;
            if (decl == null || is_decl_foreign(decl))
                return;
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                gen_sync_pos(decl->pos);
            switch (decl->kind) {
                case DECL_CONST:
                    genln();
                    c_write(defineStr, 8);
                    c_write(get_gen_name(sym));
                    c_write(' ');
                    c_write('(');
                    if (decl->const_decl.type != null) {
                        c_write('(');
                        typespec_to_cdecl(decl->const_decl.type, null);
                        c_write(')');
                        c_write('(');
                    }
                    gen_expr(decl->const_decl.expr);
                    if (decl->const_decl.type != null)
                        c_write(')');

                    c_write(')');
                    break;
                case DECL_VAR:
                    genlnf(externStr, 6);
                    c_write(' ');
                    if (decl->var.type != null && !is_incomplete_array_typespec(decl->var.type)) {
                        typespec_to_cdecl(decl->var.type, get_gen_name(sym));
                    }
                    else {
                        type_to_cdecl(sym->type, get_gen_name(sym));
                    }
                    c_write(';');
                    break;
                case DECL_FUNC:
                    gen_func_decl(decl);
                    c_write(';');
                    break;
                case DECL_STRUCT:
                case DECL_UNION:
                    gen_aggregate(decl);
                    break;
                case DECL_ENUM:
                    //gen_enum(decl);
                    genln();
                    c_write(typedef_keyword, 7);
                    c_write(' ');
                    c_write(type_names[(int)TYPE_INT], 3);
                    c_write(' ');
                    c_write(get_gen_name(sym));
                    c_write(';');
                    break;
                case DECL_TYPEDEF:
                    genln();
                    c_write(typedef_keyword);
                    c_write(' ');
                    typespec_to_cdecl(decl->typedef_decl.type, get_gen_name(sym));
                    c_write(';');
                    break;
                default:
                    assert(false);
                    break;
            }
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                genln();
        }

        void gen_sorted_decls() {
            var sym_cnt = sorted_syms->count;

            for (var i = 0; i < sym_cnt; i++) {
                var sym = *(Sym**) (sorted_syms->_begin + i);
                if (sym->reachable == REACHABLE_NATURAL) {
                    gen_decl(sym);
                }
            }
        }

        void gen_package_headers(Package* package) {
            char *header_arg_name = _I("header");
            for (var i = 0; i < package->num_decls; i++) {
                Decl *decl = package->decls[i];
                if (decl->kind != DECL_NOTE) {
                    continue;
                }
                Note note = decl->note;
                if (note.name == foreign_name) {
                    for (var j = 0; j < note.num_args; j++) {
                        if (note.args[j].name != header_arg_name) {
                            continue;
                        }
                        Expr *expr = note.args[j].expr;
                        if (expr->kind != EXPR_STR) {
                            fatal_error(decl->pos, "#foreign's header argument must be a quoted string");
                        }
                        char *header = expr->str_lit.val;
                        bool found = false;
                        for (void** it = gen_headers_buf->_begin; it != gen_headers_buf->_top; it++) {
                            if (*it == header) {
                                found = true;
                            }
                        }
                        if (!found) {
                            gen_headers_buf->Add(header);
                            genlnf(includeStr);
                            if (*header == '<')
                                c_write(header);
                            else
                                gen_str(header, false);
                        }
                    }
                }
            }
        }
        void gen_package_sources(Package* package) {
            char *source_arg_name = _I("source");
            for (var i = 0; i < package->num_decls; i++) {
                Decl *decl = package->decls[i];
                if (decl->kind != DECL_NOTE) {
                    continue;
                }
                Note note = decl->note;
                if (note.name == foreign_name) {
                    for (var j = 0; j < note.num_args; j++) {
                        if (note.args[j].name != source_arg_name) {
                            continue;
                        }
                        Expr *expr = note.args[j].expr;
                        if (expr->kind != EXPR_STR) {
                            fatal_error(decl->pos, "#foreign's source argument must be a quoted string");
                        }
                        char* source_path = stackalloc char[MAX_PATH];
                        strcpy(source_path, package->full_path);
                        path_join(source_path, expr->str_lit.val);
                        path_absolute(source_path);
                        genlnf("#include ".ToPtr());
                        gen_str(source_path, false);
                    }
                }
            }
        }

        void path_absolute(char* path) {
            char* rel_path = null;
            //path_copy(rel_path, path);
            //_fullpath(path, rel_path, MAX_PATH);
        }

        void gen_foreign_headers() {
            for (int i = 0; i < package_list->count; i++) {
                gen_package_headers(package_list->Get<Package>(i));
            }
        }

        void gen_foreign_sources() {
            for (int i = 0; i < package_list->count; i++) {
                gen_package_sources(package_list->Get<Package>(i));
            }
        }

        char*[] tiInfo;
        void gen_typeinfos() {
            tiInfo = new[] {"const TypeInfo *typeinfo_table[".ToPtr(out int ti0),
                          "] = {".ToPtr(out int ti1),
                          "int num_typeinfos = ".ToPtr(out int ti2),
                          "const TypeInfo **typeinfos = (const TypeInfo **)typeinfo_table;".ToPtr(out int ti3),
                          "NULL, // No associated type".ToPtr(out int ti4),
                          "&(TypeInfo){TYPE_VOID, .name = \"void\", .size = 0, .align = 0},".ToPtr(out int ti5),
                          "&(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = ".ToPtr(out int ti6),
                          ", .base = ".ToPtr(out int ti7),
                          "NULL, // Incomplete array type".ToPtr(out int ti8),
                          ", .base = ".ToPtr(out int ti9),
                          ", .count = ".ToPtr(out int ti10),
                          ", .name = ".ToPtr(out int ti11),
                          ", .num_fields = ".ToPtr(out int ti12),
                          ", .fields = (TypeFieldInfo[]) {".ToPtr(out int ti13),
                          "NULL, // Func".ToPtr(out int ti14),
                          "NULL, // Enum".ToPtr(out int ti15),
                          "NULL, // Incomplete: ".ToPtr(out int ti16),
                          "NULL, // Unhandled".ToPtr(out int ti17),

                          ", .type = ".ToPtr(out int ti18),
                          ", .offset = offsetof(".ToPtr(out int ti19),

                          "&(TypeInfo){".ToPtr(out int ti20),
                          ", .size = sizeof(".ToPtr(out int ti21),
                          "), .align = alignof(".ToPtr(out int ti22),
                          ", .size = 0, .align = 0".ToPtr(out int ti23),
                          "#define TYPEID0(index, kind) ((ullong)(index) | ((ullong)(kind) << 24))".ToPtr(out int ti24),
                          "#define TYPEID(index, kind, ...) ((ullong)(index) | ((ullong)sizeof(__VA_ARGS__) << 32) | ((ullong)(kind) << 24))".ToPtr(out int ti25),
            };
            genlnf(tiInfo[24], ti24);
            genlnf(tiInfo[25], ti25);
            genln();

            if (flag_notypeinfo) {
                genln();
                c_write("int num_typeinfos;");
                genln();
                c_write("const TypeInfo **typeinfos;");
            }
            else {
                uint num_typeinfos = next_typeid;
                genln();
                c_write(tiInfo[0], ti0);
                c_write(num_typeinfos.itoa());
                c_write(tiInfo[1], ti1);
                gen_indent++;
                for (int typeid = 0; typeid < num_typeinfos; typeid++) {
                    genln();
                    c_write('[');
                    c_write(typeid.itoa());
                    c_write(']');
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                    Type *type = get_type_from_typeid(typeid);
                    if (type != null && !is_excluded_typeinfo(type)) {
                        gen_typeinfo(type);
                    }
                    else {
                        c_write(tiInfo[4], ti4);
                    }
                }
                gen_indent--;
                genln();
                c_write('}');
                c_write(';');
                genln();
                c_write(tiInfo[2], ti2);
                c_write(num_typeinfos.itoa());
                c_write(';');
                genln();
                c_write(tiInfo[3], ti3);
                genln();
            }
            void gen_typeinfo_header(char* kind, Type* type) {
                if (type_sizeof(type) == 0) {
                    c_write(tiInfo[20], ti20);
                    c_write(kind);
                    c_write(tiInfo[23], ti23);
                }
                else {
                    c_write(tiInfo[20], ti20);
                    c_write(kind);
                    c_write(tiInfo[21], ti21);
                    type_to_cdecl(type, null);
                    c_write(tiInfo[22], ti22);
                    type_to_cdecl(type, null);
                    c_write(')');
                }
            }

            void gen_typeinfo_fields(Type* type) {
                gen_indent++;

                for (var i = 0; i < type->aggregate.num_fields; i++) {
                    TypeField field = type->aggregate.fields[i];
                    genln();
                    c_write('{');
                    gen_str(field.name, false);

                    c_write(tiInfo[18], ti18);
                    gen_typeid(field.type);
                    c_write(tiInfo[19], ti19);
                    c_write(get_gen_name(type->sym));
                    c_write(',');
                    c_write(' ');
                    c_write(field.name);
                    c_write(')');
                    c_write('}');
                    c_write(',');
                }
                gen_indent--;
            }

            void gen_type(TypeKind tk) {
                char* c = type_names[(int)tk];
                c_write(tiInfo[20], ti20);
                c_write(typeid_kind_names[(int)tk]);
                c_write(tiInfo[21], ti21);
                c_write(c);
                c_write(tiInfo[22], ti22);
                c_write(c);
                c_write(')');
                c_write(tiInfo[11], ti11);
                gen_str(c, false);
            }

            void gen_typeinfo(Type* type) {
                switch (type->kind) {
                    case TYPE_BOOL:
                        gen_type(TYPE_BOOL);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_CHAR:
                        gen_type(TYPE_CHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UCHAR:
                        gen_type(TYPE_UCHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SCHAR:
                        gen_type(TYPE_SCHAR);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SHORT:
                        gen_type(TYPE_SHORT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_USHORT:
                        gen_type(TYPE_USHORT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_INT:
                        gen_type(TYPE_INT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UINT:
                        gen_type(TYPE_UINT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LONG:
                        gen_type(TYPE_LONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULONG:
                        gen_type(TYPE_ULONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LLONG:
                        gen_type(TYPE_LLONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULLONG:
                        gen_type(TYPE_ULLONG);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_FLOAT:
                        gen_type(TYPE_FLOAT);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_DOUBLE:
                        gen_type(TYPE_DOUBLE);
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_VOID:
                        c_write(tiInfo[5], ti5);
                        break;
                    case TYPE_PTR:
                        c_write(tiInfo[6], ti6);
                        gen_typeid(type->@base);
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_CONST:
                        gen_typeinfo_header(typeid_kind_names[(int)TYPE_CONST], type);
                        c_write(tiInfo[7], ti7);
                        gen_typeid(type->@base);
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_ARRAY:
                        if (is_incomplete_array_type(type)) {
                            c_write(tiInfo[8], ti8);
                        }
                        else {
                            gen_typeinfo_header(typeid_kind_names[(int)TYPE_ARRAY], type);
                            c_write(tiInfo[9], ti9);
                            gen_typeid(type->@base);
                            c_write(tiInfo[10], ti10);
                            c_write(type->num_elems.itoa());
                            c_write('}');
                            c_write(',');
                        }
                        break;
                    case TYPE_STRUCT:
                    case TYPE_UNION:
                        gen_typeinfo_header(typeid_kind_names[(int)type->kind], type);
                        c_write(tiInfo[11], ti11);
                        gen_str(get_gen_name(type->sym), false);
                        c_write(tiInfo[12], ti12);
                        c_write(type->aggregate.num_fields.itoa());
                        c_write(tiInfo[13], ti13);
                        gen_typeinfo_fields(type);
                        c_write('}');
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_FUNC:
                        c_write(tiInfo[14], ti14);
                        break;
                    case TYPE_ENUM:
                        c_write(tiInfo[15], ti15);
                        break;
                    case TYPE_INCOMPLETE:
                        c_write(tiInfo[16], ti16);
                        c_write(get_gen_name(type->sym));
                        break;
                    default:
                        c_write(tiInfo[17], ti17);
                        break;
                }
            }

        }
        void gen_package_external_names() {
            for (int i = 0; i < package_list->count; i++) {
                Package *package = package_list->Get<Package>(i);
                if (package->external_name == null) {
                    char *external_name = xmalloc<char>(strlen(package->path)+2);
                    char* p2 = external_name;
                    for (char* ptr = package->path; *ptr != 0; ptr++) {
                        if (*ptr == '/')
                            *p2++ = '_';
                        else
                            *p2++ = *ptr;
                    }
                    *p2++ = '_';
                    *p2++ = '\0';
                    package->external_name = _I(external_name);
                }
            }
        }

        void gen_all() {
            c_write(preamble.ToPtr());
            gen_package_external_names();
            genlnf("// Foreign header files".ToPtr());
            gen_foreign_headers();
            genln();
            genlnf("// Forward declarations".ToPtr());
            gen_forward_decls();
            genln();
            genlnf("// Sorted declarations".ToPtr());
            gen_sorted_decls();
            genlnf("// Typeinfo".ToPtr());
            gen_typeinfos();
            genlnf("// Definitions".ToPtr());
            gen_defs();
            genlnf("// Foreign source files".ToPtr());
            gen_foreign_sources();
            genln();
            c_write(gen_postamble.ToPtr());
        }

        void init_chars() {
            // TODO: Need to expand this and also deal with non-printable chars via \x
            char_to_escape['\0'] = '0';
            char_to_escape['\n'] = 'n';
            char_to_escape['\r'] = 'r';
            char_to_escape['\t'] = 't';
            char_to_escape['\v'] = 'v';
            char_to_escape['\b'] = 'b';
            char_to_escape['\a'] = 'a';
            char_to_escape['\\'] = '\\';
            char_to_escape['"'] = '"';
            char_to_escape['\''] = '\'';
        }
    }
}