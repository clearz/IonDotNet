using System;
using static System.Console;

namespace Lang
{
    unsafe partial class Ion
    {
        static void Main() {

            var ion = new Ion();
            ion.lex_init();

            ion.resolve_test();
            //Timer.Time(13, ion.resolve_test);
            //WriteLine();
            // Timer.Time2(10, ion.resolve_test);
            ReadKey();
			ion.init_parse_test();
			ion.lex_test();
            WriteLine("\n  --print_test\n");
            ReadKey();
            ion.print_test();
            WriteLine("\n  --parse_test_and_print\n");
            ReadKey();
            ion.parse_test_and_print();
            WriteLine("\n  --resolve_test\n");
            ReadKey();
            ion.resolve_test();
            ReadKey();


        }


    }

}
