// Forward includes
#include <stdio.h>

// Preamble
#include <stdbool.h>
#include <stdint.h>
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

// Sorted declarations
#line 1 "<builtin>"
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

#line 237 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 26 "<builtin>"
struct TypeFieldInfo {
    #line 27
    const char(*name);
    #line 28
    typeid type;
    #line 29
    int offset;
};

#line 32
struct TypeInfo {
    #line 33
    TypeKind kind;
    #line 34
    int size;
    #line 35
    int align;
    #line 36
    const char(*name);
    #line 37
    int count;
    #line 38
    typeid base;
    #line 39
    TypeFieldInfo(*fields);
    #line 40
    int num_fields;
};

#line 49
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

#line 136
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

#line 68
void test_loops(void);

#line 102
void test_nonmodifiable(void);

#line 114
struct UartCtrl {
    #line 115
    bool tx_enable;
    #line 115
    bool rx_enable;
};

#line 118
#define UART_CTRL_REG (uint *)(0x12345678)

#line 120
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 205
struct Vector {
    #line 206
    int x;
    #line 206
    int y;
};

#line 163
typedef IntOrPtr U;

#line 169
union IntOrPtr {
    #line 170
    int i;
    #line 171
    int(*p);
};

#line 146
int g(U u);

void k(void(*vp), int(*ip));

#line 155
void f1(void);

#line 160
void f3(int (a[]));

#line 165
int example_test(void);

#line 221
int fact_rec(int n);

#line 213
int fact_iter(int n);

#line 174
extern const char (escape_to_char[256]);

#line 184
extern int (a2[11]);

#line 187
int is_even(int digit);

#line 203
extern int i;

#line 209
void f2(Vector v);

#line 231
extern T(*p);

#line 229
#define M (1) + (sizeof(p))

struct T {
    #line 234
    int (a[M]);
};

#line 245
extern const char * (color_names[NUM_COLORS]);

#line 252
void test_enum(void);

#line 261
void test_assign(void);

#line 284
void benchmark(int n);

#line 291
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 297
void test_lits(void);

#line 314
void test_ops(void);

#line 344
#define IS_DEBUG true

#line 346
void test_bool(void);

#line 353
int test_ctrl(void);

#line 363
extern const int (j);

#line 364
extern const int(*q);

#line 365
extern const Vector (cv);

#line 367
void f4(const char(*x));

#line 370
struct ConstVector {
    #line 371
    const int (x);
    #line 371
    const int (y);
};

#line 374
void f5(const int(*p));

#line 377
void test_convert(void);

#line 385
void test_const(void);

#line 408
void test_init(void);

#line 421
void test_sizeof(void);

#line 429
void test_cast(void);

#line 438
struct Any {
    #line 439
    void(*ptr);
    #line 440
    typeid type;
};

#line 443
void print_any(Any any);

#line 461
void print_type(typeid type);

#line 456
void println_any(Any any);

#line 486
void println_type(typeid type);

#line 491
void print_typeinfo(typeid type);

#line 515
void println_typeinfo(typeid type);

#line 520
void test_typeof(void);

#line 535
void test_typeinfo(void);

