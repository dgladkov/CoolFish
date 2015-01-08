using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;

namespace ReleaseManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    return;
                }


#if DEBUG
                const string prefix = "Debug_";
#else
                const string prefix = "Release_";
#endif

                var mainFileName = args[0];

                var version = FileVersionInfo.GetVersionInfo(mainFileName).FileVersion;
                using (var archive = new ZipFile(prefix + version + ".zip"))
                {
                    archive.AddFile(mainFileName);
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (File.Exists(args[i]))
                        {
                            archive.AddFile(args[i]);
                        }
                        else if (Directory.Exists(args[i]))
                        {
                            archive.AddDirectory(args[i], args[i]);
                        }
                        else
                        {
                            Console.WriteLine("Could not determine type for arg: " + args[i]);
                        }
                        archive.Save();

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }
        }
    }
}
