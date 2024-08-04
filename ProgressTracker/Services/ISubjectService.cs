using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectViewModel>> GetAllForUser();
        Task<SubjectViewModel?> GetOneById(int id);
        Task<bool> RemoveOne(int id);
        Task<bool> AddOne(SubjectAddModel subjectViewModel);
        Task<List<SubjectViewModel>> GetAll();
    }
}