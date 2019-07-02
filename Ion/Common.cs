using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DotNetCross.Memory;
using static System.Console;

namespace Lang
{

    unsafe partial class Ion
    {
        #region Macros*

        public static long MAX(long a, long b) => a > b ? a : b;
        public static long MIN(long a, long b) => a < b ? a : b;
        public static long CLAMP_MIN(long x, long min) => MAX(x, min);
        public static long CLAMP_MAX(long x, long max) => MIN(x, max);
        public static bool IS_POW2(long x) => x != 0 && (x & x - 1) == 0;
        public static long ALIGN_DOWN(long n, long a) => n & ~(a - 1);
        public static long ALIGN_UP(long n, long a) => n + a - 1 & ~(a - 1);
        public static unsafe void* ALIGN_DOWN_PTR(void* p, long a) => (void*)ALIGN_DOWN((long)p, a);
        public static void* ALIGN_UP_PTR(void* p, long a) => (void*)ALIGN_UP((long)p, a);

        #endregion

        #region MemAlloc

        internal static void memcpy(void* dst, void* src, int len) => Unsafe.CopyBlock(dst, src, (uint)len);
         
        internal static void* xcalloc(int num_elems, int elem_size)
        {
            int size = num_elems * elem_size;
            void* v = xmalloc(size);
            assert(size < int.MaxValue);
            Unsafe.InitBlock(v, 0, (uint)size);
            return v;
        }

        internal static T* xcalloc<T>(int num_elems = 1) where T : unmanaged => xcalloc<T>(num_elems, sizeof(T));
        internal static T* xcalloc<T>(int num_elems, int elem_size) where T : unmanaged => (T*)xcalloc(num_elems, elem_size);
        internal static void* xrealloc(void* ptr, int num_bytes) => (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes); //realloc(ptr, num_bytes);
        internal static T* xrealloc<T>(T* ptr, int num_bytes) where T : unmanaged => (T*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes); //(T*)realloc(ptr, num_bytes);
        internal static void* xmalloc(int num_bytes) => (void*)Marshal.AllocHGlobal((IntPtr)num_bytes); //malloc(num_bytes);
        internal static T* xmalloc<T>() where T : unmanaged => (T*)xmalloc(sizeof(T));
        internal static T* xmalloc<T>(int n) where T : unmanaged => (T*)xmalloc(n * sizeof(T));

        internal static void xfree(void* ptr) => Marshal.FreeHGlobal((IntPtr)ptr);

        internal static void* memdup(void* src, int size)
        {
            void* dest = xmalloc(size);
            Unsafe.CopyBlock(dest, src, (uint)size);
            return dest;
        }

        static char* read_file(string path)
        {
            var buf = File.ReadAllText(path).ToPtr2();
            return buf;
        }

        static bool write_file(string path, char* buf)
        {
            File.WriteAllText(path, new string(buf));
            return true;
        }

        static char* get_ext(char* path)
        {
            for (char* ptr = path + strlen(path); ptr != path; ptr--)
            {
                if (ptr[-1] == '.')
                {
                    return ptr;
                }
            }
            return null;
        }

        static string replace_ext(char* path, char* new_ext)
        {
            char* ext = get_ext(path);
            if (ext == null)
            {
                return null;
            }
            int base_len = (int)(ext - path);
            int new_ext_len = strlen(new_ext);
            int new_path_len = base_len + new_ext_len;
            char* new_path = xmalloc<char>(new_path_len + 1);
            memcpy(new_path, path, base_len * 2);
            memcpy(new_path + base_len, new_ext, new_ext_len * 2);
            new_path[new_path_len] = '\0';
            return new string(new_path);
        }

        #endregion

        public unsafe static void Write<T>(void* p, ref T value) where T : unmanaged
        {
            *(T*)p = value;
        }
        public static T As<T>(object obj) where T : class
        {
            return (T)obj;
        }
        public unsafe static void* AsPointer<T>(ref T value) where T : unmanaged
        {
            fixed (void* v = &value)
                return v;
        }



        #region Buffers
        internal struct Buffer<T> where T : unmanaged
        {
            internal T* _begin, _top;
            private const int START_CAPACITY = 4,
                              MULTIPLIER = 2;

            public int count, buffer_size, item_size;
            private int _capacity, _multiplier;

            public static implicit operator T*(Buffer<T> b) => b._begin;

