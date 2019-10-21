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
            Console.Out.WriteLine($"{_S(pos.name)}({pos.line},{pos.col}): warning ION123: {string.Format(format, pmz)}");
        }


        [DebuggerHidden]
        static void fatal(string format, params object[] pmz) {
            Console.Error.WriteLine("FATAL: " + format, pmz);
            Exit();
        }

        [DebuggerHidden]
        static void error(SrcPos pos, string format, params object[] pmz) {
            if (pos.name == null) {
                pos = pos_builtin;
            }
            Console.Error.WriteLine($"{_S(pos.name)}({pos.line},{pos.col}): error ION000: {string.Format(format, pmz)}");
        }

        [DebuggerHidden]
        static void fatal_error(SrcPos pos, string format, params object[] pmz) {
            error(pos, format, pmz);
            Exit();
        }

        [DebuggerHidden]
        void error_here(string format, params object[] pmz) {
            error(token.pos, format, pmz);
            Exit();
        }

        [DebuggerHidden]
        void warning_here(string format, params object[] pmz) {
            warning(token.pos, format, pmz);
        }


        [DebuggerHidden]
        void fatal_error_here(string format, params object[] pmz)
        {
            error_here(format, pmz);
            Exit();
        }


        [Conditional("DEBUG")]
        [DebuggerHidden]
        static void assert(long l = 0) => assert(l != 0);

        [Conditional("DEBUG")]
        [DebuggerHidden]
        static void assert(void* v) => assert(v != null);

        [Conditional("DEBUG")]
        [DebuggerHidden]
        static void assert(bool b) => Debug.Assert(b);

        [DebuggerHidden]
        static void Exit(int wait_ms = 2000, int rtn_code = 1) {
            assert(false);
            Console.Error.WriteLine("Exiting...");
            Thread.Sleep(wait_ms);
            Environment.Exit(rtn_code);
        }
    }

}