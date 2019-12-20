using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IonLangManaged
{
    unsafe partial class Ion
    {
        void initialise() {

            resolved_sym_map = new Dictionary<object, Sym>();
            resolved_val_map = new Dictionary<Expr, Val>();
            package_map = new Dictionary<string, Package>();
            resolved_expected_type_map = new Dictionary<Expr, Type>();
            resolved_type_map = new Dictionary<object, Type>();
            package_map = new Dictionary<string, Package>();
            pointer_promo_map = new Dictionary<Expr, Type>();
            labels_map = new Dictionary<string, Label>();
            reachable_map = new Dictionary<Type, SymReachable>();
            type_conv_map = new Dictionary<Expr, Type>();
            local_syms = new Sym[MAX_LOCAL_SYMS];
            local_sym_pos = 0;
            implicit_any_list = new List<Expr>();
            reachable_syms = new List<Sym>();
            package_list = new List<Package>();
            sorted_syms = new List<Sym>(256);
            decl_note_names = new List<string>();
            labels = new List<Label>();
            type_allocator = new Type();
            type_allocator_ptr = new Type();
            init_compiler();
            init_chars();
            inited = true;
        }
        public static long MAX(long a, long b) {
            return a > b ? a : b;
        }

        public static bool IS_POW2(long x) {
            return x != 0 && (x & (x - 1)) == 0;
        }

        public static long ALIGN_UP(long n, long a) {
            return (n + a - 1) & ~(a - 1);
        }

        #region Std Functions


        public static bool isalpha(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        public static bool isspace(char c) {
            return c == ' ' || c == '\n' || c == '\r' || c == '\t' || c == '\v';
        }

        public static bool islower(char c) {
            return c >= 'a' && c <= 'z';
        }

        public static bool isupper(char c) {
            return c >= 'A' && c <= 'Z';
        }

        public static bool isdigit(char c) {
            return c >= '0' && c <= '9';
        }

        public static bool isprint(char c) {
            return c >= ' ' && c <= '~';
        }

        public static char tolower(char c) {
            return c >= 'a' ? c : (char)(c + 32);
        }

        //public static void strcpy(string c1, string c2) {
        //    while ((c1++ = c2++) != 0)
        //        ;
        //}
        //public static string strstr(string c1) {
        //    uint i = (uint)strlen(c1);
        //    string c = xmalloc<char>();
        //    Unsafe.CopyBlock(c, c1, i);
        //    return c;
        //}

        //public static int strcmp(string c1, string c2) {
        //    while (c1++ == c2++)
        //        if (c1 == '\0')
        //            return 0;

        //    return 1;
        //}

        //public static string strcat2(string c1, string c2) {
        //    var s = strlen(c1) + strlen(c2)+ 1;
        //    string rtn = xmalloc<char>(s);
        //    string p = rtn;

        //    while (c1 != '\0')
        //        p++ = c1++;
        //    while (c2 != '\0')
        //        p++ = c2++;

        //    p = '\0';
        //    return rtn;
        //}


        #endregion

        void Normalize(ref Span<char> s) {
            int i = -1;
            while (s[++i] != '\0')
                if (s[i] == '\\')
                    s[i] = '/';

            s[i++] = '/';
            s = s.Slice(0, i);

        }

        void path_normalize(char* path) {
            char* ptr;
            for (ptr = path; *ptr != 0; ptr++) {
                if (*ptr == '\\') {
                    *ptr = '/';
                }
            }
            if (ptr != path && ptr[-1] == '/') {
                ptr[-1] = '\0';
            }
        }

        bool str_islower(ReadOnlySpan<char> str) {
            int i = 0;
            while (str.Length > i) {
                if (isalpha(str[i]) && !islower(str[i])) {
                    return false;
                }
                i++;
            }
            return true;
        }

        public static int copy_to_pos(char* c1, char* c2, int n = 0) {
            while ((c1[n] = *c2++) != 0)
                n++;
            return n;
        }

        void print(string format, params object[] objs) {
            Console.WriteLine(format, objs);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Val
    {
        [FieldOffset(0)] public bool b;
        [FieldOffset(0)] public char c;
        [FieldOffset(0)] public byte uc;
        [FieldOffset(0)] public sbyte sc;
        [FieldOffset(0)] public short s;
        [FieldOffset(0)] public ushort us;
        [FieldOffset(0)] public int i;
        [FieldOffset(0)] public uint u;
        [FieldOffset(0)] public int l;
        [FieldOffset(0)] public uint ul;
        [FieldOffset(0)] public long ll;
        [FieldOffset(0)] public ulong ull;
        [FieldOffset(0)] public void* p;
    }
}