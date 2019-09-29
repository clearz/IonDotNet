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
TypeInfo * get_typeinfo(typeid type);

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

#line 347
#define IS_DEBUG true

#line 349
void test_bool(void);

#line 356
int test_ctrl(void);

#line 366
extern const int (j);

#line 367
extern const int(*q);

#line 368
extern const Vector (cv);

#line 370
void f4(const char(*x));

#line 373
struct ConstVector {
    #line 374
    const int (x);
    #line 374
    const int (y);
};

#line 377
void f5(const int(*p));

#line 380
void test_convert(void);

#line 388
void test_const(void);

#line 411
void test_init(void);

#line 424
void test_sizeof(void);

#line 432
void test_cast(void);

#line 441
struct Any {
    #line 442
    void(*ptr);
    #line 443
    typeid type;
};

#line 446
void print_any(Any any);

#line 464
void print_type(typeid type);

#line 459
void println_any(Any any);

#line 488
void println_type(typeid type);

#line 493
void print_typeinfo(typeid type);

#line 517
void println_typeinfo(typeid type);

#line 522
void test_typeinfo(void);

#line 540
void test_compound_literals(void);

#line 546
void test_complete(void);

#line 574
void test_alignof(void);

#line 582
struct BufHdr {
    #line 583
    usize cap;
    #line 583
    usize len;
    #line 584
    char (buf[1]);
};

#line 589
void test_offsetof(void);

#line 594
struct Thing {
    #line 595
    int a;
};

#line 598
extern Thing thing;

#line 600
Thing * returns_ptr(void);

const Thing * returns_ptr_to_const(void);

void test_lvalue(void);

#line 616
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[89] = {
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
    [21] = NULL, // Incomplete: SomeIncompleteType
    [22] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S1), .align = alignof(S1), .name = "S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(S1, a)},
        {"b", .type = 49, .offset = offsetof(S1, b)},}},
    [23] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(S2), .align = alignof(S2), .name = "S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = 22, .offset = offsetof(S2, s1)},}},
    [24] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(UartCtrl), .align = alignof(UartCtrl), .name = "UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = 2, .offset = offsetof(UartCtrl, tx_enable)},
        {"rx_enable", .type = 2, .offset = offsetof(UartCtrl, rx_enable)},}},
    [25] = &(TypeInfo){TYPE_UNION, .size = sizeof(IntOrPtr), .align = alignof(IntOrPtr), .name = "IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = 8, .offset = offsetof(IntOrPtr, i)},
        {"p", .type = 51, .offset = offsetof(IntOrPtr, p)},}},
    [26] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Vector), .align = alignof(Vector), .name = "Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 8, .offset = offsetof(Vector, x)},
        {"y", .type = 8, .offset = offsetof(Vector, y)},}},
    [27] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(T), .align = alignof(T), .name = "T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 66, .offset = offsetof(T, a)},}},
    [28] = NULL, // Enum
    [29] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ConstVector), .align = alignof(ConstVector), .name = "ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = 49, .offset = offsetof(ConstVector, x)},
        {"y", .type = 49, .offset = offsetof(ConstVector, y)},}},
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = 16, .offset = offsetof(Any, ptr)},
        {"type", .type = 8, .offset = offsetof(Any, type)},}},
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(BufHdr), .align = alignof(BufHdr), .name = "BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = 13, .offset = offsetof(BufHdr, cap)},
        {"len", .type = 13, .offset = offsetof(BufHdr, len)},
        {"buf", .type = 81, .offset = offsetof(BufHdr, buf)},}},
    [32] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Thing), .align = alignof(Thing), .name = "Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = 8, .offset = offsetof(Thing, a)},}},
    [33] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = 3},
    [34] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 33},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 19},
    [36] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 20},
    [37] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 36},
    [38] = NULL, // Func
    [39] = NULL, // Func
    [40] = NULL, // Func
    [41] = NULL, // Func
    [42] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = 1},
    [43] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 42},
    [44] = NULL, // Func
    [45] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 21},
    [46] = NULL, // Func
    [47] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [48] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
    [49] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = 8},
    [50] = NULL, // Func
    [51] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [52] = NULL, // Func
    [53] = NULL, // Incomplete array type
    [54] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 9},
    [55] = NULL, // Func
    [56] = NULL, // Func
    [57] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 26},
    [58] = NULL, // Func
    [59] = NULL, // Func
    [60] = NULL, // Func
    [61] = NULL, // Func
    [62] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = 33, .count = 256},
    [63] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = 8, .count = 11},
    [64] = NULL, // Func
    [65] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 27},
    [66] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = 8, .count = 9},
    [67] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 34, .count = 4},
    [68] = NULL, // Func
    [69] = NULL, // Func
    [70] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 49},
    [71] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Vector), .align = alignof(const Vector), .base = 26},
    [72] = NULL, // Func
    [73] = NULL, // Func
    [74] = NULL, // Func
    [75] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = 14},
    [76] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 75},
    [77] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 14},
    [78] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 16},
    [79] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = 70, .count = 42},
    [80] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = 30},
    [81] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = 3, .count = 1},
    [82] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 32},
    [83] = NULL, // Func
    [84] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Thing), .align = alignof(const Thing), .base = 32},
    [85] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 84},
    [86] = NULL, // Func
    [87] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 34},
    [88] = NULL, // Func
};
int num_typeinfos = 89;
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

