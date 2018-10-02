using System;
using System.Diagnostics;

namespace MLang
{

    class Error {

        [Conditional("DEBUG")]
        public static void assert(bool b) => Debug.Assert(b);


        public static void fatal(string format, params object[] pmz)
        {
            Console.WriteLine("FATAL: " + format, pmz);
            Environment.Exit(1);
        }


        public static void syntax_error(string format, params object[] pmz) 
        {
            Console.WriteLine("Syntax Error: " + format, pmz);
        }


        public static void fatal_syntax_error(string format, params object[] pmz)
        {
            syntax_error(format, pmz);
            Environment.Exit(1);
        }
      
    }
}
