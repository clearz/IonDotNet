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

void va_arg_ptr(va_list* args, void* dest, ullong type);

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
extern const char(*IONOS);

extern const char(*IONARCH);

typedef ullong typeid;

typedef int TypeKind;

#define TYPE_NONE ((int)(0))

#define TYPE_VOID ((int)((TYPE_NONE) + (1)))

#define TYPE_BOOL ((int)((TYPE_VOID) + (1)))

#define TYPE_CHAR ((int)((TYPE_BOOL) + (1)))

#define TYPE_UCHAR ((int)((TYPE_CHAR) + (1)))

#define TYPE_SCHAR ((int)((TYPE_UCHAR) + (1)))

#define TYPE_SHORT ((int)((TYPE_SCHAR) + (1)))

#define TYPE_USHORT ((int)((TYPE_SHORT) + (1)))

#define TYPE_INT ((int)((TYPE_USHORT) + (1)))

#define TYPE_UINT ((int)((TYPE_INT) + (1)))

#define TYPE_LONG ((int)((TYPE_UINT) + (1)))

#define TYPE_ULONG ((int)((TYPE_LONG) + (1)))

#define TYPE_LLONG ((int)((TYPE_ULONG) + (1)))

#define TYPE_ULLONG ((int)((TYPE_LLONG) + (1)))

#define TYPE_FLOAT ((int)((TYPE_ULLONG) + (1)))

#define TYPE_DOUBLE ((int)((TYPE_FLOAT) + (1)))

#define TYPE_CONST ((int)((TYPE_DOUBLE) + (1)))

#define TYPE_PTR ((int)((TYPE_CONST) + (1)))

#define TYPE_ARRAY ((int)((TYPE_PTR) + (1)))

#define TYPE_STRUCT ((int)((TYPE_ARRAY) + (1)))

#define TYPE_UNION ((int)((TYPE_STRUCT) + (1)))

#define TYPE_FUNC ((int)((TYPE_UNION) + (1)))

struct TypeInfo {
	TypeKind kind;
	int size;
	int align;
	const char(*name);
	int count;
	typeid base;
	TypeFieldInfo(*fields);
	int num_fields;
};

TypeKind typeid_kind(typeid type);

int typeid_index(typeid type);

size_t typeid_size(typeid type);

const TypeInfo* get_typeinfo(typeid type);

extern char(*test1_esc_test_str);

extern int(*test1_some_array);


extern test1_SomeIncompleteType(*test1_incomplete_ptr);

#define test1_PI (3.14f)

#define test1_PI2 ((test1_PI) + (test1_PI))

#define test1_U8 ((uint8_t)(42))

extern char test1_c;

extern uchar test1_uc;

extern schar test1_sc;

typedef void (*test1_F1)(void);

typedef int (*test1_F2)(int (*)(int, int));

typedef void (*test1_F3)(void (*)(void));

#define test1_N ((((char)(42)) + (8)) != (0))

uchar test1_h(void);

typedef int(test1_A[(1) + ((2) * (sizeof(test1_h())))]);

extern char(*test1_code);

void test1_test_packages(void);

void test1_test_modify(void);

void test1_f10(wchar_t(a[3]));

void test1_test_arrays(void);

void test1_test_loops(void);

void test1_test_nonmodifiable(void);

#define test1_UART_CTRL_REG ((uint32_t *)(0x12345678))

struct test1_UartCtrl {
	bool tx_enable;
	bool rx_enable;
};

uint32_t test1_pack(test1_UartCtrl ctrl);

test1_UartCtrl test1_unpack(uint32_t word);

void test1_test_uart(void);

typedef test1_IntOrPtr test1_U;

union test1_IntOrPtr {
	int i;
	int(*p);
};

int test1_g(test1_U u);

void test1_k(void(*vp), int(*ip));

void test1_f1(void);

void test1_f3(int(a[]));

int test1_example_test(void);

extern const char(test1_escape_to_char[256]);

extern int(test1_a2[11]);

int test1_is_even(int digit);

extern int test1_i;

struct test1_Vector {
	int x;
	int y;
};

void test1_f2(test1_Vector v);

int test1_fact_iter(int n);

int test1_fact_rec(int n);

extern test1_T(*test1_p);

#define test1_M ((1) + (sizeof(test1_p)))

typedef int test1_Color;

#define test1_COLOR_NONE ((int)(0))

#define test1_COLOR_RED ((int)((test1_COLOR_NONE) + (1)))

#define test1_COLOR_GREEN ((int)((test1_COLOR_RED) + (1)))

#define test1_COLOR_BLUE ((int)((test1_COLOR_GREEN) + (1)))

#define test1_NUM_COLORS ((int)((test1_COLOR_BLUE) + (1)))

extern const char* (test1_color_names[test1_NUM_COLORS]);

void test1_test_enum(void);

void test1_test_assign(void);

void test1_benchmark(int n);

int test1_va_test(int x, ...);

