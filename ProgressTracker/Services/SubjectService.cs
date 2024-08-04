using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(IHttpClientFactory httpClientFactory, ILogger<SubjectService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ProgressTrackerSubjectService");
            _logger = logger;
        }

        public async Task<List<SubjectViewModel>> GetAll()
        {
            var subjectResponse = await _httpClient.GetAsync("subject/all");
            if (subjectResponse.IsSuccessStatusCode)
            {
                return await subjectResponse.Content.ReadFromJsonAsync<List<SubjectViewModel>>() ??
                       new List<SubjectViewModel>();
            }

            _logger.LogError($"Error fetching subjects: {subjectResponse.ReasonPhrase}");
            return new List<SubjectViewModel>();
        }

        public async Task<List<SubjectViewModel>> GetAllForUser()
        {
            var subjectResponse = await _httpClient.GetAsync("subject/all/1");
            if (subjectResponse.IsSuccessStatusCode)
            {
                return await subjectResponse.Content.ReadFromJsonAsync<List<SubjectViewModel>>() ??
                       new List<SubjectViewModel>();
            }

            _logger.LogError($"Error fetching subjects: {subjectResponse.ReasonPhrase}");
            return new List<SubjectViewModel>();
        }

        public async Task<SubjectViewModel?> GetOneById(int id)
        {
            var subjectResponse = await _httpClient.GetAsync($"subject/view/1/{id}");
            if (subjectResponse.IsSuccessStatusCode)
            {
                return await subjectResponse.Content.ReadFromJsonAsync<SubjectViewModel>();
            }

            _logger.LogError($"Error fetching subject: {subjectResponse.ReasonPhrase}");
            return null;
        }

        public async Task<bool> AddOne(SubjectAddModel subjectModel)
        {
            subjectModel.UserId = 1;
            var response = await _httpClient.PostAsJsonAsync("subject/add", subjectModel);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _logger.LogError($"Error adding subject: {response.ReasonPhrase}");
                return false;
            }
        }

        public async Task<bool> RemoveOne(int id)
        {
            var response = await _httpClient.DeleteAsync($"subject/remove/{id}/1");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _logger.LogError($"Error deleting subject with ID {id}: {response.ReasonPhrase}");
                return false;
            }
        }
    }
}