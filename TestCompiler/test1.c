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
typedef struct SomeIncompleteType SomeIncompleteType;
typedef struct S1 S1;
typedef struct S2 S2;
typedef struct UartCtrl UartCtrl;
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;
typedef struct ConstVector ConstVector;
typedef struct Any Any;
typedef struct BufHdr BufHdr;

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

#line 238 "test1.ion"
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
TypeInfo * get_typeinfo(typeid type);

#line 14 "test1.ion"
extern SomeIncompleteType(*incomplete_ptr);

#line 27
#define PI 3.14f

#line 28
#define PI2 (PI) + (PI)

#line 30
#define U8 (uint8)(42)

#line 32
extern char c;

#line 33
extern uchar uc;

#line 34
extern schar sc;

#line 36
#define N (((char)(42)) + (8)) != (0)

#line 137
uchar h(void);

#line 38
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 40
extern char (*code);

#line 49
struct S1 {
    #line 50
    int a;
    #line 51
    const int (b);
};

#line 54
struct S2 {
    #line 55
    S1 s1;
};

#line 58
void f10(int (a[3]));

void test_arrays(void);

#line 69
void test_loops(void);

#line 103
void test_nonmodifiable(void);

#line 115
struct UartCtrl {
    #line 116
    bool tx_enable;
    #line 116
    bool rx_enable;
};

#line 119
#define UART_CTRL_REG (uint *)(0x12345678)

#line 121
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 206
struct Vector {
    #line 207
    int x;
    #line 207
    int y;
};

#line 164
typedef IntOrPtr U;

#line 170
union IntOrPtr {
    #line 171
    int i;
    #line 172
    int(*p);
};

#line 147
int g(U u);

void k(void(*vp), int(*ip));

#line 156
void f1(void);

#line 161
void f3(int (a[]));

#line 166
int example_test(void);

#line 222
int fact_rec(int n);

#line 214
int fact_iter(int n);

#line 175
extern const char (escape_to_char[256]);

#line 185
extern int (a2[11]);

#line 188
int is_even(int digit);

#line 204
extern int i;

#line 210
void f2(Vector v);

#line 232
extern T(*p);

#line 230
#define M (1) + (sizeof(p))

struct T {
    #line 235
    int (a[M]);
};

#line 246
extern const char * (color_names[NUM_COLORS]);

#line 253
void test_enum(void);

#line 262
void test_assign(void);

#line 285
void benchmark(int n);

#line 292
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 298
void test_lits(void);

#line 315
void test_ops(void);

#line 345
#define IS_DEBUG true

#line 347
void test_bool(void);

#line 354
int test_ctrl(void);

#line 364
extern const int (j);

#line 365
extern const int(*q);

#line 366
extern const Vector (cv);

#line 368
void f4(const char(*x));

#line 371
struct ConstVector {
    #line 372
    const int (x);
    #line 372
    const int (y);
};

#line 375
void f5(const int(*p));

#line 378
void test_convert(void);

#line 386
void test_const(void);

#line 409
void test_init(void);

#line 422
void test_sizeof(void);

#line 430
void test_cast(void);

#line 439
struct Any {
    #line 440
    void(*ptr);
    #line 441
    typeid type;
};

#line 444
void print_any(Any any);

#line 462
void print_type(typeid type);

#line 457
void println_any(Any any);

#line 486
void println_type(typeid type);

#line 491
void print_typeinfo(typeid type);

#line 515
void println_typeinfo(typeid type);

#line 520
void test_typeinfo(void);

#line 538
void test_compound_literals(void);

#line 544
struct BufHdr {
    #line 545
    usize cap;
    #line 545
    usize len;
    #line 546
    char (buf[1]);
};

#line 551
void test_complete(void);

