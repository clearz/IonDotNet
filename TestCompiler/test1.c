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

#line 86
struct Vector {
    #line 87
    int x;
    #line 87
    int y;
};

#line 45
typedef IntOrPtr U;

#line 31
int g(U u);

#line 51
union IntOrPtr {
    #line 52
    int i;
    #line 53
    int (*p);
};

#line 35
void k(void (*vp), int (*ip));

#line 40
void f1(void);

#line 47
int example_test(void);

#line 102
int fact_rec(int n);

#line 94
int fact_iter(int n);

#line 56
char const (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 66
int (array[11]) = {1, 2, 3, [10] = 4};

#line 68
int is_even(int digit);

#line 84
int i;

#line 90
void f2(Vector v);

#line 112
T (*p);

#line 110
#define M ((1) + (sizeof(p)))

struct T {
    #line 115
    int (a[M]);
};

#line 118
void benchmark(int n);

#line 125
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 131
void test_lits(void);

#line 146
void test_ops(void);

#line 176
#define IS_DEBUG (true)

#line 178
void test_bool(void);

#line 185
int test_ctrl(void);

#line 195
int const (j);

#line 196
int const ((*q));

#line 197
Vector const (cv);

#line 199
void f4(char const ((*x)));

#line 202
struct ConstVector {
    #line 203
    int const (x);
    #line 203
    int const (y);
};

#line 206
void test_const(void);

#line 229
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

#line 68
int is_even(int digit) {
    #line 69
    int b = 0;
    #line 70
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 72
            b = 1;
            break;
        }
    }
    #line 74
    return b;
}

#line 90
void f2(Vector v) {
    #line 91
    v = (Vector){0};
}

#line 94
int fact_iter(int n) {
    #line 95
    int r = 1;
    #line 96
    for (int i = 0; (i) <= (n); i++) {
        #line 97
        r *= i;
    }
    #line 99
    return r;
}

#line 102
int fact_rec(int n) {
    #line 103
    if ((n) == (0)) {
        #line 104
        return 1;
    } else {
        #line 106
        return (n) * ((fact_rec)((n) - (1)));
    }
}

#line 118
void benchmark(int n) {
    #line 119
    int r = 1;
    #line 120
    for (int i = 1; (i) <= (n); i++) {
        #line 121
        r *= i;
    }
}

#line 125
int va_test(int x, ...) {
    #line 126
    return 0;
}

#line 131
void test_lits(void) {
    #line 132
    float f = 3.14f;
    #line 133
    double d = 3.14;
    #line 134
    int i = 1;
    #line 135
    uint u = 0xFFFFFFFFu;
    #line 136
    long l = 1l;
    #line 137
    ulong ul = 1ul;
    #line 138
    llong ll = 0x100000000ll;
    #line 139
    ullong ull = 0xFFFFFFFFFFFFFFFFull;
    #line 140
    uint x1 = 0xFFFFFFFF;
    #line 141
    llong x2 = 4294967295;
    #line 142
    ullong x3 = 0xFFFFFFFFFFFFFFFF;
    #line 143
    int x4 = (0xAA) + (0x55);
}

#line 146
void test_ops(void) {
    #line 147
    float pi = 3.14f;
    #line 148
    float f = 0.0f;
    #line 149
    f = +(pi);
    #line 150
    f = -(pi);
    #line 151
    int n = -(1);
    #line 152
    n = ~(n);
    #line 153
    f = ((f) * (pi)) + (n);
    #line 154
    f = (pi) / (pi);
    #line 155
    n = (3) % (2);
    #line 156
    n = (n) + ((uchar)(1));
    #line 157
    int (*p) = &(n);
    #line 158
    p = (p) + (1);
    #line 159
    n = (int)(((p) + (1)) - (p));
    #line 160
    n = (n) << (1);
    #line 161
    n = (n) >> (1);
    #line 162
    int b = ((p) + (1)) > (p);
    #line 163
    b = ((p) + (1)) >= (p);
    #line 164
    b = ((p) + (1)) < (p);
    #line 165
    b = ((p) + (1)) <= (p);
    #line 166
    b = ((p) + (1)) == (p);
    #line 167
    b = (1) > (2);
    #line 168
    b = (1.23f) <= (pi);
    #line 169
    n = 0xFF;
    #line 170
    b = (n) & (~(1));
    #line 171
    b = (n) & (1);
    #line 172
    b = ((n) & (~(1))) ^ (1);
    #line 173
    b = (p) && (pi);
}

#line 178
void test_bool(void) {
    #line 179
    bool b = false;
    #line 180
    b = true;
    #line 181
    int i = 0;
    #line 182
    i = IS_DEBUG;
}

#line 185
int test_ctrl(void) {
    #line 186
    while (1) {
        #line 187
        while (1) {
            #line 188
            break;
        }
        #line 190
        return 42;
    }
    #line 192
    return 0;
}

#line 199
void f4(char const ((*x))) {
}

#line 206
void test_const(void) {
    #line 207
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 210
    i = 1;
    #line 213
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 218
    char const ((*p)) = (char const *)(0);
    #line 219
    p = (escape_to_char) + (1);
    #line 220
    char (*q) = (char *)(escape_to_char);
    #line 221
    c = q['n'];
    p = (char const *)(1);
    #line 226
    i = (int)((ullong)(p));
}

#line 229
int main(int argc, char const ((*(*argv)))) {
    #line 230
    if ((argv) == (0)) {
        #line 231
        (printf)("argv is null\n");
    }
    #line 233
    (test_lits)();
    #line 234
    (test_const)();
    #line 235
    (test_bool)();
    #line 236
    (test_ops)();
    #line 237
    int b = (example_test)();
    #line 238
    (puts)("Hello, world!");
    #line 239
    int c = (getchar)();
    #line 240
    (printf)("You wrote \'%c\'\n", c);
    #line 241
    (va_test)(1);
    #line 242
    (va_test)(1, 2);
    #line 243
    argv = NULL;
    #line 244
    return 0;
}
