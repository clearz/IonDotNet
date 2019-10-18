// Preamble
#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif
#if _MSC_VER >= 1900 || __STDC_VERSION__ >= 201112L
// Visual Studio 2015 supports enough C99/C11 features for us.
#else
#error "C11 support required or Visual Studio 2015 or later"
#endif

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
typedef ullong typeid;

#ifdef _MSC_VER
#define alignof(x) __alignof(x)
#else
#define alignof(x) __alignof__(x)
#endif

const char *ION_OS = "win32";
const char *ION_ARCH = "x64";

// Foreign header files
#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Forward declarations
typedef struct TypeInfo TypeInfo;
typedef struct test1_SomeIncompleteType test1_SomeIncompleteType;
typedef struct test1_UartCtrl test1_UartCtrl;
typedef union test1_IntOrPtr test1_IntOrPtr;
typedef struct test1_Vector test1_Vector;
typedef struct Any Any;
typedef struct test1_Thing test1_Thing;
typedef struct TypeFieldInfo TypeFieldInfo;
typedef struct test1_S1 test1_S1;
typedef struct test1_S2 test1_S2;
typedef struct test1_T test1_T;
typedef struct test1_ConstVector test1_ConstVector;
typedef struct test1_Ints test1_Ints;
typedef struct test1_BufHdr test1_BufHdr;

// Sorted declarations
#line 1 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
typedef int TypeKind;

#line 2
#define TYPE_NONE ((int)(0))

#line 3
#define TYPE_VOID ((int)((TYPE_NONE) + (1)))

#line 4
#define TYPE_BOOL ((int)((TYPE_VOID) + (1)))

#line 5
#define TYPE_CHAR ((int)((TYPE_BOOL) + (1)))

#line 6
#define TYPE_UCHAR ((int)((TYPE_CHAR) + (1)))

#line 7
#define TYPE_SCHAR ((int)((TYPE_UCHAR) + (1)))

#line 8
#define TYPE_SHORT ((int)((TYPE_SCHAR) + (1)))

#line 9
#define TYPE_USHORT ((int)((TYPE_SHORT) + (1)))

#line 10
#define TYPE_INT ((int)((TYPE_USHORT) + (1)))

#line 11
#define TYPE_UINT ((int)((TYPE_INT) + (1)))

#line 12
#define TYPE_LONG ((int)((TYPE_UINT) + (1)))

#line 13
#define TYPE_ULONG ((int)((TYPE_LONG) + (1)))

#line 14
#define TYPE_LLONG ((int)((TYPE_ULONG) + (1)))

#line 15
#define TYPE_ULLONG ((int)((TYPE_LLONG) + (1)))

#line 16
#define TYPE_FLOAT ((int)((TYPE_ULLONG) + (1)))

#line 17
#define TYPE_DOUBLE ((int)((TYPE_FLOAT) + (1)))

#line 18
#define TYPE_CONST ((int)((TYPE_DOUBLE) + (1)))

#line 19
#define TYPE_PTR ((int)((TYPE_CONST) + (1)))

#line 20
#define TYPE_ARRAY ((int)((TYPE_PTR) + (1)))

#line 21
#define TYPE_STRUCT ((int)((TYPE_ARRAY) + (1)))

#line 22
#define TYPE_UNION ((int)((TYPE_STRUCT) + (1)))

#line 23
#define TYPE_FUNC ((int)((TYPE_UNION) + (1)))

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
TypeKind typeid_kind(typeid type);

int typeid_index(typeid type);

usize typeid_size(typeid type);

const TypeInfo * get_typeinfo(typeid type);

#line 6 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
extern char (*test1_esc_test_str);

#line 8
extern int(*test1_some_array);

#line 10

extern test1_SomeIncompleteType(*test1_incomplete_ptr);

#line 25
#define test1_PI (3.14f)

#line 26
#define test1_PI2 ((test1_PI) + (test1_PI))

#line 28
#define test1_U8 ((uint8)(42))

#line 30
extern char test1_c;

#line 31
extern uchar test1_uc;

#line 32
extern schar test1_sc;

#line 34
#define test1_N ((((char)(42)) + (8)) != (0))

#line 160
uchar test1_h(void);

#line 36
typedef int (test1_A[(1) + ((2) * (sizeof((test1_h)())))]);

