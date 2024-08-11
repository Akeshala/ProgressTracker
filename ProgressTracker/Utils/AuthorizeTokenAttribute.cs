namespace ProgressTracker.Utils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeTokenAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Request.Cookies["token"];
        var controllerName = context.RouteData.Values["controller"]?.ToString();

        // Exclude login, register, or other public actions from redirection
        if (controllerName == "Login")
        {
            // Allow access to the login page without checking the token
            return;
        }

        if (string.IsNullOrEmpty(token))
        {
            // Redirect to the login page if the token is missing
            context.Result = new RedirectToActionResult("Index", "Login", null);
        }

        base.OnActionExecuting(context);
    }
}
