using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ion.Console
{
    static class VsTools
    {

        [Conditional("DEBUG")]
        public static void Break((int line, string file, string input) dVal)
        {
            var ide = (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.15.0");
            ide.Debugger.Breakpoints.Add(Line: dVal.line, File: dVal.file);
           // var p = Parser.GetInstance();
           // p.Parse(dVal.input);
        }

        [Conditional("DEBUG")]
        public static void ClearOutputWindow()
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            //Application.DoEvents();  // This is for Windows.Forms.
            // This delay to get it to work. Unsure why. See http://stackoverflow.com/questions/2391473/can-the-visual-studio-debug-output-window-be-programatically-cleared
            Thread.Sleep(100);
            // In VS2008 use EnvDTE80.DTE2
            var ide = (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.15.0");
            if (ide != null)
            {
                ide.ExecuteCommand("Edit.ClearOutputWindow");
                Marshal.ReleaseComObject(ide);
            }
        }
    }
}