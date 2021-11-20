using System;
using System.Windows.Forms;
using PixelStacker.UI;
using PixelStacker.Resources;
using PixelStacker.UI.Forms;
using PixelStacker.Logic.IO.Config;
using PixelStacker.IO;
using PixelStacker.Logic.Model;

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

    }
}
