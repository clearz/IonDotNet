using System.Runtime.InteropServices;

namespace IonLang
{

    internal unsafe struct SrcPos
    {
        public char* name;
        public long line;
        public long col;
    }

    internal unsafe struct StmtList
    {
        public Stmt** stmts;
        public int num_stmts;
        public SrcPos pos;
    }

    unsafe struct Label
    {
        public char *name;
        public SrcPos pos;
        public bool referenced;
        public bool defined;
    }
    internal struct StmtCtx
    {
        public bool is_break_legal;
        public bool is_continue_legal;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Typespec
    {
        [FieldOffset(0)] public TypespecKind kind;
        [FieldOffset(4)] public SrcPos pos;
        [FieldOffset(28)] public Typespec* @base;
        [FieldOffset(28 + Ion.PTR_SIZE)] public char** names;
        [FieldOffset(28 + Ion.PTR_SIZE)] public FuncTypespec func;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Expr *num_elems;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Tuple tuple;
        [FieldOffset(28 + Ion.PTR_SIZE * 2)] public int num_names;

        internal struct Tuple
        {
            public Typespec **fields;
            public int num_fields;
        }

        internal struct FuncTypespec
        {
            public Typespec** args;
            public int num_args;
            public bool has_varargs;
            public Typespec* ret;
        }

    }

    internal unsafe struct FuncParam
    {
        public char* name;
        public Typespec* type;
        public SrcPos pos;
    }


    internal unsafe struct EnumItem
    {
        public char* name;
        public Expr* init;
        public SrcPos pos;
    }

    internal unsafe struct Aggregate
    {
        public SrcPos pos;
        public AggregateKind kind;
        public AggregateItem *items;
        public int num_items;
    }

    internal unsafe struct AggregateItem
    {
        public char** names;
        public int num_names;
        public Typespec* type;
        public SrcPos pos;
        public AggregateItemKind kind;
        public Aggregate* subaggregate;

    }

    internal unsafe struct ImportItem
    {
        public char* name;
        public char* rename;
    }

    internal unsafe struct Decls
    {
        public Decl** decls;
        public int num_decls;
    }


    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Decl
    {
        [FieldOffset(0)] public DeclKind kind;
        [FieldOffset(4)] public char* name;
        [FieldOffset(4  + Ion.PTR_SIZE)] public bool is_incomplete;
        [FieldOffset(8  + Ion.PTR_SIZE)] public SrcPos pos;
        [FieldOffset(32 + Ion.PTR_SIZE)] public Notes notes;
        [FieldOffset(48 + Ion.PTR_SIZE)] public Note note;
        [FieldOffset(48 + Ion.PTR_SIZE)] public ImportDecl import;
        [FieldOffset(48 + Ion.PTR_SIZE)] public EnumDecl enum_decl;
        [FieldOffset(48 + Ion.PTR_SIZE)] public Aggregate* aggregate;
        [FieldOffset(48 + Ion.PTR_SIZE)] public FuncDecl func;
        [FieldOffset(48 + Ion.PTR_SIZE)] public TypedefDecl typedef_decl;
        [FieldOffset(48 + Ion.PTR_SIZE)] public VarDecl var;
        [FieldOffset(48 + Ion.PTR_SIZE)] public ConstDecl const_decl;


        internal struct FuncDecl
        {
            public FuncParam* @params;
            public int num_params;
            public bool has_varargs;
            public Typespec *varargs_type;
            public Typespec* ret_type;
            public StmtList block;
        }


        internal struct EnumDecl
        {
            public Typespec* type;
            public EnumItem* items;
            public int num_items;
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
            public Typespec *type;
            public Expr* expr;
        }

        internal struct ImportDecl
        {
            public bool is_relative;
            public char** names;
            public long num_names;
            public bool import_all;
            public ImportItem *items;
            public long num_items;
        }
    }
    unsafe struct NoteArg
    {
        public SrcPos pos;
        public char *name;
        public Expr *expr;
    }
    unsafe struct Note
    {
        public SrcPos pos;
        public char *name;
        public NoteArg *args;
        public int num_args;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    unsafe struct Notes
    {
        public Note *notes;
        public int num_notes;
    }

