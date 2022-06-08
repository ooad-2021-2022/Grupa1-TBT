using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResearchHub.Models
{
    public class User
    {
        public User() { }

        [Required]
        [Key]
        public int id { get; set; }
        //[ForeignKey("AspNetUsers")]
        public int AspNetUserID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public System.DateTime dateOfBirth { get; set; }
        public string profileImageUrl { get; set; }
        public int numberOfDownloads { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }

    }
}