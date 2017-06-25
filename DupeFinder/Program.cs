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
    internal class Program
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
                ).ToLookup(fm => fm.MatchName + fm.Extension)
                .Where(grp => grp.Count() > 1)
                .ToList();

            foreach (var grp in enumerable)
            {
                var same = new List<FileMatch>();
                var different = new List<FileMatch>();
                var grpKey = grp.Key;
                for (var i = 0; i < grp.Count(); i++)
                {
                    var left = grp.ElementAt(i);

                    for (var j = i + 1; j < grp.Count(); j++)
                    {
                        var right = grp.ElementAt(j);
                        string format;
                        if (right.IsDuplicateOf(left)) format = $"{left.FullName} is a dupe of {right.FullName}";
                        else
                            format = $"{left.FullName} is NOT a dupe of {right.FullName}";
                        Console.WriteLine(format);
                    }
                }
            }
            Console.ReadLine();
        }

        private class FileMatch
        {
            public FileMatch(string fullName, string matchName)
            {
                FullName = fullName;
                MatchName = matchName;
                var info = ShellFile.FromFilePath(FullName);
                var systemProps = info.Properties.System;
                Extension = systemProps.FileExtension.Value.ToLower();

                var imageProps = systemProps.Image;
                var vidProps = systemProps.Video;

                TestProps = new Dictionary<string, string>
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
            }

            public string Extension { get; }

            public Dictionary<string, string> TestProps { get; }

            public string FullName { get; }
            public string MatchName { get; }
            public bool IsSuffixed => FullName != MatchName;

            public bool IsDuplicateOf(FileMatch other)
            {
                bool all = true;
                foreach (var kvp in TestProps)
                {
                    var thisTestProp = kvp.Value;
                    var otherTestProp = other.TestProps[kvp.Key];
                    if (otherTestProp != thisTestProp)
                    {
                        all = false;
                        break;
                    }
                }
                return string.Equals(Extension, other.Extension) 
                    && string.Equals(MatchName.ToLower(), other.MatchName.ToLower()) 
                    && all;
            }
        }
    }
}