// Forward includes
#include <stdio.h>

// Preamble
#include <stdbool.h>
#include <stdint.h>
 #include <assert.h>
#include <stddef.h>

typedef unsigned char uchar;
typedef signed char schar;
typedef unsigned short ushort;
typedef unsigned int uint;
typedef unsigned long ulong;
typedef long long llong;
typedef unsigned long long ullong;

typedef uint8_t uint8;
typedef int8_t int8;
typedef uint16_t uint16;
typedef int16_t int16;
typedef uint32_t uint32;
typedef int32_t int32;
typedef uint64_t uint64;
typedef int64_t int64;

typedef uintptr_t uintptr;
typedef size_t usize;
typedef ptrdiff_t ssize;
typedef int typeid;

#ifdef _MSC_VER
#define alignof(x) __alignof(x)
#else
#define alignof(x) __alignof__(x)
#endif

// Forward declarations
typedef struct TypeFieldInfo TypeFieldInfo;
typedef struct TypeInfo TypeInfo;
typedef struct Any Any;
typedef struct SomeIncompleteType SomeIncompleteType;
typedef struct S1 S1;
typedef struct S2 S2;
typedef struct UartCtrl UartCtrl;
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;
typedef struct ConstVector ConstVector;
typedef struct Ints Ints;
typedef struct BufHdr BufHdr;
typedef struct Thing Thing;

// Sorted declarations
#line 4 "<builtin>"
typedef int TypeKind;

#line 265 "test1.ion"
typedef int Color;

#line 5 "<builtin>"
#define TYPE_NONE ((int)(0))

#line 6
#define TYPE_VOID ((int)((TYPE_NONE) + (1)))

#line 7
#define TYPE_BOOL ((int)((TYPE_VOID) + (1)))

#line 8
#define TYPE_CHAR ((int)((TYPE_BOOL) + (1)))

#line 9
#define TYPE_UCHAR ((int)((TYPE_CHAR) + (1)))

#line 10
#define TYPE_SCHAR ((int)((TYPE_UCHAR) + (1)))

#line 11
#define TYPE_SHORT ((int)((TYPE_SCHAR) + (1)))

#line 12
#define TYPE_USHORT ((int)((TYPE_SHORT) + (1)))

#line 13
#define TYPE_INT ((int)((TYPE_USHORT) + (1)))

#line 14
#define TYPE_UINT ((int)((TYPE_INT) + (1)))

#line 15
#define TYPE_LONG ((int)((TYPE_UINT) + (1)))

#line 16
#define TYPE_ULONG ((int)((TYPE_LONG) + (1)))

#line 17
#define TYPE_LLONG ((int)((TYPE_ULONG) + (1)))

#line 18
#define TYPE_ULLONG ((int)((TYPE_LLONG) + (1)))

#line 19
#define TYPE_FLOAT ((int)((TYPE_ULLONG) + (1)))

#line 20
#define TYPE_DOUBLE ((int)((TYPE_FLOAT) + (1)))

#line 21
#define TYPE_CONST ((int)((TYPE_DOUBLE) + (1)))

#line 22
#define TYPE_PTR ((int)((TYPE_CONST) + (1)))

#line 23
#define TYPE_ARRAY ((int)((TYPE_PTR) + (1)))

#line 24
#define TYPE_STRUCT ((int)((TYPE_ARRAY) + (1)))

#line 25
#define TYPE_UNION ((int)((TYPE_STRUCT) + (1)))

#line 26
#define TYPE_FUNC ((int)((TYPE_UNION) + (1)))

#line 29
struct TypeFieldInfo {
    #line 30
    const char(*name);
    #line 31
    typeid type;
    #line 32
    int offset;
};

#line 35
struct TypeInfo {
    #line 36
    TypeKind kind;
    #line 37
    int size;
    #line 38
    int align;
    #line 39
    const char(*name);
    #line 40
    int count;
    #line 41
    typeid base;
    #line 42
    TypeFieldInfo(*fields);
    #line 43
    int num_fields;
};

