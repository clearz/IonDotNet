using System;
using System.IO;

namespace IonLangManaged
{
    unsafe partial class Ion
    {   
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
            const string BASE_PATH = @"../../../../../ion-pkgs";
#if X64
            string ARCH = "x64";
#else
            string ARCH = "x86";
#endif
            var dir = new DirectoryInfo(BASE_PATH);

            Environment.SetEnvironmentVariable("IONHOME", dir.FullName, EnvironmentVariableTarget.Process);
            var b = ion_main(new []{pkg, "-z", "-v", "-a", ARCH, "-s", "win32", "-o", @$"{dir.Parent.FullName}\TestCompiler\test.c" });
            assert(b == 0);
        }
    }
}
