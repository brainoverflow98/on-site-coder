using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Common.DataBase;
using Common.DataBase.Entities;
using Common.Helpers;
using WebApp.Models.Challenge;
using WebApp.Helpers;
using WebApp.Exceptions;
using Common.Environment;

namespace WebApp.Controllers
{
    public class ChallengesController : Controller
    {
        private readonly IDbContext _dbContext;

        public ChallengesController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ChallengeCreationModel form)
        {
            new ChallengeCreationModelValidator().ValidateOrThrow(form);

            var no = 1;
            foreach (var testCase in form.TestCases)
            {
                testCase.No = no++;
            }

            #region Map form to entity
            var challenge = new Challenge
            {
                OwnerId = HttpContext.User.Id(),
                OwnerDisplayName = HttpContext.User.DisplayName(),
                CreationDate = DateTime.UtcNow,
                DueDate = form.DueDate,
                PrivacyLevel = form.PrivacyLevel,
                PrivacyDomain = form.PrivacyDomain ?? "",
                AllowSolutionUpload = form.AllowSolutionUpload,
                Name = form.Name,
                ProgramingLanguage = form.ProgramingLanguage,
                ProblemDefinition = form.ProblemDefinition,
                Files = form.Files,
                TestCases = form.TestCases,
                TestCaseCount = form.TestCases.Count()
            };
            #endregion

            await _dbContext.Challenges.InsertOneAsync(challenge);             

            return RedirectToAction("Index", "Home");
        }        

        
        [HttpGet("MyChallenges/{id?}")]
        public async Task<IActionResult> MyChallenges(string id)
        {
            MyChallengesVm viewModel = null;
            if (!string.IsNullOrEmpty(id))
            {
                #region GET CURRENT CHALLENGE DETAILS
                var projection = Builders<Challenge>.Projection.Expression(c => new MyChallengesVm
                {
                    CurrentChallengeDetails = new MyChallengesVm.ChallengeDetails
                    {
                        Id = c.Id,
                        CreationDate = c.CreationDate,
                        DueDate = c.DueDate,
                        PrivacyLevel = c.PrivacyLevel,
                        PrivacyDomain = c.PrivacyDomain,
                        Name = c.Name,
                        ProgramingLanguage = c.ProgramingLanguage,
                        ProblemDefinition = c.ProblemDefinition
                    }
                });

                viewModel = await _dbContext.Challenges.Find(c => c.Id == id).Project(projection).FirstOrDefaultAsync();
                #endregion

                #region ADD SOLUTIONS TO VIEW MODEL FOR THE CURRENT CHALLENGE
                var solutions = await _dbContext.Solutions.Find(s => s.ChallengeId == id)
                        .Project(Builders<Solution>.Projection.Expression(s => new MyChallengesVm.ChallengeDetails.Solution
                        {
                            OwnerName = s.OwnerDisplayName,
                            CreationDate = s.CreationDate,
                            TestCaseCount = s.TestCaseCount,
                            SuccessfulTestCount = s.SuccessfulTestCount
                        }))
                        .ToListAsync();

                viewModel.CurrentChallengeDetails.Solutions = solutions;
                #endregion
            }
            
            #region ADD MY CHALLENGE LIST TO VIEW MODEL
            var myChallengeList = await _dbContext.Challenges.Find(c => c.OwnerId == HttpContext.User.Id())
                    .Project(Builders<Challenge>.Projection.Expression(c => new MyChallengesVm.ChallengeListItem
                    {
                        Id = c.Id,
                        Name = c.Name
                    }))
                    .ToListAsync();

            if (viewModel == null)
            {
                viewModel = new MyChallengesVm();
            }

            viewModel.MyChallengeList = myChallengeList; 
            #endregion            

            return View(viewModel);
        }
        

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new NotFoundException("No Challenge exists with the given id");

            await _dbContext.Solutions.DeleteManyAsync(s => s.ChallengeId == id);

            await _dbContext.Challenges.DeleteOneAsync(c => c.Id == id);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Explore")]
        public async Task<IActionResult> Explore([FromQuery]SearchForm form)
        {
            #region Map entity to view model
            var projection = Builders<Challenge>.Projection.Expression(c => new ChallengeOverviewModel
            {
                Id = c.Id,
                OwnerId = c.OwnerId,
                OwnerDisplayName = c.OwnerDisplayName,
                CreationDate = c.CreationDate,
                DueDate = c.DueDate,
                Name = c.Name,
                ProgramingLanguage = c.ProgramingLanguage,
                ParticipantCount = c.ParticipantCount
            });
            #endregion

            if (!string.IsNullOrWhiteSpace(form.SearchText))
            {
                var filter = Builders<Challenge>.Filter.Where(c => c.Id == form.SearchText);
                var challenge = await _dbContext.Challenges.Find(filter).Project(projection).FirstOrDefaultAsync();
                return View(new List<ChallengeOverviewModel> { challenge });
            }

            #region Create filter
            var filterList = new List<FilterDefinition<Challenge>>();

            filterList.Add(Builders<Challenge>.Filter.Where(c => c.PrivacyLevel == ChallengePrivacyLevel.Public));

            

            if (form.ProgramingLanguage != null)
                filterList.Add(Builders<Challenge>.Filter.Where(c => c.ProgramingLanguage == form.ProgramingLanguage));

            var finalFilter = Builders<Challenge>.Filter.And(filterList);
            #endregion

            

            #region Apply filter and sorting. Then get the data
            var tempQuery = _dbContext.Challenges.Find(finalFilter).Project(projection).Skip(form.PerPage * (form.PageNo - 1)).Limit(form.PerPage);

            IEnumerable<ChallengeOverviewModel> model;
            if (form.SortBy == SortField.ParticipantCount)
            {
                model = await tempQuery.SortByDescending(c => c.ParticipantCount).ToListAsync();                
            }
            else //if(form.SortBy == SortField.ParticipantCount)
            {
                model = await tempQuery.SortByDescending(c => c.CreationDate).ToListAsync();
            }
            #endregion

            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(ChallengeDetailsModel form)
        //{
        //    new ChallengeDetailsModelValidator(_user.Role).ValidateOrThrow(form);

        //    var no = 1;
        //    foreach (var testCase in form.TestCases)
        //    {
        //        testCase.No = no++;
        //    }

        //    #region Select entity fields update
        //    var challenge = Builders<Challenge>.Update
        //            .Set(c => c.DueDate, form.DueDate)
        //            .Set(c => c.PrivacyLevel, form.PrivacyLevel)
        //            .Set(c => c.PrivacyDomain, form.PrivacyDomain)
        //            .Set(c => c.AllowSolutionUpload, form.AllowSolutionUpload)
        //            .Set(c => c.Name, form.Name)
        //            .Set(c => c.ProblemDefinition, form.ProblemDefinition)
        //            .Set(c => c.Files, form.Files)
        //            .Set(c => c.TestCases, form.TestCases)
        //            .Set(c => c.TestCaseCount, form.TestCases.Count());
        //    #endregion

        //    await _dbContext.Challenges.UpdateOneAsync(c => c.Id == form.Id, challenge);

        //    return RedirectToAction("Index", "Home");
        //}

    }
}
