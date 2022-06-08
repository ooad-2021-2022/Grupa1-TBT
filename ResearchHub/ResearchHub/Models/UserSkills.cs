using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class UserSkills
    {
        public UserSkills() { }
        [Key]
        public int id { get; set; }
        [ForeignKey("AspNetUsers")]
        public int userID { get; set; }
        [EnumDataType(typeof(ResearchTopic))]
        public int skill { get; set; }

        public User User { get; set; }
    }
}
