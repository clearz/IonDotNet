using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lang
{
    public unsafe partial class Ion
    {
        string str = null;
        bool ion_compile_file(string spath)
        {
            var str = File.OpenText(spath).ReadToEnd();
            if (str == null)
            {
                return false;
            }
            var path = spath.ToPtr();
            init_stream(str, spath);
            init_global_syms();
            DeclSet* ds = parse_file();
            sym_global_decls(ds);
            finalize_syms();
            _gen_all();
            return false;
            string c_path = replace_ext(path, "c".ToPtr());
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

        StringBuilder ion_compile_str(char* str)
        {
            init_stream(str);
            init_global_syms();
            sym_global_decls(parse_file());
            finalize_syms();
            gen_all();
            return _gen_buf;
        }

        void ion_test()
        {
            bool b = ion_compile_file("test1.ion");
            assert(b);
        }

        int ion_main(String[] args)
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
