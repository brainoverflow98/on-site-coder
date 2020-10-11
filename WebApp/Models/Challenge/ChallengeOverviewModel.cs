using System;
using Common.Environment;

namespace WebApp.Models.Challenge
{
    public class ChallengeOverviewModel
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string OwnerDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Name { get; set; }
        public ProgramingLanguage ProgramingLanguage { get; set; }
        public int ParticipantCount { get; set; }
    }
}
