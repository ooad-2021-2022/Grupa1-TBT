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
        public string question { get; set; }
        public bool answer { get; set; }

        public Quiz Quiz { get; set; }
    }
}
