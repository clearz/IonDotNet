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
#line 209 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 12
#define PI 3.14f

#line 13
#define PI2 (PI) + (PI)

#line 15
#define U8 (uint8)(42)

#line 17
char c = 1;

#line 18
uchar uc = 1;

#line 19
schar sc = 1;

#line 21
#define N (((char)(42)) + (8)) != (0)

#line 108
uchar h(void);

#line 23
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 25
char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 34
struct S1 {
    #line 35
    int a;
    #line 36
    const int (b);
};

#line 39
struct S2 {
    #line 40
    S1 s1;
};

#line 43
void f10(int (a[3]));

void test_arrays(void);

#line 53
void test_loops(void);

#line 74
void test_nonmodifiable(void);

#line 86
struct UartCtrl {
    #line 87
    bool tx_enable;
    #line 87
    bool rx_enable;
};

#line 90
#define UART_CTRL_REG (uint *)(0x12345678)

#line 92
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 177
struct Vector {
    #line 178
    int x;
    #line 178
    int y;
};

#line 135
typedef IntOrPtr U;

#line 141
union IntOrPtr {
    #line 142
    int i;
    #line 143
    int(*p);
};

#line 118
int g(U u);

void k(void(*vp), int(*ip));

#line 127
void f1(void);

#line 132
void f3(int (a[]));

#line 137
int example_test(void);

#line 193
int fact_rec(int n);

#line 185
int fact_iter(int n);

#line 146
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 156
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 159
int is_even(int digit);

#line 175
int i;

#line 181
void f2(Vector v);

#line 203
T(*p);

#line 201
#define M (1) + (sizeof(p))

struct T {
    #line 206
    int (a[M]);
};

#line 217
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 224
void test_enum(void);

#line 233
void test_assign(void);

#line 256
void benchmark(int n);

#line 263
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 269
void test_lits(void);

#line 284
void test_ops(void);

#line 314
#define IS_DEBUG true

#line 316
void test_bool(void);

#line 323
int test_ctrl(void);

#line 333
const int (j);

#line 334
const int(*q);

#line 335
const Vector (cv);

#line 337
void f4(const char(*x));

#line 340
struct ConstVector {
    #line 341
    const int (x);
    #line 341
    const int (y);
};

#line 344
void f5(const int(*p));

#line 347
void test_convert(void);

#line 355
void test_const(void);

#line 378
void test_init(void);

#line 391
void test_sizeof(void);

#line 399
void test_cast(void);

#line 408
int main(int argc, const char *(*argv));

// Function declarations
#line 43
void f10(int (a[3])) {
    #line 44
    a[1] = 42;
}

#line 47
void test_arrays(void) {
    #line 48
    int (a[3]) = {1, 2, 3};
    (f10)(a);
}

#line 53
void test_loops(void) {
    #line 54
    while (0) {
    }
    #line 56
    for (int i = 0; (i) < (10); i++) {
    }
    #line 58
    for (;;) {
        #line 59
        break;
    }
    #line 61
    for (int i = 0;;) {
        #line 62
        break;
    }
    #line 64
    for (; 0;) {
    }
    #line 66
    for (int i = 0;; i++) {
        #line 67
        break;
    }
    #line 69
    int i = 0;
    #line 70
    for (;; i++) {
        #line 71
        break;
    }
}

#line 74
void test_nonmodifiable(void) {
    #line 75
    S1 s1;
    #line 76
    s1.a = 0;
    #line 79
    S2 s2;
    #line 80
    s2.s1.a = 0;
}