#line 544
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[77] = {
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
    [19] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {{"name", .type = 32, .offset = offsetof(TypeFieldInfo, name)},{"type", .type = 8, .offset = offsetof(TypeFieldInfo, type)},{"offset", .type = 8, .offset = offsetof(TypeFieldInfo, offset)},}},
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {{"kind", .type = 18, .offset = offsetof(TypeInfo, kind)},{"size", .type = 8, .offset = offsetof(TypeInfo, size)},{"align", .type = 8, .offset = offsetof(TypeInfo, align)},{"name", .type = 32, .offset = offsetof(TypeInfo, name)},{"count", .type = 8, .offset = offsetof(TypeInfo, count)},{"base", .type = 8, .offset = offsetof(TypeInfo, base)},{"fields", .type = 33, .offset = offsetof(TypeInfo, fields)},{"num_fields", .type = 8, .offset = offsetof(TypeInfo, num_fields)},}},
    [21] = NULL, // Incomplete: SomeIncompleteType
    [22] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S1), .align = alignof(S1), .name = "S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"a", .type = 8, .offset = offsetof(S1, a)},{"b", .type = 44, .offset = offsetof(S1, b)},}},
    [23] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S2), .align = alignof(S2), .name = "S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {{"s1", .type = 22, .offset = offsetof(S2, s1)},}},
    [24] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(UartCtrl), .align = alignof(UartCtrl), .name = "UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"tx_enable", .type = 2, .offset = offsetof(UartCtrl, tx_enable)},{"rx_enable", .type = 2, .offset = offsetof(UartCtrl, rx_enable)},}},
    [25] = &(TypeInfo){TYPE_UNION, .size = sizeof(IntOrPtr), .align = alignof(IntOrPtr), .name = "IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"i", .type = 8, .offset = offsetof(IntOrPtr, i)},{"p", .type = 46, .offset = offsetof(IntOrPtr, p)},}},
    [26] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Vector), .align = alignof(Vector), .name = "Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"x", .type = 8, .offset = offsetof(Vector, x)},{"y", .type = 8, .offset = offsetof(Vector, y)},}},
    [27] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(T), .align = alignof(T), .name = "T", .num_fields = 1, .fields = (TypeFieldInfo[]) {{"a", .type = 61, .offset = offsetof(T, a)},}},
    [28] = NULL, // Enum
    [29] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"x", .type = 44, .offset = offsetof(ConstVector, x)},{"y", .type = 44, .offset = offsetof(ConstVector, y)},}},
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {{"ptr", .type = 16, .offset = offsetof(Any, ptr)},{"type", .type = 8, .offset = offsetof(Any, type)},}},
    [31] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = 3},
    [32] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 31},
    [33] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 19},
    [34] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 20},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 34},
    [36] = NULL, // Func
    [37] = NULL, // Func
    [38] = NULL, // Func
    [39] = NULL, // Func
    [40] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 21},
    [41] = NULL, // Func
    [42] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [43] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
    [44] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = 8},
    [45] = NULL, // Func
    [46] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [47] = NULL, // Func
    [48] = NULL, // Incomplete array type
    [49] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [50] = NULL, // Func
    [51] = NULL, // Func
    [52] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [53] = NULL, // Func
    [54] = NULL, // Func
    [55] = NULL, // Func
    [56] = NULL, // Func
    [57] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 31, .count = 256},
    [58] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [59] = NULL, // Func
    [60] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [61] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = 8, .count = 9},
    [62] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 32, .count = 4},
    [63] = NULL, // Func
    [64] = NULL, // Func
    [65] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 44},
    [66] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 26},
    [67] = NULL, // Func
    [68] = NULL, // Func
    [69] = NULL, // Func
    [70] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [71] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 70},
    [72] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [73] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 46},
    [74] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 65, .count = 42},
    [75] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 32},
    [76] = NULL, // Func
};
int num_typeinfos = 77;
TypeInfo **typeinfos = typeinfo_table;

// Definitions

