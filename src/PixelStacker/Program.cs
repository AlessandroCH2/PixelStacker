using System;
using System.Windows.Forms;
using PixelStacker.UI;
using PixelStacker.Resources;
using PixelStacker.IO;
using PixelStacker.Logic.Model;
using System.IO;

namespace PixelStacker
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.ThreadException += ErrorReporter.OnThreadException;
                AppDomain.CurrentDomain.UnhandledException += ErrorReporter.OnUnhandledException;
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                ResxHelper.InjectIntoTextResx();
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var form = new MainForm();
                ErrorReporter.MF = form;
                //var form = new TestForm();
                Application.Run(form);
            }
            catch(Exception ex)
            {
                byte[] errData = ErrorReporter.SendExceptionInfoToZipBytes(System.Threading.CancellationToken.None, ex, new ErrorReportInfo() { Exception = ex }, false, "Error from main try catch").Result;
                File.WriteAllBytes("pixelstacker-error.zip", errData);
            }
        }

    }
}
