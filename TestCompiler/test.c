#define __USE_MINGW_ANSI_STDIO 1
#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif
#ifndef _CRT_NONSTDC_NO_DEPRECATE
#define _CRT_NONSTDC_NO_DEPRECATE
#endif

#if _MSC_VER >= 1900 || __STDC_VERSION__ >= 201112L
// Visual Studio 2015 supports enough C99/C11 features for us.
#else
#error C11 support required or Visual Studio 2015 or later
#endif

#if _MSC_VER
#define THREADLOCAL __declspec(thread)
#define INLINE static inline __forceinline
#define NOINLINE __declspec(noinline)
#endif

#if __GNUC__
#define THREADLOCAL __thread
#define INLINE static inline __attribute__((always_inline))
#define NOINLINE __attribute__((noinline))
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wvarargs"
#endif

#include <stdbool.h>
#include <stdint.h>
#include <stddef.h>
#include <stdarg.h>
#include <assert.h>
#include <string.h>
#include <stdlib.h>

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

typedef void *(*AllocFunc)(void *data, size_t size, size_t align);
typedef void (*FreeFunc)(void *data, void *ptr);

typedef struct Allocator {
    AllocFunc alloc;
    FreeFunc free;
} Allocator;

INLINE
void *default_alloc(void *allocator, size_t size, size_t align) {
    // todo: use _aligned_malloc, etc
    return malloc(size);
}

INLINE
void default_free(void *allocator, void *ptr) {
    // todo: use _aligned_free, etc
    free(ptr);
}

THREADLOCAL
Allocator *current_allocator = &(Allocator){default_alloc, default_free};

INLINE
void *generic_alloc(Allocator *allocator, size_t size, size_t align) {
    if (!size) {
        return 0;
    }
    if (!allocator) {
        allocator = current_allocator;
    } 
    return allocator->alloc(allocator, size, align);
}

INLINE
void generic_free(Allocator *allocator, void *ptr) {
    if (!allocator) {
        allocator = current_allocator;
    } 
    allocator->free(allocator, ptr);
}

INLINE
void *generic_alloc_copy(Allocator *allocator, size_t size, size_t align, const void *src) {
    if (!allocator) {
        allocator = current_allocator;
    } 
    void *ptr = allocator->alloc(allocator, size, align);
    if (!ptr) {
        return 0;
    }
    memcpy(ptr, src, size);
    return ptr;
}

#define tls_alloc(size, align) (generic_alloc(current_allocator, (size), (align)))
#define tls_free(ptr) (generic_free(current_allocator, (ptr)))
#define alloc_copy(size, align, src) (generic_alloc_copy(current_allocator, (size), (align), (src)))

typedef struct ForeignStruct { int x; const int y; } ForeignStruct;
typedef struct tm tm_t;
#define anew(t, allocator) (std_anew_func((allocator), sizeof(t), alignof(t)))
#define apush(t, a, v) (std_apush_func((void **)&(a), (t[]){(v)}, sizeof(t), alignof(t)))
#define alen(t, a) std_alen_func((a), sizeof(t), alignof(t))
#define acap(t, a) std_acap_func((a), sizeof(t), alignof(t))
#define asetcap(t, a, n) std_asetcap_func((void **)&(a), (n), sizeof(t), alignof(t))
#define apop(t, a) std_apop_func((a), sizeof(t), alignof(t))
#define acat(t, a, b) std_acat_func(&(a), (b), sizeof(t), alignof(t))
#define afree(t, a) std_afree_func((void **)&(a), sizeof(t), alignof(t))
#define afill(t, a, v, n) std_afill_func((void **)&(a), (t[]){(v)}, (n), sizeof(t), alignof(t))
#define acatn(t, a, b, n) std_acatn_func(&(a), (b), (n), sizeof(t), alignof(t))
#define adeli(t, a, i) std_adeln_func((a), (i), 1, sizeof(t), alignof(t))
#define adeln(t, a, i, n) std_adeln_func((a), (i), (n), sizeof(t), alignof(t))
#define aprintf(a, fmt, ...) std_aprintf_func(&(a), (fmt), ##__VA_ARGS__)
#define aindexv(t, a, new_index) std_aindex_func(&(a), (new_index), sizeof(t), sizeof(t), alignof(t))
#define aputv(t, a, v) std_aput_func(&(a), (t[]){(v)}, sizeof(t), sizeof(t), alignof(t))
#define adelv(t, a, v) std_adel_func((a), (t[]){(v)}, sizeof(t), sizeof(t), alignof(t))
#define agetvi(t, a, v) std_ageti_func((a), (t[]){(v)}, sizeof(t), sizeof(t), alignof(t))
#define agetv(t, a, v) (std_agetp_func((a), (t[]){(v)}, sizeof(t), sizeof(t), alignof(t)) != NULL)
#define aindex(t, tk, a, new_index) std_aindex_func(&(a), (new_index), sizeof(tk), sizeof(t), alignof(t))
#define aput(t, tk, a, k, v) std_aput_func(&(a), &(t){(k), (v)}, sizeof(tk), sizeof(t), alignof(t))
#define adel(t, tk, a, k) std_adel_func((a), (tk[]){(k)}, sizeof(tk), sizeof(t), alignof(t))
#define ageti(t, tk, a, k) std_ageti_func((a), (t[]){(k)}, sizeof(tk), sizeof(t), alignof(t))
int const *foreign_func(void) { return 0; }
#define recover(ctx) (std_temp_ctx = (ctx), std_temp_ctx->base = std_make_disposable(std_recover_dispose), setjmp(std_temp_ctx->libc_env) == 0)
#define aget(t, tk, tv, a, k) (*(tv *)std_aget_func(&(a), (tk[]){(k)}, sizeof(tk), sizeof(t), alignof(t)))
#define adefault(t, tv, a, v) std_asetdefault_func(&(a), (t[]){(v)}, sizeof(tv), sizeof(t), alignof(t))
#define agetp(t, tk, tv, a, k) ((tv *)std_agetp_func((a), (tk[]){(k)}, sizeof(tk), sizeof(t), alignof(t)))
#define ahdr(t, a) std_ahdr_func((a), sizeof(t), alignof(t))
#define asetlen(t, a, new_len) std_asetlen_func((a), (new_len), sizeof(t), alignof(t))

// -Foreign Headers-
#include <limits.h>
#include <stdint.h>
#include <ctype.h>
#include <errno.h>
#include <setjmp.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

// -Forward Declerations-
typedef struct tuple139 tuple139;
typedef struct tuple143 tuple143;
typedef struct tuple146 tuple146;
typedef struct tuple149 tuple149;
typedef struct tuple162 tuple162;
typedef struct tuple198 tuple198;
typedef struct tuple202 tuple202;
typedef struct tuple209 tuple209;
typedef struct any any;
typedef struct TypeFieldInfo TypeFieldInfo;
typedef struct TypeInfo TypeInfo;
typedef struct test1_S1 test1_S1;
typedef struct test1_S2 test1_S2;
typedef struct test1_Bar test1_Bar;
typedef struct test1_Foo test1_Foo;
typedef struct test1_Thing test1_Thing;
typedef struct test1_BufHdr test1_BufHdr;
typedef struct test1_Vector test1_Vector;
typedef struct test1_Ints test1_Ints;
typedef struct test1_ConstVector test1_ConstVector;
typedef struct test1_float2 test1_float2;
typedef struct std_TempAllocator std_TempAllocator;
typedef struct std_Index std_Index;
typedef struct test1_FuncPair test1_FuncPair;
typedef struct std_ArenaAllocator std_ArenaAllocator;
typedef struct std_NameMap std_NameMap;
typedef struct test1_Person test1_Person;
typedef struct std_AllocatorEvent std_AllocatorEvent;
typedef struct std_TraceAllocator std_TraceAllocator;
typedef struct std_TempMark std_TempMark;
typedef struct std_Disposable std_Disposable;
typedef struct std_Recover std_Recover;
typedef struct std_File std_File;
typedef struct test1_SillyStruct test1_SillyStruct;
typedef struct std_Ahdr std_Ahdr;
typedef struct std_Indexer std_Indexer;
typedef struct test1_UartCtrl test1_UartCtrl;
typedef union test1_IntOrPtr test1_IntOrPtr;
typedef struct std_HashSlot std_HashSlot;
typedef struct std_HashIndex std_HashIndex;
typedef struct test1_Node test1_Node;
typedef struct std_NameNode std_NameNode;

// -Sorted Declerations-
struct tuple139 {char _0;int _1;};
struct tuple143 {int _0;int _1;int _2;};
struct tuple146 {float _0;float _1;};
struct tuple149 {test1_Node (*_0);int _1;};
struct tuple162 {ullong _0;std_NameNode (*_1);};
struct tuple198 {int _0;int _1;};
struct tuple202 {double _0;double _1;};
struct tuple209 {int _0;int _1;};

#line 1307 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
int main(int argc, char * (*argv));

#line 1 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/config_win32.ion"
extern char (*IONOS);

#line 1 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/config_x64.ion"
extern char (*IONARCH);

#line 1 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/typeinfo.ion"
typedef ullong typeid;

typedef int TypeKind;

#line 4
#define TYPE_NONE ((TypeKind)(0))

#line 5
#define TYPE_VOID ((TypeKind)((TYPE_NONE) + (1)))

#line 6
#define TYPE_BOOL ((TypeKind)((TYPE_VOID) + (1)))

#line 7
#define TYPE_CHAR ((TypeKind)((TYPE_BOOL) + (1)))

#line 8
#define TYPE_UCHAR ((TypeKind)((TYPE_CHAR) + (1)))

#line 9
#define TYPE_SCHAR ((TypeKind)((TYPE_UCHAR) + (1)))

#line 10
#define TYPE_SHORT ((TypeKind)((TYPE_SCHAR) + (1)))

#line 11
#define TYPE_USHORT ((TypeKind)((TYPE_SHORT) + (1)))

#line 12
#define TYPE_INT ((TypeKind)((TYPE_USHORT) + (1)))

#line 13
#define TYPE_UINT ((TypeKind)((TYPE_INT) + (1)))

#line 14
#define TYPE_LONG ((TypeKind)((TYPE_UINT) + (1)))

#line 15
#define TYPE_ULONG ((TypeKind)((TYPE_LONG) + (1)))

#line 16
#define TYPE_LLONG ((TypeKind)((TYPE_ULONG) + (1)))

#line 17
#define TYPE_ULLONG ((TypeKind)((TYPE_LLONG) + (1)))

#line 18
#define TYPE_FLOAT ((TypeKind)((TYPE_ULLONG) + (1)))

#line 19
#define TYPE_DOUBLE ((TypeKind)((TYPE_FLOAT) + (1)))

#line 20
#define TYPE_CONST ((TypeKind)((TYPE_DOUBLE) + (1)))

#line 21
#define TYPE_PTR ((TypeKind)((TYPE_CONST) + (1)))

#line 22
#define TYPE_ARRAY ((TypeKind)((TYPE_PTR) + (1)))

#line 23
#define TYPE_STRUCT ((TypeKind)((TYPE_ARRAY) + (1)))

#line 24
#define TYPE_UNION ((TypeKind)((TYPE_STRUCT) + (1)))

#line 25
#define TYPE_FUNC ((TypeKind)((TYPE_UNION) + (1)))

#line 26
#define TYPE_TUPLE ((TypeKind)((TYPE_FUNC) + (1)))

#line 52
TypeKind typeid_kind(ullong type);

#line 56
int typeid_index(ullong type);

#line 60
ullong typeid_size(ullong type);

#line 64
TypeInfo * get_typeinfo(ullong type);

#line 65 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/types.ion"
#define UCHAR_MIN ((uchar)(0))

#line 88
#define USHORT_MIN ((short)(0))

#line 99
#define UINT_MIN ((uint)(0))

#line 110
#define ULLONG_MIN ((ullong)(0))

#line 115
#define UINT8_MIN (UCHAR_MIN)

#line 126
#define UINT16_MIN (USHORT_MIN)

#line 137
#define UINT32_MIN (UINT_MIN)

#line 148
#define UINT64_MIN (ULLONG_MIN)

#line 19 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/types_win32.ion"
#define ULONG_MIN ((ulong)(INT32_MIN))

#line 15 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/types_x64.ion"
#define USIZE_MIN (UINT64_MIN)

#line 26
#define UINTPTR_MIN (UINT64_MIN)

#line 73 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/typeinfo.ion"
struct any {
    void (*ptr);
    typeid type;
};

#line 1 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"

#line 137 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_test_nonmodifiable(void);

#line 883
void test1_test_const_ptr_deref(void);

#line 745
void test1_test_limits(void);

#line 680
void test1_test_va_list(char (*fmt), ...);

#line 870
void test1_test_os_arch(void);

#line 62
void test1_test_packages(void);

#line 853
void test1_test_if(void);

#line 66
void test1_test_modify(void);

#line 833
void test1_test_lvalue(void);

#line 799
void test1_test_alignof(void);

#line 814
void test1_test_offsetof(void);

#line 717
void test1_test_complete(void);

#line 694
void test1_test_compound_literals(void);

#line 100
void test1_test_loops(void);

#line 493
void test1_test_sizeof(void);

#line 329
void test1_test_assign(void);

#line 315
void test1_test_enum(void);

#line 91
void test1_test_arrays(void);

#line 501
void test1_test_cast(void);

#line 477
void test1_test_init(void);

#line 363
void test1_test_lits(void);

#line 451
void test1_test_const(void);

#line 412
void test1_test_bool(void);

#line 380
void test1_test_ops(void);

#line 642
void test1_test_typeinfo(void);

#line 866
void test1_test_reachable(void);

#line 916
void test1_test_type_path(void);

#line 636
void test1_test_push(void);

#line 676
void test1_test_va_type(char (*fmt), ...);

#line 672
void test1_test_void(void);

#line 934
void test1_test_dynamic_arrays(void);

#line 922
void test1_test_aprintf(void);

#line 983
void test1_test_index_arrays(void);

#line 1058
void test1_test_tuples(void);

