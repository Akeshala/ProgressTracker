namespace ProgressTracker.Models
{
    public class SessionModel
    {
        private static readonly Dictionary<int, SessionModel> _sessions = new Dictionary<int, SessionModel>();
        private static int _nextId = 0;
        private static readonly object _lock = new object();
        
        public int SubjectId { get; set; }
        public int Id { get; private set; }
        public TimeSpan Time;

        public SessionModel(int subjectId, int hours, int minutes)
        {
            Id = GenerateId();
            SubjectId = subjectId;
            SetTime(hours, minutes);
        }
        
        private static int GenerateId()
        {
            lock (_lock)
            {
                return _nextId++;
            }
        }

        public void SetTime(int hours, int minutes)
        {
            Time = new TimeSpan(hours, minutes, 0);
        }
        
        public static SessionModel? GetOneById(int id)
        {
            _sessions.TryGetValue(id, out var session);
            return session;
        }
        
        public static List<SessionModel> GetMultiByIds(List<int> sessionIds)
        {
            var result = new List<SessionModel>();
            foreach (var sessionId in sessionIds)
            {
                _sessions.TryGetValue(sessionId, out var session);
                if (session != null)
                {
                    result.Add(session);
                }
            }
            return result;
        }

        public static List<SessionModel> GetAll()
        {
            return [.._sessions.Values];
        }
        
        public static Dictionary<int, SessionModel> GetAllMapped()
        {
            return new Dictionary<int, SessionModel>(_sessions);
        }
        
        public static void AddOne(SessionModel sessionModel)
        {
            lock (_lock)
            {
                _sessions[sessionModel.Id] = sessionModel;
            }
        }
        
        public static bool RemoveOne(int id)
        {
            lock (_lock)
            {
                return _sessions.Remove(id);
            }
        }

        public TimeSpan GetTime()
        {
            return Time;
        }
    }
}