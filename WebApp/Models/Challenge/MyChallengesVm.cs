using System;
using System.Collections.Generic;
using Common.Environment;

namespace WebApp.Models.Challenge
{
    public class MyChallengesVm
    {
        public IEnumerable<ChallengeListItem> MyChallengeList { get; set; }
    
        public ChallengeDetails CurrentChallengeDetails { get; set; }

        public class ChallengeListItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class ChallengeDetails 
        {
            public string Id { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime DueDate { get; set; }
            public ChallengePrivacyLevel PrivacyLevel { get; set; }
            public string PrivacyDomain { get; set; }            
            public string Name { get; set; }
            public ProgramingLanguage ProgramingLanguage { get; set; }
            public string ProblemDefinition { get; set; }
            public IEnumerable<Solution> Solutions { get; set; }

            public class Solution
            {
                public string OwnerName { get; set; }
                public int TestCaseCount { get; set; }
                public int SuccessfulTestCount { get; set; }
                public DateTime CreationDate { get; set; }
            }
        }
    }

    //internal class ChallengeDetailsModelValidator : AbstractValidator<MyChallengesViewModel>
    //{
    //    public ChallengeDetailsModelValidator(Role role)
    //    {
    //        RuleFor(d => d.PrivacyLevel).Must(privacyLevel => (privacyLevel == ChallengePrivacyLevel.Public) || role == Role.PremiumUser);
    //        RuleFor(d => d.PrivacyDomain).Must(privacyDomain => (string.IsNullOrWhiteSpace(privacyDomain)) || role == Role.PremiumUser);
    //    }
    //}
}
