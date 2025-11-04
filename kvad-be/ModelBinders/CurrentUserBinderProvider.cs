// ModelBinders/CurrentUserBinderProvider.cs
using kvad_be.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

public sealed class CurrentUserBinderProvider : IModelBinderProvider
{
  public IModelBinder? GetBinder(ModelBinderProviderContext ctx)
  {
    if (ctx.BindingInfo.BindingSource == BindingSource.Services &&
        ctx.Metadata.ModelType == typeof(User))
    {
      // MVC will resolve CurrentUserBinder via DI. No BuildServiceProvider().
      return new BinderTypeModelBinder(typeof(CurrentUserBinder));
    }
    return null;
  }
}
