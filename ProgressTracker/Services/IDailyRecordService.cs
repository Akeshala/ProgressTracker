using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface IDailyRecordService
    {
        IEnumerable<DailyRecordModel> GetAll();
        DailyRecordModel? GetOneById(int id);
        void RemoveOne(int id);
        void AddOne(DailyRecordModel dailyRecord);
        IEnumerable<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate);
        
    }
}