using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MLang
{
    static class Common
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MAX(long a, long b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MIN(long a, long b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CLAMP_MIN(long x, long min) => MAX(x, min);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CLAMP_MAX(long x, long max) => MIN(x, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IS_POW2(long x) => x != 0 && (x & x - 1) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ALIGN_DOWN(long n, long a) => n & ~(a - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ALIGN_UP(long n, long a) => n + a - 1 & ~(a - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* ALIGN_DOWN_PTR(void* p, long a) => (void*)ALIGN_DOWN((long)p, a);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void* ALIGN_UP_PTR(void* p, long a) => (void*)ALIGN_UP((long)p, a);
    }
}
