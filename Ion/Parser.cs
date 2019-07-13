using System;

namespace Lang
{
    using static TokenKind;
    using static DeclKind;
    using static ExprKind;
    using static StmtKind;
    using static CompoundFieldKind;

    public unsafe partial class Ion
    {
        private static readonly string[] decls =
        {
            "var i: long = cast(long*, 42)",
            "var x: char[256] = {1, 2, 3, ['a'] = 4}",
            "struct Vector { x, y: float; }",
            "var v = Vector{x = 1.0, y = -1.0}",
            "var v: Vector = {1.0, -1.0}",
            "const n = sizeof(:long*[16])",
            "const n = sizeof(1+2)",
            "var x = b == 1 ? 1+2 : 3-4",
            "func fact(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }",
            "func fact(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }",
            "var foo = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0",
            "func f(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }",
            "enum Color { RED = 3, GREEN, BLUE = 0 }",
            "const pi = 3.14",
            "union IntOrFloat { i: long; f: float; }",
            "typedef Vectors = Vector[1+2]",
            "func f() { do { print(42); } while(1); }",
            "typedef T = (func(long):long)[16]",
            "func f() { enum E { A, B, C } return; }",
            "func f() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }"
        };

        private readonly string code1 =
            "var n: long = 31051\nvar i0: long = cast(long*, 42)\nvar x0: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector0 { x, y: float; }\nvar v0 = Vector{x = 1.0, y = -1.0}\nvar v0: Vector = {1.0, -1.0}\nconst n0 = sizeof(:long*[16])\nconst n0 = sizeof(1+2)\nvar x0 = b0 == 1 ? 1+2 : 3-4\nfunc fact0(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact0(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo0 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f0(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color0 { RED = 3, GREEN, BLUE = 0 }\nconst pi0 = 3.14\nunion IntOrFloat0 { i: long; f: float; }\ntypedef Vectors0 = Vector[1+2]\nfunc f0() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f0() { enum E0 { A, B, C } return; }\nfunc f0() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i1: long = cast(long*, 42)\nvar x1: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector1 { x, y: float; }\nvar v1 = Vector{x = 1.0, y = -1.0}\nvar v1: Vector = {1.0, -1.0}\nconst n1 = sizeof(:long*[16])\nconst n1 = sizeof(1+2)\nvar x1 = b1 == 1 ? 1+2 : 3-4\nfunc fact1(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact1(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo1 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f1(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color1 { RED = 3, GREEN, BLUE = 0 }\nconst pi1 = 3.14\nunion IntOrFloat1 { i: long; f: float; }\ntypedef Vectors1 = Vector[1+2]\nfunc f1() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f1() { enum E1 { A, B, C } return; }\nfunc f1() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i2: long = cast(long*, 42)\nvar x2: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector2 { x, y: float; }\nvar v2 = Vector{x = 1.0, y = -1.0}\nvar v2: Vector = {1.0, -1.0}\nconst n2 = sizeof(:long*[16])\nconst n2 = sizeof(1+2)\nvar x2 = b2 == 1 ? 1+2 : 3-4\nfunc fact2(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact2(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo2 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f2(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color2 { RED = 3, GREEN, BLUE = 0 }\nconst pi2 = 3.14\nunion IntOrFloat2 { i: long; f: float; }\ntypedef Vectors2 = Vector[1+2]\nfunc f2() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f2() { enum E2 { A, B, C } return; }\nfunc f2() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i3: long = cast(long*, 42)\nvar x3: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector3 { x, y: float; }\nvar v3 = Vector{x = 1.0, y = -1.0}\nvar v3: Vector = {1.0, -1.0}\nconst n3 = sizeof(:long*[16])\nconst n3 = sizeof(1+2)\nvar x3 = b3 == 1 ? 1+2 : 3-4\nfunc fact3(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact3(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo3 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f3(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color3 { RED = 3, GREEN, BLUE = 0 }\nconst pi3 = 3.14\nunion IntOrFloat3 { i: long; f: float; }\ntypedef Vectors3 = Vector[1+2]\nfunc f3() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f3() { enum E3 { A, B, C } return; }\nfunc f3() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i4: long = cast(long*, 42)\nvar x4: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector4 { x, y: float; }\nvar v4 = Vector{x = 1.0, y = -1.0}\nvar v4: Vector = {1.0, -1.0}\nconst n4 = sizeof(:long*[16])\nconst n4 = sizeof(1+2)\nvar x4 = b4 == 1 ? 1+2 : 3-4\nfunc fact4(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact4(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo4 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f4(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color4 { RED = 3, GREEN, BLUE = 0 }\nconst pi4 = 3.14\nunion IntOrFloat4 { i: long; f: float; }\ntypedef Vectors4 = Vector[1+2]\nfunc f4() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f4() { enum E4 { A, B, C } return; }\nfunc f4() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i5: long = cast(long*, 42)\nvar x5: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector5 { x, y: float; }\nvar v5 = Vector{x = 1.0, y = -1.0}\nvar v5: Vector = {1.0, -1.0}\nconst n5 = sizeof(:long*[16])\nconst n5 = sizeof(1+2)\nvar x5 = b5 == 1 ? 1+2 : 3-4\nfunc fact5(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact5(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo5 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f5(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color5 { RED = 3, GREEN, BLUE = 0 }\nconst pi5 = 3.14\nunion IntOrFloat5 { i: long; f: float; }\ntypedef Vectors5 = Vector[1+2]\nfunc f5() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f5() { enum E5 { A, B, C } return; }\nfunc f5() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i6: long = cast(long*, 42)\nvar x6: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector6 { x, y: float; }\nvar v6 = Vector{x = 1.0, y = -1.0}\nvar v6: Vector = {1.0, -1.0}\nconst n6 = sizeof(:long*[16])\nconst n6 = sizeof(1+2)\nvar x6 = b6 == 1 ? 1+2 : 3-4\nfunc fact6(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact6(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo6 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f6(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color6 { RED = 3, GREEN, BLUE = 0 }\nconst pi6 = 3.14\nunion IntOrFloat6 { i: long; f: float; }\ntypedef Vectors6 = Vector[1+2]\nfunc f6() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f6() { enum E6 { A, B, C } return; }\nfunc f6() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i7: long = cast(long*, 42)\nvar x7: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector7 { x, y: float; }\nvar v7 = Vector{x = 1.0, y = -1.0}\nvar v7: Vector = {1.0, -1.0}\nconst n7 = sizeof(:long*[16])\nconst n7 = sizeof(1+2)\nvar x7 = b7 == 1 ? 1+2 : 3-4\nfunc fact7(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact7(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo7 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f7(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color7 { RED = 3, GREEN, BLUE = 0 }\nconst pi7 = 3.14\nunion IntOrFloat7 { i: long; f: float; }\ntypedef Vectors7 = Vector[1+2]\nfunc f7() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f7() { enum E7 { A, B, C } return; }\nfunc f7() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i8: long = cast(long*, 42)\nvar x8: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector8 { x, y: float; }\nvar v8 = Vector{x = 1.0, y = -1.0}\nvar v8: Vector = {1.0, -1.0}\nconst n8 = sizeof(:long*[16])\nconst n8 = sizeof(1+2)\nvar x8 = b8 == 1 ? 1+2 : 3-4\nfunc fact8(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact8(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo8 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f8(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color8 { RED = 3, GREEN, BLUE = 0 }\nconst pi8 = 3.14\nunion IntOrFloat8 { i: long; f: float; }\ntypedef Vectors8 = Vector[1+2]\nfunc f8() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f8() { enum E8 { A, B, C } return; }\nfunc f8() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i9: long = cast(long*, 42)\nvar x9: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector9 { x, y: float; }\nvar v9 = Vector{x = 1.0, y = -1.0}\nvar v9: Vector = {1.0, -1.0}\nconst n9 = sizeof(:long*[16])\nconst n9 = sizeof(1+2)\nvar x9 = b9 == 1 ? 1+2 : 3-4\nfunc fact9(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact9(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo9 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f9(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color9 { RED = 3, GREEN, BLUE = 0 }\nconst pi9 = 3.14\nunion IntOrFloat9 { i: long; f: float; }\ntypedef Vectors9 = Vector[1+2]\nfunc f9() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f9() { enum E9 { A, B, C } return; }\nfunc f9() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i10: long = cast(long*, 42)\nvar x10: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector10 { x, y: float; }\nvar v10 = Vector{x = 1.0, y = -1.0}\nvar v10: Vector = {1.0, -1.0}\nconst n10 = sizeof(:long*[16])\nconst n10 = sizeof(1+2)\nvar x10 = b10 == 1 ? 1+2 : 3-4\nfunc fact10(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact10(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo10 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f10(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color10 { RED = 3, GREEN, BLUE = 0 }\nconst pi10 = 3.14\nunion IntOrFloat10 { i: long; f: float; }\ntypedef Vectors10 = Vector[1+2]\nfunc f10() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f10() { enum E10 { A, B, C } return; }\nfunc f10() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i11: long = cast(long*, 42)\nvar x11: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector11 { x, y: float; }\nvar v11 = Vector{x = 1.0, y = -1.0}\nvar v11: Vector = {1.0, -1.0}\nconst n11 = sizeof(:long*[16])\nconst n11 = sizeof(1+2)\nvar x11 = b11 == 1 ? 1+2 : 3-4\nfunc fact11(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact11(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo11 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f11(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color11 { RED = 3, GREEN, BLUE = 0 }\nconst pi11 = 3.14\nunion IntOrFloat11 { i: long; f: float; }\ntypedef Vectors11 = Vector[1+2]\nfunc f11() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f11() { enum E11 { A, B, C } return; }\nfunc f11() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i12: long = cast(long*, 42)\nvar x12: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector12 { x, y: float; }\nvar v12 = Vector{x = 1.0, y = -1.0}\nvar v12: Vector = {1.0, -1.0}\nconst n12 = sizeof(:long*[16])\nconst n12 = sizeof(1+2)\nvar x12 = b12 == 1 ? 1+2 : 3-4\nfunc fact12(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact12(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo12 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f12(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color12 { RED = 3, GREEN, BLUE = 0 }\nconst pi12 = 3.14\nunion IntOrFloat12 { i: long; f: float; }\ntypedef Vectors12 = Vector[1+2]\nfunc f12() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f12() { enum E12 { A, B, C } return; }\nfunc f12() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i13: long = cast(long*, 42)\nvar x13: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector13 { x, y: float; }\nvar v13 = Vector{x = 1.0, y = -1.0}\nvar v13: Vector = {1.0, -1.0}\nconst n13 = sizeof(:long*[16])\nconst n13 = sizeof(1+2)\nvar x13 = b13 == 1 ? 1+2 : 3-4\nfunc fact13(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact13(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo13 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f13(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color13 { RED = 3, GREEN, BLUE = 0 }\nconst pi13 = 3.14\nunion IntOrFloat13 { i: long; f: float; }\ntypedef Vectors13 = Vector[1+2]\nfunc f13() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f13() { enum E13 { A, B, C } return; }\nfunc f13() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i14: long = cast(long*, 42)\nvar x14: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector14 { x, y: float; }\nvar v14 = Vector{x = 1.0, y = -1.0}\nvar v14: Vector = {1.0, -1.0}\nconst n14 = sizeof(:long*[16])\nconst n14 = sizeof(1+2)\nvar x14 = b14 == 1 ? 1+2 : 3-4\nfunc fact14(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact14(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo14 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f14(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color14 { RED = 3, GREEN, BLUE = 0 }\nconst pi14 = 3.14\nunion IntOrFloat14 { i: long; f: float; }\ntypedef Vectors14 = Vector[1+2]\nfunc f14() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f14() { enum E14 { A, B, C } return; }\nfunc f14() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i15: long = cast(long*, 42)\nvar x15: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector15 { x, y: float; }\nvar v15 = Vector{x = 1.0, y = -1.0}\nvar v15: Vector = {1.0, -1.0}\nconst n15 = sizeof(:long*[16])\nconst n15 = sizeof(1+2)\nvar x15 = b15 == 1 ? 1+2 : 3-4\nfunc fact15(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact15(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo15 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f15(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color15 { RED = 3, GREEN, BLUE = 0 }\nconst pi15 = 3.14\nunion IntOrFloat15 { i: long; f: float; }\ntypedef Vectors15 = Vector[1+2]\nfunc f15() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f15() { enum E15 { A, B, C } return; }\nfunc f15() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i16: long = cast(long*, 42)\nvar x16: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector16 { x, y: float; }\nvar v16 = Vector{x = 1.0, y = -1.0}\nvar v16: Vector = {1.0, -1.0}\nconst n16 = sizeof(:long*[16])\nconst n16 = sizeof(1+2)\nvar x16 = b16 == 1 ? 1+2 : 3-4\nfunc fact16(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact16(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo16 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f16(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color16 { RED = 3, GREEN, BLUE = 0 }\nconst pi16 = 3.14\nunion IntOrFloat16 { i: long; f: float; }\ntypedef Vectors16 = Vector[1+2]\nfunc f16() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f16() { enum E16 { A, B, C } return; }\nfunc f16() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i17: long = cast(long*, 42)\nvar x17: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector17 { x, y: float; }\nvar v17 = Vector{x = 1.0, y = -1.0}\nvar v17: Vector = {1.0, -1.0}\nconst n17 = sizeof(:long*[16])\nconst n17 = sizeof(1+2)\nvar x17 = b17 == 1 ? 1+2 : 3-4\nfunc fact17(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact17(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo17 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f17(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color17 { RED = 3, GREEN, BLUE = 0 }\nconst pi17 = 3.14\nunion IntOrFloat17 { i: long; f: float; }\ntypedef Vectors17 = Vector[1+2]\nfunc f17() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f17() { enum E17 { A, B, C } return; }\nfunc f17() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i18: long = cast(long*, 42)\nvar x18: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector18 { x, y: float; }\nvar v18 = Vector{x = 1.0, y = -1.0}\nvar v18: Vector = {1.0, -1.0}\nconst n18 = sizeof(:long*[16])\nconst n18 = sizeof(1+2)\nvar x18 = b18 == 1 ? 1+2 : 3-4\nfunc fact18(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact18(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo18 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f18(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color18 { RED = 3, GREEN, BLUE = 0 }\nconst pi18 = 3.14\nunion IntOrFloat18 { i: long; f: float; }\ntypedef Vectors18 = Vector[1+2]\nfunc f18() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f18() { enum E18 { A, B, C } return; }\nfunc f18() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i19: long = cast(long*, 42)\nvar x19: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector19 { x, y: float; }\nvar v19 = Vector{x = 1.0, y = -1.0}\nvar v19: Vector = {1.0, -1.0}\nconst n19 = sizeof(:long*[16])\nconst n19 = sizeof(1+2)\nvar x19 = b19 == 1 ? 1+2 : 3-4\nfunc fact19(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact19(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo19 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f19(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color19 { RED = 3, GREEN, BLUE = 0 }\nconst pi19 = 3.14\nunion IntOrFloat19 { i: long; f: float; }\ntypedef Vectors19 = Vector[1+2]\nfunc f19() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f19() { enum E19 { A, B, C } return; }\nfunc f19() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i20: long = cast(long*, 42)\nvar x20: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector20 { x, y: float; }\nvar v20 = Vector{x = 1.0, y = -1.0}\nvar v20: Vector = {1.0, -1.0}\nconst n20 = sizeof(:long*[16])\nconst n20 = sizeof(1+2)\nvar x20 = b20 == 1 ? 1+2 : 3-4\nfunc fact20(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact20(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo20 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f20(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color20 { RED = 3, GREEN, BLUE = 0 }\nconst pi20 = 3.14\nunion IntOrFloat20 { i: long; f: float; }\ntypedef Vectors20 = Vector[1+2]\nfunc f20() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f20() { enum E20 { A, B, C } return; }\nfunc f20() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i21: long = cast(long*, 42)\nvar x21: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector21 { x, y: float; }\nvar v21 = Vector{x = 1.0, y = -1.0}\nvar v21: Vector = {1.0, -1.0}\nconst n21 = sizeof(:long*[16])\nconst n21 = sizeof(1+2)\nvar x21 = b21 == 1 ? 1+2 : 3-4\nfunc fact21(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact21(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo21 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f21(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color21 { RED = 3, GREEN, BLUE = 0 }\nconst pi21 = 3.14\nunion IntOrFloat21 { i: long; f: float; }\ntypedef Vectors21 = Vector[1+2]\nfunc f21() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f21() { enum E21 { A, B, C } return; }\nfunc f21() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i22: long = cast(long*, 42)\nvar x22: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector22 { x, y: float; }\nvar v22 = Vector{x = 1.0, y = -1.0}\nvar v22: Vector = {1.0, -1.0}\nconst n22 = sizeof(:long*[16])\nconst n22 = sizeof(1+2)\nvar x22 = b22 == 1 ? 1+2 : 3-4\nfunc fact22(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact22(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo22 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f22(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color22 { RED = 3, GREEN, BLUE = 0 }\nconst pi22 = 3.14\nunion IntOrFloat22 { i: long; f: float; }\ntypedef Vectors22 = Vector[1+2]\nfunc f22() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f22() { enum E22 { A, B, C } return; }\nfunc f22() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i23: long = cast(long*, 42)\nvar x23: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector23 { x, y: float; }\nvar v23 = Vector{x = 1.0, y = -1.0}\nvar v23: Vector = {1.0, -1.0}\nconst n23 = sizeof(:long*[16])\nconst n23 = sizeof(1+2)\nvar x23 = b23 == 1 ? 1+2 : 3-4\nfunc fact23(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact23(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo23 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f23(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color23 { RED = 3, GREEN, BLUE = 0 }\nconst pi23 = 3.14\nunion IntOrFloat23 { i: long; f: float; }\ntypedef Vectors23 = Vector[1+2]\nfunc f23() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f23() { enum E23 { A, B, C } return; }\nfunc f23() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i24: long = cast(long*, 42)\nvar x24: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector24 { x, y: float; }\nvar v24 = Vector{x = 1.0, y = -1.0}\nvar v24: Vector = {1.0, -1.0}\nconst n24 = sizeof(:long*[16])\nconst n24 = sizeof(1+2)\nvar x24 = b24 == 1 ? 1+2 : 3-4\nfunc fact24(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact24(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo24 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f24(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color24 { RED = 3, GREEN, BLUE = 0 }\nconst pi24 = 3.14\nunion IntOrFloat24 { i: long; f: float; }\ntypedef Vectors24 = Vector[1+2]\nfunc f24() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f24() { enum E24 { A, B, C } return; }\nfunc f24() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i25: long = cast(long*, 42)\nvar x25: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector25 { x, y: float; }\nvar v25 = Vector{x = 1.0, y = -1.0}\nvar v25: Vector = {1.0, -1.0}\nconst n25 = sizeof(:long*[16])\nconst n25 = sizeof(1+2)\nvar x25 = b25 == 1 ? 1+2 : 3-4\nfunc fact25(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact25(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo25 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f25(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color25 { RED = 3, GREEN, BLUE = 0 }\nconst pi25 = 3.14\nunion IntOrFloat25 { i: long; f: float; }\ntypedef Vectors25 = Vector[1+2]\nfunc f25() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f25() { enum E25 { A, B, C } return; }\nfunc f25() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i26: long = cast(long*, 42)\nvar x26: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector26 { x, y: float; }\nvar v26 = Vector{x = 1.0, y = -1.0}\nvar v26: Vector = {1.0, -1.0}\nconst n26 = sizeof(:long*[16])\nconst n26 = sizeof(1+2)\nvar x26 = b26 == 1 ? 1+2 : 3-4\nfunc fact26(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact26(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo26 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f26(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color26 { RED = 3, GREEN, BLUE = 0 }\nconst pi26 = 3.14\nunion IntOrFloat26 { i: long; f: float; }\ntypedef Vectors26 = Vector[1+2]\nfunc f26() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f26() { enum E26 { A, B, C } return; }\nfunc f26() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i27: long = cast(long*, 42)\nvar x27: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector27 { x, y: float; }\nvar v27 = Vector{x = 1.0, y = -1.0}\nvar v27: Vector = {1.0, -1.0}\nconst n27 = sizeof(:long*[16])\nconst n27 = sizeof(1+2)\nvar x27 = b27 == 1 ? 1+2 : 3-4\nfunc fact27(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact27(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo27 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f27(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color27 { RED = 3, GREEN, BLUE = 0 }\nconst pi27 = 3.14\nunion IntOrFloat27 { i: long; f: float; }\ntypedef Vectors27 = Vector[1+2]\nfunc f27() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f27() { enum E27 { A, B, C } return; }\nfunc f27() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i28: long = cast(long*, 42)\nvar x28: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector28 { x, y: float; }\nvar v28 = Vector{x = 1.0, y = -1.0}\nvar v28: Vector = {1.0, -1.0}\nconst n28 = sizeof(:long*[16])\nconst n28 = sizeof(1+2)\nvar x28 = b28 == 1 ? 1+2 : 3-4\nfunc fact28(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact28(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo28 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f28(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color28 { RED = 3, GREEN, BLUE = 0 }\nconst pi28 = 3.14\nunion IntOrFloat28 { i: long; f: float; }\ntypedef Vectors28 = Vector[1+2]\nfunc f28() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f28() { enum E28 { A, B, C } return; }\nfunc f28() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i29: long = cast(long*, 42)\nvar x29: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector29 { x, y: float; }\nvar v29 = Vector{x = 1.0, y = -1.0}\nvar v29: Vector = {1.0, -1.0}\nconst n29 = sizeof(:long*[16])\nconst n29 = sizeof(1+2)\nvar x29 = b29 == 1 ? 1+2 : 3-4\nfunc fact29(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact29(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo29 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f29(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color29 { RED = 3, GREEN, BLUE = 0 }\nconst pi29 = 3.14\nunion IntOrFloat29 { i: long; f: float; }\ntypedef Vectors29 = Vector[1+2]\nfunc f29() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f29() { enum E29 { A, B, C } return; }\nfunc f29() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i30: long = cast(long*, 42)\nvar x30: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector30 { x, y: float; }\nvar v30 = Vector{x = 1.0, y = -1.0}\nvar v30: Vector = {1.0, -1.0}\nconst n30 = sizeof(:long*[16])\nconst n30 = sizeof(1+2)\nvar x30 = b30 == 1 ? 1+2 : 3-4\nfunc fact30(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact30(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo30 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f30(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color30 { RED = 3, GREEN, BLUE = 0 }\nconst pi30 = 3.14\nunion IntOrFloat30 { i: long; f: float; }\ntypedef Vectors30 = Vector[1+2]\nfunc f30() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f30() { enum E30 { A, B, C } return; }\nfunc f30() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i31: long = cast(long*, 42)\nvar x31: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector31 { x, y: float; }\nvar v31 = Vector{x = 1.0, y = -1.0}\nvar v31: Vector = {1.0, -1.0}\nconst n31 = sizeof(:long*[16])\nconst n31 = sizeof(1+2)\nvar x31 = b31 == 1 ? 1+2 : 3-4\nfunc fact31(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact31(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo31 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f31(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color31 { RED = 3, GREEN, BLUE = 0 }\nconst pi31 = 3.14\nunion IntOrFloat31 { i: long; f: float; }\ntypedef Vectors31 = Vector[1+2]\nfunc f31() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f31() { enum E31 { A, B, C } return; }\nfunc f31() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i32: long = cast(long*, 42)\nvar x32: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector32 { x, y: float; }\nvar v32 = Vector{x = 1.0, y = -1.0}\nvar v32: Vector = {1.0, -1.0}\nconst n32 = sizeof(:long*[16])\nconst n32 = sizeof(1+2)\nvar x32 = b32 == 1 ? 1+2 : 3-4\nfunc fact32(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact32(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo32 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f32(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color32 { RED = 3, GREEN, BLUE = 0 }\nconst pi32 = 3.14\nunion IntOrFloat32 { i: long; f: float; }\ntypedef Vectors32 = Vector[1+2]\nfunc f32() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f32() { enum E32 { A, B, C } return; }\nfunc f32() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i33: long = cast(long*, 42)\nvar x33: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector33 { x, y: float; }\nvar v33 = Vector{x = 1.0, y = -1.0}\nvar v33: Vector = {1.0, -1.0}\nconst n33 = sizeof(:long*[16])\nconst n33 = sizeof(1+2)\nvar x33 = b33 == 1 ? 1+2 : 3-4\nfunc fact33(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact33(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo33 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f33(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color33 { RED = 3, GREEN, BLUE = 0 }\nconst pi33 = 3.14\nunion IntOrFloat33 { i: long; f: float; }\ntypedef Vectors33 = Vector[1+2]\nfunc f33() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f33() { enum E33 { A, B, C } return; }\nfunc f33() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i34: long = cast(long*, 42)\nvar x34: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector34 { x, y: float; }\nvar v34 = Vector{x = 1.0, y = -1.0}\nvar v34: Vector = {1.0, -1.0}\nconst n34 = sizeof(:long*[16])\nconst n34 = sizeof(1+2)\nvar x34 = b34 == 1 ? 1+2 : 3-4\nfunc fact34(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact34(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo34 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f34(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color34 { RED = 3, GREEN, BLUE = 0 }\nconst pi34 = 3.14\nunion IntOrFloat34 { i: long; f: float; }\ntypedef Vectors34 = Vector[1+2]\nfunc f34() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f34() { enum E34 { A, B, C } return; }\nfunc f34() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i35: long = cast(long*, 42)\nvar x35: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector35 { x, y: float; }\nvar v35 = Vector{x = 1.0, y = -1.0}\nvar v35: Vector = {1.0, -1.0}\nconst n35 = sizeof(:long*[16])\nconst n35 = sizeof(1+2)\nvar x35 = b35 == 1 ? 1+2 : 3-4\nfunc fact35(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact35(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo35 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f35(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color35 { RED = 3, GREEN, BLUE = 0 }\nconst pi35 = 3.14\nunion IntOrFloat35 { i: long; f: float; }\ntypedef Vectors35 = Vector[1+2]\nfunc f35() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f35() { enum E35 { A, B, C } return; }\nfunc f35() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i36: long = cast(long*, 42)\nvar x36: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector36 { x, y: float; }\nvar v36 = Vector{x = 1.0, y = -1.0}\nvar v36: Vector = {1.0, -1.0}\nconst n36 = sizeof(:long*[16])\nconst n36 = sizeof(1+2)\nvar x36 = b36 == 1 ? 1+2 : 3-4\nfunc fact36(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact36(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo36 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f36(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color36 { RED = 3, GREEN, BLUE = 0 }\nconst pi36 = 3.14\nunion IntOrFloat36 { i: long; f: float; }\ntypedef Vectors36 = Vector[1+2]\nfunc f36() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f36() { enum E36 { A, B, C } return; }\nfunc f36() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i37: long = cast(long*, 42)\nvar x37: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector37 { x, y: float; }\nvar v37 = Vector{x = 1.0, y = -1.0}\nvar v37: Vector = {1.0, -1.0}\nconst n37 = sizeof(:long*[16])\nconst n37 = sizeof(1+2)\nvar x37 = b37 == 1 ? 1+2 : 3-4\nfunc fact37(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact37(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo37 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f37(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color37 { RED = 3, GREEN, BLUE = 0 }\nconst pi37 = 3.14\nunion IntOrFloat37 { i: long; f: float; }\ntypedef Vectors37 = Vector[1+2]\nfunc f37() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f37() { enum E37 { A, B, C } return; }\nfunc f37() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i38: long = cast(long*, 42)\nvar x38: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector38 { x, y: float; }\nvar v38 = Vector{x = 1.0, y = -1.0}\nvar v38: Vector = {1.0, -1.0}\nconst n38 = sizeof(:long*[16])\nconst n38 = sizeof(1+2)\nvar x38 = b38 == 1 ? 1+2 : 3-4\nfunc fact38(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact38(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo38 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f38(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color38 { RED = 3, GREEN, BLUE = 0 }\nconst pi38 = 3.14\nunion IntOrFloat38 { i: long; f: float; }\ntypedef Vectors38 = Vector[1+2]\nfunc f38() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f38() { enum E38 { A, B, C } return; }\nfunc f38() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i39: long = cast(long*, 42)\nvar x39: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector39 { x, y: float; }\nvar v39 = Vector{x = 1.0, y = -1.0}\nvar v39: Vector = {1.0, -1.0}\nconst n39 = sizeof(:long*[16])\nconst n39 = sizeof(1+2)\nvar x39 = b39 == 1 ? 1+2 : 3-4\nfunc fact39(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact39(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo39 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f39(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color39 { RED = 3, GREEN, BLUE = 0 }\nconst pi39 = 3.14\nunion IntOrFloat39 { i: long; f: float; }\ntypedef Vectors39 = Vector[1+2]\nfunc f39() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f39() { enum E39 { A, B, C } return; }\nfunc f39() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i40: long = cast(long*, 42)\nvar x40: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector40 { x, y: float; }\nvar v40 = Vector{x = 1.0, y = -1.0}\nvar v40: Vector = {1.0, -1.0}\nconst n40 = sizeof(:long*[16])\nconst n40 = sizeof(1+2)\nvar x40 = b40 == 1 ? 1+2 : 3-4\nfunc fact40(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact40(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo40 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f40(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color40 { RED = 3, GREEN, BLUE = 0 }\nconst pi40 = 3.14\nunion IntOrFloat40 { i: long; f: float; }\ntypedef Vectors40 = Vector[1+2]\nfunc f40() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f40() { enum E40 { A, B, C } return; }\nfunc f40() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i41: long = cast(long*, 42)\nvar x41: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector41 { x, y: float; }\nvar v41 = Vector{x = 1.0, y = -1.0}\nvar v41: Vector = {1.0, -1.0}\nconst n41 = sizeof(:long*[16])\nconst n41 = sizeof(1+2)\nvar x41 = b41 == 1 ? 1+2 : 3-4\nfunc fact41(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact41(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo41 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f41(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color41 { RED = 3, GREEN, BLUE = 0 }\nconst pi41 = 3.14\nunion IntOrFloat41 { i: long; f: float; }\ntypedef Vectors41 = Vector[1+2]\nfunc f41() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f41() { enum E41 { A, B, C } return; }\nfunc f41() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i42: long = cast(long*, 42)\nvar x42: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector42 { x, y: float; }\nvar v42 = Vector{x = 1.0, y = -1.0}\nvar v42: Vector = {1.0, -1.0}\nconst n42 = sizeof(:long*[16])\nconst n42 = sizeof(1+2)\nvar x42 = b42 == 1 ? 1+2 : 3-4\nfunc fact42(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact42(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo42 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f42(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color42 { RED = 3, GREEN, BLUE = 0 }\nconst pi42 = 3.14\nunion IntOrFloat42 { i: long; f: float; }\ntypedef Vectors42 = Vector[1+2]\nfunc f42() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f42() { enum E42 { A, B, C } return; }\nfunc f42() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i43: long = cast(long*, 42)\nvar x43: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector43 { x, y: float; }\nvar v43 = Vector{x = 1.0, y = -1.0}\nvar v43: Vector = {1.0, -1.0}\nconst n43 = sizeof(:long*[16])\nconst n43 = sizeof(1+2)\nvar x43 = b43 == 1 ? 1+2 : 3-4\nfunc fact43(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact43(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo43 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f43(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color43 { RED = 3, GREEN, BLUE = 0 }\nconst pi43 = 3.14\nunion IntOrFloat43 { i: long; f: float; }\ntypedef Vectors43 = Vector[1+2]\nfunc f43() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f43() { enum E43 { A, B, C } return; }\nfunc f43() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i44: long = cast(long*, 42)\nvar x44: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector44 { x, y: float; }\nvar v44 = Vector{x = 1.0, y = -1.0}\nvar v44: Vector = {1.0, -1.0}\nconst n44 = sizeof(:long*[16])\nconst n44 = sizeof(1+2)\nvar x44 = b44 == 1 ? 1+2 : 3-4\nfunc fact44(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact44(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo44 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f44(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color44 { RED = 3, GREEN, BLUE = 0 }\nconst pi44 = 3.14\nunion IntOrFloat44 { i: long; f: float; }\ntypedef Vectors44 = Vector[1+2]\nfunc f44() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f44() { enum E44 { A, B, C } return; }\nfunc f44() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i45: long = cast(long*, 42)\nvar x45: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector45 { x, y: float; }\nvar v45 = Vector{x = 1.0, y = -1.0}\nvar v45: Vector = {1.0, -1.0}\nconst n45 = sizeof(:long*[16])\nconst n45 = sizeof(1+2)\nvar x45 = b45 == 1 ? 1+2 : 3-4\nfunc fact45(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact45(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo45 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f45(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color45 { RED = 3, GREEN, BLUE = 0 }\nconst pi45 = 3.14\nunion IntOrFloat45 { i: long; f: float; }\ntypedef Vectors45 = Vector[1+2]\nfunc f45() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f45() { enum E45 { A, B, C } return; }\nfunc f45() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i46: long = cast(long*, 42)\nvar x46: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector46 { x, y: float; }\nvar v46 = Vector{x = 1.0, y = -1.0}\nvar v46: Vector = {1.0, -1.0}\nconst n46 = sizeof(:long*[16])\nconst n46 = sizeof(1+2)\nvar x46 = b46 == 1 ? 1+2 : 3-4\nfunc fact46(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact46(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo46 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f46(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color46 { RED = 3, GREEN, BLUE = 0 }\nconst pi46 = 3.14\nunion IntOrFloat46 { i: long; f: float; }\ntypedef Vectors46 = Vector[1+2]\nfunc f46() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f46() { enum E46 { A, B, C } return; }\nfunc f46() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i47: long = cast(long*, 42)\nvar x47: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector47 { x, y: float; }\nvar v47 = Vector{x = 1.0, y = -1.0}\nvar v47: Vector = {1.0, -1.0}\nconst n47 = sizeof(:long*[16])\nconst n47 = sizeof(1+2)\nvar x47 = b47 == 1 ? 1+2 : 3-4\nfunc fact47(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact47(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo47 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f47(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color47 { RED = 3, GREEN, BLUE = 0 }\nconst pi47 = 3.14\nunion IntOrFloat47 { i: long; f: float; }\ntypedef Vectors47 = Vector[1+2]\nfunc f47() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f47() { enum E47 { A, B, C } return; }\nfunc f47() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i48: long = cast(long*, 42)\nvar x48: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector48 { x, y: float; }\nvar v48 = Vector{x = 1.0, y = -1.0}\nvar v48: Vector = {1.0, -1.0}\nconst n48 = sizeof(:long*[16])\nconst n48 = sizeof(1+2)\nvar x48 = b48 == 1 ? 1+2 : 3-4\nfunc fact48(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact48(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo48 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f48(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color48 { RED = 3, GREEN, BLUE = 0 }\nconst pi48 = 3.14\nunion IntOrFloat48 { i: long; f: float; }\ntypedef Vectors48 = Vector[1+2]\nfunc f48() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f48() { enum E48 { A, B, C } return; }\nfunc f48() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i49: long = cast(long*, 42)\nvar x49: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector49 { x, y: float; }\nvar v49 = Vector{x = 1.0, y = -1.0}\nvar v49: Vector = {1.0, -1.0}\nconst n49 = sizeof(:long*[16])\nconst n49 = sizeof(1+2)\nvar x49 = b49 == 1 ? 1+2 : 3-4\nfunc fact49(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact49(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo49 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f49(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color49 { RED = 3, GREEN, BLUE = 0 }\nconst pi49 = 3.14\nunion IntOrFloat49 { i: long; f: float; }\ntypedef Vectors49 = Vector[1+2]\nfunc f49() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f49() { enum E49 { A, B, C } return; }\nfunc f49() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i50: long = cast(long*, 42)\nvar x50: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector50 { x, y: float; }\nvar v50 = Vector{x = 1.0, y = -1.0}\nvar v50: Vector = {1.0, -1.0}\nconst n50 = sizeof(:long*[16])\nconst n50 = sizeof(1+2)\nvar x50 = b50 == 1 ? 1+2 : 3-4\nfunc fact50(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact50(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo50 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f50(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color50 { RED = 3, GREEN, BLUE = 0 }\nconst pi50 = 3.14\nunion IntOrFloat50 { i: long; f: float; }\ntypedef Vectors50 = Vector[1+2]\nfunc f50() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f50() { enum E50 { A, B, C } return; }\nfunc f50() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i51: long = cast(long*, 42)\nvar x51: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector51 { x, y: float; }\nvar v51 = Vector{x = 1.0, y = -1.0}\nvar v51: Vector = {1.0, -1.0}\nconst n51 = sizeof(:long*[16])\nconst n51 = sizeof(1+2)\nvar x51 = b51 == 1 ? 1+2 : 3-4\nfunc fact51(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact51(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo51 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f51(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color51 { RED = 3, GREEN, BLUE = 0 }\nconst pi51 = 3.14\nunion IntOrFloat51 { i: long; f: float; }\ntypedef Vectors51 = Vector[1+2]\nfunc f51() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f51() { enum E51 { A, B, C } return; }\nfunc f51() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i52: long = cast(long*, 42)\nvar x52: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector52 { x, y: float; }\nvar v52 = Vector{x = 1.0, y = -1.0}\nvar v52: Vector = {1.0, -1.0}\nconst n52 = sizeof(:long*[16])\nconst n52 = sizeof(1+2)\nvar x52 = b52 == 1 ? 1+2 : 3-4\nfunc fact52(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact52(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo52 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f52(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color52 { RED = 3, GREEN, BLUE = 0 }\nconst pi52 = 3.14\nunion IntOrFloat52 { i: long; f: float; }\ntypedef Vectors52 = Vector[1+2]\nfunc f52() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f52() { enum E52 { A, B, C } return; }\nfunc f52() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i53: long = cast(long*, 42)\nvar x53: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector53 { x, y: float; }\nvar v53 = Vector{x = 1.0, y = -1.0}\nvar v53: Vector = {1.0, -1.0}\nconst n53 = sizeof(:long*[16])\nconst n53 = sizeof(1+2)\nvar x53 = b53 == 1 ? 1+2 : 3-4\nfunc fact53(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact53(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo53 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f53(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color53 { RED = 3, GREEN, BLUE = 0 }\nconst pi53 = 3.14\nunion IntOrFloat53 { i: long; f: float; }\ntypedef Vectors53 = Vector[1+2]\nfunc f53() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f53() { enum E53 { A, B, C } return; }\nfunc f53() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i54: long = cast(long*, 42)\nvar x54: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector54 { x, y: float; }\nvar v54 = Vector{x = 1.0, y = -1.0}\nvar v54: Vector = {1.0, -1.0}\nconst n54 = sizeof(:long*[16])\nconst n54 = sizeof(1+2)\nvar x54 = b54 == 1 ? 1+2 : 3-4\nfunc fact54(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact54(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo54 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f54(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color54 { RED = 3, GREEN, BLUE = 0 }\nconst pi54 = 3.14\nunion IntOrFloat54 { i: long; f: float; }\ntypedef Vectors54 = Vector[1+2]\nfunc f54() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f54() { enum E54 { A, B, C } return; }\nfunc f54() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i55: long = cast(long*, 42)\nvar x55: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector55 { x, y: float; }\nvar v55 = Vector{x = 1.0, y = -1.0}\nvar v55: Vector = {1.0, -1.0}\nconst n55 = sizeof(:long*[16])\nconst n55 = sizeof(1+2)\nvar x55 = b55 == 1 ? 1+2 : 3-4\nfunc fact55(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact55(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo55 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f55(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color55 { RED = 3, GREEN, BLUE = 0 }\nconst pi55 = 3.14\nunion IntOrFloat55 { i: long; f: float; }\ntypedef Vectors55 = Vector[1+2]\nfunc f55() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f55() { enum E55 { A, B, C } return; }\nfunc f55() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i56: long = cast(long*, 42)\nvar x56: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector56 { x, y: float; }\nvar v56 = Vector{x = 1.0, y = -1.0}\nvar v56: Vector = {1.0, -1.0}\nconst n56 = sizeof(:long*[16])\nconst n56 = sizeof(1+2)\nvar x56 = b56 == 1 ? 1+2 : 3-4\nfunc fact56(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact56(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo56 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f56(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color56 { RED = 3, GREEN, BLUE = 0 }\nconst pi56 = 3.14\nunion IntOrFloat56 { i: long; f: float; }\ntypedef Vectors56 = Vector[1+2]\nfunc f56() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f56() { enum E56 { A, B, C } return; }\nfunc f56() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i57: long = cast(long*, 42)\nvar x57: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector57 { x, y: float; }\nvar v57 = Vector{x = 1.0, y = -1.0}\nvar v57: Vector = {1.0, -1.0}\nconst n57 = sizeof(:long*[16])\nconst n57 = sizeof(1+2)\nvar x57 = b57 == 1 ? 1+2 : 3-4\nfunc fact57(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact57(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo57 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f57(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color57 { RED = 3, GREEN, BLUE = 0 }\nconst pi57 = 3.14\nunion IntOrFloat57 { i: long; f: float; }\ntypedef Vectors57 = Vector[1+2]\nfunc f57() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f57() { enum E57 { A, B, C } return; }\nfunc f57() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i58: long = cast(long*, 42)\nvar x58: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector58 { x, y: float; }\nvar v58 = Vector{x = 1.0, y = -1.0}\nvar v58: Vector = {1.0, -1.0}\nconst n58 = sizeof(:long*[16])\nconst n58 = sizeof(1+2)\nvar x58 = b58 == 1 ? 1+2 : 3-4\nfunc fact58(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact58(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo58 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f58(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color58 { RED = 3, GREEN, BLUE = 0 }\nconst pi58 = 3.14\nunion IntOrFloat58 { i: long; f: float; }\ntypedef Vectors58 = Vector[1+2]\nfunc f58() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f58() { enum E58 { A, B, C } return; }\nfunc f58() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i59: long = cast(long*, 42)\nvar x59: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector59 { x, y: float; }\nvar v59 = Vector{x = 1.0, y = -1.0}\nvar v59: Vector = {1.0, -1.0}\nconst n59 = sizeof(:long*[16])\nconst n59 = sizeof(1+2)\nvar x59 = b59 == 1 ? 1+2 : 3-4\nfunc fact59(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact59(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo59 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f59(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color59 { RED = 3, GREEN, BLUE = 0 }\nconst pi59 = 3.14\nunion IntOrFloat59 { i: long; f: float; }\ntypedef Vectors59 = Vector[1+2]\nfunc f59() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f59() { enum E59 { A, B, C } return; }\nfunc f59() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i60: long = cast(long*, 42)\nvar x60: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector60 { x, y: float; }\nvar v60 = Vector{x = 1.0, y = -1.0}\nvar v60: Vector = {1.0, -1.0}\nconst n60 = sizeof(:long*[16])\nconst n60 = sizeof(1+2)\nvar x60 = b60 == 1 ? 1+2 : 3-4\nfunc fact60(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact60(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo60 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f60(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color60 { RED = 3, GREEN, BLUE = 0 }\nconst pi60 = 3.14\nunion IntOrFloat60 { i: long; f: float; }\ntypedef Vectors60 = Vector[1+2]\nfunc f60() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f60() { enum E60 { A, B, C } return; }\nfunc f60() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i61: long = cast(long*, 42)\nvar x61: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector61 { x, y: float; }\nvar v61 = Vector{x = 1.0, y = -1.0}\nvar v61: Vector = {1.0, -1.0}\nconst n61 = sizeof(:long*[16])\nconst n61 = sizeof(1+2)\nvar x61 = b61 == 1 ? 1+2 : 3-4\nfunc fact61(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact61(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo61 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f61(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color61 { RED = 3, GREEN, BLUE = 0 }\nconst pi61 = 3.14\nunion IntOrFloat61 { i: long; f: float; }\ntypedef Vectors61 = Vector[1+2]\nfunc f61() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f61() { enum E61 { A, B, C } return; }\nfunc f61() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i62: long = cast(long*, 42)\nvar x62: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector62 { x, y: float; }\nvar v62 = Vector{x = 1.0, y = -1.0}\nvar v62: Vector = {1.0, -1.0}\nconst n62 = sizeof(:long*[16])\nconst n62 = sizeof(1+2)\nvar x62 = b62 == 1 ? 1+2 : 3-4\nfunc fact62(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact62(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo62 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f62(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color62 { RED = 3, GREEN, BLUE = 0 }\nconst pi62 = 3.14\nunion IntOrFloat62 { i: long; f: float; }\ntypedef Vectors62 = Vector[1+2]\nfunc f62() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f62() { enum E62 { A, B, C } return; }\nfunc f62() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i63: long = cast(long*, 42)\nvar x63: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector63 { x, y: float; }\nvar v63 = Vector{x = 1.0, y = -1.0}\nvar v63: Vector = {1.0, -1.0}\nconst n63 = sizeof(:long*[16])\nconst n63 = sizeof(1+2)\nvar x63 = b63 == 1 ? 1+2 : 3-4\nfunc fact63(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact63(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo63 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f63(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color63 { RED = 3, GREEN, BLUE = 0 }\nconst pi63 = 3.14\nunion IntOrFloat63 { i: long; f: float; }\ntypedef Vectors63 = Vector[1+2]\nfunc f63() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f63() { enum E63 { A, B, C } return; }\nfunc f63() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i64: long = cast(long*, 42)\nvar x64: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector64 { x, y: float; }\nvar v64 = Vector{x = 1.0, y = -1.0}\nvar v64: Vector = {1.0, -1.0}\nconst n64 = sizeof(:long*[16])\nconst n64 = sizeof(1+2)\nvar x64 = b64 == 1 ? 1+2 : 3-4\nfunc fact64(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact64(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo64 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f64(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color64 { RED = 3, GREEN, BLUE = 0 }\nconst pi64 = 3.14\nunion IntOrFloat64 { i: long; f: float; }\ntypedef Vectors64 = Vector[1+2]\nfunc f64() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f64() { enum E64 { A, B, C } return; }\nfunc f64() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i65: long = cast(long*, 42)\nvar x65: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector65 { x, y: float; }\nvar v65 = Vector{x = 1.0, y = -1.0}\nvar v65: Vector = {1.0, -1.0}\nconst n65 = sizeof(:long*[16])\nconst n65 = sizeof(1+2)\nvar x65 = b65 == 1 ? 1+2 : 3-4\nfunc fact65(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact65(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo65 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f65(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color65 { RED = 3, GREEN, BLUE = 0 }\nconst pi65 = 3.14\nunion IntOrFloat65 { i: long; f: float; }\ntypedef Vectors65 = Vector[1+2]\nfunc f65() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f65() { enum E65 { A, B, C } return; }\nfunc f65() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i66: long = cast(long*, 42)\nvar x66: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector66 { x, y: float; }\nvar v66 = Vector{x = 1.0, y = -1.0}\nvar v66: Vector = {1.0, -1.0}\nconst n66 = sizeof(:long*[16])\nconst n66 = sizeof(1+2)\nvar x66 = b66 == 1 ? 1+2 : 3-4\nfunc fact66(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact66(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo66 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f66(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color66 { RED = 3, GREEN, BLUE = 0 }\nconst pi66 = 3.14\nunion IntOrFloat66 { i: long; f: float; }\ntypedef Vectors66 = Vector[1+2]\nfunc f66() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f66() { enum E66 { A, B, C } return; }\nfunc f66() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i67: long = cast(long*, 42)\nvar x67: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector67 { x, y: float; }\nvar v67 = Vector{x = 1.0, y = -1.0}\nvar v67: Vector = {1.0, -1.0}\nconst n67 = sizeof(:long*[16])\nconst n67 = sizeof(1+2)\nvar x67 = b67 == 1 ? 1+2 : 3-4\nfunc fact67(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact67(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo67 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f67(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color67 { RED = 3, GREEN, BLUE = 0 }\nconst pi67 = 3.14\nunion IntOrFloat67 { i: long; f: float; }\ntypedef Vectors67 = Vector[1+2]\nfunc f67() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f67() { enum E67 { A, B, C } return; }\nfunc f67() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i68: long = cast(long*, 42)\nvar x68: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector68 { x, y: float; }\nvar v68 = Vector{x = 1.0, y = -1.0}\nvar v68: Vector = {1.0, -1.0}\nconst n68 = sizeof(:long*[16])\nconst n68 = sizeof(1+2)\nvar x68 = b68 == 1 ? 1+2 : 3-4\nfunc fact68(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact68(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo68 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f68(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color68 { RED = 3, GREEN, BLUE = 0 }\nconst pi68 = 3.14\nunion IntOrFloat68 { i: long; f: float; }\ntypedef Vectors68 = Vector[1+2]\nfunc f68() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f68() { enum E68 { A, B, C } return; }\nfunc f68() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i69: long = cast(long*, 42)\nvar x69: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector69 { x, y: float; }\nvar v69 = Vector{x = 1.0, y = -1.0}\nvar v69: Vector = {1.0, -1.0}\nconst n69 = sizeof(:long*[16])\nconst n69 = sizeof(1+2)\nvar x69 = b69 == 1 ? 1+2 : 3-4\nfunc fact69(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact69(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo69 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f69(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color69 { RED = 3, GREEN, BLUE = 0 }\nconst pi69 = 3.14\nunion IntOrFloat69 { i: long; f: float; }\ntypedef Vectors69 = Vector[1+2]\nfunc f69() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f69() { enum E69 { A, B, C } return; }\nfunc f69() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i70: long = cast(long*, 42)\nvar x70: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector70 { x, y: float; }\nvar v70 = Vector{x = 1.0, y = -1.0}\nvar v70: Vector = {1.0, -1.0}\nconst n70 = sizeof(:long*[16])\nconst n70 = sizeof(1+2)\nvar x70 = b70 == 1 ? 1+2 : 3-4\nfunc fact70(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact70(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo70 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f70(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color70 { RED = 3, GREEN, BLUE = 0 }\nconst pi70 = 3.14\nunion IntOrFloat70 { i: long; f: float; }\ntypedef Vectors70 = Vector[1+2]\nfunc f70() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f70() { enum E70 { A, B, C } return; }\nfunc f70() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i71: long = cast(long*, 42)\nvar x71: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector71 { x, y: float; }\nvar v71 = Vector{x = 1.0, y = -1.0}\nvar v71: Vector = {1.0, -1.0}\nconst n71 = sizeof(:long*[16])\nconst n71 = sizeof(1+2)\nvar x71 = b71 == 1 ? 1+2 : 3-4\nfunc fact71(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact71(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo71 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f71(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color71 { RED = 3, GREEN, BLUE = 0 }\nconst pi71 = 3.14\nunion IntOrFloat71 { i: long; f: float; }\ntypedef Vectors71 = Vector[1+2]\nfunc f71() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f71() { enum E71 { A, B, C } return; }\nfunc f71() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i72: long = cast(long*, 42)\nvar x72: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector72 { x, y: float; }\nvar v72 = Vector{x = 1.0, y = -1.0}\nvar v72: Vector = {1.0, -1.0}\nconst n72 = sizeof(:long*[16])\nconst n72 = sizeof(1+2)\nvar x72 = b72 == 1 ? 1+2 : 3-4\nfunc fact72(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact72(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo72 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f72(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color72 { RED = 3, GREEN, BLUE = 0 }\nconst pi72 = 3.14\nunion IntOrFloat72 { i: long; f: float; }\ntypedef Vectors72 = Vector[1+2]\nfunc f72() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f72() { enum E72 { A, B, C } return; }\nfunc f72() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i73: long = cast(long*, 42)\nvar x73: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector73 { x, y: float; }\nvar v73 = Vector{x = 1.0, y = -1.0}\nvar v73: Vector = {1.0, -1.0}\nconst n73 = sizeof(:long*[16])\nconst n73 = sizeof(1+2)\nvar x73 = b73 == 1 ? 1+2 : 3-4\nfunc fact73(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact73(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo73 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f73(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color73 { RED = 3, GREEN, BLUE = 0 }\nconst pi73 = 3.14\nunion IntOrFloat73 { i: long; f: float; }\ntypedef Vectors73 = Vector[1+2]\nfunc f73() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f73() { enum E73 { A, B, C } return; }\nfunc f73() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i74: long = cast(long*, 42)\nvar x74: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector74 { x, y: float; }\nvar v74 = Vector{x = 1.0, y = -1.0}\nvar v74: Vector = {1.0, -1.0}\nconst n74 = sizeof(:long*[16])\nconst n74 = sizeof(1+2)\nvar x74 = b74 == 1 ? 1+2 : 3-4\nfunc fact74(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact74(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo74 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f74(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color74 { RED = 3, GREEN, BLUE = 0 }\nconst pi74 = 3.14\nunion IntOrFloat74 { i: long; f: float; }\ntypedef Vectors74 = Vector[1+2]\nfunc f74() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f74() { enum E74 { A, B, C } return; }\nfunc f74() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i75: long = cast(long*, 42)\nvar x75: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector75 { x, y: float; }\nvar v75 = Vector{x = 1.0, y = -1.0}\nvar v75: Vector = {1.0, -1.0}\nconst n75 = sizeof(:long*[16])\nconst n75 = sizeof(1+2)\nvar x75 = b75 == 1 ? 1+2 : 3-4\nfunc fact75(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact75(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo75 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f75(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color75 { RED = 3, GREEN, BLUE = 0 }\nconst pi75 = 3.14\nunion IntOrFloat75 { i: long; f: float; }\ntypedef Vectors75 = Vector[1+2]\nfunc f75() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f75() { enum E75 { A, B, C } return; }\nfunc f75() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i76: long = cast(long*, 42)\nvar x76: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector76 { x, y: float; }\nvar v76 = Vector{x = 1.0, y = -1.0}\nvar v76: Vector = {1.0, -1.0}\nconst n76 = sizeof(:long*[16])\nconst n76 = sizeof(1+2)\nvar x76 = b76 == 1 ? 1+2 : 3-4\nfunc fact76(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact76(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo76 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f76(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color76 { RED = 3, GREEN, BLUE = 0 }\nconst pi76 = 3.14\nunion IntOrFloat76 { i: long; f: float; }\ntypedef Vectors76 = Vector[1+2]\nfunc f76() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f76() { enum E76 { A, B, C } return; }\nfunc f76() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i77: long = cast(long*, 42)\nvar x77: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector77 { x, y: float; }\nvar v77 = Vector{x = 1.0, y = -1.0}\nvar v77: Vector = {1.0, -1.0}\nconst n77 = sizeof(:long*[16])\nconst n77 = sizeof(1+2)\nvar x77 = b77 == 1 ? 1+2 : 3-4\nfunc fact77(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact77(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo77 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f77(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color77 { RED = 3, GREEN, BLUE = 0 }\nconst pi77 = 3.14\nunion IntOrFloat77 { i: long; f: float; }\ntypedef Vectors77 = Vector[1+2]\nfunc f77() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f77() { enum E77 { A, B, C } return; }\nfunc f77() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i78: long = cast(long*, 42)\nvar x78: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector78 { x, y: float; }\nvar v78 = Vector{x = 1.0, y = -1.0}\nvar v78: Vector = {1.0, -1.0}\nconst n78 = sizeof(:long*[16])\nconst n78 = sizeof(1+2)\nvar x78 = b78 == 1 ? 1+2 : 3-4\nfunc fact78(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact78(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo78 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f78(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color78 { RED = 3, GREEN, BLUE = 0 }\nconst pi78 = 3.14\nunion IntOrFloat78 { i: long; f: float; }\ntypedef Vectors78 = Vector[1+2]\nfunc f78() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f78() { enum E78 { A, B, C } return; }\nfunc f78() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i79: long = cast(long*, 42)\nvar x79: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector79 { x, y: float; }\nvar v79 = Vector{x = 1.0, y = -1.0}\nvar v79: Vector = {1.0, -1.0}\nconst n79 = sizeof(:long*[16])\nconst n79 = sizeof(1+2)\nvar x79 = b79 == 1 ? 1+2 : 3-4\nfunc fact79(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact79(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo79 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f79(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color79 { RED = 3, GREEN, BLUE = 0 }\nconst pi79 = 3.14\nunion IntOrFloat79 { i: long; f: float; }\ntypedef Vectors79 = Vector[1+2]\nfunc f79() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f79() { enum E79 { A, B, C } return; }\nfunc f79() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i80: long = cast(long*, 42)\nvar x80: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector80 { x, y: float; }\nvar v80 = Vector{x = 1.0, y = -1.0}\nvar v80: Vector = {1.0, -1.0}\nconst n80 = sizeof(:long*[16])\nconst n80 = sizeof(1+2)\nvar x80 = b80 == 1 ? 1+2 : 3-4\nfunc fact80(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact80(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo80 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f80(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color80 { RED = 3, GREEN, BLUE = 0 }\nconst pi80 = 3.14\nunion IntOrFloat80 { i: long; f: float; }\ntypedef Vectors80 = Vector[1+2]\nfunc f80() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f80() { enum E80 { A, B, C } return; }\nfunc f80() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i81: long = cast(long*, 42)\nvar x81: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector81 { x, y: float; }\nvar v81 = Vector{x = 1.0, y = -1.0}\nvar v81: Vector = {1.0, -1.0}\nconst n81 = sizeof(:long*[16])\nconst n81 = sizeof(1+2)\nvar x81 = b81 == 1 ? 1+2 : 3-4\nfunc fact81(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact81(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo81 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f81(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color81 { RED = 3, GREEN, BLUE = 0 }\nconst pi81 = 3.14\nunion IntOrFloat81 { i: long; f: float; }\ntypedef Vectors81 = Vector[1+2]\nfunc f81() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f81() { enum E81 { A, B, C } return; }\nfunc f81() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i82: long = cast(long*, 42)\nvar x82: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector82 { x, y: float; }\nvar v82 = Vector{x = 1.0, y = -1.0}\nvar v82: Vector = {1.0, -1.0}\nconst n82 = sizeof(:long*[16])\nconst n82 = sizeof(1+2)\nvar x82 = b82 == 1 ? 1+2 : 3-4\nfunc fact82(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact82(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo82 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f82(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color82 { RED = 3, GREEN, BLUE = 0 }\nconst pi82 = 3.14\nunion IntOrFloat82 { i: long; f: float; }\ntypedef Vectors82 = Vector[1+2]\nfunc f82() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f82() { enum E82 { A, B, C } return; }\nfunc f82() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i83: long = cast(long*, 42)\nvar x83: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector83 { x, y: float; }\nvar v83 = Vector{x = 1.0, y = -1.0}\nvar v83: Vector = {1.0, -1.0}\nconst n83 = sizeof(:long*[16])\nconst n83 = sizeof(1+2)\nvar x83 = b83 == 1 ? 1+2 : 3-4\nfunc fact83(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact83(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo83 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f83(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color83 { RED = 3, GREEN, BLUE = 0 }\nconst pi83 = 3.14\nunion IntOrFloat83 { i: long; f: float; }\ntypedef Vectors83 = Vector[1+2]\nfunc f83() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f83() { enum E83 { A, B, C } return; }\nfunc f83() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i84: long = cast(long*, 42)\nvar x84: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector84 { x, y: float; }\nvar v84 = Vector{x = 1.0, y = -1.0}\nvar v84: Vector = {1.0, -1.0}\nconst n84 = sizeof(:long*[16])\nconst n84 = sizeof(1+2)\nvar x84 = b84 == 1 ? 1+2 : 3-4\nfunc fact84(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact84(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo84 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f84(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color84 { RED = 3, GREEN, BLUE = 0 }\nconst pi84 = 3.14\nunion IntOrFloat84 { i: long; f: float; }\ntypedef Vectors84 = Vector[1+2]\nfunc f84() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f84() { enum E84 { A, B, C } return; }\nfunc f84() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i85: long = cast(long*, 42)\nvar x85: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector85 { x, y: float; }\nvar v85 = Vector{x = 1.0, y = -1.0}\nvar v85: Vector = {1.0, -1.0}\nconst n85 = sizeof(:long*[16])\nconst n85 = sizeof(1+2)\nvar x85 = b85 == 1 ? 1+2 : 3-4\nfunc fact85(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact85(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo85 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f85(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color85 { RED = 3, GREEN, BLUE = 0 }\nconst pi85 = 3.14\nunion IntOrFloat85 { i: long; f: float; }\ntypedef Vectors85 = Vector[1+2]\nfunc f85() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f85() { enum E85 { A, B, C } return; }\nfunc f85() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i86: long = cast(long*, 42)\nvar x86: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector86 { x, y: float; }\nvar v86 = Vector{x = 1.0, y = -1.0}\nvar v86: Vector = {1.0, -1.0}\nconst n86 = sizeof(:long*[16])\nconst n86 = sizeof(1+2)\nvar x86 = b86 == 1 ? 1+2 : 3-4\nfunc fact86(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact86(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo86 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f86(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color86 { RED = 3, GREEN, BLUE = 0 }\nconst pi86 = 3.14\nunion IntOrFloat86 { i: long; f: float; }\ntypedef Vectors86 = Vector[1+2]\nfunc f86() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f86() { enum E86 { A, B, C } return; }\nfunc f86() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i87: long = cast(long*, 42)\nvar x87: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector87 { x, y: float; }\nvar v87 = Vector{x = 1.0, y = -1.0}\nvar v87: Vector = {1.0, -1.0}\nconst n87 = sizeof(:long*[16])\nconst n87 = sizeof(1+2)\nvar x87 = b87 == 1 ? 1+2 : 3-4\nfunc fact87(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact87(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo87 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f87(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color87 { RED = 3, GREEN, BLUE = 0 }\nconst pi87 = 3.14\nunion IntOrFloat87 { i: long; f: float; }\ntypedef Vectors87 = Vector[1+2]\nfunc f87() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f87() { enum E87 { A, B, C } return; }\nfunc f87() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i88: long = cast(long*, 42)\nvar x88: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector88 { x, y: float; }\nvar v88 = Vector{x = 1.0, y = -1.0}\nvar v88: Vector = {1.0, -1.0}\nconst n88 = sizeof(:long*[16])\nconst n88 = sizeof(1+2)\nvar x88 = b88 == 1 ? 1+2 : 3-4\nfunc fact88(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact88(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo88 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f88(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color88 { RED = 3, GREEN, BLUE = 0 }\nconst pi88 = 3.14\nunion IntOrFloat88 { i: long; f: float; }\ntypedef Vectors88 = Vector[1+2]\nfunc f88() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f88() { enum E88 { A, B, C } return; }\nfunc f88() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i89: long = cast(long*, 42)\nvar x89: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector89 { x, y: float; }\nvar v89 = Vector{x = 1.0, y = -1.0}\nvar v89: Vector = {1.0, -1.0}\nconst n89 = sizeof(:long*[16])\nconst n89 = sizeof(1+2)\nvar x89 = b89 == 1 ? 1+2 : 3-4\nfunc fact89(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact89(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo89 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f89(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color89 { RED = 3, GREEN, BLUE = 0 }\nconst pi89 = 3.14\nunion IntOrFloat89 { i: long; f: float; }\ntypedef Vectors89 = Vector[1+2]\nfunc f89() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f89() { enum E89 { A, B, C } return; }\nfunc f89() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i90: long = cast(long*, 42)\nvar x90: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector90 { x, y: float; }\nvar v90 = Vector{x = 1.0, y = -1.0}\nvar v90: Vector = {1.0, -1.0}\nconst n90 = sizeof(:long*[16])\nconst n90 = sizeof(1+2)\nvar x90 = b90 == 1 ? 1+2 : 3-4\nfunc fact90(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact90(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo90 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f90(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color90 { RED = 3, GREEN, BLUE = 0 }\nconst pi90 = 3.14\nunion IntOrFloat90 { i: long; f: float; }\ntypedef Vectors90 = Vector[1+2]\nfunc f90() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f90() { enum E90 { A, B, C } return; }\nfunc f90() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i91: long = cast(long*, 42)\nvar x91: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector91 { x, y: float; }\nvar v91 = Vector{x = 1.0, y = -1.0}\nvar v91: Vector = {1.0, -1.0}\nconst n91 = sizeof(:long*[16])\nconst n91 = sizeof(1+2)\nvar x91 = b91 == 1 ? 1+2 : 3-4\nfunc fact91(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact91(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo91 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f91(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color91 { RED = 3, GREEN, BLUE = 0 }\nconst pi91 = 3.14\nunion IntOrFloat91 { i: long; f: float; }\ntypedef Vectors91 = Vector[1+2]\nfunc f91() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f91() { enum E91 { A, B, C } return; }\nfunc f91() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i92: long = cast(long*, 42)\nvar x92: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector92 { x, y: float; }\nvar v92 = Vector{x = 1.0, y = -1.0}\nvar v92: Vector = {1.0, -1.0}\nconst n92 = sizeof(:long*[16])\nconst n92 = sizeof(1+2)\nvar x92 = b92 == 1 ? 1+2 : 3-4\nfunc fact92(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact92(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo92 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f92(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color92 { RED = 3, GREEN, BLUE = 0 }\nconst pi92 = 3.14\nunion IntOrFloat92 { i: long; f: float; }\ntypedef Vectors92 = Vector[1+2]\nfunc f92() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f92() { enum E92 { A, B, C } return; }\nfunc f92() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i93: long = cast(long*, 42)\nvar x93: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector93 { x, y: float; }\nvar v93 = Vector{x = 1.0, y = -1.0}\nvar v93: Vector = {1.0, -1.0}\nconst n93 = sizeof(:long*[16])\nconst n93 = sizeof(1+2)\nvar x93 = b93 == 1 ? 1+2 : 3-4\nfunc fact93(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact93(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo93 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f93(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color93 { RED = 3, GREEN, BLUE = 0 }\nconst pi93 = 3.14\nunion IntOrFloat93 { i: long; f: float; }\ntypedef Vectors93 = Vector[1+2]\nfunc f93() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f93() { enum E93 { A, B, C } return; }\nfunc f93() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i94: long = cast(long*, 42)\nvar x94: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector94 { x, y: float; }\nvar v94 = Vector{x = 1.0, y = -1.0}\nvar v94: Vector = {1.0, -1.0}\nconst n94 = sizeof(:long*[16])\nconst n94 = sizeof(1+2)\nvar x94 = b94 == 1 ? 1+2 : 3-4\nfunc fact94(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact94(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo94 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f94(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color94 { RED = 3, GREEN, BLUE = 0 }\nconst pi94 = 3.14\nunion IntOrFloat94 { i: long; f: float; }\ntypedef Vectors94 = Vector[1+2]\nfunc f94() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f94() { enum E94 { A, B, C } return; }\nfunc f94() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i95: long = cast(long*, 42)\nvar x95: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector95 { x, y: float; }\nvar v95 = Vector{x = 1.0, y = -1.0}\nvar v95: Vector = {1.0, -1.0}\nconst n95 = sizeof(:long*[16])\nconst n95 = sizeof(1+2)\nvar x95 = b95 == 1 ? 1+2 : 3-4\nfunc fact95(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact95(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo95 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f95(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color95 { RED = 3, GREEN, BLUE = 0 }\nconst pi95 = 3.14\nunion IntOrFloat95 { i: long; f: float; }\ntypedef Vectors95 = Vector[1+2]\nfunc f95() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f95() { enum E95 { A, B, C } return; }\nfunc f95() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i96: long = cast(long*, 42)\nvar x96: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector96 { x, y: float; }\nvar v96 = Vector{x = 1.0, y = -1.0}\nvar v96: Vector = {1.0, -1.0}\nconst n96 = sizeof(:long*[16])\nconst n96 = sizeof(1+2)\nvar x96 = b96 == 1 ? 1+2 : 3-4\nfunc fact96(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact96(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo96 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f96(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color96 { RED = 3, GREEN, BLUE = 0 }\nconst pi96 = 3.14\nunion IntOrFloat96 { i: long; f: float; }\ntypedef Vectors96 = Vector[1+2]\nfunc f96() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f96() { enum E96 { A, B, C } return; }\nfunc f96() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i97: long = cast(long*, 42)\nvar x97: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector97 { x, y: float; }\nvar v97 = Vector{x = 1.0, y = -1.0}\nvar v97: Vector = {1.0, -1.0}\nconst n97 = sizeof(:long*[16])\nconst n97 = sizeof(1+2)\nvar x97 = b97 == 1 ? 1+2 : 3-4\nfunc fact97(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact97(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo97 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f97(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color97 { RED = 3, GREEN, BLUE = 0 }\nconst pi97 = 3.14\nunion IntOrFloat97 { i: long; f: float; }\ntypedef Vectors97 = Vector[1+2]\nfunc f97() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f97() { enum E97 { A, B, C } return; }\nfunc f97() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i98: long = cast(long*, 42)\nvar x98: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector98 { x, y: float; }\nvar v98 = Vector{x = 1.0, y = -1.0}\nvar v98: Vector = {1.0, -1.0}\nconst n98 = sizeof(:long*[16])\nconst n98 = sizeof(1+2)\nvar x98 = b98 == 1 ? 1+2 : 3-4\nfunc fact98(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact98(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo98 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f98(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color98 { RED = 3, GREEN, BLUE = 0 }\nconst pi98 = 3.14\nunion IntOrFloat98 { i: long; f: float; }\ntypedef Vectors98 = Vector[1+2]\nfunc f98() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f98() { enum E98 { A, B, C } return; }\nfunc f98() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }\nvar i99: long = cast(long*, 42)\nvar x99: char[256] = {1, 2, 3, ['a'] = 4}\nstruct Vector99 { x, y: float; }\nvar v99 = Vector{x = 1.0, y = -1.0}\nvar v99: Vector = {1.0, -1.0}\nconst n99 = sizeof(:long*[16])\nconst n99 = sizeof(1+2)\nvar x99 = b99 == 1 ? 1+2 : 3-4\nfunc fact99(n: long): long { trace(\"fact\"); if (n == 0) { return 1; } else { return n * fact(n-1); } }\nfunc fact99(n: long): long { p := 1; for (i := 1; i <= n; i++) { p *= i; } return p; }\nvar foo99 = a ? a&b + c<<d + e*f == +u-v-w + *g/h(x,y) + -i%k[x] && m <= n*(p+q)/r : 0\nfunc f99(x: long): bool { switch(x) { case 0: case 1: return true; case 2: default: return false; } }\nenum Color99 { RED = 3, GREEN, BLUE = 0 }\nconst pi99 = 3.14\nunion IntOrFloat99 { i: long; f: float; }\ntypedef Vectors99 = Vector[1+2]\nfunc f99() { do { print(42); } while(1); }\ntypedef T = (func(long):long)[16]\nfunc f99() { enum E99 { A, B, C } return; }\nfunc f99() { if (1) { return 1; } else if (2) { return 2; } else { return 3; } }";

