using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace ResearchHub.Models
{
    public class ResearchPaper
    {
        public ResearchPaper() { }
        [Key]
        public int ID { get; set; }

        public int numberOfRatings { get; set; }

        [StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "Naslov mora biti između 5 i 50 znakova dužine")]
        [MinLength(5)]
        [Display(Name = "Research paper title")]
        public string title { get; set; }

        [Display(Name = "Research paper rating")]
        
        [Range(0,10)]
        public double rating { get; set; }
        [Display(Name = "Abstract")]

        [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "Abstract mora biti iumeđu 10 i 500 znakova dužine")]
        public string paperAbstract { get; set; }
        public bool hasPdf { get; set; }

        public string pdfFileUrl { get; set; }

        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile pdfFile{ get; set; }

       
    }
}
