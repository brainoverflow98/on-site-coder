using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public static class HttpContextExtensions
    {
        private static readonly RouteData EmptyRouteData = new RouteData();

        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();

        public static Task WriteActionResult<TResult>(this HttpContext context, TResult result) where TResult : IActionResult
        {
            var executor = context.RequestServices.GetService<IActionResultExecutor<TResult>>();

            if (executor == null)
            {
                throw new InvalidOperationException($"No action result executor for {typeof(TResult).FullName} registered.");
            }

            var routeData = context.GetRouteData() ?? EmptyRouteData;
            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            return executor.ExecuteAsync(actionContext, result);
        }

        public static Task WriteStatusCodeResult(this HttpContext context, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            return context.Response.Body.FlushAsync();
        }

        public static Task WriteStatusCodeResult(this HttpContext context, int statusCode, object value)
        {
            var result = new JsonResult(value)
            {
                StatusCode = statusCode
            };

            return context.WriteActionResult(result);
        }

        public static Task WriteViewResult(this HttpContext context, string viewDataKey, object errorModel, object model = null)
        {
            var data = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            data.Add(viewDataKey, errorModel);
            if (model != null)
                data.Model = model;
            var view = new ViewResult { ViewData = data };
            return context.WriteActionResult(view);
        }

        public static Task WriteErrorViewResult(this HttpContext context, string errorMessage)
        {
            var data = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            data.Model = errorMessage;
            var view = new ViewResult { ViewName = "CustomError", ViewData = data };
            return context.WriteActionResult(view);
        }


        public static bool AcceptsJsonResult(this HttpContext context)
        {
            string acceptHeader = context.Request.Headers["Accept"].ToString();
            return acceptHeader.Contains("application/json") || !acceptHeader.Contains("html");            
        }
    }
}
