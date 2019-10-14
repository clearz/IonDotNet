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

        void add_package_search_path(string path) {
            printf("Adding package search path {0}\n", path);
            package_search_paths->Add(path.ToPtr());
        }

        void init_package_search_paths() {
            string ionhome_var = Environment.GetEnvironmentVariable("IONHOME");
            if (string.IsNullOrEmpty(ionhome_var)) {
                error_here("Set the environment variable IONHOME to the Ion home directory (where system_packages is located)\n");
            }
            string sys_path = ionhome_var + "\\system_packages";
            add_package_search_path(sys_path);
            add_package_search_path(ionhome_var);
            string ionpath_var = Environment.GetEnvironmentVariable("IONPATH");
            if (!string.IsNullOrEmpty(ionpath_var)) {
                foreach (var d in ionpath_var.Split(';'))
                    add_package_search_path(d);
            }
        }

        void init_compiler() {
            lex_init();
            init_package_search_paths();
            init_keywords();
            init_types();
            decl_note_names.map_put(declare_note_name, (void*)1);
        }

        private void ion_test(string pkg) {
            Environment.SetEnvironmentVariable("IONHOME", @"..\..\..\..", EnvironmentVariableTarget.Process);
            var b = ion_main(new []{pkg, "-o", @"..\..\..\..\..\TestCompiler\test.c"});
            assert(b == 0);
        }

        int usage() {
            printf("Usage: {0} <package> [-o <output-c-file>]\n", Assembly.GetEntryAssembly()?.FullName);
            return 1;
        }

        private int ion_main(string[] args) {
            if (args.Length == 0) {
                return usage();
            }
            var package_name = args[0];
            initialise();

            builtin_package = import_package("builtin".ToPtr());
            if (builtin_package == null) {
                error_here("Failed to compile package 'builtin'.\n");
            }
            builtin_package->external_name = _I("");
            init_builtin_syms();
            Package *main_package = import_package(package_name.ToPtr());
            if (main_package == null) {
                error_here("Failed to compile package '{0}'\n", package_name);
            }
            char *main_name = _I("main");
            Sym *main_sym = get_package_sym(main_package , main_name);
            if (main_sym == null) {
                error_here("No 'main' entry point defined in package '{0}'\n", package_name);
            }

            main_sym->external_name = main_name;
            resolve_package_syms(builtin_package);
            resolve_package_syms(main_package);

            finalize_reachable_syms();
            printf("Compilation succeeded.\n");

            string c_path;
            if (args.Any(s => s == "-o")) {
                int pos = Array.IndexOf(args, "-o");
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