using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Gcpe.Hub.WebApp
{
    public class ApiHttpException : Exception
    {
        public ApiHttpException(HttpStatusCode statusCode, Exception innerException = null) : base(null, innerException)
        {
            StatusCode = (int)statusCode;
        }

        public int StatusCode { get; private set; }
    }
}

namespace Gcpe.Hub.WebApp.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ApiHttpException ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = ex.StatusCode;

                // Use an InnerException first.
                string errMsg = (ex.InnerException ?? ex).Message;

                // ApiHttpException takes an Exception on the constructor for the InnerException.
                var result = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    message = errMsg
                });

                await response.WriteAsync(result);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiErrorMiddleware>();
        }
    }
}
