using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchHub.Models
{
    public enum ResearchTopic
    {
        [Display(Name = "Computer science")] ComputerScience,
        [Display(Name = "Electrical Engineering")] ElectricalEngineering,
        [Display(Name = "Technology")] Technology,
        [Display(Name = "Chemistry")] Chemistry,
        [Display(Name = "Biology")] Biology,
        [Display(Name = "Math")] Math,
        [Display(Name = "Physics")] Physics,
        [Display(Name = "Mechanical Engineering")] MechanicalEngineering,
        [Display(Name = "Programming")] Programming,
        [Display(Name = "Mathemetical modeling")] MathModeling,
        [Display(Name = "Machine Learning")] MachineLearning,
        [Display(Name = "Robotics Engineering")] Robotics
    }

}