        private long _len = -1;
        private char* _p2;
        private char** _ptr;

        internal DeclSet* parse_file()
        {
            var buf = PtrBuffer.GetPooledBuffer();
            try
            {
                while (!is_token(TOKEN_EOF))
                {
                    var decl = parse_decl();
                    assert(decl != null);
                    buf->Add(decl);
                }

                return declset_new((Decl**) buf->_begin, buf->count);
            }
            finally
            {
                buf->Release();
            }
        }

        private Typespec* parse_type_func()
        {
            var buf = PtrBuffer.GetPooledBuffer();
            var pos = token.pos;
            try
            {
                expect_token(TOKEN_LPAREN);
                if (!is_token(TOKEN_RPAREN))
                {
                    buf->Add(parse_type());
                    while (match_token(TOKEN_COMMA)) buf->Add(parse_type());
                }

                expect_token(TOKEN_RPAREN);
                Typespec* ret = null;
                if (match_token(TOKEN_COLON)) ret = parse_type();

                return typespec_func(pos, (Typespec**) buf->_begin, buf->count,
                    ret); // ast_dup(buf, buf->count * PTR_SIZE)
            }
            finally
            {
                buf->Release();
            }
        }

        private Typespec* parse_type_base()
        {
            var pos = token.pos;
            if (is_token(TOKEN_NAME))
            {
                var name = token.name;
                next_token();
                return typespec_name(pos, name);
            }

            if (match_keyword(func_keyword)) return parse_type_func();

            if (match_token(TOKEN_LPAREN))
            {
                var type = parse_type();
                expect_token(TOKEN_RPAREN);
                return type;
            }

            fatal_syntax_error("Unexpected token {0} in type", token_info());
            return null;
        }

