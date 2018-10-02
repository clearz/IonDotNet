using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lang
{
    using static TypeKind;
    using static SymState;
    using static DeclKind;
    using static SymKind;
    using static TypespecKind;
    using static ExprKind;
    using static TokenKind;
    using static StmtKind;
    using static CompoundFieldKind;

    #region Typedefs

#if X64
    using size_t = Int64;
#else
    using size_t = System.Int32;
#endif

    #endregion
    unsafe partial class Ion
    {
        Buffer* gen_buf = Buffer.Create(2);

        void genf(string fmt, params object[] pms) => gen_buf->Add(string.Format(fmt, pms).ToPtr());

        void genlnf(string fmt, params object[] pms) {
            gen_buf->Add("\n".PadLeft(gen_indent * 4, ' ').ToPtr());
            gen_buf->Add(string.Format(fmt, pms).ToPtr());
        }

        int gen_indent;

        void genln()
        {
            gen_buf->Add("\n".PadLeft(gen_indent * 4, ' ').ToPtr());
        }

         char* cdecl_paren( char* str, bool b) {
            return b? strf("({0})", str) : str;
        }

         char* cdecl_name(Type * type) {
            switch (type->kind) {
                case TYPE_VOID:
                    return "void".ToPtr();
                case TYPE_CHAR:
                    return "char".ToPtr();
                case TYPE_INT:
                    return "int".ToPtr();
                case TYPE_FLOAT:
                    return "float".ToPtr();
                case TYPE_STRUCT:
                case TYPE_UNION:
                    return type->sym->name;
                default:
                    assert(false);
                    return null;
            }
        }
    
        char* type_to_cdecl(Type* type,  char* str)
        {
            switch (type->kind)
            {
                case TYPE_VOID:
                case TYPE_CHAR:
                case TYPE_INT:
                case TYPE_FLOAT:
                case TYPE_STRUCT:
                case TYPE_UNION:
                    return strf("%s%s%s", cdecl_name(type), (*str) != 0 ? " ".ToPtr() : "".ToPtr(), str);
                case TYPE_PTR:
                    return type_to_cdecl(type->ptr.elem, cdecl_paren(strf("*%s", str), *str != 0));
                case TYPE_ARRAY:
                    return type_to_cdecl(type->array.elem, cdecl_paren(strf("%s[%llu]", str, type->array.size.ToString().ToPtr()), *str != 0));
                case TYPE_FUNC:
                {
                    char* result = null;
                  //  buf_printf(result, "%s(", cdecl_paren(strf("*%s", str), *str != 0));
                    if (type->func.num_params == 0)
                    {
                       // buf_printf(result, "void");
                    }
                    else
                    {
                        for (size_t i = 0; i < type->func.num_params; i++)
                        {
                            //buf_printf(result, "%s%s", i == 0 ? "" : ", ", type_to_cdecl(type->func.@params[i], ""));
                        }
                    }
                    //buf_printf(result, ")");
                    return type_to_cdecl(type->func.ret, result);
                }
                default:
                    assert(false);
                    return null;
            }
        }
    }
}
