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
typedef struct BufHdr BufHdr;
typedef struct Thing Thing;

// Sorted declarations
#line 3 "<builtin>"
typedef enum TypeKind {
    TYPE_NONE,
    TYPE_VOID,
    TYPE_BOOL,
    TYPE_CHAR,
    TYPE_UCHAR,
    TYPE_SCHAR,
    TYPE_SHORT,
    TYPE_USHORT,
    TYPE_INT,
    TYPE_UINT,
    TYPE_LONG,
    TYPE_ULONG,
    TYPE_LLONG,
    TYPE_ULLONG,
    TYPE_FLOAT,
    TYPE_DOUBLE,
    TYPE_CONST,
    TYPE_PTR,
    TYPE_ARRAY,
    TYPE_STRUCT,
    TYPE_UNION,
    TYPE_FUNC,
}TypeKind;

#line 259 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 28 "<builtin>"
struct TypeFieldInfo {
    #line 29
    const char(*name);
    #line 30
    typeid type;
    #line 31
    int offset;
};

#line 34
struct TypeInfo {
    #line 35
    TypeKind kind;
    #line 36
    int size;
    #line 37
    int align;
    #line 38
    const char(*name);
    #line 39
    int count;
    #line 40
    typeid base;
    #line 41
    TypeFieldInfo(*fields);
    #line 42
    int num_fields;
};

#line 51
const TypeInfo * get_typeinfo(typeid type);

#line 59
struct Any {
    #line 60
    void(*ptr);
    #line 61
    typeid type;
};

#line 17 "test1.ion"
extern SomeIncompleteType(*incomplete_ptr);

#line 30
#define PI 3.14f

#line 31
#define PI2 (PI) + (PI)

#line 33
#define U8 (uint8)(42)

#line 35
extern char c;

#line 36
extern uchar uc;

#line 37
extern schar sc;

#line 39
#define N (((char)(42)) + (8)) != (0)

#line 158
uchar h(void);

#line 41
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 43
extern char (*code);

#line 52
struct S1 {
    #line 53
    int a;
    #line 54
    const int (b);
};

#line 57
struct S2 {
    #line 58
    S1 s1;
};

#line 61
void test_modify(void);

#line 80
void f10(int (a[3]));

void test_arrays(void);

#line 90
void test_loops(void);

#line 124
void test_nonmodifiable(void);

#line 136
struct UartCtrl {
    #line 137
    bool tx_enable;
    #line 137
    bool rx_enable;
};

#line 140
#define UART_CTRL_REG (uint *)(0x12345678)

#line 142
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 227
struct Vector {
    #line 228
    int x;
    #line 228
    int y;
};

#line 185
typedef IntOrPtr U;

#line 191
union IntOrPtr {
    #line 192
    int i;
    #line 193
    int(*p);
};

#line 168
int g(U u);

void k(void(*vp), int(*ip));

#line 177
void f1(void);

#line 182
void f3(int (a[]));

#line 187
int example_test(void);

#line 243
int fact_rec(int n);

#line 235
int fact_iter(int n);

#line 196
extern const char (escape_to_char[256]);

#line 206
extern int (a2[11]);

#line 209
int is_even(int digit);

#line 225
extern int i;

#line 231
void f2(Vector v);

#line 253
extern T(*p);

#line 251
#define M (1) + (sizeof(p))

struct T {
    #line 256
    int (a[M]);
};

#line 267
extern const char * (color_names[NUM_COLORS]);

#line 274
void test_enum(void);

#line 283
void test_assign(void);

#line 306
void benchmark(int n);

#line 313
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 319
void test_lits(void);

#line 336
void test_ops(void);

#line 366
#define IS_DEBUG true

#line 368
void test_bool(void);

#line 375
int test_ctrl(void);

#line 385
extern const int (j);

#line 386
extern const int(*q);

#line 387
extern const Vector (cv);

#line 389
void f4(const char(*x));

#line 392
struct ConstVector {
    #line 393
    const int (x);
    #line 393
    const int (y);
};

