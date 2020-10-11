using Common.Environment;
using System;
using System.Collections.Generic;

namespace Common.DataBase.Entities
{
    public class Challenge
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string OwnerDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public ChallengePrivacyLevel PrivacyLevel { get; set; }
        public string PrivacyDomain { get; set; }
        public bool AllowSolutionUpload { get; set; }
        public string Name { get; set; }
        public ProgramingLanguage ProgramingLanguage { get; set; }
        public string ProblemDefinition { get; set; }
        public IEnumerable<ChallengeFile> Files { get; set; }
        public int TestCaseCount { get; set; }
        public IEnumerable<TestCase> TestCases { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class ChallengeFile
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public FileType FileType { get; set; }
    }

    public class TestCase
    {
        public int No { get; set; }
        public string Arguments { get; set; }
        public bool IsHidden { get; set; }
        public string ExpectedOutput { get; set; }
    }
    
}


