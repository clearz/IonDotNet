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
#define PI (3.14)

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
void test_ops(void);

#line 161
#define IS_DEBUG (true)

#line 163
void test_bool(void);

#line 170
int test_ctrl(void);

#line 180
int const (j);

#line 181
int const ((*q));

#line 182
Vector const (cv);

#line 184
void f4(char const ((*x)));

#line 187
struct ConstVector {
    #line 188
    int const (x);
    #line 188
    int const (y);
};

#line 191
void test_const(void);

#line 214
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
void test_ops(void) {
    #line 132
    float pi = 3.14;
    #line 133
    float f = 0.0;
    #line 134
    f = +(pi);
    #line 135
    f = -(pi);
    #line 136
    int n = -(1);
    #line 137
    n = ~(n);
    #line 138
    f = ((f) * (pi)) + (n);
    #line 139
    f = (pi) / (pi);
    #line 140
    n = (3) % (2);
    #line 141
    n = (n) + ((uchar)(1));
    #line 142
    int (*p) = &(n);
    #line 143
    p = (p) + (1);
    #line 144
    n = (int)(((p) + (1)) - (p));
    #line 145
    n = (n) << (1);
    #line 146
    n = (n) >> (1);
    #line 147
    int b = ((p) + (1)) > (p);
    #line 148
    b = ((p) + (1)) >= (p);
    #line 149
    b = ((p) + (1)) < (p);
    #line 150
    b = ((p) + (1)) <= (p);
    #line 151
    b = ((p) + (1)) == (p);
    #line 152
    b = (1) > (2);
    #line 153
    b = (1.23) <= (pi);
    #line 154
    n = 0xff;
    #line 155
    b = (n) & (~(1));
    #line 156
    b = (n) & (1);
    #line 157
    b = ((n) & (~(1))) ^ (1);
    #line 158
    b = (p) && (pi);
}

#line 163
void test_bool(void) {
    #line 164
    bool b = false;
    #line 165
    b = true;
    #line 166
    int i = 0;
    #line 167
    i = IS_DEBUG;
}

#line 170
int test_ctrl(void) {
    #line 171
    while (1) {
        #line 172
        while (1) {
            #line 173
            break;
        }
        #line 175
        return 42;
    }
    #line 177
    return 0;
}

#line 184
void f4(char const ((*x))) {
}

#line 191
void test_const(void) {
    #line 192
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 195
    i = 1;
    #line 198
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 203
    char const ((*p)) = (char const *)(0);
    #line 204
    p = (escape_to_char) + (1);
    #line 205
    char (*q) = (char *)(escape_to_char);
    #line 206
    c = q['n'];
    p = (char const *)(1);
    #line 211
    i = (int)((ullong)(p));
}

#line 214
int main(int argc, char const ((*(*argv)))) {
    #line 215
    if ((argv) == (0)) {
        #line 216
        (printf)("argv is null\n");
    }
    #line 218
    (test_const)();
    #line 219
    (test_bool)();
    #line 220
    (test_ops)();
    #line 221
    int b = (example_test)();
    #line 222
    (puts)("Hello, world!");
    #line 223
    int c = (getchar)();
    #line 224
    (printf)("You wrote \'%c\'\n", c);
    #line 225
    (va_test)(1);
    #line 226
    (va_test)(1, 2);
    #line 227
    argv = NULL;
    #line 228
    return 0;
}
