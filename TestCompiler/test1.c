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

#line 53
uchar h(void);

#line 19
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 21
char (*code) = 
    "\n"
    "\t#include <stdio.h>\n"
    "\n"
    "\tint main(int argc, char **argv) {\n"
    "\t\tprintf(\"Hello, world!\\n\");\n"
    "\t\treturn 0;\n"
    "\t}\n";

#line 23
struct S1 {
    #line 24
    int a;
    #line 25
    const int (b);
};

#line 28
struct S2 {
    #line 29
    S1 s1;
};

#line 32
void f10(int (a[]));

void test_arrays(void);

#line 41
void test_nonmodifiable(void);

#line 122
struct Vector {
    #line 123
    int x;
    #line 123
    int y;
};

#line 80
typedef IntOrPtr U;

#line 86
union IntOrPtr {
    #line 87
    int i;
    #line 88
    int (*p);
};

#line 63
int g(U u);

void k(void (*vp), int (*ip));

#line 72
void f1(void);

#line 77
void f3(int (a[]));

#line 82
int example_test(void);

#line 138
int fact_rec(int n);

#line 130
int fact_iter(int n);

#line 91
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 101
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 104
int is_even(int digit);

#line 120
int i;

#line 126
void f2(Vector v);

#line 148
T (*p);

#line 146
#define M ((1) + (sizeof(p)))

struct T {
    #line 151
    int (a[M]);
};

#line 154
void benchmark(int n);

#line 161
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 167
void test_lits(void);

#line 182
void test_ops(void);

#line 212
#define IS_DEBUG (true)

#line 214
void test_bool(void);

#line 221
int test_ctrl(void);

#line 231
const int (j);

#line 232
const int ((*q));

#line 233
const Vector (cv);

#line 235
void f4(const char ((*x)));

#line 238
struct ConstVector {
    #line 239
    const int (x);
    #line 239
    const int (y);
};

#line 242
void f5(const int ((*p)));

#line 245
void test_convert(void);

#line 253
void test_const(void);

#line 276
void test_init(void);

#line 289
void test_cast(void);

#line 298
int main(int argc, const char ((*(*argv))));

// Function declarations
#line 32
void f10(int (a[])) {
    #line 33
    a[1] = 42;
}

#line 36
void test_arrays(void) {
    #line 37
    int (a[3]) = {1, 2, 3};
    #line 38
    (f10)(a);
}

#line 41
void test_nonmodifiable(void) {
    #line 42
    S1 s1;
    #line 43
    s1.a = 0;
    #line 46
    S2 s2;
    #line 47
    s2.s1.a = 0;
}

uchar h(void) {
    #line 54
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 55
    Vector (*v) = &((Vector){1, 2});
    #line 56
    v->x = 42;
    #line 57
    int (*p) = &((int){0});
    #line 58
    ulong x = ((uint){1}) + ((long){2});
    #line 59
    int y = +(c);
    #line 60
    return (uchar)(x);
}

#line 63
int g(U u) {
    #line 64
    return u.i;
}

#line 67
void k(void (*vp), int (*ip)) {
    #line 68
    vp = ip;
    #line 69
    ip = vp;
}

#line 72
void f1(void) {
    #line 73
    int (*p) = &((int){0});
    #line 74
    *(p) = 42;
}

#line 77
void f3(int (a[])) {
}

#line 82
int example_test(void) {
    #line 83
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 104
int is_even(int digit) {
    #line 105
    int b = 0;
    #line 106
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 108
            b = 1;
            break;
        }
    }
    #line 110
    return b;
}

#line 126
void f2(Vector v) {
    #line 127
    v = (Vector){0};
}

#line 130
int fact_iter(int n) {
    #line 131
    int r = 1;
    #line 132
    for (int i = 0; (i) <= (n); i++) {
        #line 133
        r *= i;
    }
    #line 135
    return r;
}

