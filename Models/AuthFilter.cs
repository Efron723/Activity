using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class AuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        if (controllerName == "Home" && actionName == "Login")
        {
            return;
        }

        var isLoginString = context.HttpContext.Session.GetString("IsLogin");

        if (string.IsNullOrWhiteSpace(isLoginString))
        {
            context.Result = new RedirectToActionResult("Login", "Home", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
