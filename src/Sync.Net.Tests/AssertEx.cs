using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.IO;

namespace Sync.Net.Tests
{
    public static class AssertEx
    {
        public static void EqualStructure(IDirectoryObject source, IDirectoryObject target)
        {
            var sourceFiles = source.GetFiles().ToList();
            var targetFiles = target.GetFiles().ToList();
            foreach (var sourceFile in sourceFiles)
            {
                Assert.IsTrue(targetFiles.Any(t => t.Name == sourceFile.Name));
            }

            var subDirectories = source.GetDirectories();

            foreach (var sourceSubDir in subDirectories)
            {
                var targetSubDir = target.GetDirectory(sourceSubDir.Name);
                Assert.IsTrue(targetSubDir.Exists);
                AssertEx.EqualStructure(sourceSubDir, targetSubDir);
            }
        }
    }
}
