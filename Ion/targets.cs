namespace IonLang
{
    using static OS;
    using static Arch;
    using static TypeKind;


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

        char*[] os_names = new char*[(int)NUM_OSES];
        char*[] arch_names = new char*[(int)NUM_ARCHES];

        TypeMetrics* type_metrics;

        TypeMetrics* win32_x86_metrics;
        TypeMetrics* win32_x64_metrics;
        TypeMetrics* ilp32_metrics;
        TypeMetrics* lp64_metrics;

        OS get_os(char* name) {
            for (int i = 1; i < (int)NUM_OSES; i++) {
                if (strcmp(os_names[i], name) == 0) {
                    return (OS)i;
                }
            }
            return 0;
        }

        Arch get_arch(char* name) {
            for (int i = 1; i < (int)NUM_ARCHES; i++) {
                if (strcmp(arch_names[i], name) == 0) {
                    return (Arch)i;
                }
            }
            return 0;
        }
        void init_target_names() {

            os_names[(int)OS_WIN32] = "win32".ToPtr();
            os_names[(int)OS_LINUX] = "linux".ToPtr();
            os_names[(int)OS_OSX] = "osx".ToPtr();
            arch_names[(int)ARCH_X64] = "x64".ToPtr();
            arch_names[(int)ARCH_X86] = "x86".ToPtr();
        }
        void init_target() {
            type_metrics = null;
            win32_x86_metrics = xmalloc<TypeMetrics>((int)NUM_TYPE_KINDS);
            win32_x64_metrics = xmalloc<TypeMetrics>((int)NUM_TYPE_KINDS);
            ilp32_metrics = xmalloc<TypeMetrics>((int)NUM_TYPE_KINDS);
            lp64_metrics = xmalloc<TypeMetrics>((int)NUM_TYPE_KINDS);

            var default_metrics = stackalloc TypeMetrics[(int)NUM_TYPE_KINDS];
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

            memcpy(win32_x86_metrics, default_metrics, sizeof(TypeMetrics) * (int)NUM_TYPE_KINDS);
            win32_x86_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 4, align = 4 };
            win32_x86_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            win32_x86_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            memcpy(win32_x64_metrics, default_metrics, sizeof(TypeMetrics) * (int)NUM_TYPE_KINDS);
            win32_x64_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 8, align = 8 };
            win32_x64_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            win32_x64_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            memcpy(ilp32_metrics, default_metrics, sizeof(TypeMetrics) * (int)NUM_TYPE_KINDS);
            ilp32_metrics[(int)TYPE_PTR] = new TypeMetrics { size = 4, align = 4 };
            ilp32_metrics[(int)TYPE_LONG] = new TypeMetrics { size = 4, align = 4, max = 0x7fffffff, sign = true };
            ilp32_metrics[(int)TYPE_ULONG] = new TypeMetrics { size = 4, align = 4, max = 0xffffffff, sign = true };

            memcpy(lp64_metrics, default_metrics, sizeof(TypeMetrics) * (int)NUM_TYPE_KINDS);
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
                error_here("Unsupported os/arch combination: {0}/{1}\n", _S(os_names[(int)target_os]), _S(arch_names[(int)target_arch]));
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

        bool is_excluded_target_filename(char* name) {
            int len = strlen(name) - 4;
            char* str1 = stackalloc char[64];
            char* str2 = stackalloc char[64];

            char *end = name + len;
            char* start = end;

            while(--start != name)
                if (start[-1] == '/' || start[-1] == '\\') 
                    break;

            if (start == name) return true;

            char *ptr1 = end;
            while (ptr1 != start && ptr1[-1] != '_') {
                ptr1--;
            }

            if (ptr1 != start) { 
                memcpy(str1, ptr1, (int)((end - ptr1) * 2));
                ptr1--;
            }

            char *ptr2 = ptr1;
            while (ptr2 != start && ptr2[-1] != '_') {
                ptr2--;
            }
            if (ptr2 != start) { 
                memcpy(str2, ptr2, (int)((ptr1 - ptr2) * 2));
            }

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