typedef int (*test1_F)(int, ...);

void test1_test_lits(void);

void test1_test_ops(void);

#define test1_IS_DEBUG (true)

void test1_test_bool(void);

int test1_test_ctrl(void);

extern const int(test1_j);

extern const int(*test1_q);

extern const test1_Vector(test1_cv);

void test1_f4(const char(*x));

void test1_f5(const int(*p));

void test1_test_convert(void);

void test1_test_const(void);

void test1_test_init(void);

void test1_test_sizeof(void);

void test1_test_cast(void);

struct Any {
	void(*ptr);
	typeid type;
};

void test1_print_any(Any any);

void test1_println_any(Any any);

void test1_print_typeid(typeid type);

void test1_print_type(typeid type);

void test1_println_type(typeid type);

void test1_print_typeinfo(typeid type);

void test1_println_typeinfo(typeid type);

void test1_test_typeinfo(void);

void test1_test_va_list(const char(*fmt), ...);

void test1_test_compound_literals(void);

void test1_test_complete(void);

void test1_test_alignof(void);

void test1_test_offsetof(void);

struct test1_Thing {
	int a;
};

extern test1_Thing test1_thing;

test1_Thing* test1_returns_ptr(void);

const test1_Thing* test1_returns_ptr_to_const(void);

void test1_test_lvalue(void);

void test1_test_if(void);

void test1_test_reachable(void);

void test1_test_os_arch(void);

int main(int argc, char* (*argv));

struct TypeFieldInfo {
	const char(*name);
	typeid type;
	int offset;
};

struct test1_S1 {
	int a;
	const int(b);
};

struct test1_S2 {
	test1_S1 s1;
};

int test1_subtest1_func1(void);

struct test1_T {
	int(a[test1_M]);
};

struct test1_ConstVector {
	const int(x);
	const int(y);
};

struct test1_Ints {
	int num_ints;
	int(*int_ptr);
	int(int_arr[3]);
};

struct test1_BufHdr {
	size_t cap;
	size_t len;
	char(buf[1]);
};

void test1_subtest1_func2(void);

void test1_subtest1_func3(void);

void test1_subtest1_func4(void);

// Typeinfo
#define TYPEID0(index, kind) ((ullong)(index) | ((kind) << 24ull))
#define TYPEID(index, kind, ...) ((ullong)(index) | (sizeof(__VA_ARGS__) << 32ull) | ((kind) << 24ull))

