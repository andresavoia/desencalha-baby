using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Middlewares
{
    public class ApplicationMiddleware
    {
        private readonly RequestDelegate _next;

        public ApplicationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string newCorrelation = Guid.NewGuid().ToString();

            httpContext.Response.Headers.Add("X-Correlation-ID", newCorrelation);
            httpContext.TraceIdentifier = newCorrelation;
            
            return _next(httpContext);
        }
    }
}
