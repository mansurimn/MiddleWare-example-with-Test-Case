using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.WebAPI
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MyCustomMiddlware
    {
        private readonly RequestDelegate _next;

        public MyCustomMiddlware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyCustomMiddlwareExtensions
    {
        public static IApplicationBuilder UseMyCustomMiddlware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyCustomMiddlware>();
        }
    }
}
