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
#line 218 "test1.ion"
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

#line 117
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

#line 83
void test_nonmodifiable(void);

#line 95
struct UartCtrl {
    #line 96
    bool tx_enable;
    #line 96
    bool rx_enable;
};

#line 99
#define UART_CTRL_REG (uint *)(0x12345678)

#line 101
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 186
struct Vector {
    #line 187
    int x;
    #line 187
    int y;
};

#line 144
typedef IntOrPtr U;

#line 150
union IntOrPtr {
    #line 151
    int i;
    #line 152
    int(*p);
};

#line 127
int g(U u);

void k(void(*vp), int(*ip));

#line 136
void f1(void);

#line 141
void f3(int (a[]));

#line 146
int example_test(void);

#line 202
int fact_rec(int n);

#line 194
int fact_iter(int n);

#line 155
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 165
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 168
int is_even(int digit);

#line 184
int i;

#line 190
void f2(Vector v);

#line 212
T(*p);

#line 210
#define M (1) + (sizeof(p))

struct T {
    #line 215
    int (a[M]);
};

#line 226
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 233
void test_enum(void);

#line 242
void test_assign(void);

#line 265
void benchmark(int n);

#line 272
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 278
void test_lits(void);

#line 293
void test_ops(void);

#line 323
#define IS_DEBUG true

#line 325
void test_bool(void);

#line 332
int test_ctrl(void);

#line 342
const int (j);

#line 343
const int(*q);

#line 344
const Vector (cv);

#line 346
void f4(const char(*x));

#line 349
struct ConstVector {
    #line 350
    const int (x);
    #line 350
    const int (y);
};

#line 353
void f5(const int(*p));

#line 356
void test_convert(void);

#line 364
void test_const(void);

#line 387
void test_init(void);

#line 400
void test_sizeof(void);

#line 408
void test_cast(void);

#line 417
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
    #line 63
    while (0) {
    }
    #line 65
    for (int i = 0; (i) < (10); i++) {
    }
    #line 67
    for (;;) {
        #line 68
        break;
    }
    #line 70
    for (int i = 0;;) {
        #line 71
        break;
    }
    #line 73
    for (; 0;) {
    }
    #line 75
    for (int i = 0;; i++) {
        #line 76
        break;
    }
    #line 78
    int i = 0;
    #line 79
    for (;; i++) {
        #line 80
        break;
    }
}

#line 83
void test_nonmodifiable(void) {
    #line 84
    S1 s1;
    #line 85
    s1.a = 0;
    #line 88
    S2 s2;
    #line 89
    s2.s1.a = 0;
}

