using System.ComponentModel.DataAnnotations;

namespace ResearchHub.Models
{
    public enum ResearchType
    {
        [Display(Name = "Doctorate")] Doctorate,
        [Display(Name = "Masters")] Masters,
        [Display(Name = "Journal")] Journal,
        [Display(Name = "Book")] Book,
        [Display(Name = "Article")] Article,
        [Display(Name = "Project")] Project
    }
}
