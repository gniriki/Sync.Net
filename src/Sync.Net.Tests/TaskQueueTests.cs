using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sync.Net.Processing;

namespace Sync.Net.Tests
{
    [TestClass]
    public class TaskQueueTests
    {
        [TestMethod]
        public void RetriesTask()
        {
            var queue = new Mock<TaskQueue>();
            queue.CallBase = true;

            var wasExecuted = false;
            var task = new Mock<ITask>();
            task.Setup(x => x.Execute()).Callback(() =>
            {
                if (!wasExecuted)
                {
                    wasExecuted = true;
                    throw new TimeoutException();
                }
            });

            queue.Object.ExecuteTask(task.Object);
            task.Verify(x => x.Execute(), Times.Exactly(2));
        }
    }
}
