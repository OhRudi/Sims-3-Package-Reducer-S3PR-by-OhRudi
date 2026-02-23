using S3PR;
namespace S3PR_GUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // the main code is not in this method
            // look at Form1.cs
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}