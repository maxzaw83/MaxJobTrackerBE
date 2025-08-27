namespace MaxJobTracker.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string JobPosition { get; set; }
        public string Company { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } // e.g., Bookmarked, Applied, Interviewing
        public DateTime DateSaved { get; set; }
        public DateTime? DateApplied { get; set; }
        public string? JobUrl { get; set; }
        public string? JobDescription { get; set; }
        public string? JobSource { get; set; } // e.g., LinkedIn, Seek, etc.


        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
