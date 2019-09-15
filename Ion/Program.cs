using System;
using System.IO;
using static System.Console;
namespace IonLang
{
    using static TypeKind;

    unsafe partial class Ion
    {
        private static void Main(string[] args) {

            Ion ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                Timer.Time(() => ion.ion_compile_file("test1.ion"));
                WriteLine(ion.gen_buf);
                ReadKey();
            }

        }

    }
}