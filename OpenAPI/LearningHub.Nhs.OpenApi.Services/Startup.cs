namespace LearningHub.Nhs.OpenApi.Services
{
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Services;
    using LearningHub.Nhs.OpenApi.Services.Services.Findwise;
    using LearningHub.Nhs.OpenApi.Services.Services.Messaging;
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
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFindwiseClient, FindwiseClient>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ILearningHubService, LearningHubService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<ILearningHubApiHttpClient, LearningHubApiHttpClient>();

            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IFileTypeService, FileTypeService>();
            services.AddScoped<IHierarchyService, HierarchyService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IQueueCommunicatorService, QueueCommunicatorService>();
            services.AddTransient<ICatalogueService, CatalogueService>();
            services.AddScoped<IFindwiseApiFacade, FindwiseApiFacade>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<IInternalSystemService, InternalSystemService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IProviderService, ProviderService>();

        }
    }
}