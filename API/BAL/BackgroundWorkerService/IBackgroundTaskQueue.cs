using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.BackgroundWorkerService
{
    public interface IBackgroundTaskQueue
    {
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        public Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
