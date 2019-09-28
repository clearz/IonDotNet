using static System.Console;

namespace IonLang
{
    partial class Ion
    {
        private static int Main(string[] args) {
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                Timer.Time(() => ion.ion_test(), 1);
                WriteLine(ion.gen_buf);
                ReadKey();
            }
            return 0;
        }
    }
}