#line 49 "<builtin>"
TypeInfo * get_typeinfo(typeid type) {
    #line 50
    if ((typeinfos) && ((type) < (num_typeinfos))) {
        #line 51
        return typeinfos[type];
    } else {
        #line 53
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

#line 68
void test_loops(void) {
    #line 71
    switch (0) {default: {
            if (1) {
                #line 74
                break;
            }
            #line 76
            for (;;) {
                #line 77
                continue;
            }
            break;
        }
    }
    #line 82
    while (0) {
    }
    #line 84
    for (int i = 0; (i) < (10); i++) {
    }
    #line 86
    for (;;) {
        #line 87
        break;
    }
    #line 89
    for (int i = 0;;) {
        #line 90
        break;
    }
    #line 92
    for (; 0;) {
    }
    #line 94
    for (int i = 0;; i++) {
        #line 95
        break;
    }
    #line 97
    int i = 0;
    #line 98
    for (;; i++) {
        #line 99
        break;
    }
}

#line 102
void test_nonmodifiable(void) {
    #line 103
    S1 s1;
    #line 104
    s1.a = 0;
    #line 107
    S2 s2;
    #line 108
    s2.s1.a = 0;
}

#line 120
uint32 pack(UartCtrl ctrl) {
    #line 121
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 124
UartCtrl unpack(uint32 word) {
    #line 125
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 128
void test_uart(void) {
    #line 129
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 130
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 131
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 132
    ctrl.rx_enable = true;
    #line 133
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 136
uchar h(void) {
    #line 137
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 138
    Vector (*v) = &((Vector){1, 2});
    #line 139
    v->x = 42;
    #line 140
    int (*p) = &((int){0});
    #line 141
    ulong x = ((uint){1}) + ((long){2});
    #line 142
    int y = +(c);
    #line 143
    return (uchar)(x);
}

#line 146
int g(U u) {
    #line 147
    return u.i;
}

#line 150
void k(void(*vp), int(*ip)) {
    #line 151
    vp = ip;
    #line 152
    ip = vp;
}

#line 155
void f1(void) {
    #line 156
    int (*p) = &((int){0});
    #line 157
    *(p) = 42;
}

#line 160
void f3(int (a[])) {
}

#line 165
int example_test(void) {
    #line 166
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 187
int is_even(int digit) {
    #line 188
    int b = 0;
    #line 189
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 191
            b = 1;
            break;
        }
    }
    #line 193
    return b;
}

int i;

#line 209
void f2(Vector v) {
    #line 210
    v = (Vector){0};
}

#line 213
int fact_iter(int n) {
    #line 214
    int r = 1;
    #line 215
    for (int i = 0; (i) <= (n); i++) {
        #line 216
        r *= i;
    }
    #line 218
    return r;
}

#line 221
int fact_rec(int n) {
    #line 222
    if ((n) == (0)) {
        #line 223
        return 1;
    } else {
        #line 225
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 252
void test_enum(void) {
    #line 253
    Color a = COLOR_RED;
    #line 254
    Color b = COLOR_RED;
    #line 255
    int c = (a) + (b);
    #line 256
    int i = a;
    #line 257
    a = i;
    #line 258
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 261
void test_assign(void) {
    #line 262
    int i = 0;
    #line 263
    float f = 3.14f;
    #line 264
    int(*p) = &(i);
    #line 265
    i++;
    #line 266
    i--;
    #line 267
    p++;
    #line 268
    p--;
    #line 269
    p += 1;
    #line 270
    i /= 2;
    #line 271
    i *= 123;
    #line 272
    i %= 3;
    #line 273
    i <<= 1;
    #line 274
    i >>= 2;
    #line 275
    i &= 0xFF;
    #line 276
    i |= 0xFF00;
    #line 277
    i ^= 0xFF0;
}

#line 284
void benchmark(int n) {
    #line 285
    int r = 1;
    #line 286
    for (int i = 1; (i) <= (n); i++) {
        #line 287
        r *= i;
    }
}

#line 291
int va_test(int x, ...) {
    #line 292
    return 0;
}

#line 297
void test_lits(void) {
    #line 298
    float f = 3.14f;
    #line 299
    double d = 3.14;
    #line 300
    int i = 1;
    #line 301
    uint u = 0xFFFFFFFFu;
    #line 302
    long l = 1l;
    #line 303
    ulong ul = 1ul;
    #line 304
    llong ll = 0x100000000ll;
    #line 305
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 306
    uint x1 = 0xFFFFFFFF;
    #line 307
    llong x2 = 4294967295;
    #line 308
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 309
    int x4 = (0xAA) + (0x55);
}

#line 314
void test_ops(void) {
    #line 315
    float pi = 3.14f;
    #line 316
    float f = 0.0f;
    #line 317
    f = +(pi);
    #line 318
    f = -(pi);
    #line 319
    int n = -(1);
    #line 320
    n = ~(n);
    #line 321
    f = ((f) * (pi)) + (n);
    #line 322
    f = (pi) / (pi);
    #line 323
    n = (3) % (2);
    #line 324
    n = (n) + ((uchar)(1));
    #line 325
    int (*p) = &(n);
    #line 326
    p = (p) + (1);
    #line 327
    n = (int)(((p) + (1)) - (p));
    #line 328
    n = (n) << (1);
    #line 329
    n = (n) >> (1);
    #line 330
    int b = ((p) + (1)) > (p);
    #line 331
    b = ((p) + (1)) >= (p);
    #line 332
    b = ((p) + (1)) < (p);
    #line 333
    b = ((p) + (1)) <= (p);
    #line 334
    b = ((p) + (1)) == (p);
    #line 335
    b = (1) > (2);
    #line 336
    b = (1.23f) <= (pi);
    #line 337
    n = 0xFF;
    #line 338
    b = (n) & (~(1));
    #line 339
    b = (n) & (1);
    #line 340
    b = ((n) & (~(1))) ^ (1);
    #line 341
    b = (p) && (pi);
}

#line 346
void test_bool(void) {
    #line 347
    bool b = false;
    #line 348
    b = true;
    #line 349
    int i = 0;
    #line 350
    i = IS_DEBUG;
}

#line 353
int test_ctrl(void) {
    #line 354
    switch (1) {
        case 0: {
            #line 356
            return 0;
            break;
        }default: {
            #line 358
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 367
void f4(const char(*x)) {
}

#line 374
void f5(const int(*p)) {
}

#line 377
void test_convert(void) {
    #line 378
    const int(*a) = 0;
    #line 379
    int(*b) = 0;
    #line 380
    a = b;
    #line 381
    void(*p) = 0;
    #line 382
    (f5)(p);
}

#line 385
void test_const(void) {
    #line 386
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 389
    i = 1;
    #line 392
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 397
    const char (*p) = (const char *)(0);
    #line 398
    p = (escape_to_char) + (1);
    #line 399
    char (*q) = (char *)(escape_to_char);
    #line 400
    c = q['n'];
    p = (const char *)(1);
    #line 405
    i = (int)((ullong)(p));
}

#line 408
void test_init(void) {
    #line 409
    int x = (const int)(0);
    #line 410
    int y;
    #line 411
    y = 0;
    #line 412
    int z = 42;
    #line 413
    int (a[3]) = {1, 2, 3};
    #line 416
    for (ullong i = 0; (i) < (10); i++) {
        #line 417
        (printf)("%llu\n", i);
    }
}

#line 421
void test_sizeof(void) {
    #line 422
    int i = 0;
    #line 423
    ullong n = sizeof(i);
    #line 424
    n = sizeof(int);
    #line 425
    n = sizeof(int);
    #line 426
    n = sizeof(int *);
}

#line 429
void test_cast(void) {
    #line 430
    int(*p) = 0;
    #line 431
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 443
void print_any(Any any) {
    #line 444
    switch (any.type) {
        case 8: {
            #line 446
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 448
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 450
            (printf)("<unknown>");
            break;
        }
    }
    #line 452
    (printf)(": ");
    #line 453
    (print_type)(any.type);
}

#line 456
void println_any(Any any) {
    #line 457
    (print_any)(any);
    #line 458
    (printf)("\n");
}

#line 461
void print_type(typeid type) {
    #line 462
    TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 463
    if (!(typeinfo)) {
        #line 464
        (printf)("<typeid %d>", type);
        #line 465
        return;
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
void test_typeof(void) {
    #line 521
    int i = 42;
    #line 522
    float f = 3.14f;
    #line 523
    int (*p) = &(i);
    int type1 = 8;
    #line 526
    int type2 = 8;
    #line 527
    int type3 = 8;
    #line 528
    int type4 = 46;
    (println_any)((Any){&(i), 8});
    #line 531
    (println_any)((Any){&(f), 14});
    #line 532
    (println_any)((Any){&(p), 46});
}

#line 535
void test_typeinfo(void) {
    #line 536
    (println_typeinfo)(8);
    #line 537
    (println_typeinfo)(24);
    #line 538
    (println_type)(8);
    #line 539
    (println_type)(65);
    #line 540
    (println_type)(74);
    #line 541
    (println_type)(24);
}

#line 544
int main(int argc, const char *(*argv)) {
    #line 545
    if ((argv) == (0)) {
        #line 546
        (printf)("argv is null\n");
    }
    #line 548
    (test_typeof)();
    #line 549
    (test_loops)();
    #line 550
    (test_sizeof)();
    #line 551
    (test_assign)();
    #line 552
    (test_enum)();
    #line 553
    (test_arrays)();
    #line 554
    (test_cast)();
    #line 555
    (test_init)();
    #line 556
    (test_lits)();
    #line 557
    (test_const)();
    #line 558
    (test_bool)();
    #line 559
    (test_ops)();
    #line 560
    (test_typeinfo)();
    #line 561
    (getchar)();
    #line 562
    return 0;
}
