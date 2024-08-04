using System.Runtime.InteropServices.JavaScript;
using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class DailyRecordService : IDailyRecordService
    {
        private static readonly Dictionary<int, DailyRecordModel>
            DailyRecords = new Dictionary<int, DailyRecordModel>();

        private static int _nextId = 0;
        private static readonly object Lock = new object();
        private static bool _initialized = false;

        private readonly ISessionService _sessionService;

        public DailyRecordService(ISessionService sessionService)
        {
            _sessionService = sessionService;
            InitializeDailyRecords();
        }

        private void InitializeDailyRecords()
        {
            if (_initialized) return;

            lock (Lock)
            {
                if (_initialized) return;

                var record1 = new DailyRecordModel(2024, 7, 1);
                record1.AddOneSessionId(1);
                record1.AddOneSessionId(2);
                record1.SetBreak(0, 45);
                AddOne(record1);

                var record2 = new DailyRecordModel(2024, 7, 2);
                record2.AddOneSessionId(3);
                record2.AddOneSessionId(4);
                record2.SetBreak(0, 30);
                AddOne(record2);

                var record3 = new DailyRecordModel(2024, 7, 3);
                record3.AddOneSessionId(5);
                record3.AddOneSessionId(6);
                record3.AddOneSessionId(7);
                record3.SetBreak(0, 10);
                AddOne(record3);

                var record4 = new DailyRecordModel(2024, 7, 4);
                record4.AddOneSessionId(8);
                record4.AddOneSessionId(9);
                record4.SetBreak(0, 20);
                AddOne(record4);

                var record5 = new DailyRecordModel(2024, 7, 5);
                record5.AddOneSessionId(10);
                record5.AddOneSessionId(11);
                record5.SetBreak(0, 15);
                AddOne(record5);

                var record6 = new DailyRecordModel(2024, 7, 6);
                record6.AddOneSessionId(12);
                record6.AddOneSessionId(13);
                record6.SetBreak(0, 25);
                AddOne(record6);

                var record7 = new DailyRecordModel(2024, 7, 7);
                record7.AddOneSessionId(14);
                record7.AddOneSessionId(15);
                record7.SetBreak(0, 27);
                AddOne(record7);

                var record8 = new DailyRecordModel(2024, 7, 8);
                record8.AddOneSessionId(14);
                record8.AddOneSessionId(15);
                record8.SetBreak(0, 29);
                AddOne(record8);

                var record9 = new DailyRecordModel(2024, 7, 9);
                record9.AddOneSessionId(14);
                record9.AddOneSessionId(15);
                record9.SetBreak(0, 23);
                AddOne(record9);

                var record10 = new DailyRecordModel(2024, 7, 10);
                record10.AddOneSessionId(16);
                record10.AddOneSessionId(17);
                record10.AddOneSessionId(18);
                record10.SetBreak(0, 20);
                AddOne(record10);

                var record11 = new DailyRecordModel(2024, 7, 11);
                record11.AddOneSessionId(19);
                record11.AddOneSessionId(20);
                record11.AddOneSessionId(21);
                record11.AddOneSessionId(22);
                record11.SetBreak(0, 30);
                AddOne(record11);

                var record12 = new DailyRecordModel(2024, 7, 12);
                record12.AddOneSessionId(23);
                record12.AddOneSessionId(24);
                record12.AddOneSessionId(25);
                record12.SetBreak(0, 10);
                AddOne(record12);

                _initialized = true;
            }
        }

        public List<DailyRecordModel> GetAll()
        {
            return DailyRecords.Values.ToList();
        }

        public DailyRecordModel? GetOneById(int id)
        {
            return DailyRecords.GetValueOrDefault(id);
        }

        public void AddOne(DailyRecordModel? dailyRecord)
        {
            if (dailyRecord != null)
            {
                var existingRecord = GetRecordForDate(dailyRecord.Date);
                if (existingRecord != null)
                {
                    throw new Exception("Duplicate Records found for that day! Edit the existing record.");
                }

                if (dailyRecord.Id == 0)
                {
                    dailyRecord.Id = GenerateUniqueId();
                    DailyRecords[dailyRecord.Id] = dailyRecord;
                }
                else
                {
                    DailyRecords[dailyRecord.Id] = dailyRecord;
                }
            }
        }

        private int GenerateUniqueId()
        {
            lock (Lock)
            {
                return ++_nextId;
            }
        }

        public void RemoveOne(int id)
        {
            lock (Lock)
            {
                DailyRecords.Remove(id);
            }
        }

        public List<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate)
        {
            var dailyRecords = DailyRecords.Values
                .Where(record => record?.Date >= startDate && record?.Date <= endDate)
                .ToArray();
            return dailyRecords.ToList();
        }

        public async Task<TimeSpan> GetLearned(DailyRecordModel dailyRecord)
        {
            var sessionIds = dailyRecord.SessionIds;
            var sessionIterator = await _sessionService.GetMultiByIds(sessionIds);
            return sessionIterator.Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time);
        }

        public async Task<TimeSpan> GetUntracked(DailyRecordModel dailyRecord)
        {
            var sessionIds = dailyRecord.SessionIds;
            var breakTime = dailyRecord.Break;
            var target = dailyRecord.GetTarget();
            var sessionIterator = await _sessionService.GetMultiByIds(sessionIds);
            return target - sessionIterator
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + breakTime;
        }

        public async Task<TimeSpan> GetTracked(DailyRecordModel dailyRecord)
        {
            var sessionIds = dailyRecord.SessionIds;
            var breakTime = dailyRecord.Break;
            var sessionIterator = await _sessionService.GetMultiByIds(sessionIds);
            return sessionIterator
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + breakTime;
        }

        public async Task<TimeSpan> GetRecorded(DailyRecordModel dailyRecord)
        {
            var sessionIds = dailyRecord.SessionIds;
            var breakTime = dailyRecord.Break;
            var sessionIterator = await _sessionService.GetMultiByIds(sessionIds);
            return sessionIterator
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + breakTime;
        }

        private DailyRecordModel? GetRecordForDate(DateTime date)
        {
            var dailyRecord = DailyRecords.Values
                .FirstOrDefault(record => record?.Date == date);
            return dailyRecord;
        }
    }
}