const TypeInfo* typeinfo_table[117] = {
	[0] = NULL, // No associated type
	[1] = &(TypeInfo) { TYPE_VOID, .name = "void", .size = 0, .align = 0 },
	[2] = &(TypeInfo) { TYPE_BOOL, .size = sizeof(bool), .align = alignof(bool), .name = "bool" },
	[3] = &(TypeInfo) { TYPE_CHAR, .size = sizeof(char), .align = alignof(char), .name = "char" },
	[4] = &(TypeInfo) { TYPE_UCHAR, .size = sizeof(uchar), .align = alignof(uchar), .name = "uchar" },
	[5] = &(TypeInfo) { TYPE_SCHAR, .size = sizeof(schar), .align = alignof(schar), .name = "schar" },
	[6] = &(TypeInfo) { TYPE_SHORT, .size = sizeof(short), .align = alignof(short), .name = "short" },
	[7] = &(TypeInfo) { TYPE_USHORT, .size = sizeof(ushort), .align = alignof(ushort), .name = "ushort" },
	[8] = &(TypeInfo) { TYPE_INT, .size = sizeof(int), .align = alignof(int), .name = "int" },
	[9] = &(TypeInfo) { TYPE_UINT, .size = sizeof(uint), .align = alignof(uint), .name = "uint" },
	[10] = &(TypeInfo) { TYPE_LONG, .size = sizeof(long), .align = alignof(long), .name = "long" },
	[11] = &(TypeInfo) { TYPE_ULONG, .size = sizeof(ulong), .align = alignof(ulong), .name = "ulong" },
	[12] = &(TypeInfo) { TYPE_LLONG, .size = sizeof(llong), .align = alignof(llong), .name = "llong" },
	[13] = &(TypeInfo) { TYPE_ULLONG, .size = sizeof(ullong), .align = alignof(ullong), .name = "ullong" },
	[14] = &(TypeInfo) { TYPE_FLOAT, .size = sizeof(float), .align = alignof(float), .name = "float" },
	[15] = &(TypeInfo) { TYPE_DOUBLE, .size = sizeof(double), .align = alignof(double), .name = "double" },
	[16] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = TYPEID(3, TYPE_CHAR, char) },
	[17] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(16, TYPE_CONST, const char) },
	[18] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(3, TYPE_CHAR, char) },
	[19] = NULL, // Enum
	[20] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3, .fields = (TypeFieldInfo[]) {
{"name", .type = TYPEID(17, TYPE_PTR, const char*), .offset = offsetof(TypeFieldInfo, name)},
{"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeFieldInfo, type)},
{"offset", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeFieldInfo, offset)},}
},
[21] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8, .fields = (TypeFieldInfo[]) {
{"kind", .type = TYPEID(19, TYPE_NONE, TypeKind), .offset = offsetof(TypeInfo, kind)},
{"size", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, size)},
{"align", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, align)},
{"name", .type = TYPEID(17, TYPE_PTR, const char*), .offset = offsetof(TypeInfo, name)},
{"count", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, count)},
{"base", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeInfo, base)},
{"fields", .type = TYPEID(22, TYPE_PTR, TypeFieldInfo*), .offset = offsetof(TypeInfo, fields)},
{"num_fields", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, num_fields)},}
},
[22] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(20, TYPE_STRUCT, TypeFieldInfo) },
[23] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const TypeInfo), .align = alignof(const TypeInfo), .base = TYPEID(21, TYPE_STRUCT, TypeInfo) },
[24] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(23, TYPE_CONST, const TypeInfo) },
[25] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(24, TYPE_PTR, const TypeInfo*) },
[26] = NULL, // Func
[27] = NULL, // Func
[28] = NULL, // Func
[29] = NULL, // Func
[30] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(Any), .align = alignof(Any), .name = "Any", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"ptr", .type = TYPEID(31, TYPE_PTR, void*), .offset = offsetof(Any, ptr)},
{"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(Any, type)},}
},
[31] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID0(1, TYPE_VOID) },
[32] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(8, TYPE_INT, int) },
[33] = NULL, // Incomplete array type
[34] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[3]), .align = alignof(int[3]), .base = TYPEID(8, TYPE_INT, int), .count = 3 },
[35] = NULL, // Incomplete: test1_SomeIncompleteType
[36] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID0(35, TYPE_NONE) },
[37] = NULL, // Func
[38] = NULL, // Func
[39] = NULL, // Func
[40] = NULL, // Func
[41] = NULL, // Func
[42] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_S1), .align = alignof(test1_S1), .name = "test1_S1", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_S1, a)},
{"b", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_S1, b)},}
},
[43] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_S2), .align = alignof(test1_S2), .name = "test1_S2", .num_fields = 1, .fields = (TypeFieldInfo[]) {
{"s1", .type = TYPEID(42, TYPE_STRUCT, test1_S1), .offset = offsetof(test1_S2, s1)},}
},
[44] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(ushort[3]), .align = alignof(ushort[3]), .base = TYPEID(7, TYPE_USHORT, ushort), .count = 3 },
[45] = NULL, // Func
[46] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_UartCtrl), .align = alignof(test1_UartCtrl), .name = "test1_UartCtrl", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"tx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, tx_enable)},
{"rx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, rx_enable)},}
},
[47] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(9, TYPE_UINT, uint) },
[48] = NULL, // Func
[49] = NULL, // Func
[50] = &(TypeInfo) {
TYPE_UNION, .size = sizeof(test1_IntOrPtr), .align = alignof(test1_IntOrPtr), .name = "test1_IntOrPtr", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"i", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_IntOrPtr, i)},
{"p", .type = TYPEID(32, TYPE_PTR, int*), .offset = offsetof(test1_IntOrPtr, p)},}
},
[51] = NULL, // Func
[52] = NULL, // Func
[53] = NULL, // Func
[54] = NULL, // Func
[55] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(const char[256]), .align = alignof(const char[256]), .base = TYPEID(16, TYPE_CONST, const char), .count = 256 },
[56] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[11]), .align = alignof(int[11]), .base = TYPEID(8, TYPE_INT, int), .count = 11 },
[57] = NULL, // Func
[58] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_Vector), .align = alignof(test1_Vector), .name = "test1_Vector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, x)},
{"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, y)},}
},
[59] = NULL, // Func
[60] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_T), .align = alignof(test1_T), .name = "test1_T", .num_fields = 1, .fields = (TypeFieldInfo[]) {
{"a", .type = TYPEID(89, TYPE_ARRAY, int[9]), .offset = offsetof(test1_T, a)},}
},
[61] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(60, TYPE_STRUCT, test1_T) },
[62] = NULL, // Enum
[63] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(const char* [4]), .align = alignof(const char* [4]), .base = TYPEID(17, TYPE_PTR, const char*), .count = 4 },
[64] = NULL, // Func
[65] = NULL, // Func
[66] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = TYPEID(8, TYPE_INT, int) },
[67] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(66, TYPE_CONST, const int) },
[68] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const test1_Vector), .align = alignof(const test1_Vector), .base = TYPEID(58, TYPE_STRUCT, test1_Vector) },
[69] = NULL, // Func
[70] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_ConstVector), .align = alignof(test1_ConstVector), .name = "test1_ConstVector", .num_fields = 2, .fields = (TypeFieldInfo[]) {
{"x", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, x)},
{"y", .type = TYPEID(66, TYPE_CONST, const int), .offset = offsetof(test1_ConstVector, y)},}
},
[71] = NULL, // Func
[72] = NULL, // Func
[73] = NULL, // Func
[74] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_Ints), .align = alignof(test1_Ints), .name = "test1_Ints", .num_fields = 3, .fields = (TypeFieldInfo[]) {
{"num_ints", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Ints, num_ints)},
{"int_ptr", .type = TYPEID(32, TYPE_PTR, int*), .offset = offsetof(test1_Ints, int_ptr)},
{"int_arr", .type = TYPEID(34, TYPE_ARRAY, int[3]), .offset = offsetof(test1_Ints, int_arr)},}
},
[75] = NULL, // Func
[76] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_BufHdr), .align = alignof(test1_BufHdr), .name = "test1_BufHdr", .num_fields = 3, .fields = (TypeFieldInfo[]) {
{"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, cap)},
{"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, len)},
{"buf", .type = TYPEID(116, TYPE_ARRAY, char[1]), .offset = offsetof(test1_BufHdr, buf)},}
},
[77] = &(TypeInfo) {
TYPE_STRUCT, .size = sizeof(test1_Thing), .align = alignof(test1_Thing), .name = "test1_Thing", .num_fields = 1, .fields = (TypeFieldInfo[]) {
{"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Thing, a)},}
},
[78] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(77, TYPE_STRUCT, test1_Thing) },
[79] = NULL, // Func
[80] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const test1_Thing), .align = alignof(const test1_Thing), .base = TYPEID(77, TYPE_STRUCT, test1_Thing) },
[81] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(80, TYPE_CONST, const test1_Thing) },
[82] = NULL, // Func
[83] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(18, TYPE_PTR, char*) },
[84] = NULL, // Func
[85] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(58, TYPE_STRUCT, test1_Vector) },
[86] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[16]), .align = alignof(int[16]), .base = TYPEID(8, TYPE_INT, int), .count = 16 },
[87] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(7, TYPE_USHORT, ushort) },
[88] = NULL, // Incomplete array type
[89] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[9]), .align = alignof(int[9]), .base = TYPEID(8, TYPE_INT, int), .count = 9 },
[90] = NULL, // Func
[91] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[4]), .align = alignof(int[4]), .base = TYPEID(8, TYPE_INT, int), .count = 4 },
[92] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const float), .align = alignof(const float), .base = TYPEID(14, TYPE_FLOAT, float) },
[93] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(92, TYPE_CONST, const float) },
[94] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const TypeKind), .align = alignof(const TypeKind), .base = TYPEID(19, TYPE_NONE, TypeKind) },
[95] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const ullong), .align = alignof(const ullong), .base = TYPEID(13, TYPE_ULLONG, ullong) },
[96] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const const char*), .align = alignof(const const char*), .base = TYPEID(17, TYPE_PTR, const char*) },
[97] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const TypeFieldInfo*), .align = alignof(const TypeFieldInfo*), .base = TYPEID(22, TYPE_PTR, TypeFieldInfo*) },
[98] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(14, TYPE_FLOAT, float) },
[99] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(31, TYPE_PTR, void*) },
[100] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(const int* [42]), .align = alignof(const int* [42]), .base = TYPEID(67, TYPE_PTR, const int*), .count = 42 },
[101] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(50, TYPE_UNION, test1_IntOrPtr) },
[102] = &(TypeInfo) { TYPE_CONST, .size = 0, .align = 0, .base = TYPEID0(1, TYPE_VOID) },
[103] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID0(102, TYPE_CONST) },
[104] = NULL, // Func
[105] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(17, TYPE_PTR, const char*) },
[106] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const char*), .align = alignof(const char*), .base = TYPEID(18, TYPE_PTR, char*) },
[107] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(106, TYPE_CONST, const char*) },
[108] = NULL, // Func
[109] = NULL, // Func
[110] = &(TypeInfo) { TYPE_PTR, .size = sizeof(void*), .align = alignof(void*), .base = TYPEID(12, TYPE_LLONG, llong) },
[111] = NULL, // Func
[112] = &(TypeInfo) { TYPE_CONST, .size = sizeof(const Any), .align = alignof(const Any), .base = TYPEID(30, TYPE_STRUCT, Any) },
[113] = NULL, // Incomplete array type
[114] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(int[2]), .align = alignof(int[2]), .base = TYPEID(8, TYPE_INT, int), .count = 2 },
[115] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(test1_Ints[2]), .align = alignof(test1_Ints[2]), .base = TYPEID(74, TYPE_STRUCT, test1_Ints), .count = 2 },
[116] = &(TypeInfo) { TYPE_ARRAY, .size = sizeof(char[1]), .align = alignof(char[1]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1 },
};
int num_typeinfos = 117;
const TypeInfo** typeinfos = (const TypeInfo**)typeinfo_table;

