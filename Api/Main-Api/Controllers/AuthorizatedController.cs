using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Main_Api.Controllers;

public class AuthorizatedController(
    ILogger<AuthorizatedController> logger) : Controller
{
    private ILogger<AuthorizatedController> _logger;

    protected ISession Session => HttpContext.Session;

    protected string? UserId => Session.GetString("user_id");

    protected string? UserRoles => Session.GetString("roles");

    protected bool IsAuthenticated => Session.GetInt32("auth") == 1;

    protected void SetSession(string userId, string roles)
    {
        Session.SetInt32("auth", 1);
        Session.SetString("user_id", userId);
        Session.SetString("roles", roles);
    }

    protected void ClearSession()
    {
        Session.Remove("auth");
        Session.Remove("user_id");
        Session.Remove("roles");
    }

    protected IActionResult RedirectToHome()
    {
        return RedirectToAction("Index", "Home");
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        var isAdmin = UserRoles?.Split(',').Contains("Admin") == true;

        ViewData["isAdmin"] = isAdmin;
    }

    private void SetNoCacheHeaders()
    {
        // Prevent caching in browsers and proxies
        Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
        Response.Headers.Add("Pragma", "no-cache");
        Response.Headers.Add("Expires", "0");
    }

    protected void AuthorizatedGuard(string requiredRoles = "")
    {
        logger.LogInformation("AuthorizatedGuard called");
        if (!IsAuthenticated)
        {
            SetNoCacheHeaders();
            logger.LogWarning("User is not authenticated, redirecting to login");
            Response.Redirect("/Home/Login", true);
            return;
        }

        if (!string.IsNullOrEmpty(requiredRoles) && !UserRoles?.Split(',').Any(requiredRoles.Contains) == true)
        {
            SetNoCacheHeaders();
            logger.LogWarning("User does not have required roles, redirecting to login");
            Response.Redirect("/Home/Login", true);
            return;
        }
        logger.LogInformation("User is authenticated and authorized");

    }
}