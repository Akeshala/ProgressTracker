using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.Models;

public class SubjectModel
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Name { get; set; }

    [Range(1, 10)]
    public int Credits { get; set; }
    
    [Display(Name = "Learning Hours")]
    [Range(1, 100)]
    public int LearningHours { get; set; }
    
    public SubjectModel(string name, int credits, int learningHours)
    {
        Name = name;
        Credits = credits;
        LearningHours = learningHours;
    }

    public string GetName()
    {
        return Name;
    }
}