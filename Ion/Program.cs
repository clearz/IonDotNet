using System;
using System.IO;

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

        void ion_test(string pkg) {
#if X64
            string BACK_PATH = @"C:\Users\john\source\repos\IonDotNet\Ion";
            string ARCH = "x64";
#else
            string BACK_PATH = @"C:\Users\john\source\repos\IonDotNet\Ion";
            string ARCH = "x86";
#endif
            var dir = new DirectoryInfo(BACK_PATH);

            Environment.SetEnvironmentVariable("IONHOME", dir.FullName, EnvironmentVariableTarget.Process);
            var b = ion_main(new []{pkg, "-z", "-sl", "-a", ARCH, "-s", "win32", "-o", @$"{dir.Parent.FullName}\TestCompiler\test.c" });
            assert(b == 0);
        }
    }
}
