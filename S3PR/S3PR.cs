using s3molib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OhRudi;

namespace OhRudi
{
    public class S3PR
    {

        private bool RemoveThumbnail { get; set; } = false;
        private bool RemoveIcon { get; set; } = false;
        private bool CompressFile { get; set; } = false;
        private bool DecompressFile { get; set; } = false;
        private bool SearchRecursive { get; set; } = false;
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
            if (args.Length == 0)
            {
                Console.WriteLine("Drag and drop a folder containing the files to merge on top of S3PR.exe.");
            }
            else
            {
                try
                {
                    IEnumerable<string> pathEnumerable = S3PR.GetInstance.FindPackageFiles(args, S3PR.GetInstance.SearchRecursive);
                    if (pathEnumerable.Count() <= 0) throw new Exception("The selected folder{(listPaths.Length > 1 ? \"s do\" : \" does\")} not contain any Package-Files. Please select another folder.");
                    Console.WriteLine($"Editing {pathEnumerable.Count()} packages");
                    foreach (string path in pathEnumerable)
                    {
                        if (GetInstance.RemoveIcon || GetInstance.RemoveThumbnail) GetInstance.EditPackage(path, GetInstance.RemoveThumbnail, GetInstance.RemoveIcon);
                        if (GetInstance.CompressFile) GetInstance.S3RC.Compress(path);
                        if (GetInstance.DecompressFile) GetInstance.S3RC.Decompress(path);
                    }
                    Console.WriteLine(GetInstance.GetSkippedFilesAndFoldersMessage());
                    Console.WriteLine($"Done.");
                }
                catch (Exception exception) {
                    Console.WriteLine(exception.Message);
                }
            }
            Console.WriteLine("Press any key to close the program....");
            Console.ReadKey();
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