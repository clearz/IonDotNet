using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DotNetCross.Memory;

namespace Lang
{
    static unsafe class Extensions
    {
        
        public static void* ToArrayPtr<T>(this T[] objs) {
            var size_of = Unsafe.SizeOf<T>();
            var size = size_of * objs.Length;    
            byte* ptr = (byte*)Marshal.AllocHGlobal(size);
            for (int i = 0, j = 0; i < size; i += size_of, j++) {
                var obj = objs[j];
                Unsafe.CopyBlock(ptr+i, Unsafe.AsPointer(ref obj), (uint)size_of);
            }

            return ptr;
        }
        
        public static void ToCharArrayPointer(this Dictionary<int, string> dict, char*** ptr){
            *ptr = (char**)Marshal.AllocHGlobal(dict.Count * sizeof(char**));

            var keys = dict.Keys.ToArray();
            for (int i=0; i < dict.Count; i++) {
                var kVal = keys[i];
                var sVal = dict[kVal];
                sVal.ToPtr(*ptr, kVal);
            }
        }

        
        public static void ToPtr(this string s, char** cptr, int pos = 0)
        {
            *(cptr + pos) = s.ToPtr();
        }
        
        public static char* ToPtr(this string s) {
            //var size = s.Length * 2 + 2;
            //char* ptr = (char*) Marshal.AllocHGlobal(size);
            fixed (char* c = s) return c;
            //Unsafe.CopyBlock(ptr, c, (uint)size);
            //return ptr;
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


        public static void Time(int iterations, Action parse_test)
        {
            var sw = new Stopwatch();
            Console.WriteLine("{0} iterations", iterations);
            int it = iterations;


            sw.Start();
            while (--it >= 0) parse_test();
            sw.Stop();

            decimal duration = (sw.ElapsedTicks) * 100 / (decimal)iterations;
            decimal sec = (decimal)sw.Elapsed.TotalSeconds;
            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sec, duration);
        }

        public static void Time2(int iterations, Action parse_test)
        {
            var sw = new Timer();
            Console.WriteLine("{0} iterations", iterations);
            int it = iterations;


            sw.Start();
            while (--it >= 0) parse_test();
            sw.Stop();

            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sw.DurationSeconds(), sw.Duration(iterations));
        }

        public void Start() => QueryPerformanceCounter(out _start);

        public void Stop() => QueryPerformanceCounter(out _stop);

        public long Duration(long iterations = 1) => (_stop - _start) * _multiplier / _frequency / iterations;
        public decimal DurationSeconds(long iterations = 1) => (decimal)(_stop - _start) / (decimal)_frequency / (decimal)iterations;

        private long _start;
        private long _stop;
        private readonly long _frequency;
        private const long _multiplier = 1000000000;
    }
}
