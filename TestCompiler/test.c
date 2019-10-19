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

#ifdef _MSC_VER
#define alignof(x) __alignof(x)
#else
#define alignof(x) __alignof__(x)
#endif

// Foreign header files
#include <ctype.h>
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
#line 1 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/config_win32.ion"
extern const char (*IONOS);

#line 1 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/config_x64.ion"
extern const char (*IONARCH);

#line 1 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
typedef ullong typeid;

#line 3
typedef int TypeKind;

#line 4
#define TYPE_NONE ((int)(0))

#line 5
#define TYPE_VOID ((int)((TYPE_NONE) + (1)))

#line 6
#define TYPE_BOOL ((int)((TYPE_VOID) + (1)))

#line 7
#define TYPE_CHAR ((int)((TYPE_BOOL) + (1)))

#line 8
#define TYPE_UCHAR ((int)((TYPE_CHAR) + (1)))

#line 9
#define TYPE_SCHAR ((int)((TYPE_UCHAR) + (1)))

#line 10
#define TYPE_SHORT ((int)((TYPE_SCHAR) + (1)))

#line 11
#define TYPE_USHORT ((int)((TYPE_SHORT) + (1)))

#line 12
#define TYPE_INT ((int)((TYPE_USHORT) + (1)))

#line 13
#define TYPE_UINT ((int)((TYPE_INT) + (1)))

#line 14
#define TYPE_LONG ((int)((TYPE_UINT) + (1)))

#line 15
#define TYPE_ULONG ((int)((TYPE_LONG) + (1)))

#line 16
#define TYPE_LLONG ((int)((TYPE_ULONG) + (1)))

#line 17
#define TYPE_ULLONG ((int)((TYPE_LLONG) + (1)))

#line 18
#define TYPE_FLOAT ((int)((TYPE_ULLONG) + (1)))

#line 19
#define TYPE_DOUBLE ((int)((TYPE_FLOAT) + (1)))

#line 20
#define TYPE_CONST ((int)((TYPE_DOUBLE) + (1)))

#line 21
#define TYPE_PTR ((int)((TYPE_CONST) + (1)))

#line 22
#define TYPE_ARRAY ((int)((TYPE_PTR) + (1)))

#line 23
#define TYPE_STRUCT ((int)((TYPE_ARRAY) + (1)))

#line 24
#define TYPE_UNION ((int)((TYPE_STRUCT) + (1)))

#line 25
#define TYPE_FUNC ((int)((TYPE_UNION) + (1)))

#line 34
struct TypeInfo {
    #line 35
    TypeKind kind;
    #line 36
    int size;
    #line 37
    int align;
    #line 38
    const char (*name);
    #line 39
    int count;
    #line 40
    typeid base;
    #line 41
    TypeFieldInfo (*fields);
    #line 42
    int num_fields;
};

#line 51
TypeKind typeid_kind(typeid type);

int typeid_index(typeid type);

size_t typeid_size(typeid type);

const TypeInfo * get_typeinfo(typeid type);

#line 6 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
extern char (*test1_esc_test_str);

#line 8
extern int (*test1_some_array);

#line 10

extern test1_SomeIncompleteType (*test1_incomplete_ptr);

#line 25
#define test1_PI (3.14f)

#line 26
#define test1_PI2 ((test1_PI) + (test1_PI))

#line 28
#define test1_U8 ((uint8_t)(42))

#line 30
extern char test1_c;

#line 31
extern uchar test1_uc;

#line 32
extern schar test1_sc;

#line 34
typedef void (*test1_F1)(void);

#line 35
typedef int (*test1_F2)(int (*)(int, int));

#line 36
typedef void (*test1_F3)(void (*)(void));

#line 38
#define test1_N ((((char)(42)) + (8)) != (0))

#line 164
uchar test1_h(void);

#line 40
typedef int (test1_A[(1) + ((2) * (sizeof((test1_h)())))]);

#line 42
extern char (*test1_code);

#line 60
void test1_test_packages(void);

void test1_test_modify(void);

#line 85
void test1_f10(int (a[3]));

void test1_test_arrays(void);

#line 96
void test1_test_loops(void);

#line 130
void test1_test_nonmodifiable(void);