#line 61 "test1.ion"
void f10(int (a[3])) {
    #line 62
    a[1] = 42;
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
    for (int i = 0; (i) < (10); i++) {
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
    for (int i = 0;; i++) {
        #line 98
        break;
    }
    #line 100
    int i = 0;
    #line 101
    for (;; i++) {
        #line 102
        break;
    }
}

#line 105
void test_nonmodifiable(void) {
    #line 106
    S1 s1;
    #line 107
    s1.a = 0;
    #line 110
    S2 s2;
    #line 111
    s2.s1.a = 0;
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
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 134
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 135
    ctrl.rx_enable = true;
    #line 136
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 139
uchar h(void) {
    #line 140
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 141
    Vector (*v) = &((Vector){1, 2});
    #line 142
    v->x = 42;
    #line 143
    int (*p) = &((int){0});
    #line 144
    ulong x = ((uint){1}) + ((long){2});
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
    vp = ip;
    #line 155
    ip = vp;
}

#line 158
void f1(void) {
    #line 159
    int (*p) = &((int){0});
    #line 160
    *(p) = 42;
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
            b = 1;
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
    v = (Vector){0};
}

#line 216
int fact_iter(int n) {
    #line 217
    int r = 1;
    #line 218
    for (int i = 0; (i) <= (n); i++) {
        #line 219
        r *= i;
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
    a = i;
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
    i++;
    #line 269
    i--;
    #line 270
    p++;
    #line 271
    p--;
    #line 272
    p += 1;
    #line 273
    i /= 2;
    #line 274
    i *= 123;
    #line 275
    i %= 3;
    #line 276
    i <<= 1;
    #line 277
    i >>= 2;
    #line 278
    i &= 0xFF;
    #line 279
    i |= 0xFF00;
    #line 280
    i ^= 0xFF0;
}

#line 287
void benchmark(int n) {
    #line 288
    int r = 1;
    #line 289
    for (int i = 1; (i) <= (n); i++) {
        #line 290
        r *= i;
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
    long l = 1l;
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
    f = +(pi);
    #line 321
    f = -(pi);
    #line 322
    int n = -(1);
    #line 323
    n = ~(n);
    #line 324
    f = ((f) * (pi)) + (n);
    #line 325
    f = (pi) / (pi);
    #line 326
    n = (3) % (2);
    #line 327
    n = (n) + ((uchar)(1));
    #line 328
    int (*p) = &(n);
    #line 329
    p = (p) + (1);
    #line 330
    n = (int)(((p) + (1)) - (p));
    #line 331
    n = (n) << (1);
    #line 332
    n = (n) >> (1);
    #line 333
    int b = ((p) + (1)) > (p);
    #line 334
    b = ((p) + (1)) >= (p);
    #line 335
    b = ((p) + (1)) < (p);
    #line 336
    b = ((p) + (1)) <= (p);
    #line 337
    b = ((p) + (1)) == (p);
    #line 338
    b = (1) > (2);
    #line 339
    b = (1.23f) <= (pi);
    #line 340
    n = 0xFF;
    #line 341
    b = (n) & (~(1));
    #line 342
    b = (n) & (1);
    #line 343
    b = ((n) & (~(1))) ^ (1);
    #line 344
    b = (p) && (pi);
}

#line 349
void test_bool(void) {
    #line 350
    bool b = false;
    #line 351
    b = true;
    #line 352
    int i = 0;
    #line 353
    i = IS_DEBUG;
}

#line 356
int test_ctrl(void) {
    #line 357
    switch (1) {
        case 0: {
            #line 359
            return 0;
            break;
        }default: {
            #line 361
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 370
void f4(const char(*x)) {
}

#line 377
void f5(const int(*p)) {
}

#line 380
void test_convert(void) {
    #line 381
    const int(*a) = 0;
    #line 382
    int(*b) = 0;
    #line 383
    a = b;
    #line 384
    void(*p) = 0;
    #line 385
    (f5)(p);
}

#line 388
void test_const(void) {
    #line 389
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 392
    i = 1;
    #line 395
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 400
    const char (*p) = (const char *)(0);
    #line 401
    p = (escape_to_char) + (1);
    #line 402
    char (*q) = (char *)(escape_to_char);
    #line 403
    c = q['n'];
    p = (const char *)(1);
    #line 408
    i = (int)((ullong)(p));
}

#line 411
void test_init(void) {
    #line 412
    int x = (const int)(0);
    #line 413
    int y;
    #line 414
    y = 0;
    #line 415
    int z = 42;
    #line 416
    int (a[3]) = {1, 2, 3};
    #line 419
    for (ullong i = 0; (i) < (10); i++) {
        #line 420
        (printf)("%llu\n", i);
    }
}

#line 424
void test_sizeof(void) {
    #line 425
    int i = 0;
    #line 426
    ullong n = sizeof(i);
    #line 427
    n = sizeof(int);
    #line 428
    n = sizeof(int);
    #line 429
    n = sizeof(int *);
}

#line 432
void test_cast(void) {
    #line 433
    int(*p) = 0;
    #line 434
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 446
void print_any(Any any) {
    #line 447
    switch (any.type) {
        case 8: {
            #line 449
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 451
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 453
            (printf)("<unknown>");
            break;
        }
    }
    #line 455
    (printf)(": ");
    #line 456
    (print_type)(any.type);
}

#line 459
void println_any(Any any) {
    #line 460
    (print_any)(any);
    #line 461
    (printf)("\n");
}

#line 464
void print_type(typeid type) {
    #line 465
    TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 466
    if (!(typeinfo)) {
        #line 467
        (printf)("<typeid %d>", type);
    }
    #line 469
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 471
            (print_type)(typeinfo->base);
            #line 472
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 474
            (print_type)(typeinfo->base);
            #line 475
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 477
            (print_type)(typeinfo->base);
            #line 478
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 480
            if (typeinfo->name) {
                #line 481
                (printf)("%s", typeinfo->name);
            } else {
                #line 483
                (printf)("typeid %d>", type);
            }
            break;
        }
    }
}

#line 488
void println_type(typeid type) {
    #line 489
    (print_type)(type);
    #line 490
    (printf)("\n");
}

#line 493
void print_typeinfo(typeid type) {
    #line 494
    TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 495
    if (!(typeinfo)) {
        #line 496
        (printf)("<typeid %d>", type);
        #line 497
        return;
    }
    #line 499
    (printf)("<");
    #line 500
    (print_type)(type);
    #line 501
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 502
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 505
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 506
            for (int i = 0; (i) < (typeinfo->num_fields); i++) {
                #line 507
                TypeFieldInfo field = typeinfo->fields[i];
                #line 508
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 509
                (print_type)(field.type);
                #line 510
                (printf)("; ");
            }
            #line 512
            (printf)("}");
            break;
        }
    }
    #line 514
    (printf)(">");
}

#line 517
void println_typeinfo(typeid type) {
    #line 518
    (print_typeinfo)(type);
    #line 519
    (printf)("\n");
}

#line 522
void test_typeinfo(void) {
    #line 523
    int i = 42;
    #line 524
    float f = 3.14f;
    #line 525
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 528
    (println_any)((Any){&(f), 14});
    #line 529
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 532
    (println_type)(70);
    #line 533
    (println_type)(79);
    #line 534
    (println_type)(24);
    (println_typeinfo)(8);
    #line 537
    (println_typeinfo)(24);
}

#line 540
void test_compound_literals(void) {
    #line 541
    int i = 42;
    #line 542
    const Any (x) = {&(i), 8};
    #line 543
    Any y = {&(i), 8};
}

#line 546
void test_complete(void) {
    #line 547
    int x = 0;
    #line 550
    int y = 0;
    if ((x) == (0)) {
        #line 553
        y = 1;
    } else if ((x) == (1)) {
        #line 555
        y = 2;
    } else {
        #line 551
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 558
    x = 1;
    assert((x) >= (0));
    x = 0;
    #line 566
    switch (x) {
        case 0: {
            #line 568
            y = 3;
            break;
        }
        case 1: {
            #line 570
            y = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 574
void test_alignof(void) {
    #line 575
    int i = 42;
    #line 576
    ullong n1 = alignof(int);
    #line 577
    ullong n2 = alignof(int);
    #line 578
    ullong n3 = alignof(ullong);
    #line 579
    ullong n4 = alignof(int *);
}

#line 589
void test_offsetof(void) {
    #line 590
    ullong n = offsetof(BufHdr, buf);
}

Thing thing;

Thing * returns_ptr(void) {
    #line 601
    return &(thing);
}

#line 604
const Thing * returns_ptr_to_const(void) {
    #line 605
    return &(thing);
}

#line 608
void test_lvalue(void) {
    (returns_ptr)()->a = 5;
    const Thing (*p) = (returns_ptr_to_const)();
}

#line 616
int main(int argc, const char *(*argv)) {
    #line 617
    if ((argv) == (0)) {
        #line 618
        (printf)("argv is null\n");
    }
    #line 620
    (test_lvalue)();
    #line 621
    (test_alignof)();
    #line 622
    (test_offsetof)();
    #line 623
    (test_complete)();
    #line 624
    (test_compound_literals)();
    #line 625
    (test_loops)();
    #line 626
    (test_sizeof)();
    #line 627
    (test_assign)();
    #line 628
    (test_enum)();
    #line 629
    (test_arrays)();
    #line 630
    (test_cast)();
    #line 631
    (test_init)();
    #line 632
    (test_lits)();
    #line 633
    (test_const)();
    #line 634
    (test_bool)();
    #line 635
    (test_ops)();
    #line 636
    (test_typeinfo)();
    #line 637
    (getchar)();
    #line 638
    return 0;
}
