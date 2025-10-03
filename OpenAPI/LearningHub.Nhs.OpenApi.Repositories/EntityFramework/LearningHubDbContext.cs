namespace LearningHub.Nhs.OpenApi.Repositories.EntityFramework
{
    using LearningHub.Nhs.Entities.Resource;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Content;
    using LearningHub.Nhs.Models.Entities.External;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Notification;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.Models.User;
    using Microsoft.EntityFrameworkCore;
    using Event = LearningHub.Nhs.Models.Entities.Analytics.Event;

    /// <summary>
    /// The learning hub db context.
    /// </summary>
    public partial class LearningHubDbContext : DbContext
    {
        /// <summary>
        /// The options..
        /// </summary>
        private readonly LearningHubDbContextOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="LearningHubDbContextOptions"/>.</param>
        public LearningHubDbContext(LearningHubDbContextOptions options)
            : base(options.Options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the Options.
        /// </summary>
        public LearningHubDbContextOptions Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public virtual DbSet<Address> Address { get; set; }

        /// <summary>
        /// Gets or sets the Log.
        /// </summary>
        public virtual DbSet<Log> Log { get; set; }

        /// <summary>
        /// Gets or sets the Permission.
        /// </summary>
        public virtual DbSet<Permission> Permission { get; set; }

        /// <summary>
        /// Gets or sets the permission role..
        /// </summary>
        public virtual DbSet<PermissionRole> PermissionRole { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        public virtual DbSet<Role> Role { get; set; }

        /// <summary>
        /// Gets or sets the role user group..
        /// </summary>
        public virtual DbSet<RoleUserGroup> RoleUserGroup { get; set; }

        /// <summary>
        /// Gets or sets the password requests.
        /// </summary>
        public virtual DbSet<PasswordResetRequests> PasswordResetRequests { get; set; }

        /// <summary>
        /// Gets or sets the email change validation token.
        /// </summary>
        public virtual DbSet<EmailChangeValidationToken> EmailChangeValidationToken { get; set; }

        /// <summary>
        /// Gets or sets the User.
        /// </summary>
        public virtual DbSet<User> User { get; set; }

        /// <summary>
        /// Gets or sets the user group..
        /// </summary>
        public virtual DbSet<UserGroup> UserGroup { get; set; }

        /// <summary>
        /// Gets or sets the user group attribute.
        /// </summary>
        public virtual DbSet<UserGroupAttribute> UserGroupAttribute { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public virtual DbSet<Scope> Scope { get; set; }

        /// <summary>
        /// Gets or sets the user user group..
        /// </summary>
        public virtual DbSet<UserUserGroup> UserUserGroup { get; set; }

        /// <summary>
        /// Gets or sets the role user group view model.
        /// </summary>
        public virtual DbSet<RoleUserGroupViewModel> RoleUserGroupViewModel { get; set; }

        /// <summary>
        /// Gets or sets the Notification.
        /// </summary>
        public virtual DbSet<Notification> Notification { get; set; }

        /// <summary>
        /// Gets or sets the Notification Count.
        /// </summary>
        public virtual DbSet<NotificationCount> NotificationCount { get; set; }

        /// <summary>
        /// Gets or sets the user notification..
        /// </summary>
        public virtual DbSet<UserNotification> UserNotification { get; set; }

        /// <summary>
        /// Gets or sets the Roadmap.
        /// </summary>
        public virtual DbSet<Roadmap> Roadmap { get; set; }

        /// <summary>
        /// Gets or sets the roadmap type.
        /// </summary>
        public virtual DbSet<RoadmapType> RoadmapType { get; set; }

        /// <summary>
        /// Gets or sets the resource activity.
        /// </summary>
        public virtual DbSet<ResourceActivity> ResourceActivity { get; set; }

        /// <summary>
        /// Gets or sets the node activity.
        /// </summary>
        public virtual DbSet<NodeActivity> NodeActivity { get; set; }

        /// <summary>
        /// Gets or sets the Resource.
        /// </summary>
        public virtual DbSet<Resource> Resource { get; set; }

        /// <summary>
        /// gets or sets the resource type.
        /// </summary>
        public virtual DbSet<ResourceType> ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the resource reference.
        /// </summary>
        public virtual DbSet<ResourceReference> ResourceReference { get; set; }

        /// <summary>
        /// Gets or sets the resource version..
        /// </summary>
        public virtual DbSet<ResourceVersion> ResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the resource version author..
        /// </summary>
        public virtual DbSet<ResourceVersionAuthor> ResourceVersionAuthor { get; set; }

        /// <summary>
        /// Gets or sets the resource version keyword..
        /// </summary>
        public virtual DbSet<ResourceVersionKeyword> ResourceVersionKeyword { get; set; }

        /// <summary>
        /// Gets or sets the resource version flag..
        /// </summary>
        public virtual DbSet<ResourceVersionFlag> ResourceVersionFlag { get; set; }


        /// <summary>
        /// Gets or sets the resource version validation result.
        /// </summary>
        public virtual DbSet<ResourceVersionValidationResult> ResourceVersionValidationResult { get; set; }

        /// <summary>
        /// Gets or sets the resource version validation rule result.
        /// </summary>
        public virtual DbSet<ResourceVersionValidationRuleResult> ResourceVersionValidationRuleResult { get; set; }

        /// <summary>
        /// Gets or sets the resource version event..
        /// </summary>
        public virtual DbSet<ResourceVersionEvent> ResourceVersionEvent { get; set; }

        /// <summary>
        /// Gets or sets the resource version ratings..
        /// </summary>
        public virtual DbSet<ResourceVersionRating> ResourceVersionRating { get; set; }

        /// <summary>
        /// Gets or sets the resource version rating summaries..
        /// </summary>
        public virtual DbSet<ResourceVersionRatingSummary> ResourceVersionRatingSummary { get; set; }

        /// <summary>
        /// Gets or sets the article resource version..
        /// </summary>
        public virtual DbSet<ArticleResourceVersion> ArticleResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the embedded resource version..
        /// </summary>
        public virtual DbSet<EmbeddedResourceVersion> EmbeddedResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the equipment resource version..
        /// </summary>
        public virtual DbSet<EquipmentResourceVersion> EquipmentResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the web link resource version..
        /// </summary>
        public virtual DbSet<WebLinkResourceVersion> WebLinkResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the case resource version..
        /// </summary>
        public virtual DbSet<CaseResourceVersion> CaseResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the assessment resource version..
        /// </summary>
        public virtual DbSet<AssessmentResourceVersion> AssessmentResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the article resource version file..
        /// </summary>
        public virtual DbSet<ArticleResourceVersionFile> ArticleResourceVersionFile { get; set; }

        /// <summary>
        /// Gets or sets the file type..
        /// </summary>
        public virtual DbSet<FileType> FileType { get; set; }

        /// <summary>
        /// Gets or sets the generic file resource version..
        /// </summary>
        public virtual DbSet<GenericFileResourceVersion> GenericFileResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the image resource version..
        /// </summary>
        public virtual DbSet<ImageResourceVersion> ImageResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the video resource version..
        /// </summary>
        public virtual DbSet<VideoResourceVersion> VideoResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the audio resource version..
        /// </summary>
        public virtual DbSet<AudioResourceVersion> AudioResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the scorm resource version..
        /// </summary>
        public virtual DbSet<ScormResourceVersion> ScormResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the scorm resource version manifest..
        /// </summary>
        public virtual DbSet<ScormResourceVersionManifest> ScormResourceVersionManifest { get; set; }

        /// <summary>
        /// Gets or sets the external reference. Entity stores the LH external reference used by LH ESR links.
        /// </summary>
        public virtual DbSet<ExternalReference> ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the UrlRewriting. Entity stores a full historic URL used by an ESR link to a migrated SCORM resource.
        /// </summary>
        public virtual DbSet<UrlRewriting> UrlRewriting { get; set; }

        /// <summary>
        /// Gets or sets the external reference user agreement.
        /// </summary>
        public virtual DbSet<ExternalReferenceUserAgreement> ExternalReferenceUserAgreement { get; set; }

        /// <summary>
        /// Gets or sets the resource azure media asset..
        /// </summary>
        public virtual DbSet<ResourceAzureMediaAsset> ResourceAzureMediaAsset { get; set; }

        /// <summary>
        /// Gets or sets the resource sync..
        /// </summary>
        public virtual DbSet<ResourceSync> ResourceSync { get; set; }

        /// <summary>
        /// Gets or sets the File.
        /// </summary>
        public virtual DbSet<File> File { get; set; }

        /// <summary>
        /// Gets or sets the partial file..
        /// </summary>
        public virtual DbSet<PartialFile> PartialFile { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image file..
        /// </summary>
        public virtual DbSet<WholeSlideImageFile> WholeSlideImageFile { get; set; }

        /// <summary>
        /// Gets or sets the file chunk detail..
        /// </summary>
        public virtual DbSet<FileChunkDetail> FileChunkDetail { get; set; }

        /// <summary>
        /// Gets or sets the ResourceActivityDto. These are not entities. They are returned from the [activity].[GetResourceActivityPerResourceMajorVersion] stored proc..
        /// </summary>
        public virtual DbSet<ResourceActivityDTO> ResourceActivityDTO { get; set; }

        /// <summary>
        /// Gets or sets the scorm activity.
        /// </summary>
        public virtual DbSet<ScormActivity> ScormActivity { get; set; }

        /// <summary>
        /// Gets or sets the RecentlyAddedResources. These are not entities. They are returned from the [resources].[GetRecentlyAddedResources] stored proc..
        /// </summary>
        public virtual DbSet<RecentlyAddedResourceViewModel> RecentlyAddedResources { get; set; }

        /// <summary>
        /// Gets or sets the ResourceContributionDto. These are not entities. They are returned from the [resources].[GetContributions] stored proc..
        /// </summary>
        public virtual DbSet<ResourceContributionDto> ResourceContributionDto { get; set; }

        /// <summary>
        /// Gets or sets the DashboardResourceDto
        /// Gets or sets DashboardResourceDto. These are not entities. They are returned from the [resources].[GetDashboardResources] stored proc..
        /// </summary>
        public virtual DbSet<DashboardResourceDto> DashboardResourceDto { get; set; }


        /// <summary>
        /// Gets or sets the UserCertificateViewModel
        /// Gets or sets DashboardResourceDto. These are not entities. They are returned from the [resources].[GetUserCertificateDetails] stored proc..
        /// </summary>
        public virtual DbSet<UserCertificateViewModel> UserCertificateViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ExternalContentDetailsViewModel.
        /// </summary>
        public virtual DbSet<ExternalContentDetailsViewModel> ExternalContentDetailsViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ContentServerViewModel.
        /// </summary>
        public virtual DbSet<ContentServerViewModel> ContentServerViewModel { get; set; }

        /// <summary>
        /// Gets or sets the DashboardCatalogueDto
        /// Gets or sets DashboardCatalogueDto. These are not entities. They are returned from the [hierarchy].[GetDashboardCatalogues] stored proc..
        /// </summary>
        public virtual DbSet<DashboardCatalogueDto> DashboardCatalogueDto { get; set; }

        /// <summary>
        /// Gets or sets the Node.
        /// </summary>
        public virtual DbSet<Node> Node { get; set; }

        /// <summary>
        /// Gets or sets the node version..
        /// </summary>
        public virtual DbSet<NodeVersion> NodeVersion { get; set; }

        /// <summary>
        /// Gets or sets the node path..
        /// </summary>
        public virtual DbSet<NodePath> NodePath { get; set; }

        /// <summary>
        /// Gets or sets the node path node..
        /// </summary>
        public virtual DbSet<NodePathNode> NodePathNode { get; set; }

        /// <summary>
        /// Gets or sets the NodeViewModel.
        /// </summary>
        public virtual DbSet<NodeViewModel> NodeViewModel { get; set; }


        /// <summary>
        /// Gets or sets the NodePathNodeViewModel.
        /// </summary>
        public virtual DbSet<NodePathNodeViewModel> NodePathNodeViewModel { get; set; }

        /// <summary>
        /// Gets or sets the NodeContentBrowseViewModel.
        /// </summary>
        public virtual DbSet<NodeContentBrowseViewModel> NodeContentBrowseViewModel { get; set; }

        /// <summary>
        /// Gets or sets the NodeContentEditorViewModel.
        /// </summary>
        public virtual DbSet<NodeContentEditorViewModel> NodeContentEditorViewModel { get; set; }


        /// <summary>
        /// Gets or sets the NodeContentAdminViewModel.
        /// </summary>
        public virtual DbSet<NodeContentAdminViewModel> NodeContentAdminViewModel { get; set; }

        /// <summary>
        /// Gets or sets the User recent my learning activities.
        /// </summary>
        public virtual DbSet<MyLearningActivitiesViewModel> MyLearningActivitiesViewModel { get; set; }

        /// <summary>
        /// Gets or sets the node resource..
        /// </summary>
        public virtual DbSet<NodeResource> NodeResource { get; set; }

        /// <summary>
        /// Gets or sets the node resource lookup.
        /// </summary>
        public virtual DbSet<NodeResourceLookup> NodeResourceLookup { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy edit.
        /// </summary>
        public virtual DbSet<HierarchyEdit> HierarchyEdit { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy edit detail.
        /// </summary>
        public virtual DbSet<HierarchyEditDetail> HierarchyEditDetail { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy edit detail.
        /// </summary>
        public virtual DbSet<MoveResourceResultViewModel> MoveResourceResultViewModel { get; set; }

        /// <summary>
        /// Gets or sets the CacheOperationViewModel.
        /// </summary>
        public virtual DbSet<CacheOperationViewModel> CacheOperationViewModel { get; set; }

        /// <summary>
        /// Gets or sets the Publication.
        /// </summary>
        public virtual DbSet<Publication> Publication { get; set; }

        /// <summary>
        /// Gets or sets the catalogue location view model..
        /// </summary>
        public virtual DbSet<CatalogueLocationViewModel> CatalogueLocationViewModel { get; set; }

        /// <summary>
        /// Gets or sets the restricted catalogue user view model.
        /// </summary>
        public virtual DbSet<RestrictedCatalogueUserViewModel> RestrictedCatalogueUserViewModel { get; set; }

        /// <summary>
        /// Gets or sets the restricted catalogue access request view model.
        /// </summary>
        public virtual DbSet<RestrictedCatalogueAccessRequestViewModel> RestrictedCatalogueAccessRequestViewModel { get; set; }

        /// <summary>
        /// Gets or sets the restricted catalogue summary view model.
        /// </summary>
        public virtual DbSet<RestrictedCatalogueSummaryViewModel> RestrictedCatalogueSummaryViewModel { get; set; }

        /// <summary>
        /// Gets or sets the catalogue access request .
        /// </summary>
        public virtual DbSet<CatalogueAccessRequest> CatalogueAccessRequest { get; set; }

        /// <summary>
        /// Gets or sets the folder node version.
        /// </summary>
        public virtual DbSet<FolderNodeVersion> FolderNodeVersion { get; set; }

        /// <summary>
        /// Gets or sets the catalogue node version.
        /// </summary>
        public virtual DbSet<CatalogueNodeVersion> CatalogueNodeVersion { get; set; }

        public virtual DbSet<CatalogueNodeVersionProvider> CatalogueNodeVersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the catalogue node version keyword..
        /// </summary>
        public virtual DbSet<CatalogueNodeVersionKeyword> CatalogueNodeVersionKeyword { get; set; }

        /// <summary>
        /// Gets or sets the Migration.
        /// </summary>
        public virtual DbSet<Migration> Migration { get; set; }

        /// <summary>
        /// Gets or sets the migration input record..
        /// </summary>
        public virtual DbSet<MigrationInputRecord> MigrationInputRecord { get; set; }

        /// <summary>
        /// Gets or sets the migration source..
        /// </summary>
        public virtual DbSet<MigrationSource> MigrationSource { get; set; }

        /// <summary>
        /// Gets or sets the Event.
        /// </summary>
        public virtual DbSet<Event> Event { get; set; }

        /// <summary>
        /// Gets or sets the Attribute.
        /// </summary>
        public virtual DbSet<Attribute> Attribute { get; set; }

        /// <summary>
        /// Gets or sets the assessment resource activity..
        /// </summary>
        public virtual DbSet<AssessmentResourceActivity> AssessmentResourceActivity { get; set; }

        /// <summary>
        /// Gets or sets the assessment resource activity interactions..
        /// </summary>
        public virtual DbSet<AssessmentResourceActivityInteraction> AssessmentResourceActivityInteraction { get; set; }

        /// <summary>
        /// Gets or sets the assessment resource activity interaction answer..
        /// </summary>
        public virtual DbSet<AssessmentResourceActivityInteractionAnswer> AssessmentResourceActivityInteractionAnswer { get; set; }

        /// <summary>
        /// Gets or sets the assessment resource activity match question.
        /// </summary>
        public virtual DbSet<AssessmentResourceActivityMatchQuestion> AssessmentResourceActivityMatchQuestion { get; set; }

        /// <summary>
        /// Gets or sets the media resource activity..
        /// </summary>
        public virtual DbSet<MediaResourceActivity> MediaResourceActivity { get; set; }

        /// <summary>
        /// Gets or sets the media resource activity interactions..
        /// </summary>
        public virtual DbSet<MediaResourceActivityInteraction> MediaResourceActivityInteraction { get; set; }

        /// <summary>
        /// Gets or sets the media resource activity type..
        /// </summary>
        public virtual DbSet<MediaResourceActivityType> MediaResourceActivityType { get; set; }

        /// <summary>
        /// Gets or sets the media resource played segments..
        /// </summary>
        public virtual DbSet<MediaResourcePlayedSegment> MediaResourcePlayedSegment { get; set; }

        /// <summary>
        /// Gets or sets the user profile.
        /// </summary>
        public virtual DbSet<UserProfile> UserProfile { get; set; }

        /// <summary>
        /// Gets or sets the email template.
        /// </summary>
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public virtual DbSet<Message> Message { get; set; }

        /// <summary>
        /// Gets or sets the user details.
        /// </summary>
        public virtual DbSet<UserDetails> UserDetails { get; set; }

        /// <summary>
        /// Gets or sets the message meta data.
        /// </summary>
        public virtual DbSet<MessageMetaData> MessageMetaData { get; set; }

        /// <summary>
        /// Gets or sets the FullMessageDto. These are not entities. They are returned from the [messaging].[GetPendingMessages] stored proc.
        /// </summary>
        public virtual DbSet<FullMessageDto> FullMessageDto { get; set; }

        /// <summary>
        /// Gets or sets the notificaton template.
        /// </summary>
        public virtual DbSet<NotificationTemplate> NotificationTemplate { get; set; }

        /// <summary>
        /// Gets or sets the block collection.
        /// </summary>
        public virtual DbSet<BlockCollection> BlockCollection { get; set; }

        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        public virtual DbSet<Block> Block { get; set; }

        /// <summary>
        /// Gets or sets the text block.
        /// </summary>
        public virtual DbSet<TextBlock> TextBlock { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image block.
        /// </summary>
        public virtual DbSet<WholeSlideImageBlock> WholeSlideImageBlock { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image block item.
        /// </summary>
        public virtual DbSet<WholeSlideImageBlockItem> WholeSlideImageBlockItem { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image.
        /// </summary>
        public virtual DbSet<WholeSlideImage> WholeSlideImage { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image annotation.
        /// </summary>
        public virtual DbSet<ImageAnnotation> ImageAnnotation { get; set; }

        /// <summary>
        /// Gets or sets the whole slide image annotation mark.
        /// </summary>
        public virtual DbSet<ImageAnnotationMark> ImageAnnotationMark { get; set; }

        /// <summary>
        /// Gets or sets the media block.
        /// </summary>
        public virtual DbSet<MediaBlock> MediaBlock { get; set; }

        /// <summary>
        /// Gets or sets the question block.
        /// </summary>
        public virtual DbSet<QuestionBlock> QuestionBlock { get; set; }

        /// <summary>
        /// Gets or sets the question answer.
        /// </summary>
        public virtual DbSet<QuestionAnswer> QuestionAnswer { get; set; }

        /// <summary>
        /// Gets or sets the attachment.
        /// </summary>
        public virtual DbSet<Attachment> Attachment { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public virtual DbSet<Image> Image { get; set; }

        /// <summary>
        /// Gets or sets the video.
        /// </summary>
        public virtual DbSet<Video> Video { get; set; }

        /// <summary>
        /// Gets or sets the video file.
        /// </summary>
        public virtual DbSet<VideoFile> VideoFile { get; set; }

        /// <summary>
        /// Gets or sets the ExternalSystem.
        /// </summary>
        public virtual DbSet<ExternalSystem> ExternalSystem { get; set; }

        /// <summary>
        /// Gets or sets the ExternalSystemDeepLink.
        /// </summary>
        public virtual DbSet<ExternalSystemDeepLink> ExternalSystemDeepLink { get; set; }

        /// <summary>
        /// Gets or sets the ExternalSystemUser.
        /// </summary>
        public virtual DbSet<ExternalSystemUser> ExternalSystemUser { get; set; }

        /// <summary>
        /// Gets or sets the Page.
        /// </summary>
        public virtual DbSet<Page> Page { get; set; }

        /// <summary>
        /// Gets or sets the PageSection.
        /// </summary>
        public virtual DbSet<PageSection> PageSection { get; set; }

        /// <summary>
        /// Gets or sets the PageSectionDetail.
        /// </summary>
        public virtual DbSet<PageSectionDetail> PageSectionDetail { get; set; }

        /// <summary>
        /// Gets or sets the VideoAsset.
        /// </summary>
        public virtual DbSet<VideoAsset> VideoAsset { get; set; }


        /// <summary>
        /// Gets or sets UserBookmark.
        /// </summary>
        public virtual DbSet<UserBookmark> UserBookmark { get; set; }

        /// <summary>
        /// Gets or sets the AllCatalogueAlphabet.
        /// </summary>
        public virtual DbSet<AllCatalogueAlphabetModel> AllCatalogueAlphabetModel { get; set; }

        /// <summary>
        /// Gets or sets the AllCatalogueAlphabet.
        /// </summary>
        public virtual DbSet<AllCatalogueViewModel> AllCatalogueViewModel { get; set; }

        /// <summary>
        /// Gets or sets Provider.
        /// </summary>
        public virtual DbSet<Provider> Provider { get; set; }

        /// <summary>
        /// Gets or sets User Provider.
        /// </summary>
        public virtual DbSet<UserProvider> UserProvider { get; set; }

        /// <summary>
        /// Gets or sets Resource Version Provider.
        /// </summary>
        public virtual DbSet<ResourceVersionProvider> ResourceVersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the resource activity.
        /// </summary>
        public virtual DbSet<MyLearningActivity> MyLearningActivity { get; set; }

        /// <summary>
        /// Gets or sets the html resource version.
        /// </summary>
        public virtual DbSet<HtmlResourceVersion> HtmlResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the AssessmentResourceActivityQuestionViewModel.
        /// </summary>
        public virtual DbSet<AssessmentActivityCompletionViewModel> AssessmentActivityCompletionViewModel { get; set; }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder<see cref="ModelBuilder"/>.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var mapping in this.options.Mappings)
            {
                mapping.Map(modelBuilder);
            }
        }
    }
}
