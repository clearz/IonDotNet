using DotNetCross.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lang
{
    internal static unsafe class Extensions
    {
        public static void* ToArrayPtr<T>(this T[] objs) where T : unmanaged {
            var size_of = Marshal.SizeOf<T>();
            var size = size_of * objs.Length;
            var ptr = (byte*) Ion.xmalloc(size);
            for (int i = 0, j = 0; i < size; i += size_of, j++) {
                var obj = objs[j];
                Unsafe.CopyBlock(ptr + i, Ion.AsPointer(ref obj), (uint)size_of);
            }

            return ptr;
        }

        public static void ToCharArrayPointer(this Dictionary<TokenKind, string> dict, char*** ptr) {
            *ptr = (char**)Ion.xmalloc(dict.Count * sizeof(char**));
            var keys = dict.Keys.ToArray();
            for (var i = 0; i < (long)dict.Count; i++) {
                var kVal = keys[i];
                var sVal = dict[kVal];
                sVal.ToPtr(*ptr, (long)kVal);
            }
        }


        private static void ToPtr(this string s, char** cptr, long pos = 0) {
            *(cptr + pos) = s.ToPtr();
        }

        public static char* ToPtr(this string s) {
            var stream = Ion.xmalloc<char>(s.Length + 1);
            fixed (char* c = s) {
                Unsafe.CopyBlock(stream, c, (uint)s.Length << 1);
            }

            stream[s.Length] = '\0';
            return stream;
        }

        public static char* ToPtr2(this string s) {
            var stream = Ion.xmalloc<char>(s.Length + 1);
            fixed (char* c = s) {
                Unsafe.CopyBlock(stream, c, (uint)s.Length << 1);
            }

            stream[s.Length] = '\0';
            return stream;
        }
    }


    internal class Timer
    {
        private const long _multiplier = 1000000000;
        private readonly long _frequency;

        private long _start;
        private long _stop;

        private Timer() {
            if (QueryPerformanceFrequency(out _frequency) == false)
                throw new Win32Exception();
        }

        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);


        public static void Time(long iterations, Action parse_test) {
            var sw = new Stopwatch();
            Console.WriteLine("{0} iterations", iterations);
            var it = iterations;


            sw.Start();
            do {
                parse_test();
            } while (--it > 0);

            sw.Stop();

            var duration = sw.ElapsedTicks * 100 / (decimal) iterations;
            var sec = (decimal) sw.Elapsed.TotalSeconds;
            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sec, duration);
        }

        public static void Time2(long iterations, Action parse_test) {
            var sw = new Timer();
            Console.WriteLine("{0} iterations", iterations);
            var it = iterations;


            sw.Start();
            do {
                parse_test();
            } while (--it > 0);

            sw.Stop();

            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sw.DurationSeconds(),
                sw.Duration(iterations));
        }

        public void Start() {
            QueryPerformanceCounter(out _start);
        }

        public void Stop() {
            QueryPerformanceCounter(out _stop);
        }

        public long Duration(long iterations = 1) {
            return (_stop - _start) * _multiplier / _frequency / iterations;
        }

        public decimal DurationSeconds(long iterations = 1) {
            return (_stop - _start) / (decimal)_frequency / iterations;
        }
    }
}