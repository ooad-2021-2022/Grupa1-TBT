using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class PaperType
    {
        public PaperType() { }

        [Key]
        public int id { get; set; }

        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }
        [EnumDataType(typeof(ResearchType))]
        public int type { get; set; }

        public ResearchPaper ResearchPaper { get; set; }
    }
}
