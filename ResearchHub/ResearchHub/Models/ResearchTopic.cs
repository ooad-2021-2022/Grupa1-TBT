using System.ComponentModel.DataAnnotations;

namespace ResearchHub.Models
{
    public enum ResearchTopic
    {
        [Display(Name = "Computer science")] ComputerScience,
        [Display(Name = "Engineering")] Engineering,
        [Display(Name = "Technology")] Technology,
        [Display(Name = "Chemistry")] Chemistry,
        [Display(Name = "Biology")] Biology,
        [Display(Name = "Math")] Math,
        [Display(Name = "Physics")] Physics,
    }
}
