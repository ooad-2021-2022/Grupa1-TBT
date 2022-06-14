using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class Downloads
    {
        public Downloads() { }

        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        public int downloaderID { get; set; }
        [ForeignKey("User")]
        public int downloadeeID { get; set; }
        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }

        public User downloader { get; set; }
        public User downloadee { get; set; }
        public ResearchPaper ResearchPaper { get; set; }
    }
}
