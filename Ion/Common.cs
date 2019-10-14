using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IonLang
{
    unsafe partial class Ion
    {
        private static Map interns;
        private static MemArena intern_arena;

        private void initialise() {
            local_syms = Buffer<Sym>.Create(MAX_LOCAL_SYMS);
            reachable_syms = PtrBuffer.Create();
            package_list = PtrBuffer.Create();
            sorted_syms = PtrBuffer.Create(capacity: 256);
            ast_arena = MemArena.Create();
            intern_arena = MemArena.Create();
            init_compiler();
            init_chars();

            inited = true;
        }

        public static void Write<T>(void* p, ref T value) where T : unmanaged {
            *(T*)p = value;
        }

        public static void Write<T>(void* p, T* value) where T : unmanaged {
            *(T*)p = *value;
        }

        public static void* AsPointer<T>(ref T value) where T : unmanaged {
            fixed (void* v = &value) {
                return v;
            }
        }


        public static char* _I(string s) {
            var c = s.ToPtr();
            return Intern.InternRange(c, c + s.Length);
        }

        public static char* _I(char* s) {
            var len = strlen(s);
            return Intern.InternRange(s, s + len);
        }

        internal class MemArena
        {
            internal byte* ptr;
            internal byte* end;

            internal PtrBuffer* arenas;

            private const int ARENA_ALIGNMENT = 8;
            private const int ARENA_BLOCK_SIZE = 1024 * 1024;


            private void arena_grow(long min_size) {
                //Console.WriteLine("Growing: " + arenas->count);
                var size = (int) ALIGN_UP(CLAMP_MIN(min_size, ARENA_BLOCK_SIZE), ARENA_ALIGNMENT);
                ptr = (byte*)xmalloc(size);
                assert(ptr == ALIGN_DOWN_PTR(ptr, ARENA_ALIGNMENT));
                end = ptr + size;
                arenas->Add(ptr);
            }

            public static MemArena Create() {
                var arena = new MemArena();
                arena.arenas = PtrBuffer.Create();
                arena.ptr = (byte*)xmalloc(ARENA_BLOCK_SIZE);
                assert(arena.ptr == ALIGN_DOWN_PTR(arena.ptr, ARENA_ALIGNMENT));
                arena.end = arena.ptr + ARENA_BLOCK_SIZE;
                arena.arenas->Add(arena.ptr);
                return arena;
            }

            public void* Alloc(int size) {
                var left = end - ptr;
                if (size > left) {
                    arena_grow(size);
                    assert(size <= end - ptr);
                }

                void* new_ptr = ptr;
                ptr = (byte*)ALIGN_UP_PTR(ptr + size, ARENA_ALIGNMENT);
                assert(ptr <= end);
                assert(new_ptr == ALIGN_DOWN_PTR(new_ptr, ARENA_ALIGNMENT));
                return new_ptr;
            }

            internal void free() {
                for (int i = 0; i < arenas->count; i++) {
                    byte* p = *((byte**)arenas->_begin) + (PTR_SIZE * i);
                    xfree(p);
                }
            }
        }


        internal struct Intern
        {
            private long len;
            private char* str;
            private Intern* next;


            public static char* InternRange(char* start, char* end) {
                var len = (int) (end - start);
                var key = Map.hash_bytes(start, len) | 1;
                var intern = (Intern*) interns.map_get(key);
                if (intern != null)
                    return intern->str;

                var new_intern = (Intern*) intern_arena.Alloc(sizeof(Intern));
                new_intern->len = len;
                new_intern->next = intern;

                var len_bytes = len << 1;
                new_intern->str = (char*)intern_arena.Alloc(len_bytes + 2);
                Unsafe.CopyBlock(new_intern->str, start, (uint)len_bytes);
                *(new_intern->str + len) = '\0';
                interns.map_put(key, new_intern);
                return new_intern->str;
            }
        }


        #region Macros*

        public static long MAX(long a, long b) {
            return a > b ? a : b;
        }

        public static long MIN(long a, long b) {
            return a < b ? a : b;
        }

        public static long CLAMP_MIN(long x, long min) {
            return MAX(x, min);
        }

        public static long CLAMP_MAX(long x, long max) {
            return MIN(x, max);
        }

        public static bool IS_POW2(long x) {
            return x != 0 && (x & (x - 1)) == 0;
        }

        public static long ALIGN_DOWN(long n, long a) {
            return n & ~(a - 1);
        }

        public static long ALIGN_UP(long n, long a) {
            return (n + a - 1) & ~(a - 1);
        }

        public static void* ALIGN_DOWN_PTR(void* p, long a) {
            return (void*)ALIGN_DOWN((long)p, a);
        }

        public static void* ALIGN_UP_PTR(void* p, long a) {
            return (void*)ALIGN_UP((long)p, a);
        }

        #endregion

        #region MemAlloc

        internal static void memcpy(void* dst, void* src, int len) {
            Unsafe.CopyBlock(dst, src, (uint)len);
        }

        internal static void* xcalloc(int num_elems, int elem_size) {
            var size = num_elems * elem_size;
            var v = xmalloc(size);
            assert(size < int.MaxValue);

            Unsafe.InitBlock(v, 0, (uint)size);
            return v;
        }

        internal static T* xcalloc<T>(int num_elems = 1) where T : unmanaged {
            return xcalloc<T>(num_elems, sizeof(T));
        }

        internal static T* xcalloc<T>(int num_elems, int elem_size) where T : unmanaged {
            return (T*)xcalloc(num_elems, elem_size);
        }

        internal static void* xrealloc(void* ptr, int num_bytes) {
            return (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes);
        }

        internal static T* xrealloc<T>(T* ptr, int num_bytes) where T : unmanaged {
            return (T*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes);
        }

        internal static void* xmalloc(int num_bytes) {
            return (void*)Marshal.AllocHGlobal((IntPtr)num_bytes);
        }

        internal static T* xmalloc<T>() where T : unmanaged {
            return (T*)xmalloc(sizeof(T));
        }

        internal static T* xmalloc<T>(int n) where T : unmanaged {
            return (T*)xmalloc(n * sizeof(T));
        }

        internal static void xfree(void* ptr) {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        internal static void* memdup(void* src, int size) {
            var dest = xmalloc(size);
            Unsafe.CopyBlock(dest, src, (uint)size);
            return dest;
        }

        private static char* read_file(string path) {
            var text = File.ReadAllText(path);
            var buf = text.ToPtr();
            return buf;
        }

        private static bool write_file(string path, char* buf) {
            File.WriteAllText(path, _S(buf));
            return true;
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

        void path_join(char* path, char* src) {
            char* ptr = path + strlen(path);
            if (ptr != path && ptr[-1] == '/') {
                //ptr--;
            }
            else
                *ptr = '/';
            if (*src == '/') {
                src++;
            }
            strcat(path, src);
        }

        char* path_file(char* path) {
            path_normalize(path);
            for (char* ptr = path + strlen(path); ptr != path; ptr--) {
                if (ptr[-1] == '/') {
                    return ptr;
                }
            }
            return path;
        }

        char* path_ext(char* path) {
            for (char* ptr = path + strlen(path); ptr != path; ptr--) {
                if (ptr[-1] == '.') {
                    return ptr;
                }
            }
            return path;
        }

        #endregion

        static string _S(char* c) => new string(c);

        #region Buffers

        internal struct Buffer<T> where T : unmanaged
        {
            internal T* _begin, _top;

            private const int START_CAPACITY = 4,
                MULTIPLIER = 2;
            public int count { get; set; }
            public int  buffer_size, item_size;
            private int _capacity, _multiplier;

            public static implicit operator T*(Buffer<T> b) {
                return b._begin;
            }

            public T* this[int i] => _begin;

            public static Buffer<T> Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
                assert(capacity >= START_CAPACITY);
                assert(multiplier > 1);
                var b = new Buffer<T>
                {
                    item_size = sizeof(T),
                    _capacity = capacity,
                    _multiplier = multiplier,
                    count = 0
                };
                b.buffer_size = b._capacity * b.item_size;
                b._begin = (T*)xmalloc(b.buffer_size);
                b._top = b._begin;
                return b;
            }

            public void Add(T val) {
                Write(_top, ref val);

                if (++count == _capacity) {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = xrealloc(_begin, buffer_size);
                    _top = _begin + count;
                }
                else {
                    _top++;
                }
            }

            public void Add(T* val) {
                Write(_top, val);

                if (++count == _capacity) {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = xrealloc(_begin, buffer_size);
                    _top = _begin + count;
                }
                else {
                    _top++;
                }
            }

            public void Append(T* val, int len) {
                if (count + len >= _capacity) {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = xrealloc(_begin, buffer_size);
                    _top = _begin + count;
                }

                Unsafe.CopyBlock(_top, val, (uint)len << 1);
                count += len;
                _top += len;
            }

            public void free() {
                xfree(_begin);
                this = default;
            }

            public void clear() {
                _top = _begin;
                count = 0;
            }
        }


        internal struct PtrBuffer
        {
            private const int START_CAPACITY = 8,
                MULTIPLIER = 2;

            public int count;
            internal void** _begin, _top, _end;
            public int Count => (int)(_top-_begin) / PTR_SIZE;
            public int buf_byte_size;
            private int _capacity, _multiplier;

            internal static PtrBuffer* buffers = Create();

            public static PtrBuffer* GetPooledBuffer() {
                if (buffers->count > 0)
                    return (PtrBuffer*)buffers->Remove();
                var buf = Create();
                return buf;
            }


            public static PtrBuffer* Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
                assert(multiplier >= MULTIPLIER);
                assert(capacity > 0);
                var b = (PtrBuffer*) xmalloc(sizeof(PtrBuffer));

                b->_capacity = capacity;
                b->_multiplier = multiplier;
                b->count = 0;
                b->buf_byte_size = capacity * PTR_SIZE;
                b->_begin = (void**)xmalloc(b->buf_byte_size);
                b->_top = b->_begin;
                b->_end = b->_begin + b->buf_byte_size;
                return b;
            }

            public static PtrBuffer Create2(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
                assert(multiplier >= MULTIPLIER);
                assert(capacity > 0);

                var b = new PtrBuffer{
                    _capacity = capacity,
                    _multiplier = multiplier,
                    count = 0,
                    buf_byte_size = capacity * PTR_SIZE,
                };
                b._begin = (void**)xmalloc(b.buf_byte_size);
                b._top = b._begin;
                b._end = b._begin + b.buf_byte_size;
                return b;
            }

            public void* Remove() {
                assert(_top != _begin);
                _top--;
                count--;
                return *_top;
            }

            public void Add(void* val) {
                *_top = val;

                if (++count == _capacity) {
                    _capacity *= _multiplier;
                    buf_byte_size = _capacity * PTR_SIZE;
                    _begin = (void**)xrealloc(_begin, buf_byte_size);
                    _top = _begin + count;
                    _end = _begin + _capacity;
                }
                else {
                    _top++;
                }
            }

            public T** Cast<T>() where T : unmanaged {
                return (T**)_begin;
            }

            public T* Get<T>(int i) where T : unmanaged {
                return *(((T**)_begin) + i);
            }

            public void free() {
                xfree(_begin);
            }

            public void clear() {
                _top = _begin;
                count = 0;
            }

            public void Release() {
                clear();
                fixed (void* v = &this) {
                    buffers->Add(v);
                }
            }
        }

        #endregion

        #region Std Functions

        public static bool isalnum(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9';
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

        public static void strcpy(char* c1, char* c2) {
            while ((*c1++ = *c2++) != 0);
        }

        public static int strcmp(char* c1, char* c2) {
            while (*c1++ == *c2++)
                if (*c1 == '\0')
                    return 0;

            return 1;
        }

        public static char* strcat2(char* c1, char* c2) {
            var s = strlen(c1) + strlen(c2)+ 1;
            char* rtn = xmalloc<char>(s);
            char* p = rtn;

            while (*c1 != '\0')
                *p++ = *c1++;
            while (*c2 != '\0')
                *p++ = *c2++;

            *p = '\0';
            return rtn;
        }

        public static char* strcat(char* c1, char* c2) {
            c1 += strlen(c1);
            while ((*c1 = *c2++) != 0)
                c1++;

            return c1;
        }

        public static int strlen(char* c) {
            var i = 0;
            while (*c++ != '\0')
                i++;

            return i;
        }

        #endregion

        public static int copy_to_pos(char* c1, char* c2, int n = 0) {
            while ((c1[n] = *c2++) != 0)
                n++;
            return n;
        }

        internal unsafe struct Map
        {
            private ulong* keys;
            private void** vals;
            private long len;
            private long cap;


            internal static ulong hash_uint64(ulong x) {
                ulong a = x * 0xff51afd7ed558ccd;
                ulong b = a ^ (x >> 32);
                return b;
            }


            internal static ulong hash_ptr(void* ptr) {
                return hash_uint64((ulong)ptr);
            }

            internal static ulong hash_mix(ulong x, ulong y) {
                x ^= y;
                x *= 0xff51afd7ed558ccd;
                x ^= x >> 32;
                return x;
            }

            public static ulong hash_bytes(void* ptr, long len) {
                var x = 0xcbf29ce484222325;
                char *buf = (char *)ptr;
                for (long i = 0; i < len; i++) {
                    x ^= buf[i];
                    x *= 0x100000001b3;
                    x ^= x >> 32;
                }

                return x;
            }

            public static ulong hash_ptr(void* ptr, long len) {
                var buf = (char*) ptr;
                return hash_bytes(buf, len);
            }


            private void map_grow(long new_cap) {
                new_cap = CLAMP_MIN(new_cap, 16);
                var new_map = new Map
                {
                    keys = (ulong*) xcalloc((int) new_cap, sizeof(ulong)),
                    vals = (void**) xmalloc((int) new_cap * sizeof(void*)),
                    cap = new_cap
                };

                for (long i = 0; i < cap; i++)
                    if (keys[i] != 0)
                        new_map.map_put(keys[i], vals[i]);
                xfree(keys);
                xfree(vals);
                this = new_map;
            }

            public T* map_get<T>(void* key) where T : unmanaged {
                return (T*)map_get(hash_ptr(key));
            }

            public void* map_get(ulong key) {
                if (len == 0)
                    return null;
                assert(IS_POW2(cap));
                var i = key;
                assert(len < cap);
                for (; ; )
                {
                    i &= (ulong)cap - 1;
                    if (keys[i] == key)
                        return vals[i];
                    if (keys[i] == 0)
                        return null;
                    i++;
                }
            }

            public void** map_put(void* key, void* val) {
                return map_put(hash_ptr(key), val);
            }

            public void** map_put(ulong key, void* val) {
                assert(key != 0);
                assert(val != null);
                if (2 * len >= cap)
                    map_grow(2 * cap);
                assert(2 * len < cap);
                assert(IS_POW2(cap));
                var i = key;

                for (; ; )
                {
                    i &= (ulong)cap - 1;
                    if (keys[i] == 0) {
                        len++;
                        keys[i] = key;
                        vals[i] = val;
                        return vals + i;
                    }

                    if (keys[i] == key) {
                        vals[i] = val;
                        return vals + i;
                    }

                    i++;
                }
            }

            internal bool exists(char* key) => map_get(hash_ptr(key)) != null;

            internal void free() {
                xfree(keys);
                xfree(vals);
                len = cap = 0;
                keys = null;
                vals = null;
            }
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

        public override string ToString() {
            String str;

            if ((ull >> 32) > 0)
                str = Convert.ToString((int)(ull >> 32), 2) + Convert.ToString(l, 2).PadLeft(8, '0');
            else
                str = Convert.ToString(l, 2);
            return $"[{str.Length}:{str}]";
        }
    }
}