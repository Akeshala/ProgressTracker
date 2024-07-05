using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.Subject;
public class SubjectViewModel
{
    public string Name { get; set; }
    public int Credits { get; set; }
    
    [Display(Name = "Learning Hours")]
    public int LearningHours { get; set; }
}