using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lang
{
    public unsafe partial class Ion
    {
        private bool ion_compile_file(string spath) {
            lex_init();
            var path = spath.ToPtr();
            lex_init();
            init_stream(read_file(spath), $"\"{spath}\"".ToPtr());
            init_global_syms();
            var ds = parse_file();
            sym_global_decls(ds);
            finalize_syms();
            gen_all();
            return false;
            //Console.WriteLine("Path: " + new string(path));
            var c_path = replace_ext(spath.ToPtr(), "c".ToPtr());
            if (c_path == null)
                return false;
           // if (!write_file(c_path, gen_buf))
           //     return false;
            return true;
        }

        private char* ion_compile_str(string path) {
            lex_init();
            init_stream(read_file(path), $"\"{path}\"".ToPtr());
            init_global_syms();
            sym_global_decls(parse_file());
            finalize_syms();
            gen_all();
            return gen_buf.ToString().ToPtr();
        }

        private void ion_test() {
            var b = ion_compile_file("test1.ion");
            assert(b);
        }

        private long ion_main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <ion-source-file>\n", Assembly.GetEntryAssembly()?.FullName);
                return 1;
            }
            var path = args[0];
            string output = replace_ext(path.ToPtr(), "c".ToPtr());
            if(args.Any(s => s == "-o")) {
                int pos = Array.IndexOf(args, "-o");
                if(pos < args.Length) {
                    output = args[pos + 1];
                }
            }
            bool b = write_file(output, ion_compile_str(path));
            if (!b) {
                printf("Compilation failed.\n");
                return 1;
            }
            //File.Copy(path, @"C:\Users\john\source\repos\Test\TestCompiler\test1.ion", true);
            printf("Compilation succeeded.\n");
            return 0;
        }
    }
}