#line 1075
void test1_test_func_interning(void);

#line 1035
void test1_test_hashing(void);

#line 1081
void test1_test_void_ptr_arithmetic(void);

#line 1110
void test1_test_foreign_const(void);

#line 1117
void test1_test_const_implicit(void);

#line 1123
void test1_test_intern(void);

#line 1133
void test1_test_namemap(void);

#line 1164
void test1_test_threadlocal(void);

#line 1188
void test1_test_new(void);

#line 1242
std_File * test1_test_disposable(void);

#line 1147
void test1_test_aget(void);

#line 1237
void test1_test_tuple_deps(void);

#line 1260
void test1_test_autohash(void);

#line 1279
void test1_test_undef(void);

#line 2

#line 29 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/typeinfo.ion"
struct TypeFieldInfo {
    char (*name);
    typeid type;
    int offset;
};

struct TypeInfo {
    TypeKind kind;
    int size;
    int align;
    char (*name);
    int count;
    typeid base;
    TypeFieldInfo (*fields);
    int num_fields;
};

#line 53 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
struct test1_S1 {
    int a;
    int b;
};

struct test1_S2 {
    test1_S1 s1;
};

#line 875
struct test1_Bar {
    int rc;
};

struct test1_Foo {
    test1_Bar (*bar);
};

#line 32
extern char test1_c;

#line 22 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
int test1_subtest1_func1(void);

#line 825 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
test1_Thing * test1_returns_ptr(void);

#line 819
struct test1_Thing {
    int a;
};

#line 829
test1_Thing * test1_returns_ptr_to_const(void);

#line 807
struct test1_BufHdr {
    size_t cap;
    #line 808
    size_t len;
    char (buf[1]);
};

#line 258
struct test1_Vector {
    int x;
    #line 259
    int y;
};

#line 529
void test1_println_any(any x);

#line 666
struct test1_Ints {
    int num_ints;
    int (*int_ptr);
    int (int_arr[3]);
};

#line 290
typedef int test1_Color;

#line 291
#define TEST1_COLOR_NONE ((test1_Color)(0))

#line 292
#define TEST1_COLOR_RED ((test1_Color)((TEST1_COLOR_NONE) + (1)))

#line 293
#define TEST1_COLOR_GREEN ((test1_Color)((TEST1_COLOR_RED) + (1)))

#line 294
#define TEST1_COLOR_BLUE ((test1_Color)((TEST1_COLOR_GREEN) + (1)))

#line 299
#define TEST1_FOO ((int)(0))

#line 300
#define TEST1_BAR ((int)((TEST1_FOO) + (1)))

#line 303
typedef int8_t test1_TypedEnum;

#line 304
#define TEST1_BAZ ((test1_TypedEnum)(0))

#line 305
#define TEST1_QUUX ((test1_TypedEnum)((TEST1_BAZ) + (1)))

#line 87
void test1_f10(ushort (a[3]));

#line 436
struct test1_ConstVector {
    int x;
    #line 437
    int y;
};

#line 431
extern test1_Vector test1_cv;

#line 224
extern char (test1_escape_to_char[256]);

#line 211
extern char (test1_char_to_escape[256]);

#line 8
extern char (*test1_esc_test_str);

#line 433
void test1_f4(char (*x));

#line 410
#define TEST1_IS_DEBUG (true)

#line 575
void test1_printf_any(char (*fmt), ...);

#line 570
void test1_println_type(ullong type);

#line 622
void test1_println_typeinfo(ullong type);

#line 3

#line 914
typedef time_t test1_my_time_t;

#line 630
void test1_push(any x);

#line 930
struct test1_float2 {
    float x;
    #line 931
    float y;
};

#line 140 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
struct std_TempAllocator {
    Allocator base;
    void (*start);
    void (*next);
    void (*end);
};

#line 164
std_TempAllocator std_temp_allocator(void (*buf), ullong size);

#line 396
struct std_Index {
    void (*data);
    std_Indexer (*indexer);
};

#line 606
std_Index std_hash_index(Allocator (*allocator));

#line 1066 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
struct test1_FuncPair {
    int (*f)(int, int);
    int (*g)(int, int);
};

int test1_funcpair_f(int x, int y);

#line 380 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
char * std_intern(char (*str));

#line 178
struct std_ArenaAllocator {
    Allocator base;
    Allocator (*allocator);
    size_t block_size;
    char* (*blocks);
    char (*next);
    char (*end);
};

#line 330
struct std_NameMap {
    std_ArenaAllocator arena;
    tuple162 (*nodes);
    std_NameNode* (*collisions);
};

void std_namemap_init(std_NameMap (*self), Allocator (*allocator));

#line 374
char * std_namemap_get(std_NameMap (*self), char (*str));

#line 1162 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
extern THREADLOCAL int test1_tls_test;

#line 1168
struct test1_Person {
    char (*name);
    int age;
};

#line 289 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
typedef int std_AllocatorEventKind;

#line 294
struct std_AllocatorEvent {
    std_AllocatorEventKind kind;
    time_t time;
    void (*ptr);
    size_t size;
    size_t align;
};

struct std_TraceAllocator {
    Allocator base;
    Allocator (*allocator);
    std_AllocatorEvent (*events);
};

#line 321
std_TraceAllocator std_trace_allocator(Allocator (*allocator));

#line 147
struct std_TempMark {
    void (*ptr);
};

#line 168
std_TempMark std_temp_begin(std_TempAllocator (*self));

#line 190
std_ArenaAllocator std_arena_allocator(void (*allocator));

#line 172
void std_temp_end(std_TempAllocator (*self), std_TempMark mark);

#line 53
typedef void (*std_DisposeFunc)(void*);

#line 54
typedef size_t std_DisposeMark;

struct std_Disposable {
    std_DisposeFunc dispose;
    std_DisposeMark mark;
};

#line 110
struct std_Recover {
    std_Disposable base;
    jmp_buf libc_env;
};

#line 45
std_File * std_open(char (*path));

#line 77
void * std_secure(void (*data));

#line 1215 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_test_panic(std_Recover (*ctx), int i);

#line 94 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
void std_dispose(void (*data));

#line 62
extern THREADLOCAL std_Disposable * (*std_disposables);

#line 31
struct std_File {
    std_Disposable base;
    FILE (*libc_file);
};

#line 1228 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
typedef tuple202 test1_SillyTuple;

struct test1_SillyStruct {
    test1_SillyTuple x;
};

#line 1235
extern test1_SillyStruct test1_x;

#line 659 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
struct std_Ahdr {
    Allocator (*allocator);
    size_t len;
    size_t cap;
    std_Index index;
    char (buf[1]);
};

#line 388
struct std_Indexer {
    size_t (*get)(void*, void*, void*, size_t, size_t, size_t);
    size_t (*put)(void*, void*, void*, size_t, size_t, size_t);
    size_t (*del)(void*, void*, void*, size_t, size_t, size_t);
    void (*set)(void*, void*, void*, size_t, size_t, size_t, size_t);
    void (*free)(void*);
};

