using Savoia.Desencalha.Host.WebApi.Middlewares;

namespace Savoia.Desencalha.Host.WebApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApplicationMiddleware>();
        }
    }
}
