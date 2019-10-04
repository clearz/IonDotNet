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

#line 240 "test1.ion"
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

#line 139
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
void f10(int (a[3]));

void test_arrays(void);

#line 71
void test_loops(void);

#line 105
void test_nonmodifiable(void);

#line 117
struct UartCtrl {
    #line 118
    bool tx_enable;
    #line 118
    bool rx_enable;
};

#line 121
#define UART_CTRL_REG (uint *)(0x12345678)

#line 123
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 208
struct Vector {
    #line 209
    int x;
    #line 209
    int y;
};

#line 166
typedef IntOrPtr U;

#line 172
union IntOrPtr {
    #line 173
    int i;
    #line 174
    int(*p);
};

#line 149
int g(U u);

void k(void(*vp), int(*ip));

#line 158
void f1(void);

#line 163
void f3(int (a[]));

#line 168
int example_test(void);

#line 224
int fact_rec(int n);

#line 216
int fact_iter(int n);

#line 177
extern const char (escape_to_char[256]);

#line 187
extern int (a2[11]);

#line 190
int is_even(int digit);

#line 206
extern int i;

#line 212
void f2(Vector v);

#line 234
extern T(*p);

#line 232
#define M (1) + (sizeof(p))

struct T {
    #line 237
    int (a[M]);
};

#line 248
extern const char * (color_names[NUM_COLORS]);

#line 255
void test_enum(void);

#line 264
void test_assign(void);

#line 287
void benchmark(int n);

#line 294
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 300
void test_lits(void);

#line 317
void test_ops(void);

#line 353
#define IS_DEBUG true

#line 355
void test_bool(void);

#line 362
int test_ctrl(void);

#line 372
extern const int (j);

#line 373
extern const int(*q);

#line 374
extern const Vector (cv);

#line 376
void f4(const char(*x));

#line 379
struct ConstVector {
    #line 380
    const int (x);
    #line 380
    const int (y);
};

#line 383
void f5(const int(*p));

#line 386
void test_convert(void);

#line 394
void test_const(void);

#line 417
void test_init(void);

#line 430
void test_sizeof(void);

#line 438
void test_cast(void);

#line 447
void print_any(Any any);

#line 469
void print_type(typeid type);

#line 460
void println_any(Any any);

#line 465
void print_typeid(typeid type);

#line 494
void println_type(typeid type);

#line 499
void print_typeinfo(typeid type);

#line 523
void println_typeinfo(typeid type);

#line 528
void test_typeinfo(void);

#line 548
void test_compound_literals(void);

#line 554
void test_complete(void);

#line 582
void test_alignof(void);

#line 590
struct BufHdr {
    #line 591
    usize cap;
    #line 591
    usize len;
    #line 592
    char (buf[1]);
};

#line 597
void test_offsetof(void);

#line 602
struct Thing {
    #line 603
    int a;
};

#line 606
extern Thing thing;

#line 608
Thing * returns_ptr(void);

const Thing * returns_ptr_to_const(void);

void test_lvalue(void);

