namespace LearningHub.Nhs.MessagingService
{
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.MessagingService.Model;
    using LearningHub.Nhs.MessagingService.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Contains methods to configure the project to be called in an API startup class.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Registers the implementations in the project with ASP.NET DI.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfiguration.</param>
        public static void AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MessagingServiceModel>(configuration.GetSection("GovNotify"));
            services.AddScoped<IGovNotifyService, GovNotifyService>();
        }
    }
}
