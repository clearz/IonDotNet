using System;
using System.Collections.Generic;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToCharArrayPointer(this Dictionary<int, string> dict, char*** ptr){
            *ptr = (char**)Marshal.AllocHGlobal(dict.Count * sizeof(char**));

            var keys = dict.Keys.ToArray();
            for (int i=0; i < dict.Count; i++) {
                var kVal = keys[i];
                var sVal = dict[kVal];
                sVal.ToPtr(*ptr, kVal);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToPtr(this string s, char** cptr, int pos = 0)
        {
            *(cptr + pos) = s.ToPtr();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* ToPtr(this string s) {
            //var size = s.Length * 2 + 2;
            //char* ptr = (char*) Marshal.AllocHGlobal(size);
            fixed (char* c = s) return c;
            //Unsafe.CopyBlock(ptr, c, (uint)size);
            //return ptr;
        }
    }
}
