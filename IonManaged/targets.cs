namespace IonLangManaged
{
    using static OS;
    using static Arch;
    using static TypeKind;
    using System;

    internal enum Arch
    {
        ARCH_X64 = 1,
        ARCH_X86,
        NUM_ARCHES,
    }

    internal enum OS
    {
        OS_WIN32 = 1,
        OS_LINUX,
        OS_OSX,
        NUM_OSES,
    }

    public unsafe partial class Ion
    {

        OS target_os;
        Arch target_arch;

        string[] os_names = new string[(int)NUM_OSES];
        string[] arch_names = new string[(int)NUM_ARCHES];

        TypeMetrics[] type_metrics;

        TypeMetrics[] win32_x86_metrics;
        TypeMetrics[] win32_x64_metrics;
        TypeMetrics[] ilp32_metrics;
        TypeMetrics[] lp64_metrics;

        OS get_os(string name) {
            for (int i = 1; i < (int)NUM_OSES; i++) {
                if (os_names[i] ==  name) {
                    return (OS)i;
                }
            }
            return 0;
        }

        Arch get_arch(string name) {
            for (int i = 1; i < (int)NUM_ARCHES; i++) {
                if (arch_names[i] ==  name) {
                    return (Arch)i;
                }
            }
            return 0;
        }
        void init_target_names() {

            os_names[(int)OS_WIN32] = "win32";
            os_names[(int)OS_LINUX] = "linux";
            os_names[(int)OS_OSX] = "osx";
            arch_names[(int)ARCH_X64] = "x64";
            arch_names[(int)ARCH_X86] = "x86";
        }
        void init_target() {
            type_metrics = null;
            win32_x86_metrics = new TypeMetrics[(int)NUM_TYPE_KINDS];
            win32_x64_metrics = new TypeMetrics[(int)NUM_TYPE_KINDS];
            ilp32_metrics = new TypeMetrics[(int)NUM_TYPE_KINDS];
            lp64_metrics = new TypeMetrics[(int)NUM_TYPE_KINDS];

            Span<TypeMetrics> default_metrics = stackalloc TypeMetrics[(int)NUM_TYPE_KINDS];
            default_metrics[(int)TYPE_BOOL] = new TypeMetrics { size = 1, align = 1 };
            default_metrics[(int)TYPE_CHAR] = new TypeMetrics { size = 1, align = 1, max = 0x7f, sign = true };
            default_metrics[(int)TYPE_SCHAR] = new TypeMetrics { size = 1, align = 1, max = 0x7f, sign = true };
            default_metrics[(int)TYPE_UCHAR] = new TypeMetrics { size = 1, align = 1, max = 0xff };
            default_metrics[(int)TYPE_SHORT] = new TypeMetrics { size = 2, align = 2, max = 0x7fff, sign = true };
            default_metrics[(int)TYPE_USHORT] = new TypeMetrics { size = 2, align = 2, max = 0xffff };
            default_metrics[(int)TYPE_INT] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            default_metrics[(int)TYPE_UINT] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff };
            default_metrics[(int)TYPE_LLONG] = new TypeMetrics { size = 8, align = 8, max = 0x7fffffffffffffff, sign = true };
            default_metrics[(int)TYPE_ULLONG] = new TypeMetrics { size = 8, align = 8, max = 0xffffffffffffffff };
            default_metrics[(int)TYPE_FLOAT] = new TypeMetrics { size = 4, align = 4 };
            default_metrics[(int)TYPE_DOUBLE] = new TypeMetrics { size = 8, align = 8 };

            win32_x86_metrics = default_metrics.ToArray();
            win32_x86_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 4, align = 4 };
            win32_x86_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            win32_x86_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            win32_x64_metrics = default_metrics.ToArray();
            win32_x64_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 8, align = 8 };
            win32_x64_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            win32_x64_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            ilp32_metrics = default_metrics.ToArray();
            ilp32_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 4, align = 4 };
            ilp32_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            ilp32_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            lp64_metrics = default_metrics.ToArray();
            lp64_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 8, align = 8 };
            lp64_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 8, align = 8, max = 0x7fffffffffffffff, sign = true };
            lp64_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 8, align = 8, max = 0xffffffffffffffff, sign = true };

            switch (target_os) {
                case OS_WIN32:
                    switch (target_arch) {
                        case ARCH_X86:
                            type_metrics = win32_x86_metrics;
                            break;
                        case ARCH_X64:
                            type_metrics = win32_x64_metrics;
                            break;
                        default:
                            break;
                    }
                    break;
                case OS_LINUX:
                    switch (target_arch) {
                        case ARCH_X86:
                            type_metrics = ilp32_metrics;
                            break;
                        case ARCH_X64:
                            type_metrics = lp64_metrics;
                            break;
                        default:
                            break;
                    }
                    break;
                case OS_OSX:
                    switch (target_arch) {
                        case ARCH_X64:
                            type_metrics = lp64_metrics;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            if(type_metrics == null) {
                error_here("Unsupported os/arch combination: {0}/{1}\n", os_names[(int)target_os], arch_names[(int)target_arch]);
            }
            if (type_metrics[(int)TYPE_PTR].size == 4) {
                type_uintptr = type_uint;
                type_usize = type_uint;
                type_ssize = type_int;
            }
            else {
                assert(type_metrics[(int)TYPE_PTR].size == 8);
                type_uintptr = type_ullong;
                type_usize = type_ullong;
                type_ssize = type_llong;
            }
            return;
        }

        bool is_excluded_target_filename(string name) {
            int len = name.Length - 4;
            string str1, str2;

            var start = name.LastIndexOfAny(new[] {'\\', '/'}) + 1;

            if (start == 0)
                return true;

            var span = name.Substring(start, len - start);
            var split = span.Split('_');

            if (split.Length == 3) {
                str1 = split[1];
                str2 = split[2];
            }
            else if (split.Length == 2) {
                str1 = split[1];
                str2 = "";
            }
            else
                return false;

            OS os1 = get_os(str1);
            Arch arch1 = get_arch(str1);
            OS os2 = get_os(str2);
            Arch arch2 = get_arch(str2);
            if (arch1 != 0 && os2 != 0) {
                return arch1 != target_arch || os2 != target_os;
            }
            else if (arch2 != 0 && os1 != 0) {
                return arch2 != target_arch || os1 != target_os;
            }
            else if (os1 != 0) {
                return os1 != target_os;
            }
            else if (arch1 != 0) {
                return arch1 != target_arch;
            }
            else {
                return false;
            }
        }

    }
}