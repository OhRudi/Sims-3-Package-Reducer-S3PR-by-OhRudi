using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace OhRudi
{
    class S3RC
    {
        private string exePath;

        private static S3RC Instance;

        public static S3RC GetInstance { get { Instance ??= new S3RC(); return Instance; } private set; }


        /**
         * run Sims 3 Recompressor Executeable
         */
        private S3RC()
        {
            exePath = ExtractTool();
            if (!File.Exists(exePath))
            {
                throw new Exception("Crucial Files of this Software are missing. Did you delete something? This can't be fixed manually. Please reinstall this program.");
            }
        }

        /**
         * extract the Sims 3 Recompressor Tool (s3rc.exe) into the temp folder from the packaged exe file of this tool
         */
        private string ExtractTool()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"s3rc_{Guid.NewGuid()}.exe");

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "S3PR_GUI.s3rc.exe";

            using (Stream resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null) throw new Exception("Embedded tool not found.");

                using (FileStream file = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }

            return tempPath;
        }

        /**
         * Delete extracted S3RC tool
         */
        public void DeleteTool()
        {
            try { File.Delete(exePath); }
            catch { /* Do nothing */ }
            finally { exePath = ""; }
        }

        public void Compress(string inputPathFile)
        {
            if (!File.Exists(exePath)) exePath = ExtractTool();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                Arguments = $"\"{inputPathFile}\"",
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            using (Process process = Process.Start(psi))
            {
                process?.WaitForExit();
            }
        }

        public void Decompress(string inputPathFile)
        {
            if (!File.Exists(exePath)) exePath = ExtractTool();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                Arguments = $"-d \"{inputPathFile}\"",
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            using (Process process = Process.Start(psi))
            {
                process?.WaitForExit();
            }
        }
    }
}
