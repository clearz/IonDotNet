using System;
using System.Diagnostics;
using System.Threading;

namespace IonLang
{
    public unsafe partial class Ion
    {
        void warning(SrcPos pos, string format, params object[] pmz) {
            if (pos.name == null) {
                pos = pos_builtin;
            }
            Console.Out.WriteLine($"{new string(pos.name)}({pos.line},{pos.col}): warning ION123: {string.Format(format, pmz)}");
        }


        [DebuggerHidden]
        private static void fatal(string format, params object[] pmz) {
            Console.Error.WriteLine("FATAL: " + format, pmz);
            Exit();
        }
        [DebuggerHidden]
        private static void error(SrcPos pos, string format, params object[] pmz) {
            if (pos.name == null) {
                pos = pos_builtin;
            }
            Console.Error.WriteLine($"{new string(pos.name)}({pos.line},{pos.col}): error ION000: {string.Format(format, pmz)}");
        }

        [DebuggerHidden]
        private static void fatal_error(SrcPos pos, string format, params object[] pmz) {
            error(pos, format, pmz);
            Exit();
        }

        [DebuggerHidden]
        private void error_here(string format, params object[] pmz) {
            error(token.pos, format, pmz);
        }

        [DebuggerHidden]
        private void warning_here(string format, params object[] pmz) {
            warning(token.pos, format, pmz);
        }


        [DebuggerHidden]
        private void fatal_error_here(string format, params object[] pmz)
        {
            error_here(format, pmz);
            Exit();
        }


        [Conditional("DEBUG")]
        [DebuggerHidden]
        private static void assert(long l) => assert(l != 0);

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