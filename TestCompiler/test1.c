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
typedef enum Color Color;

// Sorted declarations
#line 305 "test1.ion"
enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
};

#line 10
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

#line 60
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

#line 30
struct S1 {
    #line 31
    int a;
    #line 32
    const int (b);
};

#line 35
struct S2 {
    #line 36
    S1 s1;
};

#line 39
void f10(int (a[]));

void test_arrays(void);

#line 48
void test_nonmodifiable(void);

#line 129
struct Vector {
    #line 130
    int x;
    #line 130
    int y;
};

#line 87
typedef IntOrPtr U;

#line 93
union IntOrPtr {
    #line 94
    int i;
    #line 95
    int (*p);
};

#line 70
int g(U u);

void k(void (*vp), int (*ip));

#line 79
void f1(void);

#line 84
void f3(int (a[]));

#line 89
int example_test(void);

#line 145
int fact_rec(int n);

#line 137
int fact_iter(int n);

#line 98
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 108
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 111
int is_even(int digit);

#line 127
int i;

#line 133
void f2(Vector v);

#line 155
T (*p);

#line 153
#define M ((1) + (sizeof(p)))

struct T {
    #line 158
    int (a[M]);
};

#line 161
void benchmark(int n);

#line 168
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 174
void test_lits(void);

#line 189
void test_ops(void);

#line 219
#define IS_DEBUG (true)

#line 221
void test_bool(void);

#line 228
int test_ctrl(void);

#line 238
const int (j);

#line 239
const int ((*q));

#line 240
const Vector (cv);

#line 242
void f4(const char ((*x)));

#line 245
struct ConstVector {
    #line 246
    const int (x);
    #line 246
    const int (y);
};

#line 249
void f5(const int ((*p)));

#line 252
void test_convert(void);

#line 260
void test_const(void);

#line 283
void test_init(void);

#line 296
void test_cast(void);

#line 312
void test_enum(void);

#line 317
int main(int argc, const char ((*(*argv))));

// Function declarations
#line 39
void f10(int (a[])) {
    #line 40
    a[1] = 42;
}

#line 43
void test_arrays(void) {
    #line 44
    int (a[3]) = {1, 2, 3};
    #line 45
    (f10)(a);
}

#line 48
void test_nonmodifiable(void) {
    #line 49
    S1 s1;
    #line 50
    s1.a = 0;
    #line 53
    S2 s2;
    #line 54
    s2.s1.a = 0;
}

uchar h(void) {
    #line 61
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 62
    Vector (*v) = &((Vector){1, 2});
    #line 63
    v->x = 42;
    #line 64
    int (*p) = &((int){0});
    #line 65
    ulong x = ((uint){1}) + ((long){2});
    #line 66
    int y = +(c);
    #line 67
    return (uchar)(x);
}

#line 70
int g(U u) {
    #line 71
    return u.i;
}

#line 74
void k(void (*vp), int (*ip)) {
    #line 75
    vp = ip;
    #line 76
    ip = vp;
}

#line 79
void f1(void) {
    #line 80
    int (*p) = &((int){0});
    #line 81
    *(p) = 42;
}

#line 84
void f3(int (a[])) {
}

#line 89
int example_test(void) {
    #line 90
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 111
int is_even(int digit) {
    #line 112
    int b = 0;
    #line 113
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 115
            b = 1;
            break;
        }
    }
    #line 117
    return b;
}

#line 133
void f2(Vector v) {
    #line 134
    v = (Vector){0};
}

#line 137
int fact_iter(int n) {
    #line 138
    int r = 1;
    #line 139
    for (int i = 0; (i) <= (n); i++) {
        #line 140
        r *= i;
    }
    #line 142
    return r;
}

