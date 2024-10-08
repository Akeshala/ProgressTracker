using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.WeeklyReport;
public class WeeklySubjectReportViewModel
{
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; }
    
    [Display(Name = "Subject ID")]
    public int SubjectId { get; set; }
    
    [Display(Name = "Study Time")]
    public TimeSpan Learned { get; set; }
}