#line 138
int fact_rec(int n) {
    #line 139
    if ((n) == (0)) {
        #line 140
        return 1;
    } else {
        #line 142
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 154
void benchmark(int n) {
    #line 155
    int r = 1;
    #line 156
    for (int i = 1; (i) <= (n); i++) {
        #line 157
        r *= i;
    }
}

#line 161
int va_test(int x, ...) {
    #line 162
    return 0;
}

#line 167
void test_lits(void) {
    #line 168
    float f = 3.14f;
    #line 169
    double d = 3.14;
    #line 170
    int i = 1;
    #line 171
    uint u = 0xFFFFFFFFu;
    #line 172
    long l = 1l;
    #line 173
    ulong ul = 1ul;
    #line 174
    llong ll = 0x100000000ll;
    #line 175
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 176
    uint x1 = 0xFFFFFFFF;
    #line 177
    llong x2 = 4294967295;
    #line 178
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 179
    int x4 = (0xAA) + (0x55);
}

#line 182
void test_ops(void) {
    #line 183
    float pi = 3.14f;
    #line 184
    float f = 0.0f;
    #line 185
    f = +(pi);
    #line 186
    f = -(pi);
    #line 187
    int n = -(1);
    #line 188
    n = ~(n);
    #line 189
    f = ((f) * (pi)) + (n);
    #line 190
    f = (pi) / (pi);
    #line 191
    n = (3) % (2);
    #line 192
    n = (n) + ((uchar)(1));
    #line 193
    int (*p) = &(n);
    #line 194
    p = (p) + (1);
    #line 195
    n = (int)(((p) + (1)) - (p));
    #line 196
    n = (n) << (1);
    #line 197
    n = (n) >> (1);
    #line 198
    int b = ((p) + (1)) > (p);
    #line 199
    b = ((p) + (1)) >= (p);
    #line 200
    b = ((p) + (1)) < (p);
    #line 201
    b = ((p) + (1)) <= (p);
    #line 202
    b = ((p) + (1)) == (p);
    #line 203
    b = (1) > (2);
    #line 204
    b = (1.23f) <= (pi);
    #line 205
    n = 0xFF;
    #line 206
    b = (n) & (~(1));
    #line 207
    b = (n) & (1);
    #line 208
    b = ((n) & (~(1))) ^ (1);
    #line 209
    b = (p) && (pi);
}

#line 214
void test_bool(void) {
    #line 215
    bool b = false;
    #line 216
    b = true;
    #line 217
    int i = 0;
    #line 218
    i = IS_DEBUG;
}

#line 221
int test_ctrl(void) {
    #line 222
    while (1) {
        #line 223
        while (1) {
            #line 224
            break;
        }
        #line 226
        return 42;
    }
    #line 228
    return 0;
}

#line 235
void f4(const char ((*x))) {
}

#line 242
void f5(const int ((*p))) {
}

#line 245
void test_convert(void) {
    #line 246
    const int ((*a)) = 0;
    #line 247
    int (*b) = 0;
    #line 248
    a = b;
    #line 249
    void (*p) = 0;
    #line 250
    (f5)(p);
}

#line 253
void test_const(void) {
    #line 254
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 257
    i = 1;
    #line 260
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 265
    const char ((*p)) = (const char *)(0);
    #line 266
    p = (escape_to_char) + (1);
    #line 267
    char (*q) = (char *)(escape_to_char);
    #line 268
    c = q['n'];
    p = (const char *)(1);
    #line 273
    i = (int)((ullong)(p));
}

#line 276
void test_init(void) {
    #line 277
    int x = (const int)(0);
    #line 278
    int y;
    #line 279
    y = 0;
    #line 280
    int z = 42;
    #line 281
    int (a[3]) = {1, 2, 3};
    #line 284
    for (ullong i = 0; (i) < (10); i++) {
        #line 285
        (printf)("%llu\n", i);
    }
}

#line 289
void test_cast(void) {
    #line 290
    int (*p) = 0;
    #line 291
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 298
int main(int argc, const char ((*(*argv)))) {
    #line 299
    if ((argv) == (NULL)) {
        #line 300
        (printf)("argv is null\n");
    }
    #line 302
    (test_arrays)();
    #line 303
    (test_cast)();
    #line 304
    (test_init)();
    #line 305
    (test_lits)();
    #line 306
    (test_const)();
    #line 307
    (test_bool)();
    #line 308
    (test_ops)();
    #line 309
    int b = (example_test)();
    #line 310
    (puts)("Hello, world!");
    #line 311
    int c = (getchar)();
    #line 312
    (printf)("You wrote \'%c\'\n", c);
    #line 313
    (va_test)(1);
    #line 314
    (va_test)(1, 2);
    #line 315
    argv = NULL;
    #line 316
    return 0;
}
