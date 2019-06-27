using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static System.Console;

namespace Lang
{        /*
            int x
            int (*x)
            int (x[10])
            int (*x)(int)

            int (*(x[10]))(int)

            int (*x)(int *)
            int (*x)(int [10])
            int (*(*x)(void))(int [10])
            int (*((*x)(void)[10]))(void)

            int x
            int (*x)
            int (x[10])
            int (*x)(int)

            int *(int) (x[10])

            int (*x)(int)
            int (*x)(int)
            int *(int) (*x)(void
            int *(void (*x)(void

        */
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
        static void Main()
        {
#if DEBUG
            var ion = new Ion();
            ion.lex_init();
            ion.gen_test();
            ReadLine();
#else
            var ion = new Ion();
            ion.lex_init();
            Timer.Time(1, () =>
            {
                ion.ion_compile_file("test3.ion");
            });
#endif
            //ion.gen_test();
            // ion.ion_compile_file("test3.ion");
            //Timer.Time(1, () => ion.ion_compile_file("test3.ion"));
            //Timer.Time2(1, () => ion.ion_compile_file("test3.ion"));
            // Timer.Time(1000000, ion.cdecl_test2);
            //Console.WriteLine(new String(ion.gen_buf));
            ReadKey();



        }
    }

}