    internal enum CompoundFieldKind
    {
        FIELD_DEFAULT,
        FIELD_NAME,
        FIELD_INDEX
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CompoundField
    {
        [FieldOffset(0)] public CompoundFieldKind kind;
        [FieldOffset(4)] public SrcPos pos;
        [FieldOffset(28)] public Expr* init;
        [FieldOffset(Ion.PTR_SIZE + 28)] public char* name;
        [FieldOffset(Ion.PTR_SIZE + 28)] public Expr* index;
    }


    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Expr
    {
        [FieldOffset(0)] public ExprKind kind;
        [FieldOffset(4)] public SrcPos pos;
        [FieldOffset(28)] public Type* type;
        [FieldOffset(28 + Ion.PTR_SIZE)] public char* name;
        [FieldOffset(28 + Ion.PTR_SIZE)] public _int_lit int_lit;
        [FieldOffset(28 + Ion.PTR_SIZE)] public _float_lit float_lit;
        [FieldOffset(28 + Ion.PTR_SIZE)] public _str_lit str_lit;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Expr* typeof_expr;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Paren paren;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Typespec* typeof_type;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Expr* sizeof_expr;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Expr* alignof_expr;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Typespec* alignof_type;
        [FieldOffset(28 + Ion.PTR_SIZE)] public _offsetof_field offsetof_field;
        [FieldOffset(28 + Ion.PTR_SIZE)] public _modify modify;
        [FieldOffset(28 + Ion.PTR_SIZE)] public Typespec* sizeof_type;
        [FieldOffset(28 + Ion.PTR_SIZE)] public CompoundExpr compound;
        [FieldOffset(28 + Ion.PTR_SIZE)] public CastExpr cast;
        [FieldOffset(28 + Ion.PTR_SIZE)] public UnaryExpr unary;
        [FieldOffset(28 + Ion.PTR_SIZE)] public BinaryExpr binary;
        [FieldOffset(28 + Ion.PTR_SIZE)] public TernaryExpr ternary;
        [FieldOffset(28 + Ion.PTR_SIZE)] public CallExpr call;
        [FieldOffset(28 + Ion.PTR_SIZE)] public IndexExpr index;
        [FieldOffset(28 + Ion.PTR_SIZE)] public FieldExpr field;
        [FieldOffset(28 + Ion.PTR_SIZE)] public NewExpr new_expr;

        internal struct NewExpr
        {
            public Expr *alloc;
            public Expr *len;
            public Expr *arg;
        }

        internal struct _offsetof_field
        {
            public Typespec* type;
            public char *name;
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
            public char* start;
            public char* end;
            public TokenSuffix suffix;
        }

        internal struct _str_lit
        {
            public char* val;
            public TokenMod mod;
        }

        internal struct _modify
        {
            public TokenKind op;
            public bool post;
            public Expr* expr;
        }
        internal struct CompoundExpr
        {
            public Typespec* type;
            public CompoundField* fields;
            public int num_fields;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct Paren
        {
            public Expr *expr;
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
            public int num_args;
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

    unsafe struct SwitchCasePattern
    {
        public Expr *start;
        public Expr *end;
    }

    internal unsafe struct ElseIf
    {
        public Expr* cond;
        public StmtList block;
    }


    internal unsafe struct SwitchCase
    {
        public SwitchCasePattern *patterns;
        public int  num_patterns;
        public bool is_default;
        public StmtList block;
    }


    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Stmt
    {
        [FieldOffset(0)] public StmtKind kind;
        [FieldOffset(4)] public SrcPos pos;
        [FieldOffset(28)] public Notes notes;
        [FieldOffset(44)] public IfStmt if_stmt;
        [FieldOffset(44)] public Note note;
        [FieldOffset(44)] public WhileStmt while_stmt;
        [FieldOffset(44)] public ForStmt for_stmt;
        [FieldOffset(44)] public SwitchStmt switch_stmt;
        [FieldOffset(44)] public StmtList block;
        [FieldOffset(44)] public AssignStmt assign;
        [FieldOffset(44)] public InitStmt init;
        [FieldOffset(44)] public Expr* expr;
        [FieldOffset(44)] public Decl* decl;
        [FieldOffset(44)] public char* label;


        internal struct IfStmt
        {
            public Expr* cond;
            public Stmt *init;
            public StmtList then_block;
            public ElseIf** elseifs;
            public int num_elseifs;
            public StmtList else_block;
        }


        internal struct WhileStmt
        {
            public Expr* cond;
            public StmtList block;
        }


        internal struct ForStmt
        {
            public Stmt* init;
            public Expr* cond;
            public Stmt* next;
            public StmtList block;
        }


        internal struct SwitchStmt
        {
            public Expr* expr;
            public SwitchCase* cases;
            public int num_cases;
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
            public Typespec *type;
            public Expr* expr;
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