#line 38
extern char (*test1_code);

#line 56
void test1_test_packages(void);

void test1_test_modify(void);

#line 81
void test1_f10(int (a[3]));

void test1_test_arrays(void);

#line 92
void test1_test_loops(void);

#line 126
void test1_test_nonmodifiable(void);

#line 142
#define test1_UART_CTRL_REG ((uint *)(0x12345678))

#line 138
struct test1_UartCtrl {
    #line 139
    bool tx_enable;
    #line 139
    bool rx_enable;
};

#line 144
uint32 test1_pack(test1_UartCtrl ctrl);

test1_UartCtrl test1_unpack(uint32 word);

void test1_test_uart(void);

#line 187
typedef test1_IntOrPtr test1_U;

#line 193
union test1_IntOrPtr {
    #line 194
    int i;
    #line 195
    int(*p);
};

#line 170
int test1_g(test1_U u);

void test1_k(void(*vp), int(*ip));

#line 179
void test1_f1(void);

#line 184
void test1_f3(int (a[]));

#line 189
int test1_example_test(void);

#line 198
extern const char (test1_escape_to_char[256]);

#line 208
extern int (test1_a2[11]);

#line 211
int test1_is_even(int digit);

#line 227
extern int test1_i;

#line 229
struct test1_Vector {
    #line 230
    int x;
    #line 230
    int y;
};

#line 233
void test1_f2(test1_Vector v);

int test1_fact_iter(int n);

#line 245
int test1_fact_rec(int n);

#line 255
extern test1_T(*test1_p);

#line 253
#define test1_M ((1) + (sizeof(test1_p)))

#line 261
typedef int test1_Color;

#line 262
#define test1_COLOR_NONE ((int)(0))

#line 263
#define test1_COLOR_RED ((int)((test1_COLOR_NONE) + (1)))

#line 264
#define test1_COLOR_GREEN ((int)((test1_COLOR_RED) + (1)))

#line 265
#define test1_COLOR_BLUE ((int)((test1_COLOR_GREEN) + (1)))

#line 266
#define test1_NUM_COLORS ((int)((test1_COLOR_BLUE) + (1)))

#line 269
extern const char * (test1_color_names[test1_NUM_COLORS]);

#line 276
void test1_test_enum(void);

#line 285
void test1_test_assign(void);

#line 308
void test1_benchmark(int n);

#line 315
int test1_va_test(int x, ...);

typedef int (*test1_F)(int, ...);

#line 321
void test1_test_lits(void);

#line 338
void test1_test_ops(void);

#line 368
#define test1_IS_DEBUG (true)

#line 370
void test1_test_bool(void);

#line 377
int test1_test_ctrl(void);

#line 387
extern const int (test1_j);

#line 388
extern const int(*test1_q);

#line 389
extern const test1_Vector (test1_cv);

#line 391
void test1_f4(const char(*x));

#line 398
void test1_f5(const int(*p));

#line 401
void test1_test_convert(void);

#line 409
void test1_test_const(void);

#line 434
void test1_test_init(void);

#line 450
void test1_test_sizeof(void);

#line 458
void test1_test_cast(void);

#line 70 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
struct Any {
    #line 71
    void(*ptr);
    #line 72
    typeid type;
};

#line 467 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
void test1_print_any(Any any);

#line 480
void test1_println_any(Any any);

#line 485
void test1_print_typeid(typeid type);

#line 490
void test1_print_type(typeid type);

#line 515
void test1_println_type(typeid type);

#line 520
void test1_print_typeinfo(typeid type);

#line 544
void test1_println_typeinfo(typeid type);

#line 549
void test1_test_typeinfo(void);

#line 575
void test1_test_compound_literals(void);

#line 593
void test1_test_complete(void);

#line 621
void test1_test_alignof(void);

#line 636
void test1_test_offsetof(void);

#line 641
struct test1_Thing {
    #line 642
    int a;
};

#line 645
extern test1_Thing test1_thing;

#line 647
test1_Thing * test1_returns_ptr(void);

const test1_Thing * test1_returns_ptr_to_const(void);

void test1_test_lvalue(void);

#line 662
void test1_test_if(void);

#line 675
void test1_test_reachable(void);

int main(int argc, const char *(*argv));

#line 3 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
void test1_subtest1_bogus_func(void);

