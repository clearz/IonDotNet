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
#line 216 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 19
#define PI 3.14f

#line 20
#define PI2 (PI) + (PI)

#line 22
#define U8 (uint8)(42)

#line 24
char c = 1;

#line 25
uchar uc = 1;

#line 26
schar sc = 1;

#line 28
#define N (((char)(42)) + (8)) != (0)

#line 115
uchar h(void);

#line 30
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 32
char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 41
struct S1 {
    #line 42
    int a;
    #line 43
    const int (b);
};

#line 46
struct S2 {
    #line 47
    S1 s1;
};

#line 50
void f10(int (a[3]));

void test_arrays(void);

#line 60
void test_loops(void);

#line 81
void test_nonmodifiable(void);

#line 93
struct UartCtrl {
    #line 94
    bool tx_enable;
    #line 94
    bool rx_enable;
};

#line 97
#define UART_CTRL_REG (uint *)(0x12345678)

#line 99
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 184
struct Vector {
    #line 185
    int x;
    #line 185
    int y;
};

#line 142
typedef IntOrPtr U;

#line 148
union IntOrPtr {
    #line 149
    int i;
    #line 150
    int(*p);
};

#line 125
int g(U u);

void k(void(*vp), int(*ip));

#line 134
void f1(void);

#line 139
void f3(int (a[]));

#line 144
int example_test(void);

#line 200
int fact_rec(int n);

#line 192
int fact_iter(int n);

#line 153
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 163
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 166
int is_even(int digit);

#line 182
int i;

#line 188
void f2(Vector v);

#line 210
T(*p);

#line 208
#define M (1) + (sizeof(p))

struct T {
    #line 213
    int (a[M]);
};

#line 224
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 231
void test_enum(void);

#line 240
void test_assign(void);

#line 263
void benchmark(int n);

#line 270
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 276
void test_lits(void);

#line 291
void test_ops(void);

#line 321
#define IS_DEBUG true

#line 323
void test_bool(void);

#line 330
int test_ctrl(void);

#line 340
const int (j);

#line 341
const int(*q);

#line 342
const Vector (cv);

#line 344
void f4(const char(*x));

#line 347
struct ConstVector {
    #line 348
    const int (x);
    #line 348
    const int (y);
};

#line 351
void f5(const int(*p));

#line 354
void test_convert(void);

#line 362
void test_const(void);

#line 385
void test_init(void);

#line 398
void test_sizeof(void);

#line 406
void test_cast(void);

#line 415
int main(int argc, const char *(*argv));

// Function declarations
#line 50
void f10(int (a[3])) {
    #line 51
    a[1] = 42;
}

