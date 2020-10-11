using Common.DataBase;
using Common.DataBase.Entities;
using Common.Environment;
using ExecutionService.Exceptions;
using ExecutionService.Hubs;
using ExecutionService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutionService.Services
{
    public class ExecutionBackgroundService : BackgroundService
    {
        private readonly ExecutionTaskQueue _taskQueue;        
        private readonly DockerExecutionService _executionService;
        private readonly IDbContext _dbContext;
        private readonly IHubContext<ExecutionHub> _hubContext;
        private readonly ILogger<ExecutionBackgroundService> _logger;

        public ExecutionBackgroundService
        (
            ExecutionTaskQueue taskQueue,
            DockerExecutionService executionService,
            IDbContext dbContext,
            IHubContext<ExecutionHub> hubContext,
            ILogger<ExecutionBackgroundService> logger
        )
        {
            _taskQueue = taskQueue;            
            _executionService = executionService;
            _dbContext = dbContext;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var service1 =  BackgroundProcessing(stoppingToken);
            var service2 = BackgroundProcessing(stoppingToken);
            var service3 = BackgroundProcessing(stoppingToken);

            await Task.WhenAll(service1, service2, service3);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            var processId = Guid.NewGuid();
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _taskQueue.DequeueAsync(stoppingToken);
                _logger.LogInformation("Background Service: "+ processId + " processing " + task.Command.ToString() + " request from User: " + task.UserId);
                try
                {
                    switch (task.Command)
                    {

                        case Command.SaveFile:
                            {
                                var form = (FileSaveForm)task.Data;                                

                                var filter1 = Builders<Solution>.Filter.Where(c => c.Id == form.SolutionId);

                                var update1 = Builders<Solution>.Update.PullFilter(s => s.Files, f => f.Name == form.FileName);
                                var result1 = (await _dbContext.Solutions.UpdateOneAsync(filter1, update1));

                                var update2 = Builders<Solution>.Update.Push(s => s.Files, new SolutionFile { Name = form.FileName, Content = form.FileContent });
                                var result2 = (await _dbContext.Solutions.UpdateOneAsync(filter1, update2));

                                if (result1.ModifiedCount < 1 || result2.ModifiedCount < 1)
                                    throw new ExecutionServiceException("File could not be saved try again later or refresh the page!");

                                _executionService.SaveFile(task.UserId, form.FileName, form.FileContent);
                            }
                            break;

                        case Command.CreateFile:
                            {
                                var form = (FileCreateForm)task.Data;

                                var filter1 = Builders<Solution>.Filter.Where(c => c.Id == form.SolutionId);
                                var filter2 = Builders<Solution>.Filter.ElemMatch(c => c.Files, f => f.Name == form.FileName);
                                var existInSolution = await _dbContext.Solutions.Find(Builders<Solution>.Filter.And(filter1, filter2)).AnyAsync();

                                var challengeId = await _dbContext.Solutions.AsQueryable()
                                    .Where(s => s.Id == form.SolutionId)
                                    .Select(s => s.ChallengeId).FirstAsync();

                                var filter3 = Builders<Challenge>.Filter.Where(c => c.Id == challengeId);
                                var filter4 = Builders<Challenge>.Filter.ElemMatch(c => c.Files, f => f.Name == form.FileName);
                                var existInChallenge = await _dbContext.Challenges.Find(Builders<Challenge>.Filter.And(filter3, filter4)).AnyAsync();

                                if (existInChallenge || existInSolution)
                                    throw new ExecutionServiceException("File name already exists in the environment !");

                                var update = Builders<Solution>.Update.Push(s => s.Files, new SolutionFile { Name = form.FileName, Content = "" });
                                await _dbContext.Solutions.UpdateOneAsync(s => s.Id == form.SolutionId, update);

                                _executionService.CreateFile(task.UserId, form.FileName);
                            }
                            break;

                        case Command.DeleteFile:
                            {
                                var form = (FileDeleteForm)task.Data;

                                var filter = Builders<Solution>.Filter.Where(c => c.Id == form.SolutionId);
                                var update = Builders<Solution>.Update.PullFilter(s => s.Files, f => f.Name == form.FileName);
                                var result = (await _dbContext.Solutions.UpdateOneAsync(filter, update));

                                if (result.ModifiedCount < 1)
                                    throw new ExecutionServiceException("File could not be deleted try again later.");

                                _executionService.DeleteFile(task.UserId, form.FileName);
                            }
                            break;

                        case Command.CreateEnvironment:
                            {
                                var solutionId = (string)task.Data;
                                
                                var solution = await _dbContext.Solutions.AsQueryable()
                                    .Where(s => s.Id == solutionId)
                                    .Select(s => new { s.OwnerId, s.Files, s.ChallengeId }).FirstOrDefaultAsync();

                                if (solution == null || task.UserId != solution.OwnerId)
                                    throw new ExecutionServiceException("Could not find the solution with the given id.");

                                var challenge = await _dbContext.Challenges.AsQueryable().Where(c => c.Id == solution.ChallengeId)
                                    .Select(c => new { Files = c.Files.Where(f => f.FileType != FileType.TemplateFile), c.TestCases, c.ProgramingLanguage }).FirstOrDefaultAsync();

                                var allFiles = new List<SolutionFile>();
                                if (solution.Files != null)
                                    allFiles.AddRange(solution.Files);
                                if (challenge.Files != null)
                                    allFiles.AddRange(challenge.Files.Select(f => new SolutionFile { Name = f.Name, Content = f.Content }));

                                _executionService.CreateEnvironment(task.UserId, challenge.ProgramingLanguage, allFiles, challenge.TestCases);
                            }
                            break;

                        case Command.DeleteEnvironment:
                            {
                                _executionService.DeleteEnvironment(task.UserId);
                            }
                            break;

                        case Command.CompileAndExecute:
                            {
                                var solutionId = (string)task.Data;
                                var result = _executionService.CompileAndExecute(task.UserId);
                                
                                await _dbContext.Solutions.UpdateOneAsync(s => s.Id == solutionId, Builders<Solution>.Update.Set(s => s.SuccessfulTestCount, result.SuccessfulTestCount));

                                await _hubContext.Clients.User(task.UserId)
                                    .SendAsync(task.Command.ToString(), new { Data = result });                                
                            }
                            break;
                    }
                }
                catch (ExecutionServiceException e)
                {
                    await _hubContext.Clients.User(task.UserId).SendAsync(task.Command.ToString(), new { Error = e.Message });
                    _logger.LogWarning("Error while processing " + task.Command.ToString() + " request from User: " + task.UserId + "Error Message: " + e.Message);
                }
                catch (Exception e)
                {
                    await _hubContext.Clients.User(task.UserId).SendAsync(task.Command.ToString(), new { Error = "An Error Occured" });
                    var message = new StringBuilder();
                    message.AppendLine("Unexpected Error occurred while processing " + task.Command.ToString() + " from User: " + task.UserId);
                    message.AppendLine("Error Date: " + DateTime.UtcNow);
                    message.AppendLine("==> Data Object <==");
                    message.AppendLine(JsonConvert.SerializeObject(task.Data, Formatting.Indented));
                    message.AppendLine(" ");
                    message.AppendLine("==> Exception Details <==");
                    message.AppendLine(e.ToString());
                    _logger.LogError(message.ToString());
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Container Background Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
