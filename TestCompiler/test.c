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
#include <stdarg.h>
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

#define va_start_ptr(args, arg) (va_start(*(args), *(arg)))
#define va_copy_ptr(dest, src) (va_copy(*(dest), *(src)))
#define va_end_ptr(args) (va_end(*(args)))

void va_arg_ptr(va_list *args, void *dest, ullong type);

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

#line 166
uchar test1_h(void);

#line 40
typedef int (test1_A[(1) + ((2) * (sizeof(test1_h())))]);

#line 42
extern char (*test1_code);

#line 60
void test1_test_packages(void);

void test1_test_modify(void);

#line 85
void test1_f10(wchar_t (a[3]));

void test1_test_arrays(void);

#line 98
void test1_test_loops(void);

#line 132
void test1_test_nonmodifiable(void);

#line 148
#define test1_UART_CTRL_REG ((uint32_t *)(0x12345678))

#line 144
struct test1_UartCtrl {
    #line 145
    bool tx_enable;
    #line 145
    bool rx_enable;
};

#line 150
uint32_t test1_pack(test1_UartCtrl ctrl);

test1_UartCtrl test1_unpack(uint32_t word);

void test1_test_uart(void);

#line 193
typedef test1_IntOrPtr test1_U;

#line 199
union test1_IntOrPtr {
    #line 200
    int i;
    #line 201
    int (*p);
};

#line 176
int test1_g(test1_U u);

void test1_k(void (*vp), int (*ip));

#line 185
void test1_f1(void);

#line 190
void test1_f3(int (a[]));

#line 195
int test1_example_test(void);

#line 204
extern const char (test1_escape_to_char[256]);

#line 214
extern int (test1_a2[11]);

#line 217
int test1_is_even(int digit);

#line 233
extern int test1_i;

#line 235
struct test1_Vector {
    #line 236
    int x;
    #line 236
    int y;
};

#line 239
void test1_f2(test1_Vector v);

int test1_fact_iter(int n);

#line 251
int test1_fact_rec(int n);

#line 261
extern test1_T (*test1_p);

#line 259
#define test1_M ((1) + (sizeof(test1_p)))

#line 267
typedef int test1_Color;

#line 268
#define test1_COLOR_NONE ((int)(0))

#line 269
#define test1_COLOR_RED ((int)((test1_COLOR_NONE) + (1)))

#line 270
#define test1_COLOR_GREEN ((int)((test1_COLOR_RED) + (1)))

#line 271
#define test1_COLOR_BLUE ((int)((test1_COLOR_GREEN) + (1)))

#line 272
#define test1_NUM_COLORS ((int)((test1_COLOR_BLUE) + (1)))

#line 275
extern const char * (test1_color_names[test1_NUM_COLORS]);

#line 282
void test1_test_enum(void);

#line 291
void test1_test_assign(void);

#line 314
void test1_benchmark(int n);

#line 321
int test1_va_test(int x, ...);

typedef int (*test1_F)(int, ...);

#line 327
void test1_test_lits(void);

#line 344
void test1_test_ops(void);

#line 374
#define test1_IS_DEBUG (true)

#line 376
void test1_test_bool(void);

#line 383
int test1_test_ctrl(void);

#line 393
extern const int (test1_j);

#line 394
extern const int (*test1_q);

#line 395
extern const test1_Vector (test1_cv);

#line 397
void test1_f4(const char (*x));

#line 404
void test1_f5(const int (*p));

#line 407
void test1_test_convert(void);

#line 415
void test1_test_const(void);

#line 440
void test1_test_init(void);

#line 456
void test1_test_sizeof(void);

#line 464
void test1_test_cast(void);

#line 72 "C:/Users/john/source/repos/IonDotNet/Ion/system_packages/builtin/typeinfo.ion"
struct Any {
    #line 73
    void (*ptr);
    #line 74
    typeid type;
};

#line 473 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
void test1_print_any(Any any);

#line 486
void test1_println_any(Any any);

#line 491
void test1_print_typeid(typeid type);

#line 496
void test1_print_type(typeid type);

#line 521
void test1_println_type(typeid type);

#line 526
void test1_print_typeinfo(typeid type);

#line 550
void test1_println_typeinfo(typeid type);

