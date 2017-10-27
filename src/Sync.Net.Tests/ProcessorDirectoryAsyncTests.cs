﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sync.Net.Tests
{
    [TestClass]
    public class ProcessorDirectoryAsyncTests : ProcessorFullDirectoryTests
    {
        [TestMethod]
        public void UploadsDirectory()
        {
            _syncNet.ProcessDirectory(_sourceDirectory);
            AssertEx.EqualStructure(_sourceDirectory, _targetDirectory);
        }
    }
}