#line 624
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[94] = {
    [0] = NULL, // No associated type
    [1] = &(TypeInfo){TYPE_VOID, .name = "void", .size = 0, .align = 0},
    [2] = &(TypeInfo){ TYPE_BOOL, .size = sizeof(bool), .align = alignof(bool), .name = "bool"},
    [3] = &(TypeInfo){ TYPE_CHAR, .size = sizeof(char), .align = alignof(char), .name = "char"},
    [4] = &(TypeInfo){ TYPE_UCHAR, .size = sizeof(uchar), .align = alignof(uchar), .name = "uchar"},
    [5] = &(TypeInfo){ TYPE_SCHAR, .size = sizeof(schar), .align = alignof(schar), .name = "schar"},
    [6] = &(TypeInfo){ TYPE_SHORT, .size = sizeof(short), .align = alignof(short), .name = "short"},
    [7] = &(TypeInfo){ TYPE_USHORT, .size = sizeof(ushort), .align = alignof(ushort), .name = "ushort"},
    [8] = &(TypeInfo){ TYPE_INT, .size = sizeof(int), .align = alignof(int), .name = "int"},
    [9] = &(TypeInfo){ TYPE_UINT, .size = sizeof(uint), .align = alignof(uint), .name = "uint"},
    [10] = &(TypeInfo){ TYPE_LONG, .size = sizeof(size_t), .align = alignof(size_t), .name = "size_t"},
    [11] = &(TypeInfo){ TYPE_ULONG, .size = sizeof(ulong), .align = alignof(ulong), .name = "ulong"},
    [12] = &(TypeInfo){ TYPE_LLONG, .size = sizeof(llong), .align = alignof(llong), .name = "llong"},
    [13] = &(TypeInfo){ TYPE_ULLONG, .size = sizeof(ullong), .align = alignof(ullong), .name = "ullong"},
    [14] = &(TypeInfo){ TYPE_FLOAT, .size = sizeof(float), .align = alignof(float), .name = "float"},
    [15] = &(TypeInfo){ TYPE_DOUBLE, .size = sizeof(double), .align = alignof(double), .name = "double"},
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
        {"a", .type = 67, .offset = offsetof(T, a)},}},
    [29] = NULL, // Enum
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 50, .offset = offsetof(ConstVector, x)},
        {"y", .type = 50, .offset = offsetof(ConstVector, y)},}},
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(BufHdr), .align = alignof(BufHdr), .name = "BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = 13, .offset = offsetof(BufHdr, cap)},
        {"len", .type = 13, .offset = offsetof(BufHdr, len)},
        {"buf", .type = 86, .offset = offsetof(BufHdr, buf)},}},
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
    [53] = NULL, // Func
    [54] = NULL, // Incomplete array type
    [55] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [56] = NULL, // Func
    [57] = NULL, // Func
    [58] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [59] = NULL, // Func
    [60] = NULL, // Func
    [61] = NULL, // Func
    [62] = NULL, // Func
    [63] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 33, .count = 256},
    [64] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [65] = NULL, // Func
    [66] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 28},
    [67] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [5]), .align = alignof(int [5]), .base = 8, .count = 5},
    [68] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 34, .count = 4},
    [69] = NULL, // Func
    [70] = NULL, // Func
    [71] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 50},
    [72] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 27},
    [73] = NULL, // Func
    [74] = NULL, // Func
    [75] = NULL, // Func
    [76] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [77] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 76},
    [78] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = 18},
    [79] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = 34},
    [80] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = 35},
    [81] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [82] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 16},
    [83] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 71, .count = 42},
    [84] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [85] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = 21},
    [86] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = 3, .count = 1},
    [87] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 32},
    [88] = NULL, // Func
    [89] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Thing), .align = alignof(const Thing), .base = 32},
    [90] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 89},
    [91] = NULL, // Func
    [92] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 34},
    [93] = NULL, // Func
};
int num_typeinfos = 94;
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
void f10(int (a[3])) {
    #line 62
    (a[1]) = 42;
}

