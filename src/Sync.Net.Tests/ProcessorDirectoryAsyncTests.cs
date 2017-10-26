using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorDirectoryAsyncTests : ProcessorFullDirectoryTests
    {
        [TestMethod]
        public async Task UploadsDirectory()
        {
            await _syncNet.ProcessDirectoryAsync(_sourceDirectory);
            AssertEx.EqualStructure(_sourceDirectory, _targetDirectory);
        }
    }
}