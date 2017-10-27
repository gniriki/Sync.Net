using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.Processing
{
    public class SyncTaskQueue : TaskQueue
    {
        public override void Queue(ITask task)
        {
            OnTaskStarting(task);
            task.Execute();
            OnTaskCompleted(task);
        }
    }
}
