// <copyright file="Settings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    using System;

    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the log config dir.
        /// </summary>
        public string LogConfigDir { get; set; }

        /// <summary>
        /// Gets or sets the authentication service url.
        /// </summary>
        public string AuthenticationServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the max logon attempts.
        /// </summary>
        public int MaxLogonAttempts { get; set; }

        /// <summary>
        /// Gets or sets the learning hub tenant id.
        /// </summary>
        public int LearningHubTenantId { get; set; }

        /// <summary>
        /// Gets or sets the auth client identity key.
        /// </summary>
        public string AuthClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the lh client identity key.
        /// </summary>
        public string LHClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the ContentServerClientIdentityKey.
        /// </summary>
        public string ContentServerClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the ReportApiClientIdentityKey.
        /// </summary>
        public string ReportApiClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the learning hub url.
        /// </summary>
        public string LearningHubUrl { get; set; }

        /// <summary>
        /// Gets or sets the learning hub url.
        /// </summary>
        public string LearningHubContentServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the security questions required.
        /// </summary>
        public int SecurityQuestionsRequired { get; set; }

        /// <summary>
        /// Gets or sets the website emails to.
        /// </summary>
        public string WebsiteEmailsTo { get; set; }

        /// <summary>
        /// Gets or sets the AzureFileStorageConnectionString.
        /// </summary>
        public string AzureFileStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the AzureFileStorageResourceShareName.
        /// </summary>
        public string AzureFileStorageResourceShareName { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaSubscriptionId.
        /// </summary>
        public string AzureMediaSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaResourceGroup.
        /// </summary>
        public string AzureMediaResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaAccountName.
        /// </summary>
        public string AzureMediaAccountName { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaArmEndpoint.
        /// </summary>
        public Uri AzureMediaArmEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaAadTenantId.
        /// </summary>
        public string AzureMediaAadTenantId { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaAadClientId.
        /// </summary>
        public string AzureMediaAadClientId { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaAadSecret.
        /// </summary>
        public string AzureMediaAadSecret { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob settings.
        /// </summary>
        public AzureBlobSettings AzureBlobSettings { get; set; }

        /// <summary>
        /// Gets or sets the AzureClientCredentials settings.
        /// </summary>
        public AzureClientCredentialSettings AzureClientCredentials { get; set; }

        /// <summary>
        /// Gets or sets the Migration Tool settings.
        /// </summary>
        public MigrationToolSettings MigrationTool { get; set; }

        /// <summary>
        /// Gets or sets the findwise settings.
        /// </summary>
        public FindwiseSettings Findwise { get; set; }

        /// <summary>
        /// Gets or sets the azure storage account connectionstring for queue.
        /// </summary>
        public string AzureStorageQueueConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage ResourcePublishQueueRouteName.
        /// </summary>
        public string ResourcePublishQueueRouteName { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage HierarchyEditPublishQueueName.
        /// </summary>
        public string HierarchyEditPublishQueueName { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage WholeSlideImageProcessingQueueRouteName.
        /// </summary>
        public string WholeSlideImageProcessingQueueRouteName { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage JavaWholeSlideImageProcessingQueueRouteName.
        /// </summary>
        public string JavaWholeSlideImageProcessingQueueRouteName { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage VideoProcessingQueueRouteName.
        /// </summary>
        public string VideoProcessingQueueRouteName { get; set; }

        /// <summary>
        /// Gets or sets the azure queue storage ContentManagementQueueName.
        /// </summary>
        public string ContentManagementQueueName { get; set; }

        /// <summary>
        /// Gets or sets the special search characters.
        /// </summary>
        public string SpecialSearchCharacters { get; set; }

        /// <summary>
        /// Gets or sets the findwise settings.
        /// </summary>
        public NotificationSettings Notifications { get; set; }

        /// <summary>
        /// Gets or sets the support contact url.
        /// </summary>
        public string SupportContact { get; set; }

        /// <summary>
        /// Gets or sets the date on which detailed media activity recording started to take place.
        /// </summary>
        public DateTimeOffset DetailedMediaActivityRecordingStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the Redis Cache.
        /// </summary>
        public bool UseRedisCache { get; set; }

        /// <summary>
        /// Gets or sets the ELFH cache settings.
        /// </summary>
        public ElfhCacheSettings ElfhCacheSettings { get; set; }

        /// <summary>
        /// Gets or sets emailConfirmationTokenExpiryMinutes.
        /// </summary>
        public int EmailConfirmationTokenExpiryMinutes { get; set; }

        /// <summary>
        /// Gets or sets support pages.
        /// </summary>
        public string SupportPages { get; set; }

        /// <summary>
        /// Gets or sets support form.
        /// </summary>
        public string SupportForm { get; set; }
    }
}
