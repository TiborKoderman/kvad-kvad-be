using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace kvad_be.Filters;

public sealed class AuthExceptionFilter : IExceptionFilter
{
  public void OnException(ExceptionContext context)
  {
    if (context.Exception is UnauthorizedAccessException)
    {
      context.Result = new UnauthorizedResult();
      context.ExceptionHandled = true;
    }
  }
}
