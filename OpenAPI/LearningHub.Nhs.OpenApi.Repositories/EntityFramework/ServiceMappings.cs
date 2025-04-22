namespace LearningHub.Nhs.OpenApi.Repositories.EntityFramework
{
    using AutoMapper;
    using LearningHub.Nhs.Models.Automapper;
    using LearningHub.Nhs.OpenApi.Repositories.Map;
    using LearningHub.Nhs.OpenApi.Repositories.Map.Activity;
    using LearningHub.Nhs.OpenApi.Repositories.Map.Content;
    using LearningHub.Nhs.OpenApi.Repositories.Map.External;
    using LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Map.Messaging;
    using LearningHub.Nhs.OpenApi.Repositories.Map.Resources;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension class for <see cref="IServiceCollection"/> service mappings.
    /// </summary>
    public static class ServiceMappings
    {
        /// <summary>
        /// Configures Automapper.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AllowNullCollections = true;
                mc.ShouldMapMethod = m => false;
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        /// <summary>
        /// The add learning hub mappings.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public static void AddLearningHubMappings(this IServiceCollection services, IConfiguration configuration)
        {
            // Alphabetical ordering
            services.AddSingleton<IEntityTypeMap, ArticleResourceVersionFileMap>();
            services.AddSingleton<IEntityTypeMap, ArticleResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, AttachmentMap>();
            services.AddSingleton<IEntityTypeMap, AttributeMap>();
            services.AddSingleton<IEntityTypeMap, BlockCollectionMap>();
            services.AddSingleton<IEntityTypeMap, BlockMap>();
            services.AddSingleton<IEntityTypeMap, CaseResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, EmbeddedResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, EquipmentResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ExternalReferenceMap>();
            services.AddSingleton<IEntityTypeMap, ExternalReferenceUserAgreementMap>();
            services.AddSingleton<IEntityTypeMap, FileMap>();
            services.AddSingleton<IEntityTypeMap, FileTypeMap>();
            services.AddSingleton<IEntityTypeMap, FileChunkDetailMap>();
            services.AddSingleton<IEntityTypeMap, GenericFileResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ImageResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, HtmlResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, VideoResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, AudioResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ScormResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ScormResourceVersionManifestMap>();
            services.AddSingleton<IEntityTypeMap, AddressMap>();
            services.AddSingleton<IEntityTypeMap, LogMap>();
            services.AddSingleton<IEntityTypeMap, MediaBlockMap>();
            services.AddSingleton<IEntityTypeMap, ImageMap>();
            services.AddSingleton<IEntityTypeMap, NotificationMap>();
            services.AddSingleton<IEntityTypeMap, PageMap>();
            services.AddSingleton<IEntityTypeMap, PageSectionMap>();
            services.AddSingleton<IEntityTypeMap, PageSectionDetailMap>();
            services.AddSingleton<IEntityTypeMap, ImageAssetMap>();
            services.AddSingleton<IEntityTypeMap, VideoAssetMap>();

            services.AddSingleton<IEntityTypeMap, PartialFileMap>();
            services.AddSingleton<IEntityTypeMap, PermissionMap>();
            services.AddSingleton<IEntityTypeMap, PermissionRoleMap>();
            services.AddSingleton<IEntityTypeMap, ProviderMap>();
            services.AddSingleton<IEntityTypeMap, PublicationMap>();
            services.AddSingleton<IEntityTypeMap, QuestionBlockMap>();
            services.AddSingleton<IEntityTypeMap, QuestionAnswerMap>();
            services.AddSingleton<IEntityTypeMap, ResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, ResourceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceReferenceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionAuthorMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionEventMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionRatingMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionRatingSummaryMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionFlagMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionKeywordMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionProviderMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionUserAcceptanceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionValidationResultMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionValidationRuleResultMap>();
            services.AddSingleton<IEntityTypeMap, ResourceLicenceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceTypeMap>();
            services.AddSingleton<IEntityTypeMap, RoleMap>();
            services.AddSingleton<IEntityTypeMap, RoleUserGroupMap>();
            services.AddSingleton<IEntityTypeMap, ScopeMap>();
            services.AddSingleton<IEntityTypeMap, RoleUserGroupMap>();
            services.AddSingleton<IEntityTypeMap, TextBlockMap>();
            services.AddSingleton<IEntityTypeMap, UserMap>();
            services.AddSingleton<IEntityTypeMap, UserGroupMap>();
            services.AddSingleton<IEntityTypeMap, UserGroupAttributeMap>();
            services.AddSingleton<IEntityTypeMap, UserNotificationMap>();
            services.AddSingleton<IEntityTypeMap, UserUserGroupMap>();
            services.AddSingleton<IEntityTypeMap, VideoMap>();
            services.AddSingleton<IEntityTypeMap, VideoFileMap>();
            services.AddSingleton<IEntityTypeMap, WebLinkResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, WholeSlideImageBlockMap>();
            services.AddSingleton<IEntityTypeMap, WholeSlideImageBlockItemMap>();
            services.AddSingleton<IEntityTypeMap, WholeSlideImageMap>();
            services.AddSingleton<IEntityTypeMap, ImageAnnotationMap>();
            services.AddSingleton<IEntityTypeMap, ImageAnnotationMarkMap>();
            services.AddSingleton<IEntityTypeMap, WholeSlideImageFileMap>();
            services.AddSingleton<IEntityTypeMap, UrlRewritingMap>();
            services.AddSingleton<IEntityTypeMap, ResourceSyncMap>();
            services.AddSingleton<IEntityTypeMap, EmailTemplateMap>();
            services.AddSingleton<IEntityTypeMap, EmailTemplateLayoutMap>();
            services.AddSingleton<IEntityTypeMap, MessageMap>();
            services.AddSingleton<IEntityTypeMap, MessageSendMap>();
            services.AddSingleton<IEntityTypeMap, MessageSendRecipientMap>();
            services.AddSingleton<IEntityTypeMap, MessageTypeMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueAccessRequestMap>();
            services.AddSingleton<IEntityTypeMap, UserDetailsMap>();
            services.AddSingleton<IEntityTypeMap, MessageMetaDataMap>();
            services.AddSingleton<IEntityTypeMap, NotificationTemplateMap>();

            services.AddSingleton<IEntityTypeMap, NodeActivityMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionKeywordMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionProviderMap>();
            services.AddSingleton<IEntityTypeMap, NodeMap>();
            services.AddSingleton<IEntityTypeMap, NodeLinkMap>();
            services.AddSingleton<IEntityTypeMap, NodeVersionMap>();
            services.AddSingleton<IEntityTypeMap, NodePathMap>();
            services.AddSingleton<IEntityTypeMap, NodePathNodeMap>();
            services.AddSingleton<IEntityTypeMap, NodeResourceMap>();
            services.AddSingleton<IEntityTypeMap, PublicationMap>();
            services.AddSingleton<IEntityTypeMap, ResourceAzureMediaAssetMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityInteractionMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityInteractionAnswerMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourceActivityInteractionMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourcePlayedSegmentMap>();
            services.AddSingleton<IEntityTypeMap, RoadmapMap>();
            services.AddSingleton<IEntityTypeMap, RoadmapTypeMap>();
            services.AddSingleton<IEntityTypeMap, EventMap>();
            services.AddSingleton<IEntityTypeMap, ExternalSystemMap>();
            services.AddSingleton<IEntityTypeMap, ExternalSystemDeepLinkMap>();
            services.AddSingleton<IEntityTypeMap, ExternalSystemUserMap>();

            // External
            services.AddSingleton<IEntityTypeMap, UserProfileMap>();

            var dbContextOptions = new DbContextOptionsBuilder<LearningHubDbContext>()
                .UseSqlServer(configuration.GetConnectionString("LearningHubDbConnection")).Options;

            services.AddSingleton(dbContextOptions);
            services.AddSingleton<LearningHubDbContextOptions>();
        }
    }
}
