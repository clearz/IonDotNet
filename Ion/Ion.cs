using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IonLang
{
    public unsafe partial class Ion
    {
        const int MAX_SEARCH_PATHS = 256;
        PtrBuffer* package_search_paths = PtrBuffer.Create();
        bool flag_verbose = false, flag_lazy = false;
        void add_package_search_path(string path) {
            if (flag_verbose)
                printf("Adding package search path {0}\n", path);
            package_search_paths->Add(path.ToPtr());
        }

        void init_package_search_paths() {
            string ionhome_var = Environment.GetEnvironmentVariable("IONHOME");
            if (string.IsNullOrEmpty(ionhome_var)) {
                error_here("Set the environment variable IONHOME to the Ion home directory (where system_packages is located)\n");
            }
            string path = ionhome_var + "/system_packages";
            add_package_search_path(path);
            add_package_search_path(ionhome_var);
            add_package_search_path(".");
            string ionpath_var = Environment.GetEnvironmentVariable("IONPATH");
            if (!string.IsNullOrEmpty(ionpath_var)) {
                foreach (var d in ionpath_var.Split(';'))
                    add_package_search_path(d);
            }
        }

        void init_compiler() {
            init_target();
            lex_init();
            init_package_search_paths();
            init_keywords();
            init_builtin_types();
            decl_note_names.map_put(declare_note_name, (void*)1);
        }

        private void ion_test(string pkg) {
#if X64
            string BACK_PATH = "../../../.."; // bin/x64/debug
            string ARCH = "x64";
#else
            string BACK_PATH = @"../../../.."; // bin/debug
            string ARCH = "x86";
#endif
            var dir = new DirectoryInfo(BACK_PATH);

            Environment.SetEnvironmentVariable("IONHOME", dir.FullName, EnvironmentVariableTarget.Process);
            var b = ion_main(new []{pkg, "-v", "-hl", "-a", ARCH, "-s", "win32", "-o", @$"{dir.Parent.FullName}\TestCompiler\test.c" });
            assert(b == 0);
        }

        int usage() {
            printf("Usage: {0} <package> [-v -l] [-o <output-c-file>] [-a <architecture>] [-s <system-os>]\n", Assembly.GetEntryAssembly()?.FullName);
            return 1;
        }

        private int ion_main(string[] args) {
            if (args.Length == 0) {
                return usage();
            }
            init_target_names();
            var package_name = args[0];
            flag_verbose = args.Contains("-v");
            flag_lazy = args.Contains("-l");
            int pos = Array.IndexOf(args, "-s");
            if (pos != -1) {
                if (pos < args.Length) {
                    target_os = get_os(args[pos + 1].ToPtr());
                }
                else
                    return usage();
            }
            pos = Array.IndexOf(args, "-a");
            if (pos != -1) {
                if (pos < args.Length) {
                    target_arch = get_arch(args[pos + 1].ToPtr());
                }
                else
                    return usage();
            }

            initialise();
            if (flag_verbose) {
                printf("Target operating system: {0}\n", os_names[(int)target_os]);
                printf("Target architecture: {0}\n", arch_names[(int)target_arch]);
            }
            builtin_package = import_package("builtin".ToPtr());
            if (builtin_package == null) {
                error_here("Failed to compile package 'builtin'.\n");
            }
            builtin_package->external_name = _I("");
            Package *main_package = import_package(package_name.ToPtr());
            if (main_package == null) {
                error_here("Failed to compile package '{0}'\n", package_name);
            }
            char *main_name = _I("main");
            Sym *main_sym = get_package_sym(main_package, main_name);
            if (main_sym == null) {
                error_here("No 'main' entry point defined in package '{0}'\n", package_name);
            }

            main_sym->external_name = main_name;
            resolve_package_syms(builtin_package);
            resolve_package_syms(main_package);
            if (!flag_lazy) {
                for (int i = 0; i < package_list->count; i++) {
                    resolve_package_syms(package_list->Get<Package>(i));
                }
            }
            finalize_reachable_syms();
            if (flag_verbose)
                printf("Compiled {0} symbols in {1} packages\n", reachable_syms->count, package_list->count);

            string c_path;
            pos = Array.IndexOf(args, "-o");
            if (pos != -1) {
                if (pos < args.Length) {
                    c_path = args[pos + 1];
                }
                else
                    return usage();
            }
            else {
                c_path = $"out_{package_name}.c";
            }
            printf("Generating {0}\n", c_path);
            gen_all();
            try {
                File.WriteAllText(c_path, gen_buf.ToString());
            }
            catch {
                printf("error: Failed to write file: {0}\n", c_path);
                return 1;
            }

            return 0;
        }
    }
}