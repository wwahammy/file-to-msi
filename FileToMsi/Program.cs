using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace FileToMsi
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Program().RunMain(args);
        }

        public void RunMain(string[] args)
        {
            if (args.Count() != 1)
            {
                WriteUsage();
                return;
            }

            var file = args[0];

            if (!File.Exists(file))
            {
                Console.WriteLine("{0} does not exist.", file);
                WriteUsage();
                return;
            }

            var hash = new int[4];
            Installer.GetFileHash(file, hash);


            var allMsi = Directory.GetFiles(@"C:\Windows\Installer", "*.msi", SearchOption.AllDirectories);
            Console.WriteLine("There are {0} installed Msi's", allMsi.Count());

            var found = allMsi.Aggregate(false, (current, msi) => current | SearchMsiFileHashsForOurHash(msi, hash));

            if (!found)
                Console.WriteLine("The file wasn't found in any MSI on the system.");
        }

        public bool SearchMsiFileHashsForOurHash(string msi, int [] ourHash)
        {
            using (var session = new Database(msi, DatabaseOpenMode.ReadOnly))
            {
               // var tables = session.Database.Tables;
                if (session.Tables.Contains("MsiFileHash"))
                {
                    var results =
                        session.ExecuteQuery(
                            "SELECT `HashPart1`, `HashPart2`, `HashPart3`, `HashPart4` FROM `MsiFileHash`")
                            .Cast<int>()
                            .ToArray();
                    for (int i = 0, segmentBegin = 0; segmentBegin < results.Count(); i++, segmentBegin = i*4)
                    {
                        var hashParts = results.Skip(segmentBegin).Take(4).ToArray();
                        if (hashParts.SequenceEqual(ourHash))
                        {
                            Console.WriteLine("{0} contains the file.", msi);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private void WriteUsage()
        {
            Console.WriteLine(
                    @"
Find which MSI a file came from
Usage: FileToMsi.exe path_to_file");
        }
    }

  

    
}