#line 525
ullong std_hash_get(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 536
ullong std_hash_put(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 552
ullong std_hash_del(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 565
void std_hash_set(void (*data), void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size);

#line 579
void std_hash_free(void (*data));

#line 598
extern std_Indexer (*std_hash_indexer);

#line 17 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
void test1_subtest1_func2(void);

#line 823 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
extern test1_Thing test1_thing;

#line 523
void test1_print_any(any x);

#line 510
void test1_print_any_value(any x);

#line 539
void test1_print_type(ullong type);

#line 151
struct test1_UartCtrl {
    bool tx_enable;
    #line 152
    bool rx_enable;
};

#line 599
void test1_print_typeinfo(ullong type);

#line 206
union test1_IntOrPtr {
    int i;
    int (*p);
};

#line 627
extern char (test1_stack[1024]);

#line 628
extern char (*test1_stack_ptr);

#line 735 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
INLINE
void * std_anew_func(Allocator (*allocator), ullong elem_size, ullong elem_align);

#line 768
INLINE
ullong std_apush_func(void * (*ap), void (*x), ullong elem_size, ullong elem_align);

#line 683
INLINE
ullong std_alen_func(void (*a), ullong elem_size, ullong elem_align);

#line 688
INLINE
ullong std_acap_func(void (*a), ullong elem_size, ullong elem_align);

#line 832
NOINLINE
void std_asetcap_func(void * (*ap), ullong new_cap, ullong elem_size, ullong elem_align);

#line 749
INLINE
void std_apop_func(void (*a), ullong elem_size, ullong elem_align);

#line 804
void std_acat_func(void * (*ap), void (*src), ullong elem_size, ullong elem_align);

#line 725
INLINE
void std_afree_func(void * (*ap), ullong elem_size, ullong elem_align);

#line 755
void std_afill_func(void * (*ap), void (*x), ullong n, ullong elem_size, ullong elem_align);

#line 792
void std_acatn_func(void * (*ap), void (*src), ullong src_len, ullong elem_size, ullong elem_align);

#line 780
void std_adeln_func(void (*a), ullong i, ullong n, ullong elem_size, ullong elem_align);

#line 808
void std_aprintf_func(void * (*ap), char (*fmt), ...);

#line 153
void * std_temp_alloc(void (*allocator), ullong size, ullong align);

#line 137
void std_noop_free(void (*data), void (*ptr));

#line 894
void std_aindex_func(void * (*ap), std_Index new_index, ullong key_size, ullong elem_size, ullong elem_align);

#line 452
struct std_HashSlot {
    uint32_t i;
    uint32_t h;
};

struct std_HashIndex {
    Allocator (*allocator);
    std_HashSlot (*slots);
    uint32_t mask;
    uint32_t occupied;
    uint32_t max_occupied;
};

#line 491
void std_hash_init(std_HashIndex (*index), uint len, Allocator (*allocator));

#line 933
ullong std_aput_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 953
void std_adel_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 909
ullong std_ageti_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 917
void * std_agetp_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 1054 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
struct test1_Node {
    int id;
};

#line 378 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
extern std_NameMap std_intern_namemap;

#line 325
struct std_NameNode {
    uint32_t len;
    char (buf[1]);
};

#line 348
char * std_namemap_getn(std_NameMap (*self), char (*buf), ullong len);

#line 308
void * std_trace_alloc(void (*allocator), ullong size, ullong align);

#line 315
void std_trace_free(void (*allocator), void (*ptr));

#line 214
void * std_arena_alloc(void (*allocator), ullong size, ullong align);

#line 187
#define STD_ARENA_MIN_BLOCK_SIZE ((size_t)(sizeof(ullong)))

#line 64
std_Disposable std_make_disposable(void(*dispose)(void *));

#line 115
void std_recover_dispose(void (*data));

#line 120
extern THREADLOCAL std_Recover (*std_temp_ctx);

#line 36
void std_file_dispose(void (*data));

#line 68
bool std_secured(void (*data));

#line 132
void std_panic(std_Recover (*ctx));

#line 922
void * std_aget_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 879
void std_asetdefault_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align);

#line 678
INLINE
std_Ahdr * std_ahdr_func(void (*a), ullong elem_size, ullong elem_align);

#line 469
ullong std_hash(void (*buf), ullong size);

#line 501
std_HashSlot * std_hash_get_slot(std_HashIndex (*index), void (*a), void (*x), uint h, ullong stride, ullong size);

#line 465
#define STD_HASH_EMPTY ((uint32_t)(0xFFFFFFFF))

#line 585
void std_hash_rehash(std_HashIndex (*index), uint len);

#line 466
#define STD_HASH_DELETED ((uint32_t)(0xFFFFFFFE))

#line 9 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
void test1_subtest1_func3(void);

#line 14
void test1_subtest1_func4(void);

#line 534 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_print_typeid(ullong type);

#line 668 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
INLINE
ullong std_ahdrsize_func(ullong elem_size, ullong elem_align);

#line 673
INLINE
ullong std_ahdralign_func(ullong elem_size, ullong elem_align);

#line 433
ullong std_linear_get(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 427
void std_null_set(void (*data), void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size);

#line 430
void std_null_free(void (*data));

#line 612
extern std_Indexer (*std_default_indexer);

#line 693
INLINE
void * std_amem_func(void (*a), ullong elem_size, ullong elem_align);

#line 423
INLINE
void std_index_free(std_Index index);

#line 888
INLINE
void std_afit_func(void * (*ap), ullong min_cap, ullong elem_size, ullong elem_align);

#line 417
INLINE
void std_index_set(std_Index index, void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size);

#line 480
uint std_next_pow2(uint x);

#line 467
#define STD_HASH_MIN_SLOTS ((uint32_t)(16))

#line 407
INLINE
ullong std_index_put(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 412
INLINE
ullong std_index_del(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 402
INLINE
ullong std_index_get(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size);

#line 290
#define STD_EVENT_ALLOC ((std_AllocatorEventKind)(0))

#line 291
#define STD_EVENT_FREE ((std_AllocatorEventKind)((STD_EVENT_ALLOC) + (1)))

#line 194
void * std_arena_alloc_grow(std_ArenaAllocator (*self), ullong size, ullong align);

#line 705
INLINE
ullong std_asetlen_func(void (*a), ullong new_len, ullong elem_size, ullong elem_align);

#line 875
INLINE
void * std_adefault_func(void (*a), ullong elem_size, ullong elem_align);

#line 513
void std_hash_put_slot(std_HashIndex (*index), std_HashSlot new_slot);

#line 1 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"

#line 188 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
#define STD_ARENA_MIN_BLOCK_ALIGN ((size_t)(sizeof(ullong)))

// -Typeinfo-
#define TYPEID0(index, kind) ((ullong)(index) | ((ullong)(kind) << 24))
#define TYPEID(index, kind, ...) ((ullong)(index) | ((ullong)sizeof(__VA_ARGS__) << 32) | ((ullong)(kind) << 24))

TypeInfo *typeinfo_table[269] = {
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
    [16] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(3, TYPE_CHAR, char)},
    [17] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(1, TYPE_VOID)},
    [18] = NULL, // Func
    [19] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(Allocator), .align = alignof(Allocator), .name = "Allocator", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"alloc", .type = TYPEID(24, TYPE_FUNC, void *(*)(void *, ullong, ullong)), .offset = offsetof(Allocator, alloc)},
            {"free", .type = TYPEID(25, TYPE_FUNC, void(*)(void *, void *)), .offset = offsetof(Allocator, free)},
        }
    },
    [20] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(19, TYPE_STRUCT, Allocator)},
    [21] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(any), .align = alignof(any), .name = "any", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"ptr", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(any, ptr)},
            {"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(any, type)},
        }
    },
    [22] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(16, TYPE_PTR, char *)},
    [23] = NULL, // Func
    [24] = NULL, // Func
    [25] = NULL, // Func
    [26] = NULL, // Func
    [27] = NULL, // Func
    [28] = NULL, // Func
    [29] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [6]), .align = alignof(char [6]), .base = TYPEID(3, TYPE_CHAR, char), .count = 6},
    [30] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [4]), .align = alignof(char [4]), .base = TYPEID(3, TYPE_CHAR, char), .count = 4},
    [31] = NULL, // Enum
    [32] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeFieldInfo), .align = alignof(TypeFieldInfo), .name = "TypeFieldInfo", .num_fields = 3,
        .fields = (TypeFieldInfo[]) {
            {"name", .type = TYPEID(16, TYPE_PTR, char *), .offset = offsetof(TypeFieldInfo, name)},
            {"type", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeFieldInfo, type)},
            {"offset", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeFieldInfo, offset)},
        }
    },
    [33] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(TypeInfo), .align = alignof(TypeInfo), .name = "TypeInfo", .num_fields = 8,
        .fields = (TypeFieldInfo[]) {
            {"kind", .type = TYPEID(31, TYPE_NONE, TypeKind), .offset = offsetof(TypeInfo, kind)},
            {"size", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, size)},
            {"align", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, align)},
            {"name", .type = TYPEID(16, TYPE_PTR, char *), .offset = offsetof(TypeInfo, name)},
            {"count", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, count)},
            {"base", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(TypeInfo, base)},
            {"fields", .type = TYPEID(53, TYPE_PTR, TypeFieldInfo *), .offset = offsetof(TypeInfo, fields)},
            {"num_fields", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(TypeInfo, num_fields)},
        }
    },
    [34] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(33, TYPE_STRUCT, TypeInfo)},
    [35] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(34, TYPE_PTR, TypeInfo *)},
    [36] = NULL, // Func
    [37] = NULL, // Func
    [38] = NULL, // Func
    [39] = NULL, // Func
    [40] = &(TypeInfo){TYPE_CONST, .size = sizeof(const char), .align = alignof(const char), .base = TYPEID(3, TYPE_CHAR, char)},
    [41] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(40, TYPE_CONST, const char)},
    [42] = NULL, // Func
    [43] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [14]), .align = alignof(char [14]), .base = TYPEID(3, TYPE_CHAR, char), .count = 14},
    [44] = NULL, // Func
    [45] = NULL, // Func
    [46] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [9]), .align = alignof(char [9]), .base = TYPEID(3, TYPE_CHAR, char), .count = 9},
    [47] = NULL, // Func
    [48] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [3]), .align = alignof(char [3]), .base = TYPEID(3, TYPE_CHAR, char), .count = 3},
    [49] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_File), .align = alignof(std_File), .name = "std_File", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"base", .type = TYPEID(186, TYPE_STRUCT, std_Disposable), .offset = offsetof(std_File, base)},
            {"libc_file", .type = TYPEID(197, TYPE_PTR, FILE *), .offset = offsetof(std_File, libc_file)},
        }
    },
    [50] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(49, TYPE_STRUCT, std_File)},
    [51] = NULL, // Func
    [52] = NULL, // Func
    [53] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(32, TYPE_STRUCT, TypeFieldInfo)},
    [54] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S1), .align = alignof(test1_S1), .name = "test1_S1", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_S1, a)},
            {"b", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_S1, b)},
        }
    },
    [55] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_S2), .align = alignof(test1_S2), .name = "test1_S2", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"s1", .type = TYPEID(54, TYPE_STRUCT, test1_S1), .offset = offsetof(test1_S2, s1)},
        }
    },
    [56] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Bar), .align = alignof(test1_Bar), .name = "test1_Bar", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"rc", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Bar, rc)},
        }
    },
    [57] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Foo), .align = alignof(test1_Foo), .name = "test1_Foo", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"bar", .type = TYPEID(58, TYPE_PTR, test1_Bar *), .offset = offsetof(test1_Foo, bar)},
        }
    },
    [58] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(56, TYPE_STRUCT, test1_Bar)},
    [59] = NULL, // Func
    [60] = NULL, // Func
    [61] = NULL, // Func
    [62] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [19]), .align = alignof(char [19]), .base = TYPEID(3, TYPE_CHAR, char), .count = 19},
    [63] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [29]), .align = alignof(char [29]), .base = TYPEID(3, TYPE_CHAR, char), .count = 29},
    [64] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [33]), .align = alignof(char [33]), .base = TYPEID(3, TYPE_CHAR, char), .count = 33},
    [65] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(8, TYPE_INT, int)},
    [66] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [16]), .align = alignof(int [16]), .base = TYPEID(8, TYPE_INT, int), .count = 16},
    [67] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Thing), .align = alignof(test1_Thing), .name = "test1_Thing", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"a", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Thing, a)},
        }
    },
    [68] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(67, TYPE_STRUCT, test1_Thing)},
    [69] = NULL, // Func
    [70] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_BufHdr), .align = alignof(test1_BufHdr), .name = "test1_BufHdr", .num_fields = 3,
        .fields = (TypeFieldInfo[]) {
            {"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, cap)},
            {"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(test1_BufHdr, len)},
            {"buf", .type = TYPEID(71, TYPE_ARRAY, char [1]), .offset = offsetof(test1_BufHdr, buf)},
        }
    },
    [71] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1]), .align = alignof(char [1]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1},
    [72] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Vector), .align = alignof(test1_Vector), .name = "test1_Vector", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, x)},
            {"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Vector, y)},
        }
    },
    [73] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(72, TYPE_STRUCT, test1_Vector)},
    [74] = NULL, // Incomplete array type
    [75] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = TYPEID(8, TYPE_INT, int), .count = 3},
    [76] = NULL, // Func
    [77] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Ints), .align = alignof(test1_Ints), .name = "test1_Ints", .num_fields = 3,
        .fields = (TypeFieldInfo[]) {
            {"num_ints", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Ints, num_ints)},
            {"int_ptr", .type = TYPEID(65, TYPE_PTR, int *), .offset = offsetof(test1_Ints, int_ptr)},
            {"int_arr", .type = TYPEID(75, TYPE_ARRAY, int [3]), .offset = offsetof(test1_Ints, int_arr)},
        }
    },
    [78] = NULL, // Incomplete array type
    [79] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(test1_Ints [2]), .align = alignof(test1_Ints [2]), .base = TYPEID(77, TYPE_STRUCT, test1_Ints), .count = 2},
    [80] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [2]), .align = alignof(int [2]), .base = TYPEID(8, TYPE_INT, int), .count = 2},
    [81] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(test1_Ints [2]), .align = alignof(test1_Ints [2]), .base = TYPEID(77, TYPE_STRUCT, test1_Ints), .count = 2},
    [82] = NULL, // Enum
    [83] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [13]), .align = alignof(char [13]), .base = TYPEID(3, TYPE_CHAR, char), .count = 13},
    [84] = NULL, // Enum
    [85] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(ushort [3]), .align = alignof(ushort [3]), .base = TYPEID(7, TYPE_USHORT, ushort), .count = 3},
    [86] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(ushort [3]), .align = alignof(ushort [3]), .base = TYPEID(7, TYPE_USHORT, ushort), .count = 3},
    [87] = NULL, // Func
    [88] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(7, TYPE_USHORT, ushort)},
    [89] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [3]), .align = alignof(int [3]), .base = TYPEID(8, TYPE_INT, int), .count = 3},
    [90] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = TYPEID(8, TYPE_INT, int), .count = 4},
    [91] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_ConstVector), .align = alignof(test1_ConstVector), .name = "test1_ConstVector", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_ConstVector, x)},
            {"y", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_ConstVector, y)},
        }
    },
    [92] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [256]), .align = alignof(char [256]), .base = TYPEID(3, TYPE_CHAR, char), .count = 256},
    [93] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [27]), .align = alignof(char [27]), .base = TYPEID(3, TYPE_CHAR, char), .count = 27},
    [94] = NULL, // Func
    [95] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(14, TYPE_FLOAT, float)},
    [96] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(17, TYPE_PTR, void *)},
    [97] = NULL, // Func
    [98] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [17]), .align = alignof(char [17]), .base = TYPEID(3, TYPE_CHAR, char), .count = 17},
    [99] = NULL, // Func
    [100] = &(TypeInfo){TYPE_CONST, .size = sizeof(const int), .align = alignof(const int), .base = TYPEID(8, TYPE_INT, int)},
    [101] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(100, TYPE_CONST, const int)},
    [102] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(const int * [42]), .align = alignof(const int * [42]), .base = TYPEID(101, TYPE_PTR, const int *), .count = 42},
    [103] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_UartCtrl), .align = alignof(test1_UartCtrl), .name = "test1_UartCtrl", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"tx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, tx_enable)},
            {"rx_enable", .type = TYPEID(2, TYPE_BOOL, bool), .offset = offsetof(test1_UartCtrl, rx_enable)},
        }
    },
    [104] = &(TypeInfo){TYPE_UNION, .size = sizeof(test1_IntOrPtr), .align = alignof(test1_IntOrPtr), .name = "test1_IntOrPtr", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"i", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_IntOrPtr, i)},
            {"p", .type = TYPEID(65, TYPE_PTR, int *), .offset = offsetof(test1_IntOrPtr, p)},
        }
    },
    [105] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(104, TYPE_UNION, test1_IntOrPtr)},
    [106] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(12, TYPE_LLONG, llong)},
    [107] = NULL, // Func
    [108] = NULL, // Incomplete array type
    [109] = NULL, // Func
    [110] = NULL, // Func
    [111] = NULL, // Func
    [112] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [12]), .align = alignof(char [12]), .base = TYPEID(3, TYPE_CHAR, char), .count = 12},
    [113] = NULL, // Func
    [114] = NULL, // Func
    [115] = NULL, // Incomplete array type
    [116] = &(TypeInfo){TYPE_CONST, .size = 0, .align = 0, .base = TYPEID0(1, TYPE_VOID)},
    [117] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(116, TYPE_CONST)},
    [118] = NULL, // Func
    [119] = NULL, // Func
    [120] = NULL, // Func
    [121] = NULL, // Func
    [122] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_float2), .align = alignof(test1_float2), .name = "test1_float2", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"x", .type = TYPEID(14, TYPE_FLOAT, float), .offset = offsetof(test1_float2, x)},
            {"y", .type = TYPEID(14, TYPE_FLOAT, float), .offset = offsetof(test1_float2, y)},
        }
    },
    [123] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(122, TYPE_STRUCT, test1_float2)},
    [124] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [15]), .align = alignof(char [15]), .base = TYPEID(3, TYPE_CHAR, char), .count = 15},
    [125] = NULL, // Func
    [126] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [1024]), .align = alignof(char [1024]), .base = TYPEID(3, TYPE_CHAR, char), .count = 1024},
    [127] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_TempAllocator), .align = alignof(std_TempAllocator), .name = "std_TempAllocator", .num_fields = 4,
        .fields = (TypeFieldInfo[]) {
            {"base", .type = TYPEID(19, TYPE_STRUCT, Allocator), .offset = offsetof(std_TempAllocator, base)},
            {"start", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_TempAllocator, start)},
            {"next", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_TempAllocator, next)},
            {"end", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_TempAllocator, end)},
        }
    },
    [128] = NULL, // Func
    [129] = NULL, // Incomplete array type
    [130] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_Index), .align = alignof(std_Index), .name = "std_Index", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"data", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_Index, data)},
            {"indexer", .type = TYPEID(132, TYPE_PTR, std_Indexer *), .offset = offsetof(std_Index, indexer)},
        }
    },
    [131] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_Indexer), .align = alignof(std_Indexer), .name = "std_Indexer", .num_fields = 5,
        .fields = (TypeFieldInfo[]) {
            {"get", .type = TYPEID(207, TYPE_FUNC, ullong(*)(void *, void *, void *, ullong, ullong, ullong)), .offset = offsetof(std_Indexer, get)},
            {"put", .type = TYPEID(207, TYPE_FUNC, ullong(*)(void *, void *, void *, ullong, ullong, ullong)), .offset = offsetof(std_Indexer, put)},
            {"del", .type = TYPEID(207, TYPE_FUNC, ullong(*)(void *, void *, void *, ullong, ullong, ullong)), .offset = offsetof(std_Indexer, del)},
            {"set", .type = TYPEID(208, TYPE_FUNC, void(*)(void *, void *, void *, ullong, ullong, ullong, ullong)), .offset = offsetof(std_Indexer, set)},
            {"free", .type = TYPEID(26, TYPE_FUNC, void(*)(void *)), .offset = offsetof(std_Indexer, free)},
        }
    },
    [132] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(131, TYPE_STRUCT, std_Indexer)},
    [133] = NULL, // Func
    [134] = NULL, // Func
    [135] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(127, TYPE_STRUCT, std_TempAllocator)},
    [136] = NULL, // Func
    [137] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [16]), .align = alignof(char [16]), .base = TYPEID(3, TYPE_CHAR, char), .count = 16},
    [138] = NULL, // Func
    [139] = NULL, // Unhandled
    [140] = NULL, // Incomplete array type
    [141] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(139, TYPE_TUPLE, tuple139)},
    [142] = NULL, // Func
    [143] = NULL, // Unhandled
    [144] = NULL, // Incomplete array type
    [145] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(143, TYPE_TUPLE, tuple143)},
    [146] = NULL, // Unhandled
    [147] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Node), .align = alignof(test1_Node), .name = "test1_Node", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"id", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Node, id)},
        }
    },
    [148] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(147, TYPE_STRUCT, test1_Node)},
    [149] = NULL, // Unhandled
    [150] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(149, TYPE_TUPLE, tuple149)},
    [151] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_FuncPair), .align = alignof(test1_FuncPair), .name = "test1_FuncPair", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"f", .type = TYPEID(152, TYPE_FUNC, int(*)(int, int)), .offset = offsetof(test1_FuncPair, f)},
            {"g", .type = TYPEID(152, TYPE_FUNC, int(*)(int, int)), .offset = offsetof(test1_FuncPair, g)},
        }
    },
    [152] = NULL, // Func
    [153] = NULL, // Func
    [154] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(ForeignStruct), .align = alignof(ForeignStruct), .name = "ForeignStruct", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"x", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(ForeignStruct, x)},
            {"y", .type = TYPEID(100, TYPE_CONST, const int), .offset = offsetof(ForeignStruct, y)},
        }
    },
    [155] = NULL, // Func
    [156] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [10240]), .align = alignof(char [10240]), .base = TYPEID(3, TYPE_CHAR, char), .count = 10240},
    [157] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_NameMap), .align = alignof(std_NameMap), .name = "std_NameMap", .num_fields = 3,
        .fields = (TypeFieldInfo[]) {
            {"arena", .type = TYPEID(158, TYPE_STRUCT, std_ArenaAllocator), .offset = offsetof(std_NameMap, arena)},
            {"nodes", .type = TYPEID(164, TYPE_PTR, tuple162 *), .offset = offsetof(std_NameMap, nodes)},
            {"collisions", .type = TYPEID(166, TYPE_PTR, std_NameNode * *), .offset = offsetof(std_NameMap, collisions)},
        }
    },
    [158] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_ArenaAllocator), .align = alignof(std_ArenaAllocator), .name = "std_ArenaAllocator", .num_fields = 6,
        .fields = (TypeFieldInfo[]) {
            {"base", .type = TYPEID(19, TYPE_STRUCT, Allocator), .offset = offsetof(std_ArenaAllocator, base)},
            {"allocator", .type = TYPEID(20, TYPE_PTR, Allocator *), .offset = offsetof(std_ArenaAllocator, allocator)},
            {"block_size", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_ArenaAllocator, block_size)},
            {"blocks", .type = TYPEID(22, TYPE_PTR, char * *), .offset = offsetof(std_ArenaAllocator, blocks)},
            {"next", .type = TYPEID(16, TYPE_PTR, char *), .offset = offsetof(std_ArenaAllocator, next)},
            {"end", .type = TYPEID(16, TYPE_PTR, char *), .offset = offsetof(std_ArenaAllocator, end)},
        }
    },
    [159] = NULL, // Incomplete array type
    [160] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_NameNode), .align = alignof(std_NameNode), .name = "std_NameNode", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"len", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_NameNode, len)},
            {"buf", .type = TYPEID(71, TYPE_ARRAY, char [1]), .offset = offsetof(std_NameNode, buf)},
        }
    },
    [161] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(160, TYPE_STRUCT, std_NameNode)},
    [162] = NULL, // Unhandled
    [163] = NULL, // Incomplete array type
    [164] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(162, TYPE_TUPLE, tuple162)},
    [165] = NULL, // Incomplete array type
    [166] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(161, TYPE_PTR, std_NameNode *)},
    [167] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(157, TYPE_STRUCT, std_NameMap)},
    [168] = NULL, // Func
    [169] = NULL, // Func
    [170] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_Person), .align = alignof(test1_Person), .name = "test1_Person", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"name", .type = TYPEID(16, TYPE_PTR, char *), .offset = offsetof(test1_Person, name)},
            {"age", .type = TYPEID(8, TYPE_INT, int), .offset = offsetof(test1_Person, age)},
        }
    },
    [171] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(170, TYPE_STRUCT, test1_Person)},
    [172] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_TraceAllocator), .align = alignof(std_TraceAllocator), .name = "std_TraceAllocator", .num_fields = 3,
        .fields = (TypeFieldInfo[]) {
            {"base", .type = TYPEID(19, TYPE_STRUCT, Allocator), .offset = offsetof(std_TraceAllocator, base)},
            {"allocator", .type = TYPEID(20, TYPE_PTR, Allocator *), .offset = offsetof(std_TraceAllocator, allocator)},
            {"events", .type = TYPEID(176, TYPE_PTR, std_AllocatorEvent *), .offset = offsetof(std_TraceAllocator, events)},
        }
    },
    [173] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_AllocatorEvent), .align = alignof(std_AllocatorEvent), .name = "std_AllocatorEvent", .num_fields = 5,
        .fields = (TypeFieldInfo[]) {
            {"kind", .type = TYPEID(174, TYPE_NONE, std_AllocatorEventKind), .offset = offsetof(std_AllocatorEvent, kind)},
            {"time", .type = TYPEID(12, TYPE_LLONG, llong), .offset = offsetof(std_AllocatorEvent, time)},
            {"ptr", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_AllocatorEvent, ptr)},
            {"size", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_AllocatorEvent, size)},
            {"align", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_AllocatorEvent, align)},
        }
    },
    [174] = NULL, // Enum
    [175] = NULL, // Incomplete array type
    [176] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(173, TYPE_STRUCT, std_AllocatorEvent)},
    [177] = NULL, // Func
    [178] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(172, TYPE_STRUCT, std_TraceAllocator)},
    [179] = NULL, // Incomplete array type
    [180] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [32768]), .align = alignof(char [32768]), .base = TYPEID(3, TYPE_CHAR, char), .count = 32768},
    [181] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_TempMark), .align = alignof(std_TempMark), .name = "std_TempMark", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"ptr", .type = TYPEID(17, TYPE_PTR, void *), .offset = offsetof(std_TempMark, ptr)},
        }
    },
    [182] = NULL, // Func
    [183] = NULL, // Func
    [184] = NULL, // Func
    [185] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_Recover), .align = alignof(std_Recover), .name = "std_Recover", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"base", .type = TYPEID(186, TYPE_STRUCT, std_Disposable), .offset = offsetof(std_Recover, base)},
            {"libc_env", .type = TYPEID0(187, TYPE_STRUCT), .offset = offsetof(std_Recover, libc_env)},
        }
    },
    [186] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_Disposable), .align = alignof(std_Disposable), .name = "std_Disposable", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"dispose", .type = TYPEID(26, TYPE_FUNC, void(*)(void *)), .offset = offsetof(std_Disposable, dispose)},
            {"mark", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_Disposable, mark)},
        }
    },
    [187] = NULL, // No associated type
    [188] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(185, TYPE_STRUCT, std_Recover)},
    [189] = NULL, // Func
    [190] = NULL, // Func
    [191] = NULL, // Func
    [192] = NULL, // Func
    [193] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(186, TYPE_STRUCT, std_Disposable)},
    [194] = NULL, // Incomplete array type
    [195] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(193, TYPE_PTR, std_Disposable *)},
    [196] = NULL, // Incomplete: FILE
    [197] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID0(196, TYPE_NONE)},
    [198] = NULL, // Unhandled
    [199] = NULL, // Incomplete array type
    [200] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(198, TYPE_TUPLE, tuple198)},
    [201] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(test1_SillyStruct), .align = alignof(test1_SillyStruct), .name = "test1_SillyStruct", .num_fields = 1,
        .fields = (TypeFieldInfo[]) {
            {"x", .type = TYPEID(202, TYPE_TUPLE, tuple202), .offset = offsetof(test1_SillyStruct, x)},
        }
    },
    [202] = NULL, // Unhandled
    [203] = NULL, // Incomplete array type
    [204] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_Ahdr), .align = alignof(std_Ahdr), .name = "std_Ahdr", .num_fields = 5,
        .fields = (TypeFieldInfo[]) {
            {"allocator", .type = TYPEID(20, TYPE_PTR, Allocator *), .offset = offsetof(std_Ahdr, allocator)},
            {"len", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_Ahdr, len)},
            {"cap", .type = TYPEID(13, TYPE_ULLONG, ullong), .offset = offsetof(std_Ahdr, cap)},
            {"index", .type = TYPEID(130, TYPE_STRUCT, std_Index), .offset = offsetof(std_Ahdr, index)},
            {"buf", .type = TYPEID(71, TYPE_ARRAY, char [1]), .offset = offsetof(std_Ahdr, buf)},
        }
    },
    [205] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(204, TYPE_STRUCT, std_Ahdr)},
    [206] = NULL, // Func
    [207] = NULL, // Func
    [208] = NULL, // Func
    [209] = NULL, // Unhandled
    [210] = NULL, // Incomplete array type
    [211] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(209, TYPE_TUPLE, tuple209)},
    [212] = NULL, // Incomplete array type
    [213] = NULL, // Incomplete array type
    [214] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(int [4]), .align = alignof(int [4]), .base = TYPEID(8, TYPE_INT, int), .count = 4},
    [215] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [2]), .align = alignof(char [2]), .base = TYPEID(3, TYPE_CHAR, char), .count = 2},
    [216] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [5]), .align = alignof(char [5]), .base = TYPEID(3, TYPE_CHAR, char), .count = 5},
    [217] = NULL, // Func
    [218] = NULL, // Func
    [219] = NULL, // Func
    [220] = NULL, // Func
    [221] = NULL, // Func
    [222] = NULL, // Func
    [223] = NULL, // Func
    [224] = NULL, // Func
    [225] = NULL, // Func
    [226] = NULL, // Func
    [227] = NULL, // Func
    [228] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_HashIndex), .align = alignof(std_HashIndex), .name = "std_HashIndex", .num_fields = 5,
        .fields = (TypeFieldInfo[]) {
            {"allocator", .type = TYPEID(20, TYPE_PTR, Allocator *), .offset = offsetof(std_HashIndex, allocator)},
            {"slots", .type = TYPEID(232, TYPE_PTR, std_HashSlot *), .offset = offsetof(std_HashIndex, slots)},
            {"mask", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_HashIndex, mask)},
            {"occupied", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_HashIndex, occupied)},
            {"max_occupied", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_HashIndex, max_occupied)},
        }
    },
    [229] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(228, TYPE_STRUCT, std_HashIndex)},
    [230] = &(TypeInfo){TYPE_STRUCT, .size = sizeof(std_HashSlot), .align = alignof(std_HashSlot), .name = "std_HashSlot", .num_fields = 2,
        .fields = (TypeFieldInfo[]) {
            {"i", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_HashSlot, i)},
            {"h", .type = TYPEID(9, TYPE_UINT, uint), .offset = offsetof(std_HashSlot, h)},
        }
    },
    [231] = NULL, // Incomplete array type
    [232] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(230, TYPE_STRUCT, std_HashSlot)},
    [233] = NULL, // Func
    [234] = NULL, // Func
    [235] = NULL, // Func
    [236] = NULL, // Func
    [237] = NULL, // Func
    [238] = &(TypeInfo){TYPE_PTR, .size = sizeof(void *), .align = alignof(void *), .base = TYPEID(158, TYPE_STRUCT, std_ArenaAllocator)},
    [239] = NULL, // Func
    [240] = NULL, // Func
    [241] = NULL, // Func
    [242] = NULL, // Func
    [243] = NULL, // Func
    [244] = NULL, // Func
    [245] = NULL, // Func
    [246] = NULL, // Func
    [247] = NULL, // Func
    [248] = NULL, // Func
    [249] = NULL, // Func
    [250] = NULL, // Func
    [251] = NULL, // Func
    [252] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [10]), .align = alignof(char [10]), .base = TYPEID(3, TYPE_CHAR, char), .count = 10},
    [253] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [7]), .align = alignof(char [7]), .base = TYPEID(3, TYPE_CHAR, char), .count = 7},
    [254] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [18]), .align = alignof(char [18]), .base = TYPEID(3, TYPE_CHAR, char), .count = 18},
    [255] = NULL, // Func
    [256] = NULL, // Func
    [257] = NULL, // Func
    [258] = NULL, // Func
    [259] = NULL, // Func
    [260] = NULL, // Func
    [261] = NULL, // Func
    [262] = NULL, // Func
    [263] = NULL, // Func
    [264] = NULL, // Func
    [265] = NULL, // Func
    [266] = NULL, // Func
    [267] = NULL, // Func
    [268] = &(TypeInfo){TYPE_ARRAY, .size = sizeof(char [11]), .align = alignof(char [11]), .base = TYPEID(3, TYPE_CHAR, char), .count = 11},
};
int num_typeinfos = 269;
TypeInfo **typeinfos = (TypeInfo **)typeinfo_table;

