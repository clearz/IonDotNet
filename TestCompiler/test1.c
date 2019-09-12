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

// Forward declarations
typedef union IntOrPtr IntOrPtr;
typedef struct Vector Vector;
typedef struct T T;

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
typedef int (A[(1) + ((2) * (sizeof(h())))]);

#line 84
struct Vector {
    #line 85
    int x;
    #line 85
    int y;
};

#line 43
typedef IntOrPtr U;

#line 29
int g(U u);

#line 49
union IntOrPtr {
    #line 50
    int i;
    #line 51
    int (*p);
};

#line 33
void k(void (*vp), int (*ip));

#line 38
void f1(void);

#line 45
int example_test(void);

#line 100
int fact_rec(int n);

#line 92
int fact_iter(int n);

#line 54
int (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 64
int (array[11]) = {1, 2, 3, [10] = 4};

#line 66
int is_even(int digit);

#line 82
int i;

#line 88
void f2(Vector v);

#line 110
T (*p);

#line 108
#define M ((1) + (sizeof(p)))

struct T {
    #line 113
    int (a[M]);
};

#line 116
void benchmark(int n);

#line 123
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 129
void test_ops(void);

#line 159
int test_ctrl(void);

#line 170
#define IS_DEBUG (true)

#line 172
void test_bool(void);

#line 179
int main(int argc, char (*(*argv)));

// Function declarations
#line 21
uchar h(void) {
    #line 22
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 23
    int (*p) = &((int){0});
    #line 24
    ulong x = ((uint){1}) + ((long){2});
    #line 25
    int y = +(c);
    #line 26
    return x;
}

#line 29
int g(U u) {
    #line 30
    return u.i;
}

#line 33
void k(void (*vp), int (*ip)) {
    #line 34
    vp = ip;
    #line 35
    ip = vp;
}

#line 38
void f1(void) {
    #line 39
    int (*p) = &((int){0});
    #line 40
    *(p) = 42;
}

#line 45
int example_test(void) {
    #line 46
    return (fact_rec(10)) == (fact_iter(10));
}

#line 66
int is_even(int digit) {
    #line 67
    int b = 0;
    #line 68
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 70
            b = 1;
            break;
        }
    }
    #line 72
    return b;
}

#line 88
void f2(Vector v) {
    #line 89
    v = (Vector){0};
}

#line 92
int fact_iter(int n) {
    #line 93
    int r = 1;
    #line 94
    for (int i = 0; (i) <= (n); i++) {
        #line 95
        r *= i;
    }
    #line 97
    return r;
}

#line 100
int fact_rec(int n) {
    #line 101
    if ((n) == (0)) {
        #line 102
        return 1;
    } else {
        #line 104
        return (n) * (fact_rec((n) - (1)));
    }
}

#line 116
void benchmark(int n) {
    #line 117
    int r = 1;
    #line 118
    for (int i = 1; (i) <= (n); i++) {
        #line 119
        r *= i;
    }
}

#line 123
int va_test(int x, ...) {
    #line 124
    return 0;
}

#line 129
void test_ops(void) {
    #line 130
    float pi = 3.14;
    #line 131
    float f = 0.0;
    #line 132
    f = +(pi);
    #line 133
    f = -(pi);
    #line 134
    int n = -(1);
    #line 135
    n = ~(n);
    #line 136
    f = ((f) * (pi)) + (n);
    #line 137
    f = (pi) / (pi);
    #line 138
    n = (3) % (2);
    #line 139
    n = (n) + ((uchar)(1));
    #line 140
    int (*p) = &(n);
    #line 141
    p = (p) + (1);
    #line 142
    n = ((p) + (1)) - (p);
    #line 143
    n = (n) << (1);
    #line 144
    n = (n) >> (1);
    #line 145
    int b = ((p) + (1)) > (p);
    #line 146
    b = ((p) + (1)) >= (p);
    #line 147
    b = ((p) + (1)) < (p);
    #line 148
    b = ((p) + (1)) <= (p);
    #line 149
    b = ((p) + (1)) == (p);
    #line 150
    b = (1) > (2);
    #line 151
    b = (1.23) <= (pi);
    #line 152
    n = 0xff;
    #line 153
    b = (n) & (~(1));
    #line 154
    b = (n) & (1);
    #line 155
    b = ((n) & (~(1))) ^ (1);
    #line 156
    b = (p) && (pi);
}

#line 159
int test_ctrl(void) {
    #line 160
    while ((3) < (6)) {
        #line 161
        while (1) {
            #line 162
            break;
        }
        #line 164
        return 42;
    }
    #line 166
    return 0;
}

void test_bool(void) {
    #line 173
    bool b = false;
    #line 174
    b = true;
    #line 175
    int i = 0;
    #line 176
    i = IS_DEBUG;
}

#line 179
int main(int argc, char (*(*argv))) {
    #line 180
    test_bool();
    #line 181
    test_ops();
    #line 182
    int b = example_test();
    #line 183
    puts("Hello, world!");
    #line 184
    int c = getchar();
    #line 185
    printf("You wrote \'%c\'\n", c);
    #line 186
    va_test(1);
    #line 187
    va_test(1, 2);
    #line 188
    return 0;
}