#line 101
uint32 pack(UartCtrl ctrl) {
    #line 102
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 105
UartCtrl unpack(uint32 word) {
    #line 106
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 109
void test_uart(void) {
    #line 110
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 111
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 112
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 113
    ctrl.rx_enable = true;
    #line 114
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 117
uchar h(void) {
    #line 118
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 119
    Vector (*v) = &((Vector){1, 2});
    #line 120
    v->x = 42;
    #line 121
    int (*p) = &((int){0});
    #line 122
    ulong x = ((uint){1}) + ((long){2});
    #line 123
    int y = +(c);
    #line 124
    return (uchar)(x);
}

#line 127
int g(U u) {
    #line 128
    return u.i;
}

#line 131
void k(void(*vp), int(*ip)) {
    #line 132
    vp = ip;
    #line 133
    ip = vp;
}

#line 136
void f1(void) {
    #line 137
    int (*p) = &((int){0});
    #line 138
    *(p) = 42;
}

#line 141
void f3(int (a[])) {
}

#line 146
int example_test(void) {
    #line 147
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 168
int is_even(int digit) {
    #line 169
    int b = 0;
    #line 170
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 172
            b = 1;
            break;
        }
    }
    #line 174
    return b;
}

#line 190
void f2(Vector v) {
    #line 191
    v = (Vector){0};
}

#line 194
int fact_iter(int n) {
    #line 195
    int r = 1;
    #line 196
    for (int i = 0; (i) <= (n); i++) {
        #line 197
        r *= i;
    }
    #line 199
    return r;
}

#line 202
int fact_rec(int n) {
    #line 203
    if ((n) == (0)) {
        #line 204
        return 1;
    } else {
        #line 206
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 233
void test_enum(void) {
    #line 234
    Color a = COLOR_RED;
    #line 235
    Color b = COLOR_RED;
    #line 236
    int c = (a) + (b);
    #line 237
    int i = a;
    #line 238
    a = i;
    #line 239
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 242
void test_assign(void) {
    #line 243
    int i = 0;
    #line 244
    float f = 3.14f;
    #line 245
    int(*p) = &(i);
    #line 246
    i++;
    #line 247
    i--;
    #line 248
    p++;
    #line 249
    p--;
    #line 250
    p += 1;
    #line 251
    i /= 2;
    #line 252
    i *= 123;
    #line 253
    i %= 3;
    #line 254
    i <<= 1;
    #line 255
    i >>= 2;
    #line 256
    i &= 0xFF;
    #line 257
    i |= 0xFF00;
    #line 258
    i ^= 0xFF0;
}

#line 265
void benchmark(int n) {
    #line 266
    int r = 1;
    #line 267
    for (int i = 1; (i) <= (n); i++) {
        #line 268
        r *= i;
    }
}

#line 272
int va_test(int x, ...) {
    #line 273
    return 0;
}

#line 278
void test_lits(void) {
    #line 279
    float f = 3.14f;
    #line 280
    double d = 3.14;
    #line 281
    int i = 1;
    #line 282
    uint u = 0xFFFFFFFFu;
    #line 283
    long l = 1l;
    #line 284
    ulong ul = 1ul;
    #line 285
    llong ll = 0x100000000ll;
    #line 286
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 287
    uint x1 = 0xFFFFFFFF;
    #line 288
    llong x2 = 4294967295;
    #line 289
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 290
    int x4 = (0xAA) + (0x55);
}

#line 293
void test_ops(void) {
    #line 294
    float pi = 3.14f;
    #line 295
    float f = 0.0f;
    #line 296
    f = +(pi);
    #line 297
    f = -(pi);
    #line 298
    int n = -(1);
    #line 299
    n = ~(n);
    #line 300
    f = ((f) * (pi)) + (n);
    #line 301
    f = (pi) / (pi);
    #line 302
    n = (3) % (2);
    #line 303
    n = (n) + ((uchar)(1));
    #line 304
    int (*p) = &(n);
    #line 305
    p = (p) + (1);
    #line 306
    n = (int)(((p) + (1)) - (p));
    #line 307
    n = (n) << (1);
    #line 308
    n = (n) >> (1);
    #line 309
    int b = ((p) + (1)) > (p);
    #line 310
    b = ((p) + (1)) >= (p);
    #line 311
    b = ((p) + (1)) < (p);
    #line 312
    b = ((p) + (1)) <= (p);
    #line 313
    b = ((p) + (1)) == (p);
    #line 314
    b = (1) > (2);
    #line 315
    b = (1.23f) <= (pi);
    #line 316
    n = 0xFF;
    #line 317
    b = (n) & (~(1));
    #line 318
    b = (n) & (1);
    #line 319
    b = ((n) & (~(1))) ^ (1);
    #line 320
    b = (p) && (pi);
}

#line 325
void test_bool(void) {
    #line 326
    bool b = false;
    #line 327
    b = true;
    #line 328
    int i = 0;
    #line 329
    i = IS_DEBUG;
}

#line 332
int test_ctrl(void) {
    #line 333
    switch (1) {
        case 0: {
            #line 335
            return 0;
            break;
        }default: {
            #line 337
            return 1;
            break;
        }
    }
}

#line 346
void f4(const char(*x)) {
}

#line 353
void f5(const int(*p)) {
}

#line 356
void test_convert(void) {
    #line 357
    const int(*a) = 0;
    #line 358
    int(*b) = 0;
    #line 359
    a = b;
    #line 360
    void(*p) = 0;
    #line 361
    (f5)(p);
}

#line 364
void test_const(void) {
    #line 365
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 368
    i = 1;
    #line 371
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 376
    const char ((*p)) = (const char *)(0);
    #line 377
    p = (escape_to_char) + (1);
    #line 378
    char (*q) = (char *)(escape_to_char);
    #line 379
    c = q['n'];
    p = (const char *)(1);
    #line 384
    i = (int)((ullong)(p));
}

#line 387
void test_init(void) {
    #line 388
    int x = (const int)(0);
    #line 389
    int y;
    #line 390
    y = 0;
    #line 391
    int z = 42;
    #line 392
    int (a[3]) = {1, 2, 3};
    #line 395
    for (ullong i = 0; (i) < (10); i++) {
        #line 396
        (printf)("%llu\n", i);
    }
}

#line 400
void test_sizeof(void) {
    #line 401
    int i = 0;
    #line 402
    ullong n = sizeof(i);
    #line 403
    n = sizeof(int);
    #line 404
    n = sizeof(int);
    #line 405
    n = sizeof(int *);
}

#line 408
void test_cast(void) {
    #line 409
    int(*p) = 0;
    #line 410
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 417
int main(int argc, const char *(*argv)) {
    #line 418
    if ((argv) == (0)) {
        #line 419
        (printf)("argv is null\n");
    }
    #line 421
    (test_loops)();
    #line 422
    (test_sizeof)();
    #line 423
    (test_assign)();
    #line 424
    (test_enum)();
    #line 425
    (test_arrays)();
    #line 426
    (test_cast)();
    #line 427
    (test_init)();
    #line 428
    (test_lits)();
    #line 429
    (test_const)();
    #line 430
    (test_bool)();
    #line 431
    (test_ops)();
    #line 432
    int b = (example_test)();
    #line 433
    (puts)("Hello, world!");
    #line 434
    int c = (getchar)();
    #line 435
    (printf)("You wrote \'%c\'\n", c);
    #line 436
    (va_test)(1);
    #line 437
    (va_test)(1, 2);
    #line 438
    argv = NULL;
    #line 439
    return 0;
}
