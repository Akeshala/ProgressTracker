using ProgressTracker.Services;

namespace ProgressTracker.Middleware;

public class UserIdMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check for the token in the request cookies
        if (context.Request.Cookies.TryGetValue("token", out string? token))
        {
            try
            {
                // Extract userId from token
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
            }
            catch (Exception ex)
            {
                // Handle or log any errors that occur during token parsing
                // e.g., _logger.LogError(ex, "Error processing token.");
            }
        }

        // Continue to the next middleware
        await _next(context);
    }
}
