using static System.Console;

namespace IonLang
{
    unsafe partial class Ion
    {
        bool compile_extras = true;
        private static int Main(string[] args) {
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                Timer.Time(() => ion.ion_test("test1"), 1);
#if DEBUG
               // WriteLine(ion.gen_buf);
#endif
                ReadKey();
            }
            return 0;
        }
    }
}
