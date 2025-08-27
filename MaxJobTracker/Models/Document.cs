namespace MaxJobTracker.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; } // Path on the server where the file is stored
        public string ContentType { get; set; }
        public int JobApplicationId { get; set; }
        public JobApplication JobApplication { get; set; }
    }

}
