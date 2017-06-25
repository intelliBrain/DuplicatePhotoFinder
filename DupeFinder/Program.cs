using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExifLib;
using XperiCode.JpegMetadata;

namespace DupeFinder
{
    class Program
    {
        private static void Main(string[] args)
        {
            var regex = new Regex(@"\\(?<name>[^\\]*?)(?<dupe>\s\(\d\))?\.");

            var enumerable = Directory.GetFiles(@"D:\Users\Ally\Pictures\Photos\2017-05")
                .Select(f =>
                    {
                        var match = regex.Match(f);
                        if (match.Success)
                        {
                            var matchGroup = match.Groups["name"].Value;
                            return new FileMatch(f, matchGroup);
                        }
                        return new FileMatch(f, f);
                    }
                ).ToLookup(fm => fm.MatchName + fm.Info.Extension)
                .Where(grp => grp.Count() > 1)
                .ToList();

            foreach (var grp in enumerable)
            {
                List<FileMatch> same = new List<FileMatch>();
                List<FileMatch> different = new List<FileMatch>();
                var grpKey = grp.Key;
                for (int i = 0; i < grp.Count(); i++)
                {
                    var left = grp.ElementAt(i);
                    var leftImg = Image.FromFile(left.FullName);

                    for (int j = i + 1; j < grp.Count(); j++)
                    {
                        var right = grp.ElementAt(j);
                        if (left.Info.Length == right.Info.Length &&
                            left.Info.CreationTimeUtc == right.Info.CreationTimeUtc)
                        {
                            switch (left.Info.Extension.ToLower())
                            {
                                case ".jpg":
                                case ".jpeg":
                                case ".gif":
                                case ".png":
                                    
                                    var rightImg = Image.FromFile(right.FullName);

                                    if (leftImg.PhysicalDimension == rightImg.PhysicalDimension)
                                    {
                                        Console.WriteLine($"{left.FullName} is a dupe of {right.FullName}");
                                    }
                                    else
                                    {
                                        //Console.WriteLine($"{left.FullName} is NOT a dupe of {right.FullName}");
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"{left.FullName} is a dupe of {right.FullName}");
                                    break;
                            }
                        }else
                        {
                            Console.WriteLine($"{left.FullName} is NOT a dupe of {right.FullName}");
                        }
                    }
                }
            }
            Console.ReadLine();
        }

        public static bool Same<T>(ExifTags tags, ExifReader left, ExifReader right)
        {
            T leftVal;
            var leftExists = left.GetTagValue<T>(tags, out leftVal);
            T rightVal;
            var rightExists = left.GetTagValue<T>(tags, out rightVal);
            if (!leftExists && !rightExists) return true;

            if (leftExists != rightExists) return false;

            return leftVal.Equals(rightVal);

        }

        private class FileMatch
        {
            private Image _image;

            public FileMatch(string fullName, string matchName)
            {
                FullName = fullName;
                MatchName = matchName;
                Info = new FileInfo(FullName);

                switch (Info.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        ImageSize = Image.FromFile(FullName).PhysicalDimension;
                        break;
                }
            }

            public string FullName { get; }
            public FileInfo Info { get; }

            public SizeF ImageSize { get; }

            public string MatchName { get; }
            public bool IsSuffixed => FullName != MatchName;
            
        }
    }
}