#line 146
#define test1_UART_CTRL_REG ((uint *)(0x12345678))

#line 142
struct test1_UartCtrl {
    #line 143
    bool tx_enable;
    #line 143
    bool rx_enable;
};

#line 148
uint32_t test1_pack(test1_UartCtrl ctrl);

test1_UartCtrl test1_unpack(uint32_t word);

void test1_test_uart(void);

#line 191
typedef test1_IntOrPtr test1_U;

#line 197
union test1_IntOrPtr {
    #line 198
    int i;
    #line 199
    int (*p);
};

#line 174
int test1_g(test1_U u);

void test1_k(void (*vp), int (*ip));

#line 183
void test1_f1(void);

#line 188
void test1_f3(int (a[]));

#line 193
int test1_example_test(void);

#line 202
extern const char (test1_escape_to_char[256]);

#line 212
extern int (test1_a2[11]);

#line 215
int test1_is_even(int digit);

#line 231
extern int test1_i;

#line 233
struct test1_Vector {
    #line 234
    int x;
    #line 234
    int y;
};

#line 237
void test1_f2(test1_Vector v);

int test1_fact_iter(int n);

#line 249
int test1_fact_rec(int n);

#line 259
extern test1_T (*test1_p);

#line 257
#define test1_M ((1) + (sizeof(test1_p)))

#line 265
typedef int test1_Color;

#line 266
#define test1_COLOR_NONE ((int)(0))

#line 267
#define test1_COLOR_RED ((int)((test1_COLOR_NONE) + (1)))

#line 268
#define test1_COLOR_GREEN ((int)((test1_COLOR_RED) + (1)))

#line 269
#define test1_COLOR_BLUE ((int)((test1_COLOR_GREEN) + (1)))

#line 270
#define test1_NUM_COLORS ((int)((test1_COLOR_BLUE) + (1)))

#line 273
extern const char * (test1_color_names[test1_NUM_COLORS]);

#line 280
void test1_test_enum(void);

#line 289
void test1_test_assign(void);

#line 312
void test1_benchmark(int n);

#line 319
int test1_va_test(int x, ...);

typedef int (*test1_F)(int, ...);

#line 325
void test1_test_lits(void);

#line 342
void test1_test_ops(void);

#line 372
#define test1_IS_DEBUG (true)

#line 374
void test1_test_bool(void);

#line 381
int test1_test_ctrl(void);

#line 391
extern const int (test1_j);

#line 392
extern const int (*test1_q);

#line 393
extern const test1_Vector (test1_cv);

#line 395
void test1_f4(const char (*x));

#line 402
void test1_f5(const int (*p));

#line 405
void test1_test_convert(void);

#line 413
void test1_test_const(void);

#line 438
void test1_test_init(void);

#line 454
void test1_test_sizeof(void);

#line 462
void test1_test_cast(void);

#line 72 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
struct Any {
    #line 73
    void (*ptr);
    #line 74
    typeid type;
};

#line 471 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
void test1_print_any(Any any);

#line 484
void test1_println_any(Any any);

#line 489
void test1_print_typeid(typeid type);

#line 494
void test1_print_type(typeid type);

#line 519
void test1_println_type(typeid type);

#line 524
void test1_print_typeinfo(typeid type);

#line 548
void test1_println_typeinfo(typeid type);

#line 553
void test1_test_typeinfo(void);

#line 579
void test1_test_compound_literals(void);

#line 597
void test1_test_complete(void);

#line 625
void test1_test_alignof(void);

#line 640
void test1_test_offsetof(void);

#line 645
struct test1_Thing {
    #line 646
    int a;
};

#line 649
extern test1_Thing test1_thing;

#line 651
test1_Thing * test1_returns_ptr(void);

const test1_Thing * test1_returns_ptr_to_const(void);

void test1_test_lvalue(void);

#line 666
void test1_test_if(void);

#line 679
void test1_test_reachable(void);

void test1_test_os_arch(void);

#line 688
int main(int argc, const char * (*argv));

#line 28 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
struct TypeFieldInfo {
    #line 29
    const char (*name);
    #line 30
    typeid type;
    #line 31
    int offset;
};

#line 51 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
struct test1_S1 {
    #line 52
    int a;
    #line 53
    const int (b);
};

