using Common.Environment;

namespace WebApp.Models.Challenge
{
    public class SearchForm
    {
        public string SearchText { get; set; }
        public ProgramingLanguage? ProgramingLanguage { get; set; }
        public int PerPage { get; set; } = 25;
        public int PageNo { get; set; } = 1;
        public SortField SortBy { get; set; }
    }

    public enum SortField
    {        
        CreationDate,
        ParticipantCount
    }
}