// Definitions
const char(*IONOS) = "win32";

const char(*IONARCH) = "x64";

TypeKind typeid_kind(typeid type) {
	return (TypeKind)((((type) >> (24)))& (0xFF));
}

int typeid_index(typeid type) {
	return (int)((type) & (0xFFFFFF));
}

size_t typeid_size(typeid type) {
	return (size_t)((type) >> (32));
}

const TypeInfo* get_typeinfo(typeid type) {
	int index = typeid_index(type);
	if ((typeinfos) && ((index) < (num_typeinfos))) {
		return typeinfos[index];
	}
	else {
		return NULL;
	}
}

char(*test1_esc_test_str) = "Hello\nworld\nHex: \xFHello\xFF";

int(*test1_some_array) = (int[]){ 1, 2, 3 };

test1_SomeIncompleteType(*test1_incomplete_ptr);

char test1_c = 1;

uchar test1_uc = 1;

schar test1_sc = 1;

uchar test1_h(void) {
	((test1_Vector) { .x = 1, .y = 2 }.x) = 42;
	test1_Vector(*v) = &((test1_Vector) { 1, 2 });
	(v->x) = 42;
	int(*p) = &((int) { 0 });
	ulong x = ((uint) { 1 }) + ((long) { 2 });
	int y = +(test1_c);
	return (uchar)(x);
}

