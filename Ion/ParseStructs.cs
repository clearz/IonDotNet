using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lang
{
#if X64
    using size_t = Int64;
#else
    using size_t = Int32;
#endif

    unsafe struct StmtBlock
    {
        public Stmt** stmts;
        public size_t num_stmts;
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Typespec
    {
        [FieldOffset(0)] public TypespecKind kind;
        [FieldOffset(4)] public char* name;
        [FieldOffset(4)] public PtrTypespec ptr;
        [FieldOffset(4)] public FuncTypespec func;
        [FieldOffset(4)] public ArrayTypespec array;


        internal struct FuncTypespec
        {
            public Typespec** args;
            public size_t num_args;
            public Typespec* ret;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct PtrTypespec
        {
            public Typespec* elem;
        }


        internal struct ArrayTypespec
        {
            public Typespec* elem;
            public Expr* size;
        }

    }


    unsafe struct FuncParam
    {
        public char* name;
        public Typespec* type;
    }


    unsafe struct EnumItem
    {
        public char* name;
        public Expr* init;
    }

    unsafe struct AggregateItem
    {
        public char** names;
        public size_t num_names;
        public Typespec* type;
    }


    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Decl
    {
        [FieldOffset(0)] public DeclKind kind;
        [FieldOffset(4)] public char* name;
        [FieldOffset(4 + Ion.PTR_SIZE)] public EnumDecl enum_decl;
        [FieldOffset(4 + Ion.PTR_SIZE)] public AggregateDecl aggregate;
        [FieldOffset(4 + Ion.PTR_SIZE)] public FuncDecl func;
        [FieldOffset(4 + Ion.PTR_SIZE)] public TypedefDecl typedef_decl;
        [FieldOffset(4 + Ion.PTR_SIZE)] public VarDecl var;
        [FieldOffset(4 + Ion.PTR_SIZE)] public ConstDecl const_decl;



        internal struct FuncDecl
        {
            public FuncParam* @params;
            public size_t num_params;
            public Typespec* ret_type;
            public StmtBlock block;
        }


        internal struct EnumDecl
        {
            public EnumItem* items;
            public size_t num_items;
        }

        internal struct AggregateDecl
        {
            public AggregateItem* items;
            public size_t num_items;
        }


        internal struct TypedefDecl
        {
            public Typespec* type;
        }


        internal struct VarDecl
        {
            public Typespec* type;
            public Expr* expr;
        }


        internal struct ConstDecl
        {
            public Expr* expr;
        }

    }
    enum CompoundFieldKind
    {
        FIELD_DEFAULT,
        FIELD_NAME,
        FIELD_INDEX,
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct CompoundField
    {
        [FieldOffset(0)] public CompoundFieldKind kind;
        [FieldOffset(4)] public Expr* init;
        [FieldOffset(Ion.PTR_SIZE + 4)] public char* name;
        [FieldOffset(Ion.PTR_SIZE + 4)] public Expr* index;
    }
    

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Expr
    {

        [FieldOffset(0)] public ExprKind kind;
        [FieldOffset(4)] public long int_val;
        [FieldOffset(4)] public double float_val;
        [FieldOffset(4)] public char* str_val;
        [FieldOffset(4)] public char* name;
        [FieldOffset(4)] public Expr* sizeof_expr;
        [FieldOffset(4)] public Typespec* sizeof_type;
        [FieldOffset(4)] public CompoundExpr compound;
        [FieldOffset(4)] public CastExpr cast;
        [FieldOffset(4)] public UnaryExpr unary;
        [FieldOffset(4)] public BinaryExpr binary;
        [FieldOffset(4)] public TernaryExpr ternary;
        [FieldOffset(4)] public CallExpr call;
        [FieldOffset(4)] public IndexExpr index;
        [FieldOffset(4)] public FieldExpr field;


        internal struct CompoundExpr
        {
            public Typespec* type;
            public CompoundField* fields;
            public size_t num_fields;
        }


        internal struct CastExpr
        {
            public Typespec* type;
            public Expr* expr;
        }


        internal struct UnaryExpr
        {
            public TokenKind op;
            public Expr* expr;
        }


        internal struct BinaryExpr
        {
            public TokenKind op;
            public Expr* left;
            public Expr* right;
        }


        internal struct TernaryExpr
        {
            public Expr* cond;
            public Expr* then_expr;
            public Expr* else_expr;
        }


        internal struct CallExpr
        {
            public Expr* expr;
            public Expr** args;
            public size_t num_args;
        }


        internal struct IndexExpr
        {
            public Expr* expr;
            public Expr* index;
        }


        internal struct FieldExpr
        {
            public Expr* expr;
            public char* name;
        }

    }


    unsafe struct ElseIf
    {
        public Expr* cond;
        public StmtBlock block;
    }


    unsafe struct SwitchCase
    {
        public Expr** exprs;
        public size_t num_exprs;
        public bool is_default;
        public StmtBlock block;
    }


    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Stmt
    {
        [FieldOffset(0)] public StmtKind kind;
        [FieldOffset(4)] public IfStmt if_stmt;
        [FieldOffset(4)] public WhileStmt while_stmt;
        [FieldOffset(4)] public ForStmt for_stmt;
        [FieldOffset(4)] public SwitchStmt switch_stmt;
        [FieldOffset(4)] public StmtBlock block;
        [FieldOffset(4)] public AssignStmt assign;
        [FieldOffset(4)] public InitStmt init;
        [FieldOffset(4)] public Expr* expr;
        [FieldOffset(4)] public Decl* decl;


        internal struct IfStmt
        {
            public Expr* cond;
            public StmtBlock then_block;
            public ElseIf* elseifs;
            public size_t num_elseifs;
            public StmtBlock else_block;
        }


        internal struct WhileStmt
        {
            public Expr* cond;
            public StmtBlock block;
        }


        internal struct ForStmt
        {
            public Stmt* init;
            public Expr* cond;
            public Stmt* next;
            public StmtBlock block;
        }


        internal struct SwitchStmt
        {
            public Expr* expr;
            public SwitchCase* cases;
            public size_t num_cases;
        }


        internal struct AssignStmt
        {
            public TokenKind op;
            public Expr* left;
            public Expr* right;
        }


        internal struct InitStmt
        {
            public char* name;
            public Expr* expr;
        }

    }



    enum TypespecKind
    {
        TYPESPEC_NONE,
        TYPESPEC_NAME,
        TYPESPEC_FUNC,
        TYPESPEC_ARRAY,
        TYPESPEC_PTR,
    }

    enum DeclKind
    {
        DECL_NONE,
        DECL_ENUM,
        DECL_STRUCT,
        DECL_UNION,
        DECL_VAR,
        DECL_CONST,
        DECL_TYPEDEF,
        DECL_FUNC,
    }

    enum ExprKind
    {
        EXPR_NONE,
        EXPR_INT,
        EXPR_FLOAT,
        EXPR_STR,
        EXPR_NAME,
        EXPR_CAST,
        EXPR_CALL,
        EXPR_INDEX,
        EXPR_FIELD,
        EXPR_COMPOUND,
        EXPR_UNARY,
        EXPR_BINARY,
        EXPR_TERNARY,
        EXPR_SIZEOF_EXPR,
        EXPR_SIZEOF_TYPE,
    }


    enum StmtKind
    {
        STMT_NONE,
        STMT_DECL,
        STMT_RETURN,
        STMT_BREAK,
        STMT_CONTINUE,
        STMT_BLOCK,
        STMT_IF,
        STMT_WHILE,
        STMT_DO_WHILE,
        STMT_FOR,
        STMT_SWITCH,
        STMT_ASSIGN,
        STMT_INIT,
        STMT_EXPR,
    }
}