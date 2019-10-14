using System;
using System.Runtime.CompilerServices;

namespace IonLang
{
    partial class Ion
    {
        private static int Main(string[] args) {
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                try {
                    ion.ion_test("test");
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
            return 0;
        }
    }
}
