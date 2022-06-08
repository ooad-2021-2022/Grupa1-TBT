namespace ResearchHub.Models
{
    public class Questions
    {
        public int ID { get; set; }
        public int quizID { get; set; }
        public string question { get; set; }
        public bool answer { get; set; }

    }
}
