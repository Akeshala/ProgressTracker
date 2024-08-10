using Microsoft.EntityFrameworkCore;
using ProgressTracker.Data;
using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class DailyRecordService : IDailyRecordService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DailyRecordService> _logger;
        private readonly ISessionService _sessionService;

        public DailyRecordService(ISessionService sessionService, ILogger<DailyRecordService> logger, AppDbContext context)
        {
            _sessionService = sessionService;
            _logger = logger;
            _context = context;
        }

        public async Task<List<DailyRecordModel>> GetAll()
        {
            try
            {
                var dailyRecords = await _context.DailyRecords.ToListAsync();
                return dailyRecords.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all daily records.");
                throw;
            }
        }

        public async Task<List<DailyRecordModel>> GetAllByUser(int userId)
        {
            return await _context.DailyRecords.Where(record => record.UserId == userId).ToListAsync();
        }

        public async Task<DailyRecordModel?> GetOneById(int id)
        {
            try
            {
                return await _context.DailyRecords.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving daily record with ID {id}.");
                throw;
            }
        }

        public async Task AddOne(DailyRecordModel? dailyRecord)
        {
            if (dailyRecord != null)
            {
                try
                {
                    var existingRecord = await GetRecordForDate(dailyRecord.Date);
                    if (existingRecord != null)
                    {
                        throw new Exception("Duplicate records found for that day! Edit the existing record.");
                    }

                    await _context.DailyRecords.AddAsync(dailyRecord);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding daily record.");
                    throw;
                }
            }
        }

        public Task SaveChanges()
        {
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<bool> RemoveOne(int id)
        {
            try
            {
                var dailyRecord = await _context.DailyRecords.FindAsync(id);
                if (dailyRecord == null)
                {
                    return false;
                }

                _context.DailyRecords.Remove(dailyRecord);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing daily record with ID {id}.");
                throw;
            }
        }

        public async Task<List<DailyRecordModel>> GetAllInRange(DateTime startDate, DateTime endDate)
        {
            return await _context.DailyRecords
                .Where(record => record.Date >= startDate && record.Date <= endDate)
                .ToListAsync();
        }

        public async Task<List<DailyRecordModel>> GetAllInRangeByUser(DateTime startDate, DateTime endDate, int userId)
        {
            return await _context.DailyRecords
                .Where(record => record.Date >= startDate && record.Date <= endDate && record.UserId == userId)
                .ToListAsync();
        }

        public async Task<TimeSpan> GetLearned(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord);
        }

        public async Task<TimeSpan> GetUntracked(DailyRecordModel dailyRecord)
        {
            var learnedTime = await CalculateTotalTime(dailyRecord);
            return dailyRecord.GetTarget() - learnedTime + dailyRecord.Break;
        }

        public async Task<TimeSpan> GetTracked(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord) + dailyRecord.Break;
        }

        public async Task<TimeSpan> GetRecorded(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord) + dailyRecord.Break;
        }

        private async Task<TimeSpan> CalculateTotalTime(DailyRecordModel dailyRecord)
        {
            try
            {
                var sessionIterator = await _sessionService.GetMultiByIds(dailyRecord.SessionIds);
                return sessionIterator.Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total session time.");
                throw;
            }
        }

        private async Task<DailyRecordModel?> GetRecordForDate(DateTime date)
        {
            return await _context.DailyRecords.FirstOrDefaultAsync(record => record.Date == date);
        }
    }
}
