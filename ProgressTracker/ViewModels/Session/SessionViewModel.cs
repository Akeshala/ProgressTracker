using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.Session;
public class SessionViewModel
{
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; }
    public TimeSpan Time { get; set; }
    public int Id { get; set; }
}