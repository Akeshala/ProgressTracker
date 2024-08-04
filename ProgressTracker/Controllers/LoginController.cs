using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.Login;

namespace ProgressTracker.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IUserService _userService;

    public LoginController(ILogger<LoginController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var token = await _userService.LoginWithPassword(model);
            if (token != null)
            {
                var userId = _userService.GetUserIdFromToken(token);
                if (userId != null)
                {
                    HttpContext.Response.Cookies.Append("userId", userId, new CookieOptions
                    {
                        HttpOnly = true,       // Prevents JavaScript access to the cookie
                        Secure = false,         // Ensures the cookie is sent only over HTTPS disabled
                        SameSite = SameSiteMode.Strict, // Helps to mitigate CSRF attacks
                        Expires = DateTime.UtcNow.AddMinutes(300) // Set the expiration time
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }
        return View("Index");
    }
    
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var status = await _userService.Register(model);
            if (status)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid registration attempt.");
            }
        }
        return View("Register");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}