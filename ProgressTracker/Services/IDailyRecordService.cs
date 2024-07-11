using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface IDailyRecordService
    {
        List<DailyRecordModel> GetAll();
        DailyRecordModel? GetOneById(int id);
        void RemoveOne(int id);
        void AddOne(DailyRecordModel dailyRecord);
        List<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate);
        
    }
}