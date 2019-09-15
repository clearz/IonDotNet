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
typedef struct ConstVector ConstVector;

// Sorted declarations
#line 1 "test1.ion"
char const (escape_to_char[256]) = {['n'] = '\n', ['r'] = '\r', ['t'] = '\t', ['v'] = '\v', ['b'] = '\b', ['a'] = '\a', ['0'] = 0};

#line 11
struct ConstVector {
    #line 12
    int const (x);
    #line 12
    int const (y);
};

#line 15
int const (j);

#line 16
int const ((*q));

#line 17
ConstVector const (cv);

#line 19
void f4(char const ((*x)));

#line 22
void test_const(void);

// Function declarations
#line 19
void f4(char const ((*x))) {
}

#line 22
void test_const(void) {
    #line 23
    ConstVector cv2 = {1, 2};
    int i = 0;
    #line 26
    i = 1;
    #line 29
    int x = cv.x;
    char c = escape_to_char[0];
    (f4)(escape_to_char);
    #line 34
    char const ((*p)) = (char const *)(0);
    #line 35
    p = (escape_to_char) + (1);
    #line 36
    char (*q) = (char *)(escape_to_char);
    #line 37
    c = q['n'];
    p = (char const *)(1);
    #line 42
    i = (int)((ullong)(p));
}
