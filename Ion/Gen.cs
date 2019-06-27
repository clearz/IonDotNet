using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCross.Memory;

namespace Lang
{
    using static TypeKind;
    using static DeclKind;
    using static TypespecKind;
    using static ExprKind;
    using static StmtKind;
    using static CompoundFieldKind;

    #region Typedefs

#if X64
    using size_t = System.Int64;
#else
    using size_t = System.Int32;
#endif

    #endregion
    unsafe partial class Ion
    {
        // This file is in rapid flux, so there's not much point in reporting bugs yet.

        StringBuilder _gen_buf = new StringBuilder();

        static char* cdecl_buffer = xmalloc<char>(1024 * 1024);
        char* gen_buf = xmalloc<char>(1024 * 1024 * 1024);
        const uint center = (1024 * 1024) / 2;
        uint pos = center;
        uint pre = center;
        uint gen_pos = 0;
        void genf(string str) { /*Console.Write(str);*/ _gen_buf.Append(str); }
        void genf(string fmt, params object[] items) { /*Console.Write(fmt, items);*/ _gen_buf.AppendFormat(fmt, items); }
        void genlnf(string fmt) { genln(); genf(fmt); }
        void genlnf(string fmt, params object[] items) { genln(); genf(fmt, items); }
        void center_pos()
        {
            pos = pre = center;
        }
        char* reset()
        {
            var len = (pos - pre);
            char* c = (char*)xmalloc((size_t)len << 1 + 2);

            //  for(int i=0; i < len;i++) *(c+i) = *(buffer + pre + i);
            Unsafe.CopyBlock(c, cdecl_buffer + pre, len << 1);
            c[len] = '\0';
            pos = pre = center;
            return c;

        }
        uint gen_indent;
        char* line = "                                                                                         ".ToPtr();
        string cdecl = "";

        void indent()
        {
            uint size = gen_indent * 4;
            Unsafe.CopyBlock(gen_buf + gen_pos, line, size << 1);
            gen_pos += size;
        }
        void writeln()
        {
            *(gen_buf + gen_pos++) = '\n';
        }

        void write(char c)
        {
            *(cdecl_buffer + pos++) = c;
        }
        void write(char* c)
        {
            var len = strlen(c);
            Unsafe.CopyBlock(cdecl_buffer + pos, c, len << 1);
            pos += len;
        }
        void write(char* c, uint len)
        {
            Unsafe.CopyBlock(cdecl_buffer + pos, c, len << 1);
            pos += len;
        }

        void pwrite(char c)
        {
            *(gen_buf + gen_pos++) = c;
        }
        void pwrite(char* c)
        {
            while ((*(gen_buf + gen_pos) = *(c++)) != 0) gen_pos++;
        }
        void pwrite(char* c, uint len)
        {
            Unsafe.CopyBlock(gen_buf + gen_pos, c, len << 1);
            gen_pos += len;
        }

        void _genlnf(char* fmt) { _genln(); pwrite(fmt); }

        void _genln()
        {
            writeln();
            indent();
        }
        void genln()
        {
            _gen_buf.Append('\n').Append(' ', (int)gen_indent * 4);
        }


        char* VOID = "void".ToPtr();
        char* CHAR = "char".ToPtr();
        char* INT = "int".ToPtr();
        char* FLOAT = "float".ToPtr();


        char* _cdecl_name(Type* type)
        {
            switch (type->kind)
            {
                case TYPE_VOID:
                    return VOID;
                case TYPE_CHAR:
                    return CHAR;
                case TYPE_INT:
                    return INT;
                case TYPE_FLOAT:
                    return FLOAT;
                case TYPE_STRUCT:
                case TYPE_UNION:
                    return type->sym->name;
                default:
                    assert(false);
                    return null;
                    break;
            }
        }
        void _type_to_cdecl(Type* type, char* str)
        {
            switch (type->kind)
            {
                case TYPE_VOID:
                case TYPE_CHAR:
                case TYPE_INT:
                case TYPE_FLOAT:
                case TYPE_STRUCT:
                case TYPE_UNION:
                    if (str != null)
                    {
                        write(_cdecl_name(type));
                        write(' ');
                        write(str);
                    }
                    else write(_cdecl_name(type));
                    break;
                case TYPE_PTR:
                    if (str != null)
                    {
                        write('(');
                        write('*');
                        write(str);
                        write(')');
                        _type_to_cdecl(type->ptr.elem, reset());
                    }
                    else
                    {
                        //write(_cdecl_name(type->ptr.elem));
                        _type_to_cdecl(type->ptr.elem, null);
                        write(' ');
                        write('*');
                    }
                    break;
                case TYPE_ARRAY:
                    if (str != null)
                    {
                        write('(');
                        write(str);
                        write('[');
                        write(type->array.size.ToString().ToPtr());
                        write(']');
                        write(')');
                        _type_to_cdecl(type->array.elem, reset());
                    }
                    else
                    {
                        _type_to_cdecl(type->ptr.elem, null);
                        write(' ');
                        write('[');
                        write(type->array.size.ToString().ToPtr());
                        write(']');
                    }
                    break;
                case TYPE_FUNC:
                    {
                        if (str != null)
                        {
                            write('(');
                            write('*');
                            write(str);
                            write(')');
                        }
                        write('(');
                        if (type->func.num_params == 0)
                        {
                            write(VOID, 4);
                        }
                        else
                        {
                            for (size_t i = 0; i < type->func.num_params; i++)
                            {
                                if (i > 0)
                                {
                                    write(',');
                                    write(' ');
                                }
                                _type_to_cdecl(type->func.@params[i], null);
                            }

                        }
                        write(')');
                        _type_to_cdecl(type->func.ret, reset());
                    }
                    break;
                default:
                    assert(false);
                    break;
            }
        }



