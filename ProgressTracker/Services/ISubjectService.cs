using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectViewModel>> GetAllForUser(int userId);
        Task<SubjectViewModel?> GetOneById(int id);
        Task<bool> RemoveOne(int id, int userId);
        Task<bool> AddOne(SubjectAddModel subjectViewModel);
        Task<List<SubjectViewModel>> GetAll();
    }
}