#line 145
int fact_rec(int n) {
    #line 146
    if ((n) == (0)) {
        #line 147
        return 1;
    } else {
        #line 149
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 161
void benchmark(int n) {
    #line 162
    int r = 1;
    #line 163
    for (int i = 1; (i) <= (n); i++) {
        #line 164
        r *= i;
    }
}

#line 168
int va_test(int x, ...) {
    #line 169
    return 0;
}

#line 174
void test_lits(void) {
    #line 175
    float f = 3.14f;
    #line 176
    double d = 3.14;
    #line 177
    int i = 1;
    #line 178
    uint u = 0xFFFFFFFFu;
    #line 179
    long l = 1l;
    #line 180
    ulong ul = 1ul;
    #line 181
    llong ll = 0x100000000ll;
    #line 182
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 183
    uint x1 = 0xFFFFFFFF;
    #line 184
    llong x2 = 4294967295;
    #line 185
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 186
    int x4 = (0xAA) + (0x55);
}

#line 189
void test_ops(void) {
    #line 190
    float pi = 3.14f;
    #line 191
    float f = 0.0f;
    #line 192
    f = +(pi);
    #line 193
    f = -(pi);
    #line 194
    int n = -(1);
    #line 195
    n = ~(n);
    #line 196
    f = ((f) * (pi)) + (n);
    #line 197
    f = (pi) / (pi);
    #line 198
    n = (3) % (2);
    #line 199
    n = (n) + ((uchar)(1));
    #line 200
    int (*p) = &(n);
    #line 201
    p = (p) + (1);
    #line 202
    n = (int)(((p) + (1)) - (p));
    #line 203
    n = (n) << (1);
    #line 204
    n = (n) >> (1);
    #line 205
    int b = ((p) + (1)) > (p);
    #line 206
    b = ((p) + (1)) >= (p);
    #line 207
    b = ((p) + (1)) < (p);
    #line 208
    b = ((p) + (1)) <= (p);
    #line 209
    b = ((p) + (1)) == (p);
    #line 210
    b = (1) > (2);
    #line 211
    b = (1.23f) <= (pi);
    #line 212
    n = 0xFF;
    #line 213
    b = (n) & (~(1));
    #line 214
    b = (n) & (1);
    #line 215
    b = ((n) & (~(1))) ^ (1);
    #line 216
    b = (p) && (pi);
}

#line 221
void test_bool(void) {
    #line 222
    bool b = false;
    #line 223
    b = true;
    #line 224
    int i = 0;
    #line 225
    i = IS_DEBUG;
}

#line 228
int test_ctrl(void) {
    #line 229
    while (1) {
        #line 230
        while (1) {
            #line 231
            break;
        }
        #line 233
        return 42;
    }
    #line 235
    return 0;
}

#line 242
void f4(const char ((*x))) {
}

#line 249
void f5(const int ((*p))) {
}

#line 252
void test_convert(void) {
    #line 253
    const int ((*a)) = 0;
    #line 254
    int (*b) = 0;
    #line 255
    a = b;
    #line 256
    void (*p) = 0;
    #line 257
    (f5)(p);
}

#line 260
void test_const(void) {
    #line 261
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 264
    i = 1;
    #line 267
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 272
    const char ((*p)) = (const char *)(0);
    #line 273
    p = (escape_to_char) + (1);
    #line 274
    char (*q) = (char *)(escape_to_char);
    #line 275
    c = q['n'];
    p = (const char *)(1);
    #line 280
    i = (int)((ullong)(p));
}

#line 283
void test_init(void) {
    #line 284
    int x = (const int)(0);
    #line 285
    int y;
    #line 286
    y = 0;
    #line 287
    int z = 42;
    #line 288
    int (a[3]) = {1, 2, 3};
    #line 291
    for (ullong i = 0; (i) < (10); i++) {
        #line 292
        (printf)("%llu\n", i);
    }
}

#line 296
void test_cast(void) {
    #line 297
    int (*p) = 0;
    #line 298
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 312
void test_enum(void) {
    #line 313
    Color c = COLOR_RED;
    #line 314
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 317
int main(int argc, const char ((*(*argv)))) {
    #line 318
    if ((argv) == (NULL)) {
        #line 319
        (printf)("argv is null\n");
    }
    #line 321
    (test_enum)();
    #line 322
    (test_arrays)();
    #line 323
    (test_cast)();
    #line 324
    (test_init)();
    #line 325
    (test_lits)();
    #line 326
    (test_const)();
    #line 327
    (test_bool)();
    #line 328
    (test_ops)();
    #line 329
    int b = (example_test)();
    #line 330
    (puts)("Hello, world!");
    #line 331
    int c = (getchar)();
    #line 332
    (printf)("You wrote \'%c\'\n", c);
    #line 333
    (va_test)(1);
    #line 334
    (va_test)(1, 2);
    #line 335
    argv = NULL;
    #line 336
    return 0;
}
