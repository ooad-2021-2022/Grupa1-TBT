namespace ResearchHub.Models
{
    public class Quiz
    {
        public int ID { get; set; }
        public int numberOfCorrectAnswers { get; set; }
        public double minimumScoreNeeded { get; set; }
        public double pointsPerQuestion { get; set; }
        public int researchPaperID { get; set; }

    }
}
