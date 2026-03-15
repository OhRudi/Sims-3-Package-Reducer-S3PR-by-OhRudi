using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace S3PR_GUI
{
    internal class ConsoleHelper
    {
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        public static void AttachToParentConsole()
        {
            if (AttachConsole(ATTACH_PARENT_PROCESS))
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
            }
        }
    }
}
