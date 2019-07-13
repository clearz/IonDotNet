using System;
using System.Runtime.CompilerServices;
using DotNetCross.Memory;

namespace Lang
{
    using static CompoundFieldKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static TypeKind;
    using static TypespecKind;


    unsafe partial class Ion
    {
        private const int _1MB = 1024 * 1024;


        private static readonly char* line =
            "                                                                                         ".ToPtr();

        private readonly char* cdecl_buffer = xmalloc<char>(_1MB);
        private readonly char* CHAR = "char".ToPtr();
        private readonly char[] char_to_escape = new char[256];
        private readonly char* FLOAT = "float".ToPtr();
        private readonly char* gen_preamble = "#include <stdio.h>\n\n".ToPtr();
        private readonly char* INT = "int".ToPtr();

        private readonly char* lineStr = "#line ".ToPtr();

        private readonly char* VOID = "void".ToPtr();
        private SrcPos _genPos;
        private int _pos, _gen_indent;

        private readonly string code5 = "struct Vector { x, y: int; }\n" +
/*
            "func example_test(): int { return fact_rec(10) == fact_iter(10); }\n" +
        "union IntOrPtr { i: int; p: int*; }\n" +
                "// func f() {\n"+
        "//     u1 := IntOrPtr{i = 42};\n"+
        "//     u2 := IntOrPtr{p = cast(int*, 42)};\n"+
        "//     u2 := IntOrPtr{p = (:int*)42};\n"+
        "//     u1.i = 0;\n"+
        "//     u2.p = cast(int*, 0);\n"+
        "//     u2.p = (:int*)0;\n"+
        "// }\n" +
        "var i: int\n" +
        "struct Vector { x: int; y: int; }\n" +
        "func fact_iter(n: int): int { r := 1; for (i := 2; i <= n; i++) { r *= i; } return r; }\n" +
        "func fact_rec(n: int): int { if (n == 0) { return 1; } else { return n * fact_rec(n-1); } }\n" +
          
         "func f1() { v := Vector{1, 2}; j := i; i++; j++; v.x = 2*j; }\n"
         "func f2(n: long): long { return 2*n; }\n"
         "func f3(x: long): long { if (x) { return -x; } else if (x % 2 == 0) { return 42; } else { return -1; } }\n"
         "func f4(n: long): long { for (i := 0; i < n; i++) { if (i % 3 == 0) { return n; } } return 0; }\n"
         "func f5(x: long): long { switch(x) { case 0: case 1: return 42; case 3: default: return -1; } }\n"
         "func f6(n: long): long { p := 1; while (n) { p *= 2; n--; } return p; }\n"
         "func f7(n: long): long { p := 1; do { p *= 2; n--; } while (n); return p; }\n"
        */
                                        "const z = 1+sizeof(p)\n" +
                                        "var p: Tso*\n" +
                                        "struct Tso { a: int[z + 3]; }\n";


        private readonly char* fd = "// Forward declarations".ToPtr();
        private Buffer<char> gen_buf = Buffer<char>.Create(_1MB, 4);
        private readonly char* od = "// Ordered declarations".ToPtr();

        private void reset_pos()
        {
            _pos = 0;
        }

        private char* copy_buf()
        {
            var len = _pos;
            var c = (char*) xmalloc(len << (1 + 2));

            Unsafe.CopyBlock(c, cdecl_buffer, (uint) len << 1);
            c[len] = '\0';
            _pos = 0;
            return c;
        }

        private void indent()
        {
            var size = _gen_indent * 4;

            gen_buf.Add(line, size);
            _genPos.line++;
            // Unsafe.CopyBlock(gen_buf + _genPos, line, (uint)size << 1);
            //_genPos += size;
        }

        private void writeln()
        {
            gen_buf.Add('\n');
        }

        private void c_write(char c)
        {
            *(cdecl_buffer + _pos++) = c;
        }

        private void c_write(char* c)
        {
            var len = strlen(c);
            Unsafe.CopyBlock(cdecl_buffer + _pos, c, (uint) len << 1);
            _pos += len;
        }

        private void c_write(char* c, int len)
        {
            Unsafe.CopyBlock(cdecl_buffer + _pos, c, (uint) len << 1);
            _pos += len;
        }

        private void buf_write(char c)
        {
            gen_buf.Add(c);
        }

