using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;

namespace DupeFinder
{
    public class DupelicateFinder
    {
        private static readonly Regex Regex = new Regex(@"\\(?<name>[^\\]*?)(?<dupe>\s\((?<number>\d)\))?\.");

        public static void CheckFolderForDupes(string path)
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
    }
}