        private Typespec* parse_type()
        {
            var pos = token.pos;
            var type = parse_type_base();
            while (is_token(TOKEN_LBRACKET) || is_token(TOKEN_MUL))
                if (match_token(TOKEN_LBRACKET))
                {
                    Expr* expr = null;
                    if (!is_token(TOKEN_RBRACKET)) expr = parse_expr();

                    expect_token(TOKEN_RBRACKET);
                    type = typespec_array(pos, type, expr);
                }
                else
                {
                    assert(is_token(TOKEN_MUL));
                    next_token();
                    type = typespec_ptr(pos, type);
                }

            return type;
        }

        private CompoundField parse_expr_compound_field()
        {
            var pos = token.pos;
            if (match_token(TOKEN_LBRACKET))
            {
                var index = parse_expr();
                expect_token(TOKEN_RBRACKET);
                expect_token(TOKEN_ASSIGN);
                return new CompoundField {pos = pos, kind = FIELD_INDEX, init = parse_expr(), index = index};
            }

            var expr = parse_expr();
            if (match_token(TOKEN_ASSIGN))
            {
                if (expr->kind != EXPR_NAME)
                    fatal_syntax_error("Named initializer in compound literal must be preceded by field name");
                return new CompoundField {pos = pos, kind = FIELD_NAME, init = parse_expr(), name = expr->name};
            }

            return new CompoundField {pos = pos, kind = FIELD_DEFAULT, init = expr};
        }

