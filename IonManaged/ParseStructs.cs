namespace IonLangManaged
{
    internal unsafe struct SrcPos
    {
        public string name;
        public long line;
        public long col;
    }

    internal unsafe struct StmtList
    {
        public Stmt[] stmts;
        public int num_stmts;
        public SrcPos pos;
    }

    unsafe class Label
    {
        public string name;
        public SrcPos pos;
        public bool referenced;
        public bool defined;
    }
    internal struct StmtCtx
    {
        public bool is_break_legal;
        public bool is_continue_legal;
    }

    internal unsafe class Typespec
    {
        public TypespecKind kind;
        public SrcPos pos;
        public Typespec @base;
        public FuncTypespec func;
        public Expr num_elems;
        public Tuple tuple;
        public int num_names;
        public string[] names;

        internal struct Tuple
        {
            public Typespec []fields;
            public int num_fields;
        }

        internal struct FuncTypespec
        {
            public Typespec[] args;
            public int num_args;
            public bool has_varargs;
            public Typespec ret;
        }

    }

    internal unsafe struct FuncParam
    {
        public string name;
        public Typespec type;
        public SrcPos pos;
    }


    internal unsafe struct EnumItem
    {
        public string name;
        public Expr init;
        public SrcPos pos;
    }

    internal unsafe struct Aggregate
    {
        public SrcPos pos;
        public AggregateKind kind;
        public AggregateItem[] items;
        public int num_items;
    }

    internal unsafe class AggregateItem
    {
        public string[] names;
        public int num_names;
        public Typespec type;
        public SrcPos pos;
        public AggregateItemKind kind;
        public Aggregate subaggregate;

    }

    internal unsafe struct ImportItem
    {
        public string name;
        public string rename;
    }

    internal unsafe struct Decls
    {
        public Decl[] decls;
        public int num_decls;
    }


    internal unsafe class Decl
    {
        public DeclKind kind;
        public string name;
        public bool is_incomplete;
        public SrcPos pos;
        public Notes notes;
        public Note note;
        public ImportDecl import;
        public EnumDecl enum_decl;
        public Aggregate aggregate;
        public FuncDecl func;
        public TypedefDecl typedef_decl;
        public VarDecl var;
        public ConstDecl const_decl;


        internal struct FuncDecl
        {
            public FuncParam[] @params;
            public int num_params;
            public bool has_varargs;
            public Typespec varargs_type;
            public Typespec ret_type;
            public StmtList block;
        }


        internal struct EnumDecl
        {
            public Typespec type;
            public EnumItem[] items;
            public int num_items;
        }

        internal struct TypedefDecl
        {
            public Typespec type;
        }

        internal struct VarDecl
        {
            public Typespec type;
            public Expr expr;
        }

        internal struct ConstDecl
        {
            public Typespec type;
            public Expr expr;
        }

        internal struct ImportDecl
        {
            public bool is_relative;
            public string[] names;
            public long num_names;
            public bool import_all;
            public ImportItem[] items;
            public long num_items;
        }
    }
    unsafe class NoteArg
    {
        public SrcPos pos;
        public string name;
        public Expr expr;
    }
    unsafe struct Note
    {
        public SrcPos pos;
        public string name;
        public NoteArg[] args;
        public int num_args;
    }

    unsafe struct Notes
    {
        public Note[] notes;
        public int num_notes;
    }

    internal enum CompoundFieldKind
    {
        FIELD_DEFAULT,
        FIELD_NAME,
        FIELD_INDEX
    }

    internal unsafe struct CompoundField
    {
        public CompoundFieldKind kind;
        public SrcPos pos;
        public Expr init;
        public string name;
        public Expr index;
    }


    internal unsafe class Expr
    {
        public ExprKind kind;
        public SrcPos pos;
        public Type type;
        public string name;
        public _int_lit int_lit;
        public _float_lit float_lit;
        public _str_lit str_lit;
        public Expr typeof_expr;
        public Paren paren;
        public Typespec typeof_type;
        public Expr sizeof_expr;
        public Expr alignof_expr;
        public Typespec alignof_type;
        public _offsetof_field offsetof_field;
        public _modify modify;
        public Typespec sizeof_type;
        public CompoundExpr compound;
        public CastExpr cast;
        public UnaryExpr unary;
        public BinaryExpr binary;
        public TernaryExpr ternary;
        public CallExpr call;
        public IndexExpr index;
        public FieldExpr field;
        public NewExpr new_expr;

        internal struct NewExpr
        {
            public Expr alloc;
            public Expr len;
            public Expr arg;
        }

        internal struct _offsetof_field
        {
            public Typespec type;
            public string name;
        }
        internal struct _int_lit
        {
            public ulong val;
            public TokenMod mod;
            public TokenSuffix suffix;
        }

        internal struct _float_lit
        {
            public double val;
            public string strVal;
            public TokenSuffix suffix;
        }

        internal struct _str_lit
        {
            public string val;
            public TokenMod mod;
        }

        internal struct _modify
        {
            public TokenKind op;
            public bool post;
            public Expr expr;
        }
        internal struct CompoundExpr
        {
            public Typespec type;
            public CompoundField[] fields;
            public int num_fields;
        }

        internal struct Paren
        {
            public Expr expr;
        }

        internal struct CastExpr
        {
            public Typespec type;
            public Expr expr;
        }


        internal struct UnaryExpr
        {
            public TokenKind op;
            public Expr expr;
        }


        internal struct BinaryExpr
        {
            public TokenKind op;
            public Expr left;
            public Expr right;
        }


        internal struct TernaryExpr
        {
            public Expr cond;
            public Expr then_expr;
            public Expr else_expr;
        }


        internal struct CallExpr
        {
            public Expr expr;
            public Expr[] args;
            public int num_args;
        }


        internal struct IndexExpr
        {
            public Expr expr;
            public Expr index;
        }


        internal struct FieldExpr
        {
            public Expr expr;
            public string name;
        }
    }

    unsafe struct SwitchCasePattern
    {
        public Expr start;
        public Expr end;
    }

    internal unsafe struct ElseIf
    {
        public Expr cond;
        public StmtList block;
    }


    internal unsafe struct SwitchCase
    {
        public SwitchCasePattern[] patterns;
        public int num_patterns;
        public bool is_default;
        public StmtList block;
    }


    internal unsafe class Stmt
    {
        public StmtKind kind;
        public SrcPos pos;
        public Notes notes;
        public IfStmt if_stmt;
        public Note note;
        public WhileStmt while_stmt;
        public ForStmt for_stmt;
        public SwitchStmt switch_stmt;
        public StmtList block;
        public AssignStmt assign;
        public InitStmt init;
        public Expr expr;
        public Decl decl;
        public string label;


        internal struct IfStmt
        {
            public Expr cond;
            public Stmt init;
            public StmtList then_block;
            public ElseIf[] elseifs;
            public int num_elseifs;
            public StmtList else_block;
        }


        internal struct WhileStmt
        {
            public Expr cond;
            public StmtList block;
        }


        internal struct ForStmt
        {
            public Stmt init;
            public Expr cond;
            public Stmt next;
            public StmtList block;
        }


        internal struct SwitchStmt
        {
            public Expr expr;
            public SwitchCase[] cases;
            public int num_cases;
        }


        internal struct AssignStmt
        {
            public TokenKind op;
            public Expr left;
            public Expr right;
        }


        internal struct InitStmt
        {
            public string name;
            public Typespec type;
            public Expr expr;
            public bool is_undef;
        }
    }

    enum AggregateKind
    {
        AGGREGATE_NONE,
        AGGREGATE_STRUCT,
        AGGREGATE_UNION,
    }

    enum AggregateItemKind
    {
        AGGREGATE_ITEM_NONE,
        AGGREGATE_ITEM_FIELD,
        AGGREGATE_ITEM_SUBAGGREGATE,
    }

    internal enum TypespecKind
    {
        TYPESPEC_NONE,
        TYPESPEC_NAME,
        TYPESPEC_FUNC,
        TYPESPEC_ARRAY,
        TYPESPEC_PTR,
        TYPESPEC_CONST,
        TYPESPEC_TUPLE
    }

    internal enum DeclKind
    {
        DECL_NONE,
        DECL_ENUM,
        DECL_STRUCT,
        DECL_UNION,
        DECL_VAR,
        DECL_CONST,
        DECL_TYPEDEF,
        DECL_FUNC,
        DECL_NOTE,
        DECL_IMPORT,
    }

    internal enum ExprKind
    {
        EXPR_NONE,
        EXPR_PAREN,
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
        EXPR_MODIFY,
        EXPR_SIZEOF_EXPR,
        EXPR_SIZEOF_TYPE,
        EXPR_TYPEOF_EXPR,
        EXPR_TYPEOF_TYPE,
        EXPR_ALIGNOF_EXPR,
        EXPR_ALIGNOF_TYPE,
        EXPR_OFFSETOF,
        EXPR_NEW,
    }

    internal enum StmtKind
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
        STMT_NOTE,
        STMT_GOTO,
        STMT_LABEL,
    }
}