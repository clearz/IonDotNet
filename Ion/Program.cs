namespace IonLang
{
    unsafe partial class Ion {
        static int Main(string[] args) {
            var ion = new Ion();
            if (args.Length > 0)
                ion.ion_main(args);
            else {
                Timer.Time(() => ion.ion_test("test"));
            }
            return 0;
        }
    }
}
