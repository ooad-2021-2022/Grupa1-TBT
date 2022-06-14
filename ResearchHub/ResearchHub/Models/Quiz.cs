using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchHub.Models
{
    public class Quiz
    {
        public Quiz() { }
        [Key]
        public int ID { get; set; }

        [Display(Name = "Number of correct answers:")]
        public int numberOfCorrectAnswers { get; set; }

        [Range(0.00, 100.00)]
        [Display(Name = "Minimum score needed (in percentages - %)")]
        public double minimumScoreNeeded { get; set; }
        
        [Range(0.00, double.MaxValue)]
        [Display(Name = "Points per question")]
        public double pointsPerQuestion { get; set; }
        [ForeignKey("ResearchPaper")]
        public int researchPaperID { get; set; }
        public ResearchPaper ResearchPaper { get; set; }

    }
}
