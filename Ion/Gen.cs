using System;
using System.Globalization;
using System.Runtime.CompilerServices;
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
        private const int _1MB = 1024 * 1024;

        private readonly char* cdecl_buffer = xmalloc<char>(_1MB);
        // private Buffer<char> gen_buf = Buffer<char>.Create(_1MB, 4);
        StringBuilder gen_buf = new StringBuilder();
        private readonly char[] char_to_escape  = new char[256];
        PtrBuffer* gen_headers_buf = PtrBuffer.Create();

        private readonly string preamble = "// Preamble\n#include <stdbool.h>\n#include <stdint.h>\n#include <stddef.h>\n\n"+
                                            "typedef unsigned char uchar;\n"+
                                            "typedef signed char schar;\n"+
                                            "typedef unsigned short ushort;\n"+
                                            "typedef unsigned int uint;\n"+
                                            "typedef unsigned long ulong;\n"+
                                            "typedef long long llong;\n"+
                                            "typedef unsigned long long ullong;\n"+
                                            "\n"+
                                            "typedef uint8_t uint8;\n"        +
                                            "typedef int8_t int8;\n"         +
                                            "typedef uint16_t uint16;\n"     +
                                            "typedef int16_t int16;\n"       +
                                            "typedef uint32_t uint32;\n"     +
                                            "typedef int32_t int32;\n"       +
                                            "typedef uint64_t uint64;\n"     +
                                            "typedef int64_t int64;\n"       +
                                            "\n"                             +
                                            "typedef uintptr_t uintptr;\n"   +
                                            "typedef size_t usize;\n"        +
                                            "typedef ptrdiff_t ssize;\n"     +
                                            "typedef int typeid;\n"          +
                                            "\n" +
                                            "#ifdef _MSC_VER\n"              +
                                            "#define alignof(x) __alignof(x)\n"+
                                            "#else\n"+
                                            "#define alignof(x) __alignof__(x)\n"+
                                            "#endif\n";

        readonly char* defineStr = "#define ".ToPtr();
        readonly char* lineStr = "#line ".ToPtr();
        readonly char* VOID = "void".ToPtr();
        readonly char* externStr = "extern".ToPtr();


        readonly char* forward_includes     = "// Forward includes".ToPtr();
        readonly char* forward_declarations = "// Forward declarations".ToPtr();
        readonly char* sorted_declarations = "// Sorted declarations".ToPtr();
        readonly char* definitions = "// Definitions".ToPtr();
        char *include_name;

        SrcPos gen_pos;
        int gen_indent;

        private void indent() {
            var size = gen_indent * 4;

            gen_buf.Append(' ', size);
            gen_pos.line++;
        }

        private void writeln() {
            gen_buf.Append('\n');
        }

        private void c_write(char c) {
            gen_buf.Append(c);
        }

        private void c_write(char* c) {
            gen_buf.Append(c, strlen(c));
            //while ((*(gen_buf + gen_pos) = *(c++)) != 0) gen_pos++;
        }

        private void c_write(char* c, int len) {
            gen_buf.Append(c, len);
            //Unsafe.CopyBlock(gen_buf + gen_pos, c, (uint)len << 1);
            //gen_pos += len;
        }
        private void genlnf(char c) {
            genln();
            c_write(c);
        }
        private void genlnf(char* fmt) {
            genln();
            c_write(fmt);
        }
        private void genlnf(char* fmt, int len) {
            genln();
            c_write(fmt, len);
        }

        private void genln() {
            writeln();
            indent();
            gen_pos.line++;
        }




        private char* cdecl_name(Type* type) {
            char* type_name = type_names[(int)type->kind];
            if (type_name != null) {
                return type_name;
            }
            else {
                assert(type->sym != null);
                return type->sym->name;
            }
        }

        private void type_to_cdecl(Type* type, char* str) {
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
                    if (str != null) {
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }

                    c_write('(');
                    if (type->func.num_params == 0)
                        c_write(VOID, 4);
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
                    type_to_cdecl(type->func.ret, null);
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

        private void gen_str(char* str, bool multiline) {
            if (multiline) {
                gen_indent++;
                genln();
            }
            c_write('\"');
            while (*str != 0) {
                var start = str;
                while (*str != 0 && !Char.IsControl(*str) && char_to_escape[*str] == 0)
                    str++;
                if (start != str)
                    c_write(start, (int)(str - start));
                if (*str != 0) {
                    if (char_to_escape[*str] != 0) {
                        c_write('\\');
                        c_write(char_to_escape[*str]);
                        if (str[0] == '\n' && str[1] != 0) {
                            c_write('\"');
                            genlnf('\"');
                        }
                    } else {
                        assert(Char.IsControl(*str));

                        c_write('\\');
                        c_write('x');
                        c_write(Convert.ToString((int)*str, 16).ToPtr());
                    }
                    str++;
                }
            }
            c_write('\"');
            if (multiline) {
                gen_indent--;
            }
        }

        private void gen_sync_pos(SrcPos pos) {
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

        private void typespec_to_cdecl(Typespec* typespec, char* str) {
            if(typespec == null) {
                c_write(VOID);
                if (*str != 0)
                    c_write(' ');
            }
            switch (typespec->kind) {
                case TYPESPEC_NAME:
                    if (str != null) {
                        c_write(typespec->name);
                        c_write(' ');
                        c_write(str);
                    }
                    else {
                        c_write(typespec->name);
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
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }
                    else {
                        if(typespec->@base != null)
                            typespec_to_cdecl(typespec->@base, null);
                        if (typespec->name != null)
                            c_write(typespec->name);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPESPEC_ARRAY:
                    if (str != null) {
                        typespec_to_cdecl(typespec->@base, null);
                        c_write(' ');

                        c_write('(');
                        c_write(str);
                        c_write('[');
                        if (typespec->num_elems != null)
                            gen_expr(typespec->num_elems);
                        c_write(']');
                        c_write(')');
                    }
                    else {
                        c_write(typespec->name);
                        c_write(' ');
                        c_write('[');
                        gen_expr(typespec->num_elems);
                        c_write(']');
                    }

                    break;
                case TYPESPEC_FUNC: {
                    if(typespec->func.ret != null)
                        typespec_to_cdecl(typespec->func.ret, null);
                    c_write(' ');
                    if (str != null) {

                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }

                    c_write('(');
                    if (typespec->func.num_args == 0) {
                        c_write(VOID, 4);
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
                        c_write(')');
                    }

                    break;
                }

                default:
                    assert(false);
                    break;
            }
        }

        private void gen_defs() {
            for (Sym** it = (Sym**)global_syms_buf->_begin; it < global_syms_buf->_top; it++) {
                Sym* sym = *it;
                Decl* decl = sym->decl;
                if (decl == null || is_decl_foreign(decl) || decl->is_incomplete) {
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
                        typespec_to_cdecl(decl->var.type, sym->name);
                    }
                    else {
                        type_to_cdecl(sym->type, sym->name);
                    }
                    if (decl->var.expr != null) {
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_init_expr(decl->var.expr);
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
                c_write(decl->name);
                c_write('(');
            }
            else {
                genln();
                c_write(VOID);
                c_write(' ');
                c_write(decl->name);
                c_write('(');
            }

            if (decl->func.num_params == 0)
                c_write(VOID);
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
        private void gen_forward_decls() {
            for (var it = (Sym**)global_syms_buf->_begin; it != global_syms_buf->_top; it++) {
                var sym = *it;

                var decl = sym->decl;
                if (decl == null)
                    continue;
                if (is_decl_foreign(decl))
                    continue;
                
                var name = sym->name;
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

        private void gen_aggregate(Decl* decl) {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            genln();
            if (decl->kind == DECL_STRUCT)
                c_write(struct_keyword);
            else
                c_write(union_keyword);
            c_write(' ');
            c_write(decl->name);
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

        void gen_expr_compound(Expr* expr, bool is_init) {
            if (is_init) {
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

                gen_init_expr(field.init);
            
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
            else if (!Char.IsControl(c)) {
                c_write(c);
            }
            else {
                c_write('\\');
                c_write('x');
                c_write(((ulong)c).itoa(16));
            }
            c_write('\'');
        }

        private void gen_expr(Expr* expr) {
            switch (expr->kind) {
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
                    c_write(expr->name);
                    break;
                case EXPR_CAST:
                    c_write('(');
                    type_to_cdecl(get_resolved_type(expr->cast.type), null);
                    c_write(')');
                    c_write('(');
                    gen_expr(expr->cast.expr);
                    c_write(')');
                    break;
                case EXPR_CALL:
                    c_write('(');
                    gen_expr(expr->call.expr);
                    c_write(')');
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
                    gen_expr_compound(expr, false);
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
                    type_to_cdecl(get_resolved_type(expr->sizeof_type), null);
                    c_write(')');
                    break;
                case EXPR_TYPEOF_EXPR: {
                    Type *type = get_resolved_type(expr->typeof_expr);
                    assert(type->typeid);
                    c_write(type->typeid.itoa());
                    break;
                }
                case EXPR_TYPEOF_TYPE: {
                    Type *type = get_resolved_type(expr->typeof_type);
                    assert(type->typeid);
                    c_write(type->typeid.itoa());
                    break;
                }
                default:
                    assert(false);
                    break;
            }
        }

        void gen_init_expr(Expr* expr) {
            if (expr->kind == EXPR_COMPOUND) {
                gen_expr_compound(expr, true);
            }
            else {
                gen_expr(expr);
            }
        }
        bool is_incomplete_array_typespec(Typespec* typespec) {
            return typespec->kind == TYPESPEC_ARRAY && typespec->num_elems == null;
        }

        private void gen_stmt_block(StmtList block) {
            c_write('{');
            gen_indent++;
            for (var i = 0; i < block.num_stmts; i++)
                gen_stmt(block.stmts[i]);
            gen_indent--;
            genln();
            c_write('}');
        }

        private void gen_simple_stmt(Stmt* stmt) {
            switch (stmt->kind) {
                case STMT_EXPR:
                    gen_expr(stmt->expr);
                    break;
                case STMT_INIT:      
                    if (stmt->init.type != null) {
                        if (is_incomplete_array_typespec(stmt->init.type)) {
                            type_to_cdecl(get_resolved_type(stmt->init.expr), stmt->init.name);
                        }
                        else {
                            typespec_to_cdecl(stmt->init.type, stmt->init.name);
                        }
                        if (stmt->init.expr != null) {
                            c_write(' ');
                            c_write('=');
                            c_write(' ');
                            gen_init_expr(stmt->init.expr);
                        }
                    }
                    else {
                        type_to_cdecl(unqualify_type(get_resolved_type(stmt->init.expr)), stmt->init.name);
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_init_expr(stmt->init.expr);
                    }
                    break;
                case STMT_ASSIGN:
                    gen_expr(stmt->assign.left);
                    if (stmt->assign.right != null) {
                        c_write(' ');
                        c_write(_token_kind_name(stmt->assign.op));
                        c_write(' ');
                        gen_expr(stmt->assign.right);
                    }
                    else {
                        c_write(_token_kind_name(stmt->assign.op));
                    }

                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void gen_stmt(Stmt* stmt) {
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
                case STMT_IF:
                    genlnf(if_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->if_stmt.cond);
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
                case STMT_SWITCH:
                    genlnf(switch_keyword);
                    c_write(' ');
                    c_write('(');
                    gen_expr(stmt->switch_stmt.expr);
                    c_write(')');
                    c_write(' ');
                    c_write('{');

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

                    gen_indent--;
                    genln();
                    c_write('}');
                    break;
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

        private void gen_decl(Sym* sym) {
            var decl = sym->decl;
            if (decl == null || is_decl_foreign(decl))
                return;
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                gen_sync_pos(decl->pos);
            switch (decl->kind) {
                case DECL_CONST:
                    genln();
                    c_write(defineStr, 8);
                    c_write(sym->name);
                    c_write(' ');
                    if (decl->const_decl.type != null) {
                        c_write('(');
                        typespec_to_cdecl(decl->const_decl.type, null);
                        c_write(')');
                        c_write('(');
                    }
                    gen_expr(decl->const_decl.expr);
                    if (decl->const_decl.type != null)
                        c_write(')');

                    break;
                case DECL_VAR:
                    genlnf(externStr, 6);
                    c_write(' ');
                    if (decl->var.type != null && !is_incomplete_array_typespec(decl->var.type)) {
                        typespec_to_cdecl(decl->var.type, sym->name);
                    }
                    else {
                        type_to_cdecl(sym->type, sym->name);
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
                    gen_enum(decl);
                    break;
                case DECL_TYPEDEF:
                    genln();
                    c_write(typedef_keyword);
                    c_write(' ');
                    typespec_to_cdecl(decl->typedef_decl.type, sym->name);
                    c_write(';');
                    break;
                default:
                    assert(false);
                    break;
            }
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                genln();
        }

        private void gen_sorted_decls() {
            var sym_cnt = sorted_syms->count;

            for (var i = 0; i < sym_cnt; i++) {
                //Console.Write($"\r{i} of {sym_cnt}    ");
                var sym = (Sym**) (sorted_syms->_begin + i);
                gen_decl(*sym);
            }
        }
               
        void gen_headers() {
            if (include_name == null)
                include_name = _I("include");
            for (var i = 0; i < global_decls->num_decls; i++) {
                Decl *decl = global_decls->decls[i];
                if (decl->kind != DECL_NOTE) {
                    continue;
                }
                Note note = decl->note;
                if (note.name == foreign_name) {
                    for (var j = 0; j < note.num_args; j++) {
                        if (note.args[j].name != include_name) {
                            continue;
                        }
                        Expr *expr = note.args[j].expr;
                        if (expr->kind != EXPR_STR) {
                            fatal_error(decl->pos, "#foreign_include's argument must be a quoted string");
                        }
                        char *header_name = expr->name;
                        bool found = false;
                        for (void** it = gen_headers_buf->_begin;  it != gen_headers_buf->_top;  it++) {
                            if (*it == header_name) {
                                found = true;
                            }
                        }
                        if (!found) {
                            gen_headers_buf->Add(header_name);
                            genln();
                            c_write('#');
                            c_write(include_name, 7);
                            c_write(' ');
                            if (*header_name == '<')
                                c_write(header_name);
                            else
                                gen_str(header_name, false);
                        }
                    }
                }
            }
        }



    void gen_typeinfos() {
            char*[] tiInfo = {"TypeInfo *typeinfo_table[".ToPtr(out int ti0),
                          "] = {".ToPtr(out int ti1),
                          "int num_typeinfos = ".ToPtr(out int ti2),
                          "TypeInfo **typeinfos = typeinfo_table;".ToPtr(out int ti3),
                          "NULL, // No associated type".ToPtr(out int ti4),
                          "&(TypeInfo){TYPE_VOID, .name = \"void\"},".ToPtr(out int ti5),
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
                          "NULL, // Unhandled kind".ToPtr(out int ti17),

                          ", .type = ".ToPtr(out int ti18),
                          ", .offset = offsetof(".ToPtr(out int ti19),

                          "&(TypeInfo){".ToPtr(out int ti20),
                          ", .size = sizeof(".ToPtr(out int ti21),
                          "), .align = alignof(".ToPtr(out int ti22),
        };

            int num_typeinfos = next_typeid;
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
                if (type != null) {
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

            void gen_typeinfo_header(char *kind, Type *type) {
                c_write(tiInfo[20], ti20);
                c_write(kind);
                c_write(tiInfo[21], ti21);
                type_to_cdecl(type, null);
                c_write(tiInfo[22], ti22);
                type_to_cdecl(type, null);
                c_write(')');
            }

            void gen_typeinfo_fields(Type* type) {
                gen_indent++;
                assert(type->kind == TYPE_STRUCT || type->kind == TYPE_UNION);
                for (var i = 0; i < type->aggregate.num_fields; i++) {
                    TypeField field = type->aggregate.fields[i];
                    c_write('{');
                    gen_str(field.name, false);

                    c_write(tiInfo[18], ti18);
                    c_write(field.type->typeid.itoa());
                    c_write(tiInfo[19], ti19);
                    c_write(type->sym->name);
                    c_write(',');
                    c_write(' ');
                    c_write(field.name);
                    c_write(')');
                    c_write('}');
                    c_write(',');
                }
                gen_indent--;
            }

            void gen_type(TypeKind tk, string s) {
                gen_buf.AppendFormat("&(TypeInfo){{ {0}, .size = sizeof({1}), .align = alignof({1}), .name = ", tk, s);
                gen_str(s.ToPtr(), false);
            }

            void gen_typeinfo(Type* type) {
                switch (type->kind) {
                    case TYPE_BOOL:
                        gen_type(TYPE_BOOL, "bool");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_CHAR:
                        gen_type(TYPE_CHAR, "char");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UCHAR:
                        gen_type(TYPE_UCHAR, "uchar");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SCHAR:
                        gen_type(TYPE_SCHAR, "schar");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_SHORT:
                        gen_type(TYPE_SHORT, "short");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_USHORT:
                        gen_type(TYPE_USHORT, "ushort");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_INT:
                        gen_type(TYPE_INT, "int");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_UINT:
                        gen_type(TYPE_UINT, "uint");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LONG:
                        gen_type(TYPE_LONG, "long");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULONG:
                        gen_type(TYPE_ULONG, "ulong");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_LLONG:
                        gen_type(TYPE_LLONG, "llong");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_ULLONG:
                        gen_type(TYPE_ULLONG, "ullong");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_FLOAT:
                        gen_type(TYPE_FLOAT, "float");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_DOUBLE:
                        gen_type(TYPE_DOUBLE, "double");
                        c_write('}');
                        c_write(',');
                        break;

                    case TYPE_VOID:
                        c_write(tiInfo[5], ti5);
                        break;
                    case TYPE_PTR:
                        c_write(tiInfo[6], ti6);
                        c_write(type->@base->typeid.itoa());
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_CONST:
                        gen_typeinfo_header("TYPE_CONST".ToPtr(), type);
                        c_write(tiInfo[7], ti7);
                        c_write(type->@base->typeid.itoa());
                        c_write('}');
                        c_write(',');
                        break;
                    case TYPE_ARRAY:
                        if (is_incomplete_array_type(type)) {
                            c_write(tiInfo[8], ti8);
                        }
                        else {
                            gen_typeinfo_header("TYPE_ARRAY".ToPtr(), type);
                            c_write(tiInfo[9], ti9);
                            c_write(type->@base->typeid.itoa());
                            c_write(tiInfo[10], ti10);
                            c_write(type->num_elems.itoa());
                            c_write('}');
                            c_write(',');
                        }
                        break;
                    case TYPE_STRUCT:
                    case TYPE_UNION:
                        gen_typeinfo_header(type->kind == TYPE_STRUCT ? "TYPE_STRUCT".ToPtr() : "TYPE_UNION".ToPtr(), type);
                        c_write(tiInfo[11], ti11);
                        gen_str(type->sym->name, false);
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
                        c_write(type->sym->name);
                        break;
                    default:
                        c_write(tiInfo[17], ti17);
                        break;
                }
            }

        }

        private void gen_all() {
            gen_buf.Clear();
            c_write(forward_includes);
            gen_headers();
            genln();
            genln();
            c_write(preamble.ToPtr(), preamble.Length);
            c_write(forward_declarations);
            gen_forward_decls();
            genln();
            genlnf(sorted_declarations);
            gen_sorted_decls();
            genln();
            c_write("// Typeinfo".ToPtr());
            genln();
            gen_typeinfos();
            genlnf(definitions);
            genln();
            gen_defs();
        }

        private void init_chars() {            
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