// -Definitions-
#line 1307 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
int main(int argc, char * (*argv)) {
    if ((argv) == (0)) {
        (printf)("argv is null\n");
    }
    test1_test_nonmodifiable();
    test1_test_const_ptr_deref();
    test1_test_limits();
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
    test1_test_type_path();
    test1_test_push();
    test1_test_va_type("%d", 42);
    test1_test_void();
    test1_test_dynamic_arrays();
    test1_test_aprintf();
    test1_test_index_arrays();
    test1_test_tuples();
    test1_test_func_interning();
    test1_test_hashing();
    test1_test_void_ptr_arithmetic();
    test1_test_foreign_const();
    test1_test_const_implicit();
    test1_test_intern();
    test1_test_namemap();
    test1_test_threadlocal();
    test1_test_new();
    test1_test_disposable();
    test1_test_aget();
    test1_test_tuple_deps();
    test1_test_autohash();
    test1_test_undef();
    #line 1361
    (getchar)();
    return 0;
}

THREADLOCAL Allocator (*current_allocator);

char (*IONOS) = "win32";

char (*IONARCH) = "x64";

TypeInfo* (*typeinfos);

int num_typeinfos;

#line 52 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/builtin/typeinfo.ion"
TypeKind typeid_kind(ullong type) {
    return (TypeKind)((((type) >> (24))) & (0xFF));
}

int typeid_index(ullong type) {
    return (int)((type) & (0xFFFFFF));
}

