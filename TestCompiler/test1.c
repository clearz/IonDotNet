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

#line 261 "test1.ion"
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

#line 15 "test1.ion"
extern int(*some_array);

extern SomeIncompleteType(*incomplete_ptr);

#line 32
#define PI 3.14f

#line 33
#define PI2 (PI) + (PI)

#line 35
#define U8 (uint8)(42)

#line 37
extern char c;

#line 38
extern uchar uc;

#line 39
extern schar sc;

#line 41
#define N (((char)(42)) + (8)) != (0)

#line 160
uchar h(void);

#line 43
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 45
extern char (*code);

#line 54
struct S1 {
    #line 55
    int a;
    #line 56
    const int (b);
};

#line 59
struct S2 {
    #line 60
    S1 s1;
};

#line 63
void test_modify(void);

#line 82
void f10(int (a[3]));

void test_arrays(void);

#line 92
void test_loops(void);

#line 126
void test_nonmodifiable(void);

#line 138
struct UartCtrl {
    #line 139
    bool tx_enable;
    #line 139
    bool rx_enable;
};

#line 142
#define UART_CTRL_REG (uint *)(0x12345678)

#line 144
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 229
struct Vector {
    #line 230
    int x;
    #line 230
    int y;
};

#line 187
typedef IntOrPtr U;

#line 193
union IntOrPtr {
    #line 194
    int i;
    #line 195
    int(*p);
};

#line 170
int g(U u);

void k(void(*vp), int(*ip));

#line 179
void f1(void);

#line 184
void f3(int (a[]));

#line 189
int example_test(void);

#line 245
int fact_rec(int n);

#line 237
int fact_iter(int n);

#line 198
extern const char (escape_to_char[256]);

#line 208
extern int (a2[11]);

#line 211
int is_even(int digit);

#line 227
extern int i;

#line 233
void f2(Vector v);

#line 255
extern T(*p);

#line 253
#define M (1) + (sizeof(p))

struct T {
    #line 258
    int (a[M]);
};

#line 269
extern const char * (color_names[NUM_COLORS]);

#line 276
void test_enum(void);

#line 285
void test_assign(void);

#line 308
void benchmark(int n);

#line 315
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 321
void test_lits(void);

#line 338
void test_ops(void);

#line 368
#define IS_DEBUG true

#line 370
void test_bool(void);

#line 377
int test_ctrl(void);

#line 387
extern const int (j);

#line 388
extern const int(*q);

#line 389
extern const Vector (cv);

#line 391
void f4(const char(*x));

#line 394
struct ConstVector {
    #line 395
    const int (x);
    #line 395
    const int (y);
};

#line 398
void f5(const int(*p));

#line 401
void test_convert(void);

#line 409
void test_const(void);

#line 432
void test_init(void);

#line 447
void test_sizeof(void);

#line 455
void test_cast(void);

#line 464
void print_any(Any any);

#line 486
void print_type(typeid type);

#line 477
void println_any(Any any);

#line 482
void print_typeid(typeid type);

#line 511
void println_type(typeid type);

#line 516
void print_typeinfo(typeid type);

#line 540
void println_typeinfo(typeid type);

#line 545
void test_typeinfo(void);

#line 565
struct Ints {
    #line 566
    int num_ints;
    #line 567
    int(*int_ptr);
    #line 568
    int (int_arr[3]);
};

#line 571
void test_compound_literals(void);

#line 587
void test_complete(void);

#line 615
void test_alignof(void);

#line 623
struct BufHdr {
    #line 624
    usize cap;
    #line 624
    usize len;
    #line 625
    char (buf[1]);
};

#line 630
void test_offsetof(void);

#line 635
struct Thing {
    #line 636
    int a;
};

#line 639
extern Thing thing;

#line 641
Thing * returns_ptr(void);

const Thing * returns_ptr_to_const(void);

void test_lvalue(void);

#line 656
int main(int argc, const char *(*argv));

// Typeinfo

TypeInfo *typeinfo_table[100] = {
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
        {"p", .type = 47, .offset = offsetof(IntOrPtr, p)},}},
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
        {"int_ptr", .type = 47, .offset = offsetof(Ints, int_ptr)},
        {"int_arr", .type = 49, .offset = offsetof(Ints, int_arr)},}},
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
    [47] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 8},
    [48] = NULL, // Incomplete array type
    [49] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = 8, .count = 3},
    [50] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 22},
    [51] = NULL, // Func
    [52] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = 3},
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
    [70] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = 35, .count = 4},
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

