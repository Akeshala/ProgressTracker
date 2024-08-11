using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProgressTracker.ViewModels.Login;

namespace ProgressTracker.Services
{
    public interface IUserService
    {
        Task<LoginViewModel?> GetOneById(int id);
        Task<string?> LoginWithPassword(LoginViewModel loginViewModel);
        Task<bool> Register(RegisterViewModel viewModel);
        
        public static ClaimsPrincipal? GetPrincipalFromToken(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ProgressTrackerKey123456789123456789");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "yourdomain.com",
                ValidAudience = "yourdomain.com",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                // Token validation failed
                return null;
            }
        }
        
        public static string? GetUserIdFromToken(string? token)
        {
            var principal = GetPrincipalFromToken(token);

            var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }
}