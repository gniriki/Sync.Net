using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.Processing;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    public class ProcessorFullDirectoryTests
    {
        protected MemoryDirectoryObject _sourceDirectory;
        protected Processor _syncNet;
        protected MemoryDirectoryObject _targetDirectory;

        [TestInitialize]
        public void Initialize()
        {
            var memoryDirectoryObject = DirectoryHelper.CreateFullDirectory();

            _sourceDirectory = memoryDirectoryObject;

            _targetDirectory = new MemoryDirectoryObject("targetDirectory");

            _syncNet = new Processor(_sourceDirectory, _targetDirectory, new SyncTaskQueue());
        }
    }
}