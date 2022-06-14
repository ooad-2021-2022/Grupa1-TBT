using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class Questions
    {
        public Questions() { }
        [Key]
        public int ID { get; set; }


        [ForeignKey("Quiz")]
        public int quizID { get; set; }

        [Display(Name = "Body of the question")]        
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "String mora biti između 5 i 30 znakova dužine")]
        public string question { get; set; }

        [Display(Name = "Is the answer positive?")]
        public bool answer { get; set; }

        public Quiz Quiz { get; set; }
    }
}
