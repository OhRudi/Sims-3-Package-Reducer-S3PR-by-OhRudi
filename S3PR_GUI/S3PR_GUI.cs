using S3PR_GUI;
using System.Runtime.InteropServices;

namespace OhRudi
{
    public static class Program
    {

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        
        [STAThread]
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
            else { 
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
                return;
            }
        }
    }
}