        void _gen_expr_str(Expr* expr)
        {
            var temp = gen_buf;
            var tPos = gen_pos;
            gen_pos = 0;
            gen_buf = cdecl_buffer + pos;
            _gen_expr(expr);
            pos += gen_pos;
            gen_pos = tPos;
            gen_buf = temp;
        }

        private string cdecl_paren(string str, bool b)
        {
            return b ? string.Format("({0})", str) : str;
        }

        private string cdecl_name(Type* type)
        {
            switch (type->kind)
            {
                case TypeKind.TYPE_VOID:
                    return "void";
                case TypeKind.TYPE_CHAR:
                    return "char";
                case TypeKind.TYPE_INT:
                    return "int";
                case TypeKind.TYPE_FLOAT:
                    return "float";
                case TypeKind.TYPE_STRUCT:
                case TypeKind.TYPE_UNION:
                    return new String(type->sym->name);
                default:
                    assert(false);
                    return (string)null;
            }
        }

        private string type_to_cdecl(Type* type, string str)
        {
            switch (type->kind)
            {
                case TypeKind.TYPE_VOID:
                case TypeKind.TYPE_CHAR:
                case TypeKind.TYPE_INT:
                case TypeKind.TYPE_FLOAT:
                case TypeKind.TYPE_STRUCT:
                case TypeKind.TYPE_UNION:
                    return string.Format("{0}{1}{2}", cdecl_name(type), str != null ? " " : "", str);
                case TypeKind.TYPE_PTR:
                    return type_to_cdecl(type->ptr.elem, cdecl_paren(string.Format("*{0}", str), str != null));
                case TypeKind.TYPE_ARRAY:
                    return type_to_cdecl(type->array.elem, cdecl_paren(string.Format("{0}[{1}]", str, type->array.size), str != null));
                case TypeKind.TYPE_FUNC:
                    var str1 = cdecl_paren("*" + str, str != null) + "(";
                    if (type->func.num_params == 0L)
                    {
                        str1 += "void";
                    }
                    else
                    {
                        long index = 0;
                        while (index < type->func.num_params)
                        {
                            str1 += string.Format("{0}{1}", index == 0L ? "" : ", ", type_to_cdecl(type->func.@params[index], null));
                            checked { ++index; }
                        }
                    }
                    str1 += ")";
                    return type_to_cdecl(type->func.ret, str1);
                default:
                    assert(false);
                    return (string)null;
            }
        }

        private string gen_expr_str(Expr* expr)
        {
            string str1 = _gen_buf.ToString();
            _gen_buf.Clear();
            gen_expr(expr);
            string str2 = _gen_buf.ToString();
            _gen_buf.Clear();
            _gen_buf.Append(str1);
            return str2;
        }

        private string typespec_to_cdecl(Typespec* typespec, string str)
        {
            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                    return string.Format("{0}{1}{2}", new string(typespec->name), str != null ? " " : "", str);
                case TYPESPEC_FUNC:
                    var str1 = string.Format("{0}(", cdecl_paren(string.Format("*{0}", str), str != null));
                    if (typespec->func.num_args == 0)
                    {
                        str1 += "void";
                    }
                    else
                    {
                        long index = 0;
                        while (index < (long)typespec->func.num_args)
                        {
                            str1 += string.Format("{0}{1}", index == 0L ? "" : ", ", this.typespec_to_cdecl(typespec->func.args[index], ""));
                            //checked { ++index; }
                        }
                    }
                    str1 += ")";
                    return typespec_to_cdecl(typespec->func.ret, str1);
                case TYPESPEC_ARRAY:
                    return typespec_to_cdecl(typespec->array.elem, cdecl_paren(string.Format("{0}[{1}]", str, this.gen_expr_str(typespec->array.size)), str != null));
                case TYPESPEC_PTR:
                    return typespec_to_cdecl(typespec->ptr.elem, cdecl_paren(string.Format("*{0}", str), str != null));
                default:
                    assert(false);
                    return null;
            }
        }

