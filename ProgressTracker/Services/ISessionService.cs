using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface ISessionService
    {
        IEnumerable<SessionModel> GetAll();
        SessionModel? GetOneById(int id);
        void RemoveOne(int id);
        void AddOne(SessionModel sessionModel);
        IEnumerable<SessionModel> GetMultiByIds(IEnumerable<int> sessionIds);
    }
}