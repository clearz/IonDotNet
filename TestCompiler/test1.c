// Preamble
#include <stdio.h>
#include <math.h>

	typedef unsigned char uchar;
typedef signed char schar;
typedef unsigned short ushort;
typedef unsigned int uint;
typedef unsigned long ulong;
typedef long long llong;
typedef unsigned long long ullong;

// Forward declarations
typedef struct vec2 vec2;

// Sorted declarations
#line 10 "test2.ion"
struct vec2 {
#line 11
	float x;
#line 11
	float y;
};

#line 14
vec2 add2(vec2 a, vec2 b);

vec2 sub2(vec2 a, vec2 b);

vec2 neg2(vec2 a);

vec2 mul2(float a, vec2 b);

vec2 addmul2(vec2 a, float b, vec2 c);

float dot2(vec2 a, vec2 b);

float len2(vec2 a);

vec2 unit2(vec2 a);

vec2 perp2(vec2 a);

vec2 dir2(vec2 a, vec2 b);

vec2 rot2(float a, vec2 b);

#line 60
int main(int argc, char(*(*argv)));

// Function declarations
#line 14
vec2 add2(vec2 a, vec2 b) {
#line 15
	return (vec2) { (a.x) + (b.x), (a.y) + (b.y) };
}

#line 18
vec2 sub2(vec2 a, vec2 b) {
#line 19
	return (vec2) { (a.x) - (b.x), (a.y) - (b.y) };
}

#line 22
vec2 neg2(vec2 a) {
#line 23
	return (vec2) { -(a.x), -(a.y) };
}

#line 26
vec2 mul2(float a, vec2 b) {
#line 27
	return (vec2) { (a)* (b.x), (a)* (b.y) };
}

#line 30
vec2 addmul2(vec2 a, float b, vec2 c) {
#line 31
	return add2(a, mul2(b, c));
}

#line 34
float dot2(vec2 a, vec2 b) {
#line 35
	return ((a.x) * (b.x)) + ((a.y) * (b.y));
}

#line 38
float len2(vec2 a) {
#line 39
	return sqrtf(dot2(a, a));
}

#line 42
vec2 unit2(vec2 a) {
#line 43
	return mul2((1) / (len2(a)), a);
}

#line 46
vec2 perp2(vec2 a) {
#line 47
	return (vec2) { -(a.y), a.x };
}

#line 50
vec2 dir2(vec2 a, vec2 b) {
#line 51
	return unit2(sub2(b, a));
}

#line 54
vec2 rot2(float a, vec2 b) {
#line 55
	float c = cosf(a);
#line 56
	float s = sinf(a);
#line 57
	return (vec2) { ((c) * (b.x)) - ((s) * (b.y)), ((s) * (b.x)) + ((c) * (b.y)) };
}

#line 60
int main(int argc, char(*(*argv))) {
#line 61
	return 0;
}