#line 580
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[80] = {
    [0] = NULL, // No associated type
    [1] = &(TypeInfo){TYPE_VOID, .name = "void"},
    [2] = &(TypeInfo){ TYPE_BOOL, .size = sizeof(bool), .align = alignof(bool), .name = "bool"},
    [3] = &(TypeInfo){ TYPE_CHAR, .size = sizeof(char), .align = alignof(char), .name = "char"},
    [4] = &(TypeInfo){ TYPE_UCHAR, .size = sizeof(uchar), .align = alignof(uchar), .name = "uchar"},
    [5] = &(TypeInfo){ TYPE_SCHAR, .size = sizeof(schar), .align = alignof(schar), .name = "schar"},
    [6] = &(TypeInfo){ TYPE_SHORT, .size = sizeof(short), .align = alignof(short), .name = "short"},
    [7] = &(TypeInfo){ TYPE_USHORT, .size = sizeof(ushort), .align = alignof(ushort), .name = "ushort"},
    [8] = &(TypeInfo){ TYPE_INT, .size = sizeof(int), .align = alignof(int), .name = "int"},
    [9] = &(TypeInfo){ TYPE_UINT, .size = sizeof(uint), .align = alignof(uint), .name = "uint"},
    [10] = &(TypeInfo){ TYPE_LONG, .size = sizeof(long), .align = alignof(long), .name = "long"},
    [11] = &(TypeInfo){ TYPE_ULONG, .size = sizeof(ulong), .align = alignof(ulong), .name = "ulong"},
    [12] = &(TypeInfo){ TYPE_LLONG, .size = sizeof(llong), .align = alignof(llong), .name = "llong"},
    [13] = &(TypeInfo){ TYPE_ULLONG, .size = sizeof(ullong), .align = alignof(ullong), .name = "ullong"},
    [14] = &(TypeInfo){ TYPE_FLOAT, .size = sizeof(float), .align = alignof(float), .name = "float"},
    [15] = &(TypeInfo){ TYPE_DOUBLE, .size = sizeof(double), .align = alignof(double), .name = "double"},
    [16] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 1},
    [17] = &(TypeInfo){TYPE_CONST, .size = sizeof(const void *), .align = alignof(const void *), .base = 16},
    [18] = NULL, // Enum
    [19] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"name", .type = 33, .offset = offsetof(TypeFieldInfo, name)},
        {"type", .type = 8, .offset = offsetof(TypeFieldInfo, type)},
        {"offset", .type = 8, .offset = offsetof(TypeFieldInfo, offset)},}},
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
        {"kind", .type = 18, .offset = offsetof(TypeInfo, kind)},
        {"size", .type = 8, .offset = offsetof(TypeInfo, size)},
        {"align", .type = 8, .offset = offsetof(TypeInfo, align)},
        {"name", .type = 33, .offset = offsetof(TypeInfo, name)},
        {"count", .type = 8, .offset = offsetof(TypeInfo, count)},
        {"base", .type = 8, .offset = offsetof(TypeInfo, base)},
        {"fields", .type = 34, .offset = offsetof(TypeInfo, fields)},
        {"num_fields", .type = 8, .offset = offsetof(TypeInfo, num_fields)},}},
    [21] = NULL, // Incomplete: SomeIncompleteType
    [22] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S1), .align = alignof(S1), .name = "S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(S1, a)},
        {"b", .type = 45, .offset = offsetof(S1, b)},}},
    [23] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S2), .align = alignof(S2), .name = "S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = 22, .offset = offsetof(S2, s1)},}},
    [24] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(UartCtrl), .align = alignof(UartCtrl), .name = "UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = 2, .offset = offsetof(UartCtrl, tx_enable)},
        {"rx_enable", .type = 2, .offset = offsetof(UartCtrl, rx_enable)},}},
    [25] = &(TypeInfo){TYPE_UNION, .size = sizeof(IntOrPtr), .align = alignof(IntOrPtr), .name = "IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = 8, .offset = offsetof(IntOrPtr, i)},
        {"p", .type = 47, .offset = offsetof(IntOrPtr, p)},}},
    [26] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Vector), .align = alignof(Vector), .name = "Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 8, .offset = offsetof(Vector, x)},
        {"y", .type = 8, .offset = offsetof(Vector, y)},}},
    [27] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(T), .align = alignof(T), .name = "T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 62, .offset = offsetof(T, a)},}},
    [28] = NULL, // Enum
    [29] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 45, .offset = offsetof(ConstVector, x)},
        {"y", .type = 45, .offset = offsetof(ConstVector, y)},}},
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = 16, .offset = offsetof(Any, ptr)},
        {"type", .type = 8, .offset = offsetof(Any, type)},}},
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(BufHdr), .align = alignof(BufHdr), .name = "BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = 13, .offset = offsetof(BufHdr, cap)},
        {"len", .type = 13, .offset = offsetof(BufHdr, len)},
        {"buf", .type = 77, .offset = offsetof(BufHdr, buf)},}},
    [32] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = 3},
    [33] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 32},
    [34] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 19},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 20},
    [36] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 35},
    [37] = NULL, // Func
    [38] = NULL, // Func
    [39] = NULL, // Func
    [40] = NULL, // Func
    [41] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 21},
    [42] = NULL, // Func
    [43] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [44] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
    [45] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = 8},
    [46] = NULL, // Func
    [47] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [48] = NULL, // Func
    [49] = NULL, // Incomplete array type
    [50] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [51] = NULL, // Func
    [52] = NULL, // Func
    [53] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [54] = NULL, // Func
    [55] = NULL, // Func
    [56] = NULL, // Func
    [57] = NULL, // Func
    [58] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 32, .count = 256},
    [59] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [60] = NULL, // Func
    [61] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [62] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = 8, .count = 9},
    [63] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 33, .count = 4},
    [64] = NULL, // Func
    [65] = NULL, // Func
    [66] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 45},
    [67] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 26},
    [68] = NULL, // Func
    [69] = NULL, // Func
    [70] = NULL, // Func
    [71] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [72] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 71},
    [73] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [74] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 16},
    [75] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 66, .count = 42},
    [76] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = 30},
    [77] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = 3, .count = 1},
    [78] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 33},
    [79] = NULL, // Func
};
int num_typeinfos = 80;
TypeInfo **typeinfos = typeinfo_table;

