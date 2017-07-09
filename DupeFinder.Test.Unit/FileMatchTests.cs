using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DupeFinder.Test.Unit
{
    class FileMatchTests
    {
        [Test]
        public void TestWhenFullNameEqualThenNotADupe()
        {
            var fullName = "File";
            var fileMatch1 = new Program.FileMatch(fullName, "Match", 0, "JPG", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch(fullName, "Match", 1, "JPG", new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenFileNameDiffButCopyNumberSameThenNotADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "Match", 1, "JPG", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("File2", "Match", 1, "JPG", new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenMatchNameSameThenADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "Match", 1, "JPG", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("File2", "Match", 2, "JPG", new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.True);
        }

        [Test]
        public void TestWhenMatchNameDiffThenNotADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "Match1", 1, "JPG", new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("File2", "Match2", 2, "JPG", new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenTestPropsTheSameThenADupe()
        {
            var testProps = new Dictionary<string, string> { ["A"] = "1", ["b"] = "2" };
            var fileMatch1 = new Program.FileMatch("File1", "Match", 1, "JPG", testProps);
            var fileMatch2 = new Program.FileMatch("File2", "Match", 2, "JPG", testProps);

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.True);
        }
        [Test]
        public void TestWhenDifferentTestPropKeysThenNotADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "Match", 1, "JPG", new Dictionary<string, string> { ["A"] = "1", ["b"] = "2" });
            var fileMatch2 = new Program.FileMatch("File2", "Match", 2, "JPG", new Dictionary<string, string> { ["b"] = "2", ["C"] = "1"});

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenTestPropsNotTheSameThenNotADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "Match", 1, "JPG", new Dictionary<string, string> { ["A"] = "1", ["b"] = "2" });
            var fileMatch2 = new Program.FileMatch("File2", "Match", 2, "JPG", new Dictionary<string, string> { ["A"] = "3", ["b"] = "2" });

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }
    }
}
