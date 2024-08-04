using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionModel>> GetAll();
        Task<SessionModel?> GetOneById(int id);
        Task<bool> RemoveOne(int id);
        void AddOne(SessionModel sessionModel);
        Task<IEnumerable<SessionModel>> GetMultiByIds(IEnumerable<int> sessionIds);
    }
}