using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class ResearchPaper
    {
        public ResearchPaper() { }
        [Key]
        public int ID { get; set; }
        public string title { get; set; }
        public double rating { get; set; }
        public string paperAbstract { get; set; }
        public bool hasPdf { get; set; }

    }
}
