using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class Ratings
    {
        public Ratings() { }
        [Key]
        public int id { get; set; }
        [ForeignKey("User")]
        public int giverID { get; set; }
        [ForeignKey("User")]
        public int receiverID { get; set; }
        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }
        public double rating { get; set; }

        public User giver { get; set; }
        public User receiver { get; set; }
    public ResearchPaper ResearchPaper { get; set; }
    }
}
