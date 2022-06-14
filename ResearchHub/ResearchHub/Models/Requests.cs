using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchHub.Models
{
    public class Requests
    {
        public Requests() { }

        [Key]
        public int id { get; set; }
        [Required]
        [ForeignKey("User")]
        public int? requesterID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int? requesteeID { get; set; }
        public System.DateTime timeRequestMade { get; set; }
    
        [Required]
        public string requestBody { get; set; }
        //requestBody - depends on what are we simulating - coffees or other kind of requests 
    }
}