#line 56
struct test1_S2 {
    #line 57
    test1_S1 s1;
};

#line 20 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
int test1_subtest1_func1(void);

#line 261 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
struct test1_T {
    #line 262
    int (a[test1_M]);
};

#line 398
struct test1_ConstVector {
    #line 399
    const int (x);
    #line 399
    const int (y);
};

#line 573
struct test1_Ints {
    #line 574
    int num_ints;
    #line 575
    int (*int_ptr);
    #line 576
    int (int_arr[3]);
};

#line 633
struct test1_BufHdr {
    #line 634
    size_t cap;
    #line 634
    size_t len;
    #line 635
    char (buf[1]);
};

#line 15 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
void test1_subtest1_func2(void);

#line 8
void test1_subtest1_func3(void);

void test1_subtest1_func4(void);

// Typeinfo
#define TYPEID0(index, kind) ((ullong)(index) | ((kind) << 24ull))
#define TYPEID(index, kind, ...) ((ullong)(index) | (sizeof(__VA_ARGS__) << 32ull) | ((kind) << 24ull))

const TypeInfo *typeinfo_table[103] = {
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
    [16] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = TYPEID(3, TYPE_CHAR, char)},
    [17] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(16, TYPE_CONST, const char)},
    [18] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(3, TYPE_CHAR, char)},
    [19] = NULL, // Enum
    [20] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"name", .type = TYPEID(17, TYPE_PTR, const char *), .offset = offsetof(TypeFieldInfo, name)},
        {"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeFieldInfo, type)},
        {"offset", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeFieldInfo, offset)},}},
    [21] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
        {"kind", .type = TYPEID(19, TYPE_NONE, TypeKind), .offset = offsetof(TypeInfo, kind)},
        {"size", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, size)},
        {"align", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, align)},
        {"name", .type = TYPEID(17, TYPE_PTR, const char *), .offset = offsetof(TypeInfo, name)},
        {"count", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, count)},
        {"base", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeInfo, base)},
        {"fields", .type = TYPEID(22, TYPE_PTR, TypeFieldInfo *), .offset = offsetof(TypeInfo, fields)},
        {"num_fields", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, num_fields)},}},
    [22] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(20, TYPE_STRUCT, TypeFieldInfo)},
    [23] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeInfo), .align = alignof(const TypeInfo), .base = TYPEID(21, TYPE_STRUCT, TypeInfo)},
    [24] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(23, TYPE_CONST, const TypeInfo)},
    [25] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(24, TYPE_PTR, const TypeInfo *)},
    [26] = NULL, // Func
    [27] = NULL, // Func
    [28] = NULL, // Func
    [29] = NULL, // Func
    [30] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"ptr", .type = TYPEID(31, TYPE_PTR, void *), .offset = offsetof(Any, ptr)},
        {"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(Any, type)},}},
    [31] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(1, TYPE_VOID)},
    [32] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(8, TYPE_INT, int)},
    [33] = NULL, // Incomplete array type
    [34] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = TYPEID(8, TYPE_INT, int), .count = 3},
    [35] = NULL, // Incomplete: test1_SomeIncompleteType
    [36] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(35, TYPE_NONE)},
    [37] = NULL, // Func
    [38] = NULL, // Func
    [39] = NULL, // Func
    [40] = NULL, // Func
    [41] = NULL, // Func
    [42] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S1), .align = alignof(test1_S1), .name = "test1_S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_S1, a)},
        {"b", .type = TYPEID(65, TYPE_CONST, const int), .offset = offsetof(test1_S1, b)},}},
    [43] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S2), .align = alignof(test1_S2), .name = "test1_S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = TYPEID(42, TYPE_STRUCT, test1_S1), .offset = offsetof(test1_S2, s1)},}},
    [44] = NULL, // Func
    [45] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_UartCtrl), .align = alignof(test1_UartCtrl), .name = "test1_UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, tx_enable)},
        {"rx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, rx_enable)},}},
    [46] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(9, TYPE_UINT, uint)},
    [47] = NULL, // Func
    [48] = NULL, // Func
    [49] = &(TypeInfo){TYPE_UNION, .size = sizeof(test1_IntOrPtr), .align = alignof(test1_IntOrPtr), .name = "test1_IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_IntOrPtr, i)},
        {"p", .type = TYPEID(32, TYPE_PTR, int *), .offset = offsetof(test1_IntOrPtr, p)},}},
    [50] = NULL, // Func
    [51] = NULL, // Func
    [52] = NULL, // Func
    [53] = NULL, // Func
    [54] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = TYPEID(16, TYPE_CONST, const char), .count = 256},
    [55] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = TYPEID(8, TYPE_INT, int), .count = 11},
    [56] = NULL, // Func
    [57] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Vector), .align = alignof(test1_Vector), .name = "test1_Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, x)},
        {"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, y)},}},
    [58] = NULL, // Func
    [59] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_T), .align = alignof(test1_T), .name = "test1_T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(85, TYPE_ARRAY, int [9]), .offset = offsetof(test1_T, a)},}},
    [60] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(59, TYPE_STRUCT, test1_T)},
    [61] = NULL, // Enum
    [62] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = TYPEID(17, TYPE_PTR, const char *), .count = 4},
    [63] = NULL, // Func
    [64] = NULL, // Func
    [65] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = TYPEID(8, TYPE_INT, int)},
    [66] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(65, TYPE_CONST, const int)},
    [67] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Vector), .align = alignof(const test1_Vector), .base = TYPEID(57, TYPE_STRUCT, test1_Vector)},
    [68] = NULL, // Func
    [69] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_ConstVector), .align = alignof(test1_ConstVector), .name = "test1_ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(65, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, x)},
        {"y", .type = TYPEID(65, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, y)},}},
    [70] = NULL, // Func
    [71] = NULL, // Func
    [72] = NULL, // Func
    [73] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Ints), .align = alignof(test1_Ints), .name = "test1_Ints", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"num_ints", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Ints, num_ints)},
        {"int_ptr", .type = TYPEID(32, TYPE_PTR, int *), .offset = offsetof(test1_Ints, int_ptr)},
        {"int_arr", .type = TYPEID(34, TYPE_ARRAY, int [3]), .offset = offsetof(test1_Ints, int_arr)},}},
    [74] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_BufHdr), .align = alignof(test1_BufHdr), .name = "test1_BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, cap)},
        {"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, len)},
        {"buf", .type = TYPEID(102, TYPE_ARRAY, char [1]), .offset = offsetof(test1_BufHdr, buf)},}},
    [75] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Thing), .align = alignof(test1_Thing), .name = "test1_Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Thing, a)},}},
    [76] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(75, TYPE_STRUCT, test1_Thing)},
    [77] = NULL, // Func
    [78] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Thing), .align = alignof(const test1_Thing), .base = TYPEID(75, TYPE_STRUCT, test1_Thing)},
    [79] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(78, TYPE_CONST, const test1_Thing)},
    [80] = NULL, // Func
    [81] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(17, TYPE_PTR, const char *)},
    [82] = NULL, // Func
    [83] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(57, TYPE_STRUCT, test1_Vector)},
    [84] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = TYPEID(8, TYPE_INT, int), .count = 16},
    [85] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = TYPEID(8, TYPE_INT, int), .count = 9},
    [86] = NULL, // Func
    [87] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = TYPEID(8, TYPE_INT, int), .count = 4},
    [88] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = TYPEID(14, TYPE_FLOAT, float)},
    [89] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(88, TYPE_CONST, const float)},
    [90] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = TYPEID(19, TYPE_NONE, TypeKind)},
    [91] = &(TypeInfo){TYPE_CONST, .size = sizeof(const ullong), .align = alignof(const ullong), .base = TYPEID(13, TYPE_ULLONG, ullong)},
    [92] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = TYPEID(17, TYPE_PTR, const char *)},
    [93] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = TYPEID(22, TYPE_PTR, TypeFieldInfo *)},
    [94] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(14, TYPE_FLOAT, float)},
    [95] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(31, TYPE_PTR, void *)},
    [96] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = TYPEID(66, TYPE_PTR, const int *), .count = 42},
    [97] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(49, TYPE_UNION, test1_IntOrPtr)},
    [98] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = TYPEID(30, TYPE_STRUCT, Any)},
    [99] = NULL, // Incomplete array type
    [100] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [2]), .align = alignof(int [2]), .base = TYPEID(8, TYPE_INT, int), .count = 2},
    [101] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(test1_Ints [2]), .align = alignof(test1_Ints [2]), .base = TYPEID(73, TYPE_STRUCT, test1_Ints), .count = 2},
    [102] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1},
};
int num_typeinfos = 103;
const TypeInfo **typeinfos = (const TypeInfo **)typeinfo_table;

