using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOS
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.SetCompatibleTextRenderingDefault(false);

            Cef.EnableHighDPISupport();

            //const bool multiThreadedMessageLoop = true;

            //var browser = new BrowserInterface(multiThreadedMessageLoop);

            //IBrowserProcessHandler browserProcessHandler;

            //if (multiThreadedMessageLoop)
            //{
            //    browserProcessHandler = new BrowserProcessHandler();
            //}
            //else
            //{
            //    //Get the current taskScheduler (must be called after the form is created)
            //    var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //    browserProcessHandler = new WinFormsBrowserProcessHandler(scheduler);
            //}

            //var settings = new CefSettings
            //{
            //    MultiThreadedMessageLoop = multiThreadedMessageLoop,
            //    ExternalMessagePump = !multiThreadedMessageLoop
            //};


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(browser);
            Application.Run(new Login());
        }
    }
}
