using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DupeFinder.Test.Unit
{
    public class ProgramTests
    {

        [Test]
        public void TestFindDupesWhenSingleFile()
        {
            var fileMatch = new Program.FileMatch("file name", "match name", 0, "jpg", new Dictionary<string, string>());

            var fileMatches = Program.FindDupes(new[] { fileMatch });
            Assert.That(fileMatches, Is.Not.Null.And.Empty);
        }

        [Test]
        public void TestFindDupesWhenSameFileTwice()
        {
            var fileMatch = new Program.FileMatch("file name", "match name", 0, "jpg", new Dictionary<string, string>());
            var fileMatches = Program.FindDupes(new[] { fileMatch, fileMatch });
            Assert.That(fileMatches, Is.Not.Null.And.Empty);
        }
        [Test]
        public void TestFindDupesWhenSingleDupe()
        {
            var fileMatch1 = new Program.FileMatch("file name1", "match name", 0, "jpg", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("file name2", "match name", 1, "jpg", new Dictionary<string, string>());
            var fileMatches = Program.FindDupes(new[] { fileMatch1, fileMatch2 });

            Assert.That(fileMatches, Is.Not.Null.And.Count.EqualTo(1));
            Assert.That(fileMatches.First(), Is.EqualTo(fileMatch2));
        }
        [Test]
        public void TestFindDupesWhenTwoDupe()
        {
            var fileMatch1 = new Program.FileMatch("file name1", "match name", 0, "jpg", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("file name2", "match name", 1, "jpg", new Dictionary<string, string>());
            var fileMatch3 = new Program.FileMatch("file name3", "match name", 2, "jpg", new Dictionary<string, string>());
            var fileMatches = Program.FindDupes(new[] { fileMatch1, fileMatch2, fileMatch3 });

            Assert.That(fileMatches, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(fileMatches.First(), Is.EqualTo(fileMatch2));
            Assert.That(fileMatches.Last(), Is.EqualTo(fileMatch3));
        }
    }
}