#line 555
void test1_test_typeinfo(void);

#line 581
void test1_test_va_list(const char (*fmt), ...);

#line 594
void test1_test_compound_literals(void);

#line 612
void test1_test_complete(void);

#line 640
void test1_test_alignof(void);

#line 655
void test1_test_offsetof(void);

#line 660
struct test1_Thing {
    #line 661
    int a;
};

#line 664
extern test1_Thing test1_thing;

#line 666
test1_Thing * test1_returns_ptr(void);

const test1_Thing * test1_returns_ptr_to_const(void);

void test1_test_lvalue(void);

#line 681
void test1_test_if(void);

#line 694
void test1_test_reachable(void);

void test1_test_os_arch(void);

#line 703
int main(int argc, char * (*argv));

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

#line 263 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
struct test1_T {
    #line 264
    int (a[test1_M]);
};

#line 400
struct test1_ConstVector {
    #line 401
    const int (x);
    #line 401
    const int (y);
};

#line 575
struct test1_Ints {
    #line 576
    int num_ints;
    #line 577
    int (*int_ptr);
    #line 578
    int (int_arr[3]);
};

#line 648
struct test1_BufHdr {
    #line 649
    size_t cap;
    #line 649
    size_t len;
    #line 650
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

const TypeInfo *typeinfo_table[114] = {
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
        {"b", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_S1, b)},}},
    [43] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S2), .align = alignof(test1_S2), .name = "test1_S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"s1", .type = TYPEID(42, TYPE_STRUCT, test1_S1), .offset = offsetof(test1_S2, s1)},}},
    [44] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(ushort [3]), .align = alignof(ushort [3]), .base = TYPEID(7, TYPE_USHORT, ushort), .count = 3},
    [45] = NULL, // Func
    [46] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_UartCtrl), .align = alignof(test1_UartCtrl), .name = "test1_UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"tx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, tx_enable)},
        {"rx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, rx_enable)},}},
    [47] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(9, TYPE_UINT, uint)},
    [48] = NULL, // Func
    [49] = NULL, // Func
    [50] = &(TypeInfo){TYPE_UNION, .size = sizeof(test1_IntOrPtr), .align = alignof(test1_IntOrPtr), .name = "test1_IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"i", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_IntOrPtr, i)},
        {"p", .type = TYPEID(32, TYPE_PTR, int *), .offset = offsetof(test1_IntOrPtr, p)},}},
    [51] = NULL, // Func
    [52] = NULL, // Func
    [53] = NULL, // Func
    [54] = NULL, // Func
    [55] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char [256]), .align = alignof(const char [256]), .base = TYPEID(16, TYPE_CONST, const char), .count = 256},
    [56] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [11]), .align = alignof(int [11]), .base = TYPEID(8, TYPE_INT, int), .count = 11},
    [57] = NULL, // Func
    [58] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Vector), .align = alignof(test1_Vector), .name = "test1_Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, x)},
        {"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, y)},}},
    [59] = NULL, // Func
    [60] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_T), .align = alignof(test1_T), .name = "test1_T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(89, TYPE_ARRAY, int [9]), .offset = offsetof(test1_T, a)},}},
    [61] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(60, TYPE_STRUCT, test1_T)},
    [62] = NULL, // Enum
    [63] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const char * [4]), .align = alignof(const char * [4]), .base = TYPEID(17, TYPE_PTR, const char *), .count = 4},
    [64] = NULL, // Func
    [65] = NULL, // Func
    [66] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = TYPEID(8, TYPE_INT, int)},
    [67] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(66, TYPE_CONST, const int)},
    [68] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Vector), .align = alignof(const test1_Vector), .base = TYPEID(58, TYPE_STRUCT, test1_Vector)},
    [69] = NULL, // Func
    [70] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_ConstVector), .align = alignof(test1_ConstVector), .name = "test1_ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
        {"x", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, x)},
        {"y", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, y)},}},
    [71] = NULL, // Func
    [72] = NULL, // Func
    [73] = NULL, // Func
    [74] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Ints), .align = alignof(test1_Ints), .name = "test1_Ints", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"num_ints", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Ints, num_ints)},
        {"int_ptr", .type = TYPEID(32, TYPE_PTR, int *), .offset = offsetof(test1_Ints, int_ptr)},
        {"int_arr", .type = TYPEID(34, TYPE_ARRAY, int [3]), .offset = offsetof(test1_Ints, int_arr)},}},
    [75] = NULL, // Func
    [76] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_BufHdr), .align = alignof(test1_BufHdr), .name = "test1_BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
        {"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, cap)},
        {"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, len)},
        {"buf", .type = TYPEID(113, TYPE_ARRAY, char [1]), .offset = offsetof(test1_BufHdr, buf)},}},
    [77] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Thing), .align = alignof(test1_Thing), .name = "test1_Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
        {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Thing, a)},}},
    [78] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(77, TYPE_STRUCT, test1_Thing)},
    [79] = NULL, // Func
    [80] = &(TypeInfo){TYPE_CONST, .size = sizeof(const test1_Thing), .align = alignof(const test1_Thing), .base = TYPEID(77, TYPE_STRUCT, test1_Thing)},
    [81] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(80, TYPE_CONST, const test1_Thing)},
    [82] = NULL, // Func
    [83] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(18, TYPE_PTR, char *)},
    [84] = NULL, // Func
    [85] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(58, TYPE_STRUCT, test1_Vector)},
    [86] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = TYPEID(8, TYPE_INT, int), .count = 16},
    [87] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(7, TYPE_USHORT, ushort)},
    [88] = NULL, // Incomplete array type
    [89] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [9]), .align = alignof(int [9]), .base = TYPEID(8, TYPE_INT, int), .count = 9},
    [90] = NULL, // Func
    [91] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = TYPEID(8, TYPE_INT, int), .count = 4},
    [92] = &(TypeInfo){TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = TYPEID(14, TYPE_FLOAT, float)},
    [93] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(92, TYPE_CONST, const float)},
    [94] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = TYPEID(19, TYPE_NONE, TypeKind)},
    [95] = &(TypeInfo){TYPE_CONST, .size = sizeof(const ullong), .align = alignof(const ullong), .base = TYPEID(13, TYPE_ULLONG, ullong)},
    [96] = &(TypeInfo){TYPE_CONST, .size = sizeof(const const char *), .align = alignof(const const char *), .base = TYPEID(17, TYPE_PTR, const char *)},
    [97] = &(TypeInfo){TYPE_CONST, .size = sizeof(const TypeFieldInfo *), .align = alignof(const TypeFieldInfo *), .base = TYPEID(22, TYPE_PTR, TypeFieldInfo *)},
    [98] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(14, TYPE_FLOAT, float)},
    [99] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(31, TYPE_PTR, void *)},
    [100] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = TYPEID(67, TYPE_PTR, const int *), .count = 42},
    [101] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(50, TYPE_UNION, test1_IntOrPtr)},
    [102] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = TYPEID0(1, TYPE_VOID)},
    [103] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(102, TYPE_CONST)},
    [104] = NULL, // Func
    [105] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(17, TYPE_PTR, const char *)},
    [106] = NULL, // Func
    [107] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(12, TYPE_LLONG, llong)},
    [108] = NULL, // Func
    [109] = &(TypeInfo){TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = TYPEID(30, TYPE_STRUCT, Any)},
    [110] = NULL, // Incomplete array type
    [111] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [2]), .align = alignof(int [2]), .base = TYPEID(8, TYPE_INT, int), .count = 2},
    [112] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(test1_Ints [2]), .align = alignof(test1_Ints [2]), .base = TYPEID(74, TYPE_STRUCT, test1_Ints), .count = 2},
    [113] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1},
};
int num_typeinfos = 114;
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
    int index = typeid_index(type);
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

