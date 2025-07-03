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
    using LearningHub.Nhs.Services;
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
            services.AddScoped<INavigationPermissionService, NavigationPermissionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();

            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IEventLogService, EventLogService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IResourceReferenceService, ResourceReferenceService>();

            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IMyLearningService, MyLearningService>();
            services.AddScoped<IUserLearningRecordService, UserLearningRecordService>();
            services.AddScoped<IFileTypeService, FileTypeService>();
            services.AddScoped<IHierarchyService, HierarchyService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IResourceSyncService, ResourceSyncService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IQueueCommunicatorService, QueueCommunicatorService>();
            services.AddScoped<IFindwiseApiFacade, FindwiseApiFacade>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
            services.AddScoped<IInternalSystemService, InternalSystemService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IUserProviderService, UserProviderService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IUserPasswordResetRequestsService, UserPasswordResetRequestsService>();
        }
    }
}