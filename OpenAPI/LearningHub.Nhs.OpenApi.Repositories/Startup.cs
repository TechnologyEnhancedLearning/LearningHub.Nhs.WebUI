namespace LearningHub.Nhs.OpenApi.Repositories
{
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories;
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
        /// <param name="config">The IConfiguration.</param>
        public static void AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.ConfigureAutomapper();
            services.AddLearningHubMappings(config);
            services.AddRepositoryImplementations();
        }

        private static void AddRepositoryImplementations(this IServiceCollection services)
        {
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<ICatalogueRepository, CatalogueRepository>();
        }
    }
}