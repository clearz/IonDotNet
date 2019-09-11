// Preamble
#include <stdio.h>

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
#line 5 "test1.ion"
char c = 1;

#line 6
uchar uc = 1;

#line 7
schar sc = 1;

#line 9
enum { N = (((char)(42)) + (8)) != (0) };

uchar h(void);

#line 11
typedef int (A[(1) + ((2) * (sizeof(h())))]);

#line 76
struct Vector {
    #line 77
    int x;
    #line 77
    int y;
};

#line 35
typedef IntOrPtr U;

#line 21
int g(U u);

#line 41
union IntOrPtr {
    #line 42
    int i;
    #line 43
    int (*p);
};

#line 25
void k(void (*vp), int (*ip));

#line 30
void f1(void);

#line 37
int example_test(void);

#line 92
int fact_rec(int n);

#line 84
int fact_iter(int n);

#line 46
int (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 56
int (array[11]) = {1, 2, 3, [10] = 4};

#line 58
int is_even(int digit);

#line 74
int i;

#line 80
void f2(Vector v);

#line 102
T (*p);

#line 100
enum { n = (1) + (sizeof(p)) };

struct T {
    #line 105
    int (a[n]);
};

#line 108
void benchmark(int n);

#line 115
int va_test(int x, ...);

typedef int (*F)(int, ...);

#line 121
void test_ops(void);

#line 151
int main(int argc, char (*(*argv)));

// Function declarations
#line 13
uchar h(void) {
    #line 14
    (Vector){.x = 1, .y = 2}.x = 42;
    #line 15
    int (*p) = &((int){0});
    #line 16
    ulong x = ((uint){1}) + ((long){2});
    #line 17
    int y = +(c);
    #line 18
    return x;
}

#line 21
int g(U u) {
    #line 22
    return u.i;
}

#line 25
void k(void (*vp), int (*ip)) {
    #line 26
    vp = ip;
    #line 27
    ip = vp;
}

#line 30
void f1(void) {
    #line 31
    int (*p) = &((int){0});
    #line 32
    *(p) = 42;
}

#line 37
int example_test(void) {
    #line 38
    return (fact_rec(10)) == (fact_iter(10));
}

#line 58
int is_even(int digit) {
    #line 59
    int b = 0;
    #line 60
    switch (digit) {
        case 0:
        case 2:
        case 4:
        case 6:
        case 8: {
            #line 62
            b = 1;
            break;
        }
    }
    #line 64
    return b;
}

#line 80
void f2(Vector v) {
    #line 81
    v = (Vector){0};
}

#line 84
int fact_iter(int n) {
    #line 85
    int r = 1;
    #line 86
    for (int i = 0; (i) <= (n); i++) {
        #line 87
        r *= i;
    }
    #line 89
    return r;
}

#line 92
int fact_rec(int n) {
    #line 93
    if ((n) == (0)) {
        #line 94
        return 1;
    } else {
        #line 96
        return (n) * (fact_rec((n) - (1)));
    }
}

#line 108
void benchmark(int n) {
    #line 109
    int r = 1;
    #line 110
    for (int i = 1; (i) <= (n); i++) {
        #line 111
        r *= i;
    }
}

#line 115
int va_test(int x, ...) {
    #line 116
    return 0;
}

#line 121
void test_ops(void) {
    #line 122
    float pi = 3.14;
    #line 123
    float f = 0.0;
    #line 124
    f = +(pi);
    #line 125
    f = -(pi);
    #line 126
    int n = -(1);
    #line 127
    n = ~(n);
    #line 128
    f = (f) * (pi);
    #line 129
    f = (pi) / (pi);
    #line 130
    n = (3) % (2);
    #line 131
    n = (n) + ((uchar)(1));
    #line 132
    int (*p) = &(n);
    #line 133
    p = (p) + (1);
    #line 134
    n = ((p) + (1)) - (p);
    #line 135
    n = (n) << (1);
    #line 136
    n = (n) >> (1);
    #line 137
    int b = ((p) + (1)) > (p);
    #line 138
    b = ((p) + (1)) >= (p);
    #line 139
    b = ((p) + (1)) < (p);
    #line 140
    b = ((p) + (1)) <= (p);
    #line 141
    b = ((p) + (1)) == (p);
    #line 142
    b = (1) > (2);
    #line 143
    b = (1.23) <= (pi);
    #line 144
    n = 0xff;
    #line 145
    b = (n) & (~(1));
    #line 146
    b = (n) & (1);
    #line 147
    b = ((n) & (~(1))) ^ (1);
    #line 148
    b = (p) && (pi);
}

#line 151
int main(int argc, char (*(*argv))) {
    ullong sc = sizeof(c);
    #line 154
    ullong suc = sizeof(uc);
    #line 155
    ullong ssc = sizeof(sc);
    #line 156
    test_ops();
    #line 157
    int b = example_test();
    #line 158
    puts("Hello, world!");
    #line 159
    int c = getchar();
    #line 160
    printf("You wrote \'%c\'\n", c);
    #line 161
    va_test(1);
    #line 162
    va_test(1, 2);
    #line 163
    return 0;
}
