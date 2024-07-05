using ProgressTracker.Models;

namespace ProgressTracker.ViewModels.DailyRecord;
public class DailyRecordViewModel
{    
    public required DailyRecordModel DailyRecordModel { get; set; }
    public TimeSpan Learned { get; set; }
}