#line 54
void test_arrays(void) {
    #line 55
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 60
void test_loops(void) {
    #line 61
    while (0) {
    }
    #line 63
    for (int i = 0; (i) < (10); i++) {
    }
    #line 65
    for (;;) {
        #line 66
        break;
    }
    #line 68
    for (int i = 0;;) {
        #line 69
        break;
    }
    #line 71
    for (; 0;) {
    }
    #line 73
    for (int i = 0;; i++) {
        #line 74
        break;
    }
    #line 76
    int i = 0;
    #line 77
    for (;; i++) {
        #line 78
        break;
    }
}

#line 81
void test_nonmodifiable(void) {
    #line 82
    S1 s1;
    #line 83
    s1.a = 0;
    #line 86
    S2 s2;
    #line 87
    s2.s1.a = 0;
}

#line 99
uint32 pack(UartCtrl ctrl) {
    #line 100
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 103
UartCtrl unpack(uint32 word) {
    #line 104
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 107
void test_uart(void) {
    #line 108
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 109
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 110
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 111
    ctrl.rx_enable = true;
    #line 112
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 115
uchar h(void) {
    #line 116
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 117
    Vector (*v) = &((Vector){1, 2});
    #line 118
    v->x = 42;
    #line 119
    int (*p) = &((int){0});
    #line 120
    ulong x = ((uint){1}) + ((long){2});
    #line 121
    int y = +(c);
    #line 122
    return (uchar)(x);
}

#line 125
int g(U u) {
    #line 126
    return u.i;
}

#line 129
void k(void(*vp), int(*ip)) {
    #line 130
    vp = ip;
    #line 131
    ip = vp;
}

#line 134
void f1(void) {
    #line 135
    int (*p) = &((int){0});
    #line 136
    *(p) = 42;
}

#line 139
void f3(int (a[])) {
}

#line 144
int example_test(void) {
    #line 145
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 166
int is_even(int digit) {
    #line 167
    int b = 0;
    #line 168
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 170
            b = 1;
            break;
        }
    }
    #line 172
    return b;
}

#line 188
void f2(Vector v) {
    #line 189
    v = (Vector){0};
}

#line 192
int fact_iter(int n) {
    #line 193
    int r = 1;
    #line 194
    for (int i = 0; (i) <= (n); i++) {
        #line 195
        r *= i;
    }
    #line 197
    return r;
}

#line 200
int fact_rec(int n) {
    #line 201
    if ((n) == (0)) {
        #line 202
        return 1;
    } else {
        #line 204
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 231
void test_enum(void) {
    #line 232
    Color a = COLOR_RED;
    #line 233
    Color b = COLOR_RED;
    #line 234
    int c = (a) + (b);
    #line 235
    int i = a;
    #line 236
    a = i;
    #line 237
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 240
void test_assign(void) {
    #line 241
    int i = 0;
    #line 242
    float f = 3.14f;
    #line 243
    int(*p) = &(i);
    #line 244
    i++;
    #line 245
    i--;
    #line 246
    p++;
    #line 247
    p--;
    #line 248
    p += 1;
    #line 249
    i /= 2;
    #line 250
    i *= 123;
    #line 251
    i %= 3;
    #line 252
    i <<= 1;
    #line 253
    i >>= 2;
    #line 254
    i &= 0xFF;
    #line 255
    i |= 0xFF00;
    #line 256
    i ^= 0xFF0;
}

#line 263
void benchmark(int n) {
    #line 264
    int r = 1;
    #line 265
    for (int i = 1; (i) <= (n); i++) {
        #line 266
        r *= i;
    }
}

#line 270
int va_test(int x, ...) {
    #line 271
    return 0;
}

#line 276
void test_lits(void) {
    #line 277
    float f = 3.14f;
    #line 278
    double d = 3.14;
    #line 279
    int i = 1;
    #line 280
    uint u = 0xFFFFFFFFu;
    #line 281
    long l = 1l;
    #line 282
    ulong ul = 1ul;
    #line 283
    llong ll = 0x100000000ll;
    #line 284
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 285
    uint x1 = 0xFFFFFFFF;
    #line 286
    llong x2 = 4294967295;
    #line 287
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 288
    int x4 = (0xAA) + (0x55);
}

#line 291
void test_ops(void) {
    #line 292
    float pi = 3.14f;
    #line 293
    float f = 0.0f;
    #line 294
    f = +(pi);
    #line 295
    f = -(pi);
    #line 296
    int n = -(1);
    #line 297
    n = ~(n);
    #line 298
    f = ((f) * (pi)) + (n);
    #line 299
    f = (pi) / (pi);
    #line 300
    n = (3) % (2);
    #line 301
    n = (n) + ((uchar)(1));
    #line 302
    int (*p) = &(n);
    #line 303
    p = (p) + (1);
    #line 304
    n = (int)(((p) + (1)) - (p));
    #line 305
    n = (n) << (1);
    #line 306
    n = (n) >> (1);
    #line 307
    int b = ((p) + (1)) > (p);
    #line 308
    b = ((p) + (1)) >= (p);
    #line 309
    b = ((p) + (1)) < (p);
    #line 310
    b = ((p) + (1)) <= (p);
    #line 311
    b = ((p) + (1)) == (p);
    #line 312
    b = (1) > (2);
    #line 313
    b = (1.23f) <= (pi);
    #line 314
    n = 0xFF;
    #line 315
    b = (n) & (~(1));
    #line 316
    b = (n) & (1);
    #line 317
    b = ((n) & (~(1))) ^ (1);
    #line 318
    b = (p) && (pi);
}

#line 323
void test_bool(void) {
    #line 324
    bool b = false;
    #line 325
    b = true;
    #line 326
    int i = 0;
    #line 327
    i = IS_DEBUG;
}

#line 330
int test_ctrl(void) {
    #line 331
    switch (1) {
        case 0: {
            #line 333
            return 0;
            break;
        }default: {
            #line 335
            return 1;
            break;
        }
    }
}

#line 344
void f4(const char(*x)) {
}

#line 351
void f5(const int(*p)) {
}

#line 354
void test_convert(void) {
    #line 355
    const int(*a) = 0;
    #line 356
    int(*b) = 0;
    #line 357
    a = b;
    #line 358
    void(*p) = 0;
    #line 359
    (f5)(p);
}

#line 362
void test_const(void) {
    #line 363
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 366
    i = 1;
    #line 369
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 374
    const char ((*p)) = (const char *)(0);
    #line 375
    p = (escape_to_char) + (1);
    #line 376
    char (*q) = (char *)(escape_to_char);
    #line 377
    c = q['n'];
    p = (const char *)(1);
    #line 382
    i = (int)((ullong)(p));
}

#line 385
void test_init(void) {
    #line 386
    int x = (const int)(0);
    #line 387
    int y;
    #line 388
    y = 0;
    #line 389
    int z = 42;
    #line 390
    int (a[3]) = {1, 2, 3};
    #line 393
    for (ullong i = 0; (i) < (10); i++) {
        #line 394
        (printf)("%llu\n", i);
    }
}

#line 398
void test_sizeof(void) {
    #line 399
    int i = 0;
    #line 400
    ullong n = sizeof(i);
    #line 401
    n = sizeof(int);
    #line 402
    n = sizeof(int);
    #line 403
    n = sizeof(int *);
}

#line 406
void test_cast(void) {
    #line 407
    int(*p) = 0;
    #line 408
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 415
int main(int argc, const char *(*argv)) {
    #line 416
    if ((argv) == (0)) {
        #line 417
        (printf)("argv is null\n");
    }
    #line 419
    (test_loops)();
    #line 420
    (test_sizeof)();
    #line 421
    (test_assign)();
    #line 422
    (test_enum)();
    #line 423
    (test_arrays)();
    #line 424
    (test_cast)();
    #line 425
    (test_init)();
    #line 426
    (test_lits)();
    #line 427
    (test_const)();
    #line 428
    (test_bool)();
    #line 429
    (test_ops)();
    #line 430
    int b = (example_test)();
    #line 431
    (puts)("Hello, world!");
    #line 432
    int c = (getchar)();
    #line 433
    (printf)("You wrote \'%c\'\n", c);
    #line 434
    (va_test)(1);
    #line 435
    (va_test)(1, 2);
    #line 436
    argv = NULL;
    #line 437
    return 0;
}
