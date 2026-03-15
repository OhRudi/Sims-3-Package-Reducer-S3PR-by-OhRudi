using OhRudi;
using s3molib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace OhRudi
{
    public class S3PR
    {

        public bool RemoveThumbnail { get; set; } = false;
        public bool RemoveIcon { get; set; } = false;
        public bool CompressFile { get; set; } = false;
        public bool DecompressFile { get; set; } = false;
        public bool SearchRecursive { get; set; } = false;
        public bool ConsoleSilent { get; set; } = false;
        public List<string> SkippedFiles { get; private set; } = [];

        public List<string> SkippedFolders { get; private set; } = [];

        private S3RC S3RC = S3RC.GetInstance;
        private static S3PR Instance { get; set; }

        public static S3PR GetInstance {
            get { Instance ??= new S3PR(); return Instance; }
            private set;
        }


        /**
         * Main Code for the console application
         */
        public static void Main(string[] args)
        {
            GetInstance.StartConsoleApplication(args);
        }


        /**
         * out string to console if silent setting is not set, but can be overriden by parameter
         */
        private void ConsoleWrite(string input, bool overrideSilentFlag = false)
        {
            if (!ConsoleSilent || overrideSilentFlag) Console.WriteLine(input);
        }


        /**
         * out active settings console
         */
        private void OutSettings()
        {
            if (SearchRecursive) ConsoleWrite("Setting: Search in subfolders for Package-Files");
            if (RemoveThumbnail) ConsoleWrite("Setting: Remove thumbnail resources");
            if (RemoveIcon) ConsoleWrite("Setting: Remove icon resources");
            if (CompressFile) ConsoleWrite("Setting: Compress Package-Files");
            if (DecompressFile) ConsoleWrite("Setting: Decompress Package-Files");
        }


        /**
         * starts console application
         */
        public void StartConsoleApplication(string[] args)
        {
            Environment.ExitCode = 0;
            if (args.Length == 0) args = ["--help"];

            List<string> paths = new List<string>();
            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "-s":
                        case "--silent":
                            ConsoleSilent = true;
                            break;
                        case "-r":
                        case "--search-recursive":
                            SearchRecursive = true;
                            break;
                        case "-t":
                        case "--remove-thumbnail":
                            RemoveThumbnail = true;
                            break;
                        case "-i":
                        case "--remove-icon":
                            RemoveIcon = true;
                            break;
                        case "-c":
                        case "--compress-file":
                            if (DecompressFile) break;
                            CompressFile = true;
                            break;
                        case "-d":
                        case "--decompress-file":
                            if (CompressFile) break;
                            DecompressFile = true;
                            break;
                        case "-h":
                        case "--help":
                            ConsoleWrite($"Usage:\r\nS3PR.exe [options] <paths to folders or files...>\r\n\r\nOptions:\r\n-r, --search-recursive     Search directories recursively when looking for Package-Files.\r\n\r\n-t, --remove-thumbnail     Remove thumbnail resources from Package-Files.\r\n\r\n-i, --remove-icon          Remove icon resources from Package-Files.\r\n\r\n-c, --compress-file        Compress Package-Files.\r\n\r\n-d, --decompress-file      Decompress Package-Files.\r\n\r\n-h, --help                 Show this help message and exit.\r\n\r\nArguments:\r\n<paths...>                 One or more files or folders to process.\r\n", true);
                            return;
                        default:
                            ConsoleWrite($"Unsupported Option \"{arg}\"", true);
                            return;
                    }
                }
                else
                {
                    paths.Add(arg);
                }
            }
            try
            {
                if (!RemoveIcon && !RemoveThumbnail && !CompressFile && !DecompressFile)
                    throw new Exception($"Please enter at least one of the options (like \"--remove-icon\", \"--remove-thumbnail\", etc.) to edit the Package-Files.");

                double fileSizeBeforeInByte = 0;
                double fileSizeAfterInByte = 0;

                IEnumerable<string> pathEnumerable = FindPackageFiles(paths.ToArray(), SearchRecursive);
                if (pathEnumerable.Count() <= 0) throw new Exception($"The selected folder{(pathEnumerable.Count() > 1 ? "s do" : " does")} not contain any Package-Files. Please select another folder.");

                OutSettings();
                ConsoleWrite($"Editing {pathEnumerable.Count()} packages");

                using (var spinner = new Spinner())
                {
                    spinner.Start();
                    foreach (string pathFile in pathEnumerable)
                    {
                        fileSizeBeforeInByte += (double)(new FileInfo(pathFile)).Length;
                        EditPackage(pathFile, RemoveThumbnail, RemoveIcon);
                        if (CompressFile) S3RC.Compress(pathFile);
                        if (DecompressFile) S3RC.Decompress(pathFile);
                        fileSizeAfterInByte += (double)(new FileInfo(pathFile)).Length;
                    }
                    spinner.Stop();
                }

                ConsoleWrite(GetSuccessSummaryMessage(pathEnumerable.Count(), fileSizeBeforeInByte, fileSizeAfterInByte));
                ConsoleWrite(GetSkippedFilesAndFoldersMessage());
            }
            catch (Exception exception)
            {
                ConsoleWrite(exception.Message, true);
                Environment.ExitCode = 1;
            }
            ConsoleWrite("Press any key to close the program ...");
            if (!ConsoleSilent) Console.ReadKey();
        }


        /**
         * private constructor for singleton pattern
         */
        private S3PR() { }


        /**
         * find list of package files
         */
        public IEnumerable<string> FindPackageFiles(string[] paths, bool searchRecursive = false)
        {
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = searchRecursive,
                IgnoreInaccessible = true,
                ReturnSpecialDirectories = false
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    if (path.EndsWith(".package", StringComparison.OrdinalIgnoreCase))
                        yield return path;

                    continue;
                }

                if (Directory.Exists(path))
                {
                    IEnumerable<string> files = Array.Empty<string>();

                    try { files = Directory.EnumerateFiles(path, "*.package", options); }
                    catch (Exception) { SkippedFolders.Add(path); continue; }

                    foreach (var file in files)
                        yield return file;
                }
            }
        }


        /**
         * edit Package-File, here's the main part of the code
         */
        public void EditPackage(string path, bool removeThumbnails = false, bool removeIcons = false)
        {
            Package importPackage = null;
            try { importPackage = Package.Load(path); }
            catch { SkippedFiles.Add(path); return; }
            String tempPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".temp" + Path.GetExtension(path));
            FileStream stream = null;
            BinaryWriter writer = null;
            try
            {
                Package package = Package.New(tempPath);
                stream = new FileStream(tempPath, FileMode.Open, FileAccess.Write, FileShare.Read);
                writer = new BinaryWriter(stream);
                stream.Position = (long)((ulong)package.IndexPosition);
                foreach (ResourceEntry re in importPackage.ResourceEntries)
                {
                    if (!(Helper.THUMResources.Contains(re.Type) && removeThumbnails) && !(Helper.ICONResources.Contains(re.Type) && removeIcons))
                    {
                        byte[] data = importPackage.GetRawData(re);
                        if (data == null)
                        {
                            throw new Exception("Unable to obtain data from " + Path.GetFileName(importPackage.FilePath));
                        }
                        package._resourceEntries.Add(new ResourceEntry(importPackage, re.Type, re.Group, re.ID1, re.ID2, (uint)stream.Position, re.FileSize, re.MemSize, re.Compressed, re.Unknown2));
                        writer.Write(data, 0, (int)re.FileSize);
                    }
                }
                package.IndexPosition = (uint)stream.Position;
                package.WriteIndex(writer);
                package.WriteHeader(stream, writer);
                writer.Close();
                stream.Close();

                if (File.Exists(path))
                {
                    try
                    {
                        FileAttributes attrs = File.GetAttributes(path);
                        if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);
                        }
                        File.Delete(path);
                        File.Move(tempPath, path);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        CleanUpTempFile(tempPath, writer, stream);
                        // Skip files without permission
                        return;
                    }
                    catch (IOException)
                    {
                        CleanUpTempFile(tempPath, writer, stream);
                        // Skip files that cannot be accessed or replaced
                        return;
                    }
                }
            }
            catch (Exception excpetion)
            {
                CleanUpTempFile(tempPath, writer, stream);
                throw new Exception(excpetion.Message);
            }
        }

        
        /**
         * close streams, remove temp file if still existing
         */
        private void CleanUpTempFile(string pathTempFile, BinaryWriter writer, FileStream stream)
        {
            writer.Close();
            stream.Close();
            // Remove temp files, if they still are there, just in case
            if (File.Exists(pathTempFile))
            {
                try { File.Delete(pathTempFile); }
                catch { }
            }
        }


        /**
         * reset skipped files counter
         */
        public void ResetSkippedFiles()
        {
            SkippedFiles = [];
        }


        /**
         * reset skipped folders counter
         */
        public void ResetSkippedFolders()
        {
            SkippedFolders = [];
        }


        public void RemoveDuplicatesInSkippedFilesAndFolders()
        {
            SkippedFiles = SkippedFiles.Distinct().ToList();
            SkippedFolders = SkippedFolders.Distinct().ToList();
        }


        /**
         * converts byte unit into a something more human friendly
         */
        private string ConvertByteToOtherUnit(double fileSize)
        {
            string[] units = { "Byte", "KB (Kilobyte)", "MB (Megabyte)", "GB (Gigabyte)", "TB (Terrabyte)", "PB (Petabyte)", "EB (Exabyte)" };
            short f = 0;
            for (; fileSize > 1000; f++) fileSize /= 1000;
            return $"{string.Format("{0:0.0}", fileSize)} {units[f]}";
        }


        /**
         * get summary message of how many files edited and the difference in file size
         */
        public string GetSuccessSummaryMessage(int progressTillStopped, double fileSizeBeforeInByte, double fileSizeAfterInByte)
        {
            string message = $"Done.";
            int progressMinusSkipped = progressTillStopped - SkippedFiles.Count;

            if (progressMinusSkipped > 0)
            {
                if ((fileSizeBeforeInByte - fileSizeAfterInByte) > 0)
                {
                    message += $"\n\n{(DecompressFile ? "Decompressed and r" : "R")}educed {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}";
                }
                else if (DecompressFile && (fileSizeBeforeInByte - fileSizeAfterInByte) != 0)
                {
                    message += $"\n\n{(RemoveThumbnail || RemoveIcon ? "Edited and d" : "D")}ecompressed {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")}. {(progressMinusSkipped != 1 ? "They take" : "It takes")} up {ConvertByteToOtherUnit(fileSizeAfterInByte - fileSizeBeforeInByte)} more space now.";
                }
                else
                {
                    message += $"\n\nIt checked {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} but, {(progressMinusSkipped != 1 ? "they were" : "it was")} already edited{(DecompressFile ? " and decompressed" : "")}, so nothing changed.";
                }
            }
            return message;
        }


        /**
         * Get Stop Summary Message
         */
        public string GetStopSummaryMessage(int progressTillStopped, double fileSizeBeforeInByte, double fileSizeAfterInByte, string lastPath = "")
        {
            string message = $"Process Stopped.";
            int progressMinusSkipped = progressTillStopped - SkippedFiles.Count;
            if (progressMinusSkipped < 0) progressMinusSkipped = 0;
            if (progressMinusSkipped > 0)
            {
                if (fileSizeBeforeInByte - fileSizeAfterInByte > 0)
                {
                    message += $"\n\nBefore it stopped, it {(DecompressFile ? "decompressed and " : "")}reduced {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}";
                }
                else if (DecompressFile && (fileSizeBeforeInByte - fileSizeAfterInByte) != 0)
                {
                    message += $"\n\nBefore it stopped, it {(RemoveThumbnail || RemoveIcon ? "edited and " : "")}decompressed {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")}. {(progressMinusSkipped != 1 ? "They take" : "It takes")} up {ConvertByteToOtherUnit(fileSizeAfterInByte - fileSizeBeforeInByte)} more space now.";
                }
                else
                {
                    message += $"\n\nBefore it stopped, it checked {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} but, {(progressMinusSkipped != 1 ? "they were" : "it was")} already edited{(DecompressFile ? " and decompressed" : "")}, so nothing changed.";
                }
            }
            return message + $"\n\nLast processed File: {lastPath}";
        }


        /**
         * get message for skipped files and folders
         */
        public string GetSkippedFilesAndFoldersMessage()
        {
            RemoveDuplicatesInSkippedFilesAndFolders();
            string message = "";
            if (SkippedFiles.Count > 0 || SkippedFolders.Count > 0)
            {
                message += $"\n\nSkipped ";
                if (SkippedFiles.Count > 0) message += $"{SkippedFiles.Count} File{(SkippedFiles.Count > 1 ? "s" : "")} ";
                if (SkippedFiles.Count > 0 && SkippedFolders.Count > 0) message += "and ";
                if (SkippedFolders.Count > 0) message += $"{SkippedFolders.Count} Folder{(SkippedFolders.Count > 1 ? "s" : "")} ";
                message += "cause the program had no access.\n";
                if (SkippedFiles.Count > 0)
                {
                    message += "\n## SKIPPED FILES ##\n";
                    foreach (string skippedFile in SkippedFiles)
                        message += $"{Path.GetFileName(skippedFile)}\n";
                }
                if (SkippedFolders.Count > 0)
                {
                    message += "\n## SKIPPED FOLDERS ##\n";
                    foreach (string skippedFolder in SkippedFolders)
                        message += $"{Path.GetFileName(skippedFolder)}\n";
                }
            }
            return message;
        }
    }
}