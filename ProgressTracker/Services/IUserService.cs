using System.Security.Claims;
using ProgressTracker.ViewModels.Login;

namespace ProgressTracker.Services
{
    public interface IUserService
    {
        Task<LoginViewModel?> GetOneById(int id);
        Task<string?> LoginWithPassword(LoginViewModel loginViewModel);
        string? GetUserIdFromToken(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        Task<bool> Register(RegisterViewModel viewModel);
    }
}