        void _typespec_to_cdecl(Typespec* typespec, char* str)
        {
            // TODO: Figure out how to handle type vs typespec in C gen for inferred types. How to prevent "flattened" values?
            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                    if (str != null)
                    {
                        write(typespec->name);
                        write(' ');
                        write(str);
                    }
                    else write(typespec->name);
                    break;
                case TYPESPEC_PTR:
                    if (str != null)
                    {
                        write('(');
                        write('*');
                        write(str);
                        write(')');
                        _typespec_to_cdecl(typespec->ptr.elem, reset());
                    }
                    else
                    {
                        write(typespec->name);
                        write(' ');
                        write('*');
                    }
                    break;
                case TYPESPEC_ARRAY:
                    if (str != null)
                    {
                        write('(');
                        write(str);
                        write('[');
                        _gen_expr_str(typespec->array.size);
                        write(']');
                        write(')');
                        _typespec_to_cdecl(typespec->array.elem, reset());
                    }
                    else
                    {
                        write(typespec->name);
                        write(' ');
                        write('[');
                        _gen_expr_str(typespec->array.size);
                        write(']');
                    }
                    break;
                case TYPESPEC_FUNC:
                    {
                        if (str != null)
                        {
                            write('(');
                            write('*');
                            write(str);
                            write(')');
                        }
                        write('(');
                        if (typespec->func.num_args == 0)
                        {
                            write(VOID, 4);
                        }
                        else
                        {
                            for (size_t i = 0; i < typespec->func.num_args; i++)
                            {
                                if (i > 0)
                                {
                                    write(',');
                                    write(' ');
                                }
                                _typespec_to_cdecl(typespec->func.args[i], null);
                            }

                            _typespec_to_cdecl(typespec->func.ret, reset());
                            write(')');
                        }
                        break;
                    }
                default:
                    assert(false);
                    break;

            }
        }
        void _gen_func_decl(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            if (decl->func.ret_type != null)
            {
                _genln();
                center_pos();
                _typespec_to_cdecl(decl->func.ret_type, decl->name);
                pwrite(cdecl_buffer + pre, pos - pre);
                pwrite('(');
            }
            else
            {
                _genln();
                pwrite(VOID);
                pwrite(' ');
                pwrite(decl->name);
                pwrite('(');
            }
            if (decl->func.num_params == 0)
            {
                pwrite(VOID);
            }
            else
            {
                for (size_t i = 0; i < decl->func.num_params; i++)
                {
                    FuncParam* param = decl->func.@params + i;
                    if (i != 0)
                    {
                        pwrite(',');
                        pwrite(' ');
                    }
                    center_pos();
                    _typespec_to_cdecl(param->type, param->name);
                    pwrite(cdecl_buffer + pre, pos - pre);
                }
            }
            pwrite(')');
        }
        void gen_func_decl(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            if (decl->func.ret_type != null)
            {
                genlnf("{0}(", typespec_to_cdecl(decl->func.ret_type, new string(decl->name)));
            }
            else
            {
                genlnf("void {0}(", new string(decl->name));
            }
            if (decl->func.num_params == 0)
            {
                genf("void");
            }
            else
            {
                for (size_t i = 0; i < decl->func.num_params; i++)
                {
                    FuncParam* param = decl->func.@params + i;
                    if (i != 0)
                    {
                        genf(", ");
                    }
                    genf("{0}", typespec_to_cdecl(param->type, new string(param->name)));
                }
            }
            genf(")");
        }
        void _gen_forward_decls()
        {
            for (ulong i = 0; i < global_syms.cap; i++)
            {
                MapEntry* entry = global_syms.entries + i;
                if (entry->key == null)
                {
                    continue;
                }
                Sym* sym = (Sym*)entry->val;
                Decl* decl = sym->decl;
                if (decl == null)
                {
                    continue;
                }
                var name = sym->name;
                switch (decl->kind)
                {
                    case DECL_STRUCT:
                        _genln();
                        pwrite(typedef_keyword);
                        pwrite(' ');
                        pwrite(struct_keyword);
                        pwrite(' ');
                        pwrite(name);
                        pwrite(' ');
                        pwrite(name);
                        pwrite(';');
                        break;
                    case DECL_UNION:
                        _genln();
                        pwrite(typedef_keyword);
                        pwrite(' ');
                        pwrite(union_keyword);
                        pwrite(' ');
                        pwrite(name);
                        pwrite(' ');
                        pwrite(name);
                        pwrite(';');
                        break;
                    case DECL_FUNC:
                        _gen_func_decl(sym->decl);
                        pwrite(';');
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }
        void gen_forward_decls()
        {
            for (ulong i = 0; i < global_syms.cap; i++)
            {
                MapEntry* entry = global_syms.entries + i;
                if (entry->key == null)
                {
                    continue;
                }
                Sym* sym = (Sym*)entry->val;
                Decl* decl = sym->decl;
                if (decl == null)
                {
                    continue;
                }
                var name = new string(sym->name);
                switch (decl->kind)
                {
                    case DECL_STRUCT:
                        genlnf("typedef struct {0} {1};", name, name);
                        break;
                    case DECL_UNION:
                        genlnf("typedef union {0} {1};", name, name);
                        break;
                    case DECL_FUNC:
                        gen_func_decl(sym->decl);
                        genf(";");
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }
        void _gen_aggregate(Decl* decl)
        {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            _genln();
            if (decl->kind == DECL_STRUCT)
                pwrite(struct_keyword);
            else pwrite(union_keyword);
            pwrite(' ');
            pwrite(decl->name);
            pwrite(' ');
            pwrite('{');

            gen_indent++;
            for (size_t i = 0; i < decl->aggregate.num_items; i++)
            {
                AggregateItem item = decl->aggregate.items[i];
                for (size_t j = 0; j < item.num_names; j++)
                {
                    _genln();
                    center_pos();
                    _typespec_to_cdecl(item.type, item.names[0]);
                    pwrite(cdecl_buffer + pre, pos - pre);
                    pwrite(';');
                }
            }
            gen_indent--;
            _genln();
            pwrite('}'); pwrite(';');
        }
        void gen_aggregate(Decl* decl)
        {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            genlnf("{0} {1} {{", decl->kind == DECL_STRUCT ? "struct" : "union", new string(decl->name));
            gen_indent++;
            for (size_t i = 0; i < decl->aggregate.num_items; i++)
            {
                AggregateItem item = decl->aggregate.items[i];
                for (size_t j = 0; j < item.num_names; j++)
                    genlnf("{0};", typespec_to_cdecl(item.type, new string(item.names[0])));
            }
            gen_indent--;
            genlnf("};");
        }

        void _gen_str(char* str)
        {
            // TODO: proper quoted string escaping
            pwrite('"');
            pwrite(str);
            pwrite('"');
        }
        void gen_str(string str)
        {
            // TODO: proper quoted string escaping
            genf("\"{0}\"", str);
        }
        void _gen_expr(Expr* expr)
        {
            switch (expr->kind)
            {
                case EXPR_INT:
                    pwrite(expr->int_val.ToString().ToPtr());
                    break;
                case EXPR_FLOAT:
                    pwrite(expr->float_val.ToString().ToPtr());
                    break;
                case EXPR_STR:
                    pwrite('\"');
                    pwrite(expr->str_val);
                    pwrite('\"');
                    break;
                case EXPR_NAME:
                    pwrite(expr->name);
                    break;
                case EXPR_CAST:
                    pwrite('(');
                    center_pos();
                    _type_to_cdecl(expr->cast.type->type, null);
                    pwrite(cdecl_buffer + pre, pos - pre);
                    pwrite(')');
                    pwrite('(');
                    _gen_expr(expr->cast.expr);
                    pwrite(')');
                    break;
                case EXPR_CALL:
                    _gen_expr(expr->call.expr);
                    pwrite('(');
                    for (size_t i = 0; i < expr->call.num_args; i++)
                    {
                        if (i != 0)
                        {
                            pwrite(',');
                            pwrite(' ');
                        }
                        _gen_expr(expr->call.args[i]);
                    }
                    pwrite(')');
                    break;
                case EXPR_INDEX:
                    _gen_expr(expr->index.expr);
                    pwrite('[');
                    _gen_expr(expr->index.index);
                    pwrite(']');
                    break;
                case EXPR_FIELD:
                    _gen_expr(expr->field.expr);
                    pwrite('.');
                    pwrite(expr->field.name);
                    break;
                case EXPR_COMPOUND:
                    if (expr->compound.type != null)
                    {
                        pwrite('(');
                        center_pos();
                        _typespec_to_cdecl(expr->compound.type, null);
                        pwrite(cdecl_buffer + pre, pos - pre);
                        pwrite(')');
                        pwrite('{');
                    }
                    else
                    {
                        pwrite('(');
                        center_pos();
                        _type_to_cdecl(expr->type, null);
                        pwrite(cdecl_buffer + pre, pos - pre);
                        pwrite(')');
                        pwrite('{');
                    }
                    for (size_t i = 0; i < expr->compound.num_fields; i++)
                    {
                        if (i != 0)
                        {
                            pwrite(',');
                            pwrite(' ');
                        }
                        CompoundField* field = expr->compound.fields + i;
                        if (field->kind == FIELD_NAME)
                        {
                            pwrite('.');
                            pwrite(field->name);
                            pwrite(' ');
                            pwrite('=');
                            pwrite(' ');
                        }
                        else if (field->kind == FIELD_INDEX)
                        {
                            pwrite('[');
                            _gen_expr(field->index);
                            pwrite(']');
                            pwrite(' ');
                            pwrite('=');
                            pwrite(' ');
                        }
                        _gen_expr(field->init);
                    }
                    pwrite('}');
                    break;
                case EXPR_UNARY:
                    pwrite(_token_kind_name(expr->unary.op));
                    pwrite('(');
                    _gen_expr(expr->unary.expr);
                    pwrite(')');
                    break;
                case EXPR_BINARY:
                    pwrite('(');
                    _gen_expr(expr->binary.left);
                    pwrite(')');
                    pwrite(' ');
                    pwrite(_token_kind_name(expr->binary.op));
                    pwrite(' ');
                    pwrite('(');
                    _gen_expr(expr->binary.right);
                    pwrite(')');
                    break;
                case EXPR_TERNARY:
                    pwrite('(');
                    _gen_expr(expr->ternary.cond);
                    pwrite(' ');
                    pwrite('?');
                    pwrite(' ');
                    _gen_expr(expr->ternary.then_expr);
                    pwrite(' ');
                    pwrite(':');
                    pwrite(' ');
                    _gen_expr(expr->ternary.else_expr);
                    pwrite(')');
                    break;
                case EXPR_SIZEOF_EXPR:
                    pwrite(sizeof_keyword);
                    pwrite('(');
                    _gen_expr(expr->sizeof_expr);
                    pwrite(')');
                    break;
                case EXPR_SIZEOF_TYPE:
                    pwrite(sizeof_keyword);
                    pwrite('(');
                    center_pos();
                    _type_to_cdecl(expr->sizeof_type->type, null);
                    pwrite(cdecl_buffer + pre, pos - pre);
                    pwrite(')');
                    break;
                default:
                    assert(false);
                    break;
            }
        }
        void gen_expr(Expr* expr)
        {
            switch (expr->kind)
            {
                case EXPR_INT:
                    genf("{0}", expr->int_val);
                    break;
                case EXPR_FLOAT:
                    genf("{0}", expr->float_val);
                    break;
                case EXPR_STR:
                    gen_str(new string(expr->str_val));
                    break;
                case EXPR_NAME:
                    genf("{0}", new string(expr->name));
                    break;
                case EXPR_CAST:
                    genf("({0})(", type_to_cdecl(expr->cast.type->type, ""));
                    gen_expr(expr->cast.expr);
                    genf(")");
                    break;
                case EXPR_CALL:
                    gen_expr(expr->call.expr);
                    genf("(");
                    for (size_t i = 0; i < expr->call.num_args; i++)
                    {
                        if (i != 0)
                        {
                            genf(", ");
                        }
                        gen_expr(expr->call.args[i]);
                    }
                    genf(")");
                    break;
                case EXPR_INDEX:
                    gen_expr(expr->index.expr);
                    genf("[");
                    gen_expr(expr->index.index);
                    genf("]");
                    break;
                case EXPR_FIELD:
                    gen_expr(expr->field.expr);
                    genf(".{0}", new string(expr->field.name));
                    break;
                case EXPR_COMPOUND:
                    if (expr->compound.type != null)
                    {
                        genf("({0}){{", typespec_to_cdecl(expr->compound.type, ""));
                    }
                    else
                    {
                        genf("({0}){{", type_to_cdecl(expr->type, ""));
                    }
                    for (size_t i = 0; i < expr->compound.num_fields; i++)
                    {
                        if (i != 0)
                        {
                            genf(", ");
                        }
                        CompoundField* field = expr->compound.fields + i;
                        if (field->kind == FIELD_NAME)
                        {
                            genf(".{0} = ", new string(field->name));
                        }
                        else if (field->kind == FIELD_INDEX)
                        {
                            genf("[");
                            gen_expr(field->index);
                            genf("] = ");
                        }
                        gen_expr(field->init);
                    }
                    genf("}");
                    break;
                case EXPR_UNARY:
                    genf("{0}(", token_kind_name(expr->unary.op));
                    gen_expr(expr->unary.expr);
                    genf(")");
                    break;
                case EXPR_BINARY:
                    genf("(");
                    gen_expr(expr->binary.left);
                    genf(") {0} (", token_kind_name(expr->binary.op).ToString());
                    gen_expr(expr->binary.right);
                    genf(")");
                    break;
                case EXPR_TERNARY:
                    genf("(");
                    gen_expr(expr->ternary.cond);
                    genf(" ? ");
                    gen_expr(expr->ternary.then_expr);
                    genf(" : ");
                    gen_expr(expr->ternary.else_expr);
                    genf(")");
                    break;
                case EXPR_SIZEOF_EXPR:
                    genf("sizeof(");
                    gen_expr(expr->sizeof_expr);
                    genf(")");
                    break;
                case EXPR_SIZEOF_TYPE:
                    genf("sizeof({0})", type_to_cdecl(expr->sizeof_type->type, ""));
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        void gen_stmt_block(StmtList block)
        {
            genf("{");
            gen_indent++;
            for (size_t i = 0; i < block.num_stmts; i++)
            {
                gen_stmt(block.stmts[i]);
            }
            gen_indent--;
            genlnf("}");
        }

        void _gen_stmt_block(StmtList block)
        {
            pwrite('{');
            gen_indent++;
            for (size_t i = 0; i < block.num_stmts; i++)
            {
                _gen_stmt(block.stmts[i]);
            }
            gen_indent--;
            _genln();
            pwrite('}');
        }
        void _gen_simple_stmt(Stmt* stmt)
        {
            switch (stmt->kind)
            {
                case STMT_EXPR:
                    _gen_expr(stmt->expr);
                    break;
                case STMT_INIT:
                    center_pos();
                    _type_to_cdecl(stmt->init.expr->type, stmt->init.name);
                    pwrite(cdecl_buffer + pre, pos - pre);
                    pwrite(' ');
                    pwrite('=');
                    pwrite(' ');
                    _gen_expr(stmt->init.expr);
                    break;
                case STMT_ASSIGN:
                    _gen_expr(stmt->assign.left);
                    if (stmt->assign.right != null)
                    {
                        pwrite(' ');
                        pwrite(_token_kind_name(stmt->assign.op));
                        pwrite(' ');
                        _gen_expr(stmt->assign.right);
                    }
                    else
                    {
                        pwrite(_token_kind_name(stmt->assign.op));
                    }
                    break;
                default:
                    assert(false);
                    break;
            }
        }
        void gen_simple_stmt(Stmt* stmt)
        {
            switch (stmt->kind)
            {
                case STMT_EXPR:
                    gen_expr(stmt->expr);
                    break;
                case STMT_INIT:
                    genf("{0} = ", type_to_cdecl(stmt->init.expr->type, new string(stmt->init.name)));
                    gen_expr(stmt->init.expr);
                    break;
                case STMT_ASSIGN:
                    gen_expr(stmt->assign.left);
                    if (stmt->assign.right != null)
                    {
                        genf(" {0} ", token_kind_name(stmt->assign.op));
                        gen_expr(stmt->assign.right);
                    }
                    else
                    {
                        genf("{0}", token_kind_name(stmt->assign.op));
                    }
                    break;
                default:
                    assert(false);
                    break;
            }
        }
        void _gen_stmt(Stmt* stmt)
        {
            switch (stmt->kind)
            {
                case STMT_RETURN:
                    _genlnf(return_keyword);
                    if (stmt->expr != null)
                    {
                        pwrite(' ');
                        _gen_expr(stmt->expr);
                    }
                    pwrite(';');
                    break;
                case STMT_BREAK:
                    _genlnf(break_keyword);
                    pwrite(';');
                    break;
                case STMT_CONTINUE:
                    _genlnf(continue_keyword);
                    pwrite(';');
                    break;
                case STMT_BLOCK:
                    _genln();
                    _gen_stmt_block(stmt->block);
                    break;
                case STMT_IF:
                    _genlnf(if_keyword);
                    pwrite(' ');
                    pwrite('(');
                    _gen_expr(stmt->if_stmt.cond);
                    pwrite(')');
                    pwrite(' ');
                    _gen_stmt_block(stmt->if_stmt.then_block);
                    for (size_t i = 0; i < stmt->if_stmt.num_elseifs; i++)
                    {
                        ElseIf elseif = stmt->if_stmt.elseifs[i];
                        pwrite(' ');
                        pwrite(else_keyword);
                        pwrite(' ');
                        pwrite(if_keyword);
                        pwrite(' ');
                        pwrite('(');
                        _gen_expr(elseif.cond);
                        pwrite(')');
                        pwrite(' ');
                        _gen_stmt_block(elseif.block);
                    }
                    if (stmt->if_stmt.else_block.stmts != null)
                    {
                        pwrite(' ');
                        pwrite(else_keyword);
                        pwrite(' ');
                        _gen_stmt_block(stmt->if_stmt.else_block);
                    }
                    break;
                case STMT_WHILE:
                    pwrite(while_keyword);
                    pwrite(' ');
                    pwrite('(');
                    _gen_expr(stmt->while_stmt.cond);
                    pwrite(')');
                    pwrite(' ');
                    _gen_stmt_block(stmt->while_stmt.block);
                    break;
                case STMT_DO_WHILE:
                    pwrite(do_keyword);
                    pwrite(' ');
                    _gen_stmt_block(stmt->while_stmt.block);
                    pwrite(' ');
                    pwrite(while_keyword);
                    pwrite(' ');
                    pwrite('(');
                    _gen_expr(stmt->while_stmt.cond);
                    pwrite(')');
                    pwrite(';');
                    break;
                case STMT_FOR:
                    _genlnf(for_keyword);
                    pwrite(' ');
                    pwrite('(');
                    if (stmt->for_stmt.init != null)
                    {
                        _gen_simple_stmt(stmt->for_stmt.init);
                    }
                    pwrite(';');
                    if (stmt->for_stmt.cond != null)
                    {
                        pwrite(' ');
                        _gen_expr(stmt->for_stmt.cond);
                    }
                    pwrite(';');
                    if (stmt->for_stmt.next != null)
                    {
                        pwrite(' ');
                        _gen_simple_stmt(stmt->for_stmt.next);
                    }
                    pwrite(')');
                    pwrite(' ');
                    _gen_stmt_block(stmt->for_stmt.block);
                    break;
                case STMT_SWITCH:
                    _genlnf(switch_keyword);
                    pwrite(' ');
                    pwrite('(');
                    _gen_expr(stmt->switch_stmt.expr);
                    pwrite(')');
                    pwrite(' ');
                    pwrite(';');
                    for (size_t i = 0; i < stmt->switch_stmt.num_cases; i++)
                    {
                        SwitchCase switch_case = stmt->switch_stmt.cases[i];
                        for (size_t j = 0; j < switch_case.num_exprs; j++)
                        {
                            _genlnf(case_keyword);
                            pwrite(' ');
                            _gen_expr(switch_case.exprs[j]);
                            pwrite(':');

                        }
                        if (switch_case.is_default)
                        {
                            pwrite(default_keyword);
                            pwrite(':');
                        }
                        pwrite(' ');
                        _gen_stmt_block(switch_case.block);
                    }
                    _genln();
                    pwrite('}');
                    break;
                default:
                    _genln();
                    _gen_simple_stmt(stmt);
                    pwrite(';');
                    break;
            }
        }

        void gen_stmt(Stmt* stmt)
        {
            switch (stmt->kind)
            {
                case STMT_RETURN:
                    genlnf("return");
                    if (stmt->expr != null)
                    {
                        genf(" ");
                        gen_expr(stmt->expr);
                    }
                    genf(";");
                    break;
                case STMT_BREAK:
                    genlnf("break;");
                    break;
                case STMT_CONTINUE:
                    genlnf("continue;");
                    break;
                case STMT_BLOCK:
                    genln();
                    gen_stmt_block(stmt->block);
                    break;
                case STMT_IF:
                    genlnf("if (");
                    gen_expr(stmt->if_stmt.cond);
                    genf(") ");
                    gen_stmt_block(stmt->if_stmt.then_block);
                    for (size_t i = 0; i < stmt->if_stmt.num_elseifs; i++)
                    {
                        ElseIf elseif = stmt->if_stmt.elseifs[i];
                        genf(" else if (");
                        gen_expr(elseif.cond);
                        genf(") ");
                        gen_stmt_block(elseif.block);
                    }
                    if (stmt->if_stmt.else_block.stmts != null)
                    {
                        genf(" else ");
                        gen_stmt_block(stmt->if_stmt.else_block);
                    }
                    break;
                case STMT_WHILE:
                    genlnf("while (");
                    gen_expr(stmt->while_stmt.cond);
                    genf(") ");
                    gen_stmt_block(stmt->while_stmt.block);
                    break;
                case STMT_DO_WHILE:
                    genlnf("do ");
                    gen_stmt_block(stmt->while_stmt.block);
                    genf(" while (");
                    gen_expr(stmt->while_stmt.cond);
                    genf(");");
                    break;
                case STMT_FOR:
                    genlnf("for (");
                    if (stmt->for_stmt.init != null)
                    {
                        gen_simple_stmt(stmt->for_stmt.init);
                    }
                    genf(";");
                    if (stmt->for_stmt.cond != null)
                    {
                        genf(" ");
                        gen_expr(stmt->for_stmt.cond);
                    }
                    genf(";");
                    if (stmt->for_stmt.next != null)
                    {
                        genf(" ");
                        gen_simple_stmt(stmt->for_stmt.next);
                    }
                    genf(") ");
                    gen_stmt_block(stmt->for_stmt.block);
                    break;
                case STMT_SWITCH:
                    genlnf("switch (");
                    gen_expr(stmt->switch_stmt.expr);
                    genf(") {");
                    for (size_t i = 0; i < stmt->switch_stmt.num_cases; i++)
                    {
                        SwitchCase switch_case = stmt->switch_stmt.cases[i];
                        for (size_t j = 0; j < switch_case.num_exprs; j++)
                        {
                            genlnf("case ");
                            gen_expr(switch_case.exprs[j]);
                            genf(":");

                        }
                        if (switch_case.is_default)
                        {
                            genlnf("default:");
                        }
                        genf(" ");
                        gen_stmt_block(switch_case.block);
                    }
                    genlnf("}");
                    break;
                default:
                    genln();
                    gen_simple_stmt(stmt);
                    genf(";");
                    break;
            }
        }
        void _gen_func(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            _gen_func_decl(decl);
            pwrite(' ');
            _gen_stmt_block(decl->func.block);
        }
        void gen_func(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            gen_func_decl(decl);
            pwrite(' ');
            gen_stmt_block(decl->func.block);
        }

        void _gen_sym(Sym* sym)
        {
            Decl* decl = sym->decl;
            if (decl == null)
            {
                return;
            }
            switch (decl->kind)
            {
                case DECL_CONST:
                    _genln();
                    pwrite(enum_keyword);
                    pwrite(' '); pwrite('{'); pwrite(' ');
                    pwrite(sym->name);
                    pwrite(' '); pwrite('='); pwrite(' ');
                    _gen_expr(decl->const_decl.expr);
                    pwrite(' '); pwrite('}'); pwrite(';');
                    break;
                case DECL_VAR:
                    if (decl->var.type != null)
                    {
                        _genln();
                        center_pos();
                        _typespec_to_cdecl(decl->var.type, sym->name);
                        pwrite(cdecl_buffer + pre, pos - pre);
                    }
                    else
                    {
                        _genln();
                        center_pos();
                        _type_to_cdecl(sym->type, sym->name);
                        pwrite(cdecl_buffer + pre, pos - pre);
                    }
                    if (decl->var.expr != null)
                    {
                        pwrite(' '); pwrite('='); pwrite(' ');
                        _gen_expr(decl->var.expr);
                    }
                    pwrite(';');
                    break;
                case DECL_FUNC:
                    _gen_func(sym->decl);
                    break;
                case DECL_STRUCT:
                case DECL_UNION:
                    _gen_aggregate(sym->decl);
                    break;
                case DECL_TYPEDEF:
                    _genln();
                    pwrite(typedef_keyword);
                    pwrite(' ');
                    pwrite(' ');
                    center_pos();
                    _type_to_cdecl(sym->type, sym->name);
                    pwrite(cdecl_buffer + pre, pos - pre);
                    pwrite(';');
                    break;
                default:
                    assert(false);
                    break;
            }
        }
        void gen_sym(Sym* sym)
        {
            Decl* decl = sym->decl;
            if (decl == null)
            {
                return;
            }
            switch (decl->kind)
            {
                case DECL_CONST:
                    genlnf("enum {{ {0} = ", new string(sym->name));
                    gen_expr(decl->const_decl.expr);
                    genf(" };");
                    break;
                case DECL_VAR:
                    if (decl->var.type != null)
                    {
                        genlnf("{0}", typespec_to_cdecl(decl->var.type, new string(sym->name)));
                    }
                    else
                    {
                        genlnf("{0}", type_to_cdecl(sym->type, new string(sym->name)));
                    }
                    if (decl->var.expr != null)
                    {
                        genf(" = ");
                        gen_expr(decl->var.expr);
                    }
                    genf(";");
                    break;
                case DECL_FUNC:
                    gen_func(sym->decl);
                    break;
                case DECL_STRUCT:
                case DECL_UNION:
                    gen_aggregate(sym->decl);
                    break;
                case DECL_TYPEDEF:
                    genlnf("typedef {0};", type_to_cdecl(sym->type, new string(sym->name)));
                    break;
                default:
                    assert(false);
                    break;
            }
        }
        void _gen_ordered_decls()
        {
            var sym_cnt = ordered_syms.count;

            for (int i = 0; i < sym_cnt; i++)
            {
                //Console.Write($"\r{i} of {sym_cnt}    ");
                Sym* sym = (ordered_syms._begin + i);
                _gen_sym(sym);
            }
        }
        void gen_ordered_decls()
        {
            var sym_cnt = ordered_syms.count;

            for (int i = 0; i < sym_cnt; i++)
            {
                //Console.Write($"\r{i} of {sym_cnt}    ");
                Sym* sym = (ordered_syms._begin + i);
                gen_sym(sym);
            }
        }

        void cdecl_test()
        {

            string _cdecl1 = type_to_cdecl(type_int, "x");
            Console.WriteLine(_cdecl1);
            string _cdecl2 = type_to_cdecl(type_ptr(type_int), "x");
            Console.WriteLine(_cdecl2);
            string _cdecl3 = type_to_cdecl(type_array(type_int, 10), "x");
            Console.WriteLine(_cdecl3);
            string _cdecl4 = type_to_cdecl(type_func(new Type*[] { type_int }, 1, type_int), "x");
            Console.WriteLine(_cdecl4);
            string _cdecl5 = type_to_cdecl(type_array(type_func(new Type*[] { type_int }, 1, type_int), 10), "x");
            Console.WriteLine(_cdecl5);
            string _cdecl6 = type_to_cdecl(type_func(new Type*[] { type_ptr(type_int) }, 1, type_int), "x");
            Console.WriteLine(_cdecl6);
            Type* _type1 = type_func(new Type*[] { type_array(type_int, 10) }, 1, type_int);
            string _cdecl7 = type_to_cdecl(_type1, "x");
            Console.WriteLine(_cdecl7);
            string _cdecl8 = type_to_cdecl(type_func((Type**)null, 0, _type1), "x");
            Console.WriteLine(_cdecl8);
            string _cdecl9 = type_to_cdecl(type_func((Type**)null, 0, type_array(type_func((Type**)null, 0, type_int), 10)), "x");
            Console.WriteLine(_cdecl9);

        }
        void cdecl_test2()
        {
            char c = 'x';
            pre = pos = center;
            _type_to_cdecl(type_int, &c);
            var str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_ptr(type_int), &c);
            str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_array(type_int, 10), &c);
            str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_func(new Type*[] { type_int }, 1, type_int), &c);
            str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_array(type_func(new Type*[] { type_int }, 1, type_int), 10), &c);
            str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_func(new Type*[] { type_ptr(type_int) }, 1, type_int), &c);
            str = new String(cdecl_buffer, (int)pre, (int)(pos - pre));
            Console.WriteLine(str);
            pre = pos = center;
            Type* type1 = type_func(new Type*[] { type_array(type_int, 10) }, 1, type_int);
            _type_to_cdecl(type1, &c);
            str = new String(reset());
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_func((Type**)null, 0, type1), &c);
            str = new String(reset());
            Console.WriteLine(str);
            pre = pos = center;
            _type_to_cdecl(type_func((Type**)null, 0, type_array(type_func((Type**)null, 0, type_int), 10)), &c);
            str = new String(reset());
            Console.WriteLine(str);

        }

        char* fd = "// Forward declarations".ToPtr();
        char* od = "// Ordered declarations".ToPtr();

        void gen_all()
        {
            _gen_buf.Clear();
            genf("// Forward declarations");
            gen_forward_decls();
            genln();
            genlnf("// Ordered declarations");
            gen_ordered_decls();
        }
        void _gen_all()
        {
            pos = pre = center;
            pwrite(fd);
            _gen_forward_decls();
            _genln();
            _genln();
            pwrite(od);
            _gen_ordered_decls();
        }
        string code5 =

            "func example_test(): int { return fact_rec(10) == fact_iter(10); }\n" +
        "union IntOrPtr { i: int; p: int*; }\n" +

        "var i: int\n" +
        "struct Vector { x: int; y: int; }\n" +
        "func fact_iter(n: int): int { r := 1; for (i := 2; i <= n; i++) { r *= i; } return r; }\n" +
        "func fact_rec(n: int): int { if (n == 0) { return 1; } else { return n * fact_rec(n-1); } }\n" +
            /*
           "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }\n"
           "func f2(n: int): int { return 2*n; }\n"
           "func f3(x: int): int { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }\n"
           "func f4(n: int): int { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }\n"
           "func f5(x: int): int { switch(x) { case 0: case 1: return 42; case 3: default: return -1; } }\n"
           "func f6(n: int): int { p := 1; while (n) { p *= 2; n--; } return p; }\n"
           "func f7(n: int): int { p := 1; do { p *= 2; n--; } while (n); return p; }\n"
            */
          "const z = 1+sizeof(p)\n" +
          "var p: Tso*\n" +
            "struct Tso { a: int[z + 3]; }\n";

        internal void gen_test()
        {
            //cdecl_test();
            //cdecl_test2();


            local_syms.clear();
            init_stream(code5, null);
            init_global_syms();
            var f = parse_file();
            sym_global_decls(f);
            finalize_syms();

            _gen_all();
            Console.WriteLine(new string(gen_buf));
           // gen_all();
            //Console.WriteLine(_gen_buf.ToString());

            /*
                extern int example_test();
                printf("example_test() == %d\n", example_test());
            */
        }

        /*
        // Forward declarations
        int example_test();
        typedef union IntOrPtr IntOrPtr;
        void f();
        typedef struct Vector Vector;
        int fact_iter(int n);
        int fact_rec(int n);
        typedef struct T T;

        // Ordered declarations
        int example_test() {
            return (fact_rec(10)) == (fact_iter(10));
        }
        int fact_rec(int n) {
            if ((n) == (0)) {
                return 1;
            } else {
                return (n) * (fact_rec((n) - (1)));
            }
        }
        int fact_iter(int n) {
            int r = 1;
            for (int i = 2; (i) <= (n); i++) {
                r *= i;
            }
            return r;
        }
        union IntOrPtr {
            int i;
            int (*p);
        };
        void f() {
            IntOrPtr u1 = (IntOrPtr){.i = 42};
            IntOrPtr u2 = (IntOrPtr){.p = (int *)(42)};
            u1.i = 0;
            u2.p = (int *)(0);
        }
        int i;
        struct Vector {
            int x;
            int y;
        };
        T (*p);
        enum { n = (1) + (sizeof(p)) };
        struct T {
            int (a[n]);
        };
        */

    }

}