#line 65
void test_arrays(void) {
    #line 66
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 71
void test_loops(void) {
    #line 74
    switch (0) {default: {
            if (1) {
                #line 77
                break;
            }
            #line 79
            for (;;) {
                #line 80
                continue;
            }
            break;
        }
    }
    #line 85
    while (0) {
    }
    #line 87
    for (int i = 0; (i) < (10); (i)++) {
    }
    #line 89
    for (;;) {
        #line 90
        break;
    }
    #line 92
    for (int i = 0;;) {
        #line 93
        break;
    }
    #line 95
    for (; 0;) {
    }
    #line 97
    for (int i = 0;; (i)++) {
        #line 98
        break;
    }
    #line 100
    int i = 0;
    #line 101
    for (;; (i)++) {
        #line 102
        break;
    }
}

#line 105
void test_nonmodifiable(void) {
    #line 106
    S1 s1;
    #line 107
    (s1.a) = 0;
    #line 110
    S2 s2;
    #line 111
    (s2.s1.a) = 0;
}

#line 123
uint32 pack(UartCtrl ctrl) {
    #line 124
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 127
UartCtrl unpack(uint32 word) {
    #line 128
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 131
void test_uart(void) {
    #line 132
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 133
    (*(UART_CTRL_REG)) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 134
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 135
    (ctrl.rx_enable) = true;
    #line 136
    (*(UART_CTRL_REG)) = (pack)(ctrl);
}

#line 139
uchar h(void) {
    #line 140
    ((Vector){.x = 1, .y = 2}.x) = 42;
    #line 141
    Vector (*v) = &((Vector){1, 2});
    #line 142
    (v->x) = 42;
    #line 143
    int (*p) = &((int){0});
    #line 144
    ulong x = ((uint){1}) + ((size_t){2});
    #line 145
    int y = +(c);
    #line 146
    return (uchar)(x);
}

#line 149
int g(U u) {
    #line 150
    return u.i;
}

#line 153
void k(void(*vp), int(*ip)) {
    #line 154
    (vp) = ip;
    #line 155
    (ip) = vp;
}

#line 158
void f1(void) {
    #line 159
    int (*p) = &((int){0});
    #line 160
    (*(p)) = 42;
}

#line 163
void f3(int (a[])) {
}

#line 168
int example_test(void) {
    #line 169
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 190
int is_even(int digit) {
    #line 191
    int b = 0;
    #line 192
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 194
            (b) = 1;
            break;
        }
    }
    #line 196
    return b;
}

int i;

#line 212
void f2(Vector v) {
    #line 213
    (v) = (Vector){0};
}

#line 216
int fact_iter(int n) {
    #line 217
    int r = 1;
    #line 218
    for (int i = 0; (i) <= (n); (i)++) {
        #line 219
        (r) *= i;
    }
    #line 221
    return r;
}

#line 224
int fact_rec(int n) {
    #line 225
    if ((n) == (0)) {
        #line 226
        return 1;
    } else {
        #line 228
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 255
void test_enum(void) {
    #line 256
    Color a = COLOR_RED;
    #line 257
    Color b = COLOR_RED;
    #line 258
    int c = (a) + (b);
    #line 259
    int i = a;
    #line 260
    (a) = i;
    #line 261
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 264
void test_assign(void) {
    #line 265
    int i = 0;
    #line 266
    float f = 3.14f;
    #line 267
    int(*p) = &(i);
    #line 268
    (i)++;
    #line 269
    (i)--;
    #line 270
    (p)++;
    #line 271
    (p)--;
    #line 272
    (p) += 1;
    #line 273
    (i) /= 2;
    #line 274
    (i) *= 123;
    #line 275
    (i) %= 3;
    #line 276
    (i) <<= 1;
    #line 277
    (i) >>= 2;
    #line 278
    (i) &= 0xFF;
    #line 279
    (i) |= 0xFF00;
    #line 280
    (i) ^= 0xFF0;
}

#line 287
void benchmark(int n) {
    #line 288
    int r = 1;
    #line 289
    for (int i = 1; (i) <= (n); (i)++) {
        #line 290
        (r) *= i;
    }
}

#line 294
int va_test(int x, ...) {
    #line 295
    return 0;
}

#line 300
void test_lits(void) {
    #line 301
    float f = 3.14f;
    #line 302
    double d = 3.14;
    #line 303
    int i = 1;
    #line 304
    uint u = 0xFFFFFFFFu;
    #line 305
    size_t l = 1l;
    #line 306
    ulong ul = 1ul;
    #line 307
    llong ll = 0x100000000ll;
    #line 308
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 309
    uint x1 = 0xFFFFFFFF;
    #line 310
    llong x2 = 4294967295;
    #line 311
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 312
    int x4 = (0xAA) + (0x55);
}

#line 317
void test_ops(void) {
    #line 318
    float pi = 3.14f;
    #line 319
    float f = 0.0f;
    #line 320
    (f) = +(pi);
    #line 321
    (f) = -(pi);
    #line 322
    int n = -(1);
    #line 323
    (n) = ~(n);
    #line 324
    (f) = ((f) * (pi)) + (n);
    #line 325
    (f) = (pi) / (pi);
    #line 326
    (n) = (3) % (2);
    #line 327
    (n) = (n) + ((uchar)(1));
    #line 328
    int (*p) = &(n);
    #line 329
    (p) = (p) + (1);
    #line 330
    (n) = (int)(((p) + (1)) - (p));
    #line 331
    (n) = (n) << (1);
    #line 332
    (n) = (n) >> (1);
    #line 333
    int b = ((p) + (1)) > (p);
    #line 334
    (b) = ((p) + (1)) >= (p);
    #line 335
    (b) = ((p) + (1)) < (p);
    #line 336
    (b) = ((p) + (1)) <= (p);
    #line 337
    (b) = ((p) + (1)) == (p);
    #line 338
    (b) = (1) > (2);
    #line 339
    (b) = (1.23f) <= (pi);
    #line 340
    (n) = 0xFF;
    #line 341
    (b) = (n) & (~(1));
    #line 342
    (b) = (n) & (1);
    #line 343
    (b) = ((n) & (~(1))) ^ (1);
    #line 344
    (b) = (p) && (pi);
    #line 345
    (p)--;
    #line 346
    int x = *((p)++);
    #line 347
    assert((x) == (*(--(p))));
    #line 348
    (*(p))++;
    #line 349
    (*(p))++;
    #line 350
    (*(p))--;
}

#line 355
void test_bool(void) {
    #line 356
    bool b = false;
    #line 357
    (b) = true;
    #line 358
    int i = 0;
    #line 359
    (i) = IS_DEBUG;
}

#line 362
int test_ctrl(void) {
    #line 363
    switch (1) {
        case 0: {
            #line 365
            return 0;
            break;
        }default: {
            #line 367
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 376
void f4(const char(*x)) {
}

#line 383
void f5(const int(*p)) {
}

#line 386
void test_convert(void) {
    #line 387
    const int(*a) = 0;
    #line 388
    int(*b) = 0;
    #line 389
    (a) = b;
    #line 390
    void(*p) = 0;
    #line 391
    (f5)(p);
}

#line 394
void test_const(void) {
    #line 395
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 398
    (i) = 1;
    #line 401
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 406
    const char (*p) = (const char *)(0);
    #line 407
    (p) = (escape_to_char) + (1);
    #line 408
    char (*q) = (char *)(escape_to_char);
    #line 409
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 414
    (i) = (int)((ullong)(p));
}

#line 417
void test_init(void) {
    #line 418
    int x = (const int)(0);
    #line 419
    int y;
    #line 420
    (y) = 0;
    #line 421
    int z = 42;
    #line 422
    int (a[3]) = {1, 2, 3};
    #line 425
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 426
        (printf)("%llu\n", i);
    }
}

#line 430
void test_sizeof(void) {
    #line 431
    int i = 0;
    #line 432
    ullong n = sizeof(i);
    #line 433
    (n) = sizeof(int);
    #line 434
    (n) = sizeof(int);
    #line 435
    (n) = sizeof(int *);
}

#line 438
void test_cast(void) {
    #line 439
    int(*p) = 0;
    #line 440
    uint64 a = 0;
    (a) = (uint64)(p);
    (p) = (int *)(a);
}

#line 447
void print_any(Any any) {
    #line 448
    switch (any.type) {
        case 8: {
            #line 450
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 452
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 454
            (printf)("<unknown>");
            break;
        }
    }
    #line 456
    (printf)(": ");
    #line 457
    (print_type)(any.type);
}

#line 460
void println_any(Any any) {
    #line 461
    (print_any)(any);
    #line 462
    (printf)("\n");
}

#line 465
void print_typeid(typeid type) {
    #line 466
    (printf)("typeid(%d)", type);
}

#line 469
void print_type(typeid type) {
    #line 470
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 471
    if (!(typeinfo)) {
        #line 472
        (print_typeid)(type);
        #line 473
        return;
    }
    #line 475
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 477
            (print_type)(typeinfo->base);
            #line 478
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 480
            (print_type)(typeinfo->base);
            #line 481
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 483
            (print_type)(typeinfo->base);
            #line 484
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 486
            if (typeinfo->name) {
                #line 487
                (printf)("%s", typeinfo->name);
            } else {
                #line 489
                (print_typeid)(type);
            }
            break;
        }
    }
}

#line 494
void println_type(typeid type) {
    #line 495
    (print_type)(type);
    #line 496
    (printf)("\n");
}

#line 499
void print_typeinfo(typeid type) {
    #line 500
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 501
    if (!(typeinfo)) {
        #line 502
        (print_typeid)(type);
        #line 503
        return;
    }
    #line 505
    (printf)("<");
    #line 506
    (print_type)(type);
    #line 507
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 508
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 511
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 512
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 513
                TypeFieldInfo field = typeinfo->fields[i];
                #line 514
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 515
                (print_type)(field.type);
                #line 516
                (printf)("; ");
            }
            #line 518
            (printf)("}");
            break;
        }
    }
    #line 520
    (printf)(">");
}

#line 523
void println_typeinfo(typeid type) {
    #line 524
    (print_typeinfo)(type);
    #line 525
    (printf)("\n");
}

#line 528
void test_typeinfo(void) {
    #line 529
    int i = 42;
    #line 530
    float f = 3.14f;
    #line 531
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 534
    (println_any)((Any){&(f), 14});
    #line 535
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 538
    (println_type)(71);
    #line 539
    (println_type)(83);
    #line 540
    (println_type)(25);
    (println_typeinfo)(8);
    #line 543
    (println_typeinfo)(25);
    #line 544
    (println_typeinfo)(84);
    #line 545
    (println_typeinfo)(26);
}

#line 548
void test_compound_literals(void) {
    #line 549
    int i = 42;
    #line 550
    const Any (x) = {&(i), 8};
    #line 551
    Any y = {&(i), 8};
}

#line 554
void test_complete(void) {
    #line 555
    int x = 0;
    #line 558
    int y = 0;
    if ((x) == (0)) {
        #line 561
        (y) = 1;
    } else if ((x) == (1)) {
        #line 563
        (y) = 2;
    } else {
        #line 559
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 566
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 574
    switch (x) {
        case 0: {
            #line 576
            (y) = 3;
            break;
        }
        case 1: {
            #line 578
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 582
void test_alignof(void) {
    #line 583
    int i = 42;
    #line 584
    ullong n1 = alignof(int);
    #line 585
    ullong n2 = alignof(int);
    #line 586
    ullong n3 = alignof(ullong);
    #line 587
    ullong n4 = alignof(int *);
}

#line 597
void test_offsetof(void) {
    #line 598
    ullong n = offsetof(BufHdr, buf);
}

Thing thing;

Thing * returns_ptr(void) {
    #line 609
    return &(thing);
}

#line 612
const Thing * returns_ptr_to_const(void) {
    #line 613
    return &(thing);
}

#line 616
void test_lvalue(void) {
    ((returns_ptr)()->a) = 5;
    const Thing (*p) = (returns_ptr_to_const)();
}

#line 624
int main(int argc, const char *(*argv)) {
    #line 625
    if ((argv) == (0)) {
        #line 626
        (printf)("argv is null\n");
    }
    #line 628
    (test_lvalue)();
    #line 629
    (test_alignof)();
    #line 630
    (test_offsetof)();
    #line 631
    (test_complete)();
    #line 632
    (test_compound_literals)();
    #line 633
    (test_loops)();
    #line 634
    (test_sizeof)();
    #line 635
    (test_assign)();
    #line 636
    (test_enum)();
    #line 637
    (test_arrays)();
    #line 638
    (test_cast)();
    #line 639
    (test_init)();
    #line 640
    (test_lits)();
    #line 641
    (test_const)();
    #line 642
    (test_bool)();
    #line 643
    (test_ops)();
    #line 644
    (test_typeinfo)();
    #line 645
    (getchar)();
    #line 646
    return 0;
}
