// Forward includes
#include <stdio.h>
// Preamble
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
typedef struct UartCtrl UartCtrl;
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;
typedef struct ConstVector ConstVector;

// Sorted declarations
#line 228 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 23
#define PI 3.14f

#line 24
#define PI2 (PI) + (PI)

#line 26
#define U8 (uint8)(42)

#line 28
char c = 1;

#line 29
uchar uc = 1;

#line 30
schar sc = 1;

#line 32
#define N (((char)(42)) + (8)) != (0)

#line 127
uchar h(void);

#line 34
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 36
char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 45
struct S1 {
    #line 46
    int a;
    #line 47
    const int (b);
};

#line 50
struct S2 {
    #line 51
    S1 s1;
};

#line 54
void f10(int (a[3]));

void test_arrays(void);

#line 64
void test_loops(void);

#line 93
void test_nonmodifiable(void);

#line 105
struct UartCtrl {
    #line 106
    bool tx_enable;
    #line 106
    bool rx_enable;
};

#line 109
#define UART_CTRL_REG (uint *)(0x12345678)

#line 111
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 196
struct Vector {
    #line 197
    int x;
    #line 197
    int y;
};

#line 154
typedef IntOrPtr U;

#line 160
union IntOrPtr {
    #line 161
    int i;
    #line 162
    int(*p);
};

#line 137
int g(U u);

void k(void(*vp), int(*ip));

#line 146
void f1(void);

#line 151
void f3(int (a[]));

#line 156
int example_test(void);

#line 212
int fact_rec(int n);

#line 204
int fact_iter(int n);

#line 165
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 175
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 178
int is_even(int digit);

#line 194
int i;

#line 200
void f2(Vector v);

#line 222
T(*p);

#line 220
#define M (1) + (sizeof(p))

struct T {
    #line 225
    int (a[M]);
};

#line 236
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 243
void test_enum(void);

#line 252
void test_assign(void);

#line 275
void benchmark(int n);

#line 282
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 288
void test_lits(void);

#line 303
void test_ops(void);

#line 333
#define IS_DEBUG true

#line 335
void test_bool(void);

#line 342
int test_ctrl(void);

#line 352
const int (j);

#line 353
const int(*q);

#line 354
const Vector (cv);

#line 356
void f4(const char(*x));

#line 359
struct ConstVector {
    #line 360
    const int (x);
    #line 360
    const int (y);
};

#line 363
void f5(const int(*p));

#line 366
void test_convert(void);

#line 374
void test_const(void);

#line 397
void test_init(void);

#line 410
void test_sizeof(void);

#line 418
void test_cast(void);

#line 427
int main(int argc, const char *(*argv));

// Function declarations
#line 54
void f10(int (a[3])) {
    #line 55
    a[1] = 42;
}