#line 396
void f5(const int(*p));

#line 399
void test_convert(void);

#line 407
void test_const(void);

#line 430
void test_init(void);

#line 445
void test_sizeof(void);

#line 453
void test_cast(void);

#line 462
void print_any(Any any);

#line 484
void print_type(typeid type);

#line 475
void println_any(Any any);

#line 480
void print_typeid(typeid type);

#line 509
void println_type(typeid type);

#line 514
void print_typeinfo(typeid type);

#line 538
void println_typeinfo(typeid type);

#line 543
void test_typeinfo(void);

#line 563
void test_compound_literals(void);

#line 569
void test_complete(void);

#line 597
void test_alignof(void);

#line 605
struct BufHdr {
    #line 606
    usize cap;
    #line 606
    usize len;
    #line 607
    char (buf[1]);
};

#line 612
void test_offsetof(void);

#line 617
struct Thing {
    #line 618
    int a;
};

#line 621
extern Thing thing;

#line 623
Thing * returns_ptr(void);

const Thing * returns_ptr_to_const(void);

void test_lvalue(void);

#line 638
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[96] = {
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
        {"name", .type = 34, .offset = offsetof(TypeFieldInfo, name)},
        {"type", .type = 8, .offset = offsetof(TypeFieldInfo, type)},
        {"offset", .type = 8, .offset = offsetof(TypeFieldInfo, offset)},}},
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
        {"kind", .type = 18, .offset = offsetof(TypeInfo, kind)},
        {"size", .type = 8, .offset = offsetof(TypeInfo, size)},
        {"align", .type = 8, .offset = offsetof(TypeInfo, align)},
        {"name", .type = 34, .offset = offsetof(TypeInfo, name)},
        {"count", .type = 8, .offset = offsetof(TypeInfo, count)},
        {"base", .type = 8, .offset = offsetof(TypeInfo, base)},
        {"fields", .type = 35, .offset = offsetof(TypeInfo, fields)},
        {"num_fields", .type = 8, .offset = offsetof(TypeInfo, num_fields)},}},
    [21] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = 16, .offset = offsetof(Any, ptr)},
        {"type", .type = 8, .offset = offsetof(Any, type)},}},
    [22] = NULL, // Incomplete: SomeIncompleteType
    [23] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S1), .align = alignof(S1), .name = "S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(S1, a)},
        {"b", .type = 50, .offset = offsetof(S1, b)},}},
    [24] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S2), .align = alignof(S2), .name = "S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = 23, .offset = offsetof(S2, s1)},}},
    [25] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(UartCtrl), .align = alignof(UartCtrl), .name = "UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = 2, .offset = offsetof(UartCtrl, tx_enable)},
        {"rx_enable", .type = 2, .offset = offsetof(UartCtrl, rx_enable)},}},
    [26] = &(TypeInfo){TYPE_UNION, .size = sizeof(IntOrPtr), .align = alignof(IntOrPtr), .name = "IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = 8, .offset = offsetof(IntOrPtr, i)},
        {"p", .type = 52, .offset = offsetof(IntOrPtr, p)},}},
    [27] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Vector), .align = alignof(Vector), .name = "Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 8, .offset = offsetof(Vector, x)},
        {"y", .type = 8, .offset = offsetof(Vector, y)},}},
    [28] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(T), .align = alignof(T), .name = "T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 68, .offset = offsetof(T, a)},}},
    [29] = NULL, // Enum
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 50, .offset = offsetof(ConstVector, x)},
        {"y", .type = 50, .offset = offsetof(ConstVector, y)},}},
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(BufHdr), .align = alignof(BufHdr), .name = "BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = 13, .offset = offsetof(BufHdr, cap)},
        {"len", .type = 13, .offset = offsetof(BufHdr, len)},
        {"buf", .type = 88, .offset = offsetof(BufHdr, buf)},}},
    [32] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Thing), .align = alignof(Thing), .name = "Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(Thing, a)},}},
    [33] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = 3},
    [34] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 33},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 19},
    [36] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeInfo), .align = alignof(const TypeInfo), .base = 20},
    [37] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 36},
    [38] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 37},
    [39] = NULL, // Func
    [40] = NULL, // Func
    [41] = NULL, // Func
    [42] = NULL, // Func
    [43] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = 1},
    [44] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 43},
    [45] = NULL, // Func
    [46] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 22},
    [47] = NULL, // Func
    [48] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [49] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
    [50] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = 8},
    [51] = NULL, // Func
    [52] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [53] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = 8, .count = 16},
    [54] = NULL, // Func
    [55] = NULL, // Incomplete array type
    [56] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [57] = NULL, // Func
    [58] = NULL, // Func
    [59] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [60] = NULL, // Func
    [61] = NULL, // Func
    [62] = NULL, // Func
    [63] = NULL, // Func
    [64] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 33, .count = 256},
    [65] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [66] = NULL, // Func
    [67] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 28},
    [68] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [5]), .align = alignof(int [5]), .base = 8, .count = 5},
    [69] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 34, .count = 4},
    [70] = NULL, // Func
    [71] = NULL, // Func
    [72] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 50},
    [73] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 27},
    [74] = NULL, // Func
    [75] = NULL, // Func
    [76] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = 8, .count = 4},
    [77] = NULL, // Func
    [78] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [79] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 78},
    [80] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = 18},
    [81] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = 34},
    [82] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = 35},
    [83] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [84] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 16},
    [85] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 72, .count = 42},
    [86] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [87] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = 21},
    [88] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = 3, .count = 1},
    [89] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 32},
    [90] = NULL, // Func
    [91] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Thing), .align = alignof(const Thing), .base = 32},
    [92] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 91},
    [93] = NULL, // Func
    [94] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 34},
    [95] = NULL, // Func
};
int num_typeinfos = 96;
const TypeInfo **typeinfos = typeinfo_table;

