namespace LearningHub.Nhs.OpenApi.Repositories
{
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Maintenance;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Migrations;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Maintenance;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Messaging;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Migrations;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources;
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
            services.AddScoped<ITimezoneOffsetManager, TimezoneOffsetManager>();
            services.AddScoped<IRoleUserGroupRepository, RoleUserGroupRepository>();
            services.AddScoped<IUserUserGroupRepository, UserUserGroupRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQuestionBlockRepository, QuestionBlockRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailChangeValidationTokenRepository, EmailChangeValidationTokenRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<IBookmarkRepository, BookmarkRepository>();
            services.AddTransient<IProviderRepository, ProviderRepository>();

            // Resources
            services.AddScoped<IArticleResourceVersionRepository, ArticleResourceVersionRepository>();
            services.AddScoped<IArticleResourceVersionFileRepository, ArticleResourceVersionFileRepository>();
            services.AddScoped<ICaseResourceVersionRepository, CaseResourceVersionRepository>();
            services.AddScoped<IWholeSlideImageRepository, WholeSlideImageRepository>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IEmbeddedResourceVersionRepository, EmbeddedResourceVersionRepository>();
            services.AddScoped<IEquipmentResourceVersionRepository, EquipmentResourceVersionRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IFileChunkDetailRepository, FileChunkDetailRepository>();
            services.AddScoped<IFileTypeRepository, FileTypeRepository>();
            services.AddScoped<IGenericFileResourceVersionRepository, GenericFileResourceVersionRepository>();
            services.AddScoped<IHtmlResourceVersionRepository, HtmlResourceVersionRepository>();
            services.AddScoped<IBlockCollectionRepository, BlockCollectionRepository>();
            services.AddScoped<IImageResourceVersionRepository, ImageResourceVersionRepository>();
            services.AddScoped<IResourceLicenceRepository, ResourceLicenceRepository>();
            services.AddScoped<IResourceReferenceRepository, ResourceReferenceRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IResourceVersionAuthorRepository, ResourceVersionAuthorRepository>();
            services.AddScoped<IResourceVersionKeywordRepository, ResourceVersionKeywordRepository>();
            services.AddScoped<IResourceVersionFlagRepository, ResourceVersionFlagRepository>();
            services.AddScoped<IResourceVersionRepository, ResourceVersionRepository>();
            services.AddScoped<IVideoResourceVersionRepository, VideoResourceVersionRepository>();
            services.AddScoped<IAudioResourceVersionRepository, AudioResourceVersionRepository>();
            services.AddScoped<IScormResourceVersionRepository, ScormResourceVersionRepository>();
            services.AddScoped<IResourceVersionValidationResultRepository, ResourceVersionValidationResultRepository>();
            services.AddScoped<IWebLinkResourceVersionRepository, WebLinkResourceVersionRepository>();
            services.AddScoped<IResourceVersionUserAcceptanceRepository, ResourceVersionUserAcceptanceRepository>();
            services.AddScoped<IAssessmentResourceVersionRepository, AssessmentResourceVersionRepository>();
            services.AddScoped<IResourceVersionProviderRepository, ResourceVersionProviderRepository>();

            services.AddScoped<IResourceActivityRepository, ResourceActivityRepository>();
            services.AddScoped<IAssessmentResourceActivityRepository, AssessmentResourceActivityRepository>();
            services.AddScoped<IAssessmentResourceActivityMatchQuestionRepository, AssessmentResourceActivityMatchQuestionRepository>();
            services.AddScoped<IAssessmentResourceActivityInteractionRepository, AssessmentResourceActivityInteractionRepository>();
            services.AddScoped<IAssessmentResourceActivityInteractionAnswerRepository, AssessmentResourceActivityInteractionAnswerRepository>();
            services.AddScoped<IMediaResourceActivityRepository, MediaResourceActivityRepository>();
            services.AddScoped<IMediaResourceActivityInteractionRepository, MediaResourceActivityInteractionRepository>();
            services.AddScoped<INodeActivityRepository, NodeActivityRepository>();

            // Activity
            services.AddScoped<IResourceActivityRepository, ResourceActivityRepository>();
            services.AddScoped<IScormActivityRepository, ScormActivityRepository>();

            // Hierarchy
            services.AddScoped<INodeRepository, NodeRepository>();
            services.AddScoped<INodeResourceRepository, NodeResourceRepository>();
            services.AddScoped<INodeResourceLookupRepository, NodeResourceLookupRepository>();
            services.AddScoped<INodePathRepository, NodePathRepository>();
            services.AddScoped<IPublicationRepository, PublicationRepository>();
            services.AddScoped<ICatalogueNodeVersionRepository, CatalogueNodeVersionRepository>();
            services.AddScoped<ICatalogueAccessRequestRepository, CatalogueAccessRequestRepository>();
            services.AddScoped<IFolderNodeVersionRepository, FolderNodeVersionRepository>();
            services.AddScoped<IHierarchyEditRepository, HierarchyEditRepository>();
            services.AddScoped<IHierarchyEditDetailRepository, HierarchyEditDetailRepository>();

            // External
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IInternalSystemRepository, InternalSystemRepository>();
            services.AddScoped<IMigrationSourceRepository, MigrationSourceRepository>();

        }
    }
}