#line 52
const TypeInfo * get_typeinfo(typeid type);

#line 60
struct Any {
    #line 61
    void(*ptr);
    #line 62
    typeid type;
};

#line 17 "test1.ion"
extern char (*esc_test_str);

#line 19
extern int(*some_array);

extern SomeIncompleteType(*incomplete_ptr);

#line 36
#define PI (3.14f)

#line 37
#define PI2 ((PI) + (PI))

#line 39
#define U8 ((uint8)(42))

#line 41
extern char c;

#line 42
extern uchar uc;

#line 43
extern schar sc;

#line 45
#define N ((((char)(42)) + (8)) != (0))

#line 164
uchar h(void);

#line 47
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 49
extern char (*code);

#line 58
struct S1 {
    #line 59
    int a;
    #line 60
    const int (b);
};

#line 63
struct S2 {
    #line 64
    S1 s1;
};

#line 67
void test_modify(void);

#line 86
void f10(int (a[3]));

void test_arrays(void);

#line 96
void test_loops(void);

#line 130
void test_nonmodifiable(void);

#line 142
struct UartCtrl {
    #line 143
    bool tx_enable;
    #line 143
    bool rx_enable;
};

#line 146
#define UART_CTRL_REG ((uint *)(0x12345678))

#line 148
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 233
struct Vector {
    #line 234
    int x;
    #line 234
    int y;
};

#line 191
typedef IntOrPtr U;

#line 197
union IntOrPtr {
    #line 198
    int i;
    #line 199
    int(*p);
};

#line 174
int g(U u);

void k(void(*vp), int(*ip));

#line 183
void f1(void);

#line 188
void f3(int (a[]));

#line 193
int example_test(void);

#line 249
int fact_rec(int n);

#line 241
int fact_iter(int n);

#line 202
extern const char (escape_to_char[256]);

#line 212
extern int (a2[11]);

#line 215
int is_even(int digit);

#line 231
extern int i;

#line 237
void f2(Vector v);

#line 259
extern T(*p);

#line 257
#define M ((1) + (sizeof(p)))

struct T {
    #line 262
    int (a[M]);
};

#line 266
#define COLOR_NONE ((int)(0))

#line 267
#define COLOR_RED ((int)((COLOR_NONE) + (1)))

#line 268
#define COLOR_GREEN ((int)((COLOR_RED) + (3)))

#line 269
#define COLOR_BLUE ((int)((COLOR_GREEN) + (1)))

#line 270
#define NUM_COLORS ((int)((COLOR_BLUE) + (1)))

#line 273
extern const char * (color_names[NUM_COLORS]);

#line 280
void test_enum(void);

#line 289
void test_assign(void);

#line 312
void benchmark(int n);

#line 319
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 325
void test_lits(void);

#line 342
void test_ops(void);

#line 372
#define IS_DEBUG (true)

#line 374
void test_bool(void);

#line 381
int test_ctrl(void);

#line 391
extern const int (j);

#line 392
extern const int(*q);

#line 393
extern const Vector (cv);

#line 395
void f4(const char(*x));

#line 398
struct ConstVector {
    #line 399
    const int (x);
    #line 399
    const int (y);
};

#line 402
void f5(const int(*p));

#line 405
void test_convert(void);

#line 413
void test_const(void);

#line 438
void test_init(void);

#line 454
void test_sizeof(void);

#line 462
void test_cast(void);

#line 471
void print_any(Any any);

#line 493
void print_type(typeid type);

#line 484
void println_any(Any any);

#line 489
void print_typeid(typeid type);

#line 518
void println_type(typeid type);

#line 523
void print_typeinfo(typeid type);

#line 547
void println_typeinfo(typeid type);

#line 552
void test_typeinfo(void);

#line 572
struct Ints {
    #line 573
    int num_ints;
    #line 574
    int(*int_ptr);
    #line 575
    int (int_arr[3]);
};

