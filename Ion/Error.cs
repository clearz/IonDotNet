using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lang
{
    public unsafe partial class Ion
    {
        [DebuggerHidden]
        private static void fatal(string format, params object[] pmz)
        {
            Console.WriteLine("FATAL: " + format, pmz);
            assert(false);
            Console.WriteLine("Exiting...");
            Thread.Sleep(2000);
            Environment.Exit(1);
        }

        [DebuggerHidden]
        private void error(SrcPos pos, string format, params object[] pmz)
        {
            Console.Write("{0}({1}): ", new string(pos.name), pos.line);
            assert(false);
            Console.WriteLine(format, pmz);
        }

        [DebuggerHidden]
        private void syntax_error(string format, params object[] pmz)
        {
            error(token.pos, format, pmz);
        }


        [DebuggerHidden]
        private void fatal_syntax_error(string format, params object[] pmz)
        {
            syntax_error(format, pmz);
            assert(false);
            Console.WriteLine("Exiting...");
            Thread.Sleep(2000);
            Environment.Exit(1);
        }


        [Conditional("DEBUG")]
        [DebuggerHidden]
        private static void assert(bool b)
        {
            Debug.Assert(b);
        }
    }


    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct SrcPos
    {
        public char* name;
        public long line;
    }
}