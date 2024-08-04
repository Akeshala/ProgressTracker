using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.Subject;

public class SubjectViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int Credits { get; set; }

    [Display(Name = "Learning Hours")]
    public int LearningHours { get; set; }

    public SubjectViewModel(string name, int credits, int learningHours)
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