        private Expr* parse_expr_compound(Typespec* type)
        {
            var pos = token.pos;
            expect_token(TOKEN_LBRACE);
            var buf = Buffer<CompoundField>.Create();
            ; // Expr**
            while (!is_token(TOKEN_RBRACE))
            {
                buf.Add(parse_expr_compound_field());
                if (!match_token(TOKEN_COMMA))
                {
                    break;
                }
            }

            expect_token(TOKEN_RBRACE);
            return expr_compound(pos, type, buf, buf.count);
        }

        private Expr* parse_expr_operand()
        {
            var pos = token.pos;
            if (is_token(TOKEN_INT))
            {
                var val = token.int_val;
                next_token();
                return expr_int(pos, val);
            }

            if (is_token(TOKEN_FLOAT))
            {
                var val = token.float_val;
                next_token();
                return expr_float(pos, val);
            }

            if (is_token(TOKEN_STR))
            {
                var val = token.str_val;
                next_token();
                return expr_str(pos, val);
            }

            if (is_token(TOKEN_NAME))
            {
                var name = token.name;
                next_token();
                if (is_token(TOKEN_LBRACE))
                    return parse_expr_compound(typespec_name(pos, name));
                return expr_name(pos, name);
            }

            if (match_keyword(sizeof_keyword))
            {
                expect_token(TOKEN_LPAREN);
                if (match_token(TOKEN_COLON))
                {
                    var type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    return expr_sizeof_type(pos, type);
                }

                var expr = parse_expr();
                expect_token(TOKEN_RPAREN);
                return expr_sizeof_expr(pos, expr);
            }

            if (is_token(TOKEN_LBRACE)) return parse_expr_compound(null);

            if (match_token(TOKEN_LPAREN))
            {
                if (match_token(TOKEN_COLON))
                {
                    var type = parse_type();
                    expect_token(TOKEN_RPAREN);
                    if (is_token(TOKEN_LBRACE))
                        return parse_expr_compound(type);
                    return expr_cast(pos, type, parse_expr_unary());
                }

                var expr = parse_expr();
                expect_token(TOKEN_RPAREN);
                return expr;
            }

            fatal_syntax_error("Unexpected token {0} in expression", token_info());
            return null;
        }