ullong typeid_size(ullong type) {
    return (size_t)((type) >> (32));
}

TypeInfo * get_typeinfo(ullong type) {
    int index = typeid_index(type);
    if ((typeinfos) && ((index) < (num_typeinfos))) {
        return typeinfos[index];
    } else {
        return NULL;
    }
}

#line 137 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_test_nonmodifiable(void) {
    test1_S1 s1 = {0};
    s1.a = 0;
    s1.a = 42;
    #line 144
    test1_S2 s2 = {0};
    s2.s1.a = 0;
}

#line 883
void test1_test_const_ptr_deref(void) {
    test1_Bar bar = {.rc = 42};
    test1_Foo foo = {.bar = &(bar)};
    int i = foo.bar->rc;
}

#line 745
void test1_test_limits(void) {
    char char_min = CHAR_MIN;
    char char_max = CHAR_MAX;
    schar schar_min = SCHAR_MIN;
    schar schar_max = SCHAR_MAX;
    uchar uchar_min = UCHAR_MIN;
    uchar uchar_max = UCHAR_MAX;
    short short_min = SHRT_MIN;
    short short_max = SHRT_MAX;
    short ushort_min = USHORT_MIN;
    ushort ushort_max = USHRT_MAX;
    int int_min = INT_MIN;
    int int_max = INT_MAX;
    uint uint_min = UINT_MIN;
    uint uint_max = UINT_MAX;
    long long_min = LONG_MIN;
    long long_max = LONG_MAX;
    ulong ulong_min = ULONG_MIN;
    ulong ulong_max = ULONG_MAX;
    llong llong_min = LLONG_MIN;
    llong llong_max = LLONG_MAX;
    ullong ullong_min = ULLONG_MIN;
    ullong ullong_max = ULLONG_MAX;
    #line 769
    short wchar_min = WCHAR_MIN;
    ushort wchar_max = WCHAR_MAX;
    #line 772
    schar int8_min = INT8_MIN;
    schar int8_max = INT8_MAX;
    uchar uint8_min = UINT8_MIN;
    uchar uint8_max = UINT8_MAX;
    short int16_min = INT16_MIN;
    short int16_max = INT16_MAX;
    short uint16_min = UINT16_MIN;
    ushort uint16_max = UINT16_MAX;
    int int32_min = INT32_MIN;
    int int32_max = INT32_MAX;
    uint uint32_min = UINT32_MIN;
    uint uint32_max = UINT32_MAX;
    llong int64_min = INT64_MIN;
    llong int64_max = INT64_MAX;
    ullong uint64_min = UINT64_MIN;
    ullong uint64_max = UINT64_MAX;
    #line 789
    ullong usize_min = USIZE_MIN;
    ullong usize_max = SIZE_MAX;
    llong ssize_min = PTRDIFF_MIN;
    llong ssize_max = PTRDIFF_MAX;
    ullong uintptr_min = UINTPTR_MIN;
    ullong uintptr_max = UINTPTR_MAX;
    llong intptr_min = INTPTR_MIN;
    llong intptr_max = INTPTR_MAX;
}

#line 680
void test1_test_va_list(char (*fmt), ...) {
    va_list init_args = {0};
    va_start(init_args, fmt);
    va_list args = {0};
    va_copy(args, init_args);
    va_end(init_args);
    int i = {0};
    i = va_arg(args, int);
    llong ll = {0};
    ll = va_arg(args, llong);
    (printf)("c=%d i=%d ll=%lld\n", test1_c, i, ll);
    va_end(args);
}

#line 870
void test1_test_os_arch(void) {
    (printf)("Target operating system: %s\n", IONOS);
    (printf)("Target machine architecture: %s\n", IONARCH);
}

#line 62
void test1_test_packages(void) {
    test1_subtest1_func1();
}

#line 853
void test1_test_if(void) {
    if (1) {
    }
    #line 858
    {
        #line 858
        int x = 42;
        if (x) {
        }
    }
    #line 860
    {
        #line 860
        int x = 42;
        if ((x) >= (0)) {
        }
    }
    #line 862
    {
        #line 862
        int x = 42;
        if ((x) >= (0)) {
        }
    }
}

#line 66
void test1_test_modify(void) {
    int i = 42;
    #line 69
    int (*p) = &(i);
    #line 71
    (p)--;
    int x = *((p)++);
    assert((x) == (*(--(p))));
    ((*(p)))++;
    ((*(p)))--;
    int (stk[16]) = {0};
    int (*sp) = stk;
    *((sp)++) = 1;
    *((sp)++) = 2;
    x = *(--(sp));
    assert((x) == (2));
    x = *(--(sp));
    assert((x) == (1));
    assert((sp) == (stk));
}

#line 833
void test1_test_lvalue(void) {
    test1_returns_ptr()->a = 5;
    #line 836
    test1_Thing (*p) = test1_returns_ptr_to_const();
}

#line 799
void test1_test_alignof(void) {
    int i = 42;
    ullong n1 = alignof(int);
    ullong n2 = alignof(int);
    ullong n3 = alignof(ullong);
    ullong n4 = alignof(int*);
}

#line 814
void test1_test_offsetof(void) {
    ullong n = offsetof(test1_BufHdr, buf);
}

#line 717
void test1_test_complete(void) {
    int x = 0;
    #line 721
    int y = 0;
    #line 723
    if ((x) == (0)) {
        y = 1;
    } else if ((x) == (1)) {
        y = 2;
    } else {
        #line 722
        assert("@complete if/elseif chain failed to handle case" && 0);
    }
    #line 729
    x = 1;
    #line 731
    assert((x) >= (0));
    #line 733
    x = 0;
    #line 737
    switch (x) {
        case 0: {
            y = 3;
            break;
        }
        case 1: {
            #line 741
            y = 4;
            break;
        }default:
            assert("@complete switch failed to handle case" && 0);
            break;
    }
}

#line 694
void test1_test_compound_literals(void) {
    test1_Vector (*w) = {0};
    w = &((test1_Vector){1, 2});
    int (a[3]) = {1, 2, 3};
    int i = 42;
    any temp = (any){(int[]){i}, TYPEID(8, TYPE_INT, int)};
    test1_println_any(temp);
    temp = (any){(int[]){42}, TYPEID(8, TYPE_INT, int)};
    test1_println_any(temp);
    any n = (any){(int[]){42}, TYPEID(8, TYPE_INT, int)};
    any x = {&(i), TYPEID(8, TYPE_INT, int)};
    any y = {&(i), TYPEID(8, TYPE_INT, int)};
    test1_Ints v = {.num_ints = 3, .int_ptr = (int []){1, 2, 3}, .int_arr = {1, 2, 3}};
    #line 711
    test1_Ints (ints_of_ints[]) = {{.num_ints = 3, .int_arr = {1, 2, 3}}, {.num_ints = 2, .int_ptr = (int [2]){-(1), -(2)}}};
}

#line 100
void test1_test_loops(void) {
    #line 103
    switch (0) {
        case 1: {
            break;
        }
        default: {
            #line 107
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

#line 493
void test1_test_sizeof(void) {
    int i = 0;
    ullong n = sizeof(i);
    n = sizeof(int);
    n = sizeof(int);
    n = sizeof(int*);
}

#line 329
void test1_test_assign(void) {
    int i = 0;
    float f = 3.14f;
    int (*p) = &(i);
    (i)++;
    (i)--;
    (p)++;
    (p)--;
    p += 1;
    i /= 2;
    i = 123;
    i %= 3;
    i <<= 1;
    i >>= 2;
    i &= 0xFF;
    i |= 0xFF00;
    i ^= 0xFF0;
}

#line 315
void test1_test_enum(void) {
    test1_Color a = TEST1_COLOR_RED;
    test1_Color b = TEST1_COLOR_RED;
    int c = (a) + (b);
    int i = a;
    a = i;
    (printf)("%d %d %d %d\n", TEST1_COLOR_NONE, TEST1_COLOR_RED, TEST1_COLOR_GREEN, TEST1_COLOR_BLUE);
    int d = TEST1_BAR;
    test1_TypedEnum e = TEST1_QUUX;
    #line 325
    test1_TypedEnum f = {0};
    f = TEST1_BAZ;
}

#line 91
void test1_test_arrays(void) {
    wchar_t (a[]) = {1, 2, 3};
    #line 94
    test1_f10(a);
    ushort (*b) = a;
    wchar_t w1 = {0};
    ushort w2 = w1;
}

#line 501
void test1_test_cast(void) {
    int (*p) = 0;
    uint64_t a = 0;
    #line 505
    a = (uint64_t)(p);
    #line 507
    p = (int*)(a);
}

#line 477
void test1_test_init(void) {
    int x = (int)(0);
    #line 480
    int y = {0};
    y = 0;
    int z = 42;
    int (a[]) = {1, 2, 3};
    #line 486
    for (ullong i = 0; (i) < (10); (i)++) {
        (printf)("%llu\n", i);
    }
    int (b[4]) = {1, 2, 3, 4};
    b[0] = a[2];
}

#line 363
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

#line 451
void test1_test_const(void) {
    test1_ConstVector cv2 = {1, 2};
    #line 454
    int i = 0;
    i = 1;
    #line 458
    int x = test1_cv.x;
    #line 460
    char c = test1_escape_to_char[0];
    c = test1_char_to_escape[c];
    c = test1_esc_test_str[0];
    test1_f4(test1_escape_to_char);
    char (*p) = (char*)(0);
    p = (test1_escape_to_char) + (1);
    char (*q) = (char*)(test1_escape_to_char);
    c = q['n'];
    #line 469
    p = (char*)(1);
    #line 472
    i = (int)((ullong)(p));
}

#line 412
void test1_test_bool(void) {
    bool b = false;
    b = true;
    int i = 0;
    i = TEST1_IS_DEBUG;
}

#line 380
void test1_test_ops(void) {
    float pi = 3.14f;
    float f = 0.0f;
    f = +(pi);
    f = -(pi);
    int n = -(1);
    n = ~(n);
    f = ((f) * (pi)) + (n);
    f = (pi) / (pi);
    n = (3) % (2);
    n = (n) + ((uchar)(1));
    int (*p) = &(n);
    p = (p) + (1);
    n = (int)((((p) + (1))) - (p));
    n = (n) << (1);
    n = (n) >> (1);
    int b = ((p) + (1)) > (p);
    b = ((p) + (1)) >= (p);
    b = ((p) + (1)) < (p);
    b = ((p) + (1)) <= (p);
    b = ((p) + (1)) == (p);
    b = (1) > (2);
    b = (1.23f) <= (pi);
    n = 0xFF;
    b = (n) & (~(1));
    b = (n) & (1);
    b = (((n) & (~(1)))) ^ (1);
    b = (p) && (pi);
}

#line 642
void test1_test_typeinfo(void) {
    int i = 42;
    float f = 3.14f;
    void (*p) = NULL;
    #line 647
    test1_println_any((any){(int[]){i}, TYPEID(8, TYPE_INT, int)});
    test1_println_any((any){(int[]){42}, TYPEID(8, TYPE_INT, int)});
    test1_println_any((any){&(i), TYPEID(8, TYPE_INT, int)});
    test1_println_any((any){&(f), TYPEID(14, TYPE_FLOAT, float)});
    test1_println_any((any){&(p), TYPEID(17, TYPE_PTR, void *)});
    #line 653
    test1_printf_any("Hello: %s %s %s\n", (any){(int[]){42}, TYPEID(8, TYPE_INT, int)}, (any){(float[]){3.14f}, TYPEID(14, TYPE_FLOAT, float)}, (any){(char *[]){"Per"}, TYPEID(16, TYPE_PTR, char *)});
    #line 655
    test1_println_type(TYPEID(8, TYPE_INT, int));
    test1_println_type(TYPEID(101, TYPE_PTR, const int *));
    test1_println_type(TYPEID(102, TYPE_ARRAY, const int * [42]));
    test1_println_type(TYPEID(103, TYPE_STRUCT, test1_UartCtrl));
    #line 660
    test1_println_typeinfo(TYPEID(8, TYPE_INT, int));
    test1_println_typeinfo(TYPEID(103, TYPE_STRUCT, test1_UartCtrl));
    test1_println_typeinfo(TYPEID(105, TYPE_PTR, test1_IntOrPtr *));
    test1_println_typeinfo(TYPEID(104, TYPE_UNION, test1_IntOrPtr));
}

#line 866
void test1_test_reachable(void) {
}

#line 916
void test1_test_type_path(void) {
    llong t1 = (time)(NULL);
    time_t t2 = (time)(NULL);
    test1_my_time_t t3 = (time)(NULL);
}

#line 636
void test1_test_push(void) {
    test1_push((any){(int[]){42}, TYPEID(8, TYPE_INT, int)});
    test1_push((any){(char *[]){"Hello"}, TYPEID(16, TYPE_PTR, char *)});
    test1_push((any){(char *[]){"World"}, TYPEID(16, TYPE_PTR, char *)});
}

#line 676
void test1_test_va_type(char (*fmt), ...) {
}

#line 672
void test1_test_void(void) {
    (void)(42);
}

#line 934
void test1_test_dynamic_arrays(void) {
    int (*a) = anew(int, 0);
    apush(int, (a), (42));
    apush(int, (a), (36));
    ullong len = alen(int, (a));
    ullong cap = acap(int, (a));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        (printf)("a[%d] = %d\n", i, a[i]);
    }
    asetcap(int, (a), (1));
    apush(int, (a), (82));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        (printf)("a[%d] = %d\n", i, a[i]);
    }
    apop(int, (a));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        (printf)("a[%d] = %d\n", i, a[i]);
    }
    int (*b) = 0;
    apush(int, (b), (1));
    apush(int, (b), (2));
    acat(int, (a), (b));
    afree(int, (b));
    afree(int, (a));
    apush(int, (a), (1));
    apush(int, (a), (2));
    afill(int, (a), (0), (15));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        printf("a[%d] = %d\n", i, a[i]);
    }
    acatn(int, (a), ((a) + (1)), (15));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        printf("a[%d] = %d\n", i, a[i]);
    }
    adeli(int, (a), (1));
    adeln(int, (a), (2), (10));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        printf("a[%d] = %d\n", i, a[i]);
    }
    afree(int, (a));
    test1_float2 (*c) = {0};
    #line 976
    apush(test1_float2, (c), ((test1_float2){1.0f, 2.0f}));
    afill(test1_float2, (c), ((test1_float2){3.14f, 1.42f}), (100));
    for (int i = 0; (i) < (alen(test1_float2, (c))); (i)++) {
        printf("[%d] {%f, %f}\n", i, c[i].x, c[i].y);
    }
}

