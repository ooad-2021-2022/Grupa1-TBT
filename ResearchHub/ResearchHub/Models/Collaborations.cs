using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchHub.Models
{
    public class Collaborations
    {
        public Collaborations() { }

        [Key]
        public int id { get; set; }
        [Required]
        [ForeignKey("User")]
        public int? collaboratorID { get; set; }
        [Required]
        [ForeignKey("User")]
        public int? collaborateeID { get; set; }
        public System.DateTime timeRequestMade { get; set; }

        public User collaborator{ get; set; }
        public User collaboratee { get; set; }

    }
}
