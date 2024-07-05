namespace ProgressTracker.Models
{
    public class DailyRecordModel
    {
        private static readonly Dictionary<int, DailyRecordModel> _dailyRecords = new Dictionary<int, DailyRecordModel>();

        private static int _nextId = 1;
        private static readonly object _lock = new object();
        public static readonly int DailyTarget = 8;

        public int Id { get; private set; }
        public DateTime Date { get; set; }
        public TimeSpan Target { get; set; }
        public TimeSpan Break { get; set; }
        private readonly List<int> _sessionIds;
        
        public DailyRecordModel()
        {
            Date = DateTime.Now;
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            _sessionIds = [];
        }
        
        public DailyRecordModel(int year, int month, int day)
        {
            Id = GenerateId(); // handle when using database
            Date = new DateTime(year, month, day);
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            _sessionIds = [];
        }

        private static int GenerateId()
        {
            lock (_lock)
            {
                return _nextId++;
            }
        }

        public static DailyRecordModel? GetOneById(int id)
        {
            _dailyRecords.TryGetValue(id, out var session);
            return session;
        }

        public static DailyRecordModel[] GetAll()
        {
            return _dailyRecords.Values.ToArray();
        }

        public static Dictionary<int, DailyRecordModel> GetAllMapped()
        {
            return new Dictionary<int, DailyRecordModel>(_dailyRecords);
        }

        public static void AddOne(DailyRecordModel dailyRecord)
        {
            lock (_lock)
            {
                _dailyRecords[dailyRecord.Id] = dailyRecord;
            }
        }

        public static bool RemoveOne(int id)
        {
            lock (_lock)
            {
                return _dailyRecords.Remove(id);
            }
        }

        public List<int> SessionIds => _sessionIds;

        public void AddOneSessionId(int sessionId)
        {
            _sessionIds.Add(sessionId);
        }

        public DateTime GetDate()
        {
            return Date;
        }
        
        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public TimeSpan GetTarget()
        {
            return Target;
        }

        public TimeSpan GetBreak()
        {
            return Break;
        }
        
        public TimeSpan GetLearned()
        {
            return SessionModel.GetMultiByIds(SessionIds)
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time);
        }
        
        public TimeSpan GetRecorded()
        {
            return SessionModel.GetMultiByIds(SessionIds)
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + Break;
        }
        
        public void SetBreak(int breakHours, int breakMinutes)
        {
            Break = new TimeSpan(breakHours, breakMinutes, 0);
        }
        
        public static void PopulateDailyRecords()
        {
            var record1 = new DailyRecordModel(2024, 6, 1);
            record1.AddOneSessionId(1);
            record1.AddOneSessionId(2);
            record1.SetBreak(0, 45);
            AddOne(record1);

            var record2 = new DailyRecordModel(2024, 6, 2);
            record2.AddOneSessionId(3);
            record2.AddOneSessionId(4);
            record2.SetBreak(0, 30);
            AddOne(record2);

            var record3 = new DailyRecordModel(2024, 6, 3);
            record3.AddOneSessionId(5);
            record3.AddOneSessionId(6);
            record3.AddOneSessionId(7);
            record3.SetBreak(0, 10);
            AddOne(record3);
        }
    }
}