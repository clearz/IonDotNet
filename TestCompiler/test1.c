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
#line 187 "test1.ion"
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

#line 86
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

#line 52
void test_nonmodifiable(void);

#line 64
struct UartCtrl {
    #line 65
    bool tx_enable;
    #line 65
    bool rx_enable;
};

#line 68
#define UART_CTRL_REG (uint *)(0x12345678)

#line 70
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 155
struct Vector {
    #line 156
    int x;
    #line 156
    int y;
};

#line 113
typedef IntOrPtr U;

#line 119
union IntOrPtr {
    #line 120
    int i;
    #line 121
    int(*p);
};

#line 96
int g(U u);

void k(void(*vp), int(*ip));

#line 105
void f1(void);

#line 110
void f3(int (a[]));

#line 115
int example_test(void);

#line 171
int fact_rec(int n);

#line 163
int fact_iter(int n);

#line 124
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 134
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 137
int is_even(int digit);

#line 153
int i;

#line 159
void f2(Vector v);

#line 181
T(*p);

#line 179
#define M (1) + (sizeof(p))

struct T {
    #line 184
    int (a[M]);
};

#line 195
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 202
void test_enum(void);

#line 211
void test_assign(void);

#line 234
void benchmark(int n);

#line 241
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 247
void test_lits(void);

#line 262
void test_ops(void);

#line 292
#define IS_DEBUG true

#line 294
void test_bool(void);

#line 301
int test_ctrl(void);

#line 311
const int (j);

#line 312
const int(*q);

#line 313
const Vector (cv);

#line 315
void f4(const char(*x));

#line 318
struct ConstVector {
    #line 319
    const int (x);
    #line 319
    const int (y);
};

#line 322
void f5(const int(*p));

#line 325
void test_convert(void);

#line 333
void test_const(void);

#line 356
void test_init(void);

#line 369
void test_sizeof(void);

#line 377
void test_cast(void);

#line 386
void test_loops(void);

#line 404
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
    #line 49
    (f10)(a);
}

#line 52
void test_nonmodifiable(void) {
    #line 53
    S1 s1;
    #line 54
    s1.a = 0;
    #line 57
    S2 s2;
    #line 58
    s2.s1.a = 0;
}

