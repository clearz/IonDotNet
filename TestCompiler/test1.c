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
typedef struct Incomplete Incomplete;
typedef struct S1 S1;
typedef struct S2 S2;
typedef struct UartCtrl UartCtrl;
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;
typedef struct ConstVector ConstVector;

// Sorted declarations
#line 233 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 14
extern Incomplete(*incomplete_ptr);

#line 28
#define PI 3.14f

#line 29
#define PI2 (PI) + (PI)

#line 31
#define U8 (uint8)(42)

#line 33
extern char c;

#line 34
extern uchar uc;

#line 35
extern schar sc;

#line 37
#define N (((char)(42)) + (8)) != (0)

#line 132
uchar h(void);

#line 39
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 41
extern char (*code);

#line 50
struct S1 {
    #line 51
    int a;
    #line 52
    const int (b);
};

#line 55
struct S2 {
    #line 56
    S1 s1;
};

#line 59
void f10(int (a[3]));

void test_arrays(void);

#line 69
void test_loops(void);

#line 98
void test_nonmodifiable(void);

#line 110
struct UartCtrl {
    #line 111
    bool tx_enable;
    #line 111
    bool rx_enable;
};

#line 114
#define UART_CTRL_REG (uint *)(0x12345678)

#line 116
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 201
struct Vector {
    #line 202
    int x;
    #line 202
    int y;
};

#line 159
typedef IntOrPtr U;

#line 165
union IntOrPtr {
    #line 166
    int i;
    #line 167
    int(*p);
};

#line 142
int g(U u);

void k(void(*vp), int(*ip));

#line 151
void f1(void);

#line 156
void f3(int (a[]));

#line 161
int example_test(void);

#line 217
int fact_rec(int n);

#line 209
int fact_iter(int n);

#line 170
extern const char (escape_to_char[256]);

#line 180
extern int (a2[11]);

#line 183
int is_even(int digit);

#line 199
extern int i;

#line 205
void f2(Vector v);

#line 227
extern T(*p);

#line 225
#define M (1) + (sizeof(p))

struct T {
    #line 230
    int (a[M]);
};

#line 241
extern const char * (color_names[NUM_COLORS]);

#line 248
void test_enum(void);

#line 257
void test_assign(void);

#line 280
void benchmark(int n);

#line 287
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 293
void test_lits(void);

#line 308
void test_ops(void);

#line 338
#define IS_DEBUG true

#line 340
void test_bool(void);

#line 347
int test_ctrl(void);

#line 357
extern const int (j);

#line 358
extern const int(*q);

#line 359
extern const Vector (cv);

#line 361
void f4(const char(*x));

#line 364
struct ConstVector {
    #line 365
    const int (x);
    #line 365
    const int (y);
};

#line 368
void f5(const int(*p));

#line 371
void test_convert(void);

#line 379
void test_const(void);

#line 402
void test_init(void);

#line 415
void test_sizeof(void);

#line 423
void test_cast(void);

#line 432
int main(int argc, const char *(*argv));

// Definitions

Incomplete(*incomplete_ptr);

char c = 1;

uchar uc = 1;

schar sc = 1;

char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 59
void f10(int (a[3])) {
    #line 60
    a[1] = 42;
}

