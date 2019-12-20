using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IonLangManaged
{
    unsafe static class Extentions
    {

        public static int Hash(Type[] @params, Type ret, bool intrinsic, Type varargs_type) {
            unchecked {
                long hash = 0x7f51afd7ed558ccd;
                for (int i = 0; i < @params.Length; i++) {
                    var t = @params[i];
                    hash = hash ^ t.GetHashCode();
                    hash *= (int)t.kind;
                }
                hash = hash ^ ret.GetHashCode();
                hash *= (int)ret.kind;

                hash ^= MurmurHash3Mixer(intrinsic ? 1ul : 0) ^ MurmurHash3Mixer((ulong)@params.Length);
                if (varargs_type != null) {
                    hash ^= MurmurHash3Mixer((ulong)varargs_type.kind);
                    hash *= (int)varargs_type.kind;
                }
                return hash.GetHashCode();
            }

            static int MurmurHash3Mixer(ulong key) {
                key = ~key;
                key ^= (key >> 33);
                key *= 0xff51afd7ed558ccd;
                key ^= (key >> 33);
                key *= 0xc4ceb9fe1a85ec53;
                key ^= (key >> 33);

                return key.GetHashCode();
            }

        }

        [DebuggerHidden]
        public static char* ToPtr(this string s) {
            var stream =  (char*)Marshal.AllocHGlobal(sizeof(char) * (s.Length + 1));
            fixed (char* c = s) {
                Unsafe.CopyBlock(stream, c, (uint)s.Length << 1);
            }

            stream[s.Length] = '\0';
            return stream;
        }

        #region Numeric Conversion

        private static char[] ZERO = {'0'};
        private static char[] buf = new char[24];
        static char[] num_vals = { '0', '1',  '2',  '3',  '4',  '5',  '6',  '7',  '8',  '9',  'A',  'B',  'C',  'D',  'E',  'F' };

        public static Span<char> ToSpan(this int i, int @base = 10) {
            if (i == 0)
                return ZERO;

            int pos = 24;
            Span<char> tmp = buf;

            var j = i;
            while (j != 0) {
                tmp[--pos] = num_vals[j % @base];
                j /= @base;
            }
            if (i < 0)
                tmp[--pos] = '-';

            return tmp.Slice(pos);
        }

        public static Span<char> ToSpan(this uint i) {
            if (i == 0)
                return ZERO;

            int pos = 24;
            Span<char> tmp = buf;

            while (i != 0) {
                tmp[--pos] = (char)((i % 10) + 48);
                i /= 10;
            }

            return tmp.Slice(pos);

        }

        public static Span<char> ToSpan(this long i) {
            if (i == 0)
                return ZERO;

            int pos = 24;
            Span<char> tmp = buf;

            var j = i;
            while (j != 0) {
                if (i < 0)
                    tmp[--pos] = (char)(-(j % 10) + 48);
                else
                    tmp[--pos] = (char)((j % 10) + 48);
                j /= 10;
            }
            if (i < 0)
                tmp[--pos] = '-';

            return tmp.Slice(pos);

        }

        public static Span<char> ToSpan(this ulong i, ulong @base = 10) {
            if (i == 0)
                return ZERO;

            int pos = 24;
            Span<char> tmp = buf;

            while (i != 0) {
                tmp[--pos] = num_vals[i % @base];
                i /= @base;
            }

            return tmp.Slice(pos);
        }
    }

    #endregion

    internal class Timer
    {
        public static void Time(Action parse_test, long iterations = 1) {
            var sw = new Stopwatch();
            Console.WriteLine("{0} iterations", iterations);
            var it = iterations;

            sw.Start();
            while (0 < it--)
                parse_test();

            sw.Stop();

            var duration = sw.ElapsedTicks * 100 / (decimal) iterations;
            var sec = (decimal) sw.Elapsed.TotalSeconds;
            Console.WriteLine("  TotalTime: {0:#,#.####}, Individual Time: {1:0,0} ns", sec, duration);
        }
    }
}