// Definitions

#line 51 "<builtin>"
TypeInfo * get_typeinfo(typeid type) {
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

#line 58 "test1.ion"
void f10(int (a[3])) {
    #line 59
    a[1] = 42;
}

#line 62
void test_arrays(void) {
    #line 63
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 69
void test_loops(void) {
    #line 72
    switch (0) {default: {
            if (1) {
                #line 75
                break;
            }
            #line 77
            for (;;) {
                #line 78
                continue;
            }
            break;
        }
    }
    #line 83
    while (0) {
    }
    #line 85
    for (int i = 0; (i) < (10); i++) {
    }
    #line 87
    for (;;) {
        #line 88
        break;
    }
    #line 90
    for (int i = 0;;) {
        #line 91
        break;
    }
    #line 93
    for (; 0;) {
    }
    #line 95
    for (int i = 0;; i++) {
        #line 96
        break;
    }
    #line 98
    int i = 0;
    #line 99
    for (;; i++) {
        #line 100
        break;
    }
}

#line 103
void test_nonmodifiable(void) {
    #line 104
    S1 s1;
    #line 105
    s1.a = 0;
    #line 108
    S2 s2;
    #line 109
    s2.s1.a = 0;
}

#line 121
uint32 pack(UartCtrl ctrl) {
    #line 122
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 125
UartCtrl unpack(uint32 word) {
    #line 126
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 129
void test_uart(void) {
    #line 130
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 131
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 132
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 133
    ctrl.rx_enable = true;
    #line 134
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 137
uchar h(void) {
    #line 138
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 139
    Vector (*v) = &((Vector){1, 2});
    #line 140
    v->x = 42;
    #line 141
    int (*p) = &((int){0});
    #line 142
    ulong x = ((uint){1}) + ((long){2});
    #line 143
    int y = +(c);
    #line 144
    return (uchar)(x);
}

#line 147
int g(U u) {
    #line 148
    return u.i;
}

#line 151
void k(void(*vp), int(*ip)) {
    #line 152
    vp = ip;
    #line 153
    ip = vp;
}

#line 156
void f1(void) {
    #line 157
    int (*p) = &((int){0});
    #line 158
    *(p) = 42;
}

#line 161
void f3(int (a[])) {
}

#line 166
int example_test(void) {
    #line 167
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 188
int is_even(int digit) {
    #line 189
    int b = 0;
    #line 190
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 192
            b = 1;
            break;
        }
    }
    #line 194
    return b;
}

int i;

#line 210
void f2(Vector v) {
    #line 211
    v = (Vector){0};
}

#line 214
int fact_iter(int n) {
    #line 215
    int r = 1;
    #line 216
    for (int i = 0; (i) <= (n); i++) {
        #line 217
        r *= i;
    }
    #line 219
    return r;
}

#line 222
int fact_rec(int n) {
    #line 223
    if ((n) == (0)) {
        #line 224
        return 1;
    } else {
        #line 226
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 253
void test_enum(void) {
    #line 254
    Color a = COLOR_RED;
    #line 255
    Color b = COLOR_RED;
    #line 256
    int c = (a) + (b);
    #line 257
    int i = a;
    #line 258
    a = i;
    #line 259
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 262
void test_assign(void) {
    #line 263
    int i = 0;
    #line 264
    float f = 3.14f;
    #line 265
    int(*p) = &(i);
    #line 266
    i++;
    #line 267
    i--;
    #line 268
    p++;
    #line 269
    p--;
    #line 270
    p += 1;
    #line 271
    i /= 2;
    #line 272
    i *= 123;
    #line 273
    i %= 3;
    #line 274
    i <<= 1;
    #line 275
    i >>= 2;
    #line 276
    i &= 0xFF;
    #line 277
    i |= 0xFF00;
    #line 278
    i ^= 0xFF0;
}

#line 285
void benchmark(int n) {
    #line 286
    int r = 1;
    #line 287
    for (int i = 1; (i) <= (n); i++) {
        #line 288
        r *= i;
    }
}

#line 292
int va_test(int x, ...) {
    #line 293
    return 0;
}

#line 298
void test_lits(void) {
    #line 299
    float f = 3.14f;
    #line 300
    double d = 3.14;
    #line 301
    int i = 1;
    #line 302
    uint u = 0xFFFFFFFFu;
    #line 303
    long l = 1l;
    #line 304
    ulong ul = 1ul;
    #line 305
    llong ll = 0x100000000ll;
    #line 306
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 307
    uint x1 = 0xFFFFFFFF;
    #line 308
    llong x2 = 4294967295;
    #line 309
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 310
    int x4 = (0xAA) + (0x55);
}

#line 315
void test_ops(void) {
    #line 316
    float pi = 3.14f;
    #line 317
    float f = 0.0f;
    #line 318
    f = +(pi);
    #line 319
    f = -(pi);
    #line 320
    int n = -(1);
    #line 321
    n = ~(n);
    #line 322
    f = ((f) * (pi)) + (n);
    #line 323
    f = (pi) / (pi);
    #line 324
    n = (3) % (2);
    #line 325
    n = (n) + ((uchar)(1));
    #line 326
    int (*p) = &(n);
    #line 327
    p = (p) + (1);
    #line 328
    n = (int)(((p) + (1)) - (p));
    #line 329
    n = (n) << (1);
    #line 330
    n = (n) >> (1);
    #line 331
    int b = ((p) + (1)) > (p);
    #line 332
    b = ((p) + (1)) >= (p);
    #line 333
    b = ((p) + (1)) < (p);
    #line 334
    b = ((p) + (1)) <= (p);
    #line 335
    b = ((p) + (1)) == (p);
    #line 336
    b = (1) > (2);
    #line 337
    b = (1.23f) <= (pi);
    #line 338
    n = 0xFF;
    #line 339
    b = (n) & (~(1));
    #line 340
    b = (n) & (1);
    #line 341
    b = ((n) & (~(1))) ^ (1);
    #line 342
    b = (p) && (pi);
}

#line 347
void test_bool(void) {
    #line 348
    bool b = false;
    #line 349
    b = true;
    #line 350
    int i = 0;
    #line 351
    i = IS_DEBUG;
}

#line 354
int test_ctrl(void) {
    #line 355
    switch (1) {
        case 0: {
            #line 357
            return 0;
            break;
        }default: {
            #line 359
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 368
void f4(const char(*x)) {
}

#line 375
void f5(const int(*p)) {
}

#line 378
void test_convert(void) {
    #line 379
    const int(*a) = 0;
    #line 380
    int(*b) = 0;
    #line 381
    a = b;
    #line 382
    void(*p) = 0;
    #line 383
    (f5)(p);
}

#line 386
void test_const(void) {
    #line 387
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 390
    i = 1;
    #line 393
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 398
    const char (*p) = (const char *)(0);
    #line 399
    p = (escape_to_char) + (1);
    #line 400
    char (*q) = (char *)(escape_to_char);
    #line 401
    c = q['n'];
    p = (const char *)(1);
    #line 406
    i = (int)((ullong)(p));
}

#line 409
void test_init(void) {
    #line 410
    int x = (const int)(0);
    #line 411
    int y;
    #line 412
    y = 0;
    #line 413
    int z = 42;
    #line 414
    int (a[3]) = {1, 2, 3};
    #line 417
    for (ullong i = 0; (i) < (10); i++) {
        #line 418
        (printf)("%llu\n", i);
    }
}

#line 422
void test_sizeof(void) {
    #line 423
    int i = 0;
    #line 424
    ullong n = sizeof(i);
    #line 425
    n = sizeof(int);
    #line 426
    n = sizeof(int);
    #line 427
    n = sizeof(int *);
}

#line 430
void test_cast(void) {
    #line 431
    int(*p) = 0;
    #line 432
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 444
void print_any(Any any) {
    #line 445
    switch (any.type) {
        case 8: {
            #line 447
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 449
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 451
            (printf)("<unknown>");
            break;
        }
    }
    #line 453
    (printf)(": ");
    #line 454
    (print_type)(any.type);
}

#line 457
void println_any(Any any) {
    #line 458
    (print_any)(any);
    #line 459
    (printf)("\n");
}

#line 462
void print_type(typeid type) {
    #line 463
    TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 464
    if (!(typeinfo)) {
        #line 465
        (printf)("<typeid %d>", type);
    }
    #line 467
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 469
            (print_type)(typeinfo->base);
            #line 470
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 472
            (print_type)(typeinfo->base);
            #line 473
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 475
            (print_type)(typeinfo->base);
            #line 476
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 478
            if (typeinfo->name) {
                #line 479
                (printf)("%s", typeinfo->name);
            } else {
                #line 481
                (printf)("typeid %d>", type);
            }
            break;
        }
    }
}

#line 486
void println_type(typeid type) {
    #line 487
    (print_type)(type);
    #line 488
    (printf)("\n");
}

#line 491
void print_typeinfo(typeid type) {
    #line 492
    TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 493
    if (!(typeinfo)) {
        #line 494
        (printf)("<typeid %d>", type);
        #line 495
        return;
    }
    #line 497
    (printf)("<");
    #line 498
    (print_type)(type);
    #line 499
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 500
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 503
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 504
            for (int i = 0; (i) < (typeinfo->num_fields); i++) {
                #line 505
                TypeFieldInfo field = typeinfo->fields[i];
                #line 506
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 507
                (print_type)(field.type);
                #line 508
                (printf)("; ");
            }
            #line 510
            (printf)("}");
            break;
        }
    }
    #line 512
    (printf)(">");
}

#line 515
void println_typeinfo(typeid type) {
    #line 516
    (print_typeinfo)(type);
    #line 517
    (printf)("\n");
}

#line 520
void test_typeinfo(void) {
    #line 521
    int i = 42;
    #line 522
    float f = 3.14f;
    #line 523
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 526
    (println_any)((Any){&(f), 14});
    #line 527
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 530
    (println_type)(66);
    #line 531
    (println_type)(75);
    #line 532
    (println_type)(24);
    (println_typeinfo)(8);
    #line 535
    (println_typeinfo)(24);
}

#line 538
void test_compound_literals(void) {
    #line 539
    int i = 42;
    #line 540
    const Any (x) = {&(i), 8};
    #line 541
    Any y = {&(i), 8};
}

#line 551
void test_complete(void) {
    #line 552
    int x = 0;
    #line 555
    int y = 0;
    if ((x) == (0)) {
        #line 558
        y = 1;
    } else if ((x) == (1)) {
        #line 560
        y = 2;
    } else {
        #line 556
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 563
    x = 0;
    #line 567
    switch (x) {
        case 0: {
            #line 569
            y = 3;
            break;
        }
        case 1: {
            #line 571
            y = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
    #line 575
    x = 1;
    assert((x) >= (0));
}

#line 580
int main(int argc, const char *(*argv)) {
    #line 581
    if ((argv) == (0)) {
        #line 582
        (printf)("argv is null\n");
    }
    #line 585
    (test_complete)();
    #line 586
    (test_compound_literals)();
    #line 587
    (test_loops)();
    #line 588
    (test_sizeof)();
    #line 589
    (test_assign)();
    #line 590
    (test_enum)();
    #line 591
    (test_arrays)();
    #line 592
    (test_cast)();
    #line 593
    (test_init)();
    #line 594
    (test_lits)();
    #line 595
    (test_const)();
    #line 596
    (test_bool)();
    #line 597
    (test_ops)();
    #line 598
    (test_typeinfo)();
    #line 599
    (getchar)();
    #line 600
    return 0;
}
