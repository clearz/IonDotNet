﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using DotNetCross.Memory;

namespace Lang
{
    static unsafe class Extensions
    {
        
        public static void* ToArrayPtr<T>(this T[] objs) where T : unmanaged {
            int size_of = Marshal.SizeOf<T>();
            int size = size_of * objs.Length;    
            byte* ptr = (byte*)Ion.xmalloc(size);
            for (int i = 0, j = 0; i < size; i += size_of, j++) {
                var obj = objs[j];
                Unsafe.CopyBlock(ptr+i, Ion.AsPointer(ref obj), (uint)size_of);
            }

            return ptr;
        }
        
        public static void ToCharArrayPointer(this Dictionary<TokenKind, string> dict, char*** ptr){
            *ptr = (char**)Ion.xmalloc(dict.Count * sizeof(char**));
            var keys = dict.Keys.ToArray();
            for (int i =0; i < (long)dict.Count; i++) {
                var kVal = keys[i];
                var sVal = dict[kVal];
                sVal.ToPtr(*ptr, (long)kVal);
            }
        }

        
        private static void ToPtr(this string s, char** cptr, long pos = 0)
        {
            *(cptr + pos) = s.ToPtr();
        }

        public static char* ToPtr(this string s)
        {
            fixed (char* c = s)
                return c;
        }

        public static char* ToPtr2(this string s)
        {
            char* stream = Ion.xmalloc<char>(s.Length + 1);
            fixed (char* c = s)
                Unsafe.CopyBlock(stream, c, (uint)s.Length << 1);
            stream[s.Length] = '\0';
            return stream;
        }
    }


    internal class Timer
    {
        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private Timer()
        {
            if (QueryPerformanceFrequency(out _frequency) == false)
            {
                throw new Win32Exception();
            }
        }


        public static void Time(long iterations, Action parse_test)
        {
            var sw = new Stopwatch();
            Console.WriteLine("{0} iterations", iterations);
            long it = iterations;


            sw.Start();
            do parse_test(); while (--it > 0);
            sw.Stop();

            decimal duration = (sw.ElapsedTicks) * 100 / (decimal)iterations;
            decimal sec = (decimal)sw.Elapsed.TotalSeconds;
            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sec, duration);
        }

        public static void Time2(long iterations, Action parse_test)
        {
            var sw = new Timer();
            Console.WriteLine("{0} iterations", iterations);
            long it = iterations;


            sw.Start();
            do parse_test(); while (--it > 0);
            sw.Stop();

            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sw.DurationSeconds(), sw.Duration((long)iterations));
        }

        public void Start() => QueryPerformanceCounter(out _start);

        public void Stop() => QueryPerformanceCounter(out _stop);

        public long Duration(long iterations = 1) => (_stop - _start) * _multiplier / _frequency / iterations;
        public decimal DurationSeconds(long iterations = 1) => (_stop - _start) / (decimal)_frequency / iterations;

        private long _start;
        private long _stop;
        private readonly long _frequency;
        private const long _multiplier = 1000000000;
    }
}
