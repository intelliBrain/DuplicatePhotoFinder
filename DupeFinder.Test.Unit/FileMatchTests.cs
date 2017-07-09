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
            var fileMatch1 = new Program.FileMatch(fullName, null, 0, null, new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch(fullName, null, 1, null, new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenFileNameDiffButCopyNumberSameThenNotADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", null, 1, null, new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("File2", null, 1, null, new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.False);
        }

        [Test]
        public void TestWhenMatchNameSameThenADupe()
        {
            var fileMatch1 = new Program.FileMatch("File1", "File", 1, null, new Dictionary<string, string>());
            var fileMatch2 = new Program.FileMatch("File2", "File", 2, null, new Dictionary<string, string>());

            Assert.That(fileMatch1.IsDuplicateOf(fileMatch2), Is.True);
        }
    }
}
