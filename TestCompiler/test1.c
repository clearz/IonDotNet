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

uchar h(void);

#line 19
typedef int (A[(1) + ((2) * (sizeof((h)())))]);

#line 87
struct Vector {
    #line 88
    int x;
    #line 88
    int y;
};

#line 45
typedef IntOrPtr U;

#line 51
union IntOrPtr {
    #line 52
    int i;
    #line 53
    int (*p);
};

#line 31
int g(U u);

void k(void (*vp), int (*ip));

#line 40
void f1(void);

#line 47
int example_test(void);

#line 103
int fact_rec(int n);

#line 95
int fact_iter(int n);

#line 56
char const (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 66
int (a2[11]) = {1, 2, 3, [10] = 4};

#line 69
int is_even(int digit);

#line 85
int i;

#line 91
void f2(Vector v);

#line 113
T (*p);

#line 111
#define M ((1) + (sizeof(p)))

struct T {
    #line 116
    int (a[M]);
};

#line 119
void benchmark(int n);

#line 126
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 132
void test_lits(void);

#line 147
void test_ops(void);

#line 177
#define IS_DEBUG (true)

#line 179
void test_bool(void);

#line 186
int test_ctrl(void);

#line 196
int const (j);

#line 197
int const ((*q));

#line 198
Vector const (cv);

#line 200
void f4(char const ((*x)));

#line 203
struct ConstVector {
    #line 204
    int const (x);
    #line 204
    int const (y);
};

#line 207
void test_const(void);

#line 230
void test_init(void);

#line 241
int main(int argc, char const ((*(*argv))));

// Function declarations
#line 21
uchar h(void) {
    #line 22
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 23
    Vector (*v) = &((Vector){1, 2});
    #line 24
    v->x = 42;
    #line 25
    int (*p) = &((int){0});
    #line 26
    ulong x = ((uint){1}) + ((long){2});
    #line 27
    int y = +(c);
    #line 28
    return (uchar)(x);
}

#line 31
int g(U u) {
    #line 32
    return u.i;
}

#line 35
void k(void (*vp), int (*ip)) {
    #line 36
    vp = ip;
    #line 37
    ip = vp;
}

#line 40
void f1(void) {
    #line 41
    int (*p) = &((int){0});
    #line 42
    *(p) = 42;
}

#line 47
int example_test(void) {
    #line 48
    return ((fact_rec)(10)) == ((fact_iter)(10));
}

#line 69
int is_even(int digit) {
    #line 70
    int b = 0;
    #line 71
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 73
            b = 1;
            break;
        }
    }
    #line 75
    return b;
}

#line 91
void f2(Vector v) {
    #line 92
    v = (Vector){0};
}

#line 95
int fact_iter(int n) {
    #line 96
    int r = 1;
    #line 97
    for (int i = 0; (i) <= (n); i++) {
        #line 98
        r *= i;
    }
    #line 100
    return r;
}

#line 103
int fact_rec(int n) {
    #line 104
    if ((n) == (0)) {
        #line 105
        return 1;
    } else {
        #line 107
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 119
void benchmark(int n) {
    #line 120
    int r = 1;
    #line 121
    for (int i = 1; (i) <= (n); i++) {
        #line 122
        r *= i;
    }
}

#line 126
int va_test(int x, ...) {
    #line 127
    return 0;
}

#line 132
void test_lits(void) {
    #line 133
    float f = 3.14f;
    #line 134
    double d = 3.14;
    #line 135
    int i = 1;
    #line 136
    uint u = 0xFFFFFFFFu;
    #line 137
    long l = 1l;
    #line 138
    ulong ul = 1ul;
    #line 139
    llong ll = 0x100000000ll;
    #line 140
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 141
    uint x1 = 0xFFFFFFFF;
    #line 142
    llong x2 = 4294967295;
    #line 143
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 144
    int x4 = (0xAA) + (0x55);
}

#line 147
void test_ops(void) {
    #line 148
    float pi = 3.14f;
    #line 149
    float f = 0.0f;
    #line 150
    f = +(pi);
    #line 151
    f = -(pi);
    #line 152
    int n = -(1);
    #line 153
    n = ~(n);
    #line 154
    f = ((f) * (pi)) + (n);
    #line 155
    f = (pi) / (pi);
    #line 156
    n = (3) % (2);
    #line 157
    n = (n) + ((uchar)(1));
    #line 158
    int (*p) = &(n);
    #line 159
    p = (p) + (1);
    #line 160
    n = (int)(((p) + (1)) - (p));
    #line 161
    n = (n) << (1);
    #line 162
    n = (n) >> (1);
    #line 163
    int b = ((p) + (1)) > (p);
    #line 164
    b = ((p) + (1)) >= (p);
    #line 165
    b = ((p) + (1)) < (p);
    #line 166
    b = ((p) + (1)) <= (p);
    #line 167
    b = ((p) + (1)) == (p);
    #line 168
    b = (1) > (2);
    #line 169
    b = (1.23f) <= (pi);
    #line 170
    n = 0xFF;
    #line 171
    b = (n) & (~(1));
    #line 172
    b = (n) & (1);
    #line 173
    b = ((n) & (~(1))) ^ (1);
    #line 174
    b = (p) && (pi);
}

#line 179
void test_bool(void) {
    #line 180
    bool b = false;
    #line 181
    b = true;
    #line 182
    int i = 0;
    #line 183
    i = IS_DEBUG;
}

#line 186
int test_ctrl(void) {
    #line 187
    while (1) {
        #line 188
        while (1) {
            #line 189
            break;
        }
        #line 191
        return 42;
    }
    #line 193
    return 0;
}

#line 200
void f4(char const ((*x))) {
}

#line 207
void test_const(void) {
    #line 208
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 211
    i = 1;
    #line 214
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 219
    char const ((*p)) = (char const *)(0);
    #line 220
    p = (escape_to_char) + (1);
    #line 221
    char (*q) = (char *)(escape_to_char);
    #line 222
    c = q['n'];
    p = (char const *)(1);
    #line 227
    i = (int)((ullong)(p));
}

#line 230
void test_init(void) {
    #line 231
    int x = (int const)(0);
    #line 232
    int y;
    #line 233
    int z = 42;
    #line 234
    int (a[3]) = {1, 2, 3};
}

#line 241
int main(int argc, char const ((*(*argv)))) {
    #line 242
    if ((argv) == (0)) {
        #line 243
        (printf)("argv is null\n");
    }
    #line 245
    (test_init)();
    #line 246
    (test_lits)();
    #line 247
    (test_const)();
    #line 248
    (test_bool)();
    #line 249
    (test_ops)();
    #line 250
    int b = (example_test)();
    #line 251
    (puts)("Hello, world!");
    #line 252
    int c = (getchar)();
    #line 253
    (printf)("You wrote \'%c\'\n", c);
    #line 254
    (va_test)(1);
    #line 255
    (va_test)(1, 2);
    #line 256
    argv = NULL;
    #line 257
    return 0;
}
