using System;
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
    #region Typedefs
#if X64
    using size_t = Int64;
    using uptr_t = UInt64;
#else
    using size_t = Int32;
    using uptr_t = UInt32;
#endif
    #endregion

    unsafe partial class Ion
    {
        #region Macros*

        public static ulong MAX(ulong a, ulong b) => a > b ? a : b;
        public static size_t MAX(size_t a, size_t b) => a > b ? a : b;
        public static size_t MIN(size_t a, size_t b) => a < b ? a : b;
        public static size_t CLAMP_MIN(size_t x, size_t min) => MAX(x, min);
        public static size_t CLAMP_MAX(size_t x, size_t max) => MIN(x, max);
        public static bool IS_POW2(size_t x) => x != 0 && (x & x - 1) == 0;
        public static bool IS_POW2(ulong x) => x != 0 && (x & x - 1) == 0;
        public static size_t ALIGN_DOWN(size_t n, size_t a) => n & ~(a - 1);
        public static size_t ALIGN_UP(size_t n, size_t a) => n + a - 1 & ~(a - 1);
        public static unsafe void* ALIGN_DOWN_PTR(void* p, size_t a) => (void*)ALIGN_DOWN((size_t)p, a);
        public static void* ALIGN_UP_PTR(void* p, size_t a) => (void*)ALIGN_UP((size_t)p, a);

        #endregion

        #region MemAlloc

        internal static void memcpy(void* dst, void* src, size_t len) => Unsafe.CopyBlock(dst, src, (uint)len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void* xcalloc(size_t num_elems, size_t elem_size)
        {
            size_t size = num_elems * elem_size;
            void* v = xmalloc(size);
            assert(size < uint.MaxValue);
            Unsafe.InitBlock(v, 0, (uint)size);
            return v;
        }

        internal static T* xcalloc<T>(size_t num_elems = 1) where T : unmanaged => xcalloc<T>(num_elems, sizeof(T));
        internal static T* xcalloc<T>(size_t num_elems, size_t elem_size) where T : unmanaged => (T*)xcalloc(num_elems, elem_size);
        internal static void* xrealloc(void* ptr, size_t num_bytes) => (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes); //realloc(ptr, num_bytes);
        internal static T* xrealloc<T>(T* ptr, size_t num_bytes) where T : unmanaged => (T*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)num_bytes); //(T*)realloc(ptr, num_bytes);
        internal static void* xmalloc(size_t num_bytes) => (void*)Marshal.AllocHGlobal((IntPtr)num_bytes); //malloc(num_bytes);
        internal static ref T rmalloc<T>() where T : unmanaged => ref *xmalloc<T>(); // useful?
        internal static T* xmalloc<T>() where T : unmanaged => (T*)xmalloc(sizeof(T));
        internal static T* xmalloc<T>(size_t num_bytes) where T : unmanaged => (T*)xmalloc(num_bytes);

        internal static void xfree(void* ptr) => Marshal.FreeHGlobal((IntPtr)ptr);

        internal static void* memdup(void* src, size_t size)
        {
            void* dest = xmalloc(size);
            Unsafe.CopyBlock(dest, src, (uint)size);
            return dest;
        }

        static char* read_file(string path)
        {
            var buf = File.ReadAllText(path).ToPtr();
            return buf;
        }

        static bool write_file(string path, char* buf)
        {
            File.WriteAllText(path, new String(buf));
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
            size_t base_len = (size_t)(ext - path);
            size_t new_ext_len = (size_t)strlen(new_ext);
            size_t new_path_len = base_len + new_ext_len;
            char* new_path = xmalloc<char>(new_path_len + 1);
            memcpy(new_path, path, base_len * 2);
            memcpy(new_path + base_len, new_ext, new_ext_len * 2);
            new_path[new_path_len] = '\0';
            return new string(new_path);
        }

        #endregion
        public unsafe delegate void MemcpyDelegate(void* dest, void* src, size_t len);
        public unsafe delegate void InitBlockDelegate(void* dest, byte src, size_t len);
        public unsafe static InitBlockDelegate InitBlock;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset(void* array, byte what, size_t length) { }
        public static void Memcpy(void* array, void* what, size_t length) { }
        public static readonly MemcpyDelegate CopyBlock;
        static Ion()
        {
            var dynamicMethod = new DynamicMethod("Memset", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard,
                null, new[] { typeof(void*), typeof(byte), typeof(size_t) }, typeof(Ion), true);

            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Initblk);
            generator.Emit(OpCodes.Ret);

            InitBlock = (InitBlockDelegate)dynamicMethod.CreateDelegate(typeof(InitBlockDelegate));

            dynamicMethod = new DynamicMethod("Memcpy", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard,
                null, new[] { typeof(void*), typeof(void*), typeof(size_t) }, typeof(Ion), true);
            generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Cpblk);
            generator.Emit(OpCodes.Ret);

            CopyBlock = (MemcpyDelegate)dynamicMethod.CreateDelegate(typeof(MemcpyDelegate));

        }
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
            private const uint START_CAPACITY = 8,
                              MULTIPLIER = 2;

            public uint count, buffer_size, item_size;
            private uint _capacity, _multiplier;

            public static implicit operator T*(Buffer<T> b) => b._begin;

            public static Buffer<T> Create(uint capacity = START_CAPACITY, uint multiplier = MULTIPLIER)
            {
                //assert(capacity >= START_CAPACITY);
                assert(multiplier > 1);
                var b = new Buffer<T>
                {
                    item_size = (uint)sizeof(T),
                    _capacity = capacity,
                    _multiplier = multiplier,
                    count = 0
                };
                b.buffer_size = b._capacity * b.item_size;
                b._begin = (T*)xmalloc((size_t)b.buffer_size);
                b._top = b._begin;
                return b;
            }

            public void Add(T val)
            {
                Write(_top, ref val);

                if (++count == _capacity)
                {
                    _capacity *= _multiplier;
                    // Console.WriteLine("Size1: " + _capacity);
                    buffer_size = _capacity * item_size;
                    _begin = (T*)xrealloc(_begin, (size_t)buffer_size);
                    _top = _begin + count;
                }
                else
                {
                    _top++;
                }

            }

            public void Add(T* val)
            {
                Write(_top, ref *val);
                if (++count == _capacity)
                {
                    _capacity *= _multiplier;
                    buffer_size = _capacity * item_size;
                    _begin = (T*)xrealloc(_begin, (size_t)buffer_size);
                    _top = _begin + count;
                }
                else
                {
                    _top++;
                }

            }
            internal void Add(T* c, uint len)
            {
                if ((count += len) >= _capacity)
                {
                    _capacity *= _multiplier;
                    // Console.WriteLine("Size2: " + _capacity);
                    assert(_capacity > count);
                    buffer_size = _capacity * item_size;
                    _begin = (T*)xrealloc(_begin, (size_t)buffer_size);
                    _top = _begin + (_capacity >> 1);
                }
                Unsafe.CopyBlock(_top, c, len << 1);
                _top += len;
            }
            public void free()
            {
                xfree(_begin);
            }


            public bool fits(size_t n) => n <= _capacity - count;


            public void clear()
            {
                _top = _begin;
                count = 0;
            }



            internal T* _begin;
            internal T* _top
            {
                get;
                set;
            }

        }


        internal struct PtrBuffer
        {
            private const int START_CAPACITY = 8,
                              MULTIPLIER = 2;

            internal void** _begin, _top, _end;

            public int count, buf_byte_size;
            private int _capacity, _multiplier;


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


            public void free()
            {
                Marshal.FreeHGlobal((IntPtr)_begin);
            }


            public bool fits(size_t n) => n <= _capacity - count;


            public void clear()
            {
                _top = _begin;
                count = 0;
            }


            public static void* Array(params void*[] ptrs)
            {
                uptr_t* array = (uptr_t*)xmalloc(PTR_SIZE * ptrs.Length);
                for (var i = 0; i < ptrs.Length; i++)
                {
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

            const int ARENA_ALIGNMENT = 8;
            const int ARENA_BLOCK_SIZE = 1024 * 1024;


            void arena_grow(size_t min_size)
            {
                // Console.WriteLine("Growing: " + arenas->count);
                size_t size = ALIGN_UP(MAX(ARENA_BLOCK_SIZE, min_size), ARENA_ALIGNMENT);
                ptr = (byte*)xmalloc(size);
                assert(ptr == ALIGN_DOWN_PTR(ptr, ARENA_ALIGNMENT));
                end = ptr + size;
                arenas->Add(ptr);
            }

            public static MemArena* Create()
            {
                MemArena* arena = (MemArena*)xmalloc(sizeof(MemArena));
                arena->arenas = PtrBuffer.Create();
                arena->ptr = (byte*)xmalloc(ARENA_BLOCK_SIZE);
                assert(arena->ptr == ALIGN_DOWN_PTR(arena->ptr, ARENA_ALIGNMENT));
                arena->end = arena->ptr + ARENA_BLOCK_SIZE;
                arena->arenas->Add(arena->ptr);
                return arena;
            }

            public void* Alloc(size_t size)
            {
                var left = end - ptr;
                if (size > left)
                {
                    arena_grow(size);
                    assert(size <= (int)(end - ptr));
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
            int len;
            char* str;
            Intern* next;


            public static char* InternRange(char* start, char* end)
            {
                var len = (int)(end - start);
                ulong hash = Map.str_hash(start, (ulong)len) | 1;
                Intern* intern = (Intern*)interns.map_get_hashed((void*)hash, hash);
                if (intern != null)
                    return intern->str;

                Intern* new_intern = (Intern*)intern_arena->Alloc(sizeof(Intern));
                new_intern->len = len;
                new_intern->next = intern;

                size_t len_bytes = (len << 1);
                new_intern->str = (char*)intern_arena->Alloc(len_bytes + 2);
                Unsafe.CopyBlock(new_intern->str, start, (uint)len_bytes);
                *(new_intern->str + len) = '\0';
                interns.map_put_hashed((void*)hash, new_intern, hash);
                return new_intern->str;
            }
        }


        public static char* _I(string s)
        {
            char* c = s.ToPtr();
            return Intern.InternRange(c, c + s.Length);

        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void* calloc(int num, int size);

        struct MapEntry
        {
            public void* key;
            public void* val;
            public ulong hash;
        }
        struct Map
        {

            public MapEntry* entries;
            internal ulong len;
            internal ulong cap;



            public static ulong uint64_hash(ulong x)
            {
                x *= 0xff51afd7ed558ccd;
                x ^= x >> 32;
                return x;
            }



            public static ulong ptr_hash(void* ptr)
            {
                return uint64_hash((ulong)ptr);
            }


            public static ulong str_hash(char* str, ulong len)
            {
                ulong fnv_init = 14695981039346656037ul;
                ulong fnv_mul = 1099511628211ul;
                ulong h = fnv_init;
                for (ulong i = 0; i < len; i++)
                {
                    h ^= str[i];
                    h *= fnv_mul;
                }
                return h;
            }

            public static ulong ptr_hash(void* ptr, ulong len)
            {
                char* buf = (char*)ptr;
                return str_hash(buf, len);
            }



            public void* map_get_hashed(void* key, ulong hash)
            {
                if (len == 0)
                {
                    return null;
                }
                assert(IS_POW2(cap));
                ulong i = hash & (cap - 1);
                assert(len < cap);
                for (; ; )
                {
                    MapEntry* entry = entries + i;
                    if (entry->key == key)
                    {
                        return entry->val;
                    }
                    else if (entry->key == null)
                    {
                        return null;
                    }
                    i++;
                    if (i == cap)
                    {
                        i = 0;
                    }
                }
            }




            void map_grow(ulong new_cap)
            {
                new_cap = MAX(new_cap, 16);
                var cap_bytes = new_cap * (ulong)sizeof(MapEntry);
                var new_map = new Map
                {
                    entries = (MapEntry*)xmalloc((size_t)cap_bytes),
                    cap = new_cap
                };

                // _memset(new_map.entries, 0, (int)cap_bytes);
                Unsafe.InitBlock(new_map.entries, 0, (uint)cap_bytes);
                for (ulong i = 0; i < cap; i++)
                {
                    MapEntry* entry = entries + i;
                    if (entry->key != null)
                    {
                        new_map.map_put_hashed(entry->key, entry->val, entry->hash);
                    }
                }
                xfree(entries);
                this = new_map;
            }




            public void** map_put_hashed(void* key, void* val, ulong hash)
            {
                assert(key != null);
                assert(val != null);
                if (2 * len >= cap)
                {
                    map_grow(2 * cap);
                }
                assert(2 * len < cap);
                assert(IS_POW2(cap));
                ulong i = hash & (cap - 1);

                for (; ; )
                {
                    MapEntry* entry = entries + i;
                    if (entry->key == null)
                    {
                        len++;
                        entry->key = key;
                        entry->val = val;
                        entry->hash = hash;
                        return &entry->val;
                    }
                    else if (entry->key == key)
                    {
                        entry->val = val;
                        return &entry->val;
                    }
                    i++;
                    if (i == cap)
                    {
                        i = 0;
                    }
                }
            }


            public void* map_get(void* key) => map_get_hashed(key, ptr_hash(key));



            public void** map_put(void* key, void* val) => map_put_hashed(key, val, ptr_hash(key));



            public void* map_get_from_uint64(ulong key) => map_get_hashed((void*)key, ptr_hash((void*)key));



            public void** map_put_from_uint64(ulong key, void* val) => map_put_hashed((void*)key, val, uint64_hash(key));
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
                if (*c1 == '\0' || *c2 == '\0')
                    return 0;

            return 1;
        }
        public static uint strlen(char* c)
        {
            uint i = 0;
            while (*c++ != '\0') i++;

            return i;
        }

        #endregion
    }
}
