using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorFileAsyncTests : ProcessorFullDirectoryTests
    {
        [TestMethod]
        public void UploadsFile()
        {
            _syncNet.CopyFile(_sourceDirectory.GetFile(DirectoryHelper.FileName));

            var files = _targetDirectory.GetFiles();
            Assert.AreEqual(DirectoryHelper.FileName, files.First().Name);
        }

        [TestMethod]
        public void UploadFromSubfolderDoesntCreateAFileInMainDirectory()
        {
            _syncNet.CopyFile(_sourceDirectory.GetDirectory(DirectoryHelper.SubDirectoryName).GetFile(DirectoryHelper.SubFileName));

            var files = _targetDirectory.GetFiles();

            Assert.IsTrue(!files.Any());
        }

        [TestMethod]
        public void UploadFromSubfolderCreatesASubDirectory()
        {
            _syncNet.CopyFile(_sourceDirectory.GetDirectory(DirectoryHelper.SubDirectoryName).GetFile(DirectoryHelper.SubFileName));

            var dirs = _targetDirectory.GetDirectories();

            Assert.IsTrue(dirs.Count() == 1);
            Assert.AreEqual(DirectoryHelper.SubFileName, dirs.First().GetFiles().First().Name);
        }
    }
}