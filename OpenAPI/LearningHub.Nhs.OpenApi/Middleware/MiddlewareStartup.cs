namespace LearningHub.NHS.OpenAPI.Middleware
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// MiddlewareStartup.
    /// </summary>
    public static class MiddlewareStartup
    {
        /// <summary>
        /// Adds custom middleware.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        public static void AddCustomMiddleware(this IServiceCollection services)
        {
            services.AddScoped<RequestClientLoggingMiddleware>();
        }

        /// <summary>
        /// Includes custom middleware in middleware chain.
        /// </summary>
        /// <param name="app">The IApplication Builder.</param>
        public static void UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestClientLoggingMiddleware>();
        }
    }
}
