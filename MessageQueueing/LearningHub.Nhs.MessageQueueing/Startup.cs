namespace LearningHub.Nhs.MessageQueueing
{
    using LearningHub.Nhs.MessageQueueing.EntityFramework;
    using LearningHub.Nhs.MessageQueueing.Repositories;
    using Microsoft.EntityFrameworkCore;
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
        public static void AddQueueingRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var dbContextOptions = new DbContextOptionsBuilder<MessageQueueDbContext>()
               .UseSqlServer(configuration.GetConnectionString("GovNotifyMessageDbConnection"), providerOptions => { providerOptions.EnableRetryOnFailure(3); })
               .Options;

            services.AddSingleton(dbContextOptions);
            services.AddSingleton<MessageQueueDbContextOptions>();

            services.AddDbContext<MessageQueueDbContext>();
            services.AddScoped<IMessageQueueRepository, MessageQueueRepository>();
        }
    }
}