#line 922
void test1_test_aprintf(void) {
    char (*a) = {0};
    aprintf(a, "Hello");
    aprintf(a, ", world! %d", 42);
    ullong len = alen(char, (a));
    printf("%s\n", a);
}

#line 983
void test1_test_index_arrays(void) {
    #line 985
    char (block[1024]) = {0};
    std_TempAllocator temp = std_temp_allocator(block, sizeof(block));
    int (*a) = 0;
    aindexv(int, (a), (std_hash_index(&(temp))));
    aputv(int, (a), (42));
    aputv(int, (a), (36));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        printf("%d\n", a[i]);
    }
    aputv(int, (a), (25));
    adelv(int, (a), (42));
    for (int i = 0; (i) < (alen(int, (a))); (i)++) {
        printf("%d\n", a[i]);
    }
    printf("%llu %llu %llu\n", agetvi(int, (a), (42)), agetvi(int, (a), (36)), agetvi(int, (a), (123)));
    assert(!(agetv(int, (a), (42))));
    assert(!(agetv(int, (a), (123))));
    assert(agetv(int, (a), (25)));
    assert(agetv(int, (a), (36)));
    #line 1005
    tuple139 (*m) = 0;
    aindex(tuple139, char, (m), (std_hash_index(&(temp))));
    aput(tuple139, char, (m), ('x'), (42));
    aput(tuple139, char, (m), ('p'), (36));
    for (int i = 0; (i) < (alen(tuple139, (m))); (i)++) {
        printf("[%d] %c => %d\n", i, m[i]._0, m[i]._1);
    }
    adel(tuple139, char, (m), ('x'));
    ullong j = ageti(tuple139, char, (m), ('p'));
    printf("[%llu] %c => %d\n", j, m[j]._0, m[j]._1);
    aput(tuple139, char, (m), ('m'), (38));
    adel(tuple139, char, (m), ('p'));
    for (int i = 0; (i) < (alen(tuple139, (m))); (i)++) {
        printf("[%d] %c => %d\n", i, m[i]._0, m[i]._1);
    }
    tuple143 (*b) = 0;
    aputv(tuple143, (b), ((tuple143){1, 2, 3}));
    ullong l = agetvi(tuple143, (b), ((tuple143){1, 2, 3}));
    l = agetvi(tuple143, (b), ((tuple143){4, 5, 6}));
    aputv(tuple143, (b), ((tuple143){4, 5, 6}));
    l = agetvi(tuple143, (b), ((tuple143){4, 5, 6}));
    l = agetvi(tuple143, (b), ((tuple143){7, 8, 9}));
}

#line 1058
void test1_test_tuples(void) {
    tuple146 p = {0};
    p._0 = 1.23f;
    p._1 = 3.14f;
    tuple149 (*counts) = {0};
}

#line 1075
void test1_test_func_interning(void) {
    test1_FuncPair p = {0};
    p.f = p.g;
    p.f = test1_funcpair_f;
}

#line 1035
void test1_test_hashing(void) {
    int (*a) = {0};
    aindexv(int, (a), (std_hash_index(0)));
    for (int x = 0; (x) < (11); (x)++) {
        aputv(int, (a), (x));
    }
    for (int x = 0; (x) < (11); (x)++) {
        ullong i = agetvi(int, (a), (x));
        assert(((i) < (alen(int, (a)))) && ((a[i]) == (x)));
    }
    aputv(int, (a), (11));
    for (int x = 0; (x) < (11); (x)++) {
        ullong i = agetvi(int, (a), (x));
        assert(((i) < (alen(int, (a)))) && ((a[i]) == (x)));
    }
    ullong i = agetvi(int, (a), (11));
    assert(((i) < (alen(int, (a)))) && ((a[i]) == (11)));
}

#line 1081
void test1_test_void_ptr_arithmetic(void) {
    void (*p) = {0};
    p = ((char *)p) + (1);
    p = (1) + ((char *)p);
    int (*q) = {0};
    #line 1087
    char (*r) = {0};
    llong n = ((char *)p) - (r);
    n = (r) - ((char *)p);
    int b = ((char *)p) <= ((char *)r);
    b = ((char *)r) <= ((char *)p);
    #line 1093
    p = (char )(p) + 1;
}

#line 1110
void test1_test_foreign_const(void) {
    int (*p) = (int *)(foreign_func());
    ForeignStruct s = {0};
    s.x = 42;
}

#line 1117
void test1_test_const_implicit(void) {
    int (*p) = {0};
    int (*q) = {0};
    q = p;
}

void test1_test_intern(void) {
    char (*a) = std_intern("Per");
    char (*b) = std_intern("Per");
    assert((a) == (b));
    char (*c) = std_intern("Ion");
    assert((a) != (c));
    char (*d) = std_intern("Ion");
    assert((c) == (d));
}

void test1_test_namemap(void) {
    char (block[(10) * (1024)]) = {0};
    std_TempAllocator temp = std_temp_allocator(block, sizeof(block));
    std_NameMap namemap = {0};
    std_namemap_init(&(namemap), &(temp));
    char (*a) = std_namemap_get(&(namemap), "Per");
    char (*b) = std_namemap_get(&(namemap), "Per");
    assert((a) == (b));
    char (*c) = std_namemap_get(&(namemap), "Ion");
    assert((a) != (c));
    char (*d) = std_namemap_get(&(namemap), "Ion");
    assert((c) == (d));
}

#line 1164
void test1_test_threadlocal(void) {
    test1_tls_test = 123;
}

#line 1188
void test1_test_new(void) {
    #line 1190
    test1_Person (*a) = ((test1_Person *)alloc_copy(sizeof(test1_Person), alignof(test1_Person), &((test1_Person){0})));
    test1_Person (*b) = ((test1_Person *)alloc_copy(sizeof(test1_Person), alignof(test1_Person), &(*(a))));
    test1_Person (*c) = ((test1_Person *)alloc_copy(sizeof(test1_Person), alignof(test1_Person), &((test1_Person){.name = "Per", .age = 37})));
    Allocator (*current) = current_allocator;
    std_TraceAllocator trace = std_trace_allocator(current);
    current_allocator = &(trace);
    void * (*ptrs) = 0;
    for (int i = 0; (i) < (32); (i)++) {
        apush(void *, (ptrs), (tls_alloc((16) + (i), 8)));
    }
    for (int i = 0; (i) < (alen(void *, (ptrs))); (i)++) {
        tls_free(ptrs[i]);
    }
    afree(void *, (ptrs));
    current_allocator = current;
    char (block[(32) * (1024)]) = {0};
    std_TempAllocator temp = std_temp_allocator(block, sizeof(block));
    test1_Person (*e) = ((test1_Person *)generic_alloc_copy((Allocator *)(&(temp)), sizeof(test1_Person), alignof(test1_Person), &((test1_Person){.name = "Per", .age = 37})));
    std_TempMark mark = std_temp_begin(&(temp));
    std_ArenaAllocator arena = std_arena_allocator(&(temp));
    #line 1211
    std_temp_end(&(temp), mark);
    float (*g) = ((float *)generic_alloc_copy((Allocator *)(&(temp)), sizeof(float), alignof(float), &((float){1.42f})));
}

#line 1242
std_File * test1_test_disposable(void) {
    std_Recover ctx = {0};
    if (recover(&(ctx))) {
        std_File (*file1) = std_open("../dummy.txt");
        std_secure(file1);
        std_File (*file2) = std_open("../dummy.txt");
        std_secure(file2);
        test1_test_panic(&(ctx), 10);
        assert(0);
        #line 1253
        std_dispose(&(ctx));
        return file1;
    }
    assert((alen(std_Disposable *, (std_disposables))) == (0));
    return 0;
}

#line 1147
void test1_test_aget(void) {
    tuple198 (*a) = 0;
    aput(tuple198, int, (a), (251182), (37));
    ullong i = ageti(tuple198, int, (a), (251182));
    ullong j = ageti(tuple198, int, (a), (1234));
    int x = aget(tuple198, int, int, (a), (251182));
    int y = aget(tuple198, int, int, (a), (1234));
    adefault(tuple198, int, (a), (-(1)));
    y = aget(tuple198, int, int, (a), (1234));
    {
        #line 1156
        int (*p) = agetp(tuple198, int, int, (a), (251182));
        if (p) {
            #line 1157
            int z = *(p);
        }
    }
}

#line 1237
void test1_test_tuple_deps(void) {
    test1_x;
}

#line 1260
void test1_test_autohash(void) {
    int (*a) = 0;
    for (int i = 0; (i) < (100); (i)++) {
        aputv(int, (a), (i));
    }
    assert((ahdr(int, (a))->index.indexer) == (std_hash_indexer));
    for (int i = 0; (i) < (100); (i)++) {
        assert(agetv(int, (a), (i)));
    }
    tuple209 (*m) = 0;
    for (int i = 0; (i) < (100); (i)++) {
        aput(tuple209, int, (m), (i), (i));
    }
    assert((ahdr(tuple209, (m))->index.indexer) == (std_hash_indexer));
    for (int i = 0; (i) < (100); (i)++) {
        assert((aget(tuple209, int, int, (m), (i))) == (i));
    }
}

void test1_test_undef(void) {
    int x = {0};
    char (block[1024]) = {0};
    std_TempAllocator temp = std_temp_allocator(block, sizeof(block));
    int (*a) = ((int *)tls_alloc(sizeof(int), alignof(int)));
    int (*b) = ((int *)generic_alloc((Allocator *)(&(temp)), sizeof(int), alignof(int)));
    int (*c) = ((int *)tls_alloc(1024 * sizeof(int), alignof(int)));
    int (*d) = ((int *)generic_alloc((Allocator *)(&(temp)), 1024* sizeof(int), alignof(int)));
    int (e[]) = {1, 2, 3, 4};
    int (*f) = ((int *)alloc_copy(4 * sizeof(int), alignof(int), &(e)));
    for (int i = 0; (i) < (4); (i)++) {
        printf("%d ", f[i]);
    }
}

char test1_c = 1;

#line 22 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
int test1_subtest1_func1(void) {
    test1_subtest1_func2();
    return 42;
}

#line 825 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
test1_Thing * test1_returns_ptr(void) {
    return &(test1_thing);
}

test1_Thing * test1_returns_ptr_to_const(void) {
    return &(test1_thing);
}

#line 529
void test1_println_any(any x) {
    test1_print_any(x);
    printf("\n");
}

#line 87
void test1_f10(ushort (a[3])) {
    a[1] = 42;
}

test1_Vector test1_cv;

char (test1_escape_to_char[256]) = {['0'] = '\0', ['\''] = '\'', ['\"'] = '\"', ['\\'] = '\\', ['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a'};

char (test1_char_to_escape[256]) = {['\0'] = '0', ['\n'] = 'n', ['\r'] = 'r', ['\t'] = 't', ['\v'] = 'v', ['\b'] = 'b', ['\a'] = 'a', ['\\'] = '\\', ['\"'] = '\"', ['\''] = '\''};

char (*test1_esc_test_str) = "Hello\nworld\nHex: \x1\x10\xfHello\xff";

#line 433
void test1_f4(char (*x)) {
}

#line 575
void test1_printf_any(char (*fmt), ...) {
    va_list args = {0};
    va_start(args, fmt);
    char (*start) = fmt;
    while (*(fmt)) {
        while ((*(fmt)) && ((*(fmt)) != ('%'))) {
            (fmt)++;
        }
        if ((*(fmt)) == ('%')) {
            (fmt)++;
            if ((*(fmt)) == ('s')) {
                printf("%.*s", (int)(((fmt) - (start)) - (1)), start);
                any arg = {0};
                arg = va_arg(args, any);
                test1_print_any_value(arg);
                (fmt)++;
                start = fmt;
            }
        }
    }
    printf("%.*s", (int)((fmt) - (start)), start);
    va_end(args);
}

#line 570
void test1_println_type(ullong type) {
    test1_print_type(type);
    printf("\n");
}

#line 622
void test1_println_typeinfo(ullong type) {
    test1_print_typeinfo(type);
    printf("\n");
}

#line 630
void test1_push(any x) {
    ullong n = typeid_size(x.type);
    (memcpy)(test1_stack_ptr, x.ptr, n);
    test1_stack_ptr += n;
}

#line 164
std_TempAllocator std_temp_allocator(void (*buf), ullong size) {
    return (std_TempAllocator){{std_temp_alloc, std_noop_free}, buf, buf, ((char *)buf) + (size)};
}

#line 606
std_Index std_hash_index(Allocator (*allocator)) {
    std_HashIndex (*index) = ((std_HashIndex *)generic_alloc_copy((Allocator *)(allocator), sizeof(std_HashIndex), alignof(std_HashIndex), &((std_HashIndex){0})));
    std_hash_init(index, 0, allocator);
    return (std_Index){.data = index, .indexer = std_hash_indexer};
}

#line 1071 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
int test1_funcpair_f(int x, int y) {
    return (x) + (y);
}

#line 380 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
char * std_intern(char (*str)) {
    return std_namemap_get(&(std_intern_namemap), str);
}

#line 336
void std_namemap_init(std_NameMap (*self), Allocator (*allocator)) {
    self->arena = std_arena_allocator(allocator);
    self->nodes = anew(tuple162, &(self->arena));
    self->collisions = anew(std_NameNode *, &(self->arena));
}

#line 374
char * std_namemap_get(std_NameMap (*self), char (*str)) {
    return std_namemap_getn(self, str, (strlen)(str));
}

THREADLOCAL int test1_tls_test = 42;

#line 321
std_TraceAllocator std_trace_allocator(Allocator (*allocator)) {
    return (std_TraceAllocator){{std_trace_alloc, std_trace_free}, .allocator = allocator, .events = anew(std_AllocatorEvent, allocator)};
}

#line 168
std_TempMark std_temp_begin(std_TempAllocator (*self)) {
    return (std_TempMark){self->next};
}

#line 190
std_ArenaAllocator std_arena_allocator(void (*allocator)) {
    return (std_ArenaAllocator){{std_arena_alloc, std_noop_free}, allocator, STD_ARENA_MIN_BLOCK_SIZE};
}

#line 172
void std_temp_end(std_TempAllocator (*self), std_TempMark mark) {
    void (*ptr) = mark.ptr;
    assert(((self->start) <= (ptr)) && ((ptr) <= (self->end)));
    self->next = ptr;
}

#line 45
std_File * std_open(char (*path)) {
    FILE (*libc_file) = (fopen)(path, "r");
    if (!(libc_file)) {
        return 0;
    }
    return ((std_File *)alloc_copy(sizeof(std_File), alignof(std_File), &((std_File){std_make_disposable(std_file_dispose), libc_file})));
}

#line 77
void * std_secure(void (*data)) {
    if (!(std_secured(data))) {
        std_Disposable (*self) = data;
        self->mark = apush(std_Disposable *, (std_disposables), (self));
    }
    return data;
}

#line 1215 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_test_panic(std_Recover (*ctx), int i) {
    if ((i) == (0)) {
        std_panic(ctx);
    } else {
        test1_test_panic(ctx, (i) - (1));
    }
}

