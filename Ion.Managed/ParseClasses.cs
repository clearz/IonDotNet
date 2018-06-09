namespace MLang
{


    class StmtBlock
    {
        public Stmt[] stmts;
        public int num_stmts;
    }

    class FuncTypespec
    {
        public Typespec[] args;
        public int num_args;
        public Typespec ret;
    }
    
    class PtrTypespec
    {
        public Typespec elem;
    }


    class ArrayTypespec
    {
        public Typespec elem;
        public Expr size;
    }

   // [StructLayout(LayoutKind.Explicit)]
    class Typespec
    {
        public TypespecKind kind;
        public string name;
        public PtrTypespec ptr;
        public FuncTypespec func;
        public ArrayTypespec array;
    }

    class FuncParam
    {
        public string name;
        public Typespec type;
    }


    class FuncDecl
    {
        public FuncParam[] @params;
        public int num_params;
        public Typespec ret_type;
        public StmtBlock block;
    }


    class EnumItem
    {
        public string name;
        public Expr init;
    }


    class EnumDecl
    {
        public EnumItem[] items;
        public int num_items;
    }


    class AggregateItem
    {
        public string[] names;
        public int num_names;
        public Typespec type;
    }


    class AggregateDecl
    {
        public AggregateItem[] items;
        public int num_items;
    }


    class TypedefDecl
    {
        public Typespec type;
    }


    class VarDecl
    {
        public Typespec type;
        public Expr expr;
    }


    class ConstDecl
    {
        public Expr expr;
    }
    
    class Decl
    {
        public DeclKind kind;
        public string name;
        public EnumDecl enum_decl;
        public AggregateDecl aggregate;
        public FuncDecl func;
        public TypedefDecl typedef_decl;
        public VarDecl var;
        public ConstDecl const_decl;

    }

    class CompoundExpr
    {
        public Typespec type;
        public Expr[] args;
        public int num_args;
    }


    class CastExpr
    {
        public Typespec type;
        public Expr expr;
    }


    class UnaryExpr
    {
        public TokenKind op;
        public Expr expr;
    }


    class BinaryExpr
    {
        public TokenKind op;
        public Expr left;
        public Expr right;
    }


    class TernaryExpr
    {
        public Expr cond;
        public Expr then_expr;
        public Expr else_expr;
    }


    class CallExpr
    {
        public Expr expr;
        public Expr[] args;
        public int num_args;
    }


    class IndexExpr
    {
        public Expr expr;
        public Expr index;
    }


    class FieldExpr
    {
        public Expr expr;
        public string name;
    }

   // [StructLayout(LayoutKind.Explicit)]
    class Expr
    {

        public ExprKind kind;
        public ulong int_val;
        public double float_val;
        public string str_val;
        public string name;
        public Expr sizeof_expr;
        public Typespec sizeof_type;
        public CompoundExpr compound;
        public CastExpr cast;
        public UnaryExpr unary;
        public BinaryExpr binary;
        public TernaryExpr ternary;
        public CallExpr call;
        public IndexExpr index;
        public FieldExpr field;
    }

    class ReturnStmt
    {
        public Expr expr;
    }


    class ElseIf
    {
        public Expr cond;
        public StmtBlock block;
    }


    class IfStmt
    {
        public Expr cond;
        public StmtBlock then_block;
        public ElseIf[] elseifs;
        public int num_elseifs;
        public StmtBlock else_block;
    }


    class WhileStmt
    {
        public Expr cond;
        public StmtBlock block;
    }


    class ForStmt
    {
        public Stmt init;
        public Expr cond;
        public Stmt next;
        public StmtBlock block;
    }


    class SwitchCase
    {
        public Expr[] exprs;
        public int num_exprs;
        public bool is_default;
        public StmtBlock block;
    }


    class SwitchStmt
    {
        public Expr expr;
        public SwitchCase[] cases;
        public int num_cases;
    }


    class AssignStmt
    {
        public TokenKind op;
        public Expr left;
        public Expr right;
    }


    class InitStmt
    {
        public string name;
        public Expr expr;
    }

    //[StructLayout(LayoutKind.Explicit)]
    class Stmt
    {
        public StmtKind kind;
        public ReturnStmt return_stmt;
        public IfStmt if_stmt;
        public WhileStmt while_stmt;
        public ForStmt for_stmt;
        public SwitchStmt switch_stmt;
        public StmtBlock block;
        public AssignStmt assign;
        public InitStmt init;
        public Expr expr;
        public Decl decl;
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