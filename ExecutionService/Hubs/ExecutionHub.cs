using Common.Helpers;
using ExecutionService.Models;
using ExecutionService.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ExecutionService.Hubs
{   
    public class ExecutionHub : Hub
    {
        private readonly ExecutionTaskQueue _taskQueue;
        private readonly ILogger<ExecutionHub> _logger;
        public ExecutionHub(ExecutionTaskQueue taskQueue, ILogger<ExecutionHub> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("User Conected, ID: " + Context.User.Id());          
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("User Disconected, ID: " + Context.User.Id());
            var task = new ExecutionTask { Command = Command.DeleteEnvironment, UserId = Context.User.Id() };
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.DeleteEnvironment.ToString() + " request added to Background Task Queue");
            await base.OnDisconnectedAsync(exception);
        }

        public void CreateEnvironment(string solutionId)
        {            
            var task = new ExecutionTask { UserId = Context.User.Id(), Command = Command.CreateEnvironment, Data = solutionId };
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.CreateEnvironment.ToString() + " request added to Background Task Queue");
        }       

        public void SaveFile(string solutionId, string fileName, string fileContent)
        {
            var form = new FileSaveForm
            {                
                SolutionId = solutionId,
                FileName = fileName,
                FileContent = fileContent
            };

            var task = new ExecutionTask { UserId = Context.User.Id(), Command = Command.SaveFile, Data = form };
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.SaveFile.ToString() + " request added to Background Task Queue");
        }

        public void CreateFile(string solutionId, string fileName)
        {
            var form = new FileCreateForm
            {                
                SolutionId = solutionId,
                FileName = fileName
            };
            var task = new ExecutionTask { UserId = Context.User.Id(), Command = Command.CreateFile, Data = form };
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.CreateFile.ToString() + " request added to Background Task Queue");
        }

        public void DeleteFile(string solutionId, string fileName)
        {
            var form = new FileDeleteForm
            {
                SolutionId = solutionId,
                FileName = fileName
            };
            var task = new ExecutionTask { UserId = Context.User.Id(), Command = Command.DeleteFile, Data = form };
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.DeleteFile.ToString() + " request added to Background Task Queue");
        }

        public void CompileAndExecute(string solutionId)
        {
            var task = new ExecutionTask { UserId = Context.User.Id(), Command = Command.CompileAndExecute, Data = solutionId};
            _taskQueue.Enqueue(task);
            _logger.LogInformation(Command.CompileAndExecute.ToString() + " request added to Background Task Queue");
        }
    }
}
