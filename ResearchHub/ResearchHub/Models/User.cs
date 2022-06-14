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
        
        [ForeignKey("AspNetUsers")]
        public string aspNetID { get; set; }

        [StringLength(maximumLength:100, MinimumLength = 2)]
        [Display(Name ="First name")]
        public string firstName { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [Display(Name = "Address")]
        public string address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Date of birth")]
        public System.DateTime dateOfBirth { get; set; }
        public int numberOfDownloads { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }

    }
}