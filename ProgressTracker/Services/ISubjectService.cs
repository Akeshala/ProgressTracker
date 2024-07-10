using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface ISubjectService
    {
        IEnumerable<SubjectModel> GetAll();
        SubjectModel? GetOneById(int id);
        void RemoveOne(int id);
        void AddOne(SubjectModel subjectModel);
    }
}