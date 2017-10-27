using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.Processing
{
    public class SyncTaskQueue : ITaskQueue
    {
        public void Queue(ITask task)
        {
            task.Execute();
        }
    }
}
