using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IonLang
{
    unsafe partial class Ion
    {
        [STAThread]
        static int Main(string[] args) {
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                ion.ion_test("test1");
            }
            return 0;
        }
    }
}
