using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutionService.Services
{
    public class ExecutionTaskQueue
    {
        private ConcurrentQueue<ExecutionTask> _taskQueue = new ConcurrentQueue<ExecutionTask>();
        //new BlockingCollection<string>(new ConcurrentQueue<string>(), MaxQueueSize);
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(ExecutionTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            _taskQueue.Enqueue(task);
            _signal.Release();
        }

        public async Task<ExecutionTask> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _taskQueue.TryDequeue(out var workItem);

            return workItem;
        }
    }

    public enum Command
    {
        CreateEnvironment,
        DeleteEnvironment,
        SaveFile,
        CreateFile,
        DeleteFile,
        CompileAndExecute
    }

    public class ExecutionTask
    {
        public string UserId { get; set; }
        public Command Command { get; set; }
        public object Data { get; set; }
    }
}
