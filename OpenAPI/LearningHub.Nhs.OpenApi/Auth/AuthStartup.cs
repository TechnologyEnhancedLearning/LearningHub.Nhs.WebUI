namespace LearningHub.NHS.OpenAPI.Auth
{
    using AspNetCore.Authentication.ApiKey;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Static methods for configuring auth.
    /// </summary>
    public static class AuthStartup
    {
        /// <summary>
        /// Adds API key authentication services to the API.
        /// </summary>
        /// <param name="services">The service collection passed into <see cref="Startup.ConfigureServices"/>.</param>
        public static void AddApiKeyAuth(this IServiceCollection services)
        {
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

            services.AddAuthentication()
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(
                    options =>
                    {
                        options.Realm = "Learning Hub Open API";
                        options.KeyName = AuthConstants.ApiKeyHeaderName;
                    });

            services.AddAuthorization();
        }

        /// <summary>
        /// Adds auth to the API middleware.
        /// </summary>
        /// <param name="app">The app builder passed into <see cref="Startup.Configure"/>.</param>
        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
