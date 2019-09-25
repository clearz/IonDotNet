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
#line 185 "test1.ion"
typedef enum Color {
    COLOR_NONE,
    COLOR_RED,
    COLOR_GREEN,
    COLOR_BLUE,
    NUM_COLORS,
}Color;

#line 12
#define PI (3.14f)

#line 13
#define PI2 ((PI) + (PI))

#line 15
char c = 1;

#line 16
uchar uc = 1;

#line 17
schar sc = 1;

#line 19
#define N ((((char)(42)) + (8)) != (0))

#line 84
uchar h(void);

#line 21
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 23
char (*code) = 
    "\n"
    "#include <stdio.h>\n"
    "\n"
    "int main(int argc, char **argv) {\n"
    "    printf(\"Hello, world!\\n\");\n"
    "    return 0;\n"
    "}\n";

#line 32
struct S1 {
    #line 33
    int a;
    #line 34
    const int (b);
};

#line 37
struct S2 {
    #line 38
    S1 s1;
};

#line 41
void f10(int (a[3]));

void test_arrays(void);

#line 50
void test_nonmodifiable(void);

#line 62
struct UartCtrl {
    #line 63
    bool tx_enable;
    #line 63
    bool rx_enable;
};

#line 66
#define UART_CTRL_REG ((uint *)(0x12345678))

#line 68
uint32 pack(UartCtrl ctrl);

UartCtrl unpack(uint32 word);

void test_uart(void);

#line 153
struct Vector {
    #line 154
    int x;
    #line 154
    int y;
};

#line 111
typedef IntOrPtr U;

#line 117
union IntOrPtr {
    #line 118
    int i;
    #line 119
    int(*p);
};

#line 94
int g(U u);

void k(void(*vp), int(*ip));

#line 103
void f1(void);

#line 108
void f3(int (a[]));

#line 113
int example_test(void);

#line 169
int fact_rec(int n);

#line 161
int fact_iter(int n);

#line 122
const char (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 132
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 135
int is_even(int digit);

#line 151
int i;

#line 157
void f2(Vector v);

#line 179
T(*p);

#line 177
#define M ((1) + (sizeof(p)))

struct T {
    #line 182
    int (a[M]);
};

#line 193
const char * (color_names[NUM_COLORS]) = {[COLOR_NONE] = "none", [COLOR_RED] = "red", [COLOR_GREEN] = "green", [COLOR_BLUE] = "blue"};

#line 200
void test_enum(void);

#line 209
void test_assign(void);

#line 232
void benchmark(int n);

#line 239
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 245
void test_lits(void);

#line 260
void test_ops(void);

#line 290
#define IS_DEBUG (true)

#line 292
void test_bool(void);

#line 299
int test_ctrl(void);

#line 309
const int (j);

#line 310
const int(*q);

#line 311
const Vector (cv);

#line 313
void f4(const char(*x));

#line 316
struct ConstVector {
    #line 317
    const int (x);
    #line 317
    const int (y);
};

#line 320
void f5(const int(*p));

#line 323
void test_convert(void);

#line 331
void test_const(void);

#line 354
void test_init(void);

#line 367
void test_sizeof(void);

#line 375
void test_cast(void);

#line 384
int main(int argc, const char *(*argv));

// Function declarations
#line 41
void f10(int (a[3])) {
    #line 42
    a[1] = 42;
}

#line 45
void test_arrays(void) {
    #line 46
    int (a[3]) = {1, 2, 3};
    #line 47
    (f10)(a);
}

#line 50
void test_nonmodifiable(void) {
    #line 51
    S1 s1;
    #line 52
    s1.a = 0;
    #line 55
    S2 s2;
    #line 56
    s2.s1.a = 0;
}

#line 68
uint32 pack(UartCtrl ctrl) {
    #line 69
    return ((ctrl.tx_enable) & (1u)) | (((ctrl.rx_enable) & (1u)) << (1));
}

#line 72
UartCtrl unpack(uint32 word) {
    #line 73
    return (UartCtrl){.tx_enable = (word) & (0x1), .rx_enable = ((word) & (0x2)) >> (1)};
}

#line 76
void test_uart(void) {
    #line 77
    bool tx_enable = (unpack)(*(UART_CTRL_REG)).tx_enable;
    #line 78
    *(UART_CTRL_REG) = (pack)((UartCtrl){.tx_enable = !(tx_enable), .rx_enable = false});
    #line 79
    UartCtrl ctrl = (unpack)(*(UART_CTRL_REG));
    #line 80
    ctrl.rx_enable = true;
    #line 81
    *(UART_CTRL_REG) = (pack)(ctrl);
}

#line 84
uchar h(void) {
    #line 85
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 86
    Vector (*v) = &((Vector){1, 2});
    #line 87
    v->x = 42;
    #line 88
    int (*p) = &((int){0});
    #line 89
    ulong x = ((uint){1}) + ((long){2});
    #line 90
    int y = +(c);
    #line 91
    return (uchar)(x);
}

#line 94
int g(U u) {
    #line 95
    return u.i;
}

#line 98
void k(void(*vp), int(*ip)) {
    #line 99
    vp = ip;
    #line 100
    ip = vp;
}

#line 103
void f1(void) {
    #line 104
    int (*p) = &((int){0});
    #line 105
    *(p) = 42;
}

#line 108
void f3(int (a[])) {
}

#line 113
int example_test(void) {
    #line 114
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 135
int is_even(int digit) {
    #line 136
    int b = 0;
    #line 137
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 139
            b = 1;
            break;
        }
    }
    #line 141
    return b;
}

