﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IonLang
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

        public static char* ToPtr(this string s) {
            var stream = Ion.xmalloc<char>(s.Length + 1);
            fixed (char* c = s) {
                Unsafe.CopyBlock(stream, c, (uint)s.Length << 1);
            }

            stream[s.Length] = '\0';
            return stream;
        }
        public static char* ToPtr(this string s, out int len) {
            len = s.Length;
            var stream = Ion.xmalloc<char>(len+ 1);
            fixed (char* c = s) {
                Unsafe.CopyBlock(stream, c, (uint)len << 1);
            }

            stream[s.Length] = '\0';
            return stream;
        }

        public static char* ToPtr2(this string s) {
            fixed (char* c = s) {
                return c;
            }
        }



        #region Numeric Conversion

        static char* ZERO = "0".ToPtr();
        static char* tmp = Ion.xmalloc<char>(24);
        static char[] num_vals = { '0', '1',  '2',  '3',  '4',  '5',  '6',  '7',  '8',  '9',  'A',  'B',  'C',  'D',  'E',  'F' };
        public static char* itoa(this int i, int @base = 10) {
            if (i == 0)
                return ZERO;
            int pos = 11;

            tmp[pos] = '\0';
            var j = i;
            while (j != 0) {
                tmp[--pos] = num_vals[j % @base];
                j /= @base;
            }
            if (i < 0)
                tmp[--pos] = '-';
            return tmp + pos;

        }
        public static char* itoa(this uint i) {
            if (i == 0) return ZERO;
            int pos = 11;

            tmp[pos] = '\0';
            var j = i;
            while (j != 0) {
                tmp[--pos] = (char)((j % 10) + 48);
                j /= 10;
            }
            return tmp + pos;

        }
        public static char* itoa(this long i) {
            if (i == 0)
                return ZERO;
            int pos = 11;

            tmp[pos] = '\0';
            var j = i;
            while (j != 0) {
                if (i < 0)
                    tmp[--pos] = (char)(-(j % 10) + 48);
                else
                    tmp[--pos] = (char)((j % 10) + 48);
                j /= 10;
            }
            if (i < 0)
                tmp[--pos] = '-';
            return tmp + pos;

        }

        public static char* itoa(this ulong i, ulong @base = 10) {
            if (i == 0)
                return ZERO;
            int pos = 11;

            tmp[pos] = '\0';
            var j = i;
            while (j != 0) {
                tmp[--pos] = num_vals[j % @base];
                j /= @base;
            }
            return tmp + pos;

        }
        #endregion;
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


        public static void Time(Action parse_test, long iterations = 1) {
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