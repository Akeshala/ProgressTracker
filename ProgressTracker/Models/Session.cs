namespace ProgressTracker.Models;

public class Session
{
    private static Dictionary<int, Session> _sessions = new Dictionary<int, Session>{};
    
    public int SubjectId;
    public int Id { get; set; }

    public TimeSpan Time;

    public Session(int subjectId, int hours, int minutes)
    {
        this.Id = _sessions.Values.ToArray().Length;
        this.SubjectId = subjectId;
        SetTime(hours, minutes);
    }

    public int GetSubjectID()
    {
        return SubjectId;
    }

    public TimeSpan GetTime()
    {
        return Time;
    }
    
    public void SetTime(int hours, int minutes)
    {
        Time = new TimeSpan(hours, minutes, 0);
    }
    
    public static Session? GetOneByID(int id)
    {
        _sessions.TryGetValue(id, out Session? session);
        return session;
    }

    public static Session[] GetAll()
    {
        return _sessions.Values.ToArray();
    }
    
    public static Dictionary<int, Session> GetAllMapped()
    {
        return _sessions;
    }
    
    public static void AddOne(Session session)
    {
        _sessions[session.Id] = session;
    }
    
    public static bool RemoveOne(int id)
    {
        return _sessions.Remove(id);
    }
}