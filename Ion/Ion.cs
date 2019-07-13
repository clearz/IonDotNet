using System;
using System.Reflection;

namespace Lang
{
    public unsafe partial class Ion
    {
        private bool ion_compile_file(string spath)
        { 
            lex_init();
            var path = spath.ToPtr();
            lex_init();
            init_stream(read_file(spath), $"\"{spath}\"".ToPtr2());
            init_global_syms();
            var ds = parse_file();
            sym_global_decls(ds);
            finalize_syms();
            _gen_all();
            //Console.WriteLine(new string(gen_buf));
            return false;
            //Console.WriteLine("Path: " + new string(path));
            var c_path = replace_ext(spath.ToPtr(), "c".ToPtr());
            if (c_path == null) return false;
            if (!write_file(c_path, gen_buf)) return false;
            return true;
        }

        private char* ion_compile_str(char* str)
        {
            lex_init();
            init_stream(str);
            init_global_syms();
            sym_global_decls(parse_file());
            finalize_syms();
            _gen_all();
            return gen_buf;
        }

        private void ion_test()
        {
            var b = ion_compile_file("test1.ion");
            assert(b);
        }

        private long ion_main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: {0} <ion-source-file>\n", Assembly.GetEntryAssembly()?.FullName);
                return 1;
            }

            var path = args[0];
            lex_init();
            if (!ion_compile_file(path))
            {
                printf("Compilation failed.\n");
                return 1;
            }

            printf("Compilation succeeded.\n");
            return 0;
        }
    }
}