            public static Buffer<T> Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER)
            {
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

            public void Add(T val)
            {
                Write(_top, ref val);

                if (++count == _capacity)
                {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = xrealloc(_begin, buffer_size);
                    _top = _begin + count;
                }
                else
                {
                    _top++;
                }

            }

            public void Add(T* val, int len)
            {
                if ((count+len) >= _capacity)
                {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = xrealloc(_begin, buffer_size);
                    _top = _begin + count;
                }
                Unsafe.CopyBlock(_top, val, (uint)len << 1);
                count += len;
                _top += len;
            }
          
            public void free()
            {
                xfree(_begin);
            }

            public void clear()
            {
                _top = _begin;
                count = 0;
            }
            
        }


        internal struct PtrBuffer
        {
            private const int START_CAPACITY = 8,
                              MULTIPLIER = 2;

            internal void** _begin, _top, _end;

            public int count, buf_byte_size;
            private int _capacity, _multiplier;
            internal static PtrBuffer* buffers = Create();

            static PtrBuffer()
            {

            }
            public static PtrBuffer* GetPooledBuffer()
            {
                if(buffers->count > 0)
                {
                    return (PtrBuffer*)buffers->Remove();
                }
                PtrBuffer* buf = Create();
                return buf;
            }

            public static PtrBuffer* Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER)
            {
                assert(multiplier >= MULTIPLIER);
                assert(capacity > 0);
                PtrBuffer* b = (PtrBuffer*)xmalloc(sizeof(PtrBuffer));

                b->_capacity = capacity;
                b->_multiplier = multiplier;
                b->count = 0;
                b->buf_byte_size = capacity * PTR_SIZE;
                b->_begin = (void**)xmalloc(b->buf_byte_size);
                b->_top = b->_begin;
                b->_end = b->_begin + b->buf_byte_size;
                return b;
            }

            public void* Remove()
            {
                assert(_top != _begin);
                _top--;
                count--;
                return *_top;
            }

            public void Add(void* val)
            {
                *_top = val;

                if (++count == _capacity)
                {
                    _capacity *= _multiplier;
                    buf_byte_size = _capacity * PTR_SIZE;
                    _begin = (void**)xrealloc(_begin, buf_byte_size);
                    _top = _begin + count;
                    _end = _begin + _capacity;
                }
                else
                    _top++;
            }

            public T** Cast<T>() where T : unmanaged => (T**)_begin;

            public void free()
            {
                xfree(_begin);
            }

            public void clear()
            {
                _top = _begin;
                count = 0;
            }

            public void Release()
            {
                clear();
                fixed(void* v = &this)
                    buffers->Add(v);
            }
        }
        #endregion
        private static Map interns;
        private static readonly MemArena* intern_arena = MemArena.Create();

        internal struct MemArena
        {
            internal byte* ptr;
            internal byte* end;

            internal PtrBuffer* arenas;

            const int ARENA_ALIGNMENT = 8;
            const int ARENA_BLOCK_SIZE = 1024 * 1024;


            void arena_grow(long min_size)
            {
                //Console.WriteLine("Growing: " + arenas->count);
                int size = (int)ALIGN_UP(MAX(ARENA_BLOCK_SIZE, min_size), ARENA_ALIGNMENT);
                ptr = (byte*)xmalloc(size);
                assert(ptr == ALIGN_DOWN_PTR(ptr, ARENA_ALIGNMENT));
                end = ptr + size;
                arenas->Add(ptr);
            }

            public static MemArena* Create()
            {
                MemArena* arena = (MemArena*)xmalloc(sizeof(MemArena));
                arena->arenas = PtrBuffer.Create();
                arena->ptr = (byte*)xmalloc((int)ARENA_BLOCK_SIZE);
                assert(arena->ptr == ALIGN_DOWN_PTR(arena->ptr, ARENA_ALIGNMENT));
                arena->end = arena->ptr + ARENA_BLOCK_SIZE;
                arena->arenas->Add(arena->ptr);
                return arena;
            }

            public void* Alloc(int size)
            {
                var left = end - ptr;
                if (size > left)
                {
                    arena_grow(size);
                    assert(size <= (end - ptr));
                }

                void* new_ptr = ptr;
                ptr = (byte*)ALIGN_UP_PTR(ptr + size, ARENA_ALIGNMENT);
                assert(ptr <= end);
                assert(new_ptr == ALIGN_DOWN_PTR(new_ptr, ARENA_ALIGNMENT));
                return new_ptr;
            }
        }


