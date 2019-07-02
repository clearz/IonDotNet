using static System.Console;

namespace Lang
{ 
    unsafe partial class Ion
    {
        static void Main()
        {
#if DEBUG2
            var ion = new Ion();
            ion.lex_init();
            ion.gen_test();
#else
            var ion = new Ion();
            ion.lex_init();
            Timer.Time(1, () =>
            {
                ion.ion_compile_file("test3.ion");
            });
#endif

            ReadKey();

        }
    }

}