// Definitions

#line 51 "<builtin>"
const TypeInfo * get_typeinfo(typeid type) {
    #line 52
    if ((typeinfos) && ((type) < (num_typeinfos))) {
        #line 53
        return typeinfos[type];
    } else {
        #line 55
        return NULL;
    }
}

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

#line 61 "test1.ion"
void test_modify(void) {
    #line 62
    int i = 42;
    #line 63
    int (*p) = &(i);
    #line 64
    (p)--;
    #line 65
    int x = *((p)++);
    #line 66
    assert((x) == (*(--(p))));
    #line 67
    (*(p))++;
    #line 68
    (*(p))--;
    #line 69
    int (stk[16]);
    #line 70
    int(*sp) = stk;
    #line 71
    (*((sp)++)) = 1;
    #line 72
    (*((sp)++)) = 2;
    #line 73
    (x) = *(--(sp));
    #line 74
    assert((x) == (2));
    #line 75
    (x) = *(--(sp));
    #line 76
    assert((x) == (1));
    #line 77
    assert((sp) == (stk));
}

#line 80
void f10(int (a[3])) {
    #line 81
    (a[1]) = 42;
}

#line 84
void test_arrays(void) {
    #line 85
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 90
void test_loops(void) {
    #line 93
    switch (0) {default: {
            if (1) {
                #line 96
                break;
            }
            #line 98
            for (;;) {
                #line 99
                continue;
            }
            break;
        }
    }
    #line 104
    while (0) {
    }
    #line 106
    for (int i = 0; (i) < (10); (i)++) {
    }
    #line 108
    for (;;) {
        #line 109
        break;
    }
    #line 111
    for (int i = 0;;) {
        #line 112
        break;
    }
    #line 114
    for (; 0;) {
    }
    #line 116
    for (int i = 0;; (i)++) {
        #line 117
        break;
    }
    #line 119
    int i = 0;
    #line 120
    for (;; (i)++) {
        #line 121
        break;
    }
}

#line 124
void test_nonmodifiable(void) {
    #line 125
    S1 s1;
    #line 126
    (s1.a) = 0;
    #line 129
    S2 s2;
    #line 130
    (s2.s1.a) = 0;
}

#line 142
uint32 pack(UartCtrl ctrl) {
    #line 143
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 146
UartCtrl unpack(uint32 word) {
    #line 147
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 150
void test_uart(void) {
    #line 151
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 152
    (*(UART_CTRL_REG)) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 153
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 154
    (ctrl.rx_enable) = true;
    #line 155
    (*(UART_CTRL_REG)) = (pack)(ctrl);
}

#line 158
uchar h(void) {
    #line 159
    ((Vector){.x = 1, .y = 2}.x) = 42;
    #line 160
    Vector (*v) = &((Vector){1, 2});
    #line 161
    (v->x) = 42;
    #line 162
    int (*p) = &((int){0});
    #line 163
    ulong x = ((uint){1}) + ((long){2});
    #line 164
    int y = +(c);
    #line 165
    return (uchar)(x);
}

#line 168
int g(U u) {
    #line 169
    return u.i;
}

#line 172
void k(void(*vp), int(*ip)) {
    #line 173
    (vp) = ip;
    #line 174
    (ip) = vp;
}

#line 177
void f1(void) {
    #line 178
    int (*p) = &((int){0});
    #line 179
    (*(p)) = 42;
}

#line 182
void f3(int (a[])) {
}

#line 187
int example_test(void) {
    #line 188
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 209
int is_even(int digit) {
    #line 210
    int b = 0;
    #line 211
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 213
            (b) = 1;
            break;
        }
    }
    #line 215
    return b;
}

int i;

#line 231
void f2(Vector v) {
    #line 232
    (v) = (Vector){0};
}

#line 235
int fact_iter(int n) {
    #line 236
    int r = 1;
    #line 237
    for (int i = 0; (i) <= (n); (i)++) {
        #line 238
        (r) *= i;
    }
    #line 240
    return r;
}

#line 243
int fact_rec(int n) {
    #line 244
    if ((n) == (0)) {
        #line 245
        return 1;
    } else {
        #line 247
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 274
void test_enum(void) {
    #line 275
    Color a = COLOR_RED;
    #line 276
    Color b = COLOR_RED;
    #line 277
    int c = (a) + (b);
    #line 278
    int i = a;
    #line 279
    (a) = i;
    #line 280
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 283
void test_assign(void) {
    #line 284
    int i = 0;
    #line 285
    float f = 3.14f;
    #line 286
    int(*p) = &(i);
    #line 287
    (i)++;
    #line 288
    (i)--;
    #line 289
    (p)++;
    #line 290
    (p)--;
    #line 291
    (p) += 1;
    #line 292
    (i) /= 2;
    #line 293
    (i) *= 123;
    #line 294
    (i) %= 3;
    #line 295
    (i) <<= 1;
    #line 296
    (i) >>= 2;
    #line 297
    (i) &= 0xFF;
    #line 298
    (i) |= 0xFF00;
    #line 299
    (i) ^= 0xFF0;
}

#line 306
void benchmark(int n) {
    #line 307
    int r = 1;
    #line 308
    for (int i = 1; (i) <= (n); (i)++) {
        #line 309
        (r) *= i;
    }
}

#line 313
int va_test(int x, ...) {
    #line 314
    return 0;
}

#line 319
void test_lits(void) {
    #line 320
    float f = 3.14f;
    #line 321
    double d = 3.14;
    #line 322
    int i = 1;
    #line 323
    uint u = 0xFFFFFFFFu;
    #line 324
    long l = 1l;
    #line 325
    ulong ul = 1ul;
    #line 326
    llong ll = 0x100000000ll;
    #line 327
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 328
    uint x1 = 0xFFFFFFFF;
    #line 329
    llong x2 = 4294967295;
    #line 330
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 331
    int x4 = (0xAA) + (0x55);
}

#line 336
void test_ops(void) {
    #line 337
    float pi = 3.14f;
    #line 338
    float f = 0.0f;
    #line 339
    (f) = +(pi);
    #line 340
    (f) = -(pi);
    #line 341
    int n = -(1);
    #line 342
    (n) = ~(n);
    #line 343
    (f) = ((f) * (pi)) + (n);
    #line 344
    (f) = (pi) / (pi);
    #line 345
    (n) = (3) % (2);
    #line 346
    (n) = (n) + ((uchar)(1));
    #line 347
    int (*p) = &(n);
    #line 348
    (p) = (p) + (1);
    #line 349
    (n) = (int)(((p) + (1)) - (p));
    #line 350
    (n) = (n) << (1);
    #line 351
    (n) = (n) >> (1);
    #line 352
    int b = ((p) + (1)) > (p);
    #line 353
    (b) = ((p) + (1)) >= (p);
    #line 354
    (b) = ((p) + (1)) < (p);
    #line 355
    (b) = ((p) + (1)) <= (p);
    #line 356
    (b) = ((p) + (1)) == (p);
    #line 357
    (b) = (1) > (2);
    #line 358
    (b) = (1.23f) <= (pi);
    #line 359
    (n) = 0xFF;
    #line 360
    (b) = (n) & (~(1));
    #line 361
    (b) = (n) & (1);
    #line 362
    (b) = ((n) & (~(1))) ^ (1);
    #line 363
    (b) = (p) && (pi);
}

#line 368
void test_bool(void) {
    #line 369
    bool b = false;
    #line 370
    (b) = true;
    #line 371
    int i = 0;
    #line 372
    (i) = IS_DEBUG;
}

#line 375
int test_ctrl(void) {
    #line 376
    switch (1) {
        case 0: {
            #line 378
            return 0;
            break;
        }default: {
            #line 380
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 389
void f4(const char(*x)) {
}

#line 396
void f5(const int(*p)) {
}

#line 399
void test_convert(void) {
    #line 400
    const int(*a) = 0;
    #line 401
    int(*b) = 0;
    #line 402
    (a) = b;
    #line 403
    void(*p) = 0;
    #line 404
    (f5)(p);
}

#line 407
void test_const(void) {
    #line 408
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 411
    (i) = 1;
    #line 414
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 419
    const char (*p) = (const char *)(0);
    #line 420
    (p) = (escape_to_char) + (1);
    #line 421
    char (*q) = (char *)(escape_to_char);
    #line 422
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 427
    (i) = (int)((ullong)(p));
}

#line 430
void test_init(void) {
    #line 431
    int x = (const int)(0);
    #line 432
    int y;
    #line 433
    (y) = 0;
    #line 434
    int z = 42;
    #line 435
    int (a[3]) = {1, 2, 3};
    #line 438
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 439
        (printf)("%llu\n", i);
    }
    #line 441
    int (b[4]) = {1, 2, 3, 4};
    #line 442
    (b[0]) = a[2];
}

#line 445
void test_sizeof(void) {
    #line 446
    int i = 0;
    #line 447
    ullong n = sizeof(i);
    #line 448
    (n) = sizeof(int);
    #line 449
    (n) = sizeof(int);
    #line 450
    (n) = sizeof(int *);
}

#line 453
void test_cast(void) {
    #line 454
    int(*p) = 0;
    #line 455
    uint64 a = 0;
    (a) = (uint64)(p);
    (p) = (int *)(a);
}

#line 462
void print_any(Any any) {
    #line 463
    switch (any.type) {
        case 8: {
            #line 465
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 467
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 469
            (printf)("<unknown>");
            break;
        }
    }
    #line 471
    (printf)(": ");
    #line 472
    (print_type)(any.type);
}

#line 475
void println_any(Any any) {
    #line 476
    (print_any)(any);
    #line 477
    (printf)("\n");
}

#line 480
void print_typeid(typeid type) {
    #line 481
    (printf)("typeid(%d)", type);
}

#line 484
void print_type(typeid type) {
    #line 485
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 486
    if (!(typeinfo)) {
        #line 487
        (print_typeid)(type);
        #line 488
        return;
    }
    #line 490
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 492
            (print_type)(typeinfo->base);
            #line 493
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 495
            (print_type)(typeinfo->base);
            #line 496
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 498
            (print_type)(typeinfo->base);
            #line 499
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 501
            if (typeinfo->name) {
                #line 502
                (printf)("%s", typeinfo->name);
            } else {
                #line 504
                (print_typeid)(type);
            }
            break;
        }
    }
}

#line 509
void println_type(typeid type) {
    #line 510
    (print_type)(type);
    #line 511
    (printf)("\n");
}

#line 514
void print_typeinfo(typeid type) {
    #line 515
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 516
    if (!(typeinfo)) {
        #line 517
        (print_typeid)(type);
        #line 518
        return;
    }
    #line 520
    (printf)("<");
    #line 521
    (print_type)(type);
    #line 522
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 523
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 526
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 527
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 528
                TypeFieldInfo field = typeinfo->fields[i];
                #line 529
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 530
                (print_type)(field.type);
                #line 531
                (printf)("; ");
            }
            #line 533
            (printf)("}");
            break;
        }
    }
    #line 535
    (printf)(">");
}

#line 538
void println_typeinfo(typeid type) {
    #line 539
    (print_typeinfo)(type);
    #line 540
    (printf)("\n");
}

#line 543
void test_typeinfo(void) {
    #line 544
    int i = 42;
    #line 545
    float f = 3.14f;
    #line 546
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 549
    (println_any)((Any){&(f), 14});
    #line 550
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 553
    (println_type)(72);
    #line 554
    (println_type)(85);
    #line 555
    (println_type)(25);
    (println_typeinfo)(8);
    #line 558
    (println_typeinfo)(25);
    #line 559
    (println_typeinfo)(86);
    #line 560
    (println_typeinfo)(26);
}

#line 563
void test_compound_literals(void) {
    #line 564
    int i = 42;
    #line 565
    const Any (x) = {&(i), 8};
    #line 566
    Any y = {&(i), 8};
}

#line 569
void test_complete(void) {
    #line 570
    int x = 0;
    #line 573
    int y = 0;
    if ((x) == (0)) {
        #line 576
        (y) = 1;
    } else if ((x) == (1)) {
        #line 578
        (y) = 2;
    } else {
        #line 574
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 581
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 589
    switch (x) {
        case 0: {
            #line 591
            (y) = 3;
            break;
        }
        case 1: {
            #line 593
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 597
void test_alignof(void) {
    #line 598
    int i = 42;
    #line 599
    ullong n1 = alignof(int);
    #line 600
    ullong n2 = alignof(int);
    #line 601
    ullong n3 = alignof(ullong);
    #line 602
    ullong n4 = alignof(int *);
}

#line 612
void test_offsetof(void) {
    #line 613
    ullong n = offsetof(BufHdr, buf);
}

Thing thing;

Thing * returns_ptr(void) {
    #line 624
    return &(thing);
}

#line 627
const Thing * returns_ptr_to_const(void) {
    #line 628
    return &(thing);
}

#line 631
void test_lvalue(void) {
    #line 632
    ((returns_ptr)()->a) = 5;
    const Thing (*p) = (returns_ptr_to_const)();
}

#line 638
int main(int argc, const char *(*argv)) {
    #line 639
    if ((argv) == (0)) {
        #line 640
        (printf)("argv is null\n");
    }
    #line 642
    (test_modify)();
    #line 643
    (test_lvalue)();
    #line 644
    (test_alignof)();
    #line 645
    (test_offsetof)();
    #line 646
    (test_complete)();
    #line 647
    (test_compound_literals)();
    #line 648
    (test_loops)();
    #line 649
    (test_sizeof)();
    #line 650
    (test_assign)();
    #line 651
    (test_enum)();
    #line 652
    (test_arrays)();
    #line 653
    (test_cast)();
    #line 654
    (test_init)();
    #line 655
    (test_lits)();
    #line 656
    (test_const)();
    #line 657
    (test_bool)();
    #line 658
    (test_ops)();
    #line 659
    (test_typeinfo)();
    #line 660
    (getchar)();
    #line 661
    return 0;
}