char(*test1_code) =
"\n"
"#include <stdio.h>\n"
"\n"
"int main(int argc, char **argv) {\n"
"    printf(\"Hello, world!\\n\");\n"
"    return 0;\n"
"}\n";

void test1_test_packages(void) {
	test1_subtest1_func1();
}

void test1_test_modify(void) {
	int i = 42;
	int(*p) = &(i);
	(p)--;
	int x = *((p)++);
	assert((x) == (*(--(p))));
	((*(p)))++;
	((*(p)))--;
	int(stk[16]) = { 0 };
	int(*sp) = stk;
	(*((sp)++)) = 1;
	(*((sp)++)) = 2;
	(x) = *(--(sp));
	assert((x) == (2));
	(x) = *(--(sp));
	assert((x) == (1));
	assert((sp) == (stk));
}

void test1_f10(wchar_t(a[3])) {
	(a[1]) = 42;
}

void test1_test_arrays(void) {
	wchar_t(a[]) = { 1, 2, 3 };
	test1_f10(a);
	ushort(*b) = a;
	wchar_t w1 = { 0 };
	ushort w2 = w1;
}

void test1_test_loops(void) {
	switch (0) {
	default: {
		if (1) {
			break;
		}
		for (;;) {
			continue;
		}
		break;
	}
	}
	while (0) {
	}
	for (int i = 0; (i) < (10); (i)++) {
	}
	for (;;) {
		break;
	}
	for (int i = 0;;) {
		break;
	}
	for (; 0;) {
	}
	for (int i = 0;; (i)++) {
		break;
	}
	int i = 0;
	for (;; (i)++) {
		break;
	}
}

void test1_test_nonmodifiable(void) {
	test1_S1 s1 = { 0 };
	(s1.a) = 0;
	test1_S2 s2 = { 0 };
	(s2.s1.a) = 0;
}

uint32_t test1_pack(test1_UartCtrl ctrl) {
	return (((ctrl.tx_enable) & (1))) | (((((ctrl.rx_enable) & (1))) << (1)));
}

test1_UartCtrl test1_unpack(uint32_t word) {
	return (test1_UartCtrl) { .tx_enable = (word) & (0x1), .rx_enable = (((word) & (0x2))) >> (1) };
}

void test1_test_uart(void) {
	bool tx_enable = test1_unpack(*(test1_UART_CTRL_REG)).tx_enable;
	(*(test1_UART_CTRL_REG)) = test1_pack((test1_UartCtrl) { .tx_enable = !(tx_enable), .rx_enable = false });
	test1_UartCtrl ctrl = test1_unpack(*(test1_UART_CTRL_REG));
	(ctrl.rx_enable) = true;
	(*(test1_UART_CTRL_REG)) = test1_pack(ctrl);
}

int test1_g(test1_U u) {
	return u.i;
}

void test1_k(void(*vp), int(*ip)) {
	(vp) = ip;
	(ip) = vp;
}

void test1_f1(void) {
	int(*p) = &((int) { 0 });
	(*(p)) = 42;
}

void test1_f3(int(a[])) {
}

int test1_example_test(void) {
	return (test1_fact_rec(10)) == (test1_fact_iter(10));
}

const char(test1_escape_to_char[256]) = { ['n'] = '\n',['r'] = '\r',['t'] = '\t',['v'] = '\v',['b'] = '\b',['a'] = '\a',['0'] = 0 };

int(test1_a2[11]) = { 1, 2, 3,[10] = 4 };

int test1_is_even(int digit) {
	int b = 0;
	switch (digit) {
	case 0:
	case 2:
	case 4:
	case 6:
	case 8: {
		(b) = 1;
		break;
	}
	}
	return b;
}

int test1_i;

