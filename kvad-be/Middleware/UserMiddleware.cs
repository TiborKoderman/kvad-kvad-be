using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class UserMiddleware
{
    private readonly RequestDelegate _next;

    public UserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        var userId = context.User.Identity?.Name;

        if (userId != null)
        {
            context.Items["User"] = await authService.GetUserById(userId);
        }
        else{
            context.Items["User"] = null;
        }
        
        await _next(context);
    }
}