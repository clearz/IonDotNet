using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lang
{
    #region Typedefs
#if X64
    using size_t = Int64;
#else
	using size_t = Int32;
#endif
    #endregion
    public unsafe partial class Ion
    {
        [DebuggerHidden]
        static void fatal(string format, params object[] pmz)
        {
            Console.WriteLine("FATAL: " + format, pmz);
#if DEBUG
            assert(false);
#endif
            Console.WriteLine("Exiting...");
            Thread.Sleep(2000);
            Environment.Exit(1);
        }

        [DebuggerHidden]
        void error(SrcLoc loc, string format, params object[] pmz)
        {
            Console.Write("{0}({1}): ", new string(loc.name), loc.line);
#if DEBUG
            assert(false);
#endif
            Console.WriteLine(format, pmz);
        }

        [DebuggerHidden]
        void syntax_error(string format, params object[] pmz) => error(new SrcLoc {name= src_name,line= src_line }, format, pmz);


        [DebuggerHidden]
        void fatal_syntax_error(string format, params object[] pmz)
        {
            syntax_error(format, pmz);
#if DEBUG
            assert(false);
#endif
            Console.WriteLine("Exiting...");
            Thread.Sleep(2000);
            Environment.Exit(1);
        }


        [Conditional("DEBUG")]
        [DebuggerHidden]
        private static void assert(bool b) => Debug.Assert(b);
    }



    [StructLayout(LayoutKind.Sequential, Size = 16)]
    unsafe struct SrcLoc
    {
        public char* name;
        public int line;
    }


}

