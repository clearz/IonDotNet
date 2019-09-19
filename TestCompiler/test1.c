// Preamble
#include <stdio.h>
#include <math.h>
#include <stdbool.h>

typedef unsigned char uchar;
typedef signed char schar;
typedef unsigned short ushort;
typedef unsigned int uint;
typedef unsigned long ulong;
typedef long long llong;
typedef unsigned long long ullong;

typedef uchar uint8;
typedef schar int8;
typedef ushort uint16;
typedef short int16;
typedef uint uint32;
typedef int int32;
typedef ullong uint64;
typedef llong int64;

// Forward declarations
typedef struct S1 S1;
typedef struct S2 S2;
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;
typedef struct ConstVector ConstVector;

// Sorted declarations
#line 10 "test1.ion"
#define PI (3.14f)

#line 11
#define PI2 ((PI) + (PI))

#line 13
char c = 1;

#line 14
uchar uc = 1;

#line 15
schar sc = 1;

#line 17
#define N ((((char)(42)) + (8)) != (0))

#line 51
uchar h(void);

#line 19
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 21
struct S1 {
    #line 22
    int a;
    #line 23
    const int (b);
};

#line 26
struct S2 {
    #line 27
    S1 s1;
};

#line 30
void f10(int (a[]));

void test_arrays(void);

#line 39
void test_nonmodifiable(void);

#line 120
struct Vector {
    #line 121
    int x;
    #line 121
    int y;
};

#line 78
typedef IntOrPtr U;

#line 84
union IntOrPtr {
    #line 85
    int i;
    #line 86
    int (*p);
};

#line 61
int g(U u);

void k(void (*vp), int (*ip));

#line 70
void f1(void);

#line 75
void f3(int (a[]));

#line 80
int example_test(void);

#line 136
int fact_rec(int n);

#line 128
int fact_iter(int n);

#line 89
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 99
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 102
int is_even(int digit);

#line 118
int i;

#line 124
void f2(Vector v);

#line 146
T (*p);

#line 144
#define M ((1) + (sizeof(p)))

struct T {
    #line 149
    int (a[M]);
};

#line 152
void benchmark(int n);

#line 159
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 165
void test_lits(void);

#line 180
void test_ops(void);

#line 210
#define IS_DEBUG (true)

#line 212
void test_bool(void);

#line 219
int test_ctrl(void);

#line 229
const int (j);

#line 230
const int ((*q));

#line 231
const Vector (cv);

#line 233
void f4(const char ((*x)));

#line 236
struct ConstVector {
    #line 237
    const int (x);
    #line 237
    const int (y);
};

#line 240
void f5(const int ((*p)));

#line 243
void test_convert(void);

#line 251
void test_const(void);

#line 274
void test_init(void);

#line 287
void test_cast(void);

#line 296
int main(int argc, const char ((*(*argv))));

// Function declarations
#line 30
void f10(int (a[])) {
    #line 31
    a[1] = 42;
}

#line 34
void test_arrays(void) {
    #line 35
    int (a[3]) = {1, 2, 3};
    #line 36
    (f10)(a);
}

#line 39
void test_nonmodifiable(void) {
    #line 40
    S1 s1;
    #line 41
    s1.a = 0;
    #line 44
    S2 s2;
    #line 45
    s2.s1.a = 0;
}

uchar h(void) {
    #line 52
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 53
    Vector (*v) = &((Vector){1, 2});
    #line 54
    v->x = 42;
    #line 55
    int (*p) = &((int){0});
    #line 56
    ulong x = ((uint){1}) + ((long){2});
    #line 57
    int y = +(c);
    #line 58
    return (uchar)(x);
}

#line 61
int g(U u) {
    #line 62
    return u.i;
}

#line 65
void k(void (*vp), int (*ip)) {
    #line 66
    vp = ip;
    #line 67
    ip = vp;
}

#line 70
void f1(void) {
    #line 71
    int (*p) = &((int){0});
    #line 72
    *(p) = 42;
}

#line 75
void f3(int (a[])) {
}

#line 80
int example_test(void) {
    #line 81
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 102
int is_even(int digit) {
    #line 103
    int b = 0;
    #line 104
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 106
            b = 1;
            break;
        }
    }
    #line 108
    return b;
}

#line 124
void f2(Vector v) {
    #line 125
    v = (Vector){0};
}

#line 128
int fact_iter(int n) {
    #line 129
    int r = 1;
    #line 130
    for (int i = 0; (i) <= (n); i++) {
        #line 131
        r *= i;
    }
    #line 133
    return r;
}