#line 63 "test1.ion"
void test_modify(void) {
    #line 64
    int i = 42;
    #line 65
    int (*p) = &(i);
    #line 66
    (p)--;
    #line 67
    int x = *((p)++);
    #line 68
    assert((x) == (*(--(p))));
    #line 69
    (*(p))++;
    #line 70
    (*(p))--;
    #line 71
    int (stk[16]);
    #line 72
    int(*sp) = stk;
    #line 73
    (*((sp)++)) = 1;
    #line 74
    (*((sp)++)) = 2;
    #line 75
    (x) = *(--(sp));
    #line 76
    assert((x) == (2));
    #line 77
    (x) = *(--(sp));
    #line 78
    assert((x) == (1));
    #line 79
    assert((sp) == (stk));
}

#line 82
void f10(int (a[3])) {
    #line 83
    (a[1]) = 42;
}

#line 86
void test_arrays(void) {
    #line 87
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 92
void test_loops(void) {
    #line 95
    switch (0) {default: {
            if (1) {
                #line 98
                break;
            }
            #line 100
            for (;;) {
                #line 101
                continue;
            }
            break;
        }
    }
    #line 106
    while (0) {
    }
    #line 108
    for (int i = 0; (i) < (10); (i)++) {
    }
    #line 110
    for (;;) {
        #line 111
        break;
    }
    #line 113
    for (int i = 0;;) {
        #line 114
        break;
    }
    #line 116
    for (; 0;) {
    }
    #line 118
    for (int i = 0;; (i)++) {
        #line 119
        break;
    }
    #line 121
    int i = 0;
    #line 122
    for (;; (i)++) {
        #line 123
        break;
    }
}

#line 126
void test_nonmodifiable(void) {
    #line 127
    S1 s1;
    #line 128
    (s1.a) = 0;
    #line 131
    S2 s2;
    #line 132
    (s2.s1.a) = 0;
}

#line 144
uint32 pack(UartCtrl ctrl) {
    #line 145
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 148
UartCtrl unpack(uint32 word) {
    #line 149
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 152
void test_uart(void) {
    #line 153
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 154
    (*(UART_CTRL_REG)) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 155
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 156
    (ctrl.rx_enable) = true;
    #line 157
    (*(UART_CTRL_REG)) = (pack)(ctrl);
}

#line 160
uchar h(void) {
    #line 161
    ((Vector){.x = 1, .y = 2}.x) = 42;
    #line 162
    Vector (*v) = &((Vector){1, 2});
    #line 163
    (v->x) = 42;
    #line 164
    int (*p) = &((int){0});
    #line 165
    ulong x = ((uint){1}) + ((long){2});
    #line 166
    int y = +(c);
    #line 167
    return (uchar)(x);
}

#line 170
int g(U u) {
    #line 171
    return u.i;
}

#line 174
void k(void(*vp), int(*ip)) {
    #line 175
    (vp) = ip;
    #line 176
    (ip) = vp;
}

#line 179
void f1(void) {
    #line 180
    int (*p) = &((int){0});
    #line 181
    (*(p)) = 42;
}

#line 184
void f3(int (a[])) {
}

#line 189
int example_test(void) {
    #line 190
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 211
int is_even(int digit) {
    #line 212
    int b = 0;
    #line 213
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 215
            (b) = 1;
            break;
        }
    }
    #line 217
    return b;
}

int i;

#line 233
void f2(Vector v) {
    #line 234
    (v) = (Vector){0};
}

#line 237
int fact_iter(int n) {
    #line 238
    int r = 1;
    #line 239
    for (int i = 0; (i) <= (n); (i)++) {
        #line 240
        (r) *= i;
    }
    #line 242
    return r;
}

#line 245
int fact_rec(int n) {
    #line 246
    if ((n) == (0)) {
        #line 247
        return 1;
    } else {
        #line 249
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 276
void test_enum(void) {
    #line 277
    Color a = COLOR_RED;
    #line 278
    Color b = COLOR_RED;
    #line 279
    int c = (a) + (b);
    #line 280
    int i = a;
    #line 281
    (a) = i;
    #line 282
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 285
void test_assign(void) {
    #line 286
    int i = 0;
    #line 287
    float f = 3.14f;
    #line 288
    int(*p) = &(i);
    #line 289
    (i)++;
    #line 290
    (i)--;
    #line 291
    (p)++;
    #line 292
    (p)--;
    #line 293
    (p) += 1;
    #line 294
    (i) /= 2;
    #line 295
    (i) *= 123;
    #line 296
    (i) %= 3;
    #line 297
    (i) <<= 1;
    #line 298
    (i) >>= 2;
    #line 299
    (i) &= 0xFF;
    #line 300
    (i) |= 0xFF00;
    #line 301
    (i) ^= 0xFF0;
}

#line 308
void benchmark(int n) {
    #line 309
    int r = 1;
    #line 310
    for (int i = 1; (i) <= (n); (i)++) {
        #line 311
        (r) *= i;
    }
}

#line 315
int va_test(int x, ...) {
    #line 316
    return 0;
}

#line 321
void test_lits(void) {
    #line 322
    float f = 3.14f;
    #line 323
    double d = 3.14;
    #line 324
    int i = 1;
    #line 325
    uint u = 0xFFFFFFFFu;
    #line 326
    long l = 1l;
    #line 327
    ulong ul = 1ul;
    #line 328
    llong ll = 0x100000000ll;
    #line 329
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 330
    uint x1 = 0xFFFFFFFF;
    #line 331
    llong x2 = 4294967295;
    #line 332
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 333
    int x4 = (0xAA) + (0x55);
}

#line 338
void test_ops(void) {
    #line 339
    float pi = 3.14f;
    #line 340
    float f = 0.0f;
    #line 341
    (f) = +(pi);
    #line 342
    (f) = -(pi);
    #line 343
    int n = -(1);
    #line 344
    (n) = ~(n);
    #line 345
    (f) = ((f) * (pi)) + (n);
    #line 346
    (f) = (pi) / (pi);
    #line 347
    (n) = (3) % (2);
    #line 348
    (n) = (n) + ((uchar)(1));
    #line 349
    int (*p) = &(n);
    #line 350
    (p) = (p) + (1);
    #line 351
    (n) = (int)(((p) + (1)) - (p));
    #line 352
    (n) = (n) << (1);
    #line 353
    (n) = (n) >> (1);
    #line 354
    int b = ((p) + (1)) > (p);
    #line 355
    (b) = ((p) + (1)) >= (p);
    #line 356
    (b) = ((p) + (1)) < (p);
    #line 357
    (b) = ((p) + (1)) <= (p);
    #line 358
    (b) = ((p) + (1)) == (p);
    #line 359
    (b) = (1) > (2);
    #line 360
    (b) = (1.23f) <= (pi);
    #line 361
    (n) = 0xFF;
    #line 362
    (b) = (n) & (~(1));
    #line 363
    (b) = (n) & (1);
    #line 364
    (b) = ((n) & (~(1))) ^ (1);
    #line 365
    (b) = (p) && (pi);
}

#line 370
void test_bool(void) {
    #line 371
    bool b = false;
    #line 372
    (b) = true;
    #line 373
    int i = 0;
    #line 374
    (i) = IS_DEBUG;
}

#line 377
int test_ctrl(void) {
    #line 378
    switch (1) {
        case 0: {
            #line 380
            return 0;
            break;
        }default: {
            #line 382
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 391
void f4(const char(*x)) {
}

#line 398
void f5(const int(*p)) {
}

#line 401
void test_convert(void) {
    #line 402
    const int(*a) = 0;
    #line 403
    int(*b) = 0;
    #line 404
    (a) = b;
    #line 405
    void(*p) = 0;
    #line 406
    (f5)(p);
}

#line 409
void test_const(void) {
    #line 410
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 413
    (i) = 1;
    #line 416
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 421
    const char (*p) = (const char *)(0);
    #line 422
    (p) = (escape_to_char) + (1);
    #line 423
    char (*q) = (char *)(escape_to_char);
    #line 424
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 429
    (i) = (int)((ullong)(p));
}

#line 432
void test_init(void) {
    #line 433
    int x = (const int)(0);
    #line 434
    int y;
    #line 435
    (y) = 0;
    #line 436
    int z = 42;
    #line 437
    int (a[3]) = {1, 2, 3};
    #line 440
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 441
        (printf)("%llu\n", i);
    }
    #line 443
    int (b[4]) = {1, 2, 3, 4};
    #line 444
    (b[0]) = a[2];
}

#line 447
void test_sizeof(void) {
    #line 448
    int i = 0;
    #line 449
    ullong n = sizeof(i);
    #line 450
    (n) = sizeof(int);
    #line 451
    (n) = sizeof(int);
    #line 452
    (n) = sizeof(int *);
}

#line 455
void test_cast(void) {
    #line 456
    int(*p) = 0;
    #line 457
    uint64 a = 0;
    (a) = (uint64)(p);
    (p) = (int *)(a);
}

#line 464
void print_any(Any any) {
    #line 465
    switch (any.type) {
        case 8: {
            #line 467
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case 14: {
            #line 469
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 471
            (printf)("<unknown>");
            break;
        }
    }
    #line 473
    (printf)(": ");
    #line 474
    (print_type)(any.type);
}

#line 477
void println_any(Any any) {
    #line 478
    (print_any)(any);
    #line 479
    (printf)("\n");
}

#line 482
void print_typeid(typeid type) {
    #line 483
    (printf)("typeid(%d)", type);
}

#line 486
void print_type(typeid type) {
    #line 487
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 488
    if (!(typeinfo)) {
        #line 489
        (print_typeid)(type);
        #line 490
        return;
    }
    #line 492
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 494
            (print_type)(typeinfo->base);
            #line 495
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 497
            (print_type)(typeinfo->base);
            #line 498
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 500
            (print_type)(typeinfo->base);
            #line 501
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 503
            if (typeinfo->name) {
                #line 504
                (printf)("%s", typeinfo->name);
            } else {
                #line 506
                (print_typeid)(type);
            }
            break;
        }
    }
}

#line 511
void println_type(typeid type) {
    #line 512
    (print_type)(type);
    #line 513
    (printf)("\n");
}

#line 516
void print_typeinfo(typeid type) {
    #line 517
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 518
    if (!(typeinfo)) {
        #line 519
        (print_typeid)(type);
        #line 520
        return;
    }
    #line 522
    (printf)("<");
    #line 523
    (print_type)(type);
    #line 524
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 525
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 528
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 529
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 530
                TypeFieldInfo field = typeinfo->fields[i];
                #line 531
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 532
                (print_type)(field.type);
                #line 533
                (printf)("; ");
            }
            #line 535
            (printf)("}");
            break;
        }
    }
    #line 537
    (printf)(">");
}

#line 540
void println_typeinfo(typeid type) {
    #line 541
    (print_typeinfo)(type);
    #line 542
    (printf)("\n");
}

#line 545
void test_typeinfo(void) {
    #line 546
    int i = 42;
    #line 547
    float f = 3.14f;
    #line 548
    void (*p) = NULL;
    (println_any)((Any){&(i), 8});
    #line 551
    (println_any)((Any){&(f), 14});
    #line 552
    (println_any)((Any){&(p), 16});
    (println_type)(8);
    #line 555
    (println_type)(73);
    #line 556
    (println_type)(86);
    #line 557
    (println_type)(25);
    (println_typeinfo)(8);
    #line 560
    (println_typeinfo)(25);
    #line 561
    (println_typeinfo)(87);
    #line 562
    (println_typeinfo)(26);
}

#line 571
void test_compound_literals(void) {
    #line 572
    int (a[3]) = {1, 2, 3};
    #line 573
    int i = 42;
    #line 574
    const Any (x) = {&(i), 8};
    #line 575
    Any y = {&(i), 8};
    #line 576
    Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 581
    Ints (ints_of_ints[2]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

void test_complete(void) {
    #line 588
    int x = 0;
    #line 591
    int y = 0;
    if ((x) == (0)) {
        #line 594
        (y) = 1;
    } else if ((x) == (1)) {
        #line 596
        (y) = 2;
    } else {
        #line 592
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 599
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 607
    switch (x) {
        case 0: {
            #line 609
            (y) = 3;
            break;
        }
        case 1: {
            #line 611
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 615
void test_alignof(void) {
    #line 616
    int i = 42;
    #line 617
    ullong n1 = alignof(int);
    #line 618
    ullong n2 = alignof(int);
    #line 619
    ullong n3 = alignof(ullong);
    #line 620
    ullong n4 = alignof(int *);
}

#line 630
void test_offsetof(void) {
    #line 631
    ullong n = offsetof(BufHdr, buf);
}

Thing thing;

Thing * returns_ptr(void) {
    #line 642
    return &(thing);
}

#line 645
const Thing * returns_ptr_to_const(void) {
    #line 646
    return &(thing);
}

#line 649
void test_lvalue(void) {
    #line 650
    ((returns_ptr)()->a) = 5;
    const Thing (*p) = (returns_ptr_to_const)();
}

#line 656
int main(int argc, const char *(*argv)) {
    #line 657
    if ((argv) == (0)) {
        #line 658
        (printf)("argv is null\n");
    }
    #line 660
    (test_modify)();
    #line 661
    (test_lvalue)();
    #line 662
    (test_alignof)();
    #line 663
    (test_offsetof)();
    #line 664
    (test_complete)();
    #line 665
    (test_compound_literals)();
    #line 666
    (test_loops)();
    #line 667
    (test_sizeof)();
    #line 668
    (test_assign)();
    #line 669
    (test_enum)();
    #line 670
    (test_arrays)();
    #line 671
    (test_cast)();
    #line 672
    (test_init)();
    #line 673
    (test_lits)();
    #line 674
    (test_const)();
    #line 675
    (test_bool)();
    #line 676
    (test_ops)();
    #line 677
    (test_typeinfo)();
    #line 678
    (getchar)();
    #line 679
    return 0;
}