#line 8
void test1_subtest1_func3(void);

void test1_subtest1_func4(void);

#line 15
void test1_subtest1_func2(void);

#line 20
int test1_subtest1_func1(void);

#line 26 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
struct TypeFieldInfo {
    #line 27
    const char(*name);
    #line 28
    typeid type;
    #line 29
    int offset;
};

#line 47 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
struct test1_S1 {
    #line 48
    int a;
    #line 49
    const int (b);
};

#line 52
struct test1_S2 {
    #line 53
    test1_S1 s1;
};

#line 257
struct test1_T {
    #line 258
    int (a[test1_M]);
};

#line 394
struct test1_ConstVector {
    #line 395
    const int (x);
    #line 395
    const int (y);
};

#line 569
struct test1_Ints {
    #line 570
    int num_ints;
    #line 571
    int(*int_ptr);
    #line 572
    int (int_arr[3]);
};

#line 629
struct test1_BufHdr {
    #line 630
    usize cap;
    #line 630
    usize len;
    #line 631
    char (buf[1]);
};

// Typeinfo
#define TYPEID0(index, kind) ((ullong)(index) | ((kind) << 24ull))
#define TYPEID(index, kind, ...) ((ullong)(index) | (sizeof(__VA_ARGS__) << 32ull) | ((kind) << 24ull))

