using System;

namespace DupeFinder
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var path = @"D:\Users\Ally\Pictures\Photos\2017-05";
            DupelicateFinder.CheckFolderForDupes(path);
            Console.ReadLine();
        }
    }
}