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
#line 161 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

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
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "\tprintf(\"Hello, world!\\n\");\n"
    "\treturn 0;\n"
    "}\n";

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
void f10(int (a[3]));

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
    int(*p);
};

#line 70
int g(U u);

void k(void(*vp), int(*ip));

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
T(*p);

#line 153
#define M ((1) + (sizeof(p)))

struct T {
    #line 158
    int (a[M]);
};

#line 169
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 176
void test_enum(void);

#line 185
void test_assign(void);

#line 208
void benchmark(int n);

#line 215
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 221
void test_lits(void);

#line 236
void test_ops(void);

#line 266
#define IS_DEBUG (true)

#line 268
void test_bool(void);

#line 275
int test_ctrl(void);

#line 285
const int (j);

#line 286
const int(*q);

#line 287
const Vector (cv);

#line 289
void f4(const char(*x));

#line 292
struct ConstVector {
    #line 293
    const int (x);
    #line 293
    const int (y);
};

#line 296
void f5(const int(*p));

#line 299
void test_convert(void);

#line 307
void test_const(void);

#line 330
void test_init(void);

#line 343
void test_cast(void);

#line 352
int main(int argc, const char *(*argv));

// Function declarations
#line 39
void f10(int (a[3])) {
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
void k(void(*vp), int(*ip)) {
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

#line 176
void test_enum(void) {
    #line 177
    Color a = COLOR_RED;
    #line 178
    Color b = COLOR_RED;
    #line 179
    int c = (a) + (b);
    #line 180
    int i = a;
    #line 181
    a = i;
    #line 182
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 185
void test_assign(void) {
    #line 186
    int i = 0;
    #line 187
    float f = 3.14f;
    #line 188
    int(*p) = &(i);
    #line 189
    i++;
    #line 190
    i--;
    #line 191
    p++;
    #line 192
    p--;
    #line 193
    p += 1;
    #line 194
    i /= 2;
    #line 195
    i *= 123;
    #line 196
    i %= 3;
    #line 197
    i <<= 1;
    #line 198
    i >>= 2;
    #line 199
    i &= 0xFF;
    #line 200
    i |= 0xFF00;
    #line 201
    i ^= 0xFF0;
}

#line 208
void benchmark(int n) {
    #line 209
    int r = 1;
    #line 210
    for (int i = 1; (i) <= (n); i++) {
        #line 211
        r *= i;
    }
}

#line 215
int va_test(int x, ...) {
    #line 216
    return 0;
}

#line 221
void test_lits(void) {
    #line 222
    float f = 3.14f;
    #line 223
    double d = 3.14;
    #line 224
    int i = 1;
    #line 225
    uint u = 0xFFFFFFFFu;
    #line 226
    long l = 1l;
    #line 227
    ulong ul = 1ul;
    #line 228
    llong ll = 0x100000000ll;
    #line 229
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 230
    uint x1 = 0xFFFFFFFF;
    #line 231
    llong x2 = 4294967295;
    #line 232
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 233
    int x4 = (0xAA) + (0x55);
}

#line 236
void test_ops(void) {
    #line 237
    float pi = 3.14f;
    #line 238
    float f = 0.0f;
    #line 239
    f = +(pi);
    #line 240
    f = -(pi);
    #line 241
    int n = -(1);
    #line 242
    n = ~(n);
    #line 243
    f = ((f) * (pi)) + (n);
    #line 244
    f = (pi) / (pi);
    #line 245
    n = (3) % (2);
    #line 246
    n = (n) + ((uchar)(1));
    #line 247
    int (*p) = &(n);
    #line 248
    p = (p) + (1);
    #line 249
    n = (int)(((p) + (1)) - (p));
    #line 250
    n = (n) << (1);
    #line 251
    n = (n) >> (1);
    #line 252
    int b = ((p) + (1)) > (p);
    #line 253
    b = ((p) + (1)) >= (p);
    #line 254
    b = ((p) + (1)) < (p);
    #line 255
    b = ((p) + (1)) <= (p);
    #line 256
    b = ((p) + (1)) == (p);
    #line 257
    b = (1) > (2);
    #line 258
    b = (1.23f) <= (pi);
    #line 259
    n = 0xFF;
    #line 260
    b = (n) & (~(1));
    #line 261
    b = (n) & (1);
    #line 262
    b = ((n) & (~(1))) ^ (1);
    #line 263
    b = (p) && (pi);
}

#line 268
void test_bool(void) {
    #line 269
    bool b = false;
    #line 270
    b = true;
    #line 271
    int i = 0;
    #line 272
    i = IS_DEBUG;
}

#line 275
int test_ctrl(void) {
    #line 276
    switch (1) {
        case 0: {
            #line 278
            return 0;
            break;
        }default: {
            #line 280
            return 1;
            break;
        }
    }
}

#line 289
void f4(const char(*x)) {
}

#line 296
void f5(const int(*p)) {
}

#line 299
void test_convert(void) {
    #line 300
    const int(*a) = 0;
    #line 301
    int(*b) = 0;
    #line 302
    a = b;
    #line 303
    void(*p) = 0;
    #line 304
    (f5)(p);
}

#line 307
void test_const(void) {
    #line 308
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 311
    i = 1;
    #line 314
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 319
    const char ((*p)) = (const char *)(0);
    #line 320
    p = (escape_to_char) + (1);
    #line 321
    char (*q) = (char *)(escape_to_char);
    #line 322
    c = q['n'];
    p = (const char *)(1);
    #line 327
    i = (int)((ullong)(p));
}

#line 330
void test_init(void) {
    #line 331
    int x = (const int)(0);
    #line 332
    int y;
    #line 333
    y = 0;
    #line 334
    int z = 42;
    #line 335
    int (a[3]) = {1, 2, 3};
    #line 338
    for (ullong i = 0; (i) < (10); i++) {
        #line 339
        (printf)("%llu\n", i);
    }
}

#line 343
void test_cast(void) {
    #line 344
    int(*p) = 0;
    #line 345
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 352
int main(int argc, const char *(*argv)) {
    #line 353
    if ((argv) == (0)) {
        #line 354
        (printf)("argv is null\n");
    }
    #line 356
    const char(*ab) = NULL;
    #line 357
    (test_assign)();
    #line 358
    (test_enum)();
    #line 359
    (test_arrays)();
    #line 360
    (test_cast)();
    #line 361
    (test_init)();
    #line 362
    (test_lits)();
    #line 363
    (test_const)();
    #line 364
    (test_bool)();
    #line 365
    (test_ops)();
    #line 366
    int b = (example_test)();
    #line 367
    (puts)("Hello, world!");
    #line 368
    int c = (getchar)();
    #line 369
    (printf)("You wrote \'%c\'\n", c);
    #line 370
    (va_test)(1);
    #line 371
    (va_test)(1, 2);
    #line 372
    argv = NULL;
    #line 373
    return 0;
}
