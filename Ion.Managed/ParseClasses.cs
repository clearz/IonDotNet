using System;

namespace MLang
{


    class StmtBlock : ICloneable
    {
        public Stmt[] stmts;
        public int num_stmts;
        public object Clone() => MemberwiseClone();
    }

    class FuncTypespec : ICloneable
    {
        public Typespec[] args;
        public int num_args;
        public Typespec ret;
        public object Clone() => MemberwiseClone();
    }
    
    class PtrTypespec : ICloneable
    {
        public Typespec elem;
        public object Clone() => MemberwiseClone();
    }


    class ArrayTypespec : ICloneable
    {
        public Typespec elem;
        public Expr size;
        public object Clone() => MemberwiseClone();
    }
    
    class Typespec : ICloneable
    {
        public TypespecKind kind;
        public string name;
        public PtrTypespec ptr;
        public FuncTypespec func;
        public ArrayTypespec array;
        public object Clone() => MemberwiseClone();
    }

    class FuncParam : ICloneable
    {
        public string name;
        public Typespec type;
        public object Clone() => MemberwiseClone();
    }


    class FuncDecl : ICloneable
    {
        public FuncParam[] @params;
        public int num_params;
        public Typespec ret_type;
        public StmtBlock block;
        public object Clone() => MemberwiseClone();
    }


    class EnumItem : ICloneable
    {
        public string name;
        public Expr init;
        public object Clone() => MemberwiseClone();
    }


    class EnumDecl : ICloneable
    {
        public EnumItem[] items;
        public int num_items;
        public object Clone() => MemberwiseClone();
    }


    class AggregateItem : ICloneable
    {
        public string[] names;
        public int num_names;
        public Typespec type;
        public object Clone() => MemberwiseClone();
    }


    class AggregateDecl : ICloneable
    {
        public AggregateItem[] items;
        public int num_items;
        public object Clone() => MemberwiseClone();
    }


    class TypedefDecl : ICloneable
    {
        public Typespec type;
        public object Clone() => MemberwiseClone();
    }


    class VarDecl : ICloneable
    {
        public Typespec type;
        public Expr expr;
        public object Clone() => MemberwiseClone();
    }


    class ConstDecl : ICloneable
    {
        public Expr expr;
        public object Clone() => MemberwiseClone();
    }
    
    class Decl : ICloneable
    {
        public DeclKind kind;
        public string name;
        public EnumDecl enum_decl; 
        public AggregateDecl aggregate;
        public FuncDecl func;
        public TypedefDecl typedef_decl;
        public VarDecl var;
        public ConstDecl const_decl;
        public object Clone() => MemberwiseClone();

    }

    class CompoundExpr : ICloneable
    {
        public Typespec type;
        public CompoundField[] fields;
        public int num_fields;
        public object Clone() => MemberwiseClone();
    }


    class CastExpr : ICloneable
    {
        public Typespec type;
        public Expr expr;
        public object Clone() => MemberwiseClone();
    }


    class UnaryExpr : ICloneable
    {
        public TokenKind op;
        public Expr expr;
        public object Clone() => MemberwiseClone();
    }


    class BinaryExpr : ICloneable
    {
        public TokenKind op;
        public Expr left;
        public Expr right;
        public object Clone() => MemberwiseClone();
    }


    class TernaryExpr : ICloneable
    {
        public Expr cond;
        public Expr then_expr;
        public Expr else_expr;
        public object Clone() => MemberwiseClone();
    }


    class CallExpr : ICloneable
    {
        public Expr expr;
        public Expr[] args;
        public int num_args;
        public object Clone() => MemberwiseClone();
    }


    class IndexExpr : ICloneable
    {
        public Expr expr;
        public Expr index;
        public object Clone() => MemberwiseClone();
    }


    class FieldExpr : ICloneable
    {
        public Expr expr;
        public string name;
        public object Clone() => MemberwiseClone();
    }
    enum CompoundFieldKind
    {
        FIELD_DEFAULT,
        FIELD_NAME,
        FIELD_INDEX,
    }

    class CompoundField : ICloneable
    {
        public CompoundFieldKind kind;
        public Expr init;
        public string name;
        public Expr index;
        public object Clone() => MemberwiseClone();
    }

    class Expr : ICloneable
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
        public object Clone() => MemberwiseClone();
    }

    class ElseIf : ICloneable
    {
        public Expr cond;
        public StmtBlock block;
        public object Clone() => MemberwiseClone();
    }


    class IfStmt : ICloneable
    {
        public Expr cond;
        public StmtBlock then_block;
        public ElseIf[] elseifs;
        public int num_elseifs;
        public StmtBlock else_block;
        public object Clone() => MemberwiseClone();
    }


    class WhileStmt : ICloneable
    {
        public Expr cond;
        public StmtBlock block;
        public object Clone() => MemberwiseClone();
    }


    class ForStmt : ICloneable
    {
        public Stmt init;
        public Expr cond;
        public Stmt next;
        public StmtBlock block;
        public object Clone() => MemberwiseClone();
    }


    class SwitchCase : ICloneable
    {
        public Expr[] exprs;
        public int num_exprs;
        public bool is_default;
        public StmtBlock block;
        public object Clone() => MemberwiseClone();
    }


    class SwitchStmt : ICloneable
    {
        public Expr expr;
        public SwitchCase[] cases;
        public int num_cases;
        public object Clone() => MemberwiseClone();
    }


    class AssignStmt : ICloneable
    {
        public TokenKind op;
        public Expr left;
        public Expr right;
        public object Clone() => MemberwiseClone();
    }


    class InitStmt : ICloneable
    {
        public string name;
        public Expr expr;
        public object Clone() => MemberwiseClone();
    }
    
    class Stmt : ICloneable
    {
        public StmtKind kind;
        public IfStmt if_stmt;
        public WhileStmt while_stmt;
        public ForStmt for_stmt;
        public SwitchStmt switch_stmt;
        public StmtBlock block;
        public AssignStmt assign;
        public InitStmt init;
        public Expr expr;
        public Decl decl;
        public object Clone() => MemberwiseClone();
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