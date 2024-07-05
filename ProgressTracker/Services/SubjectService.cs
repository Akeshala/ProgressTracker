using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class SubjectService : ISubjectService
    {
        private static Dictionary<int, SubjectModel> _subjects = new Dictionary<int, SubjectModel>();

        private static int _currentId = 0;
        private static bool _initialized = false;
        private static readonly object _initLock = new object();

        public SubjectService()
        {
            InitializeSubjects();
        }

        private void InitializeSubjects()
        {
            if (_initialized) return;

            lock (_initLock)
            {
                if (_initialized) return;

                AddOne(new SubjectModel("Mathematics", 5, 100));
                AddOne(new SubjectModel("Physics", 4, 75));
                AddOne(new SubjectModel("Chemistry", 3, 75));
                AddOne(new SubjectModel("Biology", 4, 120));
                AddOne(new SubjectModel("History", 2, 50));
                AddOne(new SubjectModel("Geography", 3, 50));
                AddOne(new SubjectModel("Computer Science", 6, 90));

                _initialized = true;
            }
        }

        public IEnumerable<SubjectModel?> GetAll()
        {
            return _subjects.Values.ToArray();
        }

        public SubjectModel? GetOneById(int id)
        {
            if (_subjects.TryGetValue(id, out SubjectModel subject))
            {
                return subject;
            }
            return null;
        }

        public void AddOne(SubjectModel? subjectModel)
        {
            if (subjectModel != null)
            {
                if (subjectModel.Id == 0)
                {
                    subjectModel.Id = GenerateUniqueId();
                    _subjects[subjectModel.Id] = subjectModel;
                }
                else
                {
                    _subjects[subjectModel.Id] = subjectModel;
                }
            }
        }

        public void RemoveOne(int id)
        {
            _subjects.Remove(id);
        }

        private int GenerateUniqueId()
        {
            return ++_currentId;
        }
    }
}
