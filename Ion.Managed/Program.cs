using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Console;

namespace MLang
{
#if X64
    using usize_t = UInt64;
#else
    using usize_t = UInt32;
#endif
    public class Ion
    {
        public void Run(int iterations) {
            Console.WriteLine("{0} iterations\n", iterations);
            int it = iterations;
            Timer t = new Timer();
            t.Start();

            var parser = new Parser();
            parser.parse_test();
            //while (--it > 0)
            //{
            //    // Resolve.resolve_test();
            //    parser.parse_test();
            //}

            t.Stop();
            Console.WriteLine("Time Parsing: {0} ns", t.Duration(iterations));
        }
        static void Main()
        {
            int iterations = 100000;
            var mi = new Ion();
            mi.Run(iterations);
            //var lexer = new Lexer();
            //var resolve = new Resolve();
            //var print = new Print();
            // WriteLine("Press a key to run lex tests");
            //// ReadKey();
            // Clear();
            // lexer.lex_init();
            // lexer.lex_test();
            // WriteLine("Press a key to run print tests");
            // //ReadKey();
            // Clear();
            // print.print_test();
            // WriteLine("Press a key to run parse tests");
            // //ReadKey();
            // Clear();
            // parser.parse_test();
            // WriteLine("Press a key to run resolve tests");
            // ReadKey();
            // Clear();

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
                // Frequency not supported
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

        public decimal Duration(long iterations = 1)
        {
            return (_stop - _start) * _multiplier / _frequency / iterations;
        }

        private long _start;
        private long _stop;
        private readonly long _frequency;
        private readonly decimal _multiplier = new decimal(1.0e9);
    }
}
