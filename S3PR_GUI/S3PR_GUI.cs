using S3PR_GUI;
using System.Runtime.InteropServices;

namespace OhRudi
{
    public static class Program
    {

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        public static void Main(string[] args)
        {
            // if main got console arguments
            // attach to the calling console
            // and start console application
            if (args.Length > 0)
            {
                ConsoleHelper.AttachToParentConsole();
                S3PR.GetInstance.StartConsoleApplication(args);
                return;
            }

            // else: start GUI
            // the main code is not in this method
            // look at Form1.cs
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

        }
    }
}