        private void buf_write(char* c)
        {
            gen_buf.Add(c, strlen(c));
            //while ((*(gen_buf + _genPos) = *(c++)) != 0) _genPos++;
        }

        private void buf_write(char* c, int len)
        {
            gen_buf.Add(c, len);
            //Unsafe.CopyBlock(gen_buf + _genPos, c, (uint)len << 1);
            //_genPos += len;
        }

        private void _genlnf(char* fmt)
        {
            _genln();
            buf_write(fmt);
        }

        private void _genln()
        {
            writeln();
            indent();
            _genPos.line++;
        }


        private char* _cdecl_name(Type* type)
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
            }
        }

        private void _type_to_cdecl(Type* type, char* str)
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
                        c_write(_cdecl_name(type));
                        c_write(' ');
                        c_write(str);
                    }
                    else
                    {
                        c_write(_cdecl_name(type));
                    }

                    break;
                case TYPE_PTR:
                    if (str != null)
                    {
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                        _type_to_cdecl(type->ptr.elem, copy_buf());
                    }
                    else
                    {
                        //c_write(_cdecl_name(type->ptr.elem));
                        _type_to_cdecl(type->ptr.elem, null);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPE_ARRAY:
                    if (str != null)
                    {
                        c_write('(');
                        c_write(str);
                        c_write('[');
                        c_write(type->array.size.ToString().ToPtr());
                        c_write(']');
                        c_write(')');
                        _type_to_cdecl(type->array.elem, copy_buf());
                    }
                    else
                    {
                        _type_to_cdecl(type->ptr.elem, null);
                        c_write(' ');
                        c_write('[');
                        c_write(type->array.size.ToString().ToPtr());
                        c_write(']');
                    }

                    break;
                case TYPE_FUNC:
                {
                    if (str != null)
                    {
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }

                    c_write('(');
                    if (type->func.num_params == 0)
                        c_write(VOID, 4);
                    else
                        for (var i = 0; i < type->func.num_params; i++)
                        {
                            if (i > 0)
                            {
                                c_write(',');
                                c_write(' ');
                            }

                            _type_to_cdecl(type->func.@params[i], null);
                        }

                    c_write(')');
                    _type_to_cdecl(type->func.ret, copy_buf());
                }
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void _gen_str(char* str)
        {
            buf_write('\"');
            while (*str != 0)
            {
                var start = str;
                while (*str != 0 && char_to_escape[*str] == 0) str++;
                if (start != str) buf_write(start, (int) (str - start));
                if (*str != 0 && char_to_escape[*str] != 0)
                {
                    buf_write('\\');
                    buf_write(char_to_escape[*str]);
                    str++;
                }
            }

            buf_write('\"');
        }

        private void gen_sync_pos(SrcPos pos)
        {
            assert(pos.name != null && pos.line != 0);
            if (_genPos.line != pos.line || _genPos.name != pos.name)
            {
                _genln();
                buf_write(lineStr, 6);
                buf_write(pos.line.ToString().ToPtr());
                buf_write(' ');
                buf_write(pos.name);
                _genPos = pos;
            }
        }

        private void _gen_expr_str(Expr* expr)
        {
            var temp = gen_buf;
            var tPos = gen_buf.count;
            gen_buf = Buffer<char>.Create(1024);
            _gen_expr(expr);
            c_write(gen_buf._begin, gen_buf.count);
            gen_buf = temp;
        }


        private void _typespec_to_cdecl(Typespec* typespec, char* str)
        {
            // TODO: Figure out how to handle type vs typespec in C gen for inferred types. How to prevent "flattened" values?
            switch (typespec->kind)
            {
                case TYPESPEC_NAME:
                    if (str != null)
                    {
                        c_write(typespec->name);
                        c_write(' ');
                        c_write(str);
                    }
                    else
                    {
                        c_write(typespec->name);
                    }

                    break;
                case TYPESPEC_PTR:
                    if (str != null)
                    {
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                        _typespec_to_cdecl(typespec->ptr.elem, copy_buf());
                    }
                    else
                    {
                        c_write(typespec->name);
                        c_write(' ');
                        c_write('*');
                    }

                    break;
                case TYPESPEC_ARRAY:
                    if (str != null)
                    {
                        c_write('(');
                        c_write(str);
                        c_write('[');
                        _gen_expr_str(typespec->array.size);
                        c_write(']');
                        c_write(')');
                        _typespec_to_cdecl(typespec->array.elem, copy_buf());
                    }
                    else
                    {
                        c_write(typespec->name);
                        c_write(' ');
                        c_write('[');
                        _gen_expr_str(typespec->array.size);
                        c_write(']');
                    }

                    break;
                case TYPESPEC_FUNC:
                {
                    if (str != null)
                    {
                        c_write('(');
                        c_write('*');
                        c_write(str);
                        c_write(')');
                    }

                    c_write('(');
                    if (typespec->func.num_args == 0)
                    {
                        c_write(VOID, 4);
                    }
                    else
                    {
                        for (var i = 0; i < typespec->func.num_args; i++)
                        {
                            if (i > 0)
                            {
                                c_write(',');
                                c_write(' ');
                            }

                            _typespec_to_cdecl(typespec->func.args[i], null);
                        }

                        _typespec_to_cdecl(typespec->func.ret, copy_buf());
                        c_write(')');
                    }

                    break;
                }

                default:
                    assert(false);
                    break;
            }
        }

        private void _gen_func_decl(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);

            gen_sync_pos(decl->pos);
            if (decl->func.ret_type != null)
            {
                _genln();
                reset_pos();
                _typespec_to_cdecl(decl->func.ret_type, decl->name);
                buf_write(cdecl_buffer, _pos);
                buf_write('(');
            }
            else
            {
                _genln();
                buf_write(VOID);
                buf_write(' ');
                buf_write(decl->name);
                buf_write('(');
            }

            if (decl->func.num_params == 0)
                buf_write(VOID);
            else
                for (var i = 0; i < decl->func.num_params; i++)
                {
                    var param = decl->func.@params + i;
                    if (i != 0)
                    {
                        buf_write(',');
                        buf_write(' ');
                    }

                    reset_pos();
                    _typespec_to_cdecl(param->type, param->name);
                    buf_write(cdecl_buffer, _pos);
                }

            buf_write(')');
        }

        private void _gen_forward_decls()
        {
            for (var it = (Sym**) global_syms_buf->_begin; it != global_syms_buf->_top; it++)
            {
                var sym = *it;

                var decl = sym->decl;
                if (decl == null) continue;
                var name = sym->name;
                switch (decl->kind)
                {
                    case DECL_STRUCT:
                        _genln();
                        buf_write(typedef_keyword);
                        buf_write(' ');
                        buf_write(struct_keyword);
                        buf_write(' ');
                        buf_write(name);
                        buf_write(' ');
                        buf_write(name);
                        buf_write(';');
                        break;
                    case DECL_UNION:
                        _genln();
                        buf_write(typedef_keyword);
                        buf_write(' ');
                        buf_write(union_keyword);
                        buf_write(' ');
                        buf_write(name);
                        buf_write(' ');
                        buf_write(name);
                        buf_write(';');
                        break;
                    case DECL_FUNC:
                        _gen_func_decl(sym->decl);
                        buf_write(';');
                        break;
                }
            }
        }

        private void _gen_aggregate(Decl* decl)
        {
            assert(decl->kind == DECL_STRUCT || decl->kind == DECL_UNION);
            _genln();
            if (decl->kind == DECL_STRUCT)
                buf_write(struct_keyword);
            else buf_write(union_keyword);
            buf_write(' ');
            buf_write(decl->name);
            buf_write(' ');
            buf_write('{');

            _gen_indent++;
            for (var i = 0; i < decl->aggregate.num_items; i++)
            {
                var item = decl->aggregate.items[i];
                gen_sync_pos(item.pos);
                for (var j = 0; j < item.num_names; j++)
                {
                    _genln();
                    reset_pos();
                    _typespec_to_cdecl(item.type, item.names[j]);
                    buf_write(cdecl_buffer, _pos);
                    buf_write(';');
                }
            }

            _gen_indent--;
            _genln();
            buf_write('}');
            buf_write(';');
        }


        private void _gen_expr(Expr* expr)
        {
            switch (expr->kind)
            {
                case EXPR_INT:
                    buf_write(expr->int_val.ToString().ToPtr());
                    break;
                case EXPR_FLOAT:
                    buf_write(expr->float_val.ToString().ToPtr());
                    break;
                case EXPR_STR:
                    _gen_str(expr->str_val);
                    break;
                case EXPR_NAME:
                    buf_write(expr->name);
                    break;
                case EXPR_CAST:
                    buf_write('(');
                    reset_pos();
                    _type_to_cdecl(expr->cast.type->type, null);
                    buf_write(cdecl_buffer, _pos);
                    buf_write(')');
                    buf_write('(');
                    _gen_expr(expr->cast.expr);
                    buf_write(')');
                    break;
                case EXPR_CALL:
                    _gen_expr(expr->call.expr);
                    buf_write('(');
                    for (var i = 0; i < expr->call.num_args; i++)
                    {
                        if (i != 0)
                        {
                            buf_write(',');
                            buf_write(' ');
                        }

                        _gen_expr(expr->call.args[i]);
                    }

                    buf_write(')');
                    break;
                case EXPR_INDEX:
                    _gen_expr(expr->index.expr);
                    buf_write('[');
                    _gen_expr(expr->index.index);
                    buf_write(']');
                    break;
                case EXPR_FIELD:
                    _gen_expr(expr->field.expr);
                    buf_write('.');
                    buf_write(expr->field.name);
                    break;
                case EXPR_COMPOUND:
                    if (expr->compound.type != null)
                    {
                        buf_write('(');
                        reset_pos();
                        _typespec_to_cdecl(expr->compound.type, null);
                        buf_write(cdecl_buffer, _pos);
                        buf_write(')');
                        buf_write('{');
                    }
                    else
                    {
                        buf_write('(');
                        reset_pos();
                        _type_to_cdecl(expr->type, null);
                        buf_write(cdecl_buffer, _pos);
                        buf_write(')');
                        buf_write('{');
                    }

                    for (var i = 0; i < expr->compound.num_fields; i++)
                    {
                        if (i != 0)
                        {
                            buf_write(',');
                            buf_write(' ');
                        }

                        var field = expr->compound.fields + i;
                        if (field->kind == FIELD_NAME)
                        {
                            buf_write('.');
                            buf_write(field->name);
                            buf_write(' ');
                            buf_write('=');
                            buf_write(' ');
                        }
                        else if (field->kind == FIELD_INDEX)
                        {
                            buf_write('[');
                            _gen_expr(field->index);
                            buf_write(']');
                            buf_write(' ');
                            buf_write('=');
                            buf_write(' ');
                        }

                        _gen_expr(field->init);
                    }

                    buf_write('}');
                    break;
                case EXPR_UNARY:
                    buf_write(_token_kind_name(expr->unary.op));
                    buf_write('(');
                    _gen_expr(expr->unary.expr);
                    buf_write(')');
                    break;
                case EXPR_BINARY:
                    buf_write('(');
                    _gen_expr(expr->binary.left);
                    buf_write(')');
                    buf_write(' ');
                    buf_write(_token_kind_name(expr->binary.op));
                    buf_write(' ');
                    buf_write('(');
                    _gen_expr(expr->binary.right);
                    buf_write(')');
                    break;
                case EXPR_TERNARY:
                    buf_write('(');
                    _gen_expr(expr->ternary.cond);
                    buf_write(' ');
                    buf_write('?');
                    buf_write(' ');
                    _gen_expr(expr->ternary.then_expr);
                    buf_write(' ');
                    buf_write(':');
                    buf_write(' ');
                    _gen_expr(expr->ternary.else_expr);
                    buf_write(')');
                    break;
                case EXPR_SIZEOF_EXPR:
                    buf_write(sizeof_keyword);
                    buf_write('(');
                    _gen_expr(expr->sizeof_expr);
                    buf_write(')');
                    break;
                case EXPR_SIZEOF_TYPE:
                    buf_write(sizeof_keyword);
                    buf_write('(');
                    reset_pos();
                    _type_to_cdecl(expr->sizeof_type->type, null);
                    buf_write(cdecl_buffer, _pos);
                    buf_write(')');
                    break;
                default:
                    assert(false);
                    break;
            }
        }


        private void _gen_stmt_block(StmtList block)
        {
            buf_write('{');
            _gen_indent++;
            for (var i = 0; i < block.num_stmts; i++) _gen_stmt(block.stmts[i]);
            _gen_indent--;
            _genln();
            buf_write('}');
        }

        private void _gen_simple_stmt(Stmt* stmt)
        {
            switch (stmt->kind)
            {
                case STMT_EXPR:
                    _gen_expr(stmt->expr);
                    break;
                case STMT_INIT:
                    reset_pos();
                    _type_to_cdecl(stmt->init.expr->type, stmt->init.name);
                    buf_write(cdecl_buffer, _pos);
                    buf_write(' ');
                    buf_write('=');
                    buf_write(' ');
                    _gen_expr(stmt->init.expr);
                    break;
                case STMT_ASSIGN:
                    _gen_expr(stmt->assign.left);
                    if (stmt->assign.right != null)
                    {
                        buf_write(' ');
                        buf_write(_token_kind_name(stmt->assign.op));
                        buf_write(' ');
                        _gen_expr(stmt->assign.right);
                    }
                    else
                    {
                        buf_write(_token_kind_name(stmt->assign.op));
                    }

                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void _gen_stmt(Stmt* stmt)
        {
            gen_sync_pos(stmt->pos);
            switch (stmt->kind)
            {
                case STMT_RETURN:
                    _genlnf(return_keyword);
                    if (stmt->expr != null)
                    {
                        buf_write(' ');
                        _gen_expr(stmt->expr);
                    }

                    buf_write(';');
                    break;
                case STMT_BREAK:
                    _genlnf(break_keyword);
                    buf_write(';');
                    break;
                case STMT_CONTINUE:
                    _genlnf(continue_keyword);
                    buf_write(';');
                    break;
                case STMT_BLOCK:
                    _genln();
                    _gen_stmt_block(stmt->block);
                    break;
                case STMT_IF:
                    _genlnf(if_keyword);
                    buf_write(' ');
                    buf_write('(');
                    _gen_expr(stmt->if_stmt.cond);
                    buf_write(')');
                    buf_write(' ');
                    _gen_stmt_block(stmt->if_stmt.then_block);
                    for (var i = 0; i < stmt->if_stmt.num_elseifs; i++)
                    {
                        var elseif = stmt->if_stmt.elseifs[i];
                        buf_write(' ');
                        buf_write(else_keyword);
                        buf_write(' ');
                        buf_write(if_keyword);
                        buf_write(' ');
                        buf_write('(');
                        _gen_expr(elseif->cond);
                        buf_write(')');
                        buf_write(' ');
                        _gen_stmt_block(elseif->block);
                    }

                    if (stmt->if_stmt.else_block.stmts != null)
                    {
                        buf_write(' ');
                        buf_write(else_keyword);
                        buf_write(' ');
                        _gen_stmt_block(stmt->if_stmt.else_block);
                    }

                    break;
                case STMT_WHILE:
                    buf_write(while_keyword);
                    buf_write(' ');
                    buf_write('(');
                    _gen_expr(stmt->while_stmt.cond);
                    buf_write(')');
                    buf_write(' ');
                    _gen_stmt_block(stmt->while_stmt.block);
                    break;
                case STMT_DO_WHILE:
                    buf_write(do_keyword);
                    buf_write(' ');
                    _gen_stmt_block(stmt->while_stmt.block);
                    buf_write(' ');
                    buf_write(while_keyword);
                    buf_write(' ');
                    buf_write('(');
                    _gen_expr(stmt->while_stmt.cond);
                    buf_write(')');
                    buf_write(';');
                    break;
                case STMT_FOR:
                    _genlnf(for_keyword);
                    buf_write(' ');
                    buf_write('(');
                    if (stmt->for_stmt.init != null) _gen_simple_stmt(stmt->for_stmt.init);
                    buf_write(';');
                    if (stmt->for_stmt.cond != null)
                    {
                        buf_write(' ');
                        _gen_expr(stmt->for_stmt.cond);
                    }

                    buf_write(';');
                    if (stmt->for_stmt.next != null)
                    {
                        buf_write(' ');
                        _gen_simple_stmt(stmt->for_stmt.next);
                    }

                    buf_write(')');
                    buf_write(' ');
                    _gen_stmt_block(stmt->for_stmt.block);
                    break;
                case STMT_SWITCH:
                    _genlnf(switch_keyword);
                    buf_write(' ');
                    buf_write('(');
                    _gen_expr(stmt->switch_stmt.expr);
                    buf_write(')');
                    buf_write(' ');
                    buf_write(';');
                    for (var i = 0; i < stmt->switch_stmt.num_cases; i++)
                    {
                        var switch_case = stmt->switch_stmt.cases[i];
                        for (var j = 0; j < switch_case.num_exprs; j++)
                        {
                            _genlnf(case_keyword);
                            buf_write(' ');
                            _gen_expr(switch_case.exprs[j]);
                            buf_write(':');
                        }

                        if (switch_case.is_default)
                        {
                            buf_write(default_keyword);
                            buf_write(':');
                        }

                        buf_write(' ');
                        _gen_stmt_block(switch_case.block);
                    }

                    _genln();
                    buf_write('}');
                    break;
                default:
                    _genln();
                    _gen_simple_stmt(stmt);
                    buf_write(';');
                    break;
            }
        }

        private void _gen_func(Decl* decl)
        {
            assert(decl->kind == DECL_FUNC);
            _gen_func_decl(decl);
            buf_write(' ');
            _gen_stmt_block(decl->func.block);
        }

        private void _gen_sym(Sym* sym)
        {
            var decl = sym->decl;
            if (decl == null) return;
            gen_sync_pos(decl->pos);
            switch (decl->kind)
            {
                case DECL_CONST:
                    _genln();
                    buf_write(enum_keyword);
                    buf_write(' ');
                    buf_write('{');
                    buf_write(' ');
                    buf_write(sym->name);
                    buf_write(' ');
                    buf_write('=');
                    buf_write(' ');
                    _gen_expr(decl->const_decl.expr);
                    buf_write(' ');
                    buf_write('}');
                    buf_write(';');
                    break;
                case DECL_VAR:
                    if (decl->var.type != null)
                    {
                        _genln();
                        reset_pos();
                        _typespec_to_cdecl(decl->var.type, sym->name);
                        buf_write(cdecl_buffer, _pos);
                    }
                    else
                    {
                        _genln();
                        reset_pos();
                        _type_to_cdecl(sym->type, sym->name);
                        buf_write(cdecl_buffer, _pos);
                    }

                    if (decl->var.expr != null)
                    {
                        buf_write(' ');
                        buf_write('=');
                        buf_write(' ');
                        _gen_expr(decl->var.expr);
                    }

                    buf_write(';');
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
                    buf_write(typedef_keyword);
                    buf_write(' ');
                    buf_write(' ');
                    reset_pos();
                    _type_to_cdecl(sym->type, sym->name);
                    buf_write(cdecl_buffer, _pos);
                    buf_write(';');
                    break;
                default:
                    assert(false);
                    break;
            }
        }

        private void _gen_ordered_decls()
        {
            var sym_cnt = ordered_syms->count;

            for (var i = 0; i < sym_cnt; i++)
            {
                //Console.Write($"\r{i} of {sym_cnt}    ");
                var sym = (Sym**) (ordered_syms->_begin + i);
                _gen_sym(*sym);
            }
        }

        private void _gen_all()
        {
            buf_write(gen_preamble);
            buf_write(fd);
            _gen_forward_decls();
            _genln();
            _genln();
            buf_write(od);
            _gen_ordered_decls();
        }


        private void cdecl_test()
        {
            var c = 'x';
            _pos = 0;
            _type_to_cdecl(type_int, &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_ptr(type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_array(type_int, 10), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_func(new[] {type_int}, 1, type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_array(type_func(new[] {type_int}, 1, type_int), 10), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_func(new[] {type_ptr(type_int)}, 1, type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            var type1 = type_func(new[] {type_array(type_int, 10)}, 1, type_int);
            _type_to_cdecl(type1, &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_func((Type**) null, 0, type1), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            _type_to_cdecl(type_func((Type**) null, 0, type_array(type_func((Type**) null, 0, type_int), 10)), &c);
            Console.WriteLine(new string(copy_buf()));
        }

        internal void gen_test()
        {
            //cdecl_test();

            local_syms.clear();
            init_stream(code5, null);
            init_global_syms();
            var f = parse_file();
            sym_global_decls(f);
            finalize_syms();

            _gen_all();
            Console.WriteLine(new string(gen_buf));
        }
    }
}