const TypeInfo *typeinfo_table[110] = {
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
    [16] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(1, TYPE_VOID)},
    [17] = &(TypeInfo){TYPE_CONST, .size = sizeof(const void *), .align = alignof(const void *), .base = TYPEID(16, TYPE_PTR, void *)},
    [18] = NULL, // Enum
    [19] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"name", .type = TYPEID(22, TYPE_PTR, const char *), .offset = offsetof(TypeFieldInfo, name)},
        {"type", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeFieldInfo, type)},
        {"offset", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeFieldInfo, offset)},}},
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
        {"kind", .type = TYPEID(18, TYPE_NONE, TypeKind), .offset = offsetof(TypeInfo, kind)},
        {"size", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, size)},
        {"align", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, align)},
        {"name", .type = TYPEID(22, TYPE_PTR, const char *), .offset = offsetof(TypeInfo, name)},
        {"count", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, count)},
        {"base", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, base)},
        {"fields", .type = TYPEID(23, TYPE_PTR, TypeFieldInfo *), .offset = offsetof(TypeInfo, fields)},
        {"num_fields", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, num_fields)},}},
    [21] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = TYPEID(3, TYPE_CHAR, char)},
    [22] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(21, TYPE_CONST, const char)},
    [23] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(19, TYPE_STRUCT, TypeFieldInfo)},
    [24] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeInfo), .align = alignof(const TypeInfo), .base = TYPEID(20, TYPE_STRUCT, TypeInfo)},
    [25] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(24, TYPE_CONST, const TypeInfo)},
    [26] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(25, TYPE_PTR, const TypeInfo *)},
    [27] = NULL, // Func
    [28] = NULL, // Func
    [29] = NULL, // Func
    [30] = NULL, // Func
    [31] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = TYPEID(16, TYPE_PTR, void *), .offset = offsetof(Any, ptr)},
        {"type", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(Any, type)},}},
    [32] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(3, TYPE_CHAR, char)},
    [33] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(8, TYPE_INT, int)},
    [34] = NULL, // Incomplete array type
    [35] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = TYPEID(8, TYPE_INT, int), .count = 3},
    [36] = NULL, // Incomplete: test1_SomeIncompleteType
    [37] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(36, TYPE_NONE)},
    [38] = NULL, // Func
    [39] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S1), .align = alignof(test1_S1), .name = "test1_S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_S1, a)},
        {"b", .type = TYPEID(63, TYPE_CONST, const int), .offset = offsetof(test1_S1, b)},}},
    [40] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S2), .align = alignof(test1_S2), .name = "test1_S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = TYPEID(39, TYPE_STRUCT, test1_S1), .offset = offsetof(test1_S2, s1)},}},
    [41] = NULL, // Func
    [42] = NULL, // Func
    [43] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_UartCtrl), .align = alignof(test1_UartCtrl), .name = "test1_UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, tx_enable)},
        {"rx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, rx_enable)},}},
    [44] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(9, TYPE_UINT, uint)},
    [45] = NULL, // Func
    [46] = NULL, // Func
    [47] = &(TypeInfo){TYPE_UNION, .size = sizeof(test1_IntOrPtr), .align = alignof(test1_IntOrPtr), .name = "test1_IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_IntOrPtr, i)},
        {"p", .type = TYPEID(33, TYPE_PTR, int *), .offset = offsetof(test1_IntOrPtr, p)},}},
    [48] = NULL, // Func
    [49] = NULL, // Func
    [50] = NULL, // Func
    [51] = NULL, // Func
    [52] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = TYPEID(21, TYPE_CONST, const char), .count = 256},
    [53] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = TYPEID(8, TYPE_INT, int), .count = 11},
    [54] = NULL, // Func
    [55] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Vector), .align = alignof(test1_Vector), .name = "test1_Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, x)},
        {"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, y)},}},
    [56] = NULL, // Func
    [57] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_T), .align = alignof(test1_T), .name = "test1_T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(94, TYPE_ARRAY, int [9]), .offset = offsetof(test1_T, a)},}},
    [58] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(57, TYPE_STRUCT, test1_T)},
    [59] = NULL, // Enum
    [60] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = TYPEID(22, TYPE_PTR, const char *), .count = 4},
    [61] = NULL, // Func
    [62] = NULL, // Func
    [63] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = TYPEID(8, TYPE_INT, int)},
    [64] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(63, TYPE_CONST, const int)},
    [65] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Vector), .align = alignof(const test1_Vector), .base = TYPEID(55, TYPE_STRUCT, test1_Vector)},
    [66] = NULL, // Func
    [67] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_ConstVector), .align = alignof(test1_ConstVector), .name = "test1_ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(63, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, x)},
        {"y", .type = TYPEID(63, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, y)},}},
    [68] = NULL, // Func
    [69] = NULL, // Func
    [70] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Ints), .align = alignof(test1_Ints), .name = "test1_Ints", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"num_ints", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Ints, num_ints)},
        {"int_ptr", .type = TYPEID(33, TYPE_PTR, int *), .offset = offsetof(test1_Ints, int_ptr)},
        {"int_arr", .type = TYPEID(35, TYPE_ARRAY, int [3]), .offset = offsetof(test1_Ints, int_arr)},}},
    [71] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_BufHdr), .align = alignof(test1_BufHdr), .name = "test1_BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, cap)},
        {"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, len)},
        {"buf", .type = TYPEID(109, TYPE_ARRAY, char [1]), .offset = offsetof(test1_BufHdr, buf)},}},
    [72] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Thing), .align = alignof(test1_Thing), .name = "test1_Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Thing, a)},}},
    [73] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(72, TYPE_STRUCT, test1_Thing)},
    [74] = NULL, // Func
    [75] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Thing), .align = alignof(const test1_Thing), .base = TYPEID(72, TYPE_STRUCT, test1_Thing)},
    [76] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(75, TYPE_CONST, const test1_Thing)},
    [77] = NULL, // Func
    [78] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(22, TYPE_PTR, const char *)},
    [79] = NULL, // Func
    [80] = NULL, // Func
    [81] = NULL, // Func
    [82] = NULL, // Func
    [83] = NULL, // Func
    [84] = NULL, // Func
    [85] = NULL, // Func
    [86] = NULL, // Func
    [87] = NULL, // Func
    [88] = NULL, // Func
    [89] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = TYPEID0(1, TYPE_VOID)},
    [90] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(89, TYPE_CONST)},
    [91] = NULL, // Func
    [92] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(55, TYPE_STRUCT, test1_Vector)},
    [93] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = TYPEID(8, TYPE_INT, int), .count = 16},
    [94] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = TYPEID(8, TYPE_INT, int), .count = 9},
    [95] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = TYPEID(8, TYPE_INT, int), .count = 4},
    [96] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = TYPEID(14, TYPE_FLOAT, float)},
    [97] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(96, TYPE_CONST, const float)},
    [98] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = TYPEID(18, TYPE_NONE, TypeKind)},
    [99] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = TYPEID(22, TYPE_PTR, const char *)},
    [100] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = TYPEID(23, TYPE_PTR, TypeFieldInfo *)},
    [101] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(14, TYPE_FLOAT, float)},
    [102] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(16, TYPE_PTR, void *)},
    [103] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = TYPEID(64, TYPE_PTR, const int *), .count = 42},
    [104] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(47, TYPE_UNION, test1_IntOrPtr)},
    [105] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = TYPEID(31, TYPE_STRUCT, Any)},
    [106] = NULL, // Incomplete array type
    [107] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [2]), .align = alignof(int [2]), .base = TYPEID(8, TYPE_INT, int), .count = 2},
    [108] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(test1_Ints [2]), .align = alignof(test1_Ints [2]), .base = TYPEID(70, TYPE_STRUCT, test1_Ints), .count = 2},
    [109] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1},
};
int num_typeinfos = 110;
const TypeInfo **typeinfos = (const TypeInfo **)typeinfo_table;