#line 94 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
void std_dispose(void (*data)) {
    if (!(data)) {
        return;
    }
    std_Disposable (*self) = data;
    ullong mark = self->mark;
    for (ullong i = alen(std_Disposable *, (std_disposables)); (i) > (mark); (i)--) {
        std_Disposable (*disposable) = std_disposables[(i) - (1)];
        if (disposable) {
            disposable->dispose(disposable);
            std_disposables[(i) - (1)] = 0;
        }
    }
    asetlen(std_Disposable *, (std_disposables), (mark));
}

THREADLOCAL std_Disposable * (*std_disposables);

test1_SillyStruct test1_x;

#line 525
ullong std_hash_get(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size) {
    std_HashIndex (*index) = data;
    ullong h = std_hash(x, size);
    std_HashSlot (*slot) = std_hash_get_slot(index, a, x, h, stride, size);
    if ((slot->i) == (STD_HASH_EMPTY)) {
        return len;
    } else {
        return slot->i;
    }
}

ullong std_hash_put(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size) {
    std_HashIndex (*index) = data;
    ullong h = std_hash(x, size);
    std_HashSlot (*slot) = std_hash_get_slot(index, a, x, h, stride, size);
    if ((slot->i) == (STD_HASH_EMPTY)) {
        *(slot) = (std_HashSlot){len, h};
        (index->occupied)++;
        if ((index->occupied) >= (index->max_occupied)) {
            std_hash_rehash(index, (len) + (1));
        }
        return len;
    } else {
        return slot->i;
    }
}

ullong std_hash_del(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size) {
    std_HashIndex (*index) = data;
    ullong h = std_hash(x, size);
    std_HashSlot (*slot) = std_hash_get_slot(index, a, x, h, stride, size);
    uint i = slot->i;
    if ((i) != (STD_HASH_EMPTY)) {
        *(slot) = (std_HashSlot){STD_HASH_DELETED};
        return i;
    } else {
        return len;
    }
}

void std_hash_set(void (*data), void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size) {
    std_HashIndex (*index) = data;
    ullong h = std_hash(x, size);
    std_HashSlot (*slot) = std_hash_get_slot(index, a, x, h, stride, size);
    uint i = slot->i;
    *(slot) = (std_HashSlot){xi, h};
    if ((i) == (STD_HASH_EMPTY)) {
        (index->occupied)++;
        if ((index->occupied) >= (index->max_occupied)) {
            std_hash_rehash(index, (len) + (1));
        }
    }
}

void std_hash_free(void (*data)) {
    std_HashIndex (*index) = data;
    afree(std_HashSlot, (index->slots));
    generic_free(index->allocator, index);
}

std_Indexer (*std_hash_indexer) = &((std_Indexer){.get = std_hash_get, .put = std_hash_put, .del = std_hash_del, .set = std_hash_set, .free = std_hash_free});

#line 17 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
void test1_subtest1_func2(void) {
    test1_subtest1_func3();
    test1_subtest1_func4();
}

test1_Thing test1_thing;

#line 523 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_print_any(any x) {
    test1_print_any_value(x);
    printf(": ");
    test1_print_type(x.type);
}

#line 510
void test1_print_any_value(any x) {
    switch (x.type) {
        case TYPEID(8, TYPE_INT, int): {
            printf("%d", *((int*)(x.ptr)));
            break;
        }
        case TYPEID(14, TYPE_FLOAT, float): {
            #line 515
            printf("%f", *((float*)(x.ptr)));
            break;
        }
        case TYPEID(41, TYPE_PTR, const char *):
        case TYPEID(16, TYPE_PTR, char *): {
            #line 517
            printf("%s", *((char**)(x.ptr)));
            break;
        }
        default: {
            #line 519
            printf("<unknown>");
            break;
        }
    }
}

#line 539
void test1_print_type(ullong type) {
    TypeInfo (*typeinfo) = get_typeinfo(type);
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
            #line 550
            test1_print_type(typeinfo->base);
            printf(" const");
            break;
        }
        case TYPE_ARRAY: {
            #line 553
            test1_print_type(typeinfo->base);
            printf("[%d]", typeinfo->count);
            break;
        }
        default: {
            #line 556
            if (typeinfo->name) {
                printf("%s", typeinfo->name);
            } else {
                test1_print_typeid(type);
            }
            break;
        }
    }
}

#line 599
void test1_print_typeinfo(ullong type) {
    TypeInfo (*typeinfo) = get_typeinfo(type);
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
            #line 610
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
    #line 619
    printf(">");
}

char (test1_stack[1024]);

char (*test1_stack_ptr) = test1_stack;

#line 735 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
INLINE
void * std_anew_func(Allocator (*allocator), ullong elem_size, ullong elem_align) {
    #line 736
    ullong size = (elem_size) + (std_ahdrsize_func(elem_size, elem_align));
    ullong align = std_ahdralign_func(elem_size, elem_align);
    void (*mem) = generic_alloc(allocator, size, align);
    std_Ahdr (*hdr) = (std_Ahdr*)((((char *)mem) + (elem_size)));
    hdr->allocator = allocator;
    hdr->len = 0;
    hdr->cap = 0;
    hdr->index = (std_Index){.indexer = std_default_indexer};
    (memset)(mem, 0, elem_size);
    return hdr->buf;
}

#line 768
INLINE
ullong std_apush_func(void * (*ap), void (*x), ullong elem_size, ullong elem_align) {
    #line 769
    void (*a) = *(ap);
    ullong i = std_alen_func(a, elem_size, elem_align);
    if ((i) >= (std_acap_func(a, elem_size, elem_align))) {
        std_asetcap_func(ap, (i) + (1), elem_size, elem_align);
        a = *(ap);
    }
    (std_ahdr_func(a, elem_size, elem_align)->len)++;
    (memcpy)(((char *)a) + ((i) * (elem_size)), x, elem_size);
    return i;
}

#line 683
INLINE
ullong std_alen_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 684
    return (a ? std_ahdr_func(a, elem_size, elem_align)->len : 0);
}

#line 688
INLINE
ullong std_acap_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 689
    return (a ? std_ahdr_func(a, elem_size, elem_align)->cap : 0);
}

#line 832
NOINLINE
void std_asetcap_func(void * (*ap), ullong new_cap, ullong elem_size, ullong elem_align) {
    #line 833
    void (*a) = *(ap);
    ullong cap = std_acap_func(a, elem_size, elem_align);
    if ((new_cap) > (cap)) {
        #line 841
        if (((2) * (new_cap)) < ((3) * (cap))) {
            new_cap = (cap) + ((cap) / (2));
        }
    }
    ullong hdrsize = std_ahdrsize_func(elem_size, elem_align);
    ullong size = ((elem_size) + (hdrsize)) + ((new_cap) * (elem_size));
    ullong align = std_ahdralign_func(elem_size, elem_align);
    void (*new_mem) = {0};
    std_Ahdr (*new_hdr) = {0};
    if (a) {
        std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
        ullong new_len = hdr->len;
        if ((new_len) > (new_cap)) {
            new_len = new_cap;
        }
        void (*mem) = std_amem_func(a, elem_size, elem_align);
        new_mem = generic_alloc(hdr->allocator, size, align);
        new_hdr = (std_Ahdr*)((((char *)new_mem) + (elem_size)));
        (memcpy)(new_mem, mem, ((elem_size) + (hdrsize)) + ((new_len) * (elem_size)));
        new_hdr->len = new_len;
        generic_free(hdr->allocator, mem);
    } else {
        new_mem = tls_alloc(size, align);
        new_hdr = (std_Ahdr*)((((char *)new_mem) + (elem_size)));
        new_hdr->allocator = current_allocator;
        new_hdr->len = 0;
        new_hdr->index = (std_Index){.indexer = std_default_indexer};
        (memset)(new_mem, 0, elem_size);
    }
    new_hdr->cap = new_cap;
    *(ap) = new_hdr->buf;
}

#line 749
INLINE
void std_apop_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 750
    if ((std_alen_func(a, elem_size, elem_align)) > (0)) {
        (std_ahdr_func(a, elem_size, elem_align)->len)--;
    }
}

#line 804
void std_acat_func(void * (*ap), void (*src), ullong elem_size, ullong elem_align) {
    std_acatn_func(ap, src, std_alen_func(src, elem_size, elem_align), elem_size, elem_align);
}

#line 725
INLINE
void std_afree_func(void * (*ap), ullong elem_size, ullong elem_align) {
    #line 726
    {
        #line 726
        void (*a) = *(ap);
        if (a) {
            #line 727
            std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
            std_index_free(hdr->index);
            generic_free(hdr->allocator, std_amem_func(a, elem_size, elem_align));
        }
    }
    #line 731
    *(ap) = 0;
}

#line 755
void std_afill_func(void * (*ap), void (*x), ullong n, ullong elem_size, ullong elem_align) {
    ullong len = std_alen_func(*(ap), elem_size, elem_align);
    std_afit_func(ap, (len) + (n), elem_size, elem_align);
    void (*a) = *(ap);
    char (*p) = ((char *)a) + ((len) * (elem_size));
    for (int i = 0; (i) < (n); (i)++) {
        (memcpy)(p, x, elem_size);
        p += elem_size;
    }
    std_ahdr_func(a, elem_size, elem_align)->len += n;
}

#line 792
void std_acatn_func(void * (*ap), void (*src), ullong src_len, ullong elem_size, ullong elem_align) {
    void (*a) = *(ap);
    ullong len = std_alen_func(a, elem_size, elem_align);
    std_afit_func(ap, (len) + (src_len), elem_size, elem_align);
    if ((((a) != (*(ap))) && ((a) <= (src))) && (((char *)src) <= ((char *)((char *)a) + ((len) * (elem_size))))) {
        src = ((char *)*(ap)) + ((((char *)src) - ((char *)a)));
    }
    a = *(ap);
    (memmove)(((char *)a) + ((len) * (elem_size)), src, (src_len) * (elem_size));
    std_ahdr_func(a, elem_size, elem_align)->len += src_len;
}

#line 780
void std_adeln_func(void (*a), ullong i, ullong n, ullong elem_size, ullong elem_align) {
    ullong len = std_alen_func(a, elem_size, elem_align);
    if ((i) >= (len)) {
        return;
    }
    if (((i) + (n)) > (len)) {
        n = (len) - (i);
    }
    (memmove)(((char *)a) + ((i) * (elem_size)), ((char *)a) + ((((i) + (n))) * (elem_size)), (((len) - (((i) + (n))))) * (elem_size));
    std_ahdr_func(a, elem_size, elem_align)->len -= n;
}

#line 808
void std_aprintf_func(void * (*ap), char (*fmt), ...) {
    void (*a) = *(ap);
    va_list args = {0};
    va_start(args, fmt);
    ullong cap = std_acap_func(a, sizeof(char), alignof(char));
    ullong len = std_alen_func(a, sizeof(char), alignof(char));
    ullong slack = (cap) - (len);
    int printed = ((vsnprintf)(((char *)a) + (len), slack, fmt, args)) + (1);
    va_end(args);
    if ((printed) > (slack)) {
        std_afit_func(ap, (len) + (printed), sizeof(char), alignof(char));
        a = *(ap);
        va_start(args, fmt);
        cap = std_acap_func(a, sizeof(char), alignof(char));
        len = std_alen_func(a, sizeof(char), alignof(char));
        slack = (cap) - (len);
        printed = ((vsnprintf)(((char *)a) + (len), slack, fmt, args)) + (1);
        va_end(args);
    }
    std_ahdr_func(a, sizeof(char), alignof(char))->len += (printed) - (1);
    *(ap) = a;
}

#line 153
void * std_temp_alloc(void (*allocator), ullong size, ullong align) {
    std_TempAllocator (*self) = allocator;
    ullong aligned = (((((uintptr_t)(self->next)) + (align)) - (1))) & (~(((align) - (1))));
    ullong next = (aligned) + (size);
    if ((next) > ((uintptr_t)(self->end))) {
        return 0;
    }
    self->next = (void*)(next);
    return (void*)(aligned);
}

#line 137
void std_noop_free(void (*data), void (*ptr)) {
}

