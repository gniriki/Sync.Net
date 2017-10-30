using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorDirectoryAsyncTests : ProcessorFullDirectoryTests
    {
        [TestMethod]
        public void UploadsDirectory()
        {
            _syncNet.ProcessDirectory(_sourceDirectory.GetDirectory(DirectoryHelper.SubDirectoryName));
            AssertEx.EqualStructure(_sourceDirectory.GetDirectory(DirectoryHelper.SubDirectoryName), 
                _targetDirectory.GetDirectory(DirectoryHelper.SubDirectoryName));
        }
    }
}