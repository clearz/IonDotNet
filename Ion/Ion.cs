﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IonLang
{
    using static SymReachable;

    public unsafe partial class Ion
    {
        PtrBuffer* package_search_paths = PtrBuffer.Create();
        bool flag_verbose = false, flag_lazy = false;
        bool flag_skip_lines = false;
        bool flag_notypeinfo = false, flag_fullgen = false; 
        bool flag_check = false;
        string output_name = null;

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

        bool parse_env_vars(string[] args) {
            flag_verbose = args.Contains("-v");
            flag_lazy = args.Contains("-z");
            flag_skip_lines = args.Contains("-l");
            flag_fullgen = args.Contains("-f");
            flag_notypeinfo = args.Contains("-g");
            flag_check = args.Contains("-c");

            int pos;
            var sys = Environment.GetEnvironmentVariable("IONOS");
            if (sys != null) {
                target_os = get_os(sys.ToPtr());
                if (target_os == 0)
                    printf("Unknown operating system in IONARCH environment variable: {0}\n", sys);
            }
            else {
                pos = Array.IndexOf(args, "-s");
                if (pos != -1) {
                    if (pos < args.Length) {
                        target_os = get_os(args[pos + 1].ToPtr());
                    }
                    else
                        return false;

                }
            }
            var arch = Environment.GetEnvironmentVariable("IONARCH");
            if (sys != null) {
                target_arch = get_arch(arch.ToPtr());
                if (target_os == 0)
                    printf("Unknown target architecture in IONARCH environment variable: {0}\n", sys);
            }
            else {
                pos = Array.IndexOf(args, "-a");
                if (pos != -1) {
                    if (pos < args.Length) {
                        target_arch = get_arch(args[pos + 1].ToPtr());
                    }
                    else
                        return false;

                }
            }

            pos = Array.IndexOf(args, "-o");
            if (pos != -1) {
                if (pos < args.Length) {
                    output_name = args[pos + 1];
                }
                else
                    return false;
            }
            return true;
        }

        int usage() {
            printf("Usage: {0} <package> [flags]\n", Assembly.GetEntryAssembly()?.FullName);
            printf("\t\t-s\t<Target operating system>\n");
            printf("\t\t-a\t<Target machine architecture>\n");
            printf("\t\t-z\tOnly compile what's reachable from the main package\n");
            printf("\t\t-g\tDon't generate any typeinfo tables\n");
            printf("\t\t-l\tDon't generate any line-info\n");
            printf("\t\t-f\tForce full code generation even for non-reachable symbols\n");
            printf("\t\t-c\tSemantic checking with no code generation\n\n");
            printf("\t\t-v\tExtra diagnostic information\n\n");
            return 1;
        }

        void ion_test(string pkg) {
#if X64
            string BACK_PATH = "../../../.."; // bin/x64/debug
            string ARCH = "x64";
#else
            string BACK_PATH = @"../../../.."; // bin/debug
            string ARCH = "x86";
#endif
            var dir = new DirectoryInfo(BACK_PATH);

            Environment.SetEnvironmentVariable("IONHOME", dir.FullName, EnvironmentVariableTarget.Process);
            var b = ion_main(new []{pkg, "-l", "-a", ARCH, "-s", "win32", "-o", @$"{dir.Parent.FullName}\TestCompiler\test.c" });
            assert(b == 0);
        }

        int ion_main(string[] args) {
            if (args.Length == 0) {
                return usage();
            }
            init_target_names();
            var package_name = args[0];

            if (!parse_env_vars(args))
                return usage();

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
            reachable_phase = (byte)REACHABLE_NATURAL;
            resolve_sym(main_sym);
            for (int i = 0; i < package_list->count; i++) {
                var pkg = package_list->Get<Package>(i);
                if (pkg->always_reachable) {
                    resolve_package_syms(pkg);
                }
            }
            finalize_reachable_syms();

            if (flag_verbose) {
                printf("Reached {0} symbols in {1} packages from {2}/main\n", reachable_syms->count, package_list->count, package_name);
            }
            if (!flag_lazy) {
                reachable_phase = (byte)REACHABLE_FORCED;
                for (int i = 0; i < package_list->count; i++) {
                    resolve_package_syms(package_list->Get<Package>(i));
                }
                finalize_reachable_syms();
            }

            printf("Processed {0} symbols in {1} packages\n", reachable_syms->count, package_list->count);

            if (!flag_check) {
                printf("Generating {0}\n", output_name);
                gen_all();

                output_name = output_name ?? $"out_{package_name}.c";

                try {
                    File.WriteAllText(output_name, gen_buf.ToString());
                }
                catch {
                    printf("error: Failed to write file: {0}\n", output_name);
                    return 1;
                }
            }
            return 0;
        }
    }
}