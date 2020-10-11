using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Common.DataBase;
using Common.DataBase.Entities;
using WebApp.Models.Solution;
using WebApp.Exceptions;
using Common.Helpers;
using System.Collections.Generic;
using Common.Environment;

namespace WebApp.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly IDbContext _dbContext;

        public SolutionsController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetInputFile/{fileName}/{challengeId}")]
        public async Task<IActionResult> GetInputFile(string challengeId, string fileName)
        {
            var file = await _dbContext.Challenges.AsQueryable().Where(s => s.Id == challengeId)
                .Select(s => s.Files.First(f => f.Name == fileName && f.FileType == FileType.InputFile)).FirstOrDefaultAsync();
            if (file == null)
                throw new NotFoundException("File you requested can not be found!");

            return Ok(file);
        }

        [HttpGet("GetFile/{fileName}/{solutionId}")]
        public async Task<IActionResult> GetFile(string solutionId, string fileName)
        {            
            var file = await _dbContext.Solutions.AsQueryable().Where(s => s.Id == solutionId)
                .Select(s => s.Files.First(f => f.Name == fileName)).FirstOrDefaultAsync();
            if (file == null)
                throw new NotFoundException("File you requested can not be found!");

            return Ok(file);
        }        

        [HttpGet("Solve/{challengeId}")]
        public async Task<IActionResult> Solve(string challengeId) //GetSolutionOrCreateIfNotExists implement this function in 
        {
            var challenge = await GetChallengeDetails(challengeId);

            ValidateChallengeDueDateAndPrivacyDomain(challenge);
            
            var existingSolution = await TryGetExistingSolutionDetails(challengeId);

            SolutionEnvironmentVm viewModel;
            if (existingSolution != null)
            {
                viewModel = new SolutionEnvironmentVm
                {
                    SolutionId = existingSolution.Id,
                    CreationDate = existingSolution.CreationDate,
                    FileNames = existingSolution.Files.Select(f => f.Name),
                    SuccessfulTestCount = existingSolution.SuccessfulTestCount,
                };                
            }
            else
            {
                var templateFiles = await TryGetTemplateFiles(challengeId);

                var solution = new Solution
                {
                    OwnerId = HttpContext.User.Id(),
                    OwnerDisplayName = HttpContext.User.DisplayName(),
                    CreationDate = DateTime.UtcNow,
                    ChallengeId = challengeId,
                    ProgramingLanguage = challenge.ProgramingLanguage,
                    Files = templateFiles.Select(f => new SolutionFile { Name = f.Name, Content = f.Content }),
                    TestCaseCount = challenge.TestCaseCount
                };

                await CreateSolution(solution);

                viewModel = new SolutionEnvironmentVm
                {
                    SolutionId = solution.Id,
                    CreationDate = solution.CreationDate,
                    FileNames = templateFiles.Select(f => f.Name)
                };
            }         
            
            
            var inputFileNames = await TryGetInputFileNames(challengeId);
            
            var testCases = await GetExampleTestCases(challengeId);


            viewModel.ChallengeId = challengeId;
            viewModel.InputFileNames = inputFileNames;
            viewModel.ProgramingLanguage = challenge.ProgramingLanguage;
            viewModel.ProblemDefinition = challenge.ProblemDefinition;
            viewModel.TestCaseCount = challenge.TestCaseCount;
            viewModel.TestCases = testCases;            

            return View(viewModel);

            #region LOCAL METHOD DEFINITIONS

            async Task<Challenge> GetChallengeDetails(string challengeId)
            {
                if (string.IsNullOrEmpty(challengeId))
                    throw new NotFoundException("No Challenge exists with the given id");

                var projection = Builders<Challenge>.Projection.Expression(c => new Challenge
                {
                    DueDate = c.DueDate,
                    PrivacyLevel = c.PrivacyLevel,
                    PrivacyDomain = c.PrivacyDomain,
                    ProgramingLanguage = c.ProgramingLanguage,
                    TestCaseCount = c.TestCaseCount,
                    ProblemDefinition = c.ProblemDefinition
                });

                var challenge = await _dbContext.Challenges.Find(c => c.Id == challengeId).Project(projection).FirstOrDefaultAsync();

                if (challenge == null)
                {
                    throw new NotFoundException("No Challenge exists with the given id");
                }

                return challenge;
            }

            void ValidateChallengeDueDateAndPrivacyDomain(Challenge challenge)
            {
                if (challenge.DueDate < DateTime.UtcNow)
                    throw new BadRequestException("This challenge is already due");

                if (challenge.PrivacyLevel == ChallengePrivacyLevel.ShareWithDomain)
                {
                    string domain = HttpContext.User.Email().Split("@")[1];
                    if (challenge.PrivacyDomain != domain)
                        throw new UnauthorizedException("This challenge requires specific domain address");
                }
            }

            async Task<Solution> TryGetExistingSolutionDetails(string challengeId)
            {
                return await _dbContext.Solutions
                    .Find(s => s.ChallengeId == challengeId && s.OwnerId == HttpContext.User.Id())
                    .Project(Builders<Solution>.Projection.Expression(s => new Solution
                    {
                        Id = s.Id,
                        CreationDate = s.CreationDate,
                        Files = s.Files,
                        SuccessfulTestCount = s.SuccessfulTestCount
                    }))
                    .FirstOrDefaultAsync();
            }

            async Task<IEnumerable<ChallengeFile>> TryGetTemplateFiles(string challengeId)
            {        
                return await _dbContext.Challenges.AsQueryable()
                    .Where(c => c.Id == challengeId)
                    .Select(c => c.Files.Where(f => f.FileType == FileType.TemplateFile))
                    .FirstOrDefaultAsync();
            }

            async Task<IEnumerable<string>> TryGetInputFileNames(string challengeId)
            {
                return await _dbContext.Challenges.AsQueryable()
                    .Where(c => c.Id == challengeId)
                    .Select(c => c.Files.Where(f => f.FileType == FileType.InputFile).Select(f => f.Name))
                    .FirstOrDefaultAsync();
            }

            async Task<IEnumerable<TestCase>> GetExampleTestCases(string challengeId)
            {
                return await _dbContext.Challenges.AsQueryable()
                .Where(c => c.Id == challengeId)
                .Select(c => c.TestCases.Where(t => !t.IsHidden))
                .FirstOrDefaultAsync();
            }

            async Task CreateSolution(Solution solution)
            {
                await _dbContext.Solutions.InsertOneAsync(solution);

                //Update participant counter of the related challenge by one
                await _dbContext.Challenges.UpdateOneAsync(c => c.Id == solution.ChallengeId, Builders<Challenge>.Update.Inc(c => c.ParticipantCount, 1));
            }
            
            #endregion
        }        

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new NotFoundException("No Solution exists with the given id");

            await _dbContext.Solutions.DeleteOneAsync(c => c.Id == id);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("MySolutions")]
        public async Task<IActionResult> MySolutions()
        {
            var solutions = await _dbContext.Solutions.Find(s => s.OwnerId == HttpContext.User.Id()).ToListAsync();
            return View(solutions);
        }
    }
}