#line 166 "C:/Users/john/source/repos/IonDotNet/Ion/test1/test1.ion"
uchar test1_h(void) {
    #line 167
    ((test1_Vector){.x = 1, .y = 2}.x) = 42;
    #line 168
    test1_Vector (*v) = &((test1_Vector){1, 2});
    #line 169
    (v->x) = 42;
    #line 170
    int (*p) = &((int){0});
    #line 171
    ulong x = ((uint){1}) + ((long){2});
    #line 172
    int y = +(test1_c);
    #line 173
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
    test1_subtest1_func1();
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
void test1_f10(wchar_t (a[3])) {
    #line 86
    (a[1]) = 42;
}

#line 89
void test1_test_arrays(void) {
    #line 90
    wchar_t (a[]) = {1, 2, 3};
    test1_f10(a);
    #line 93
    ushort (*b) = a;
    #line 94
    wchar_t w1 = {0};
    #line 95
    ushort w2 = w1;
}

#line 98
void test1_test_loops(void) {
    #line 101
    switch (0) {default: {
            if (1) {
                #line 104
                break;
            }
            #line 106
            for (;;) {
                #line 107
                continue;
            }
            break;
        }
    }
    #line 112
    while (0) {
    }
    #line 114
    for (int i = 0; (i) < (10); (i)++) {
    }
    #line 116
    for (;;) {
        #line 117
        break;
    }
    #line 119
    for (int i = 0;;) {
        #line 120
        break;
    }
    #line 122
    for (; 0;) {
    }
    #line 124
    for (int i = 0;; (i)++) {
        #line 125
        break;
    }
    #line 127
    int i = 0;
    #line 128
    for (;; (i)++) {
        #line 129
        break;
    }
}

#line 132
void test1_test_nonmodifiable(void) {
    #line 133
    test1_S1 s1 = {0};
    #line 134
    (s1.a) = 0;
    #line 137
    test1_S2 s2 = {0};
    #line 138
    (s2.s1.a) = 0;
}

#line 150
uint32_t test1_pack(test1_UartCtrl ctrl) {
    #line 151
    return (((ctrl.tx_enable) & (1))) | (((((ctrl.rx_enable) & (1))) << (1)));
}

#line 154
test1_UartCtrl test1_unpack(uint32_t word) {
    #line 155
    return (test1_UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = (((word) & (0x2))) >> (1)};
}

#line 158
void test1_test_uart(void) {
    #line 159
    bool tx_enable = test1_unpack(*(test1_UART_CTRL_REG)).tx_enable;
    #line 160
    (*(test1_UART_CTRL_REG)) = test1_pack((test1_UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 161
    test1_UartCtrl ctrl = test1_unpack(*(test1_UART_CTRL_REG));
    #line 162
    (ctrl.rx_enable) = true;
    #line 163
    (*(test1_UART_CTRL_REG)) = test1_pack(ctrl);
}

#line 176
int test1_g(test1_U u) {
    #line 177
    return u.i;
}

#line 180
void test1_k(void (*vp), int (*ip)) {
    #line 181
    (vp) = ip;
    #line 182
    (ip) = vp;
}

#line 185
void test1_f1(void) {
    #line 186
    int (*p) = &((int){0});
    #line 187
    (*(p)) = 42;
}

#line 190
void test1_f3(int (a[])) {
}

#line 195
int test1_example_test(void) {
    #line 196
    return (test1_fact_rec(10)) == (test1_fact_iter(10));
}

const char (test1_escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (test1_a2[11]) = {1, 2, 3, [10] = 4};

#line 217
int test1_is_even(int digit) {
    #line 218
    int b = 0;
    #line 219
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 221
            (b) = 1;
            break;
        }
    }
    #line 223
    return b;
}

int test1_i;

#line 239
void test1_f2(test1_Vector v) {
    #line 240
    (v) = (test1_Vector){0};
}

#line 243
int test1_fact_iter(int n) {
    #line 244
    int r = 1;
    #line 245
    for (int i = 0; (i) <= (n); (i)++) {
        #line 246
        (r) *= i;
    }
    #line 248
    return r;
}

#line 251
int test1_fact_rec(int n) {
    #line 252
    if ((n) == (0)) {
        #line 253
        return 1;
    } else {
        #line 255
        return (n) * (test1_fact_rec((n) - (1)));
    }
}

test1_T (*test1_p);

const char * (test1_color_names[test1_NUM_COLORS]) = {[test1_COLOR_NONE] = "none", [test1_COLOR_RED] = "red", [test1_COLOR_GREEN] = "green", [test1_COLOR_BLUE] = "blue"};

#line 282
void test1_test_enum(void) {
    #line 283
    test1_Color a = test1_COLOR_RED;
    #line 284
    int b = test1_COLOR_RED;
    #line 285
    int c = (a) + (b);
    #line 286
    int i = a;
    #line 287
    (a) = i;
    #line 288
    printf("%d %d %d %d\n", test1_COLOR_NONE, test1_COLOR_RED, test1_COLOR_GREEN, test1_COLOR_BLUE);
}

#line 291
void test1_test_assign(void) {
    #line 292
    int i = 0;
    #line 293
    float f = 3.14f;
    #line 294
    int (*p) = &(i);
    #line 295
    (i)++;
    #line 296
    (i)--;
    #line 297
    (p)++;
    #line 298
    (p)--;
    #line 299
    (p) += 1;
    #line 300
    (i) /= 2;
    #line 301
    (i) *= 123;
    #line 302
    (i) %= 3;
    #line 303
    (i) <<= 1;
    #line 304
    (i) >>= 2;
    #line 305
    (i) &= 0xFF;
    #line 306
    (i) |= 0xFF00;
    #line 307
    (i) ^= 0xFF0;
}

#line 314
void test1_benchmark(int n) {
    #line 315
    int r = 1;
    #line 316
    for (int i = 1; (i) <= (n); (i)++) {
        #line 317
        (r) *= i;
    }
}

#line 321
int test1_va_test(int x, ...) {
    #line 322
    return 0;
}

#line 327
void test1_test_lits(void) {
    #line 328
    float f = 3.14f;
    #line 329
    double d = 3.14;
    #line 330
    int i = 1;
    #line 331
    uint u = 0xFFFFFFFFu;
    #line 332
    long l = 1l;
    #line 333
    ulong ul = 1ul;
    #line 334
    llong ll = 0x100000000ll;
    #line 335
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 336
    uint x1 = 0xFFFFFFFF;
    #line 337
    llong x2 = 4294967295;
    #line 338
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 339
    int x4 = (0xAA) + (0x55);
}

#line 344
void test1_test_ops(void) {
    #line 345
    float pi = 3.14f;
    #line 346
    float f = 0.0f;
    #line 347
    (f) = +(pi);
    #line 348
    (f) = -(pi);
    #line 349
    int n = -(1);
    #line 350
    (n) = ~(n);
    #line 351
    (f) = ((f) * (pi)) + (n);
    #line 352
    (f) = (pi) / (pi);
    #line 353
    (n) = (3) % (2);
    #line 354
    (n) = (n) + ((uchar)(1));
    #line 355
    int (*p) = &(n);
    #line 356
    (p) = (p) + (1);
    #line 357
    (n) = (int)((((p) + (1))) - (p));
    #line 358
    (n) = (n) << (1);
    #line 359
    (n) = (n) >> (1);
    #line 360
    int b = ((p) + (1)) > (p);
    #line 361
    (b) = ((p) + (1)) >= (p);
    #line 362
    (b) = ((p) + (1)) < (p);
    #line 363
    (b) = ((p) + (1)) <= (p);
    #line 364
    (b) = ((p) + (1)) == (p);
    #line 365
    (b) = (1) > (2);
    #line 366
    (b) = (1.23f) <= (pi);
    #line 367
    (n) = 0xFF;
    #line 368
    (b) = (n) & (~(1));
    #line 369
    (b) = (n) & (1);
    #line 370
    (b) = (((n) & (~(1)))) ^ (1);
    #line 371
    (b) = (p) && (pi);
}

#line 376
void test1_test_bool(void) {
    #line 377
    bool b = false;
    #line 378
    (b) = true;
    #line 379
    int i = 0;
    #line 380
    (i) = test1_IS_DEBUG;
}

#line 383
int test1_test_ctrl(void) {
    #line 384
    switch (1) {
        case 0: {
            #line 386
            return 0;
            break;
        }default: {
            #line 388
            return 1;
            break;
        }
    }
}

const int (test1_j);

const int (*test1_q);

const test1_Vector (test1_cv);

#line 397
void test1_f4(const char (*x)) {
}

#line 404
void test1_f5(const int (*p)) {
}

#line 407
void test1_test_convert(void) {
    #line 408
    const int (*a) = 0;
    #line 409
    int (*b) = 0;
    #line 410
    (a) = b;
    #line 411
    void (*p) = 0;
    #line 412
    test1_f5(p);
}

#line 415
void test1_test_const(void) {
    #line 416
    test1_ConstVector cv2 = {1, 2};
    int i = 0;
    #line 419
    (i) = 1;
    #line 422
    int x = test1_cv.x;
    char c = test1_escape_to_char[0];
    test1_f4(test1_escape_to_char);
    #line 427
    const char (*p) = (const char *)(0);
    #line 428
    (p) = (test1_escape_to_char) + (1);
    #line 429
    char (*q) = (char *)(test1_escape_to_char);
    #line 430
    (c) = q['n'];
    (p) = (const char *)(1);
    #line 435
    (i) = (int)((ullong)(p));
}

#line 440
void test1_test_init(void) {
    #line 441
    int x = (const int)(0);
    #line 442
    #line 443
    int y = {0};
    #line 444
    (y) = 0;
    #line 445
    int z = 42;
    #line 446
    int (a[]) = {1, 2, 3};
    #line 449
    for (ullong i = 0; (i) < (10); (i)++) {
        #line 450
        printf("%llu\n", i);
    }
    #line 452
    int (b[4]) = {1, 2, 3, 4};
    #line 453
    (b[0]) = a[2];
}

#line 456
void test1_test_sizeof(void) {
    #line 457
    int i = 0;
    #line 458
    ullong n = sizeof(i);
    #line 459
    (n) = sizeof(int);
    #line 460
    (n) = sizeof(int);
    #line 461
    (n) = sizeof(int *);
}

#line 464
void test1_test_cast(void) {
    #line 465
    int (*p) = 0;
    #line 466
    uint64_t a = 0;
    (a) = (uint64_t)(p);
    (p) = (int *)(a);
}

#line 473
void test1_print_any(Any any) {
    #line 474
    switch (any.type) {
        case TYPEID(8, TYPE_INT, int): {
            #line 476
            printf("%d", *((const int *)(any.ptr)));
            break;
        }
        case TYPEID(14, TYPE_FLOAT, float): {
            #line 478
            printf("%f", *((const float *)(any.ptr)));
            break;
        }default: {
            #line 480
            printf("<unknown>");
            break;
        }
    }
    #line 482
    printf(": ");
    #line 483
    test1_print_type(any.type);
}

#line 486
void test1_println_any(Any any) {
    #line 487
    test1_print_any(any);
    #line 488
    printf("\n");
}

#line 491
void test1_print_typeid(typeid type) {
    #line 492
    int index = typeid_index(type);
    #line 493
    printf("typeid(%d)", index);
}

#line 496
void test1_print_type(typeid type) {
    #line 497
    const TypeInfo (*typeinfo) = get_typeinfo(type);
    #line 498
    if (!(typeinfo)) {
        #line 499
        test1_print_typeid(type);
        #line 500
        return;
    }
    #line 502
    switch (typeinfo->kind) {
        case TYPE_PTR: {
            #line 504
            test1_print_type(typeinfo->base);
            #line 505
            printf("*");
            break;
        }
        case TYPE_CONST: {
            #line 507
            test1_print_type(typeinfo->base);
            #line 508
            printf(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 510
            test1_print_type(typeinfo->base);
            #line 511
            printf("[%d]", typeinfo->count);
            break;
        }default: {
            #line 513
            if (typeinfo->name) {
                #line 514
                printf("%s", typeinfo->name);
            } else {
                #line 516
                test1_print_typeid(type);
            }
            break;
        }
    }
}

#line 521
void test1_println_type(typeid type) {
    #line 522
    test1_print_type(type);
    #line 523
    printf("\n");
}

#line 526
void test1_print_typeinfo(typeid type) {
    #line 527
    const TypeInfo (*typeinfo) = get_typeinfo(type);
    #line 528
    if (!(typeinfo)) {
        #line 529
        test1_print_typeid(type);
        #line 530
        return;
    }
    #line 532
    printf("<");
    #line 533
    test1_print_type(type);
    #line 534
    printf(" size=%d align=%d", typeinfo->size, typeinfo->align);
    #line 535
    switch (typeinfo->kind) {
        case TYPE_STRUCT:
        case TYPE_UNION: {
            #line 538
            printf(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
            #line 539
            for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
                #line 540
                TypeFieldInfo field = typeinfo->fields[i];
                #line 541
                printf("@offset(%d) %s: ", field.offset, field.name);
                #line 542
                test1_print_type(field.type);
                #line 543
                printf("; ");
            }
            #line 545
            printf("}");
            break;
        }
    }
    #line 547
    printf(">");
}

#line 550
void test1_println_typeinfo(typeid type) {
    #line 551
    test1_print_typeinfo(type);
    #line 552
    printf("\n");
}

#line 555
void test1_test_typeinfo(void) {
    #line 556
    int i = 42;
    #line 557
    float f = 3.14f;
    #line 558
    void (*p) = NULL;
    test1_println_any((Any){&(i), TYPEID(8, TYPE_INT, int)});
    #line 561
    test1_println_any((Any){&(f), TYPEID(14, TYPE_FLOAT, float)});
    #line 562
    test1_println_any((Any){&(p), TYPEID(31, TYPE_PTR, void *)});
    test1_println_type(TYPEID(8, TYPE_INT, int));
    #line 565
    test1_println_type(TYPEID(67, TYPE_PTR, const int *));
    #line 566
    test1_println_type(TYPEID(100, TYPE_ARRAY, const int * [42]));
    #line 567
    test1_println_type(TYPEID(46, TYPE_STRUCT, test1_UartCtrl));
    test1_println_typeinfo(TYPEID(8, TYPE_INT, int));
    #line 570
    test1_println_typeinfo(TYPEID(46, TYPE_STRUCT, test1_UartCtrl));
    #line 571
    test1_println_typeinfo(TYPEID(101, TYPE_PTR, test1_IntOrPtr *));
    #line 572
    test1_println_typeinfo(TYPEID(50, TYPE_UNION, test1_IntOrPtr));
}

#line 581
void test1_test_va_list(const char (*fmt), ...) {
    #line 582
    va_list args = {0};
    #line 583
    va_start_ptr(&(args), &(fmt));
    #line 584
    char c = {0};
    #line 585
    va_arg_ptr(&(args), &(c), TYPEID(3, TYPE_CHAR, char));
    #line 586
    int i = {0};
    #line 587
    va_arg_ptr(&(args), &(i), TYPEID(8, TYPE_INT, int));
    #line 588
    llong ll = {0};
    #line 589
    va_arg_ptr(&(args), &(ll), TYPEID(12, TYPE_LLONG, llong));
    #line 590
    printf("c=%d i=%d ll=%lld\n", c, i, ll);
    #line 591
    va_end_ptr(&(args));
}

#line 594
void test1_test_compound_literals(void) {
    #line 595
    test1_Vector (*w) = {0};
    #line 596
    (w) = &((test1_Vector){1, 2});
    #line 597
    int (a[3]) = {1, 2, 3};
    #line 598
    int i = 42;
    #line 599
    const Any (x) = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 600
    Any y = {&(i), TYPEID(8, TYPE_INT, int)};
    #line 601
    test1_Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 606
    test1_Ints (ints_of_ints[]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

void test1_test_complete(void) {
    #line 613
    int x = 0;
    #line 616
    int y = 0;
    if ((x) == (0)) {
        #line 619
        (y) = 1;
    } else if ((x) == (1)) {
        #line 621
        (y) = 2;
    } else {
        #line 617
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 624
    (x) = 1;
    assert((x) >= (0));
    (x) = 0;
    #line 632
    switch (x) {
        case 0: {
            #line 634
            (y) = 3;
            break;
        }
        case 1: {
            #line 636
            (y) = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 640
void test1_test_alignof(void) {
    #line 641
    int i = 42;
    #line 642
    ullong n1 = alignof(int);
    #line 643
    ullong n2 = alignof(int);
    #line 644
    ullong n3 = alignof(ullong);
    #line 645
    ullong n4 = alignof(int *);
}

#line 655
void test1_test_offsetof(void) {
    #line 656
    ullong n = offsetof(test1_BufHdr, buf);
}

test1_Thing test1_thing;

test1_Thing * test1_returns_ptr(void) {
    #line 667
    return &(test1_thing);
}

#line 670
const test1_Thing * test1_returns_ptr_to_const(void) {
    #line 671
    return &(test1_thing);
}

#line 674
void test1_test_lvalue(void) {
    #line 675
    (test1_returns_ptr()->a) = 5;
    const test1_Thing (*p) = test1_returns_ptr_to_const();
}

#line 681
void test1_test_if(void) {
    #line 682
    if (1) {
    }
    {
        #line 686
        int x = 42;
        if (x) {
        }
    }
    #line 688
    {
        #line 688
        int x = 42;
        if ((x) >= (0)) {
        }
    }
    #line 690
    {
        #line 690
        int x = 42;
        if ((x) >= (0)) {
        }
    }
}

#line 694
void test1_test_reachable(void) {
}

#line 698
void test1_test_os_arch(void) {
    #line 699
    printf("Target operating system: %s\n", IONOS);
    #line 700
    printf("Target machine architecture: %s\n", IONARCH);
}

#line 703
int main(int argc, char * (*argv)) {
    #line 704
    if ((argv) == (0)) {
        #line 705
        printf("argv is null\n");
    }
    #line 707
    test1_test_va_list("whatever", (char)(123), (int)(123123), (llong)(123123123123));
    #line 708
    test1_test_os_arch();
    #line 709
    test1_test_packages();
    #line 710
    test1_test_if();
    #line 711
    test1_test_modify();
    #line 712
    test1_test_lvalue();
    #line 713
    test1_test_alignof();
    #line 714
    test1_test_offsetof();
    #line 715
    test1_test_complete();
    #line 716
    test1_test_compound_literals();
    #line 717
    test1_test_loops();
    #line 718
    test1_test_sizeof();
    #line 719
    test1_test_assign();
    #line 720
    test1_test_enum();
    #line 721
    test1_test_arrays();
    #line 722
    test1_test_cast();
    #line 723
    test1_test_init();
    #line 724
    test1_test_lits();
    #line 725
    test1_test_const();
    #line 726
    test1_test_bool();
    #line 727
    test1_test_ops();
    #line 728
    test1_test_typeinfo();
    #line 729
    test1_test_reachable();
    #line 730
    getchar();
    #line 731
    return 0;
}

#line 20 "C:/Users/john/source/repos/IonDotNet/Ion/test1/subtest1/subtest1.ion"
int test1_subtest1_func1(void) {
    #line 21
    test1_subtest1_func2();
    #line 22
    return 42;
}

#line 15
void test1_subtest1_func2(void) {
    #line 16
    test1_subtest1_func3();
    #line 17
    test1_subtest1_func4();
}

#line 8
void test1_subtest1_func3(void) {
    #line 9
    printf("func3\n");
}

#line 12
void test1_subtest1_func4(void) {
}

// Foreign source files

// Postamble
void va_arg_ptr(va_list *args, void *arg, ullong type) {
    switch (typeid_kind(type)) {
    case TYPE_BOOL:
        *(bool *)arg = va_arg(*args, int);
        break;
    case TYPE_CHAR:
        *(char *)arg = va_arg(*args, int);
        break;
    case TYPE_UCHAR:
        *(uchar *)arg = va_arg(*args, int);
        break;
    case TYPE_SCHAR:
        *(schar *)arg = va_arg(*args, int);
        break;
    case TYPE_SHORT:
        *(short *)arg = va_arg(*args, int);
        break;
    case TYPE_USHORT:
        *(ushort *)arg = va_arg(*args, int);
        break;
    case TYPE_INT:
        *(int *)arg = va_arg(*args, int);
        break;
    case TYPE_UINT:
        *(uint *)arg = va_arg(*args, uint);
        break;
    case TYPE_LONG:
        *(long *)arg = va_arg(*args, long);
        break;
    case TYPE_ULONG:
        *(ulong *)arg = va_arg(*args, ulong);
        break;
    case TYPE_LLONG:
        *(llong *)arg = va_arg(*args, llong);
        break;
    case TYPE_ULLONG:
        *(ullong *)arg = va_arg(*args, ullong);
        break;
    case TYPE_FLOAT:
        *(float *)arg = va_arg(*args, double);
        break;
    case TYPE_DOUBLE:
        *(double *)arg = va_arg(*args, double);
        break;
    case TYPE_FUNC:
    case TYPE_PTR:
        *(void **)arg = va_arg(*args, void *);
        break;
    default:
        assert(0 && "argument type not supported");
        break;
    }
}
