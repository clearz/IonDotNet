using System;
using static System.Console;
namespace Lang
{
    using static TypeKind;

    unsafe partial class Ion
    {
        private static void Main(string[] args)
        {
#if DEBUG2
            var ion = new Ion();
            ion.lex_init();
            ion.resolve_test();
#else
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                Timer.Time(1, () => { ion.ion_compile_file("test1.ion"); });
                WriteLine(new string(ion.gen_buf));
                ReadKey();
            }
#endif

            
        }

    }
}