#line 136
int fact_rec(int n) {
    #line 137
    if ((n) == (0)) {
        #line 138
        return 1;
    } else {
        #line 140
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 152
void benchmark(int n) {
    #line 153
    int r = 1;
    #line 154
    for (int i = 1; (i) <= (n); i++) {
        #line 155
        r *= i;
    }
}

#line 159
int va_test(int x, ...) {
    #line 160
    return 0;
}

#line 165
void test_lits(void) {
    #line 166
    float f = 3.14f;
    #line 167
    double d = 3.14;
    #line 168
    int i = 1;
    #line 169
    uint u = 0xFFFFFFFFu;
    #line 170
    long l = 1l;
    #line 171
    ulong ul = 1ul;
    #line 172
    llong ll = 0x100000000ll;
    #line 173
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 174
    uint x1 = 0xFFFFFFFF;
    #line 175
    llong x2 = 4294967295;
    #line 176
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 177
    int x4 = (0xAA) + (0x55);
}

#line 180
void test_ops(void) {
    #line 181
    float pi = 3.14f;
    #line 182
    float f = 0.0f;
    #line 183
    f = +(pi);
    #line 184
    f = -(pi);
    #line 185
    int n = -(1);
    #line 186
    n = ~(n);
    #line 187
    f = ((f) * (pi)) + (n);
    #line 188
    f = (pi) / (pi);
    #line 189
    n = (3) % (2);
    #line 190
    n = (n) + ((uchar)(1));
    #line 191
    int (*p) = &(n);
    #line 192
    p = (p) + (1);
    #line 193
    n = (int)(((p) + (1)) - (p));
    #line 194
    n = (n) << (1);
    #line 195
    n = (n) >> (1);
    #line 196
    int b = ((p) + (1)) > (p);
    #line 197
    b = ((p) + (1)) >= (p);
    #line 198
    b = ((p) + (1)) < (p);
    #line 199
    b = ((p) + (1)) <= (p);
    #line 200
    b = ((p) + (1)) == (p);
    #line 201
    b = (1) > (2);
    #line 202
    b = (1.23f) <= (pi);
    #line 203
    n = 0xFF;
    #line 204
    b = (n) & (~(1));
    #line 205
    b = (n) & (1);
    #line 206
    b = ((n) & (~(1))) ^ (1);
    #line 207
    b = (p) && (pi);
}

#line 212
void test_bool(void) {
    #line 213
    bool b = false;
    #line 214
    b = true;
    #line 215
    int i = 0;
    #line 216
    i = IS_DEBUG;
}

#line 219
int test_ctrl(void) {
    #line 220
    while (1) {
        #line 221
        while (1) {
            #line 222
            break;
        }
        #line 224
        return 42;
    }
    #line 226
    return 0;
}

#line 233
void f4(const char ((*x))) {
}

#line 240
void f5(const int ((*p))) {
}

#line 243
void test_convert(void) {
    #line 244
    const int ((*a)) = 0;
    #line 245
    int (*b) = 0;
    #line 246
    a = b;
    #line 247
    void (*p) = 0;
    #line 248
    (f5)(p);
}

#line 251
void test_const(void) {
    #line 252
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 255
    i = 1;
    #line 258
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 263
    const char ((*p)) = (const char *)(0);
    #line 264
    p = (escape_to_char) + (1);
    #line 265
    char (*q) = (char *)(escape_to_char);
    #line 266
    c = q['n'];
    p = (const char *)(1);
    #line 271
    i = (int)((ullong)(p));
}

#line 274
void test_init(void) {
    #line 275
    int x = (const int)(0);
    #line 276
    int y;
    #line 277
    y = 0;
    #line 278
    int z = 42;
    #line 279
    int (a[3]) = {1, 2, 3};
    #line 282
    for (ullong i = 0; (i) < (10); i++) {
        #line 283
        (printf)("%llu\n", i);
    }
}

#line 287
void test_cast(void) {
    #line 288
    int (*p) = 0;
    #line 289
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 296
int main(int argc, const char ((*(*argv)))) {
    #line 297
    if ((argv) == (NULL)) {
        #line 298
        (printf)("argv is null\n");
    }
    #line 300
    (test_arrays)();
    #line 301
    (test_cast)();
    #line 302
    (test_init)();
    #line 303
    (test_lits)();
    #line 304
    (test_const)();
    #line 305
    (test_bool)();
    #line 306
    (test_ops)();
    #line 307
    int b = (example_test)();
    #line 308
    (puts)("Hello, world!");
    #line 309
    int c = (getchar)();
    #line 310
    (printf)("You wrote \'%c\'\n", c);
    #line 311
    (va_test)(1);
    #line 312
    (va_test)(1, 2);
    #line 313
    argv = NULL;
    #line 314
    return 0;
}
