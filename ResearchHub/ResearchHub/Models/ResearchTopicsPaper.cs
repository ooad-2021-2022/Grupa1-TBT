using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class ResearchTopicsPaper
    {
        public ResearchTopicsPaper() { }
        [Key]
        public int id { get; set; }
        [ForeignKey("ResearchPaper")]
        public int paperID { get; set; }
        [EnumDataType(typeof(ResearchTopic))]
        public int topic { get; set; }

        public ResearchPaper ResearchPaper { get; set; }
    }
}
