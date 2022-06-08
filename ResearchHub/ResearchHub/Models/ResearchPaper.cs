namespace ResearchHub.Models
{
    public class ResearchPaper
    {
        public int ID { get; set; }
        public string title { get; set; }
        public double rating { get; set; }
        public string paperAbstract { get; set; }
        public bool hasPdf { get; set; }

    }
}