        private Expr* parse_expr_base()
        {
            var pos = token.pos;
            var expr = parse_expr_operand();
            while (is_token(TOKEN_LPAREN) || is_token(TOKEN_LBRACKET) || is_token(TOKEN_DOT))
                if (match_token(TOKEN_LPAREN))
                {
                    var buf = PtrBuffer.GetPooledBuffer();
                    
                    try
                    {
                        if (!is_token(TOKEN_RPAREN))
                        {
                            buf->Add(parse_expr());
                            while (match_token(TOKEN_COMMA)) buf->Add(parse_expr());
                        }

                        expect_token(TOKEN_RPAREN);
                        expr = expr_call(pos, expr, (Expr**) buf->_begin, buf->count);
                    }
                    finally
                    {
                        buf->Release();
                    }
                }
                else if (match_token(TOKEN_LBRACKET))
                {
                    var index = parse_expr();
                    expect_token(TOKEN_RBRACKET);
                    expr = expr_index(pos, expr, index);
                }
                else
                {
                    assert(is_token(TOKEN_DOT));
                    next_token();
                    var field = token.name;
                    expect_token(TOKEN_NAME);
                    expr = expr_field(pos, expr, field);
                }

            return expr;
        }

        private bool is_unary_op()
        {
            return is_token(TOKEN_ADD) || is_token(TOKEN_SUB) || is_token(TOKEN_MUL) || is_token(TOKEN_AND) ||
                   is_token(TOKEN_NEG) || is_token(TOKEN_NOT);
        }

