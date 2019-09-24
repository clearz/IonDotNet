using System;
using static System.Console;
using Lang;
namespace Ion.Console
{
    class Program
    {
        static void Main(string[] args) {
            int iterations = 100000;
            var uIon = new Lang.Ion();
            var mion = new MLang.Ion();
            //uIon.Run(iterations);
            mion.Run(iterations);

            System.Console.ReadKey();
            var l = new Lang.Ion();
            l.lex_init();
            const string PROMPT = "=> ";
            var lastErrorInfo = (Line: 0, Path: "", Expr: "");
            while (true)
            {
                try
                {
                    Write(PROMPT);
                    var input = ReadLine();
                    if (HandleCmd(input)) continue;
                    VsTools.ClearOutputWindow();
                    
                    ForegroundColor = ConsoleColor.DarkYellow;
                   
                }
                catch (Exception e)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(e);
                }
                finally
                {
                    ResetColor();
                }

                bool HandleCmd(string str)
                {
                    switch (str)
                    {
                        case "exit":
                        case "quit":
                            Environment.Exit(0);
                            break;
                        case "cls":
                            Clear();
                            break;
#if DEBUG
                        case "clear":
                            VsTools.ClearOutputWindow();
                            break;
                        case "break":
                            if (lastErrorInfo.Expr != "")
                            {
                                VsTools.Break(lastErrorInfo);
                                lastErrorInfo.Expr = "";
                            }

                            break;
#endif
                        default:
                            return string.IsNullOrWhiteSpace(str);
                    }

                    return true;
                }
            }
        }
    }
}
