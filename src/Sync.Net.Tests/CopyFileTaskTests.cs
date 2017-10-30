using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Net.Processing;
using Sync.Net.TestHelpers;

namespace Sync.Net.Tests
{
    [TestClass]
    public class CopyFileTaskTests
    {
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void ThrowsWhenFileIsLocked()
        {
            var file = new MemoryFileObject("name", "contents");
            var targetDirectory = new MemoryDirectoryObject("target");

            file.IsReady = false;
            file.Exists = true;

            var copyFile = new CopyFileTask(file,
                targetDirectory, new TimeSpan(0, 0, 1));

            copyFile.Execute();
        }
    }
}
