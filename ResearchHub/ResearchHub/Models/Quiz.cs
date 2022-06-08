using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchHub.Models
{
    public class Quiz
    {
        public Quiz() { }
        [Key]
        public int ID { get; set; }
        public int numberOfCorrectAnswers { get; set; }
        public double minimumScoreNeeded { get; set; }
        public double pointsPerQuestion { get; set; }
        [ForeignKey("ResearchPaper")]
        public int researchPaperID { get; set; }
        public ResearchPaper ResearchPaper { get; set; }

    }
}