        internal struct Intern
        {
            long len;
            char* str;
            Intern* next;


            public static char* InternRange(char* start, char* end)
            {
                int len = (int)(end - start);
                ulong key = Map.str_hash(start, len) | 1;
                Intern* intern = (Intern*)interns.map_get(key);
                if (intern != null)
                    return intern->str;

                Intern* new_intern = (Intern*)intern_arena->Alloc(sizeof(Intern));
                new_intern->len = len;
                new_intern->next = intern;

                int len_bytes = (len << 1);
                new_intern->str = (char*)intern_arena->Alloc(len_bytes + 2);
                Unsafe.CopyBlock(new_intern->str, start, (uint)len_bytes);
                *(new_intern->str + len) = '\0';
                interns.map_put(key, new_intern);
                return new_intern->str;
            }
        }


        public static char* _I(string s)
        {
            char* c = s.ToPtr();
            return Intern.InternRange(c, c + s.Length);

        }

        struct MapEntry
        {
            public void* key;
            public void* val;
            public long hash;
        }
        struct Map
        {
            internal ulong* keys;
            internal void** vals;
            internal long len;
            internal long cap;



            public static ulong int64_hash(ulong x)
            {
                x *= 0xff51afd7ed558ccd;
                x ^= x >> 32;
                return x;
            }



            public static ulong ptr_hash(void* ptr)
            {
                return int64_hash((ulong)ptr);
            }


            public static ulong str_hash(char* str, long len)
            {
                ulong x = 0xcbf29ce484222325ul;
                for (long i = 0; i < len; i++)
                {
                    x ^= str[i]; 
                    x *= 0x100000001b3ul;
                    x ^= x >> 32;
                }
                return x;
            }

            public static ulong ptr_hash(void* ptr, long len)
            {
                char* buf = (char*)ptr;
                return str_hash(buf, len);
            }



            void map_grow(long new_cap)
            {
                new_cap = MAX(new_cap, 16);
                var new_map = new Map
                {
                    keys = (ulong*)xcalloc((int)new_cap, sizeof(ulong)),
                    vals = (void**)xmalloc((int)new_cap * sizeof(void*)),
                    cap = new_cap
                };

                for (long i = 0; i < cap; i++)
                {
                    if (keys[i] != 0)
                    {
                        new_map.map_put(keys[i], vals[i]);
                    }
                }
                xfree(keys);
                xfree(vals);
                this = new_map;
            }

            public void* map_get(void* key) => map_get(ptr_hash(key));
            public void* map_get(ulong key)
            {
                if (len == 0)
                {
                    return null;
                }
                assert(IS_POW2(cap));
                ulong i = key;
                assert(len < cap);
                for (; ; )
                {
                    i &= (ulong)cap - 1;
                    if (keys[i] == key)
                    {
                        return vals[i];
                    }
                    else if (keys[i] == 0)
                    {
                        return null;
                    }
                    i++;
                }
            }

            public void** map_put(void* key, void* val) => map_put(ptr_hash(key), val);
            public void** map_put(ulong key, void* val)
            {
                assert(key != 0);
                assert(val != null);
                if (2 * len >= cap)
                {
                    map_grow(2 * cap);
                }
                assert(2 * len < cap);
                assert(IS_POW2(cap));
                ulong i = key;

                for (; ; )
                {
                    i &= (ulong)cap - 1;
                    if (keys[i] == 0)
                    {
                        len++;
                        keys[i] = key;
                        vals[i] = val;
                        return vals + i;
                    }
                    else if (keys[i] == key)
                    {
                        vals[i] = val;
                        return vals + i;
                    }
                    i++;
                }
            }


        }
        #region Std Functions


        public static bool isalnum(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');


        public static bool isspace(char c) => c == ' ' || c == '\n' || c == '\r' || c == '\t' || c == '\v';


        public static bool islower(char c) => c >= 'a' && c <= 'z';

        public static bool isupper(char c) => c >= 'A' && c <= 'Z';

        public static bool isdigit(char c) => c >= '0' && c <= '9';



        public static char tolower(char c) => c >= 'a' ? c : (char)(c + 32);


        public static int strcmp(char* c1, char* c2)
        {
            while (*c1++ == *c2++)
                if (*c1 == '\0')
                    return 0;

            return 1;
        }
        public static int strlen(char* c)
        {
            int i = 0;
            while (*c++ != '\0') i++;

            return i;
        }

        #endregion
    }
}
