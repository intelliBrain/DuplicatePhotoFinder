using System;
using System.IO;

namespace DupeFinder
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var path = @"D:\Users\Ally\Pictures\Photos\";
            var duplicateFolder = Path.Combine(path, "Duplicates");
            if (!Directory.Exists(duplicateFolder))
            {
                Directory.CreateDirectory(duplicateFolder);
            }

            DuplicateFinder.CheckFolderForDupes(path, duplicateFolder);
            Console.ReadLine();
        }
    }
}