#line 63
void test_arrays(void) {
    #line 64
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 69
void test_loops(void) {
    #line 72
    while (0) {
    }
    #line 74
    for (int i = 0; (i) < (10); i++) {
    }
    #line 77
    for (;;) {
        #line 78
        break;
    }
    #line 80
    for (int i = 0;;) {
        #line 81
        break;
    }
    #line 83
    for (; 0;) {
    }
    #line 85
    for (int i = 0;; i++) {
        #line 86
        break;
    }
    #line 88
    int i = 0;
    #line 89
    for (;; i++) {
        #line 90
        break;
    }
    #line 92
    switch (0) {default: {
            break;
        }
    }
}

#line 98
void test_nonmodifiable(void) {
    #line 99
    S1 s1;
    #line 100
    s1.a = 0;
    #line 103
    S2 s2;
    #line 104
    s2.s1.a = 0;
}

#line 116
uint32 pack(UartCtrl ctrl) {
    #line 117
    return ((ctrl.tx_enable) & (1)) | (((ctrl.rx_enable) & (1)) << (1));
}

#line 120
UartCtrl unpack(uint32 word) {
    #line 121
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 124
void test_uart(void) {
    #line 125
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 126
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 127
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 128
    ctrl.rx_enable = true;
    #line 129
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 132
uchar h(void) {
    #line 133
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 134
    Vector (*v) = &((Vector){1, 2});
    #line 135
    v->x = 42;
    #line 136
    int (*p) = &((int){0});
    #line 137
    ulong x = ((uint){1}) + ((long){2});
    #line 138
    int y = +(c);
    #line 139
    return (uchar)(x);
}

#line 142
int g(U u) {
    #line 143
    return u.i;
}

#line 146
void k(void(*vp), int(*ip)) {
    #line 147
    vp = ip;
    #line 148
    ip = vp;
}

#line 151
void f1(void) {
    #line 152
    int (*p) = &((int){0});
    #line 153
    *(p) = 42;
}

#line 156
void f3(int (a[])) {
}

#line 161
int example_test(void) {
    #line 162
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

int (a2[11]) = {1, 2, 3, [10] = 4};

#line 183
int is_even(int digit) {
    #line 184
    int b = 0;
    #line 185
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 187
            b = 1;
            break;
        }
    }
    #line 189
    return b;
}

int i;

#line 205
void f2(Vector v) {
    #line 206
    v = (Vector){0};
}

#line 209
int fact_iter(int n) {
    #line 210
    int r = 1;
    #line 211
    for (int i = 0; (i) <= (n); i++) {
        #line 212
        r *= i;
    }
    #line 214
    return r;
}

#line 217
int fact_rec(int n) {
    #line 218
    if ((n) == (0)) {
        #line 219
        return 1;
    } else {
        #line 221
        return (n) * ((fact_rec)((n) - (1)));
    }
}

T(*p);

const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 248
void test_enum(void) {
    #line 249
    Color a = COLOR_RED;
    #line 250
    Color b = COLOR_RED;
    #line 251
    int c = (a) + (b);
    #line 252
    int i = a;
    #line 253
    a = i;
    #line 254
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 257
void test_assign(void) {
    #line 258
    int i = 0;
    #line 259
    float f = 3.14f;
    #line 260
    int(*p) = &(i);
    #line 261
    i++;
    #line 262
    i--;
    #line 263
    p++;
    #line 264
    p--;
    #line 265
    p += 1;
    #line 266
    i /= 2;
    #line 267
    i *= 123;
    #line 268
    i %= 3;
    #line 269
    i <<= 1;
    #line 270
    i >>= 2;
    #line 271
    i &= 0xFF;
    #line 272
    i |= 0xFF00;
    #line 273
    i ^= 0xFF0;
}

#line 280
void benchmark(int n) {
    #line 281
    int r = 1;
    #line 282
    for (int i = 1; (i) <= (n); i++) {
        #line 283
        r *= i;
    }
}

#line 287
int va_test(int x, ...) {
    #line 288
    return 0;
}

#line 293
void test_lits(void) {
    #line 294
    float f = 3.14f;
    #line 295
    double d = 3.14;
    #line 296
    int i = 1;
    #line 297
    uint u = 0xFFFFFFFFu;
    #line 298
    long l = 1l;
    #line 299
    ulong ul = 1ul;
    #line 300
    llong ll = 0x100000000ll;
    #line 301
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 302
    uint x1 = 0xFFFFFFFF;
    #line 303
    llong x2 = 4294967295;
    #line 304
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 305
    int x4 = (0xAA) + (0x55);
}

#line 308
void test_ops(void) {
    #line 309
    float pi = 3.14f;
    #line 310
    float f = 0.0f;
    #line 311
    f = +(pi);
    #line 312
    f = -(pi);
    #line 313
    int n = -(1);
    #line 314
    n = ~(n);
    #line 315
    f = ((f) * (pi)) + (n);
    #line 316
    f = (pi) / (pi);
    #line 317
    n = (3) % (2);
    #line 318
    n = (n) + ((uchar)(1));
    #line 319
    int (*p) = &(n);
    #line 320
    p = (p) + (1);
    #line 321
    n = (int)(((p) + (1)) - (p));
    #line 322
    n = (n) << (1);
    #line 323
    n = (n) >> (1);
    #line 324
    int b = ((p) + (1)) > (p);
    #line 325
    b = ((p) + (1)) >= (p);
    #line 326
    b = ((p) + (1)) < (p);
    #line 327
    b = ((p) + (1)) <= (p);
    #line 328
    b = ((p) + (1)) == (p);
    #line 329
    b = (1) > (2);
    #line 330
    b = (1.23f) <= (pi);
    #line 331
    n = 0xFF;
    #line 332
    b = (n) & (~(1));
    #line 333
    b = (n) & (1);
    #line 334
    b = ((n) & (~(1))) ^ (1);
    #line 335
    b = (p) && (pi);
}

#line 340
void test_bool(void) {
    #line 341
    bool b = false;
    #line 342
    b = true;
    #line 343
    int i = 0;
    #line 344
    i = IS_DEBUG;
}

#line 347
int test_ctrl(void) {
    #line 348
    switch (1) {
        case 0: {
            #line 350
            return 0;
            break;
        }default: {
            #line 352
            return 1;
            break;
        }
    }
}

const int (j);

const int(*q);

const Vector (cv);

#line 361
void f4(const char(*x)) {
}

#line 368
void f5(const int(*p)) {
}

#line 371
void test_convert(void) {
    #line 372
    const int(*a) = 0;
    #line 373
    int(*b) = 0;
    #line 374
    a = b;
    #line 375
    void(*p) = 0;
    #line 376
    (f5)(p);
}

#line 379
void test_const(void) {
    #line 380
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 383
    i = 1;
    #line 386
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 391
    const char ((*p)) = (const char *)(0);
    #line 392
    p = (escape_to_char) + (1);
    #line 393
    char (*q) = (char *)(escape_to_char);
    #line 394
    c = q['n'];
    p = (const char *)(1);
    #line 399
    i = (int)((ullong)(p));
}

#line 402
void test_init(void) {
    #line 403
    int x = (const int)(0);
    #line 404
    int y;
    #line 405
    y = 0;
    #line 406
    int z = 42;
    #line 407
    int (a[3]) = {1, 2, 3};
    #line 410
    for (ullong i = 0; (i) < (10); i++) {
        #line 411
        (printf)("%llu\n", i);
    }
}

#line 415
void test_sizeof(void) {
    #line 416
    int i = 0;
    #line 417
    ullong n = sizeof(i);
    #line 418
    n = sizeof(int);
    #line 419
    n = sizeof(int);
    #line 420
    n = sizeof(int *);
}

#line 423
void test_cast(void) {
    #line 424
    int(*p) = 0;
    #line 425
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 432
int main(int argc, const char *(*argv)) {
    #line 433
    if ((argv) == (0)) {
        #line 434
        (printf)("argv is null\n");
    }
    #line 436
    (test_loops)();
    #line 437
    (test_sizeof)();
    #line 438
    (test_assign)();
    #line 439
    (test_enum)();
    #line 440
    (test_arrays)();
    #line 441
    (test_cast)();
    #line 442
    (test_init)();
    #line 443
    (test_lits)();
    #line 444
    (test_const)();
    #line 445
    (test_bool)();
    #line 446
    (test_ops)();
    #line 447
    int b = (example_test)();
    #line 448
    (puts)("Hello, world!");
    #line 449
    int c = (getchar)();
    #line 450
    (printf)("You wrote \'%c\'\n", c);
    #line 451
    (va_test)(1);
    #line 452
    (va_test)(1, 2);
    #line 453
    argv = NULL;
    #line 454
    return 0;
}