        private Expr* parse_expr_unary()
        {
            var pos = token.pos;
            if (is_unary_op())
            {
                var op = token.kind;
                next_token();
                return expr_unary(pos, op, parse_expr_unary());
            }

            return parse_expr_base();
        }

        private bool is_mul_op()
        {
            return TOKEN_FIRST_MUL <= token.kind && token.kind <= TOKEN_LAST_MUL;
        }

        private Expr* parse_expr_mul()
        {
            var pos = token.pos;
            var expr = parse_expr_unary();
            while (is_mul_op())
            {
                var op = token.kind;
                next_token();
                expr = expr_binary(pos, op, expr, parse_expr_unary());
            }

            return expr;
        }

        private bool is_add_op()
        {
            return TOKEN_FIRST_ADD <= token.kind && token.kind <= TOKEN_LAST_ADD;
        }

        private Expr* parse_expr_add()
        {
            var pos = token.pos;
            var expr = parse_expr_mul();
            while (is_add_op())
            {
                var op = token.kind;
                next_token();
                expr = expr_binary(pos, op, expr, parse_expr_mul());
            }

            return expr;
        }

        private bool is_cmp_op()
        {
            return TOKEN_FIRST_CMP <= token.kind && token.kind <= TOKEN_LAST_CMP;
        }