#line 894
void std_aindex_func(void * (*ap), std_Index new_index, ullong key_size, ullong elem_size, ullong elem_align) {
    void (*a) = *(ap);
    if (!(a)) {
        *(ap) = std_anew_func(0, elem_size, elem_align);
        a = *(ap);
    }
    std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
    std_Index index = hdr->index;
    std_index_free(index);
    hdr->index = new_index;
    for (int i = 0; (i) < (hdr->len); (i)++) {
        std_index_set(new_index, a, ((char *)a) + ((i) * (elem_size)), i, hdr->len, elem_size, key_size);
    }
}

#line 491
void std_hash_init(std_HashIndex (*index), uint len, Allocator (*allocator)) {
    uint num_slots = std_next_pow2((len) + ((len) / (2)));
    if ((num_slots) < (STD_HASH_MIN_SLOTS)) {
        num_slots = STD_HASH_MIN_SLOTS;
    }
    *(index) = (std_HashIndex){.mask = (num_slots) - (1), .max_occupied = ((num_slots) / (2)) + ((num_slots) / (4)), .allocator = allocator};
    index->slots = anew(std_HashSlot, allocator);
    afill(std_HashSlot, (index->slots), ((std_HashSlot){STD_HASH_EMPTY}), (num_slots));
}

#line 933
ullong std_aput_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    void (*a) = *(ap);
    if (!(a)) {
        *(ap) = std_anew_func(0, elem_size, elem_align);
        a = *(ap);
    }
    std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
    if (((hdr->len) >= (32)) && ((hdr->index.indexer) == (std_default_indexer))) {
        std_aindex_func(ap, std_hash_index(hdr->allocator), key_size, elem_size, elem_align);
    }
    ullong i = std_index_put(hdr->index, a, x, hdr->len, elem_size, key_size);
    if ((i) == (hdr->len)) {
        std_afit_func(ap, (hdr->len) + (1), elem_size, elem_align);
        a = *(ap);
        (std_ahdr_func(a, elem_size, elem_align)->len)++;
    }
    (memcpy)(((char *)a) + ((i) * (elem_size)), x, elem_size);
    return i;
}

void std_adel_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    if (!(a)) {
        return;
    }
    std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
    ullong i = std_index_del(hdr->index, a, x, hdr->len, elem_size, key_size);
    if ((i) == (hdr->len)) {
        return;
    }
    if ((i) < ((hdr->len) - (1))) {
        char (*p) = ((char *)a) + ((i) * (elem_size));
        char (*q) = ((char *)a) + ((((hdr->len) - (1))) * (elem_size));
        std_index_set(hdr->index, a, q, i, hdr->len, elem_size, key_size);
        (memmove)(p, q, elem_size);
    }
    (hdr->len)--;
}

#line 909
ullong std_ageti_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    if (!(a)) {
        return 0;
    }
    std_Ahdr (*hdr) = std_ahdr_func(a, elem_size, elem_align);
    return std_index_get(hdr->index, a, x, hdr->len, elem_size, key_size);
}

void * std_agetp_func(void (*a), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    ullong i = std_ageti_func(a, x, key_size, elem_size, elem_align);
    return ((i) != (std_alen_func(a, elem_size, elem_align)) ? (((char *)a) + ((i) * (elem_size))) + (((elem_size) - (key_size))) : 0);
}

std_NameMap std_intern_namemap;

#line 348
char * std_namemap_getn(std_NameMap (*self), char (*buf), ullong len) {
    assert((len) <= (UINT32_MAX));
    ullong h = std_hash(buf, len);
    std_NameNode (*node) = aget(tuple162, ullong, std_NameNode *, (self->nodes), (h));
    if (node) {
        if (((node->len) == (len)) && (((memcmp)(node->buf, buf, len)) == (0))) {
            return node->buf;
        }
        for (int i = 0; (i) < (alen(std_NameNode *, (self->collisions))); (i)++) {
            {
                #line 357
                std_NameNode (*node2) = self->collisions[i];
                if (((node2->len) == (len)) && (((memcmp)(node2->buf, buf, len)) == (0))) {
                    #line 358
                    return node2->buf;
                }
            }
        }
    }
    #line 362
    std_NameNode (*new_node) = std_arena_alloc(&(self->arena), ((offsetof(std_NameNode, buf)) + (len)) + (1), alignof(std_NameNode));
    new_node->len = len;
    (memcpy)(new_node->buf, buf, len);
    new_node->buf[len] = 0;
    if (node) {
        apush(std_NameNode *, (self->collisions), (new_node));
    } else {
        aput(tuple162, ullong, (self->nodes), (h), (new_node));
    }
    return new_node->buf;
}

#line 308
void * std_trace_alloc(void (*allocator), ullong size, ullong align) {
    std_TraceAllocator (*self) = allocator;
    void (*ptr) = generic_alloc(self->allocator, size, align);
    apush(std_AllocatorEvent, (self->events), ((std_AllocatorEvent){STD_EVENT_ALLOC, (time)(0), ptr, size, align}));
    return ptr;
}

void std_trace_free(void (*allocator), void (*ptr)) {
    std_TraceAllocator (*self) = allocator;
    generic_free(self->allocator, ptr);
    apush(std_AllocatorEvent, (self->events), ((std_AllocatorEvent){STD_EVENT_FREE, (time)(0), ptr}));
}

#line 214
void * std_arena_alloc(void (*allocator), ullong size, ullong align) {
    std_ArenaAllocator (*self) = allocator;
    ullong aligned = (((((uintptr_t)(self->next)) + (align)) - (1))) & (~(((align) - (1))));
    ullong next = (aligned) + (size);
    if ((next) > ((uintptr_t)(self->end))) {
        return std_arena_alloc_grow(self, size, align);
    }
    self->next = (char*)(next);
    return (void*)(aligned);
}

#line 64
std_Disposable std_make_disposable(void(*dispose)(void *)) {
    return (std_Disposable){dispose, alen(std_Disposable *, (std_disposables))};
}

#line 115
void std_recover_dispose(void (*data)) {
    std_dispose(data);
}

THREADLOCAL std_Recover (*std_temp_ctx);

#line 36
void std_file_dispose(void (*data)) {
    std_File (*self) = data;
    if (self->libc_file) {
        (fclose)(self->libc_file);
        self->libc_file = 0;
    }
    (free)(self);
}

#line 68
bool std_secured(void (*data)) {
    if (!(data)) {
        return false;
    }
    std_Disposable (*self) = data;
    ullong mark = self->mark;
    return ((mark) < (alen(std_Disposable *, (std_disposables)))) && ((std_disposables[mark]) == (self));
}

#line 132
void std_panic(std_Recover (*ctx)) {
    std_dispose(ctx);
    (longjmp)(ctx->libc_env, 1);
}

#line 922
void * std_aget_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    if (!(*(ap))) {
        *(ap) = std_anew_func(0, elem_size, elem_align);
    }
    void (*a) = *(ap);
    ullong i = std_ageti_func(a, x, key_size, elem_size, elem_align);
    ullong len = std_alen_func(a, elem_size, elem_align);
    void (*def) = std_adefault_func(a, elem_size, elem_align);
    return ((i) != (len) ? (((char *)a) + ((i) * (elem_size))) + (((elem_size) - (key_size))) : ((char *)def) + (((elem_size) - (key_size))));
}

#line 879
void std_asetdefault_func(void * (*ap), void (*x), ullong key_size, ullong elem_size, ullong elem_align) {
    if (!(*(ap))) {
        *(ap) = std_anew_func(0, elem_size, elem_align);
    }
    void (*a) = *(ap);
    (memcpy)(((char *)std_amem_func(a, elem_size, elem_align)) + (key_size), x, (elem_size) - (key_size));
}

#line 678
INLINE
std_Ahdr * std_ahdr_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 679
    return (a ? (std_Ahdr*)((((char *)a) - (std_ahdrsize_func(elem_size, elem_align)))) : 0);
}

#line 469
ullong std_hash(void (*buf), ullong size) {
    uint64_t h = 0xCBF29CE484222325;
    char (*ptr) = buf;
    for (int i = 0; (i) < (size); (i)++) {
        h ^= *((ptr)++);
        h = 0x100000001B3;
        h ^= (h) >> (32);
    }
    return h;
}

#line 501
std_HashSlot * std_hash_get_slot(std_HashIndex (*index), void (*a), void (*x), uint h, ullong stride, ullong size) {
    for (uint i = h;;) {
        i &= index->mask;
        std_HashSlot (*slot) = &(index->slots[i]);
        if (((slot->i) == (STD_HASH_EMPTY)) || ((((slot->h) == (h)) && (((memcmp)(((char *)a) + ((slot->i) * (stride)), x, size)) == (0))))) {
            return slot;
        }
        (i)++;
    }
    return 0;
}

#line 585
void std_hash_rehash(std_HashIndex (*index), uint len) {
    std_HashIndex new_index = {0};
    std_hash_init(&(new_index), len, index->allocator);
    for (int i = 0; (i) < (alen(std_HashSlot, (index->slots))); (i)++) {
        std_HashSlot slot = index->slots[i];
        if ((slot.i) < (STD_HASH_DELETED)) {
            std_hash_put_slot(&(new_index), slot);
        }
    }
    afree(std_HashSlot, (index->slots));
    *(index) = new_index;
}

#line 9 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/subtest1/subtest1.ion"
void test1_subtest1_func3(void) {
    #line 11
    (printf)("LIBC func3\n");
}

void test1_subtest1_func4(void) {
}

#line 534 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/test1/test1.ion"
void test1_print_typeid(ullong type) {
    int index = typeid_index(type);
    printf("typeid(%d)", index);
}

#line 668 "C:/Users/john/source/repos/IonDotNet/ion-pkgs/system_packages/std/std.ion"
INLINE
ullong std_ahdrsize_func(ullong elem_size, ullong elem_align) {
    #line 669
    return ((((offsetof(std_Ahdr, buf)) + (elem_align)) - (1))) & (~(((elem_align) - (1))));
}

#line 673
INLINE
ullong std_ahdralign_func(ullong elem_size, ullong elem_align) {
    #line 674
    return ((elem_align) > (alignof(std_Ahdr)) ? elem_align : alignof(std_Ahdr));
}

#line 433
ullong std_linear_get(void (*data), void (*a), void (*x), ullong len, ullong stride, ullong size) {
    for (int i = 0; (i) < (len); (i)++) {
        if (((memcmp)(((char *)a) + ((i) * (stride)), x, size)) == (0)) {
            return i;
        }
    }
    return len;
}

#line 427
void std_null_set(void (*data), void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size) {
}

void std_null_free(void (*data)) {
}

std_Indexer (*std_default_indexer) = &((std_Indexer){.get = std_linear_get, .put = std_linear_get, .del = std_linear_get, .set = std_null_set, .free = std_null_free});

#line 693
INLINE
void * std_amem_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 694
    return (a ? (((char *)a) - (std_ahdrsize_func(elem_size, elem_align))) - (elem_size) : 0);
}

#line 423
INLINE
void std_index_free(std_Index index) {
    #line 424
    index.indexer->free(index.data);
}

#line 888
INLINE
void std_afit_func(void * (*ap), ullong min_cap, ullong elem_size, ullong elem_align) {
    #line 889
    if ((min_cap) > (std_acap_func(*(ap), elem_size, elem_align))) {
        std_asetcap_func(ap, min_cap, elem_size, elem_align);
    }
}

#line 417
INLINE
void std_index_set(std_Index index, void (*a), void (*x), ullong xi, ullong len, ullong stride, ullong size) {
    #line 418
    assert((xi) < (len));
    index.indexer->set(index.data, a, x, xi, len, stride, size);
}

#line 480
uint std_next_pow2(uint x) {
    (x)--;
    x |= (x) >> (1);
    x |= (x) >> (2);
    x |= (x) >> (4);
    x |= (x) >> (8);
    x |= (x) >> (16);
    (x)++;
    return x;
}

#line 407
INLINE
ullong std_index_put(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size) {
    #line 408
    return index.indexer->put(index.data, a, x, len, stride, size);
}

#line 412
INLINE
ullong std_index_del(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size) {
    #line 413
    return index.indexer->del(index.data, a, x, len, stride, size);
}

#line 402
INLINE
ullong std_index_get(std_Index index, void (*a), void (*x), ullong len, ullong stride, ullong size) {
    #line 403
    return index.indexer->get(index.data, a, x, len, stride, size);
}

#line 194
void * std_arena_alloc_grow(std_ArenaAllocator (*self), ullong size, ullong align) {
    ullong block_size = (2) * (self->block_size);
    if ((block_size) < (size)) {
        block_size = size;
    }
    ullong block_align = STD_ARENA_MIN_BLOCK_ALIGN;
    if ((block_align) < (align)) {
        block_align = align;
    }
    char (*block) = generic_alloc(self->allocator, block_size, block_align);
    if (!(block)) {
        return 0;
    }
    apush(char *, (self->blocks), (block));
    self->block_size = block_size;
    self->next = (block) + (size);
    self->end = (block) + (block_size);
    return block;
}

#line 705
INLINE
ullong std_asetlen_func(void (*a), ullong new_len, ullong elem_size, ullong elem_align) {
    #line 706
    if (!(a)) {
        return 0;
    }
    ullong cap = std_acap_func(a, elem_size, elem_align);
    if ((new_len) > (cap)) {
        new_len = cap;
    }
    std_ahdr_func(a, elem_size, elem_align)->len = new_len;
    return new_len;
}

#line 875
INLINE
void * std_adefault_func(void (*a), ullong elem_size, ullong elem_align) {
    #line 876
    return (a ? std_amem_func(a, elem_size, elem_align) : 0);
}

#line 513
void std_hash_put_slot(std_HashIndex (*index), std_HashSlot new_slot) {
    for (uint i = new_slot.h;;) {
        i &= index->mask;
        {
            #line 516
            std_HashSlot (*slot) = &(index->slots[i]);
            if ((slot->i) == (STD_HASH_EMPTY)) {
                #line 517
                *(slot) = new_slot;
                (index->occupied)++;
                return;
            }
        }
        #line 521
        (i)++;
    }
}

// -Foreign Sources-
// -Postamble-
#ifdef __GNUC__
#pragma GCC diagnostic pop
#endif
