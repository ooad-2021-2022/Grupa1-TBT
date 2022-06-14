using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class PublishedPapers
    {
        public PublishedPapers() { }
        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        public int userID { get; set; }
        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }
        public System.DateTime datePublished { get; set; }

        public User User { get; set; }
        public ResearchPaper ResearchPaper { get; set; }
      
    }
}
