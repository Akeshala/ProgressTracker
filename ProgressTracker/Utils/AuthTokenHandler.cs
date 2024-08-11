using ProgressTracker.Services;

namespace ProgressTracker.Utils;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Cookies.TryGetValue("token", out string? token))
        {
            var userId = UserService.GetUserIdFromToken(token);
            
            // Set userId as a cookie
            if (!string.IsNullOrEmpty(userId))
            {
                context.Response.Cookies.Append("userId", userId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Use true in production for HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(300) // Set appropriate expiration
                });
            }
            
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
