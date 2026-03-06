using s3molib;
using s3rc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S3PR
{
    public class S3PR
    {

        public static bool RemoveThumbnail { private get; set; }
        public static bool RemoveIcon { private get; set; }

        public static int SkippedFilesCounter { get; set; } = 0;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Drag and drop a folder containing the files to merge on top of S3PR.exe.");
            }
            else
            {
                ReducePackages(args);
            }
            Console.WriteLine("Press any key to close the program....");
            Console.ReadKey();
        }

        public static void ReducePackages(string[] args)
        {
            try
            {
                List<Package> mergeablePackages = new List<Package>(0);
                foreach (string path in args)
                {
                    if (File.Exists(path))
                    {
                        if (path.EndsWith(".package", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                mergeablePackages.Add(Package.Load(path));
                            }
                            catch (Exception)
                            {
                                SkippedFilesCounter++;
                            }
                        }
                    }
                    else if (Directory.Exists(path))
                    {
                        string[] newArgs = (from file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                            where file.EndsWith(".package", StringComparison.OrdinalIgnoreCase)
                                            select file).ToArray<string>();
                        ReducePackages(newArgs);
                    }
                    else
                    {
                        throw new Exception("File/directory does not exist");
                    }
                }
                if (mergeablePackages.Count > 0)
                {
                    Console.WriteLine($"Reducing {mergeablePackages.Count} packages");
                    Package.RemoveThumAndIconResourcesFromPackage(mergeablePackages, S3PR.RemoveThumbnail, S3PR.RemoveIcon);
                    Console.WriteLine($"Done reducing");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}