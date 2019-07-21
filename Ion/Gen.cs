using DotNetCross.Memory;
using System;
using System.Runtime.CompilerServices;

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
        private Buffer<char> gen_buf = Buffer<char>.Create(_1MB, 4);
        private readonly char[] char_to_escape = new char[256];
        private readonly string preamble = "// Preamble\n#include <stdio.h>\n\n"+
                                            "typedef unsigned char uchar;\n"+
                                            "typedef signed char schar;\n"+
                                            "typedef unsigned short ushort;\n"+
                                            "typedef unsigned int uint;\n"+
                                            "typedef unsigned long ulong;\n"+
                                            "typedef long long llong;\n"+
                                            "typedef unsigned long long ullong;\n"+
                                            "\n";
    
        private readonly char* lineStr = "#line ".ToPtr();
        private readonly char* VOID = "void".ToPtr();

        private SrcPos gen_pos;
        private int _pos, gen_indent;

        private void reset_pos() {
            _pos = 0;
        }

        private char* copy_buf() {
            var len = _pos;
            var c = (char*) xmalloc(len << (1 + 2));

            Unsafe.CopyBlock(c, cdecl_buffer, (uint)len << 1);
            c[len] = '\0';
            _pos = 0;
            return c;
        }

        private void indent() {
            var size = gen_indent * 4;

            gen_buf.Add(line, size);
            gen_pos.line++;
            // Unsafe.CopyBlock(gen_buf + gen_pos, line, (uint)size << 1);
            //gen_pos += size;
        }

        private void writeln() {
            gen_buf.Add('\n');
        }

        private void buf_write(char c) {
            *(cdecl_buffer + _pos++) = c;
        }

        private void buf_write(char* c) {
            var len = strlen(c);
            Unsafe.CopyBlock(cdecl_buffer + _pos, c, (uint)len << 1);
            _pos += len;
        }

        private void buf_write(char* c, int len) {
            Unsafe.CopyBlock(cdecl_buffer + _pos, c, (uint)len << 1);
            _pos += len;
        }

        private void c_write(char c) {
            gen_buf.Add(c);
        }

        private void c_write(char* c) {
            gen_buf.Add(c, strlen(c));
            //while ((*(gen_buf + gen_pos) = *(c++)) != 0) gen_pos++;
        }

        private void c_write(char* c, int len) {
            gen_buf.Add(c, len);
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
                    buf_write('(');
                    buf_write('*');
                    buf_write(str);
                    buf_write(')');
                    type_to_cdecl(type->ptr.elem, copy_buf());
                }
                else {
                    //buf_write(cdecl_name(type->ptr.elem));
                    type_to_cdecl(type->ptr.elem, null);
                    buf_write(' ');
                    buf_write('*');
                }

                break;
                case TYPE_ARRAY:
                if (str != null) {
                    buf_write('(');
                    buf_write(str);
                    buf_write('[');
                    buf_write(type->array.size.ToString().ToPtr());
                    buf_write(']');
                    buf_write(')');
                    type_to_cdecl(type->array.elem, copy_buf());
                }
                else {
                    type_to_cdecl(type->ptr.elem, null);
                    buf_write(' ');
                    buf_write('[');
                    buf_write(type->array.size.ToString().ToPtr());
                    buf_write(']');
                }

                break;
                case TYPE_FUNC: {
                    if (str != null) {
                        buf_write('(');
                        buf_write('*');
                        buf_write(str);
                        buf_write(')');
                    }

                    buf_write('(');
                    if (type->func.num_params == 0)
                        buf_write(VOID, 4);
                    else
                        for (var i = 0; i < type->func.num_params; i++) {
                            if (i > 0) {
                                buf_write(',');
                                buf_write(' ');
                            }

                            type_to_cdecl(type->func.@params[i], null);
                        }
                    if (type->func.variadic) {
                        buf_write(',');
                        buf_write(' ');
                        buf_write('.');
                        buf_write('.');
                        buf_write('.');
                    }
                    buf_write(')');
                    type_to_cdecl(type->func.ret, copy_buf());
                }
                break;
                default:
                if (str != null) {
                    buf_write(cdecl_name(type));
                    buf_write(' ');
                    buf_write(str);
                }
                else {
                    buf_write(cdecl_name(type));
                }
                break;
            }
        }

        private void gen_str(char* str) {
            c_write('\"');
            while (*str != 0) {
                var start = str;
                while (*str != 0 && char_to_escape[*str] == 0)
                    str++;
                if (start != str)
                    c_write(start, (int)(str - start));
                if (*str != 0 && char_to_escape[*str] != 0) {
                    c_write('\\');
                    c_write(char_to_escape[*str]);
                    str++;
                }
            }

            c_write('\"');
        }

        private void gen_sync_pos(SrcPos pos) {
            assert(pos.name != null && pos.line != 0);
            if (gen_pos.line != pos.line || gen_pos.name != pos.name) {
                genln();
                c_write(lineStr, 6);
                c_write(pos.line.ToString().ToPtr());
                if (gen_pos.name != pos.name) {
                    c_write(' ');
                    c_write(pos.name);
                }
                gen_pos = pos;
            }
        }

        private void gen_expr_str(Expr* expr) {
            var temp = gen_buf;
            var tPos = gen_buf.count;
            gen_buf = Buffer<char>.Create(1024);
            gen_expr(expr);
            buf_write(gen_buf._begin, gen_buf.count);
            gen_buf = temp;
        }


        private void typespec_to_cdecl(Typespec* typespec, char* str) {
            // TODO: Figure out how to handle type vs typespec in C gen for inferred types. How to prevent "flattened" values?
            switch (typespec->kind) {
                case TYPESPEC_NAME:
                if (str != null) {
                    buf_write(typespec->name);
                    buf_write(' ');
                    buf_write(str);
                }
                else {
                    buf_write(typespec->name);
                }

                break;
                case TYPESPEC_PTR:
                if (str != null) {
                    buf_write('(');
                    buf_write('*');
                    buf_write(str);
                    buf_write(')');
                    typespec_to_cdecl(typespec->ptr.elem, copy_buf());
                }
                else {
                    buf_write(typespec->name);
                    buf_write(' ');
                    buf_write('*');
                }

                break;
                case TYPESPEC_ARRAY:
                if (str != null) {
                    buf_write('(');
                    buf_write(str);
                    buf_write('[');
                    gen_expr_str(typespec->array.size);
                    buf_write(']');
                    buf_write(')');
                    typespec_to_cdecl(typespec->array.elem, copy_buf());
                }
                else {
                    buf_write(typespec->name);
                    buf_write(' ');
                    buf_write('[');
                    gen_expr_str(typespec->array.size);
                    buf_write(']');
                }

                break;
                case TYPESPEC_FUNC: {
                    if (str != null) {
                        buf_write('(');
                        buf_write('*');
                        buf_write(str);
                        buf_write(')');
                    }

                    buf_write('(');
                    if (typespec->func.num_args == 0) {
                        buf_write(VOID, 4);
                    }
                    else {
                        for (var i = 0; i < typespec->func.num_args; i++) {
                            if (i > 0) {
                                buf_write(',');
                                buf_write(' ');
                            }

                            typespec_to_cdecl(typespec->func.args[i], null);
                        }

                        if (typespec->func.variadic) {
                            buf_write(',');
                            buf_write(' ');
                            buf_write('.');
                            buf_write('.');
                            buf_write('.');
                        }
                        buf_write(')');
                        typespec_to_cdecl(typespec->func.ret, copy_buf());
                    }

                    break;
                }

                default:
                assert(false);
                break;
            }
        }

        private void gen_func_defs() {
            for (Sym** it = (Sym**)global_syms_buf->_begin; it < global_syms_buf->_top; it++) {
                Sym* sym = *it;
                Decl* decl = sym->decl;

                if (decl != null && decl->kind == DECL_FUNC) {
                    if (!is_decl_foreign(decl)) {
                        gen_func_decl(decl);
                        c_write(' ');
                        gen_stmt_block(decl->func.block);
                        genln();
                    }
                }
            }
        }
        void gen_func_decl(Decl* decl) {
            assert(decl->kind == DECL_FUNC);

            gen_sync_pos(decl->pos);
            if (decl->func.ret_type != null) {
                genln();
                reset_pos();
                typespec_to_cdecl(decl->func.ret_type, decl->name);
                c_write(cdecl_buffer, _pos);
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

                    reset_pos();
                    typespec_to_cdecl(param->type, param->name);
                    c_write(cdecl_buffer, _pos);
                }
            if (decl->func.variadic) {
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
                    reset_pos();
                    typespec_to_cdecl(item.type, item.names[j]);
                    c_write(cdecl_buffer, _pos);
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
                reset_pos();
                typespec_to_cdecl(expr->compound.type, null);
                c_write(cdecl_buffer, _pos);
                c_write(')');
                c_write('{');
            }
            else {
                c_write('(');
                reset_pos();
                type_to_cdecl(expr->type, null);
                c_write(cdecl_buffer, _pos);
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
        private void gen_expr(Expr* expr) {
            switch (expr->kind) {
                case EXPR_INT:
                c_write(expr->int_val.ToString().ToPtr());
                break;
                case EXPR_FLOAT:
                c_write(expr->float_val.ToString().ToPtr());
                break;
                case EXPR_STR:
                gen_str(expr->str_val);
                break;
                case EXPR_NAME:
                c_write(expr->name);
                break;
                case EXPR_CAST:
                c_write('(');
                reset_pos();
                type_to_cdecl(expr->cast.type->type, null);
                c_write(cdecl_buffer, _pos);
                c_write(')');
                c_write('(');
                gen_expr(expr->cast.expr);
                c_write(')');
                break;
                case EXPR_CALL:
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
                reset_pos();
                type_to_cdecl(expr->sizeof_type->type, null);
                c_write(cdecl_buffer, _pos);
                c_write(')');
                break;
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
        bool is_incomplete_array_type(Typespec* typespec) {
            return typespec->kind == TYPESPEC_ARRAY && typespec->array.size == null;
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
                reset_pos();
                type_to_cdecl(stmt->init.expr->type, stmt->init.name);
                c_write(cdecl_buffer, _pos);
                c_write(' ');
                c_write('=');
                c_write(' ');
                gen_init_expr(stmt->init.expr);
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
                c_write(while_keyword);
                c_write(' ');
                c_write('(');
                gen_expr(stmt->while_stmt.cond);
                c_write(')');
                c_write(' ');
                gen_stmt_block(stmt->while_stmt.block);
                break;
                case STMT_DO_WHILE:
                c_write(do_keyword);
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



        private void gen_decl(Sym* sym) {
            var decl = sym->decl;
            if (decl == null)
                return;
            if (decl->kind != DECL_FUNC || !is_decl_foreign(decl))
                gen_sync_pos(decl->pos);
            switch (decl->kind) {
                case DECL_CONST:
                    genln();
                    c_write(enum_keyword);
                    c_write(' ');
                    c_write('{');
                    c_write(' ');
                    c_write(sym->name);
                    c_write(' ');
                    c_write('=');
                    c_write(' ');
                    gen_expr(decl->const_decl.expr);
                    c_write(' ');
                    c_write('}');
                    c_write(';');
                    break;
                case DECL_VAR:
                    if (decl->var.type != null && !is_incomplete_array_type(decl->var.type)) {
                        genln();
                        reset_pos();
                        typespec_to_cdecl(decl->var.type, sym->name);
                        c_write(cdecl_buffer, _pos);
                    }
                    else {
                        genln();
                        reset_pos();
                        type_to_cdecl(sym->type, sym->name);
                        c_write(cdecl_buffer, _pos);
                    }

                    if (decl->var.expr != null) {
                        c_write(' ');
                        c_write('=');
                        c_write(' ');
                        gen_init_expr(decl->var.expr);
                    }

                    c_write(';');
                    break;
                case DECL_FUNC:
                    if (!is_decl_foreign(decl)) {
                        gen_func_decl(decl);
                        c_write(';');
                    }
                    break;
                case DECL_STRUCT:
                case DECL_UNION:
                    gen_aggregate(decl);
                    break;
                case DECL_TYPEDEF:
                    genln();
                    c_write(typedef_keyword);
                    c_write(' ');
                    reset_pos();
                    typespec_to_cdecl(decl->typedef_decl.type, sym->name);
                    c_write(cdecl_buffer, _pos);
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

        private readonly char* forward_declarations = "// Forward declarations".ToPtr();
        private readonly char* sorted_declarations = "// Sorted declarations".ToPtr();
        private readonly char* function_declarations = "// Function declarations".ToPtr();

        private void gen_all() {
            c_write(preamble.ToPtr(), preamble.Length);
            c_write(forward_declarations);
            gen_forward_decls();
            genln();
            genlnf(sorted_declarations);
            gen_sorted_decls();
            genlnf(function_declarations);
            gen_func_defs();
        }


        private void cdecl_test() {
            var c = 'x';
            _pos = 0;
            type_to_cdecl(type_int, &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_ptr(type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_array(type_int, 10), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_func(new[] { type_int }, 1, type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_array(type_func(new[] { type_int }, 1, type_int), 10), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_func(new[] { type_ptr(type_int) }, 1, type_int), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            var type1 = type_func(new[] {type_array(type_int, 10)}, 1, type_int);
            type_to_cdecl(type1, &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_func((Type**)null, 0, type1), &c);
            Console.WriteLine(new string(cdecl_buffer));
            _pos = 0;
            type_to_cdecl(type_func((Type**)null, 0, type_array(type_func((Type**)null, 0, type_int), 10)), &c);
            Console.WriteLine(new string(copy_buf()));
        }
    }
}