        private Expr* parse_expr_cmp()
        {
            var pos = token.pos;
            var expr = parse_expr_add();
            while (is_cmp_op())
            {
                var op = token.kind;
                next_token();
                expr = expr_binary(pos, op, expr, parse_expr_add());
            }

            return expr;
        }

        private Expr* parse_expr_and()
        {
            var pos = token.pos;
            var expr = parse_expr_cmp();
            while (match_token(TOKEN_AND_AND)) expr = expr_binary(pos, TOKEN_AND_AND, expr, parse_expr_cmp());

            return expr;
        }

        private Expr* parse_expr_or()
        {
            var pos = token.pos;
            var expr = parse_expr_and();
            while (match_token(TOKEN_OR_OR)) expr = expr_binary(pos, TOKEN_OR_OR, expr, parse_expr_and());

            return expr;
        }

        private Expr* parse_expr_ternary()
        {
            var pos = token.pos;
            var expr = parse_expr_or();
            if (match_token(TOKEN_QUESTION))
            {
                var then_expr = parse_expr_ternary();
                expect_token(TOKEN_COLON);
                var else_expr = parse_expr_ternary();
                expr = expr_ternary(pos, expr, then_expr, else_expr);
            }

            return expr;
        }

        private Expr* parse_expr()
        {
            return parse_expr_ternary();
        }

        private Expr* parse_paren_expr()
        {
            expect_token(TOKEN_LPAREN);
            var expr = parse_expr();
            expect_token(TOKEN_RPAREN);
            return expr;
        }