void test1_f2(test1_Vector v) {
	(v) = (test1_Vector){ 0 };
}

int test1_fact_iter(int n) {
	int r = 1;
	for (int i = 0; (i) <= (n); (i)++) {
		(r) *= i;
	}
	return r;
}

int test1_fact_rec(int n) {
	if ((n) == (0)) {
		return 1;
	}
	else {
		return (n) * (test1_fact_rec((n)-(1)));
	}
}

test1_T(*test1_p);

const char* (test1_color_names[test1_NUM_COLORS]) = { [test1_COLOR_NONE] = "none",[test1_COLOR_RED] = "red",[test1_COLOR_GREEN] = "green",[test1_COLOR_BLUE] = "blue" };

void test1_test_enum(void) {
	test1_Color a = test1_COLOR_RED;
	int b = test1_COLOR_RED;
	int c = (a)+(b);
	int i = a;
	(a) = i;
	printf("%d %d %d %d\n", test1_COLOR_NONE, test1_COLOR_RED, test1_COLOR_GREEN, test1_COLOR_BLUE);
}

void test1_test_assign(void) {
	int i = 0;
	float f = 3.14f;
	int(*p) = &(i);
	(i)++;
	(i)--;
	(p)++;
	(p)--;
	(p) += 1;
	(i) /= 2;
	(i) *= 123;
	(i) %= 3;
	(i) <<= 1;
	(i) >>= 2;
	(i) &= 0xFF;
	(i) |= 0xFF00;
	(i) ^= 0xFF0;
}

void test1_benchmark(int n) {
	int r = 1;
	for (int i = 1; (i) <= (n); (i)++) {
		(r) *= i;
	}
}

int test1_va_test(int x, ...) {
	return 0;
}

void test1_test_lits(void) {
	float f = 3.14f;
	double d = 3.14;
	int i = 1;
	uint u = 0xFFFFFFFFu;
	long l = 1l;
	ulong ul = 1ul;
	llong ll = 0x100000000ll;
	ullong ull = 0xFFFFFFFFFFFFFFFFull;
	uint x1 = 0xFFFFFFFF;
	llong x2 = 4294967295;
	ullong x3 = 0xFFFFFFFFFFFFFFFF;
	int x4 = (0xAA) + (0x55);
}

void test1_test_ops(void) {
	float pi = 3.14f;
	float f = 0.0f;
	(f) = +(pi);
	(f) = -(pi);
	int n = -(1);
	(n) = ~(n);
	(f) = ((f) * (pi)) + (n);
	(f) = (pi) / (pi);
	(n) = (3) % (2);
	(n) = (n)+((uchar)(1));
	int(*p) = &(n);
	(p) = (p)+(1);
	(n) = (int)((((p)+(1))) - (p));
	(n) = (n) << (1);
	(n) = (n) >> (1);
	int b = ((p)+(1)) > (p);
	(b) = ((p)+(1)) >= (p);
	(b) = ((p)+(1)) < (p);
	(b) = ((p)+(1)) <= (p);
	(b) = ((p)+(1)) == (p);
	(b) = (1) > (2);
	(b) = (1.23f) <= (pi);
	(n) = 0xFF;
	(b) = (n) & (~(1));
	(b) = (n) & (1);
	(b) = (((n) & (~(1)))) ^ (1);
	(b) = (p) && (pi);
}

void test1_test_bool(void) {
	bool b = false;
	(b) = true;
	int i = 0;
	(i) = test1_IS_DEBUG;
}

int test1_test_ctrl(void) {
	switch (1) {
	case 0: {
		return 0;
		break;
	}default: {
		return 1;
		break;
	}
	}
}

const int(test1_j);

const int(*test1_q);

const test1_Vector(test1_cv);

void test1_f4(const char(*x)) {
}

void test1_f5(const int(*p)) {
}

void test1_test_convert(void) {
	const int(*a) = 0;
	int(*b) = 0;
	(a) = b;
	void(*p) = 0;
	test1_f5(p);
}

void test1_test_const(void) {
	test1_ConstVector cv2 = { 1, 2 };
	int i = 0;
	(i) = 1;
	int x = test1_cv.x;
	char c = test1_escape_to_char[0];
	test1_f4(test1_escape_to_char);
	const char(*p) = (const char*)(0);
	(p) = (test1_escape_to_char)+(1);
	char(*q) = (char*)(test1_escape_to_char);
	(c) = q['n'];
	(p) = (const char*)(1);
	(i) = (int)((ullong)(p));
}

void test1_test_init(void) {
	int x = (const int)(0);
	int y = { 0 };
	(y) = 0;
	int z = 42;
	int(a[]) = { 1, 2, 3 };
	for (ullong i = 0; (i) < (10); (i)++) {
		printf("%llu\n", i);
	}
	int(b[4]) = { 1, 2, 3, 4 };
	(b[0]) = a[2];
}

