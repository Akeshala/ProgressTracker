using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProgressTracker.ViewModels.Session;
public class SessionViewModel
{
    public int SubjectSelectedValue { get; set; }
    public List<SelectListItem> SubjectOptions { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int DailyRecordId { get; set; }
}