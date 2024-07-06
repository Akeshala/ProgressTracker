using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class SessionService : ISessionService
    {
        private static readonly Dictionary<int, SessionModel> _sessions = new Dictionary<int, SessionModel>();
        private static int _nextId = 0;
        private static readonly object _lock = new object();
        private static bool _initialized = false;

        public SessionService()
        {
            InitializeSubjects();
        }

        private void InitializeSubjects()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                AddOne(new SessionModel(7, 1, 30));
                AddOne(new SessionModel(1, 2, 45));
                AddOne(new SessionModel(2, 3, 15));
                AddOne(new SessionModel(3, 4, 0));
                AddOne(new SessionModel(4, 3, 30));
                AddOne(new SessionModel(5, 2, 45));
                AddOne(new SessionModel(6, 3, 0));
                AddOne(new SessionModel(1, 3, 30));
                AddOne(new SessionModel(2, 4, 45));
                AddOne(new SessionModel(3, 1, 15));
                AddOne(new SessionModel(4, 0, 0));
                AddOne(new SessionModel(5, 1, 30));
                AddOne(new SessionModel(6, 0, 45));
                AddOne(new SessionModel(7, 1, 0));
                AddOne(new SessionModel(1, 2, 30));
                AddOne(new SessionModel(2, 0, 45));
                AddOne(new SessionModel(3, 1, 15));
                AddOne(new SessionModel(4, 1, 0));
                AddOne(new SessionModel(5, 1, 30));
                AddOne(new SessionModel(6, 2, 45));
                AddOne(new SessionModel(7, 1, 0));
                AddOne(new SessionModel(1, 3, 30));
                AddOne(new SessionModel(2, 2, 45));
                AddOne(new SessionModel(3, 3, 15));
                AddOne(new SessionModel(4, 1, 0)); //25

                _initialized = true;
            }
        }

        public IEnumerable<SessionModel?> GetAll()
        {
            return _sessions.Values.ToArray();
        }

        public SessionModel? GetOneById(int id)
        {
            if (_sessions.TryGetValue(id, out SessionModel session))
            {
                return session;
            }
            return null;
        }
        
        public IEnumerable<SessionModel> GetMultiByIds(IEnumerable<int> sessionIds)
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

        public void AddOne(SessionModel? session)
        {
            if (session != null)
            {
                if (session.Id == 0)
                {
                    session.Id = GenerateUniqueId();
                    _sessions[session.Id] = session;
                }
                else
                {
                    _sessions[session.Id] = session;
                }
            }
        }

        public void RemoveOne(int id)
        {
            _sessions.Remove(id);
        }

        private int GenerateUniqueId()
        {
            return ++_nextId;
        }
    }
}