#line 578
void test_compound_literals(void);

#line 594
void test_complete(void);

#line 622
void test_alignof(void);

#line 630
struct BufHdr {
    #line 631
    usize cap;
    #line 631
    usize len;
    #line 632
    char (buf[1]);
};

#line 637
void test_offsetof(void);

#line 642
struct Thing {
    #line 643
    int a;
};

#line 646
extern Thing thing;

#line 648
Thing * returns_ptr(void);

const Thing * returns_ptr_to_const(void);

void test_lvalue(void);

#line 663
void test_if(void);

#line 677
int main(int argc, const char *(*argv));

// Typeinfo

const TypeInfo *typeinfo_table[100] = {
    [0] = NULL, // No associated type
    [1] = &(TypeInfo){TYPE_VOID, .name = "void", .size = 0, .align = 0},
    [2] = &(TypeInfo){TYPE_BOOL, .size = sizeof(bool), .align = alignof(bool), .name = "bool"},
    [3] = &(TypeInfo){TYPE_CHAR, .size = sizeof(char), .align = alignof(char), .name = "char"},
    [4] = &(TypeInfo){TYPE_UCHAR, .size = sizeof(uchar), .align = alignof(uchar), .name = "uchar"},
    [5] = &(TypeInfo){TYPE_SCHAR, .size = sizeof(schar), .align = alignof(schar), .name = "schar"},
    [6] = &(TypeInfo){TYPE_SHORT, .size = sizeof(short), .align = alignof(short), .name = "short"},
    [7] = &(TypeInfo){TYPE_USHORT, .size = sizeof(ushort), .align = alignof(ushort), .name = "ushort"},
    [8] = &(TypeInfo){TYPE_INT, .size = sizeof(int), .align = alignof(int), .name = "int"},
    [9] = &(TypeInfo){TYPE_UINT, .size = sizeof(uint), .align = alignof(uint), .name = "uint"},
    [10] = &(TypeInfo){TYPE_LONG, .size = sizeof(long), .align = alignof(long), .name = "long"},
    [11] = &(TypeInfo){TYPE_ULONG, .size = sizeof(ulong), .align = alignof(ulong), .name = "ulong"},
    [12] = &(TypeInfo){TYPE_LLONG, .size = sizeof(llong), .align = alignof(llong), .name = "llong"},
    [13] = &(TypeInfo){TYPE_ULLONG, .size = sizeof(ullong), .align = alignof(ullong), .name = "ullong"},
    [14] = &(TypeInfo){TYPE_FLOAT, .size = sizeof(float), .align = alignof(float), .name = "float"},
    [15] = &(TypeInfo){TYPE_DOUBLE, .size = sizeof(double), .align = alignof(double), .name = "double"},
    [16] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 1},
    [17] = &(TypeInfo){TYPE_CONST, .size = sizeof(const void *), .align = alignof(const void *), .base = 16},
    [18] = NULL, // Enum
    [19] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"name", .type = 35, .offset = offsetof(TypeFieldInfo, name)},
        {"type", .type = 8, .offset = offsetof(TypeFieldInfo, type)},
        {"offset", .type = 8, .offset = offsetof(TypeFieldInfo, offset)},}},
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
        {"kind", .type = 18, .offset = offsetof(TypeInfo, kind)},
        {"size", .type = 8, .offset = offsetof(TypeInfo, size)},
        {"align", .type = 8, .offset = offsetof(TypeInfo, align)},
        {"name", .type = 35, .offset = offsetof(TypeInfo, name)},
        {"count", .type = 8, .offset = offsetof(TypeInfo, count)},
        {"base", .type = 8, .offset = offsetof(TypeInfo, base)},
        {"fields", .type = 36, .offset = offsetof(TypeInfo, fields)},
        {"num_fields", .type = 8, .offset = offsetof(TypeInfo, num_fields)},}},
    [21] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = 16, .offset = offsetof(Any, ptr)},
        {"type", .type = 8, .offset = offsetof(Any, type)},}},
    [22] = NULL, // Incomplete: SomeIncompleteType
    [23] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S1), .align = alignof(S1), .name = "S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(S1, a)},
        {"b", .type = 53, .offset = offsetof(S1, b)},}},
    [24] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S2), .align = alignof(S2), .name = "S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = 23, .offset = offsetof(S2, s1)},}},
    [25] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(UartCtrl), .align = alignof(UartCtrl), .name = "UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = 2, .offset = offsetof(UartCtrl, tx_enable)},
        {"rx_enable", .type = 2, .offset = offsetof(UartCtrl, rx_enable)},}},
    [26] = &(TypeInfo){TYPE_UNION, .size = sizeof(IntOrPtr), .align = alignof(IntOrPtr), .name = "IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = 8, .offset = offsetof(IntOrPtr, i)},
        {"p", .type = 48, .offset = offsetof(IntOrPtr, p)},}},
    [27] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Vector), .align = alignof(Vector), .name = "Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 8, .offset = offsetof(Vector, x)},
        {"y", .type = 8, .offset = offsetof(Vector, y)},}},
    [28] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(T), .align = alignof(T), .name = "T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 69, .offset = offsetof(T, a)},}},
    [29] = NULL, // Enum
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 53, .offset = offsetof(ConstVector, x)},
        {"y", .type = 53, .offset = offsetof(ConstVector, y)},}},
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Ints), .align = alignof(Ints), .name = "Ints", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"num_ints", .type = 8, .offset = offsetof(Ints, num_ints)},
        {"int_ptr", .type = 48, .offset = offsetof(Ints, int_ptr)},
        {"int_arr", .type = 50, .offset = offsetof(Ints, int_arr)},}},
    [32] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(BufHdr), .align = alignof(BufHdr), .name = "BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = 13, .offset = offsetof(BufHdr, cap)},
        {"len", .type = 13, .offset = offsetof(BufHdr, len)},
        {"buf", .type = 92, .offset = offsetof(BufHdr, buf)},}},
    [33] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Thing), .align = alignof(Thing), .name = "Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(Thing, a)},}},
    [34] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = 3},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 34},
    [36] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 19},
    [37] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeInfo), .align = alignof(const TypeInfo), .base = 20},
    [38] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 37},
    [39] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 38},
    [40] = NULL, // Func
    [41] = NULL, // Func
    [42] = NULL, // Func
    [43] = NULL, // Func
    [44] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = 1},
    [45] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 44},
    [46] = NULL, // Func
    [47] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
    [48] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [49] = NULL, // Incomplete array type
    [50] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [51] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 22},
    [52] = NULL, // Func
    [53] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = 8},
    [54] = NULL, // Func
    [55] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = 8, .count = 16},
    [56] = NULL, // Func
    [57] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [58] = NULL, // Func
    [59] = NULL, // Func
    [60] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [61] = NULL, // Func
    [62] = NULL, // Func
    [63] = NULL, // Func
    [64] = NULL, // Func
    [65] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 34, .count = 256},
    [66] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [67] = NULL, // Func
    [68] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 28},
    [69] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = 8, .count = 9},
    [70] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [6]), .align = alignof(const char * [6]), .base = 35, .count = 6},
    [71] = NULL, // Func
    [72] = NULL, // Func
    [73] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 53},
    [74] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 27},
    [75] = NULL, // Func
    [76] = NULL, // Func
    [77] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = 8, .count = 4},
    [78] = NULL, // Func
    [79] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [80] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 79},
    [81] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = 18},
    [82] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = 35},
    [83] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = 36},
    [84] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [85] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 16},
    [86] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 73, .count = 42},
    [87] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [88] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = 21},
    [89] = NULL, // Incomplete array type
    [90] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [2]), .align = alignof(int [2]), .base = 8, .count = 2},
    [91] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(Ints [2]), .align = alignof(Ints [2]), .base = 31, .count = 2},
    [92] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = 3, .count = 1},
    [93] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 33},
    [94] = NULL, // Func
    [95] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Thing), .align = alignof(const Thing), .base = 33},
    [96] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 95},
    [97] = NULL, // Func
    [98] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 35},
    [99] = NULL, // Func
};
int num_typeinfos = 100;
const TypeInfo **typeinfos = (const TypeInfo **)typeinfo_table;

