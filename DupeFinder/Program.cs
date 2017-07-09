using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace DupeFinder
{
    public class Program
    {
        private static readonly Regex Regex = new Regex(@"\\(?<name>[^\\]*?)(?<dupe>\s\((?<number>\d)\))?\.");

        private static void Main(string[] args)
        {
            var path = @"D:\Users\Ally\Pictures\Photos\2017-05";
            CheckFolderForDupes(path);
            Console.ReadLine();
        }

        private static void CheckFolderForDupes(string path)
        {
            Console.WriteLine("Checking "+path);
            CheckForFileDupes(path);

            foreach (var subDir in Directory.GetDirectories(path))
            {
                CheckFolderForDupes(subDir);
            }
        }

        private static void CheckForFileDupes(string path)
        {
            var enumerable = Directory.GetFiles(path)
                .Select(f =>
                    {
                        var match = Regex.Match(f);
                        if (match.Success)
                        {
                            var matchGroup = match.Groups["name"].Value;
                            var copyNumberStr = match.Groups["number"]?.Value;
                            var copyNumber = string.IsNullOrEmpty(copyNumberStr)
                                ? 0
                                : Convert.ToInt32(copyNumberStr);
                            return CreateFileMatch(f, matchGroup, copyNumber);
                        }
                        return CreateFileMatch(f);
                    }
                ).ToLookup(fm => fm.MatchName + fm.Extension)
                .Where(grp => grp.Count() > 1)
                .ToList();

            foreach (var grp in enumerable)
            {
                var dupes = FindDupes(grp);

            }
        }

        private static FileMatch CreateFileMatch(string f, string matchGroup = null, int? copyNumber =null)
        {
            var info = ShellFile.FromFilePath(f);
            var systemProps = info.Properties.System;
            var extension = systemProps.FileExtension.Value.ToLower();

            var imageProps = systemProps.Image;
            var vidProps = systemProps.Video;

            var testProps = new Dictionary<string, string>
            {
                ["DateCreated"] = systemProps.DateCreated?.Value?.ToString("O"),
                ["Size"] = systemProps.Size?.Value?.ToString(),
                ["ImageHorizontalSize"] = imageProps?.HorizontalSize?.Value?.ToString(),
                ["ImageVerticalSize"] = imageProps?.VerticalSize?.Value?.ToString(),
                ["VideoCompression"] = vidProps?.Compression?.Value,
                ["VideoFrameHeight"] = vidProps?.FrameHeight?.Value?.ToString(),
                ["VideoFrameWidth"] = vidProps?.FrameWidth?.Value?.ToString(),
                ["VideoFrameRate"] = vidProps?.FrameRate?.Value?.ToString(),
            };

            return new FileMatch(f, matchGroup ?? f, copyNumber ?? 0, extension, testProps);
        }

        public static List<FileMatch> FindDupes(IEnumerable<FileMatch> files)
        {
            var dupes = new List<FileMatch>();
            var fileMatches = files.OrderBy(f => f.CopyNumber).ToArray();

            for (var i = 0; i < fileMatches.Length; i++)
            {
                var left = fileMatches[i];

                for (var j = i + 1; j < fileMatches.Length; j++)
                {
                    var right = fileMatches[j];
                    if (left.FullName == right.FullName || dupes.Contains(right))
                    {
                        continue;
                    }

                    string format;
                    if (right.IsDuplicateOf(left))
                    {
                        format = $"{left.FullName} is a dupe of {right.FullName}";
                        dupes.Add(right);
                    }
                    else
                    {
                        format = $"{left.FullName} is NOT a dupe of {right.FullName}";
                    }
                    Console.WriteLine(format);
                }
            }
            return dupes;
        }

        public class FileMatch
        {
            public FileMatch(string fullName, string matchName, int copyNumber, string extension, Dictionary<string, string> testProps)
            {
                FullName = fullName;
                MatchName = matchName;
                CopyNumber = copyNumber;
                Extension = extension;
               
                TestProps = testProps;
            }

            public string Extension { get; }
            public Dictionary<string, string> TestProps { get; }
            public string FullName { get; }
            public string MatchName { get; }
            public int CopyNumber { get; }

            public bool IsDuplicateOf(FileMatch other)
            {
                if (FullName == other.FullName)
                    return false;

                return string.Equals(Extension, other.Extension) 
                    && string.Equals(MatchName.ToLower(), other.MatchName.ToLower()) 
                    && CopyNumber != other.CopyNumber
                    && AllTestPropsMatch(other);
            }

            private bool AllTestPropsMatch(FileMatch other)
            {
                if (TestProps.Count != other.TestProps.Count)
                    return false;

                bool allTestPropsMatch = true;
                foreach (var kvp in TestProps)
                {
                    var thisTestProp = kvp.Value;
                    if (other.TestProps.TryGetValue(kvp.Key, out string otherTestProp))
                    {
                        if (otherTestProp != thisTestProp)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return allTestPropsMatch;
            }
        }
    }
}