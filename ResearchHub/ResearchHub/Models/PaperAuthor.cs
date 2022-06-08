using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class PaperAuthor
    {
        public PaperAuthor() { }

        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        public int authorID { get; set; }
        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }

        public User User { get; set; }
        public ResearchPaper ResearchPaper { get; set; }

    }
}
