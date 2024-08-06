namespace LearningHub.Nhs.Api
{
    using System;
    using System.Configuration;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Migration.Interface.Mapping;
    using LearningHub.Nhs.Migration.Interface.Validation;
    using LearningHub.Nhs.Migration.Mapping;
    using LearningHub.Nhs.Migration.Mapping.Helpers;
    using LearningHub.Nhs.Migration.Staging.Repository;
    using LearningHub.Nhs.Migration.Validation;
    using LearningHub.Nhs.Migration.Validation.Helpers;
    using LearningHub.Nhs.Migration.Validation.Rules;
    using LearningHub.Nhs.Repository;
    using LearningHub.Nhs.Repository.Activity;
    using LearningHub.Nhs.Repository.Analytics;
    using LearningHub.Nhs.Repository.Content;
    using LearningHub.Nhs.Repository.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;
    using LearningHub.Nhs.Repository.Interface.Analytics;
    using LearningHub.Nhs.Repository.Interface.Content;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Maintenance;
    using LearningHub.Nhs.Repository.Interface.Messaging;
    using LearningHub.Nhs.Repository.Interface.Migrations;
    using LearningHub.Nhs.Repository.Interface.Report;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Repository.Maintenance;
    using LearningHub.Nhs.Repository.Map;
    using LearningHub.Nhs.Repository.Map.Activity;
    using LearningHub.Nhs.Repository.Map.Content;
    using LearningHub.Nhs.Repository.Map.Hierarchy;
    using LearningHub.Nhs.Repository.Map.Messaging;
    using LearningHub.Nhs.Repository.Map.Migrations;
    using LearningHub.Nhs.Repository.Map.Report;
    using LearningHub.Nhs.Repository.Map.Resources;
    using LearningHub.Nhs.Repository.Messaging;
    using LearningHub.Nhs.Repository.Migrations;
    using LearningHub.Nhs.Repository.Report;
    using LearningHub.Nhs.Repository.Resources;
    using LearningHub.Nhs.Services;
    using LearningHub.Nhs.Services.Findwise;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Messaging;
    using LearningHub.Nhs.Services.Interface.Report;
    using LearningHub.Nhs.Services.Messaging;
    using LearningHub.Nhs.Services.Report;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension class for <see cref="IServiceCollection"/> service mappings.
    /// </summary>
    public static class ServiceMappings
    {
        /// <summary>
        /// The add learning hub mappings.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
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
            services.AddSingleton<IEntityTypeMap, HtmlResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ImageResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, VideoResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, AudioResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ScormResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ScormResourceVersionManifestMap>();
            services.AddSingleton<IEntityTypeMap, AddressMap>();
            services.AddSingleton<IEntityTypeMap, MediaBlockMap>();
            services.AddSingleton<IEntityTypeMap, ImageMap>();
            services.AddSingleton<IEntityTypeMap, NotificationMap>();
            services.AddSingleton<IEntityTypeMap, PageMap>();
            services.AddSingleton<IEntityTypeMap, PageSectionMap>();
            services.AddSingleton<IEntityTypeMap, PageSectionDetailMap>();
            services.AddSingleton<IEntityTypeMap, ImageAssetMap>();
            services.AddSingleton<IEntityTypeMap, ImageCarouselBlockMap>();
            services.AddSingleton<IEntityTypeMap, VideoAssetMap>();

            services.AddSingleton<IEntityTypeMap, PartialFileMap>();
            services.AddSingleton<IEntityTypeMap, PermissionMap>();
            services.AddSingleton<IEntityTypeMap, PermissionRoleMap>();
            services.AddSingleton<IEntityTypeMap, PublicationMap>();
            services.AddSingleton<IEntityTypeMap, QuestionBlockMap>();
            services.AddSingleton<IEntityTypeMap, QuestionAnswerMap>();
            services.AddSingleton<IEntityTypeMap, ResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, ResourceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceReferenceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionAuthorMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionEventMap>();
            services.AddScoped<IResourceReferenceEventRepository, ResourceReferenceEventRepository>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionRatingMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionRatingSummaryMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionFlagMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionKeywordMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionUserAcceptanceMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionValidationResultMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionValidationRuleResultMap>();
            services.AddSingleton<IEntityTypeMap, ResourceVersionProviderMap>();
            services.AddSingleton<IEntityTypeMap, ResourceLicenceMap>();
            services.AddSingleton<IEntityTypeMap, RoleMap>();
            services.AddSingleton<IEntityTypeMap, RoleUserGroupMap>();
            services.AddSingleton<IEntityTypeMap, ScopeMap>();
            services.AddSingleton<IEntityTypeMap, RoleUserGroupMap>();
            services.AddSingleton<IEntityTypeMap, TextBlockMap>();
            services.AddSingleton<IEntityTypeMap, ScormActivityInteractionMap>();
            services.AddSingleton<IEntityTypeMap, ScormActivityInteractionCorrectResponseMap>();
            services.AddSingleton<IEntityTypeMap, ScormActivityInteractionObjectiveMap>();
            services.AddSingleton<IEntityTypeMap, ScormActivityMap>();
            services.AddSingleton<IEntityTypeMap, ScormActivityObjectiveMap>();
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
            services.AddSingleton<IEntityTypeMap, ResourceReferenceEventMap>();
            services.AddSingleton<IEntityTypeMap, ResourceSyncMap>();
            services.AddSingleton<IEntityTypeMap, EmailTemplateMap>();
            services.AddSingleton<IEntityTypeMap, EmailTemplateLayoutMap>();
            services.AddSingleton<IEntityTypeMap, EmailChangeValidationTokenMap>();
            services.AddSingleton<IEntityTypeMap, MessageMap>();
            services.AddSingleton<IEntityTypeMap, MessageSendMap>();
            services.AddSingleton<IEntityTypeMap, MessageSendRecipientMap>();
            services.AddSingleton<IEntityTypeMap, MessageTypeMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueAccessRequestMap>();
            services.AddSingleton<IEntityTypeMap, UserDetailsMap>();
            services.AddSingleton<IEntityTypeMap, MessageMetaDataMap>();
            services.AddSingleton<IEntityTypeMap, NotificationTemplateMap>();
            services.AddSingleton<IEntityTypeMap, DetectJsLogMap>();

            services.AddSingleton<IEntityTypeMap, NodeActivityMap>();
            services.AddSingleton<IEntityTypeMap, FolderNodeVersionMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionKeywordMap>();
            services.AddSingleton<IEntityTypeMap, CatalogueNodeVersionProviderMap>();
            services.AddSingleton<IEntityTypeMap, HierarchyEditMap>();
            services.AddSingleton<IEntityTypeMap, HierarchyEditDetailMap>();
            services.AddSingleton<IEntityTypeMap, NodeMap>();
            services.AddSingleton<IEntityTypeMap, NodeLinkMap>();
            services.AddSingleton<IEntityTypeMap, NodeVersionMap>();
            services.AddSingleton<IEntityTypeMap, NodePathMap>();
            services.AddSingleton<IEntityTypeMap, NodePathNodeMap>();
            services.AddSingleton<IEntityTypeMap, NodeResourceMap>();
            services.AddSingleton<IEntityTypeMap, NodeResourceLookupMap>();
            services.AddSingleton<IEntityTypeMap, PublicationMap>();
            services.AddSingleton<IEntityTypeMap, PublicationLogMap>();
            services.AddSingleton<IEntityTypeMap, ResourceAzureMediaAssetMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityInteractionMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityInteractionAnswerMap>();
            services.AddSingleton<IEntityTypeMap, AssessmentResourceActivityMatchQuestionMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourceActivityMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourceActivityInteractionMap>();
            services.AddSingleton<IEntityTypeMap, MediaResourcePlayedSegmentMap>();
            services.AddSingleton<IEntityTypeMap, RoadmapMap>();
            services.AddSingleton<IEntityTypeMap, RoadmapTypeMap>();
            services.AddSingleton<IEntityTypeMap, EventMap>();
            services.AddSingleton<IEntityTypeMap, UserBookmarkMap>();

            services.AddSingleton<IEntityTypeMap, ReportMap>();
            services.AddSingleton<IEntityTypeMap, ReportPageMap>();
            services.AddSingleton<IEntityTypeMap, ClientMap>();
            services.AddSingleton<IEntityTypeMap, ReportTypeMap>();
            services.AddSingleton<IEntityTypeMap, ReportStatusMap>();
            services.AddSingleton<IEntityTypeMap, ReportOrientationModeMap>();
            services.AddSingleton<IEntityTypeMap, InternalSystemMap>();
            services.AddScoped<IUserLearningRecordService, UserLearningRecordService>();

            // External
            services.AddSingleton<IEntityTypeMap, UserProfileMap>();

            var maxDatabaseRetryAttempts = configuration.GetValue<int>("Settings:MaxDatabaseRetryAttempts");

            // Provider
            services.AddSingleton<IEntityTypeMap, ProviderMap>();
            services.AddSingleton<IEntityTypeMap, UserProviderMap>();

            var dbContextOptions = new DbContextOptionsBuilder<LearningHubDbContext>()
                .UseSqlServer(configuration.GetConnectionString("LearningHubDbConnection"), providerOptions => { providerOptions.EnableRetryOnFailure(maxDatabaseRetryAttempts); })
                .Options;

            services.AddSingleton(dbContextOptions);
            services.AddSingleton<LearningHubDbContextOptions>();

            // Services
            services.AddDbContext<LearningHubDbContext>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IFileTypeService, FileTypeService>();
            services.AddScoped<IHierarchyService, HierarchyService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFindWiseHttpClient, FindWiseHttpClient>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IResourceReferenceService, ResourceReferenceService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IQueueCommunicatorService, QueueCommunicatorService>();
            services.AddTransient<IRoadmapService, RoadmapService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ICatalogueService, CatalogueService>();
            services.AddScoped<IResourceSyncService, ResourceSyncService>();
            services.AddScoped<IFindwiseApiFacade, FindwiseApiFacade>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
            services.AddScoped<IEventLogService, EventLogService>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<IPartialFileService, PartialFileService>();
            services.AddScoped<IDetectJsLogService, DetectJsLogService>();

            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IInternalSystemService, InternalSystemService>();
            services.AddTransient<IUserLearningRecordService, UserLearningRecordService>();

            services.AddDbContext<DLSDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DLSDbConnection"),
                    providerOptions => { providerOptions.EnableRetryOnFailure(maxDatabaseRetryAttempts); }));

            // External
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddTransient<IMyLearningService, MyLearningService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IScormContentServerService, ScormContentServerService>();
            services.AddScoped<IPageService, PageService>();

            // Caching services
            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<ICacheService, CacheService>();

            // Transactions
            services.AddScoped<ITransactionManager, TransactionManager>();

            services.AddScoped<ITimezoneOffsetManager, TimezoneOffsetManager>();

            // Provider
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IUserProviderService, UserProviderService>();

            // Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IScopeRepository, ScopeRepository>();
            services.AddScoped<IRoleUserGroupRepository, RoleUserGroupRepository>();
            services.AddScoped<IUserUserGroupRepository, UserUserGroupRepository>();
            services.AddScoped<IUserGroupAttributeRepository, UserGroupAttributeRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IQuestionBlockRepository, QuestionBlockRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailChangeValidationTokenRepository, EmailChangeValidationTokenRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddTransient<IResourceSyncRepository, ResourceSyncRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<IBookmarkRepository, BookmarkRepository>();
            services.AddTransient<IDetectJsLogRepository, DetectJsLogRepository>();
            services.AddTransient<IInternalSystemRepository, InternalSystemRepository>();
            services.AddTransient<IUserLearningRecordService, UserLearningRecordService>();
            services.AddTransient<IProviderRepository, ProviderRepository>();
            services.AddTransient<IUserProviderRepository, UserProviderRepository>();

            // Page
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IPageSectionRepository, PageSectionRepository>();
            services.AddScoped<IPageSectionDetailRepository, PageSectionDetailRepository>();
            services.AddScoped<IVideoAssetRepository, VideoAssetRepository>();

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
            services.AddScoped<IPartialFileRepository, PartialFileRepository>();
            services.AddScoped<IWholeSlideImageFileRepository, WholeSlideImageFileRepository>();
            services.AddScoped<IBlockCollectionRepository, BlockCollectionRepository>();
            services.AddScoped<IImageResourceVersionRepository, ImageResourceVersionRepository>();
            services.AddScoped<IResourceLicenceRepository, ResourceLicenceRepository>();
            services.AddScoped<IResourceReferenceRepository, ResourceReferenceRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IResourceVersionAuthorRepository, ResourceVersionAuthorRepository>();
            services.AddScoped<IResourceVersionKeywordRepository, ResourceVersionKeywordRepository>();
            services.AddScoped<IResourceVersionEventRepository, ResourceVersionEventRepository>();
            services.AddScoped<IResourceVersionRatingSummaryRepository, ResourceVersionRatingSummaryRepository>();
            services.AddScoped<IResourceVersionRatingRepository, ResourceVersionRatingRepository>();
            services.AddScoped<IResourceVersionFlagRepository, ResourceVersionFlagRepository>();
            services.AddScoped<IResourceVersionRepository, ResourceVersionRepository>();
            services.AddScoped<IVideoResourceVersionRepository, VideoResourceVersionRepository>();
            services.AddScoped<IAudioResourceVersionRepository, AudioResourceVersionRepository>();
            services.AddScoped<IScormResourceVersionRepository, ScormResourceVersionRepository>();
            services.AddScoped<IScormResourceVersionManifestRepository, ScormResourceVersionManifestRepository>();
            services.AddScoped<IResourceVersionValidationResultRepository, ResourceVersionValidationResultRepository>();
            services.AddScoped<IWebLinkResourceVersionRepository, WebLinkResourceVersionRepository>();
            services.AddScoped<IUrlRewritingRepository, UrlRewritingRepository>();
            services.AddScoped<IResourceAzureMediaAssetRepository, ResourceAzureMediaAssetRepository>();
            services.AddScoped<IResourceVersionUserAcceptanceRepository, ResourceVersionUserAcceptanceRepository>();
            services.AddScoped<IRoadmapRepository, RoadmapRepository>();
            services.AddScoped<IExternalReferenceUserAgreementRepository, ExternalReferenceUserAgreementRepository>();
            services.AddScoped<IExternalReferenceRepository, ExternalReferenceRepository>();
            services.AddScoped<IVideoFileRepository, VideoFileRepository>();
            services.AddScoped<IAssessmentResourceVersionRepository, AssessmentResourceVersionRepository>();
            services.AddScoped<IResourceVersionProviderRepository, ResourceVersionProviderRepository>();

            // Activity
            services.AddScoped<IResourceActivityRepository, ResourceActivityRepository>();
            services.AddScoped<IAssessmentResourceActivityRepository, AssessmentResourceActivityRepository>();
            services.AddScoped<IAssessmentResourceActivityMatchQuestionRepository, AssessmentResourceActivityMatchQuestionRepository>();
            services.AddScoped<IAssessmentResourceActivityInteractionRepository, AssessmentResourceActivityInteractionRepository>();
            services.AddScoped<IAssessmentResourceActivityInteractionAnswerRepository, AssessmentResourceActivityInteractionAnswerRepository>();
            services.AddScoped<IMediaResourceActivityRepository, MediaResourceActivityRepository>();
            services.AddScoped<IMediaResourceActivityInteractionRepository, MediaResourceActivityInteractionRepository>();
            services.AddScoped<IMediaResourcePlayedSegmentRepository, MediaResourcePlayedSegmentRepository>();
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
            services.AddScoped<IPublicationLogRepository, PublicationLogRepository>();
            services.AddScoped<INodeVersionRepository, NodeVersionRepository>();
            services.AddScoped<ICatalogueNodeVersionRepository, CatalogueNodeVersionRepository>();
            services.AddScoped<ICatalogueNodeVersionKeywordRepository, CatalogueNodeVersionKeywordRepository>();
            services.AddScoped<ICatalogueNodeVersionProviderRepository, CatalogueNodeVersionProviderRepository>();
            services.AddScoped<ICatalogueAccessRequestRepository, CatalogueAccessRequestRepository>();
            services.AddScoped<IFolderNodeVersionRepository, FolderNodeVersionRepository>();
            services.AddScoped<IHierarchyEditRepository, HierarchyEditRepository>();
            services.AddScoped<IHierarchyEditDetailRepository, HierarchyEditDetailRepository>();

            // Hub
            services.AddScoped<IEventLogRepository, EventLogRepository>();

            // External
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Reports
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            // Migration Tool
            var migrationStagingDbContextOptions = new DbContextOptionsBuilder<MigrationStagingDbContext>()
                .UseSqlServer(configuration.GetConnectionString("MigrationStagingDbConnection"), providerOptions => { providerOptions.EnableRetryOnFailure(maxDatabaseRetryAttempts); }).Options;
            services.AddSingleton(migrationStagingDbContextOptions);
            services.AddDbContext<MigrationStagingDbContext>();
            services.AddSingleton<IEntityTypeMap, MigrationMap>();
            services.AddSingleton<IEntityTypeMap, MigrationInputRecordMap>();
            services.AddSingleton<IEntityTypeMap, MigrationSourceMap>();
            services.AddScoped<IMigrationRepository, MigrationRepository>();
            services.AddScoped<IMigrationInputRecordRepository, MigrationInputRecordRepository>();
            services.AddScoped<IMigrationSourceRepository, MigrationSourceRepository>();
            services.AddScoped<IStagingTableInputModelRepository, StagingTableInputModelRepository>();
            services.AddScoped<IMigrationService, MigrationService>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IAzureMediaService, AzureMediaService>();
            services.AddScoped<IAzureDataFactoryService, AzureDataFactoryService>();
            services.AddScoped<IInputRecordValidatorFactory, InputRecordValidatorFactory>();
            services.AddScoped<IInputRecordMapperFactory, InputRecordMapperFactory>();
            services.AddScoped<StagingTableInputRecordValidator>();
            services.AddScoped<StandardInputRecordValidator>();
            services.AddScoped<AzureBlobFileValidator>();
            services.AddScoped<CatalogueNameExistsValidator>();
            services.AddScoped<FileExtensionIsAllowedValidator>();
            services.AddScoped<HtmlStringDeadLinkValidator>();
            services.AddScoped<UrlExistsValidator>();
            services.AddScoped<UserIdExistsValidator>();
            services.AddScoped<UserNameExistsValidator>();
            services.AddScoped<EsrLinkValidator>();
            services.AddScoped<UrlChecker>();
            services.AddScoped<StandardInputRecordMapper>();
            services.AddScoped<StagingTableInputRecordMapper>();
            services.AddScoped<UserLookup>();
            services.AddScoped<FileTypeLookup>();
            services.AddScoped<NodeIdLookup>();

            // nLog Logging and Mapping

            // nlog db context options and db context
            var nlogDbContextOptions = new DbContextOptionsBuilder<NLogDbContext>()
                .UseSqlServer(configuration.GetConnectionString("NLogDb"), providerOptions => { providerOptions.EnableRetryOnFailure(maxDatabaseRetryAttempts); }).Options;

            services.AddSingleton(nlogDbContextOptions);
            services.AddSingleton<NLogDbContextOptions>();
            services.AddDbContext<NLogDbContext>();

            // entity mapping
            services.AddSingleton<LearningHub.Nhs.Repository.NLogMap.IEntityTypeMap, LogMap>();

            // nlog repository
            services.AddScoped<ILogRepository, LogRepository>();

            // nlog services
            services.AddScoped<ILogService, LogService>();
        }
    }
}