#line 58
void test_arrays(void) {
    #line 59
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 64
void test_loops(void) {
    #line 67
    while (0) {
    }
    #line 69
    for (int i = 0; (i) < (10); i++) {
    }
    #line 72
    for (;;) {
        #line 73
        break;
    }
    #line 75
    for (int i = 0;;) {
        #line 76
        break;
    }
    #line 78
    for (; 0;) {
    }
    #line 80
    for (int i = 0;; i++) {
        #line 81
        break;
    }
    #line 83
    int i = 0;
    #line 84
    for (;; i++) {
        #line 85
        break;
    }
    #line 87
    switch (0) {default: {
            break;
        }
    }
}

#line 93
void test_nonmodifiable(void) {
    #line 94
    S1 s1;
    #line 95
    s1.a = 0;
    #line 98
    S2 s2;
    #line 99
    s2.s1.a = 0;
}

#line 111
uint32 pack(UartCtrl ctrl) {
    #line 112
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 115
UartCtrl unpack(uint32 word) {
    #line 116
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 119
void test_uart(void) {
    #line 120
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 121
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 122
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 123
    ctrl.rx_enable = true;
    #line 124
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 127
uchar h(void) {
    #line 128
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 129
    Vector (*v) = &((Vector){1, 2});
    #line 130
    v->x = 42;
    #line 131
    int (*p) = &((int){0});
    #line 132
    ulong x = ((uint){1}) + ((long){2});
    #line 133
    int y = +(c);
    #line 134
    return (uchar)(x);
}

#line 137
int g(U u) {
    #line 138
    return u.i;
}

#line 141
void k(void(*vp), int(*ip)) {
    #line 142
    vp = ip;
    #line 143
    ip = vp;
}

#line 146
void f1(void) {
    #line 147
    int (*p) = &((int){0});
    #line 148
    *(p) = 42;
}

#line 151
void f3(int (a[])) {
}

#line 156
int example_test(void) {
    #line 157
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 178
int is_even(int digit) {
    #line 179
    int b = 0;
    #line 180
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 182
            b = 1;
            break;
        }
    }
    #line 184
    return b;
}

#line 200
void f2(Vector v) {
    #line 201
    v = (Vector){0};
}

#line 204
int fact_iter(int n) {
    #line 205
    int r = 1;
    #line 206
    for (int i = 0; (i) <= (n); i++) {
        #line 207
        r *= i;
    }
    #line 209
    return r;
}

#line 212
int fact_rec(int n) {
    #line 213
    if ((n) == (0)) {
        #line 214
        return 1;
    } else {
        #line 216
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 243
void test_enum(void) {
    #line 244
    Color a = COLOR_RED;
    #line 245
    Color b = COLOR_RED;
    #line 246
    int c = (a) + (b);
    #line 247
    int i = a;
    #line 248
    a = i;
    #line 249
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 252
void test_assign(void) {
    #line 253
    int i = 0;
    #line 254
    float f = 3.14f;
    #line 255
    int(*p) = &(i);
    #line 256
    i++;
    #line 257
    i--;
    #line 258
    p++;
    #line 259
    p--;
    #line 260
    p += 1;
    #line 261
    i /= 2;
    #line 262
    i *= 123;
    #line 263
    i %= 3;
    #line 264
    i <<= 1;
    #line 265
    i >>= 2;
    #line 266
    i &= 0xFF;
    #line 267
    i |= 0xFF00;
    #line 268
    i ^= 0xFF0;
}

#line 275
void benchmark(int n) {
    #line 276
    int r = 1;
    #line 277
    for (int i = 1; (i) <= (n); i++) {
        #line 278
        r *= i;
    }
}

#line 282
int va_test(int x, ...) {
    #line 283
    return 0;
}

#line 288
void test_lits(void) {
    #line 289
    float f = 3.14f;
    #line 290
    double d = 3.14;
    #line 291
    int i = 1;
    #line 292
    uint u = 0xFFFFFFFFu;
    #line 293
    long l = 1l;
    #line 294
    ulong ul = 1ul;
    #line 295
    llong ll = 0x100000000ll;
    #line 296
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 297
    uint x1 = 0xFFFFFFFF;
    #line 298
    llong x2 = 4294967295;
    #line 299
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 300
    int x4 = (0xAA) + (0x55);
}

#line 303
void test_ops(void) {
    #line 304
    float pi = 3.14f;
    #line 305
    float f = 0.0f;
    #line 306
    f = +(pi);
    #line 307
    f = -(pi);
    #line 308
    int n = -(1);
    #line 309
    n = ~(n);
    #line 310
    f = ((f) * (pi)) + (n);
    #line 311
    f = (pi) / (pi);
    #line 312
    n = (3) % (2);
    #line 313
    n = (n) + ((uchar)(1));
    #line 314
    int (*p) = &(n);
    #line 315
    p = (p) + (1);
    #line 316
    n = (int)(((p) + (1)) - (p));
    #line 317
    n = (n) << (1);
    #line 318
    n = (n) >> (1);
    #line 319
    int b = ((p) + (1)) > (p);
    #line 320
    b = ((p) + (1)) >= (p);
    #line 321
    b = ((p) + (1)) < (p);
    #line 322
    b = ((p) + (1)) <= (p);
    #line 323
    b = ((p) + (1)) == (p);
    #line 324
    b = (1) > (2);
    #line 325
    b = (1.23f) <= (pi);
    #line 326
    n = 0xFF;
    #line 327
    b = (n) & (~(1));
    #line 328
    b = (n) & (1);
    #line 329
    b = ((n) & (~(1))) ^ (1);
    #line 330
    b = (p) && (pi);
}

#line 335
void test_bool(void) {
    #line 336
    bool b = false;
    #line 337
    b = true;
    #line 338
    int i = 0;
    #line 339
    i = IS_DEBUG;
}

#line 342
int test_ctrl(void) {
    #line 343
    switch (1) {
        case 0: {
            #line 345
            return 0;
            break;
        }default: {
            #line 347
            return 1;
            break;
        }
    }
}

#line 356
void f4(const char(*x)) {
}

#line 363
void f5(const int(*p)) {
}

#line 366
void test_convert(void) {
    #line 367
    const int(*a) = 0;
    #line 368
    int(*b) = 0;
    #line 369
    a = b;
    #line 370
    void(*p) = 0;
    #line 371
    (f5)(p);
}

#line 374
void test_const(void) {
    #line 375
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 378
    i = 1;
    #line 381
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 386
    const char ((*p)) = (const char *)(0);
    #line 387
    p = (escape_to_char) + (1);
    #line 388
    char (*q) = (char *)(escape_to_char);
    #line 389
    c = q['n'];
    p = (const char *)(1);
    #line 394
    i = (int)((ullong)(p));
}

#line 397
void test_init(void) {
    #line 398
    int x = (const int)(0);
    #line 399
    int y;
    #line 400
    y = 0;
    #line 401
    int z = 42;
    #line 402
    int (a[3]) = {1, 2, 3};
    #line 405
    for (ullong i = 0; (i) < (10); i++) {
        #line 406
        (printf)("%llu\n", i);
    }
}

#line 410
void test_sizeof(void) {
    #line 411
    int i = 0;
    #line 412
    ullong n = sizeof(i);
    #line 413
    n = sizeof(int);
    #line 414
    n = sizeof(int);
    #line 415
    n = sizeof(int *);
}

#line 418
void test_cast(void) {
    #line 419
    int(*p) = 0;
    #line 420
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 427
int main(int argc, const char *(*argv)) {
    #line 428
    if ((argv) == (0)) {
        #line 429
        (printf)("argv is null\n");
    }
    #line 431
    (test_loops)();
    #line 432
    (test_sizeof)();
    #line 433
    (test_assign)();
    #line 434
    (test_enum)();
    #line 435
    (test_arrays)();
    #line 436
    (test_cast)();
    #line 437
    (test_init)();
    #line 438
    (test_lits)();
    #line 439
    (test_const)();
    #line 440
    (test_bool)();
    #line 441
    (test_ops)();
    #line 442
    int b = (example_test)();
    #line 443
    (puts)("Hello, world!");
    #line 444
    int c = (getchar)();
    #line 445
    (printf)("You wrote \'%c\'\n", c);
    #line 446
    (va_test)(1);
    #line 447
    (va_test)(1, 2);
    #line 448
    argv = NULL;
    #line 449
    return 0;
}
