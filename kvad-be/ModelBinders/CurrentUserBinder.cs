using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace kvad_be.ModelBinders;

public sealed class CurrentUserBinder(AuthService auth) : IModelBinder
{
  public async Task BindModelAsync(ModelBindingContext ctx)
  {
    var user = await auth.GetUser(ctx.HttpContext.User, ctx.HttpContext.RequestAborted);
    ctx.Result = ModelBindingResult.Success(user); // throws -> your filter maps to 401
  }
}