void test1_test_sizeof(void) {
	int i = 0;
	ullong n = sizeof(i);
	(n) = sizeof(int);
	(n) = sizeof(int);
	(n) = sizeof(int*);
}

void test1_test_cast(void) {
	int(*p) = 0;
	uint64_t a = 0;
	(a) = (uint64_t)(p);
	(p) = (int*)(a);
}

void test1_print_any(Any any) {
	switch (any.type) {
	case TYPEID(8, TYPE_INT, int): {
		printf("%d", *((const int*)(any.ptr)));
		break;
	}
	case TYPEID(14, TYPE_FLOAT, float): {
		printf("%f", *((const float*)(any.ptr)));
		break;
	}default: {
		printf("<unknown>");
		break;
	}
	}
	printf(": ");
	test1_print_type(any.type);
}

void test1_println_any(Any any) {
	test1_print_any(any);
	printf("\n");
}

void test1_print_typeid(typeid type) {
	int index = typeid_index(type);
	printf("typeid(%d)", index);
}

void test1_print_type(typeid type) {
	const TypeInfo(*typeinfo) = get_typeinfo(type);
	if (!(typeinfo)) {
		test1_print_typeid(type);
		return;
	}
	switch (typeinfo->kind) {
	case TYPE_PTR: {
		test1_print_type(typeinfo->base);
		printf("*");
		break;
	}
	case TYPE_CONST: {
		test1_print_type(typeinfo->base);
		printf(" const");
		break;
	}
	case TYPE_ARRAY: {
		test1_print_type(typeinfo->base);
		printf("[%d]", typeinfo->count);
		break;
	}default: {
		if (typeinfo->name) {
			printf("%s", typeinfo->name);
		}
		else {
			test1_print_typeid(type);
		}
		break;
	}
	}
}

void test1_println_type(typeid type) {
	test1_print_type(type);
	printf("\n");
}

void test1_print_typeinfo(typeid type) {
	const TypeInfo(*typeinfo) = get_typeinfo(type);
	if (!(typeinfo)) {
		test1_print_typeid(type);
		return;
	}
	printf("<");
	test1_print_type(type);
	printf(" size=%d align=%d", typeinfo->size, typeinfo->align);
	switch (typeinfo->kind) {
	case TYPE_STRUCT:
	case TYPE_UNION: {
		printf(" %s={ ", ((typeinfo->kind) == (TYPE_STRUCT) ? "struct" : "union"));
		for (int i = 0; (i) < (typeinfo->num_fields); (i)++) {
			TypeFieldInfo field = typeinfo->fields[i];
			printf("@offset(%d) %s: ", field.offset, field.name);
			test1_print_type(field.type);
			printf("; ");
		}
		printf("}");
		break;
	}
	}
	printf(">");
}

void test1_println_typeinfo(typeid type) {
	test1_print_typeinfo(type);
	printf("\n");
}

void test1_test_typeinfo(void) {
	int i = 42;
	float f = 3.14f;
	void(*p) = NULL;
	test1_println_any((Any) { &(i), TYPEID(8, TYPE_INT, int) });
	test1_println_any((Any) { &(f), TYPEID(14, TYPE_FLOAT, float) });
	test1_println_any((Any) { &(p), TYPEID(31, TYPE_PTR, void*) });
	test1_println_type(TYPEID(8, TYPE_INT, int));
	test1_println_type(TYPEID(67, TYPE_PTR, const int*));
	test1_println_type(TYPEID(100, TYPE_ARRAY, const int* [42]));
	test1_println_type(TYPEID(46, TYPE_STRUCT, test1_UartCtrl));
	test1_println_typeinfo(TYPEID(8, TYPE_INT, int));
	test1_println_typeinfo(TYPEID(46, TYPE_STRUCT, test1_UartCtrl));
	test1_println_typeinfo(TYPEID(101, TYPE_PTR, test1_IntOrPtr*));
	test1_println_typeinfo(TYPEID(50, TYPE_UNION, test1_IntOrPtr));
}

void test1_test_va_list(const char(*fmt), ...) {
	va_list init_args = { 0 };
	va_start_ptr(&(init_args), &(fmt));
	va_list args = { 0 };
	va_copy_ptr(&(args), &(init_args));
	char c = { 0 };
	va_arg_ptr(&(args), &(c), TYPEID(3, TYPE_CHAR, char));
	int i = { 0 };
	va_arg_ptr(&(args), &(i), TYPEID(8, TYPE_INT, int));
	llong ll = { 0 };
	va_arg_ptr(&(args), &(ll), TYPEID(12, TYPE_LLONG, llong));
	printf("c=%d i=%d ll=%lld\n", c, i, ll);
	va_end_ptr(&(args));
}

