using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Utils
{
    public class RazorView
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorView(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToString<TModel>(string name, TModel model)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var httpContext = new DefaultHttpContext() { RequestServices = scope.ServiceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                var viewEnginResult = _razorViewEngine.FindView(actionContext, name, false);

                if (!viewEnginResult.Success)
                {
                    throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
                }

                var view = viewEnginResult.View;

                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(actionContext, view, new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    { Model = model, },
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), output, new HtmlHelperOptions());

                    await view.RenderAsync(viewContext);

                    return output.ToString();
                }

            }
        }
    }
}