        private StmtList parse_stmt_block()
        {
            expect_token(TOKEN_LBRACE);
            var pos = token.pos;

            var buf = PtrBuffer.GetPooledBuffer();
            try
            {
                while (!is_token_eof() && !is_token(TOKEN_RBRACE)) buf->Add(parse_stmt());

                expect_token(TOKEN_RBRACE);
                return stmt_list(pos, (Stmt**) buf->_begin, buf->count);
            }
            finally
            {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_if(SrcPos pos)
        {
            var cond = parse_paren_expr();
            var then_block = parse_stmt_block();
            var else_block = default(StmtList);

            var buf = PtrBuffer.GetPooledBuffer();
            try
            {
                while (match_keyword(else_keyword))
                {
                    if (!match_keyword(if_keyword))
                    {
                        else_block = parse_stmt_block();
                        break;
                    }

                    var elseif_cond = parse_paren_expr();
                    var elseif_block = parse_stmt_block();
                    var elif = (ElseIf*) xmalloc(sizeof(ElseIf));
                    elif->cond = elseif_cond;
                    elif->block = elseif_block;
                    buf->Add(elif);
                }

                return stmt_if(pos, cond, then_block, (ElseIf**) buf->_begin, buf->count,
                    else_block);
            }
            finally
            {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_while(SrcPos pos)
        {
            var cond = parse_paren_expr();
            return stmt_while(pos, cond, parse_stmt_block());
        }

        private Stmt* parse_stmt_do_while(SrcPos pos)
        {
            var block = parse_stmt_block();
            if (!match_keyword(while_keyword))
            {
                fatal_syntax_error("Expected 'while' after 'do' block");
                return null;
            }

            var stmt = stmt_do_while(pos, parse_paren_expr(), block);
            expect_token(TOKEN_SEMICOLON);
            return stmt;
        }

        private bool is_assign_op()
        {
            return TOKEN_FIRST_ASSIGN <= token.kind && token.kind <= TOKEN_LAST_ASSIGN;
        }

        private Stmt* parse_simple_stmt()
        {
            var pos = token.pos;
            var expr = parse_expr();
            Stmt* stmt;
            if (match_token(TOKEN_COLON_ASSIGN))
            {
                if (expr->kind != EXPR_NAME)
                {
                    fatal_syntax_error(":= must be preceded by a name");
                    return null;
                }

                stmt = stmt_init(pos, expr->name, parse_expr());
            }
            else if (is_assign_op())
            {
                var op = token.kind;
                next_token();
                stmt = stmt_assign(pos, op, expr, parse_expr());
            }
            else if (is_token(TOKEN_INC) || is_token(TOKEN_DEC))
            {
                var op = token.kind;
                next_token();
                stmt = stmt_assign(pos, op, expr, null);
            }
            else
            {
                stmt = stmt_expr(pos, expr);
            }

            return stmt;
        }

        private Stmt* parse_stmt_for(SrcPos pos)
        {
            expect_token(TOKEN_LPAREN);
            Stmt* init = null;
            if (!is_token(TOKEN_SEMICOLON)) init = parse_simple_stmt();

            expect_token(TOKEN_SEMICOLON);
            Expr* cond = null;
            if (!is_token(TOKEN_SEMICOLON)) cond = parse_expr();

            expect_token(TOKEN_SEMICOLON);
            Stmt* next = null;
            if (!is_token(TOKEN_RPAREN))
            {
                next = parse_simple_stmt();
                if (next->kind == STMT_INIT) syntax_error("Init statements not allowed in for-statement's next clause");
            }

            expect_token(TOKEN_RPAREN);
            return stmt_for(pos, init, cond, next, parse_stmt_block());
        }

        private SwitchCase parse_stmt_switch_case()
        {
            var buf = PtrBuffer.GetPooledBuffer();
            try
            {
                var is_default = false;
                while (is_keyword(case_keyword) || is_keyword(default_keyword))
                {
                    if (match_keyword(case_keyword))
                    {
                        buf->Add(parse_expr());
                        while (match_token(TOKEN_COMMA)) {
                            buf->Add(parse_expr());
                        }
                    }
                    else
                    {
                        assert(is_keyword(default_keyword));
                        next_token();
                        if (is_default) syntax_error("Duplicate default labels in same switch clause");

                        is_default = true;
                    }

                    expect_token(TOKEN_COLON);
                }

                var pos = token.pos;

                var buf2 = PtrBuffer.GetPooledBuffer();
                ;
                while (!is_token_eof() && !is_token(TOKEN_RBRACE) && !is_keyword(case_keyword) &&
                       !is_keyword(default_keyword))
                    buf2->Add(parse_stmt());

                var block = new StmtList
                {
                    pos = pos,
                    stmts = (Stmt**) buf2->_begin,
                    num_stmts = buf2->count
                };
                return new SwitchCase
                {
                    exprs = (Expr**) ast_dup(buf->_begin, buf->buf_byte_size),
                    num_exprs = buf->count,
                    is_default = is_default,
                    block = block
                };
            }
            finally
            {
                buf->Release();
            }
        }

        private Stmt* parse_stmt_switch(SrcPos pos)
        {
            var expr = parse_paren_expr();

            var buf = Buffer<SwitchCase>.Create();
            expect_token(TOKEN_LBRACE);
            while (!is_token_eof() && !is_token(TOKEN_RBRACE)) buf.Add(parse_stmt_switch_case());

            expect_token(TOKEN_RBRACE);
            return stmt_switch(pos, expr, buf, buf.count);
        }

        private Stmt* parse_stmt()
        {
            var pos = token.pos;
            if (match_keyword(if_keyword)) return parse_stmt_if(pos);

            if (match_keyword(while_keyword)) return parse_stmt_while(pos);

            if (match_keyword(do_keyword)) return parse_stmt_do_while(pos);

            if (match_keyword(for_keyword)) return parse_stmt_for(pos);

            if (match_keyword(switch_keyword)) return parse_stmt_switch(pos);

            if (is_token(TOKEN_LBRACE)) return stmt_block(pos, parse_stmt_block());

            if (match_keyword(break_keyword))
            {
                expect_token(TOKEN_SEMICOLON);
                return stmt_break(pos);
            }

            if (match_keyword(continue_keyword))
            {
                expect_token(TOKEN_SEMICOLON);
                return stmt_continue(pos);
            }

            if (match_keyword(return_keyword))
            {
                Expr* expr = null;
                if (!is_token(TOKEN_SEMICOLON)) expr = parse_expr();

                expect_token(TOKEN_SEMICOLON);
                return stmt_return(pos, expr);
            }

            var decl = parse_decl_opt();
            if (decl != null) return stmt_decl(pos, decl);

            var stmt = parse_simple_stmt();
            expect_token(TOKEN_SEMICOLON);
            return stmt;
        }

        private char* parse_name()
        {
            var name = token.name;
            expect_token(TOKEN_NAME);
            return name;
        }

        private EnumItem parse_decl_enum_item()
        {
            var pos = token.pos;
            var name = parse_name();
            Expr* init = null;
            if (match_token(TOKEN_ASSIGN)) init = parse_expr();

            return new EnumItem {pos = pos, name = name, init = init};
        }

        private Decl* parse_decl_enum(SrcPos pos)
        {
            var name = parse_name();
            expect_token(TOKEN_LBRACE);
            EnumItem* items = null;
            var buf = Buffer<EnumItem>.Create();
            while (!is_token(TOKEN_RBRACE))
            {
                buf.Add(parse_decl_enum_item());
                if (!match_token(TOKEN_COMMA)) break;
            }

            expect_token(TOKEN_RBRACE);
            return decl_enum(pos, name, buf._begin, buf.count);
        }

        private AggregateItem parse_decl_aggregate_item()
        {
            var buf = PtrBuffer.GetPooledBuffer();
            var pos = token.pos;
            try
            {
                buf->Add(parse_name());
                while (match_token(TOKEN_COMMA)) buf->Add(parse_name());

                expect_token(TOKEN_COLON);
                var type = parse_type();
                expect_token(TOKEN_SEMICOLON);
                return new AggregateItem
                {
                    pos = pos,
                    names = (char**) ast_dup(buf->_begin, buf->count * sizeof(char*)),
                    num_names = buf->count,
                    type = type
                };
            }
            finally
            {
                buf->Release();
            }
        }

        private Decl* parse_decl_aggregate(SrcPos pos, DeclKind kind)
        {
            assert(kind == DECL_STRUCT || kind == DECL_UNION);
            var name = parse_name();
            expect_token(TOKEN_LBRACE);

            var buf = Buffer<AggregateItem>.Create();
            while (!is_token_eof() && !is_token(TOKEN_RBRACE)) buf.Add(parse_decl_aggregate_item());

            expect_token(TOKEN_RBRACE);
            return decl_aggregate(pos, kind, name, buf, buf.count);
        }

        private Decl* parse_decl_var(SrcPos pos)
        {
            var name = parse_name();
            if (match_token(TOKEN_ASSIGN)) return decl_var(pos, name, null, parse_expr());

            if (match_token(TOKEN_COLON))
            {
                var type = parse_type();
                Expr* expr = null;
                if (match_token(TOKEN_ASSIGN)) expr = parse_expr();

                return decl_var(pos, name, type, expr);
            }

            fatal_syntax_error("Expected : or = after var, got {0}", token_info());
            return null;
        }

        private Decl* parse_decl_const(SrcPos pos)
        {
            var name = parse_name();
            expect_token(TOKEN_ASSIGN);
            return decl_const(pos, name, parse_expr());
        }

        private Decl* parse_decl_typedef(SrcPos pos)
        {
            var name = parse_name();
            expect_token(TOKEN_ASSIGN);
            return decl_typedef(pos, name, parse_type());
        }

        private FuncParam parse_decl_func_param()
        {
            var pos = token.pos;
            var name = parse_name();
            expect_token(TOKEN_COLON);
            var type = parse_type();
            return new FuncParam {pos = pos, name = name, type = type};
        }

        private Decl* parse_decl_func(SrcPos pos)
        {
            var name = parse_name();
            expect_token(TOKEN_LPAREN);
            FuncParam* @params = null;
            var buf = Buffer<FuncParam>.Create();
            if (!is_token(TOKEN_RPAREN))
            {
                buf.Add(parse_decl_func_param());
                while (match_token(TOKEN_COMMA)) buf.Add(parse_decl_func_param());
            }

            expect_token(TOKEN_RPAREN);
            Typespec* ret_type = null;
            if (match_token(TOKEN_COLON)) ret_type = parse_type();

            var block = parse_stmt_block();
            return decl_func(pos, name, buf, buf.count, ret_type, block);
        }

        private Decl* parse_decl_opt()
        {
            var pos = token.pos;
            if (match_keyword(enum_keyword))
                return parse_decl_enum(pos);
            if (match_keyword(struct_keyword))
                return parse_decl_aggregate(pos, DECL_STRUCT);
            if (match_keyword(union_keyword))
                return parse_decl_aggregate(pos, DECL_UNION);
            if (match_keyword(var_keyword))
                return parse_decl_var(pos);
            if (match_keyword(const_keyword))
                return parse_decl_const(pos);
            if (match_keyword(typedef_keyword))
                return parse_decl_typedef(pos);
            if (match_keyword(func_keyword))
                return parse_decl_func(pos);
            return null;
        }

        private Decl* parse_decl()
        {
            var decl = parse_decl_opt();
            if (decl == null) fatal_syntax_error("Expected declaration keyword, got {0}", token_info());

            return decl;
        }

        internal void init_parse_test()
        {
            _p2 = code1.ToPtr();
            _ptr = (char**) xmalloc(PTR_SIZE * decls.Length);
            _len = decls.Length;

            for (var i = 0; i < _len; i++)
            {
                var it = decls[i].ToPtr();
                *(_ptr + i) = it;
            }
        }

        internal void parse_test_and_print()
        {
            init_parse_test();
            Console.WriteLine();
            init_stream(_p2);
            var ds = parse_file();
            for (var i = 0; i < _len; i++)
            {
                var it = *(_ptr + i);
                init_stream(it);
                var decl = parse_decl();
                print_decl(decl);
                Console.WriteLine();
                decl = ds->decls[i];
                print_decl(decl);
                Console.WriteLine();
            }

            //var txt = File.ReadAllText("parser.output.txt");
            //var txt2 = sb.ToString();
            //assert(txt == txt2);
            //use_print_buf = false;
        }


        internal void parse_test()
        {
            for (var i = 0; i < _len; i++)
            {
                var it = *(_ptr + i);
                init_stream(it);
                var decl = parse_decl();
            }
        }

        internal void parse_test2()
        {
            init_stream(_p2);
            var ds = parse_file();
        }
    }
}