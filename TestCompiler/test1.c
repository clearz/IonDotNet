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

uchar h(void);

#line 19
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 90
struct Vector {
    #line 91
    int x;
    #line 91
    int y;
};

#line 48
typedef IntOrPtr U;

#line 54
union IntOrPtr {
    #line 55
    int i;
    #line 56
    int (*p);
};

#line 31
int g(U u);

void k(void (*vp), int (*ip));

#line 40
void f1(void);

#line 45
void f3(int (a[]));

#line 50
int example_test(void);

#line 106
int fact_rec(int n);

#line 98
int fact_iter(int n);

#line 59
char const (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 69
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 72
int is_even(int digit);

#line 88
int i;

#line 94
void f2(Vector v);

#line 116
T (*p);

#line 114
#define M ((1) + (sizeof(p)))

struct T {
    #line 119
    int (a[M]);
};

#line 122
void benchmark(int n);

#line 129
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 135
void test_lits(void);

#line 150
void test_ops(void);

#line 180
#define IS_DEBUG (true)

#line 182
void test_bool(void);

#line 189
int test_ctrl(void);

#line 199
int const (j);

#line 200
int const ((*q));

#line 201
Vector const (cv);

#line 203
void f4(char const ((*x)));

#line 206
struct ConstVector {
    #line 207
    int const (x);
    #line 207
    int const (y);
};

#line 210
void f5(void const ((*p)));

#line 213
void test_convert(void);

#line 221
void test_const(void);

#line 244
void test_init(void);

#line 253
int main(int argc, char const ((*(*argv))));

// Function declarations
#line 21
uchar h(void) {
    #line 22
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 23
    Vector (*v) = &((Vector){1, 2});
    #line 24
    v->x = 42;
    #line 25
    int (*p) = &((int){0});
    #line 26
    ulong x = ((uint){1}) + ((long){2});
    #line 27
    int y = +(c);
    #line 28
    return (uchar)(x);
}

#line 31
int g(U u) {
    #line 32
    return u.i;
}

#line 35
void k(void (*vp), int (*ip)) {
    #line 36
    vp = ip;
    #line 37
    ip = vp;
}

#line 40
void f1(void) {
    #line 41
    int (*p) = &((int){0});
    #line 42
    *(p) = 42;
}

#line 45
void f3(int (a[])) {
}

#line 50
int example_test(void) {
    #line 51
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 72
int is_even(int digit) {
    #line 73
    int b = 0;
    #line 74
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 76
            b = 1;
            break;
        }
    }
    #line 78
    return b;
}

#line 94
void f2(Vector v) {
    #line 95
    v = (Vector){0};
}

#line 98
int fact_iter(int n) {
    #line 99
    int r = 1;
    #line 100
    for (int i = 0; (i) <= (n); i++) {
        #line 101
        r *= i;
    }
    #line 103
    return r;
}

#line 106
int fact_rec(int n) {
    #line 107
    if ((n) == (0)) {
        #line 108
        return 1;
    } else {
        #line 110
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 122
void benchmark(int n) {
    #line 123
    int r = 1;
    #line 124
    for (int i = 1; (i) <= (n); i++) {
        #line 125
        r *= i;
    }
}

#line 129
int va_test(int x, ...) {
    #line 130
    return 0;
}

#line 135
void test_lits(void) {
    #line 136
    float f = 3.14f;
    #line 137
    double d = 3.14;
    #line 138
    int i = 1;
    #line 139
    uint u = 0xFFFFFFFFu;
    #line 140
    long l = 1l;
    #line 141
    ulong ul = 1ul;
    #line 142
    llong ll = 0x100000000ll;
    #line 143
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 144
    uint x1 = 0xFFFFFFFF;
    #line 145
    llong x2 = 4294967295;
    #line 146
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 147
    int x4 = (0xAA) + (0x55);
}

#line 150
void test_ops(void) {
    #line 151
    float pi = 3.14f;
    #line 152
    float f = 0.0f;
    #line 153
    f = +(pi);
    #line 154
    f = -(pi);
    #line 155
    int n = -(1);
    #line 156
    n = ~(n);
    #line 157
    f = ((f) * (pi)) + (n);
    #line 158
    f = (pi) / (pi);
    #line 159
    n = (3) % (2);
    #line 160
    n = (n) + ((uchar)(1));
    #line 161
    int (*p) = &(n);
    #line 162
    p = (p) + (1);
    #line 163
    n = (int)(((p) + (1)) - (p));
    #line 164
    n = (n) << (1);
    #line 165
    n = (n) >> (1);
    #line 166
    int b = ((p) + (1)) > (p);
    #line 167
    b = ((p) + (1)) >= (p);
    #line 168
    b = ((p) + (1)) < (p);
    #line 169
    b = ((p) + (1)) <= (p);
    #line 170
    b = ((p) + (1)) == (p);
    #line 171
    b = (1) > (2);
    #line 172
    b = (1.23f) <= (pi);
    #line 173
    n = 0xFF;
    #line 174
    b = (n) & (~(1));
    #line 175
    b = (n) & (1);
    #line 176
    b = ((n) & (~(1))) ^ (1);
    #line 177
    b = (p) && (pi);
}

#line 182
void test_bool(void) {
    #line 183
    bool b = false;
    #line 184
    b = true;
    #line 185
    int i = 0;
    #line 186
    i = IS_DEBUG;
}

#line 189
int test_ctrl(void) {
    #line 190
    while (1) {
        #line 191
        while (1) {
            #line 192
            break;
        }
        #line 194
        return 42;
    }
    #line 196
    return 0;
}

#line 203
void f4(char const ((*x))) {
}

#line 210
void f5(void const ((*p))) {
}

#line 213
void test_convert(void) {
    #line 214
    int const ((*a)) = 0;
    #line 215
    int (*b) = 0;
    #line 216
    a = b;
    #line 217
    int (*p) = 0;
    #line 218
    (f5)(p);
}

#line 221
void test_const(void) {
    #line 222
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 225
    i = 1;
    #line 228
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 233
    char const ((*p)) = (char const *)(0);
    #line 234
    p = (escape_to_char) + (1);
    #line 235
    char (*q) = (char *)(escape_to_char);
    #line 236
    c = q['n'];
    p = (char const *)(1);
    #line 241
    i = (int)((ullong)(p));
}

#line 244
void test_init(void) {
    #line 245
    int x = (int const)(0);
    #line 246
    int y;
    #line 247
    int z = 42;
    #line 248
    int (a[3]) = {1, 2, 3};
}

#line 253
int main(int argc, char const ((*(*argv)))) {
    #line 254
    if ((argv) == (0)) {
        #line 255
        (printf)("argv is null\n");
    }
    #line 257
    (test_init)();
    #line 258
    (test_lits)();
    #line 259
    (test_const)();
    #line 260
    (test_bool)();
    #line 261
    (test_ops)();
    #line 262
    int b = (example_test)();
    #line 263
    (puts)("Hello, world!");
    #line 264
    int c = (getchar)();
    #line 265
    (printf)("You wrote \'%c\'\n", c);
    #line 266
    (va_test)(1);
    #line 267
    (va_test)(1, 2);
    #line 268
    argv = NULL;
    #line 269
    return 0;
}
