using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lang
{
    public unsafe partial class Ion
    {
        [DebuggerHidden]
        private static void fatal(string format, params object[] pmz) {
            Console.Error.WriteLine("FATAL: " + format, pmz);
            Exit();
        }
        [DebuggerHidden]
        private static void error(SrcPos pos, string format, params object[] pmz) {
            Console.Error.Write("{0}({1}): error: ", new string(pos.name), pos.line);
            Console.Error.WriteLine(format, pmz);
        }

        [DebuggerHidden]
        private static void fatal_error(SrcPos pos, string format, params object[] pmz) {
            error(pos, format, pmz);
            Exit();
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
            Exit();
        }


        [Conditional("DEBUG")]
        [DebuggerHidden]
        private static void assert(bool b) => Debug.Assert(b);

        [DebuggerHidden]
        private static void Exit(int wait_ms = 2000, int rtn_code = 1) {
            assert(false);
            Console.Error.WriteLine("Exiting...");
            Thread.Sleep(wait_ms);
            Environment.Exit(rtn_code);
        }
    }

}