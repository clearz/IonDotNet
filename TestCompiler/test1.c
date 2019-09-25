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

// Sorted declarations
#line 1 "test1.ion"
void test_sizeof(void);

// Function declarations
#line 1
void test_sizeof(void) {
    #line 2
    ullong n = sizeof(1);
    #line 3
    n = sizeof(int);
    #line 4
    n = sizeof(int);
}