// Definitions
#line 49 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
TypeKind typeid_kind(typeid type) {
    #line 50
    return (TypeKind)((((type) >> (24))) & (0xFF));
}

#line 53
int typeid_index(typeid type) {
    #line 54
    return (int)((type) & (0xFFFFFF));
}

#line 57
usize typeid_size(typeid type) {
    #line 58
    return (usize)((type) >> (32));
}

#line 61
const TypeInfo * get_typeinfo(typeid type) {
    #line 62
    int index = (typeid_index)(type);
    #line 63
    if ((typeinfos) && ((index) < (num_typeinfos))) {
        #line 64
        return typeinfos[index];
    } else {
        #line 66
        return NULL;
    }
}

char (*test1_esc_test_str) = "Hello\nworld\nHex: \xFHello\xFF";

int(*test1_some_array) = (int []){1, 2, 3};

test1_SomeIncompleteType(*test1_incomplete_ptr);

char test1_c = 1;

uchar test1_uc = 1;

schar test1_sc = 1;

#line 160 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
uchar test1_h(void) {
    #line 161
    ((test1_Vector){.x = 1, .y = 2}.x) = 42;
    #line 162
    test1_Vector (*v) = &((test1_Vector){1, 2});
    #line 163
    (v->x) = 42;
    #line 164
    int (*p) = &((int){0});
    #line 165
    ulong x = ((uint){1}) + ((long){2});
    #line 166
    int y = +(test1_c);
    #line 167
    return (uchar)(x);
}

