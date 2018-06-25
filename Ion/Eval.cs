using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lang
{
    using static TokenKind;
    using static TokenMod;
    using static TypespecKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;

#if X64
    using size_t = Int64;
#else
    using size_t = Int32;
#endif

    partial class Ion
    {
        long eval_unary_int64(TokenKind op, long operand)
        {
            long result;
            switch (op)
            {
                case TOKEN_ADD:
                    result = +operand;
                    break;
                case TOKEN_SUB:
                    result = -operand;
                    break;
                case TOKEN_MUL:
                case TOKEN_AND:
                    result = 0;
                    break;
                default:
                    result = 0;
                    assert(false);
                    break;
            }
            return result;
        }

        long eval_binary_int64(TokenKind op, long left, long right)
        {
            long result;
            switch (op)
            {
                case TOKEN_MUL:
                    result = left * right;
                    break;
                case TOKEN_DIV:
                    result = right != 0 ? left / right : 0;
                    break;
                case TOKEN_MOD:
                    result = right != 0 ? left % right : 0;
                    break;
                case TOKEN_AND:
                    result = left & right;
                    break;
                case TOKEN_LSHIFT:
                    result = left << (int)right;
                    break;
                case TOKEN_RSHIFT:
                    result = left >> (int)right;
                    break;
                case TOKEN_ADD:
                    result = left + right;
                    break;
                case TOKEN_SUB:
                    result = left - right;
                    break;
                case TOKEN_XOR:
                    result = left ^ right;
                    break;
                case TOKEN_OR:
                    result = left | right;
                    break;
                case TOKEN_EQ:
                    result = left == right ? 1 : 0;
                    break;
                case TOKEN_NOTEQ:
                    result = left != right ? 1 : 0;
                    break;
                case TOKEN_LT:
                    result = left < right ? 1 : 0; ;
                    break;
                case TOKEN_LTEQ:
                    result = left <= right ? 1 : 0;
                    break;
                case TOKEN_GT:
                    result = left > right ? 1 : 0;
                    break;
                case TOKEN_GTEQ:
                    result = left >= right ? 1 : 0;
                    break;
                default:
                    result = 0;
                    assert(false);
                    break;
            }
            return result;
        }
    }
}
