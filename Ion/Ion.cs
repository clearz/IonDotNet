using System;
using System.IO;
using System.Reflection;

namespace Lang
{
    public unsafe partial class Ion
    {
        bool ion_compile_file(string spath)
        {
            var path = spath.ToPtr();
            init_stream(read_file(spath), path);
            init_global_syms();
            DeclSet* ds = parse_file();
            sym_global_decls(ds);
            finalize_syms();
            _gen_all();
            //Console.WriteLine(new string(gen_buf));
            //return false;
            //Console.WriteLine("Path: " + new string(path));
            string c_path = replace_ext(spath.ToPtr(), "c".ToPtr());
            if (c_path == null)
            {
                return false;
            }
            if (!write_file(c_path, gen_buf))
            {
                return false;
            }
            return true;
        }

        char* ion_compile_str(char* str)
        {
            init_stream(str);
            init_global_syms();
            sym_global_decls(parse_file());
            finalize_syms();
            _gen_all();
            return gen_buf;
        }

        void ion_test()
        {
            bool b = ion_compile_file("test1.ion");
            assert(b);
        }

        long ion_main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: {0} <ion-source-file>\n", Assembly.GetEntryAssembly().FullName);
                return 1;
            }
            init_keywords();
            var path = args[1];
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
