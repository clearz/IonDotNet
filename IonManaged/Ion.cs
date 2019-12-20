using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IonLangManaged
{
    using static SymReachable;

    public unsafe partial class Ion
    {
        string output_name = null;
        const string MAIN_NAME = "main";

        List<string> package_search_paths;
        bool flag_verbose = false, flag_lazy = false;
        bool flag_nosync = false;
        bool flag_notypeinfo = false, flag_fullgen = false; 
        bool flag_check = false, flag_raw_code = false;


        void add_package_search_path(string path) {
            if (flag_verbose)
                print("Adding package search path '{0}'", path);
            package_search_paths.Add(path);
        }

        void init_package_search_paths() {
            string ionhome_var = Environment.GetEnvironmentVariable("IONHOME");
            if (string.IsNullOrEmpty(ionhome_var)) {
                error_here("Set the environment variable IONHOME to the Ion home directory (where system_packages is located)");
            }
            package_search_paths = new List<string>();
            string path = ionhome_var + "\\system_packages";
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
            decl_note_names.Add(declare_note_name);
        }

        bool parse_env_vars(string[] args) {
            flag_verbose = args.Contains("-v");
            flag_lazy = args.Contains("-z");
            flag_nosync = args.Contains("-l");
            flag_fullgen = args.Contains("-f");
            flag_notypeinfo = args.Contains("-g");
            flag_check = args.Contains("-c");
            flag_raw_code = args.Contains("-r");

            int pos;
            var sys = Environment.GetEnvironmentVariable("IONOS");
            if (sys != null) {
                target_os = get_os(sys);
                if (target_os == 0)
                    print("Unknown operating system in IONARCH environment variable: {0}", sys);
            }
            else {
                pos = Array.IndexOf(args, "-s");
                if (pos != -1) {
                    if (pos < args.Length) {
                        target_os = get_os(args[pos + 1]);
                    }
                    else
                        return false;
                }
            }
            var arch = Environment.GetEnvironmentVariable("IONARCH");
            if (sys != null) {
                target_arch = get_arch(arch);
                if (target_os == 0)
                    print("Unknown target architecture in IONARCH environment variable: {0}", sys);
            }
            else {
                pos = Array.IndexOf(args, "-a");
                if (pos != -1) {
                    if (pos < args.Length) {
                        target_arch = get_arch(args[pos + 1]);
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
            print("Usage: {0} <package> [flags]", Assembly.GetEntryAssembly()?.FullName);
            print("\t-s\t<Target operating system>");
            print("\t-a\t<Target machine architecture>");
            print("\t-z\tOnly compile what's reachable from the main package");
            print("\t-g\tDon't generate any typeinfo tables");
            print("\t-l\tDon't generate any line-info");
            print("\t-f\tForce full code generation even for non-reachable symbols");
            print("\t-c\tSemantic checking with no code generation");
            print("\t-v\tExtra diagnostic information");
            return 2;
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
                print("Target operating system: {0}", os_names[(int)target_os]);
                print("Target architecture: {0}", arch_names[(int)target_arch]);
            }

            builtin_package = import_package("builtin");
            if (builtin_package == null) {
                error_here("Failed to compile package 'builtin'.");
            }
            builtin_package.external_name = "";
            enter_package(builtin_package);
            postinit_builtin();
            Sym any_sym = resolve_name("any");
            if (any_sym == null || any_sym.kind != SymKind.SYM_TYPE) {
                printf("error: Any type not defined");
                return 1;
            }
            type_any = any_sym.type;
            leave_package(builtin_package);
            fixed(char* c = package_name)
                for (char* ptr = c; *ptr != '\0'; ptr++) {
                    if (*ptr == '.') {
                        *ptr = '/';
                    }
                }
            Package main_package = import_package(package_name);
            if (main_package == null) {
                error_here("Failed to compile package '{0}'", package_name);
            }

            Sym main_sym = get_package_sym(main_package, MAIN_NAME);
            if (!flag_raw_code) {
                if (main_sym == null) {
                    error_here("No 'main' entry point defined in package '{0}'", package_name);
                }
                main_sym.external_name = MAIN_NAME;
                resolve_sym(main_sym);
            }
            reachable_phase = REACHABLE_NATURAL;

            for (int i = 0; i < package_list.Count; i++) {
                var pkg = package_list[i];
                if (pkg.always_reachable) {
                    resolve_package_syms(pkg);
                }
            }

            finalize_reachable_syms();
            if (flag_verbose) {
                print("Reached {0} symbols in {1} packages from {2}/main", reachable_syms.Count, package_list.Count, package_name);
            }
            if (!flag_lazy) {
                reachable_phase = REACHABLE_FORCED;
                for (int i = 0; i < package_list.Count; i++) {
                    var pkg = package_list[i];
                    resolve_package_syms(pkg);
                }
                finalize_reachable_syms();
            }

            print("Processed {0} symbols in {1} packages", reachable_syms.Count, package_list.Count);

            if (!flag_check) {
                print("Generating {0}", output_name);
                gen_all();

                output_name = output_name ?? $"out_{package_name}.c";

                try {
                    File.WriteAllText(output_name, gen_buf.ToString());
                }
                catch {
                    print("error: Failed to write file: {0}", output_name);
                    return 1;
                }
            }
            return 0;
        }
    }
}