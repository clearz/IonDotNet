using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Console;

namespace Lang
{
    unsafe partial class Ion
    {
        static void Main() {

            var ion = new Ion();
            ion.lex_init();
          //  ion.lex_test();
          //  ion.print_test();
            ion.resolve_test();
          //  ion.parse_test_and_print();
            ReadKey();
            int iterations = 100000;
            Console.WriteLine("{0} iterations\n", iterations);
            int it = iterations;
            Timer t = new Timer();
            ion.init_parse_test();
            t.Start();
            //  CachedPtrType type = default;
            while (--it > 0)
            {
                //ion.resolve_test();
                ion.parse_test();
            }

            t.Stop();
            Console.WriteLine("Time Parsing: {0} ns", t.Duration(iterations));
            ReadKey();
        }
    }

    internal class Timer
    {
        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public bool Running { private set; get; }

        public Timer()
        {
            if (QueryPerformanceFrequency(out _frequency) == false)
            {
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            QueryPerformanceCounter(out _start);
            Running = true;
        }

        public long Stop()
        {
            QueryPerformanceCounter(out _stop);
            Running = false;
            return _stop - _start;
        }

        public decimal Duration(long iterations = 1) => (_stop - _start) * _multiplier / _frequency / iterations;

        private long _start;
        private long _stop;
        private readonly long _frequency;
        private readonly decimal _multiplier = new decimal(1.0e9);
    }
}
