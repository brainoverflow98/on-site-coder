using Common.Environment;
using System;
using System.Collections.Generic;

namespace Common.DataBase.Entities
{
    public class Solution
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string OwnerDisplayName { get; set; }
        public string ChallengeId { get; set; }
        public DateTime CreationDate { get; set; }        
        public ProgramingLanguage ProgramingLanguage { get; set; }
        public IEnumerable<SolutionFile> Files { get; set; }
        public int TestCaseCount { get; set; }
        public int SuccessfulTestCount { get; set; }
    }

    public class SolutionFile
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

}
