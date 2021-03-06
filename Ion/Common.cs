﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IonLang
{
    unsafe partial class Ion
    {
        static Map interns;
        static readonly MemArena intern_arena = MemArena.Create();

        void initialise() {
            //local_syms = Buffer<Sym>.Create(MAX_LOCAL_SYMS);
            local_syms = (Sym*)xmalloc(sizeof(Sym) * MAX_LOCAL_SYMS);
            local_syms_end = local_syms;
            reachable_syms = PtrBuffer.Create();
            package_list = PtrBuffer.Create();
            sorted_syms = PtrBuffer.Create(capacity: 256);
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

            const int ARENA_ALIGNMENT = 8;
            const int ARENA_BLOCK_SIZE = 1024 * 512;


            void arena_grow(long min_size) {
                //Console.WriteLine("Growing: " + arenas->count);
                var size = (int) ALIGN_UP(CLAMP_MIN(min_size, ARENA_BLOCK_SIZE), ARENA_ALIGNMENT);
                ptr = (byte*)Marshal.AllocHGlobal(size);
                assert(ptr == ALIGN_DOWN_PTR(ptr, ARENA_ALIGNMENT));
                end = ptr + size;
                arenas->Add(ptr);
            }

            public static MemArena Create() {
                var arena = new MemArena();
                arena.arenas = PtrBuffer.Create();
                arena.ptr = (byte*)Marshal.AllocHGlobal(ARENA_BLOCK_SIZE);
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
                    Marshal.FreeHGlobal((IntPtr)p);
                }
            }
        }


        
        internal struct Intern
        {
            internal static long intern_memory_usage;
            long len;
            Intern* next;
            char* str;

            public static char* InternRange(char* start, char* end) {
                var len = (int) (end - start);
                var key = Map.hash_bytes(start, len * 2) | 1;
                var intern = (Intern*) interns.map_get_from_uint64(key);
                if (intern != null)
                    return intern->str;

                var new_intern = (Intern*) intern_arena.Alloc(sizeof(Intern));
                new_intern->len = len;
                new_intern->next = intern;

                var len_bytes = len << 1;
                new_intern->str = (char*)intern_arena.Alloc(len_bytes + 2);
                Unsafe.CopyBlock(new_intern->str, start, (uint)len_bytes);
                *(new_intern->str + len) = '\0';
                intern_memory_usage += sizeof(Intern) + len + 1 + 16;
                interns.map_put_from_uint64(key, new_intern);
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


        public static ulong MAX(ulong a, ulong b) {
            return a > b ? a : b;
        }

        public static ulong MIN(ulong a, ulong b) {
            return a < b ? a : b;
        }

        public static ulong CLAMP_MIN(ulong x, ulong min) {
            return MAX(x, min);
        }

        public static ulong CLAMP_MAX(ulong x, ulong max) {
            return MIN(x, max);
        }

        public static bool IS_POW2(ulong x) {
            return x != 0 && (x & (x - 1)) == 0;
        }

        public static ulong ALIGN_DOWN(ulong n, ulong a) {
            return n & ~(a - 1);
        }

        public static ulong ALIGN_UP(ulong n, ulong a) {
            return (n + a - 1) & ~(a - 1);
        }

        public static void* ALIGN_DOWN_PTR(void* p, ulong a) {
            return (void*)ALIGN_DOWN((ulong)p, a);
        }

        public static void* ALIGN_UP_PTR(void* p, ulong a) {
            return (void*)ALIGN_UP((ulong)p, a);
        }

        #endregion

        #region MemAlloc
        static MemArena main_arena = MemArena.Create();
        [DebuggerHidden]
        internal static void memcpy(void* dst, void* src, int len) {
            Unsafe.CopyBlock(dst, src, (uint)len);
        }

        [DebuggerHidden]
        internal static void* xcalloc(int num_elems, int elem_size) {
            var size = num_elems * elem_size;
            var v = xmalloc(size);
            assert(size < int.MaxValue);

            Unsafe.InitBlock(v, 0, (uint)size);
            return v;
        }

        [DebuggerHidden]
        internal static T* xcalloc<T>(int num_elems = 1) where T : unmanaged {
            return xcalloc<T>(num_elems, sizeof(T));
        }

        [DebuggerHidden]
        internal static T* xcalloc<T>(int num_elems, int elem_size) where T : unmanaged {
            return (T*)xcalloc(num_elems, elem_size);
        }

        [DebuggerHidden]
        internal static void* xrealloc(void* ptr, int num_bytes) {
            return (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes);
        }

        [DebuggerHidden]
        internal static T* xrealloc<T>(T* ptr, int num_bytes) where T : unmanaged {
            //var ptr = main_arena.Alloc(num_bytes);
            return (T*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes);
        }

        [DebuggerHidden]
        internal static void* xmalloc(int num_bytes) {
            //return main_arena.Alloc(num_bytes);
            return (void*)Marshal.AllocHGlobal((IntPtr)num_bytes);
        }

        [DebuggerHidden]
        internal static T* xmalloc<T>() where T : unmanaged {
            return (T*)xmalloc(sizeof(T));
        }

        [DebuggerHidden]
        internal static T* xmalloc<T>(int n) where T : unmanaged {
            return (T*)xmalloc(n * sizeof(T));
        }

        [DebuggerHidden]
        internal static void xfree(void* ptr) {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        [DebuggerHidden]
        internal static void* memdup(void* src, int size) {
            var dest = xmalloc(size);
            Unsafe.CopyBlock(dest, src, (uint)size);
            return dest;
        }

        static char* read_file(string path, out int len) {
            var text = File.ReadAllText(path);
            len = text.Length;
            var buf = text.ToPtr();
            return buf;
        }

        static bool write_file(string path, char* buf) {
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

        #endregion

        #region Std Functions

        public static bool isalnum(char c) {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9';
        }
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

        public static void strcpy(char* c1, char* c2) {
            while ((*c1++ = *c2++) != 0)
                ;
        }
        public static char* strstr(char* c1) {
            uint i = (uint)strlen(c1);
            char* c = xmalloc<char>();
            Unsafe.CopyBlock(c, c1, i);
            return c;
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

        bool str_islower(char* str) {
            while (*str != '\0') {
                if (isalpha(*str) && !islower(*str)) {
                    return false;
                }
                str++;
            }
            return true;
        }

        public static int copy_to_pos(char* c1, char* c2, int n = 0) {
            while ((c1[n] = *c2++) != 0)
                n++;
            return n;
        }


    }

    internal unsafe struct Map
    {
        ulong* keys, vals;
        ulong len, cap;

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

        internal static ulong hash_bytes(void* ptr, long len) {
            var x = 0xcbf29ce484222325;
            byte *buf = (byte *)ptr;
            for (long i = 0; i < len; i++) {
                x ^= buf[i];
                x *= 0x100000001b3;
                x ^= x >> 32;
            }

            return x;
        }

        void map_grow(ulong new_cap) {
            new_cap = Ion.CLAMP_MIN(new_cap, 16);
            var new_map = new Map
                {
                keys = (ulong*) Ion.xcalloc((int) new_cap, sizeof(ulong)),
                vals = (ulong*)(void**) Ion.xmalloc((int) new_cap * sizeof(ulong)),
                cap = new_cap
            };

            for (ulong i = 0; i < cap; i++)
                if (keys[i] != 0)
                    new_map.map_put_uint64_from_uint64(keys[i], vals[i]);
            Ion.xfree(keys);
            Ion.xfree(vals);
            this = new_map;
        }


        ulong map_get_uint64_from_uint64(ulong key) {
            if (len == 0)
                return 0;

            Ion.assert(Ion.IS_POW2(cap));
            Ion.assert(len < cap);

            for (var i = key; ; i++) {
                i &= cap - 1;
                if (keys[i] == key)
                    return vals[i];
                if (keys[i] == 0)
                    return 0;
            }
        }

        void map_put_uint64_from_uint64(ulong key, ulong val) {
            Ion.assert(key != 0);
            if (val == 0) {
                return;
            }

            if (2 * len >= cap)
                map_grow(2 * cap);

            Ion.assert(2 * len < cap);
            Ion.assert(Ion.IS_POW2(cap));


            for (var i = key; ; i++) {
                i &= cap - 1;
                if (keys[i] == 0) {
                    len++;
                    keys[i] = key;
                    vals[i] = val;
                    return;
                }

                if (keys[i] == key) {
                    vals[i] = val;
                    return;
                }
            }
        }


        public void map_put(void* key, void* val) => map_put_uint64_from_uint64((ulong)key, (ulong)val);
        public void map_put_from_uint64(ulong key, void* val) => map_put_uint64_from_uint64(key, (ulong)val);
        public T* map_get<T>(void* key) where T : unmanaged => (T*)map_get_from_uint64((ulong)key);
        public void* map_get_from_uint64(ulong key) => (void*)map_get_uint64_from_uint64(key);
        public ulong map_get_uint64(void* key) => map_get_uint64_from_uint64((ulong)key);
        public void map_put_uint64(void* key, ulong val) => map_put_uint64_from_uint64((ulong)key, val);

        internal bool exists(void* key) => map_get_uint64_from_uint64((ulong)key) != 0;

        internal void free() {
            Ion.xfree(keys);
            Ion.xfree(vals);
            this = default;
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

    internal unsafe struct Buffer<T> where T : unmanaged
    {
        internal T* _begin, _top;

        const int START_CAPACITY = 4,
                MULTIPLIER = 2;

        internal int count;

        int  buffer_size, item_size;
        int _capacity, _multiplier;

        public static implicit operator T*(Buffer<T> b) {
            return b._begin;
        }

        public T* this[int i] => _begin + i;

        public static Buffer<T> Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
            Ion.assert(capacity >= START_CAPACITY);
            Ion.assert(multiplier > 1);
            var b = new Buffer<T>
                {
                item_size = sizeof(T),
                _capacity = capacity,
                _multiplier = multiplier,
                count = 0
            };
            b.buffer_size = b._capacity * b.item_size;
            b._begin = (T*)Ion.xmalloc(b.buffer_size);
            b._top = b._begin;
            return b;
        }

        public void Add(T val) {
            Ion.Write(_top, ref val);

            if (++count == _capacity) {
                _capacity *= _multiplier;
                buffer_size = _capacity * item_size;
                _begin = Ion.xrealloc(_begin, buffer_size);
                _top = _begin + count;
            }
            else {
                _top++;
            }
        }

        public void Add(T* val) {
            Ion.Write(_top, val);

            if (++count == _capacity) {
                _capacity *= _multiplier;
                buffer_size = _capacity * item_size;
                _begin = Ion.xrealloc(_begin, buffer_size);
                _top = _begin + count;
            }
            else {
                _top++;
            }
        }

        public void Append(T* val, int len) {
            if (count + len >= _capacity) {
                while (_capacity < len)
                    _capacity *= _multiplier;
                buffer_size = _capacity * item_size;
                _begin = Ion.xrealloc(_begin, buffer_size);
                _top = _begin + count;
            }

            Unsafe.CopyBlock(_top, val, (uint)(len * item_size));
            count += len;
            _top += len;
        }

        public void free() {
            Ion.xfree(_begin);
            this = default;
        }

        public void clear() {
            _top = _begin;
            count = 0;
        }
    }


    internal unsafe struct PtrBuffer
    {
        const int START_CAPACITY = 64,
                MULTIPLIER = 2;

        public int count;
        internal void** _begin, _top, _end;
        public int Count => (int)(_top - _begin) / Ion.PTR_SIZE;
        public int buf_byte_size;
        int _capacity, _multiplier;

        internal static PtrBuffer* buffers = Create();

        public static PtrBuffer* GetPooledBuffer() {
            if (buffers->count > 0)
                return (PtrBuffer*)buffers->Remove();
            var buf = Create();
            return buf;
        }


        public static PtrBuffer* Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
            Ion.assert(multiplier >= MULTIPLIER);
            Ion.assert(capacity > 0);
            var b = (PtrBuffer*) Ion.xmalloc(sizeof(PtrBuffer));

            b->_capacity = capacity;
            b->_multiplier = multiplier;
            b->count = 0;
            b->buf_byte_size = capacity * Ion.PTR_SIZE;
            b->_begin = (void**)Ion.xmalloc(b->buf_byte_size);
            b->_top = b->_begin;
            b->_end = b->_begin + b->buf_byte_size;
            return b;
        }

        public void* Remove() {
            Ion.assert(_top != _begin);
            _top--;
            count--;
            return *_top;
        }

        public void Add(void* val) {
            *_top = val;

            if (++count == _capacity) {
                _capacity *= _multiplier;
                buf_byte_size = _capacity * Ion.PTR_SIZE;
                _begin = (void**)Ion.xrealloc(_begin, buf_byte_size);
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
            Ion.xfree(_begin);
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

}