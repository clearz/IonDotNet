using System;
using static System.Console;
namespace Lang
{
    using static TypeKind;

    unsafe partial class Ion
    {
        private static void Main(string[] args) {
#if TEST
            var ion = new Ion();
            ion.lex_init();
            ion.resolve_test();
#else
            if (args.Length > -1)
                new Ion().ion_main(args);
            else {
                Ion ion = new Ion();
                //ion.cdecl_test();
                //ReadKey();
                Timer.Time(() => ion.ion_compile_file("test1.ion"));
                WriteLine(ion.gen_buf);
                ReadKey();
            }
#endif

            
        }

    }
}