// Definitions

#line 52 "<builtin>"
const TypeInfo * get_typeinfo(typeid type) {
    #line 53
    if ((typeinfos) && ((type) < (num_typeinfos))) {
        #line 54
        return typeinfos[type];
    } else {
        #line 56
        return NULL;
    }
}

char (*esc_test_str) = "Hello\nworld\nHex: \xFHello\xFF";

int(*some_array) = (int []){1, 2, 3};

SomeIncompleteType(*incomplete_ptr);

char c = 1;

uchar uc = 1;

schar sc = 1;

char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 67 "test1.ion"
void test_modify(void) {
    #line 68
    int i = 42;
    #line 69
    int (*p) = &(i);
    #line 70
    (p)--;
    #line 71
    int x = *((p)++);
    #line 72
    assert((x) == (*(--(p))));
    #line 73
    (*(p))++;
    #line 74
    (*(p))--;
    #line 75
    int (stk[16]);
    #line 76
    int(*sp) = stk;
    #line 77
    (*((sp)++)) = 1;
    #line 78
    (*((sp)++)) = 2;
    #line 79
    (x) = *(--(sp));
    #line 80
    assert((x) == (2));
    #line 81
    (x) = *(--(sp));
    #line 82
    assert((x) == (1));
    #line 83
    assert((sp) == (stk));
}