#line 92
uint32 pack(UartCtrl ctrl) {
    #line 93
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 96
UartCtrl unpack(uint32 word) {
    #line 97
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 100
void test_uart(void) {
    #line 101
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 102
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 103
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 104
    ctrl.rx_enable = true;
    #line 105
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 108
uchar h(void) {
    #line 109
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 110
    Vector (*v) = &((Vector){1, 2});
    #line 111
    v->x = 42;
    #line 112
    int (*p) = &((int){0});
    #line 113
    ulong x = ((uint){1}) + ((long){2});
    #line 114
    int y = +(c);
    #line 115
    return (uchar)(x);
}

#line 118
int g(U u) {
    #line 119
    return u.i;
}

#line 122
void k(void(*vp), int(*ip)) {
    #line 123
    vp = ip;
    #line 124
    ip = vp;
}

#line 127
void f1(void) {
    #line 128
    int (*p) = &((int){0});
    #line 129
    *(p) = 42;
}

#line 132
void f3(int (a[])) {
}

#line 137
int example_test(void) {
    #line 138
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 159
int is_even(int digit) {
    #line 160
    int b = 0;
    #line 161
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 163
            b = 1;
            break;
        }
    }
    #line 165
    return b;
}

#line 181
void f2(Vector v) {
    #line 182
    v = (Vector){0};
}

#line 185
int fact_iter(int n) {
    #line 186
    int r = 1;
    #line 187
    for (int i = 0; (i) <= (n); i++) {
        #line 188
        r *= i;
    }
    #line 190
    return r;
}

#line 193
int fact_rec(int n) {
    #line 194
    if ((n) == (0)) {
        #line 195
        return 1;
    } else {
        #line 197
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 224
void test_enum(void) {
    #line 225
    Color a = COLOR_RED;
    #line 226
    Color b = COLOR_RED;
    #line 227
    int c = (a) + (b);
    #line 228
    int i = a;
    #line 229
    a = i;
    #line 230
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 233
void test_assign(void) {
    #line 234
    int i = 0;
    #line 235
    float f = 3.14f;
    #line 236
    int(*p) = &(i);
    #line 237
    i++;
    #line 238
    i--;
    #line 239
    p++;
    #line 240
    p--;
    #line 241
    p += 1;
    #line 242
    i /= 2;
    #line 243
    i *= 123;
    #line 244
    i %= 3;
    #line 245
    i <<= 1;
    #line 246
    i >>= 2;
    #line 247
    i &= 0xFF;
    #line 248
    i |= 0xFF00;
    #line 249
    i ^= 0xFF0;
}

#line 256
void benchmark(int n) {
    #line 257
    int r = 1;
    #line 258
    for (int i = 1; (i) <= (n); i++) {
        #line 259
        r *= i;
    }
}

#line 263
int va_test(int x, ...) {
    #line 264
    return 0;
}

#line 269
void test_lits(void) {
    #line 270
    float f = 3.14f;
    #line 271
    double d = 3.14;
    #line 272
    int i = 1;
    #line 273
    uint u = 0xFFFFFFFFu;
    #line 274
    long l = 1l;
    #line 275
    ulong ul = 1ul;
    #line 276
    llong ll = 0x100000000ll;
    #line 277
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 278
    uint x1 = 0xFFFFFFFF;
    #line 279
    llong x2 = 4294967295;
    #line 280
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 281
    int x4 = (0xAA) + (0x55);
}

#line 284
void test_ops(void) {
    #line 285
    float pi = 3.14f;
    #line 286
    float f = 0.0f;
    #line 287
    f = +(pi);
    #line 288
    f = -(pi);
    #line 289
    int n = -(1);
    #line 290
    n = ~(n);
    #line 291
    f = ((f) * (pi)) + (n);
    #line 292
    f = (pi) / (pi);
    #line 293
    n = (3) % (2);
    #line 294
    n = (n) + ((uchar)(1));
    #line 295
    int (*p) = &(n);
    #line 296
    p = (p) + (1);
    #line 297
    n = (int)(((p) + (1)) - (p));
    #line 298
    n = (n) << (1);
    #line 299
    n = (n) >> (1);
    #line 300
    int b = ((p) + (1)) > (p);
    #line 301
    b = ((p) + (1)) >= (p);
    #line 302
    b = ((p) + (1)) < (p);
    #line 303
    b = ((p) + (1)) <= (p);
    #line 304
    b = ((p) + (1)) == (p);
    #line 305
    b = (1) > (2);
    #line 306
    b = (1.23f) <= (pi);
    #line 307
    n = 0xFF;
    #line 308
    b = (n) & (~(1));
    #line 309
    b = (n) & (1);
    #line 310
    b = ((n) & (~(1))) ^ (1);
    #line 311
    b = (p) && (pi);
}

#line 316
void test_bool(void) {
    #line 317
    bool b = false;
    #line 318
    b = true;
    #line 319
    int i = 0;
    #line 320
    i = IS_DEBUG;
}

#line 323
int test_ctrl(void) {
    #line 324
    switch (1) {
        case 0: {
            #line 326
            return 0;
            break;
        }default: {
            #line 328
            return 1;
            break;
        }
    }
}

#line 337
void f4(const char(*x)) {
}

#line 344
void f5(const int(*p)) {
}

#line 347
void test_convert(void) {
    #line 348
    const int(*a) = 0;
    #line 349
    int(*b) = 0;
    #line 350
    a = b;
    #line 351
    void(*p) = 0;
    #line 352
    (f5)(p);
}

#line 355
void test_const(void) {
    #line 356
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 359
    i = 1;
    #line 362
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 367
    const char ((*p)) = (const char *)(0);
    #line 368
    p = (escape_to_char) + (1);
    #line 369
    char (*q) = (char *)(escape_to_char);
    #line 370
    c = q['n'];
    p = (const char *)(1);
    #line 375
    i = (int)((ullong)(p));
}

#line 378
void test_init(void) {
    #line 379
    int x = (const int)(0);
    #line 380
    int y;
    #line 381
    y = 0;
    #line 382
    int z = 42;
    #line 383
    int (a[3]) = {1, 2, 3};
    #line 386
    for (ullong i = 0; (i) < (10); i++) {
        #line 387
        (printf)("%llu\n", i);
    }
}

#line 391
void test_sizeof(void) {
    #line 392
    int i = 0;
    #line 393
    ullong n = sizeof(i);
    #line 394
    n = sizeof(int);
    #line 395
    n = sizeof(int);
    #line 396
    n = sizeof(int *);
}

#line 399
void test_cast(void) {
    #line 400
    int(*p) = 0;
    #line 401
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 408
int main(int argc, const char *(*argv)) {
    #line 409
    if ((argv) == (0)) {
        #line 410
        (printf)("argv is null\n");
    }
    #line 412
    (test_loops)();
    #line 413
    (test_sizeof)();
    #line 414
    (test_assign)();
    #line 415
    (test_enum)();
    #line 416
    (test_arrays)();
    #line 417
    (test_cast)();
    #line 418
    (test_init)();
    #line 419
    (test_lits)();
    #line 420
    (test_const)();
    #line 421
    (test_bool)();
    #line 422
    (test_ops)();
    #line 423
    int b = (example_test)();
    #line 424
    (puts)("Hello, world!");
    #line 425
    int c = (getchar)();
    #line 426
    (printf)("You wrote \'%c\'\n", c);
    #line 427
    (va_test)(1);
    #line 428
    (va_test)(1, 2);
    #line 429
    argv = NULL;
    #line 430
    return 0;
}
