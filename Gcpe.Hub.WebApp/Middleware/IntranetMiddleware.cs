using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Gcpe.Hub.WebApp.Middleware
{
    public class IntranetMiddleware
    {
        private readonly RequestDelegate _next;

        public IntranetMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["X-UA-Compatible"] = "IE=edge";

            await _next(context);
        }
    }
}
