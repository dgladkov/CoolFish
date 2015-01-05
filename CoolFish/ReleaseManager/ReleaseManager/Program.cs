using System;
using System.Diagnostics;
using System.IO.Compression;

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
                using (var archive = ZipFile.Open(prefix + version + ".zip", ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(mainFileName, mainFileName);
                    for (int i = 1; i < args.Length; i++)
                    {
                        archive.CreateEntryFromFile(args[i], args[i]);
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