#line 86
void f10(int (a[3])) {
    #line 87
    (a[1]) = 42;
}

#line 90
void test_arrays(void) {
    #line 91
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 96
void test_loops(void) {
    #line 99
    switch (0) {default: {
            if (1) {
                #line 102
                break;
            }
            #line 104
            for (;;) {
                #line 105
                continue;
            }
            break;
        }
    }
    #line 110
    while (0) {
    }
    #line 112
    for (int i = 0; (i) < (10); (i)++) {
    }
    #line 114
    for (;;) {
        #line 115
        break;
    }
    #line 117
    for (int i = 0;;) {
        #line 118
        break;
    }
    #line 120
    for (; 0;) {
    }
    #line 122
    for (int i = 0;; (i)++) {
        #line 123
        break;
    }
    #line 125
    int i = 0;
    #line 126
    for (;; (i)++) {
        #line 127
        break;
    }
}

#line 130
void test_nonmodifiable(void) {
    #line 131
    S1 s1;
    #line 132
    (s1.a) = 0;
    #line 135
    S2 s2;
    #line 136
    (s2.s1.a) = 0;
}

#line 148
uint32 pack(UartCtrl ctrl) {
    #line 149
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 152
UartCtrl unpack(uint32 word) {
    #line 153
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 156
void test_uart(void) {
    #line 157
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 158
    (*(UART_CTRL_REG)) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 159
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 160
    (ctrl.rx_enable) = true;
    #line 161
    (*(UART_CTRL_REG)) = (pack)(ctrl);
}

#line 164
uchar h(void) {
    #line 165
    ((Vector){.x = 1, .y = 2}.x) = 42;
    #line 166
    Vector (*v) = &((Vector){1, 2});
    #line 167
    (v->x) = 42;
    #line 168
    int (*p) = &((int){0});
    #line 169
    ulong x = ((uint){1}) + ((long){2});
    #line 170
    int y = +(c);
    #line 171
    return (uchar)(x);
}

#line 174
int g(U u) {
    #line 175
    return u.i;
}

#line 178
void k(void(*vp), int(*ip)) {
    #line 179
    (vp) = ip;
    #line 180
    (ip) = vp;
}

#line 183
void f1(void) {
    #line 184
    int (*p) = &((int){0});
    #line 185
    (*(p)) = 42;
}

#line 188
void f3(int (a[])) {
}

#line 193
int example_test(void) {
    #line 194
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 215
int is_even(int digit) {
    #line 216
    int b = 0;
    #line 217
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 219
            (b) = 1;
            break;
        }
    }
    #line 221
    return b;
}

int i;

#line 237
void f2(Vector v) {
    #line 238
    (v) = (Vector){0};
}

#line 241
int fact_iter(int n) {
    #line 242
    int r = 1;
    #line 243
    for (int i = 0; (i) <= (n); (i)++) {
        #line 244
        (r) *= i;
    }
    #line 246
    return r;
}

#line 249
int fact_rec(int n) {
    #line 250
    if ((n) == (0)) {
        #line 251
        return 1;
    } else {
        #line 253
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 280
void test_enum(void) {
    #line 281
    Color a = COLOR_RED;
    #line 282
    int b = COLOR_RED;
    #line 283
    int c = (a) + (b);
    #line 284
    int i = a;
    #line 285
    (a) = i;
    #line 286
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 289
void test_assign(void) {
    #line 290
    int i = 0;
    #line 291
    float f = 3.14f;
    #line 292
    int(*p) = &(i);
    #line 293
    (i)++;
    #line 294
    (i)--;
    #line 295
    (p)++;
    #line 296
    (p)--;
    #line 297
    (p) += 1;
    #line 298
    (i) /= 2;
    #line 299
    (i) *= 123;
    #line 300
    (i) %= 3;
    #line 301
    (i) <<= 1;
    #line 302
    (i) >>= 2;
    #line 303
    (i) &= 0xFF;
    #line 304
    (i) |= 0xFF00;
    #line 305
    (i) ^= 0xFF0;
}

#line 312
void benchmark(int n) {
    #line 313
    int r = 1;
    #line 314
    for (int i = 1; (i) <= (n); (i)++) {
        #line 315
        (r) *= i;
    }
}

#line 319
int va_test(int x, ...) {
    #line 320
    return 0;
}

#line 325
void test_lits(void) {
    #line 326
    float f = 3.14f;
    #line 327
    double d = 3.14;
    #line 328
    int i = 1;
    #line 329
    uint u = 0xFFFFFFFFu;
    #line 330
    long l = 1l;
    #line 331
    ulong ul = 1ul;
    #line 332
    llong ll = 0x100000000ll;
    #line 333
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 334
    uint x1 = 0xFFFFFFFF;
    #line 335
    llong x2 = 4294967295;
    #line 336
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 337
    int x4 = (0xAA) + (0x55);
}

#line 342
void test_ops(void) {
    #line 343
    float pi = 3.14f;
    #line 344
    float f = 0.0f;
    #line 345
    (f) = +(pi);
    #line 346
    (f) = -(pi);
    #line 347
    int n = -(1);
    #line 348
    (n) = ~(n);
    #line 349
    (f) = ((f) * (pi)) + (n);
    #line 350
    (f) = (pi) / (pi);
    #line 351
    (n) = (3) % (2);
    #line 352
    (n) = (n) + ((uchar)(1));
    #line 353
    int (*p) = &(n);
    #line 354
    (p) = (p) + (1);
    #line 355
    (n) = (int)(((p) + (1)) - (p));
    #line 356
    (n) = (n) << (1);
    #line 357
    (n) = (n) >> (1);
    #line 358
    int b = ((p) + (1)) > (p);
    #line 359
    (b) = ((p) + (1)) >= (p);
    #line 360
    (b) = ((p) + (1)) < (p);
    #line 361
    (b) = ((p) + (1)) <= (p);
    #line 362
    (b) = ((p) + (1)) == (p);
    #line 363
    (b) = (1) > (2);
    #line 364
    (b) = (1.23f) <= (pi);
    #line 365
    (n) = 0xFF;
    #line 366
    (b) = (n) & (~(1));
    #line 367
    (b) = (n) & (1);
    #line 368
    (b) = ((n) & (~(1))) ^ (1);
    #line 369
    (b) = (p) && (pi);
}

#line 374
void test_bool(void) {
    #line 375
    bool b = false;
    #line 376
    (b) = true;
    #line 377
    int i = 0;
    #line 378
    (i) = IS_DEBUG;
}

#line 381
int test_ctrl(void) {
    #line 382
    switch (1) {
        case 0: {
            #line 384
            return 0;
            break;
        }default: {
            #line 386
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 395
void f4(const char(*x)) {
}

#line 402
void f5(const int(*p)) {
}

#line 405
void test_convert(void) {
    #line 406
    const int(*a) = 0;
    #line 407
    int(*b) = 0;
    #line 408
    (a) = b;
    #line 409
    void(*p) = 0;
    #line 410
    (f5)(p);
}

#line 413
void test_const(void) {
    #line 414
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 417
    (i) = 1;
    #line 420
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 425
    const char (*p) = (const char *)(0);
    #line 426
    (p) = (escape_to_char) + (1);
    #line 427
    char (*q) = (char *)(escape_to_char);
    #line 428
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 433
    (i) = (int)((ullong)(p));
}

#line 438
void test_init(void) {
    #line 439
    int x = (const int)(0);
    #line 440
    #line 441
    int y;
    #line 442
    (y) = 0;
    #line 443
    int z = 42;
    #line 444
    int (a[3]) = {1, 2, 3};
    #line 447
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 448
        (printf)("%llu\n", i);
    }
    #line 450
    int (b[4]) = {1, 2, 3, 4};
    #line 451
    (b[0]) = a[2];
}

#line 454
void test_sizeof(void) {
    #line 455
    int i = 0;
    #line 456
    ullong n = sizeof(i);
    #line 457
    (n) = sizeof(int);
    #line 458
    (n) = sizeof(int);
    #line 459
    (n) = sizeof(int *);
}

#line 462
void test_cast(void) {
    #line 463
    int(*p) = 0;
    #line 464
    uint64 a = 0;
    (a) = (uint64)(p);
    (p) = (int *)(a);
}

#line 471
void print_any(Any any) {
    #line 472
    switch (any.type) {
        case 8: {
            #line 474
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 476
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 478
            (printf)("<unknown>");
            break;
        }
    }
    #line 480
    (printf)(": ");
    #line 481
    (print_type)(any.type);
}

#line 484
void println_any(Any any) {
    #line 485
    (print_any)(any);
    #line 486
    (printf)("\n");
}

#line 489
void print_typeid(typeid type) {
    #line 490
    (printf)("typeid(%d)", type);
}

#line 493
void print_type(typeid type) {
    #line 494
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 495
    if (!(typeinfo)) {
        #line 496
        (print_typeid)(type);
        #line 497
        return;
    }
    #line 499
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 501
            (print_type)(typeinfo->base);
            #line 502
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 504
            (print_type)(typeinfo->base);
            #line 505
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 507
            (print_type)(typeinfo->base);
            #line 508
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 510
            if (typeinfo->name) {
                #line 511
                (printf)("%s", typeinfo->name);
            } else {
                #line 513
                (print_typeid)(type);
            }
            break;
        }
    }
}

#line 518
void println_type(typeid type) {
    #line 519
    (print_type)(type);
    #line 520
    (printf)("\n");
}

#line 523
void print_typeinfo(typeid type) {
    #line 524
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 525
    if (!(typeinfo)) {
        #line 526
        (print_typeid)(type);
        #line 527
        return;
    }
    #line 529
    (printf)("<");
    #line 530
    (print_type)(type);
    #line 531
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 532
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 535
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 536
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 537
                TypeFieldInfo field = typeinfo->fields[i];
                #line 538
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 539
                (print_type)(field.type);
                #line 540
                (printf)("; ");
            }
            #line 542
            (printf)("}");
            break;
        }
    }
    #line 544
    (printf)(">");
}

#line 547
void println_typeinfo(typeid type) {
    #line 548
    (print_typeinfo)(type);
    #line 549
    (printf)("\n");
}

#line 552
void test_typeinfo(void) {
    #line 553
    int i = 42;
    #line 554
    float f = 3.14f;
    #line 555
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 558
    (println_any)((Any){&(f), 14});
    #line 559
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 562
    (println_type)(73);
    #line 563
    (println_type)(86);
    #line 564
    (println_type)(25);
    (println_typeinfo)(8);
    #line 567
    (println_typeinfo)(25);
    #line 568
    (println_typeinfo)(87);
    #line 569
    (println_typeinfo)(26);
}

#line 578
void test_compound_literals(void) {
    #line 579
    int (a[3]) = {1, 2, 3};
    #line 580
    int i = 42;
    #line 581
    const Any (x) = {&(i), 8};
    #line 582
    Any y = {&(i), 8};
    #line 583
    Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 588
    Ints (ints_of_ints[2]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

void test_complete(void) {
    #line 595
    int x = 0;
    #line 598
    int y = 0;
    if ((x) == (0)) {
        #line 601
        (y) = 1;
    } else if ((x) == (1)) {
        #line 603
        (y) = 2;
    } else {
        #line 599
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 606
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 614
    switch (x) {
        case 0: {
            #line 616
            (y) = 3;
            break;
        }
        case 1: {
            #line 618
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 622
void test_alignof(void) {
    #line 623
    int i = 42;
    #line 624
    ullong n1 = alignof(int);
    #line 625
    ullong n2 = alignof(int);
    #line 626
    ullong n3 = alignof(ullong);
    #line 627
    ullong n4 = alignof(int *);
}

#line 637
void test_offsetof(void) {
    #line 638
    ullong n = offsetof(BufHdr, buf);
}

Thing thing;

Thing * returns_ptr(void) {
    #line 649
    return &(thing);
}

#line 652
const Thing * returns_ptr_to_const(void) {
    #line 653
    return &(thing);
}

#line 656
void test_lvalue(void) {
    #line 657
    ((returns_ptr)()->a) = 5;
    const Thing (*p) = (returns_ptr_to_const)();
}

#line 663
void test_if(void) {
    #line 664
    if (1) {
    }
    #line 666
    {
        #line 666
        Thing (*x) = &((Thing){0});
        if (x) {
            #line 667
            (x->a) = 3;
        }
    }
    #line 669
    {
        #line 669
        int x = 42;
        if (x) {
            #line 670
            (x) = 43;
        }
    }
    #line 672
    {
        #line 672
        int x = 42;
        if ((x) >= (0)) {
            #line 673
            (x) = 2;
        }
    }
}

#line 677
int main(int argc, const char *(*argv)) {
    #line 678
    if ((argv) == (0)) {
        #line 679
        (printf)("argv is null\n");
    }
    #line 682
    (test_if)();
    #line 683
    (test_modify)();
    #line 684
    (test_lvalue)();
    #line 685
    (test_alignof)();
    #line 686
    (test_offsetof)();
    #line 687
    (test_complete)();
    #line 688
    (test_compound_literals)();
    #line 689
    (test_loops)();
    #line 690
    (test_sizeof)();
    #line 691
    (test_assign)();
    #line 692
    (test_enum)();
    #line 693
    (test_arrays)();
    #line 694
    (test_cast)();
    #line 695
    (test_init)();
    #line 696
    (test_lits)();
    #line 697
    (test_const)();
    #line 698
    (test_bool)();
    #line 699
    (test_ops)();
    #line 700
    (test_typeinfo)();
    #line 701
    (getchar)();
    #line 702
    return 0;
}
