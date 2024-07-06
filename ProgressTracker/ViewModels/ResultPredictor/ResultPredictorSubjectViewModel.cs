using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.ResultPredictor;
public class ResultPredictorSubjectViewModel
{
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; }
    
    [Display(Name = "Subject ID")]
    public int SubjectId { get; set; }
    
    [Display(Name = "Study Time")]
    public TimeSpan Learned { get; set; }
    
    [Display(Name = "Required Study Time")]
    public TimeSpan Target { get; set; }
    
    public string Grade { get; set; }
}