namespace ProgressTracker.Models
{
    public class DailyRecordModel
    {
        private static readonly Dictionary<int, DailyRecordModel> _dailyRecords = new Dictionary<int, DailyRecordModel>();

        private static int _nextId = 0;
        private static readonly object _lock = new object();
        public static readonly int DailyTarget = 8;

        public int Id { get; private set; }
        public DateTime Date;
        public TimeSpan Target;
        public TimeSpan Break { get; set; }
        private readonly List<int> _sessionIds;

        public DailyRecordModel(int year, int month, int day)
        {
            Id = GenerateId();
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

        public TimeSpan GetTarget()
        {
            return Target;
        }

        public TimeSpan GetBreak()
        {
            return Break;
        }
    }
}