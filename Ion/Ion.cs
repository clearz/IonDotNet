using System;
using System.Linq;
using System.Reflection;

namespace IonLang
{
    public unsafe partial class Ion
    {
        char *builtin_code =
                    ("@declare_note(foreign)\n"    +
                    "\n"                           +
                    "enum TypeKind {\n"            +
                    "    TYPE_NONE,\n"             +
                    "    TYPE_VOID,\n"             +
                    "    TYPE_BOOL,\n"             +
                    "    TYPE_CHAR,\n"             +
                    "    TYPE_UCHAR,\n"            +
                    "    TYPE_SCHAR,\n"            +
                    "    TYPE_SHORT,\n"            +
                    "    TYPE_USHORT,\n"           +
                    "    TYPE_INT,\n"              +
                    "    TYPE_UINT,\n"             +
                    "    TYPE_LONG,\n"             +
                    "    TYPE_ULONG,\n"            +
                    "    TYPE_LLONG,\n"            +
                    "    TYPE_ULLONG,\n"           +
                    "    TYPE_FLOAT,\n"            +
                    "    TYPE_DOUBLE,\n"           +
                    "    TYPE_CONST,\n"            +
                    "    TYPE_PTR,\n"              +
                    "    TYPE_ARRAY,\n"            +
                    "    TYPE_STRUCT,\n"           +
                    "    TYPE_UNION,\n"            +
                    "    TYPE_FUNC,\n"             +
                    "}\n"                          +
                    "\n"                           +
                    "struct TypeFieldInfo {\n"         +
                    "    name: char const*;\n"     +
                    "    type: typeid;\n"          +
                    "    offset: int;\n"           +
                    "}\n"                          +
                    "\n"                           +
                    "struct TypeInfo {\n"          +
                    "    kind: TypeKind;\n"        +
                    "    size: int;\n"             +
                    "    align: int;\n"            +
                    "    name: char const*;\n"     +
                    "    count: int;\n"            +
                    "    base: typeid;\n"          +
                    "    fields: TypeFieldInfo*;\n"    +
                    "    num_fields: int;\n"       +
                    "}\n"                          +
                    "\n"                           +
                    "@foreign\n"                   +
                    "var typeinfos: TypeInfo const**;\n" +
                    "\n"                           +
                    "@foreign\n"                   +
                    "var num_typeinfos: int;\n"    +
                    "\n"                           +
                    "func get_typeinfo(type: typeid): TypeInfo const* {\n" +
                    "    if (typeinfos && type < num_typeinfos) {\n" +
                    "        return typeinfos[type];\n" +
                    "    } else {\n" +
                    "        return NULL;\n" +
                    "    }\n" +
                    "}\n").ToPtr();

        void init_compiler() {
            lex_init();
            init_builtins();
            init_types();
        }

        bool ion_compile_builtin() {
            init_stream(builtin_code, "<builtin>".ToPtr());
            init_compiler();
            global_decls = parse_decls();
            sym_global_decls();
            return true;
        }


        private bool ion_compile_file(string spath) {
            initialise();
            var path = spath.ToPtr();
            char *str = read_file(spath);
            if (str == null) {
                printf("Failed to read %s\n", path);
                return false;
            }
            if (!ion_compile_builtin()) {
                printf("Failed to compile builtins\n");
                return false;
            }
            init_stream(str, $"{spath}".ToPtr());
            global_decls = parse_decls();
            sym_global_decls();
            finalize_syms();
            gen_all();
            return true;
            //Console.WriteLine("Path: " + new string(path));
            var c_path = replace_ext(spath.ToPtr(), "c".ToPtr());
            if (c_path == null)
                return false;
           // if (!write_file(c_path, gen_buf))
           //     return false;
            return true;
        }

        private char* ion_compile_str(string path) {
            initialise();
            init_stream(read_file(path), $"{path}".ToPtr());
            global_decls = parse_decls();
            sym_global_decls();
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
            bool b = ion_compile_file(path);
            if (!b) {
                printf("Compilation failed.\n");
                return 1;
            }
            write_file(output, gen_buf.ToString().ToPtr2());
            //File.Copy(path, @"C:\Users\john\source\repos\Test\TestCompiler\test1.ion", true);
            printf("Compilation succeeded.\n");
            return 0;
        }
    }
}