#line 70
uint32 pack(UartCtrl ctrl) {
    #line 71
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 74
UartCtrl unpack(uint32 word) {
    #line 75
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 78
void test_uart(void) {
    #line 79
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 80
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 81
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 82
    ctrl.rx_enable = true;
    #line 83
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 86
uchar h(void) {
    #line 87
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 88
    Vector (*v) = &((Vector){1, 2});
    #line 89
    v->x = 42;
    #line 90
    int (*p) = &((int){0});
    #line 91
    ulong x = ((uint){1}) + ((long){2});
    #line 92
    int y = +(c);
    #line 93
    return (uchar)(x);
}

#line 96
int g(U u) {
    #line 97
    return u.i;
}

#line 100
void k(void(*vp), int(*ip)) {
    #line 101
    vp = ip;
    #line 102
    ip = vp;
}

#line 105
void f1(void) {
    #line 106
    int (*p) = &((int){0});
    #line 107
    *(p) = 42;
}

#line 110
void f3(int (a[])) {
}

#line 115
int example_test(void) {
    #line 116
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 137
int is_even(int digit) {
    #line 138
    int b = 0;
    #line 139
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 141
            b = 1;
            break;
        }
    }
    #line 143
    return b;
}

#line 159
void f2(Vector v) {
    #line 160
    v = (Vector){0};
}

#line 163
int fact_iter(int n) {
    #line 164
    int r = 1;
    #line 165
    for (int i = 0; (i) <= (n); i++) {
        #line 166
        r *= i;
    }
    #line 168
    return r;
}

#line 171
int fact_rec(int n) {
    #line 172
    if ((n) == (0)) {
        #line 173
        return 1;
    } else {
        #line 175
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 202
void test_enum(void) {
    #line 203
    Color a = COLOR_RED;
    #line 204
    Color b = COLOR_RED;
    #line 205
    int c = (a) + (b);
    #line 206
    int i = a;
    #line 207
    a = i;
    #line 208
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 211
void test_assign(void) {
    #line 212
    int i = 0;
    #line 213
    float f = 3.14f;
    #line 214
    int(*p) = &(i);
    #line 215
    i++;
    #line 216
    i--;
    #line 217
    p++;
    #line 218
    p--;
    #line 219
    p += 1;
    #line 220
    i /= 2;
    #line 221
    i *= 123;
    #line 222
    i %= 3;
    #line 223
    i <<= 1;
    #line 224
    i >>= 2;
    #line 225
    i &= 0xFF;
    #line 226
    i |= 0xFF00;
    #line 227
    i ^= 0xFF0;
}

#line 234
void benchmark(int n) {
    #line 235
    int r = 1;
    #line 236
    for (int i = 1; (i) <= (n); i++) {
        #line 237
        r *= i;
    }
}

#line 241
int va_test(int x, ...) {
    #line 242
    return 0;
}

#line 247
void test_lits(void) {
    #line 248
    float f = 3.14f;
    #line 249
    double d = 3.14;
    #line 250
    int i = 1;
    #line 251
    uint u = 0xFFFFFFFFu;
    #line 252
    long l = 1l;
    #line 253
    ulong ul = 1ul;
    #line 254
    llong ll = 0x100000000ll;
    #line 255
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 256
    uint x1 = 0xFFFFFFFF;
    #line 257
    llong x2 = 4294967295;
    #line 258
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 259
    int x4 = (0xAA) + (0x55);
}

#line 262
void test_ops(void) {
    #line 263
    float pi = 3.14f;
    #line 264
    float f = 0.0f;
    #line 265
    f = +(pi);
    #line 266
    f = -(pi);
    #line 267
    int n = -(1);
    #line 268
    n = ~(n);
    #line 269
    f = ((f) * (pi)) + (n);
    #line 270
    f = (pi) / (pi);
    #line 271
    n = (3) % (2);
    #line 272
    n = (n) + ((uchar)(1));
    #line 273
    int (*p) = &(n);
    #line 274
    p = (p) + (1);
    #line 275
    n = (int)(((p) + (1)) - (p));
    #line 276
    n = (n) << (1);
    #line 277
    n = (n) >> (1);
    #line 278
    int b = ((p) + (1)) > (p);
    #line 279
    b = ((p) + (1)) >= (p);
    #line 280
    b = ((p) + (1)) < (p);
    #line 281
    b = ((p) + (1)) <= (p);
    #line 282
    b = ((p) + (1)) == (p);
    #line 283
    b = (1) > (2);
    #line 284
    b = (1.23f) <= (pi);
    #line 285
    n = 0xFF;
    #line 286
    b = (n) & (~(1));
    #line 287
    b = (n) & (1);
    #line 288
    b = ((n) & (~(1))) ^ (1);
    #line 289
    b = (p) && (pi);
}

#line 294
void test_bool(void) {
    #line 295
    bool b = false;
    #line 296
    b = true;
    #line 297
    int i = 0;
    #line 298
    i = IS_DEBUG;
}

#line 301
int test_ctrl(void) {
    #line 302
    switch (1) {
        case 0: {
            #line 304
            return 0;
            break;
        }default: {
            #line 306
            return 1;
            break;
        }
    }
}

#line 315
void f4(const char(*x)) {
}

#line 322
void f5(const int(*p)) {
}

#line 325
void test_convert(void) {
    #line 326
    const int(*a) = 0;
    #line 327
    int(*b) = 0;
    #line 328
    a = b;
    #line 329
    void(*p) = 0;
    #line 330
    (f5)(p);
}

#line 333
void test_const(void) {
    #line 334
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 337
    i = 1;
    #line 340
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 345
    const char ((*p)) = (const char *)(0);
    #line 346
    p = (escape_to_char) + (1);
    #line 347
    char (*q) = (char *)(escape_to_char);
    #line 348
    c = q['n'];
    p = (const char *)(1);
    #line 353
    i = (int)((ullong)(p));
}

#line 356
void test_init(void) {
    #line 357
    int x = (const int)(0);
    #line 358
    int y;
    #line 359
    y = 0;
    #line 360
    int z = 42;
    #line 361
    int (a[3]) = {1, 2, 3};
    #line 364
    for (ullong i = 0; (i) < (10); i++) {
        #line 365
        (printf)("%llu\n", i);
    }
}

#line 369
void test_sizeof(void) {
    #line 370
    int i = 0;
    #line 371
    ullong n = sizeof(i);
    #line 372
    n = sizeof(int);
    #line 373
    n = sizeof(int);
    #line 374
    n = sizeof(int *);
}

#line 377
void test_cast(void) {
    #line 378
    int(*p) = 0;
    #line 379
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 386
void test_loops(void) {
    #line 387
    while (0) {
    }
    #line 389
    for (int i = 0; (i) < (10); i++) {
    }
    #line 391
    for (;;) {
    }
    #line 393
    for (int i = 0;;) {
    }
    #line 395
    for (; 0;) {
    }
    #line 397
    for (int i = 0;; i++) {
    }
    #line 399
    int i = 0;
    #line 400
    for (;; i++) {
    }
}

#line 404
int main(int argc, const char *(*argv)) {
    #line 405
    if ((argv) == (0)) {
        #line 406
        (printf)("argv is null\n");
    }
    #line 408
    (test_sizeof)();
    #line 409
    (test_assign)();
    #line 410
    (test_enum)();
    #line 411
    (test_arrays)();
    #line 412
    (test_cast)();
    #line 413
    (test_init)();
    #line 414
    (test_lits)();
    #line 415
    (test_const)();
    #line 416
    (test_bool)();
    #line 417
    (test_ops)();
    #line 418
    int b = (example_test)();
    #line 419
    (puts)("Hello, world!");
    #line 420
    int c = (getchar)();
    #line 421
    (printf)("You wrote \'%c\'\n", c);
    #line 422
    (va_test)(1);
    #line 423
    (va_test)(1, 2);
    #line 424
    argv = NULL;
    #line 425
    return 0;
}