// Definitions
const char (*IONOS) = "win32";

const char (*IONARCH) = "x64";

#line 51 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
TypeKind typeid_kind(typeid type) {
    #line 52
    return (TypeKind)((((type) >> (24))) & (0xFF));
}

#line 55
int typeid_index(typeid type) {
    #line 56
    return (int)((type) & (0xFFFFFF));
}

#line 59
size_t typeid_size(typeid type) {
    #line 60
    return (size_t)((type) >> (32));
}

#line 63
const TypeInfo * get_typeinfo(typeid type) {
    #line 64
    int index = (typeid_index)(type);
    #line 65
    if ((typeinfos) && ((index) < (num_typeinfos))) {
        #line 66
        return typeinfos[index];
    } else {
        #line 68
        return NULL;
    }
}

char (*test1_esc_test_str) = "Hello\nworld\nHex: \xFHello\xFF";

int (*test1_some_array) = (int []){1, 2, 3};

test1_SomeIncompleteType (*test1_incomplete_ptr);

char test1_c = 1;

uchar test1_uc = 1;

schar test1_sc = 1;

#line 164 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
uchar test1_h(void) {
    #line 165
    ((test1_Vector){.x = 1, .y = 2}.x) = 42;
    #line 166
    test1_Vector (*v) = &((test1_Vector){1, 2});
    #line 167
    (v->x) = 42;
    #line 168
    int (*p) = &((int){0});
    #line 169
    ulong x = ((uint){1}) + ((long){2});
    #line 170
    int y = +(test1_c);
    #line 171
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

#line 60
void test1_test_packages(void) {
    #line 61
    (test1_subtest1_func1)();
}

#line 64
void test1_test_modify(void) {
    #line 65
    int i = 42;
    #line 66
    #line 67
    int (*p) = &(i);
    #line 68
    #line 69
    (p)--;
    #line 70
    int x = *((p)++);
    #line 71
    assert((x) == (*(--(p))));
    #line 72
    ((*(p)))++;
    #line 73
    ((*(p)))--;
    #line 74
    int (stk[16]) = {0};
    #line 75
    int (*sp) = stk;
    #line 76
    (*((sp)++)) = 1;
    #line 77
    (*((sp)++)) = 2;
    #line 78
    (x) = *(--(sp));
    #line 79
    assert((x) == (2));
    #line 80
    (x) = *(--(sp));
    #line 81
    assert((x) == (1));
    #line 82
    assert((sp) == (stk));
}

#line 85
void test1_f10(int (a[3])) {
    #line 86
    (a[1]) = 42;
}

#line 89
void test1_test_arrays(void) {
    #line 90
    int (a[3]) = {1, 2, 3};
    (test1_f10)(a);
    #line 93
    int (*b) = a;
}

#line 96
void test1_test_loops(void) {
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
void test1_test_nonmodifiable(void) {
    #line 131
    test1_S1 s1 = {0};
    #line 132
    (s1.a) = 0;
    #line 135
    test1_S2 s2 = {0};
    #line 136
    (s2.s1.a) = 0;
}

#line 148
uint32_t test1_pack(test1_UartCtrl ctrl) {
    #line 149
    return (((ctrl.tx_enable) & (1))) | (((((ctrl.rx_enable) & (1))) << (1)));
}

#line 152
test1_UartCtrl test1_unpack(uint32_t word) {
    #line 153
    return (test1_UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = (((word) & (0x2))) >> (1)};
}

#line 156
void test1_test_uart(void) {
    #line 157
    bool tx_enable = (test1_unpack)(*(test1_UART_CTRL_REG)).tx_enable;
    #line 158
    (*(test1_UART_CTRL_REG)) = (test1_pack)((test1_UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 159
    test1_UartCtrl ctrl = (test1_unpack)(*(test1_UART_CTRL_REG));
    #line 160
    (ctrl.rx_enable) = true;
    #line 161
    (*(test1_UART_CTRL_REG)) = (test1_pack)(ctrl);
}

#line 174
int test1_g(test1_U u) {
    #line 175
    return u.i;
}

#line 178
void test1_k(void (*vp), int (*ip)) {
    #line 179
    (vp) = ip;
    #line 180
    (ip) = vp;
}

#line 183
void test1_f1(void) {
    #line 184
    int (*p) = &((int){0});
    #line 185
    (*(p)) = 42;
}

#line 188
void test1_f3(int (a[])) {
}

#line 193
int test1_example_test(void) {
    #line 194
    return ((test1_fact_rec)(10)) == ((test1_fact_iter)(10));
}

const char (test1_escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (test1_a2[11]) = {1, 2, 3, [10] = 4};

#line 215
int test1_is_even(int digit) {
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

int test1_i;

#line 237
void test1_f2(test1_Vector v) {
    #line 238
    (v) = (test1_Vector){0};
}

#line 241
int test1_fact_iter(int n) {
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
int test1_fact_rec(int n) {
    #line 250
    if ((n) == (0)) {
        #line 251
        return 1;
    } else {
        #line 253
        return (n) * ((test1_fact_rec)((n) - (1)));
    }
}

test1_T (*test1_p);

const char * (test1_color_names[test1_NUM_COLORS]) = {[test1_COLOR_NONE] = "none", [test1_COLOR_RED] = "red", [test1_COLOR_GREEN] = "green", [test1_COLOR_BLUE] = "blue"};

#line 280
void test1_test_enum(void) {
    #line 281
    test1_Color a = test1_COLOR_RED;
    #line 282
    int b = test1_COLOR_RED;
    #line 283
    int c = (a) + (b);
    #line 284
    int i = a;
    #line 285
    (a) = i;
    #line 286
    (printf)("%d %d %d %d\n", test1_COLOR_NONE, test1_COLOR_RED, test1_COLOR_GREEN, test1_COLOR_BLUE);
}

#line 289
void test1_test_assign(void) {
    #line 290
    int i = 0;
    #line 291
    float f = 3.14f;
    #line 292
    int (*p) = &(i);
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
void test1_benchmark(int n) {
    #line 313
    int r = 1;
    #line 314
    for (int i = 1; (i) <= (n); (i)++) {
        #line 315
        (r) *= i;
    }
}

#line 319
int test1_va_test(int x, ...) {
    #line 320
    return 0;
}

#line 325
void test1_test_lits(void) {
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
void test1_test_ops(void) {
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
    (n) = (int)((((p) + (1))) - (p));
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
    (b) = (((n) & (~(1)))) ^ (1);
    #line 369
    (b) = (p) && (pi);
}

#line 374
void test1_test_bool(void) {
    #line 375
    bool b = false;
    #line 376
    (b) = true;
    #line 377
    int i = 0;
    #line 378
    (i) = test1_IS_DEBUG;
}

#line 381
int test1_test_ctrl(void) {
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

const int (test1_j);

const int (*test1_q);

const test1_Vector (test1_cv);

#line 395
void test1_f4(const char (*x)) {
}

#line 402
void test1_f5(const int (*p)) {
}

#line 405
void test1_test_convert(void) {
    #line 406
    const int (*a) = 0;
    #line 407
    int (*b) = 0;
    #line 408
    (a) = b;
    #line 409
    void (*p) = 0;
    #line 410
    (test1_f5)(p);
}

#line 413
void test1_test_const(void) {
    #line 414
    test1_ConstVector cv2 = {1, 2};
    int i = 0;
    #line 417
    (i) = 1;
    #line 420
    int x = test1_cv.x;
    char c = test1_escape_to_char[0];
    (test1_f4)(test1_escape_to_char);
    #line 425
    const char (*p) = (const char *)(0);
    #line 426
    (p) = (test1_escape_to_char) + (1);
    #line 427
    char (*q) = (char *)(test1_escape_to_char);
    #line 428
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 433
    (i) = (int)((ullong)(p));
}

#line 438
void test1_test_init(void) {
    #line 439
    int x = (const int)(0);
    #line 440
    #line 441
    int y = {0};
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
void test1_test_sizeof(void) {
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
void test1_test_cast(void) {
    #line 463
    int (*p) = 0;
    #line 464
    uint64_t a = 0;
    (a) = (uint64_t)(p);
    (p) = (int *)(a);
}

#line 471
void test1_print_any(Any any) {
    #line 472
    switch (any.type) {
        case TYPEID(8, TYPE_INT, int): {
            #line 474
            (printf)("%d", *((const int *)(any.ptr)));
            break;
        }
        case TYPEID(14, TYPE_FLOAT, float): {
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
    (test1_print_type)(any.type);
}

#line 484
void test1_println_any(Any any) {
    #line 485
    (test1_print_any)(any);
    #line 486
    (printf)("\n");
}

#line 489
void test1_print_typeid(typeid type) {
    #line 490
    int index = (typeid_index)(type);
    #line 491
    (printf)("typeid(%d)", index);
}

#line 494
void test1_print_type(typeid type) {
    #line 495
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 496
    if (!(typeinfo)) {
        #line 497
        (test1_print_typeid)(type);
        #line 498
        return;
    }
    #line 500
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 502
            (test1_print_type)(typeinfo->base);
            #line 503
            (printf)("*");
            break;
        }
        case TYPE_CONST: {
            #line 505
            (test1_print_type)(typeinfo->base);
            #line 506
            (printf)(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 508
            (test1_print_type)(typeinfo->base);
            #line 509
            (printf)("[%d]", typeinfo->count);
            break;
        }default: {
            #line 511
            if (typeinfo->name) {
                #line 512
                (printf)("%s", typeinfo->name);
            } else {
                #line 514
                (test1_print_typeid)(type);
            }
            break;
        }
    }
}

#line 519
void test1_println_type(typeid type) {
    #line 520
    (test1_print_type)(type);
    #line 521
    (printf)("\n");
}

#line 524
void test1_print_typeinfo(typeid type) {
    #line 525
    const TypeInfo (*typeinfo) = (get_typeinfo)(type);
    #line 526
    if (!(typeinfo)) {
        #line 527
        (test1_print_typeid)(type);
        #line 528
        return;
    }
    #line 530
    (printf)("<");
    #line 531
    (test1_print_type)(type);
    #line 532
    (printf)(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 533
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 536
            (printf)(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 537
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 538
                TypeFieldInfo field = typeinfo->fields[i];
                #line 539
                (printf)("@offset(%d) %s: ", field.offset, field.name);
                #line 540
                (test1_print_type)(field.type);
                #line 541
                (printf)("; ");
            }
            #line 543
            (printf)("}");
            break;
        }
    }
    #line 545
    (printf)(">");
}

#line 548
void test1_println_typeinfo(typeid type) {
    #line 549
    (test1_print_typeinfo)(type);
    #line 550
    (printf)("\n");
}

#line 553
void test1_test_typeinfo(void) {
    #line 554
    int i = 42;
    #line 555
    float f = 3.14f;
    #line 556
    void (*p) = NULL;
    (test1_println_any)((Any){&(i), TYPEID(8, TYPE_INT, int)});
    #line 559
    (test1_println_any)((Any){&(f), TYPEID(14, TYPE_FLOAT, float)});
    #line 560
    (test1_println_any)((Any){&(p), TYPEID(31, TYPE_PTR, void *)});
    (test1_println_type)(TYPEID(8, TYPE_INT, int));
    #line 563
    (test1_println_type)(TYPEID(66, TYPE_PTR, const int *));
    #line 564
    (test1_println_type)(TYPEID(96, TYPE_ARRAY, const int * [42]));
    #line 565
    (test1_println_type)(TYPEID(45, TYPE_STRUCT, test1_UartCtrl));
    (test1_println_typeinfo)(TYPEID(8, TYPE_INT, int));
    #line 568
    (test1_println_typeinfo)(TYPEID(45, TYPE_STRUCT, test1_UartCtrl));
    #line 569
    (test1_println_typeinfo)(TYPEID(97, TYPE_PTR, test1_IntOrPtr *));
    #line 570
    (test1_println_typeinfo)(TYPEID(49, TYPE_UNION, test1_IntOrPtr));
}

#line 579
void test1_test_compound_literals(void) {
    #line 580
    test1_Vector (*w) = {0};
    #line 581
    (w) = &((test1_Vector){1, 2});
    #line 582
    int (a[3]) = {1, 2, 3};
    #line 583
    int i = 42;
    #line 584
    const Any (x) = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 585
    Any y = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 586
    test1_Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 591
    test1_Ints (ints_of_ints[2]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

void test1_test_complete(void) {
    #line 598
    int x = 0;
    #line 601
    int y = 0;
    if ((x) == (0)) {
        #line 604
        (y) = 1;
    } else if ((x) == (1)) {
        #line 606
        (y) = 2;
    } else {
        #line 602
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 609
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 617
    switch (x) {
        case 0: {
            #line 619
            (y) = 3;
            break;
        }
        case 1: {
            #line 621
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 625
void test1_test_alignof(void) {
    #line 626
    int i = 42;
    #line 627
    ullong n1 = alignof(int);
    #line 628
    ullong n2 = alignof(int);
    #line 629
    ullong n3 = alignof(ullong);
    #line 630
    ullong n4 = alignof(int *);
}

#line 640
void test1_test_offsetof(void) {
    #line 641
    ullong n = offsetof(test1_BufHdr, buf);
}

test1_Thing test1_thing;

test1_Thing * test1_returns_ptr(void) {
    #line 652
    return &(test1_thing);
}

#line 655
const test1_Thing * test1_returns_ptr_to_const(void) {
    #line 656
    return &(test1_thing);
}

#line 659
void test1_test_lvalue(void) {
    #line 660
    ((test1_returns_ptr)()->a) = 5;
    const test1_Thing (*p) = (test1_returns_ptr_to_const)();
}

#line 666
void test1_test_if(void) {
    #line 667
    if (1) {
    }
    {
        #line 671
        int x = 42;
        if (x) {
        }
    }
    #line 673
    {
        #line 673
        int x = 42;
        if ((x) >= (0)) {
        }
    }
    #line 675
    {
        #line 675
        int x = 42;
        if ((x) >= (0)) {
        }
    }
}

#line 679
void test1_test_reachable(void) {
}

#line 683
void test1_test_os_arch(void) {
    #line 684
    (printf)("Target operating system: %s\n", IONOS);
    #line 685
    (printf)("Target machine architecture: %s\n", IONARCH);
}

#line 688
int main(int argc, const char * (*argv)) {
    #line 689
    if ((argv) == (0)) {
        #line 690
        (printf)("argv is null\n");
    }
    #line 692
    (test1_test_os_arch)();
    #line 693
    (test1_test_packages)();
    #line 694
    (test1_test_if)();
    #line 695
    (test1_test_modify)();
    #line 696
    (test1_test_lvalue)();
    #line 697
    (test1_test_alignof)();
    #line 698
    (test1_test_offsetof)();
    #line 699
    (test1_test_complete)();
    #line 700
    (test1_test_compound_literals)();
    #line 701
    (test1_test_loops)();
    #line 702
    (test1_test_sizeof)();
    #line 703
    (test1_test_assign)();
    #line 704
    (test1_test_enum)();
    #line 705
    (test1_test_arrays)();
    #line 706
    (test1_test_cast)();
    #line 707
    (test1_test_init)();
    #line 708
    (test1_test_lits)();
    #line 709
    (test1_test_const)();
    #line 710
    (test1_test_bool)();
    #line 711
    (test1_test_ops)();
    #line 712
    (test1_test_typeinfo)();
    #line 713
    (test1_test_reachable)();
    #line 714
    (getchar)();
    #line 715
    return 0;
}

#line 20 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
int test1_subtest1_func1(void) {
    #line 21
    (test1_subtest1_func2)();
    #line 22
    return 42;
}

#line 15
void test1_subtest1_func2(void) {
    #line 16
    (test1_subtest1_func3)();
    #line 17
    (test1_subtest1_func4)();
}

#line 8
void test1_subtest1_func3(void) {
    #line 9
    (printf)("func3\n");
}

#line 12
void test1_subtest1_func4(void) {
}

// Foreign source files