#line 157
void f2(Vector v) {
    #line 158
    v = (Vector){0};
}

#line 161
int fact_iter(int n) {
    #line 162
    int r = 1;
    #line 163
    for (int i = 0; (i) <= (n); i++) {
        #line 164
        r *= i;
    }
    #line 166
    return r;
}

#line 169
int fact_rec(int n) {
    #line 170
    if ((n) == (0)) {
        #line 171
        return 1;
    } else {
        #line 173
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 200
void test_enum(void) {
    #line 201
    Color a = COLOR_RED;
    #line 202
    Color b = COLOR_RED;
    #line 203
    int c = (a) + (b);
    #line 204
    int i = a;
    #line 205
    a = i;
    #line 206
    (printf)("%d %d %d %d\n", COLOR_NONE, COLOR_RED, COLOR_GREEN, COLOR_BLUE);
}

#line 209
void test_assign(void) {
    #line 210
    int i = 0;
    #line 211
    float f = 3.14f;
    #line 212
    int(*p) = &(i);
    #line 213
    i++;
    #line 214
    i--;
    #line 215
    p++;
    #line 216
    p--;
    #line 217
    p += 1;
    #line 218
    i /= 2;
    #line 219
    i *= 123;
    #line 220
    i %= 3;
    #line 221
    i <<= 1;
    #line 222
    i >>= 2;
    #line 223
    i &= 0xFF;
    #line 224
    i |= 0xFF00;
    #line 225
    i ^= 0xFF0;
}

#line 232
void benchmark(int n) {
    #line 233
    int r = 1;
    #line 234
    for (int i = 1; (i) <= (n); i++) {
        #line 235
        r *= i;
    }
}

#line 239
int va_test(int x, ...) {
    #line 240
    return 0;
}

#line 245
void test_lits(void) {
    #line 246
    float f = 3.14f;
    #line 247
    double d = 3.14;
    #line 248
    int i = 1;
    #line 249
    uint u = 0xFFFFFFFFu;
    #line 250
    long l = 1l;
    #line 251
    ulong ul = 1ul;
    #line 252
    llong ll = 0x100000000ll;
    #line 253
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 254
    uint x1 = 0xFFFFFFFF;
    #line 255
    llong x2 = 4294967295;
    #line 256
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 257
    int x4 = (0xAA) + (0x55);
}

#line 260
void test_ops(void) {
    #line 261
    float pi = 3.14f;
    #line 262
    float f = 0.0f;
    #line 263
    f = +(pi);
    #line 264
    f = -(pi);
    #line 265
    int n = -(1);
    #line 266
    n = ~(n);
    #line 267
    f = ((f) * (pi)) + (n);
    #line 268
    f = (pi) / (pi);
    #line 269
    n = (3) % (2);
    #line 270
    n = (n) + ((uchar)(1));
    #line 271
    int (*p) = &(n);
    #line 272
    p = (p) + (1);
    #line 273
    n = (int)(((p) + (1)) - (p));
    #line 274
    n = (n) << (1);
    #line 275
    n = (n) >> (1);
    #line 276
    int b = ((p) + (1)) > (p);
    #line 277
    b = ((p) + (1)) >= (p);
    #line 278
    b = ((p) + (1)) < (p);
    #line 279
    b = ((p) + (1)) <= (p);
    #line 280
    b = ((p) + (1)) == (p);
    #line 281
    b = (1) > (2);
    #line 282
    b = (1.23f) <= (pi);
    #line 283
    n = 0xFF;
    #line 284
    b = (n) & (~(1));
    #line 285
    b = (n) & (1);
    #line 286
    b = ((n) & (~(1))) ^ (1);
    #line 287
    b = (p) && (pi);
}

#line 292
void test_bool(void) {
    #line 293
    bool b = false;
    #line 294
    b = true;
    #line 295
    int i = 0;
    #line 296
    i = IS_DEBUG;
}

#line 299
int test_ctrl(void) {
    #line 300
    switch (1) {
        case 0: {
            #line 302
            return 0;
            break;
        }default: {
            #line 304
            return 1;
            break;
        }
    }
}

#line 313
void f4(const char(*x)) {
}

#line 320
void f5(const int(*p)) {
}

#line 323
void test_convert(void) {
    #line 324
    const int(*a) = 0;
    #line 325
    int(*b) = 0;
    #line 326
    a = b;
    #line 327
    void(*p) = 0;
    #line 328
    (f5)(p);
}

#line 331
void test_const(void) {
    #line 332
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 335
    i = 1;
    #line 338
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 343
    const char ((*p)) = (const char *)(0);
    #line 344
    p = (escape_to_char) + (1);
    #line 345
    char (*q) = (char *)(escape_to_char);
    #line 346
    c = q['n'];
    p = (const char *)(1);
    #line 351
    i = (int)((ullong)(p));
}

#line 354
void test_init(void) {
    #line 355
    int x = (const int)(0);
    #line 356
    int y;
    #line 357
    y = 0;
    #line 358
    int z = 42;
    #line 359
    int (a[3]) = {1, 2, 3};
    #line 362
    for (ullong i = 0; (i) < (10); i++) {
        #line 363
        (printf)("%llu\n", i);
    }
}

#line 367
void test_sizeof(void) {
    #line 368
    int i = 0;
    #line 369
    ullong n = sizeof(i);
    #line 370
    n = sizeof(int);
    #line 371
    n = sizeof(int);
    #line 372
    n = sizeof(int *);
}

#line 375
void test_cast(void) {
    #line 376
    int(*p) = 0;
    #line 377
    uint64 a = 0;
    a = (uint64)(p);
    p = (int *)(a);
}

#line 384
int main(int argc, const char *(*argv)) {
    #line 385
    if ((argv) == (0)) {
        #line 386
        (printf)("argv is null\n");
    }
    #line 388
    (test_sizeof)();
    #line 389
    (test_assign)();
    #line 390
    (test_enum)();
    #line 391
    (test_arrays)();
    #line 392
    (test_cast)();
    #line 393
    (test_init)();
    #line 394
    (test_lits)();
    #line 395
    (test_const)();
    #line 396
    (test_bool)();
    #line 397
    (test_ops)();
    #line 398
    int b = (example_test)();
    #line 399
    (puts)("Hello, world!");
    #line 400
    int c = (getchar)();
    #line 401
    (printf)("You wrote \'%c\'\n", c);
    #line 402
    (va_test)(1);
    #line 403
    (va_test)(1, 2);
    #line 404
    argv = NULL;
    #line 405
    return 0;
}
