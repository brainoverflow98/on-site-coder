using FluentValidation;
using System;
using System.Collections.Generic;
using Common.DataBase.Entities;
using Common.Environment;

namespace WebApp.Models.Challenge
{
    public class ChallengeCreationModel
    { 
        public DateTime DueDate { get; set; }
        public ChallengePrivacyLevel PrivacyLevel { get; set; }
        public string PrivacyDomain { get; set; }
        public bool AllowSolutionUpload { get; set; }
        public string Name { get; set; }
        public ProgramingLanguage ProgramingLanguage { get; set; }
        public string ProblemDefinition { get; set; }
        public List<ChallengeFile> Files { get; set; }
        public List<TestCase> TestCases { get; set; }
    }

    internal class ChallengeCreationModelValidator : AbstractValidator<ChallengeCreationModel>
    {
        public ChallengeCreationModelValidator()
        {
            //RuleFor(d => d.DisplayName).NotEmpty().MinimumLength(8).MaximumLength(20).Matches(@"[a-zA-Z0-9]*");
            //RuleFor(d => d.Email).EmailAddress();
            //RuleFor(d => d.Password).MinimumLength(8).MaximumLength(20);
        }
    }
}
