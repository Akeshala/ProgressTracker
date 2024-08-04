using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProgressTracker.ViewModels.Login;
using ProgressTracker.ViewModels.Subject;
using Newtonsoft.Json.Linq;


namespace ProgressTracker.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ProgressTrackerUserService");
            _logger = logger;
        }

        public async Task<LoginViewModel?> GetOneById(int id)
        {
            var userResponse = await _httpClient.GetAsync($"user/view/1/{id}");
            if (userResponse.IsSuccessStatusCode)
            {
                return await userResponse.Content.ReadFromJsonAsync<LoginViewModel>();
            }

            _logger.LogError($"Error fetching user: {userResponse.ReasonPhrase}");
            return null;
        }

        public async Task<string?> LoginWithPassword(LoginViewModel viewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("user/login", viewModel);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(jsonResponse);
                string token = tokenData["token"].ToString();
                DateTime expiration = DateTime.Parse(tokenData["expiration"].ToString());

                _logger.LogInformation(expiration.ToString(CultureInfo.InvariantCulture), token);
                return token;
            }
            else
            {
                _logger.LogError($"Invalid username or/and password: {response.ReasonPhrase}");
                return null;
            }
        }
        
        
        public async Task<bool> Register(RegisterViewModel viewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("user/register", viewModel);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _logger.LogError($"Invalid details: {response.ReasonPhrase}");
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
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

        public string? GetUserIdFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);

            var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }

    }
}