char (*test1_code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 56
void test1_test_packages(void) {
    #line 57
    (test1_subtest1_func1)();
}

#line 60
void test1_test_modify(void) {
    #line 61
    int i = 42;
    #line 62
    #line 63
    int (*p) = &(i);
    #line 64
    #line 65
    (p)--;
    #line 66
    int x = *((p)++);
    #line 67
    assert((x) == (*(--(p))));
    #line 68
    ((*(p)))++;
    #line 69
    ((*(p)))--;
    #line 70
    int (stk[16]) = {0};
    #line 71
    int(*sp) = stk;
    #line 72
    (*((sp)++)) = 1;
    #line 73
    (*((sp)++)) = 2;
    #line 74
    (x) = *(--(sp));
    #line 75
    assert((x) == (2));
    #line 76
    (x) = *(--(sp));
    #line 77
    assert((x) == (1));
    #line 78
    assert((sp) == (stk));
}

#line 81
void test1_f10(int (a[3])) {
    #line 82
    (a[1]) = 42;
}

#line 85
void test1_test_arrays(void) {
    #line 86
    int (a[3]) = {1, 2, 3};
    (test1_f10)(a);
    #line 89
    int (*b) = a;
}

#line 92
void test1_test_loops(void) {
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
void test1_test_nonmodifiable(void) {
    #line 127
    test1_S1 s1 = {0};
    #line 128
    (s1.a) = 0;
    #line 131
    test1_S2 s2 = {0};
    #line 132
    (s2.s1.a) = 0;
}

#line 144
uint32 test1_pack(test1_UartCtrl ctrl) {
    #line 145
    return (((ctrl.tx_enable) & (1))) | (((((ctrl.rx_enable) & (1))) << (1)));
}

#line 148
test1_UartCtrl test1_unpack(uint32 word) {
    #line 149
    return (test1_UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = (((word) & (0x2))) >> (1)};
}

#line 152
void test1_test_uart(void) {
    #line 153
    bool tx_enable = (test1_unpack)(*(test1_UART_CTRL_REG)).tx_enable;
    #line 154
    (*(test1_UART_CTRL_REG)) = (test1_pack)((test1_UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 155
    test1_UartCtrl ctrl = (test1_unpack)(*(test1_UART_CTRL_REG));
    #line 156
    (ctrl.rx_enable) = true;
    #line 157
    (*(test1_UART_CTRL_REG)) = (test1_pack)(ctrl);
}

#line 170
int test1_g(test1_U u) {
    #line 171
    return u.i;
}

#line 174
void test1_k(void(*vp), int(*ip)) {
    #line 175
    (vp) = ip;
    #line 176
    (ip) = vp;
}

#line 179
void test1_f1(void) {
    #line 180
    int (*p) = &((int){0});
    #line 181
    (*(p)) = 42;
}

#line 184
void test1_f3(int (a[])) {
}

#line 189
int test1_example_test(void) {
    #line 190
    return ((test1_fact_rec)(10)) == ((test1_fact_iter)(10));
}

const char (test1_escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (test1_a2[11]) = {1, 2, 3, [10] = 4};

#line 211
int test1_is_even(int digit) {
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

int test1_i;

#line 233
void test1_f2(test1_Vector v) {
    #line 234
    (v) = (test1_Vector){0};
}

#line 237
int test1_fact_iter(int n) {
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
int test1_fact_rec(int n) {
    #line 246
    if ((n) == (0)) {
        #line 247
        return 1;
    } else {
        #line 249
        return (n) * ((test1_fact_rec)((n) - (1)));
    }
}

test1_T(*test1_p);

const char * (test1_color_names[test1_NUM_COLORS]) = {[test1_COLOR_NONE] = "none", [test1_COLOR_RED] = "red", [test1_COLOR_GREEN] = "green", [test1_COLOR_BLUE] = "blue"};

#line 276
void test1_test_enum(void) {
    #line 277
    test1_Color a = test1_COLOR_RED;
    #line 278
    int b = test1_COLOR_RED;
    #line 279
    int c = (a) + (b);
    #line 280
    int i = a;
    #line 281
    (a) = i;
    #line 282
    (printf)("%d %d %d %d\n", test1_COLOR_NONE, test1_COLOR_RED, test1_COLOR_GREEN, test1_COLOR_BLUE);
}

#line 285
void test1_test_assign(void) {
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
void test1_benchmark(int n) {
    #line 309
    int r = 1;
    #line 310
    for (int i = 1; (i) <= (n); (i)++) {
        #line 311
        (r) *= i;
    }
}

#line 315
int test1_va_test(int x, ...) {
    #line 316
    return 0;
}

#line 321
void test1_test_lits(void) {
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
    int x1 = 0xFFFFFFFF;
    #line 331
    int x2 = 4294967295;
    #line 332
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 333
    int x4 = (0xAA) + (0x55);
}

#line 338
void test1_test_ops(void) {
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
    (n) = (int)((((p) + (1))) - (p));
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
    (b) = (((n) & (~(1)))) ^ (1);
    #line 365
    (b) = (p) && (pi);
}

#line 370
void test1_test_bool(void) {
    #line 371
    bool b = false;
    #line 372
    (b) = true;
    #line 373
    int i = 0;
    #line 374
    (i) = test1_IS_DEBUG;
}

#line 377
int test1_test_ctrl(void) {
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

const int (test1_j);

const int(*test1_q);

const test1_Vector (test1_cv);

#line 391
void test1_f4(const char(*x)) {
}

#line 398
void test1_f5(const int(*p)) {
}

#line 401
void test1_test_convert(void) {
    #line 402
    const int(*a) = 0;
    #line 403
    int(*b) = 0;
    #line 404
    (a) = b;
    #line 405
    void(*p) = 0;
    #line 406
    (test1_f5)(p);
}

#line 409
void test1_test_const(void) {
    #line 410
    test1_ConstVector cv2 = {1, 2};
    int i = 0;
    #line 413
    (i) = 1;
    #line 416
    int x = test1_cv.x;
    char c = test1_escape_to_char[0];
    (test1_f4)(test1_escape_to_char);
    #line 421
    const char (*p) = (const char *)(0);
    #line 422
    (p) = (test1_escape_to_char) + (1);
    #line 423
    char (*q) = (char *)(test1_escape_to_char);
    #line 424
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 429
    (i) = (int)((ullong)(p));
}

#line 434
void test1_test_init(void) {
    #line 435
    int x = (const int)(0);
    #line 436
    #line 437
    int y = {0};
    #line 438
    (y) = 0;
    #line 439
    int z = 42;
    #line 440
    int (a[3]) = {1, 2, 3};
    #line 443
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 444
        (printf)("%llu\n", i);
    }
    #line 446
    int (b[4]) = {1, 2, 3, 4};
    #line 447
    (b[0]) = a[2];
}

#line 450
void test1_test_sizeof(void) {
    #line 451
    int i = 0;
    #line 452
    ullong n = sizeof(i);
    #line 453
    (n) = sizeof(int);
    #line 454
    (n) = sizeof(int);
    #line 455
    (n) = sizeof(int *);
}

#line 458
void test1_test_cast(void) {
    #line 459
    int(*p) = 0;
    #line 460
    uint64 a = 0;
    (a) = (uint64)(p);
    (p) = (int *)(a);
}

#line 467
void test1_print_any(Any any) {
    #line 468
    switch (any.type) {
        case TYPEID(8, TYPE_INT, int): {
            #line 470
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case TYPEID(14, TYPE_FLOAT, float): {
            #line 472
            (printf)("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 474
            (printf)("<unknown>");
            break;
        }
    }
    #line 476
    (printf)(": ");
    #line 477
    (test1_print_type)(any.type);
}

#line 480
void test1_println_any(Any any) {
    #line 481
    (test1_print_any)(any);
    #line 482
    (printf)("\n");
}

#line 485
void test1_print_typeid(typeid type) {
    #line 486
    int index = (typeid_index)(type);
    #line 487
    (printf)("typeid(%d)", index);
}

#line 490
void test1_print_type(typeid type) {
    #line 491
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 492
    if (!(typeinfo)) {
        #line 493
        (test1_print_typeid)(type);
        #line 494
        return;
    }
    #line 496
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 498
            (test1_print_type)(typeinfo->base);
            #line 499
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 501
            (test1_print_type)(typeinfo->base);
            #line 502
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 504
            (test1_print_type)(typeinfo->base);
            #line 505
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 507
            if (typeinfo->name) {
                #line 508
                (printf)("%s", typeinfo->name);
            } else {
                #line 510
                (test1_print_typeid)(type);
            }
            break;
        }
    }
}

#line 515
void test1_println_type(typeid type) {
    #line 516
    (test1_print_type)(type);
    #line 517
    (printf)("\n");
}

#line 520
void test1_print_typeinfo(typeid type) {
    #line 521
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 522
    if (!(typeinfo)) {
        #line 523
        (test1_print_typeid)(type);
        #line 524
        return;
    }
    #line 526
    (printf)("<");
    #line 527
    (test1_print_type)(type);
    #line 528
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 529
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 532
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 533
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 534
                TypeFieldInfo field = typeinfo->fields[i];
                #line 535
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 536
                (test1_print_type)(field.type);
                #line 537
                (printf)("; ");
            }
            #line 539
            (printf)("}");
            break;
        }
    }
    #line 541
    (printf)(">");
}

#line 544
void test1_println_typeinfo(typeid type) {
    #line 545
    (test1_print_typeinfo)(type);
    #line 546
    (printf)("\n");
}

#line 549
void test1_test_typeinfo(void) {
    #line 550
    int i = 42;
    #line 551
    float f = 3.14f;
    #line 552
    void (*p) = NULL;
    (test1_println_any)((Any){&(i), TYPEID(8, TYPE_INT, int)});
    #line 555
    (test1_println_any)((Any){&(f), TYPEID(14, TYPE_FLOAT, float)});
    #line 556
    (test1_println_any)((Any){&(p), TYPEID(16, TYPE_PTR, void *)});
    (test1_println_type)(TYPEID(8, TYPE_INT, int));
    #line 559
    (test1_println_type)(TYPEID(64, TYPE_PTR, const int *));
    #line 560
    (test1_println_type)(TYPEID(103, TYPE_ARRAY, const int * [42]));
    #line 561
    (test1_println_type)(TYPEID(43, TYPE_STRUCT, test1_UartCtrl));
    (test1_println_typeinfo)(TYPEID(8, TYPE_INT, int));
    #line 564
    (test1_println_typeinfo)(TYPEID(43, TYPE_STRUCT, test1_UartCtrl));
    #line 565
    (test1_println_typeinfo)(TYPEID(104, TYPE_PTR, test1_IntOrPtr *));
    #line 566
    (test1_println_typeinfo)(TYPEID(47, TYPE_UNION, test1_IntOrPtr));
}

#line 575
void test1_test_compound_literals(void) {
    #line 576
    test1_Vector(*w) = {0};
    #line 577
    (w) = &((test1_Vector){1, 2});
    #line 578
    int (a[3]) = {1, 2, 3};
    #line 579
    int i = 42;
    #line 580
    const Any (x) = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 581
    Any y = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 582
    test1_Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 587
    test1_Ints (ints_of_ints[2]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

void test1_test_complete(void) {
    #line 594
    int x = 0;
    #line 597
    int y = 0;
    if ((x) == (0)) {
        #line 600
        (y) = 1;
    } else if ((x) == (1)) {
        #line 602
        (y) = 2;
    } else {
        #line 598
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 605
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 613
    switch (x) {
        case 0: {
            #line 615
            (y) = 3;
            break;
        }
        case 1: {
            #line 617
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 621
void test1_test_alignof(void) {
    #line 622
    int i = 42;
    #line 623
    ullong n1 = alignof(int);
    #line 624
    ullong n2 = alignof(int);
    #line 625
    ullong n3 = alignof(ullong);
    #line 626
    ullong n4 = alignof(int *);
}

#line 636
void test1_test_offsetof(void) {
    #line 637
    ullong n = offsetof(test1_BufHdr, buf);
}

test1_Thing test1_thing;

test1_Thing * test1_returns_ptr(void) {
    #line 648
    return &(test1_thing);
}

#line 651
const test1_Thing * test1_returns_ptr_to_const(void) {
    #line 652
    return &(test1_thing);
}

#line 655
void test1_test_lvalue(void) {
    #line 656
    ((test1_returns_ptr)()->a) = 5;
    const test1_Thing (*p) = (test1_returns_ptr_to_const)();
}

#line 662
void test1_test_if(void) {
    #line 663
    if (1) {
    }
    {
        #line 667
        int x = 42;
        if (x) {
        }
    }
    #line 669
    {
        #line 669
        int x = 42;
        if ((x) >= (0)) {
        }
    }
    #line 671
    {
        #line 671
        int x = 42;
        if ((x) >= (0)) {
        }
    }
}

#line 675
void test1_test_reachable(void) {
}

#line 679
int main(int argc, const char *(*argv)) {
    #line 680
    if ((argv) == (0)) {
        #line 681
        (printf)("argv is null\n");
    }
    #line 683
    (test1_test_packages)();
    #line 684
    (test1_test_if)();
    #line 685
    (test1_test_modify)();
    #line 686
    (test1_test_lvalue)();
    #line 687
    (test1_test_alignof)();
    #line 688
    (test1_test_offsetof)();
    #line 689
    (test1_test_complete)();
    #line 690
    (test1_test_compound_literals)();
    #line 691
    (test1_test_loops)();
    #line 692
    (test1_test_sizeof)();
    #line 693
    (test1_test_assign)();
    #line 694
    (test1_test_enum)();
    #line 695
    (test1_test_arrays)();
    #line 696
    (test1_test_cast)();
    #line 697
    (test1_test_init)();
    #line 698
    (test1_test_lits)();
    #line 699
    (test1_test_const)();
    #line 700
    (test1_test_bool)();
    #line 701
    (test1_test_ops)();
    #line 702
    (test1_test_typeinfo)();
    #line 703
    (test1_test_reachable)();
    #line 704
    (getchar)();
    #line 705
    return 0;
}

#line 3 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
void test1_subtest1_bogus_func(void) {
}

#line 8
void test1_subtest1_func3(void) {
    #line 9
    (printf)("func3\n");
}

#line 12
void test1_subtest1_func4(void) {
}

#line 15
void test1_subtest1_func2(void) {
    #line 16
    (test1_subtest1_func3)();
    #line 17
    (test1_subtest1_func4)();
}

#line 20
int test1_subtest1_func1(void) {
    #line 21
    (test1_subtest1_func2)();
    #line 22
    return 42;
}

// Foreign source files