void test1_test_compound_literals(void) {
	test1_Vector(*w) = { 0 };
	(w) = &((test1_Vector) { 1, 2 });
	int(a[3]) = { 1, 2, 3 };
	int i = 42;
	const Any(x) = { &(i), TYPEID(8, TYPE_INT, int) };
	Any y = { &(i), TYPEID(8, TYPE_INT, int) };
	test1_Ints v = { .num_ints = 3, .int_ptr = (int[]){1, 2, 3}, .int_arr = {1, 2, 3} };
	test1_Ints(ints_of_ints[]) = { {.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int[2]){-(1), -(2)}} };
}

void test1_test_complete(void) {
	int x = 0;
	int y = 0;
	if ((x) == (0)) {
		(y) = 1;
	}
	else if ((x) == (1)) {
		(y) = 2;
	}
	else {
		assert("@complete if/elseif chain failed to handle case" && 0);
	}
	(x) = 1;
	assert((x) >= (0));
	(x) = 0;
	switch (x) {
	case 0: {
		(y) = 3;
		break;
	}
	case 1: {
		(y) = 4;
		break;
	}default:
		assert("@complete switch failed to handle case" && 0);
		break;
	}
}

void test1_test_alignof(void) {
	int i = 42;
	ullong n1 = alignof(int);
	ullong n2 = alignof(int);
	ullong n3 = alignof(ullong);
	ullong n4 = alignof(int*);
}

void test1_test_offsetof(void) {
	ullong n = offsetof(test1_BufHdr, buf);
}

test1_Thing test1_thing;

test1_Thing* test1_returns_ptr(void) {
	return &(test1_thing);
}

const test1_Thing* test1_returns_ptr_to_const(void) {
	return &(test1_thing);
}

void test1_test_lvalue(void) {
	(test1_returns_ptr()->a) = 5;
	const test1_Thing(*p) = test1_returns_ptr_to_const();
}

void test1_test_if(void) {
	if (1) {
	}
	{
		int x = 42;
		if (x) {
		}
	}
	{
		int x = 42;
		if ((x) >= (0)) {
		}
	}
	{
		int x = 42;
		if ((x) >= (0)) {
		}
	}
}

void test1_test_reachable(void) {
}

void test1_test_os_arch(void) {
	printf("Target operating system: %s\n", IONOS);
	printf("Target machine architecture: %s\n", IONARCH);
}

int main(int argc, char* (*argv)) {
	if ((argv) == (0)) {
		printf("argv is null\n");
	}
	test1_test_va_list("whatever", (char)(123), (int)(123123), (llong)(123123123123));
	test1_test_os_arch();
	test1_test_packages();
	test1_test_if();
	test1_test_modify();
	test1_test_lvalue();
	test1_test_alignof();
	test1_test_offsetof();
	test1_test_complete();
	test1_test_compound_literals();
	test1_test_loops();
	test1_test_sizeof();
	test1_test_assign();
	test1_test_enum();
	test1_test_arrays();
	test1_test_cast();
	test1_test_init();
	test1_test_lits();
	test1_test_const();
	test1_test_bool();
	test1_test_ops();
	test1_test_typeinfo();
	test1_test_reachable();
	getchar();
	return 0;
}

int test1_subtest1_func1(void) {
	test1_subtest1_func2();
	return 42;
}

void test1_subtest1_func2(void) {
	test1_subtest1_func3();
	test1_subtest1_func4();
}

void test1_subtest1_func3(void) {
	printf("func3\n");
}

void test1_subtest1_func4(void) {
}

// Foreign source files

// Postamble
void va_arg_ptr(va_list* args, void* arg, ullong type) {
	switch (typeid_kind(type)) {
	case TYPE_BOOL:
		*(bool*)arg = va_arg(*args, int);
		break;
	case TYPE_CHAR:
		*(char*)arg = va_arg(*args, int);
		break;
	case TYPE_UCHAR:
		*(uchar*)arg = va_arg(*args, int);
		break;
	case TYPE_SCHAR:
		*(schar*)arg = va_arg(*args, int);
		break;
	case TYPE_SHORT:
		*(short*)arg = va_arg(*args, int);
		break;
	case TYPE_USHORT:
		*(ushort*)arg = va_arg(*args, int);
		break;
	case TYPE_INT:
		*(int*)arg = va_arg(*args, int);
		break;
	case TYPE_UINT:
		*(uint*)arg = va_arg(*args, uint);
		break;
	case TYPE_LONG:
		*(long*)arg = va_arg(*args, long);
		break;
	case TYPE_ULONG:
		*(ulong*)arg = va_arg(*args, ulong);
		break;
	case TYPE_LLONG:
		*(llong*)arg = va_arg(*args, llong);
		break;
	case TYPE_ULLONG:
		*(ullong*)arg = va_arg(*args, ullong);
		break;
	case TYPE_FLOAT:
		*(float*)arg = va_arg(*args, double);
		break;
	case TYPE_DOUBLE:
		*(double*)arg = va_arg(*args, double);
		break;
	case TYPE_FUNC:
	case TYPE_PTR:
		*(void**)arg = va_arg(*args, void*);
		break;
	default:
		assert(0 && "argument type not supported");
		break;
	}
}
