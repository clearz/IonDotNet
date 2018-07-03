using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCross.Memory;
using static System.Console;

namespace Lang
{
    #region Typedefs
#if X64
    using size_t = Int64;
    using uptr_t = UInt64;
#else
    using size_t = Int32;
    using uptr_t = UInt32;
#endif
    #endregion
    unsafe partial class Ion {
        #region Macros*

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t MAX(size_t a, size_t b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t MIN(size_t a, size_t b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t CLAMP_MIN(size_t x, size_t min) => MAX(x, min);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t CLAMP_MAX(size_t x, size_t max) => MIN(x, max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IS_POW2(size_t x) => x != 0 && (x & x - 1) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t ALIGN_DOWN(size_t n, size_t a) => n & ~(a - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ALIGN_UP(Int64 n, Int64 a) => n + a - 1 & ~(a - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* ALIGN_DOWN_PTR(void* p, size_t a) => (void*) ALIGN_DOWN((size_t)p, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* ALIGN_UP_PTR(void* p, Int64 a) => (void*) ALIGN_UP((Int64)p, a);

        #endregion
        #region ErrorHandling*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void fatal(string format, params object[] pmz)
        {
            Console.WriteLine("FATAL: " + format, pmz);
            Thread.Sleep(4000);
           // Environment.Exit(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void syntax_error(string format, params object[] pmz) => WriteLine("Syntax Error: " + format, pmz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void fatal_syntax_error(string format, params object[] pmz)
        {
            syntax_error(format, pmz);
            Thread.Sleep(4000);
            //Environment.Exit(1);
        }
        #endregion
        #region Buffers
        internal struct Buffer<T> where T : unmanaged
        {
            private const int START_CAPACITY = 8,
                              MULTIPLIER = 2;

            public int count, buf_byte_size, size_of;
            private int _capacity, _multiplier;
            

            public T Peek => *((T*)_top);

            public T this[size_t i] => *(T*)(_begin + i * size_of);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T* New()
            {
                if (count == _capacity)
                {
                    _capacity *= _multiplier;
                    assert(count <= _capacity);
                    assert(count <= _capacity);
                    _begin = (T*) Marshal.ReAllocHGlobal((IntPtr)_begin, (IntPtr)(_capacity * size_of));
                    _top = _begin + count;
                    buf_byte_size = _capacity * size_of;
                }
                else
                {
                    _top++;
                }
                
                count++;
                return (T*)_top;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Buffer<T> Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER)
            {
                assert(capacity >= START_CAPACITY);
                assert(multiplier > 1);
                var b = new Buffer<T> {
                    size_of = sizeof(T),
                    _capacity = capacity,
                    _multiplier = multiplier,
                    count = 0
                };
                b.buf_byte_size = b._capacity * b.size_of;
                b._begin = (T*)Marshal.AllocHGlobal((IntPtr)b.buf_byte_size);
                b._top = b._begin;
                return b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T val)
            {
                Unsafe.Write(_top, ref val);
                if (++count == _capacity)
                {
                    _capacity *= _multiplier;
                    buf_byte_size = _capacity * size_of;
                    _begin = (T*)Marshal.ReAllocHGlobal((IntPtr)_begin, (IntPtr)buf_byte_size);
                    _top = _begin + count;
                }
                else
                {
                    _top ++;
                }
                
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void free() {
                Marshal.FreeHGlobal((IntPtr)_begin);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool fits(size_t n) => n <= _capacity - count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void clear()
            {
                _top = _begin;
                count = 0;
            }

            internal T* _begin, _top;
            
        }
        internal struct Buffer
        {
            private const int START_CAPACITY = 8,
                              MULTIPLIER = 2;

            public int count, buf_byte_size, size_of;
            private int _capacity, _multiplier;

            public void* Peek => _top;

            public void* this[size_t i] => (_begin + i * size_of);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void* New()
            {
                if (count == _capacity)
                {
                    _capacity *= _multiplier;
                    assert(count <= _capacity);
                    buf_byte_size = _capacity * size_of;
                    _begin = (byte*)Marshal.ReAllocHGlobal((IntPtr)_begin, (IntPtr)buf_byte_size);
                    _top = _begin + size_of * count;
                    _end = _begin + buf_byte_size;
                }
                else
                {
                    _top += size_of;
                }

                count++;
                return _top;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Buffer* Create(int size, int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {

                Buffer* b = (Buffer*)Marshal.AllocHGlobal(sizeof(Buffer));

                b->size_of = size;
                b->_capacity = capacity;
                b->_multiplier = multiplier;
                b->count = 0;
                b->buf_byte_size = capacity * size;
                b->_begin = (byte*) Marshal.AllocHGlobal(b->buf_byte_size);
                b->_top = b->_begin;
                b->_end = b->_begin + b->buf_byte_size;
                return b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(void* val)
            {
                Unsafe.CopyBlock(_top, val, (uint)size_of);
                if (++count == _capacity) {
                    _capacity *= _multiplier;
                    assert(count <= _capacity);
                    buf_byte_size = _capacity * size_of;
                    _begin = (byte*) Marshal.ReAllocHGlobal((IntPtr) _begin, (IntPtr) (_capacity * size_of));
                    _top = _begin + size_of * count;
                    _end = _begin + buf_byte_size;
                }
                else {
                    _top += size_of;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void free() {
                Marshal.FreeHGlobal((IntPtr) _begin);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool fits(size_t n) => n <= _capacity - count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void clear()
            {
                _top = _begin;
                count = 0;
            }

            internal byte* _begin, _top, _end;
        }
        internal struct PtrBuffer
        {
            private const int START_CAPACITY = 8,
                              MULTIPLIER = 2;

            public int count, buf_byte_size;
            private int _capacity, _multiplier;

            public void* Peek => (void*)(*_top);

            public void* this[size_t i] => (void*)*(_begin + i * PTR_SIZE);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void* New<T>()
            {
                if (count == _capacity) {
                    _capacity *= _multiplier;
                    assert(count <= _capacity);
                    _begin = (uptr_t*) Marshal.ReAllocHGlobal((IntPtr) _begin, (IntPtr) (_capacity * PTR_SIZE));
                    _top = _begin + PTR_SIZE * count;
                    buf_byte_size = _capacity * PTR_SIZE;
                    _end = _begin + buf_byte_size;
                }
                else
                    _top += PTR_SIZE;

                void* block = (void*)Marshal.AllocHGlobal(Unsafe.SizeOf<T>());
                Unsafe.CopyBlock(_top, &block, PTR_SIZE);
                count++;
                return block;
            }

           // public static implicit operator void*(PtrBuffer b) => b._begin;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PtrBuffer* Create(int capacity = START_CAPACITY, int multiplier = MULTIPLIER) {
                assert(multiplier >= MULTIPLIER);
                assert(capacity > 0);
                PtrBuffer* b = (PtrBuffer*)Marshal.AllocHGlobal(sizeof(PtrBuffer));

                b->_capacity = capacity;
                b->_multiplier = multiplier;
                b->count = 0;
                b->buf_byte_size = capacity * PTR_SIZE;
                b->_begin = (uptr_t*)Marshal.AllocHGlobal(b->buf_byte_size);
                b->_top = b->_begin;
                b->_end = b->_begin + b->buf_byte_size;
                return b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(void* val) {
                *_top = (uptr_t)val;

                if (++count == _capacity) {
                    _capacity *= _multiplier;
                    buf_byte_size = _capacity * PTR_SIZE;
                    _begin = (uptr_t*) Marshal.ReAllocHGlobal((IntPtr) _begin, (IntPtr) buf_byte_size);
                    _top = _begin + count;
                    _end = _begin + _capacity;
                }
                else
                    _top++;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void free()
            {
                Marshal.FreeHGlobal((IntPtr)_begin);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool fits(size_t n) => n <= _capacity - count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void clear()
            {
                _top = _begin;
                count = 0;
            }

            internal uptr_t* _begin, _top, _end;

            public static void* Array(params void*[] ptrs) {
                uptr_t* array = (uptr_t*)Marshal.AllocHGlobal(PTR_SIZE * ptrs.Length);
                for (var i = 0; i < ptrs.Length; i++) {
                    array[i] = (uptr_t)ptrs[i];
            }

                return array;
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


            public const int ARENA_ALIGNMENT = 8;

            public const int ARENA_BLOCK_SIZE = 1024 * 1024;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void arena_grow(size_t min_size)
            {
               // Console.WriteLine("Growing: " + arenas->count);
                size_t size = ALIGN_UP(MAX(ARENA_BLOCK_SIZE, min_size), ARENA_ALIGNMENT);
                ptr = (byte*)Marshal.AllocHGlobal((IntPtr)size);
                assert(ptr == ALIGN_DOWN_PTR(ptr, ARENA_ALIGNMENT));
                end = ptr + size;
                arenas->Add(ptr);
            }

            public static MemArena* Create() {
                MemArena* arena = (MemArena*)Marshal.AllocHGlobal(sizeof(MemArena));
                arena->arenas = PtrBuffer.Create();
                arena->ptr = (byte*)Marshal.AllocHGlobal((IntPtr)ARENA_BLOCK_SIZE);
                assert(arena->ptr == ALIGN_DOWN_PTR(arena->ptr, ARENA_ALIGNMENT));
                arena->end = arena->ptr + ARENA_BLOCK_SIZE;
                arena->arenas->Add(arena->ptr);
                return arena;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void* Alloc(size_t size)
            {
                var left = end - ptr;
                if (size > left) {
                    arena_grow(size);
                    assert(size <= (int)(end - ptr));
                }

                void* new_ptr = ptr;
                ptr = (byte*) ALIGN_UP_PTR(ptr + size, ARENA_ALIGNMENT);
                assert(ptr <= end);
                assert(new_ptr == ALIGN_DOWN_PTR(new_ptr, ARENA_ALIGNMENT));
                return new_ptr;
            }
        }
        internal struct Intern
        {
            int len;
            char* str;
            Intern* next;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static char* InternRange(char* start, char* end)
            {
                int len = (int)(end - start);
                ulong hash = Map.hash_bytes(start, len);
                ulong key = hash > 0 ? hash : 1;
                Intern* intern = (Intern*)interns.map_get_from_uint64(key);
                for (Intern* it = intern; it != null; it = it->next)
                {
                    if (it->len == len)
                    {
                        for (int cnt = 0; *(it->str + cnt) == *(start + cnt); ++cnt)
                            if (cnt+1 == len)
                                return it->str;
                    }
                }
                //Intern* new_intern = arena_alloc(&intern_arena, offsetof(Intern, str) + len + 1);
                Intern* new_intern = (Intern*)intern_arena->Alloc(sizeof(Intern));
                new_intern->len = len;
                new_intern->next = intern;

                int len_bytes = (int)(len << 1);
                new_intern->str = (char*)intern_arena->Alloc(len_bytes+2);
                Unsafe.CopyBlock(new_intern->str, start, (uint)len_bytes);
                *(new_intern->str + len) = '\0';
                interns.map_put_from_uint64(key, new_intern);
                return new_intern->str;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* _I(string s) {
            char* c = s.ToPtr();
            return Intern.InternRange(c, c + s.Length);
            
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void* calloc(uint num, uint size);
        struct Map
        {

            ulong* keys;
            ulong* vals;
            internal int len;
            size_t cap;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong hash_uint64(ulong x)
            {
                x *= 0xff51afd7ed558ccd;
                x ^= x >> 32;
                return x;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong hash_ptr(void* ptr)
            {
                return hash_uint64((ulong)ptr);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong hash_mix(ulong x, ulong y)
            {
                x ^= y;
                x *= 0xff51afd7ed558ccd;
                x ^= x >> 32;
                return x;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong hash_bytes(void* ptr, int len)
            {
                ulong x = 0xcbf29ce484222325;
                char* buf = (char*)ptr;
                for (int i = 0; i < len; i++)
                {
                    x ^= buf[i];
                    x *= 0x100000001b3;
                    x ^= x >> 32;
                }
                return x;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ulong map_get_uint64_from_uint64(ulong key)
            {
                if (len == 0)
                {
                    return 0;
                }
                assert(IS_POW2(cap));
                size_t i = (size_t)hash_uint64(key);
                assert(len < cap);
                for (; ; )
                {
                    i &= cap - 1;
                    if (keys[i] == key)
                    {
                        return vals[i];
                    }
                    else if (keys[i] == 0)
                    {
                        return 0;
                    }
                    i++;
                }
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void map_grow(size_t new_cap)
            {
                new_cap = CLAMP_MIN(new_cap, 16);
                var cap_bytes = new_cap * sizeof(ulong);
                var new_map = new Map {
                    vals = (ulong*) Marshal.AllocHGlobal((IntPtr)cap_bytes),
                    cap = new_cap,
                    keys = (ulong*) Marshal.AllocHGlobal((IntPtr)cap_bytes),
                };

                Unsafe.InitBlock(new_map.keys, 0, (uint)cap_bytes);

                for (int i = 0; i < cap; i++)
                {
                    if (keys[i] != 0)
                    {
                        new_map.map_put_uint64_from_uint64(keys[i], vals[i]);
                    }
                }
                Marshal.FreeHGlobal((IntPtr)keys);
                Marshal.FreeHGlobal((IntPtr)vals);
                this = new_map;
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void map_put_uint64_from_uint64(ulong key, ulong val)
            {
                assert(key != 0);
                assert(val != 0);
                if (2 * len >= cap)
                {
                    map_grow(2 * cap);
                }
                assert(2 * len < cap);
                assert(IS_POW2(cap));
                size_t i = (size_t)hash_uint64(key);
                for (; ; )
                {
                    i &= cap - 1;
                    if (keys[i] == 0)
                    {
                        len++;
                        keys[i] = key;
                        vals[i] = val;
                        return;
                    }
                    else if (keys[i] == key)
                    {
                        vals[i] = val;
                        return;
                    }
                    i++;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void* map_get(void* key) => (void*)map_get_uint64_from_uint64((ulong)key);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void map_put(void* key, void* val) => map_put_uint64_from_uint64((ulong)key, (ulong)val);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void* map_get_from_uint64(ulong key) => (void*)map_get_uint64_from_uint64(key);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void map_put_from_uint64(ulong key, void* val) => map_put_uint64_from_uint64(key, (ulong)val);
        }
        #region Std Functions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isalnum(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isspace(char c) => c == ' ' || c == '\n' || c == '\r' || c == '\t' || c == '\v';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool islower(char c) => c >= 'a' && c <= 'z';
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isupper(char c) => c >= 'A' && c <= 'Z';
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isdigit(char c) => c >= '0' && c <= '9';


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char tolower(char c) =>c >= 'a' ? c : (char)(c + 32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strcmp(char* c1, char* c2) {
            while(*c1++ == *c2++)
                if (*c1 == '\0' || *c2 == '\0')
                    return 0;

            return 1;
        }

        #endregion
    }
}
