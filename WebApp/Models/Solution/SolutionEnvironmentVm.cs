using System;
using System.Collections.Generic;
using Common.DataBase.Entities;
using Common.Environment;

namespace WebApp.Models.Solution
{
    public class SolutionEnvironmentVm
    {
        public string SolutionId { get; set; }
        public string ChallengeId { get; set; }
        public DateTime CreationDate { get; set; }
        public ProgramingLanguage ProgramingLanguage { get; set; }
        public IEnumerable<string> FileNames { get; set; }
        public IEnumerable<string> InputFileNames { get; set; }
        public int TestCaseCount { get; set; }
        public int SuccessfulTestCount { get; set; }
        public DateTime DueDate { get; set; }
        public string ProblemDefinition { get; set; }
        public IEnumerable<TestCase> TestCases { get; set; }
    }
}
