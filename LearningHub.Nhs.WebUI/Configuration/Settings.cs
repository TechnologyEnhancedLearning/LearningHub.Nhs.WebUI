namespace LearningHub.Nhs.WebUI.Configuration
{
    using System;
    using LearningHub.Nhs.WebUI.Models.Contribute;

    /// <summary>
    /// Defines the <see cref="Settings" />.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            // Current = this;
        }

        /// <summary>
        /// Gets or sets the BuildNumber.
        /// </summary>
        public string BuildNumber { get; set; }

        /// <summary>
        /// Gets or sets the KnownProxies.
        /// </summary>
        public string KnownProxies { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the OpenApiUrl.
        /// </summary>
        public string OpenApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the UserApiUrl.
        /// </summary>
        public string UserApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubWebUiUrl.
        /// </summary>
        public string LearningHubWebUiUrl { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubAdminUrl.
        /// </summary>
        public string LearningHubAdminUrl { get; set; }

        /// <summary>
        /// Gets or sets the ELfhHubUrl.
        /// </summary>
        public string ELfhHubUrl { get; set; }

        /// <summary>
        /// Gets or sets the LHClientIdentityKey.
        /// </summary>
        public string LHClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubTenantId.
        /// </summary>
        public int LearningHubTenantId { get; set; }

        /// <summary>
        /// Gets or sets the ELfhHubLHTenantUrl.
        /// </summary>
        public string ELfhHubLHTenantUrl { get; set; }

        /// <summary>
        /// Gets or sets the LogConfigDir.
        /// </summary>
        public string LogConfigDir { get; set; }

        /// <summary>
        /// Gets or sets the TenantCode.
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// Gets or sets the TenantName.
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the TwitterCredentials.
        /// </summary>
        public TwitterSettings TwitterCredentials { get; set; } = new TwitterSettings();

        /// <summary>
        /// Gets or sets the SecurityQuestionsToAsk.
        /// </summary>
        public int SecurityQuestionsToAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Restricted.
        /// </summary>
        public bool Restricted { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaIssuer.
        /// </summary>
        public string AzureMediaIssuer { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaAudience.
        /// </summary>
        public string AzureMediaAudience { get; set; }

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
        /// Gets or sets the AzureFileStorageConnectionString.
        /// </summary>
        public string AzureFileStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the AzureSourceFileStorageConnectionString.
        /// </summary>
        public string AzureSourceArchiveStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the AzurePurgedFileStorageConnectionString.
        /// </summary>
        public string AzureContentArchiveStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the AzureFileStorageResourceShareName.
        /// </summary>
        public string AzureFileStorageResourceShareName { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaJWTPrimaryKeySecret.
        /// </summary>
        public string AzureMediaJWTPrimaryKeySecret { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaJWTTokenExpiryMinutes.
        /// </summary>
        public int AzureMediaJWTTokenExpiryMinutes { get; set; }

        /// <summary>
        /// Gets or sets the ResourceLicenseUrl.
        /// </summary>
        public string ResourceLicenseUrl { get; set; }

        /// <summary>
        /// Gets or sets the GoogleAnalyticsId.
        /// </summary>
        public string GoogleAnalyticsId { get; set; }

        /// <summary>
        /// Gets or sets the PasswordRequestLimitingPeriod.
        /// </summary>
        public int PasswordRequestLimitingPeriod { get; set; }

        /// <summary>
        /// Gets or sets the PasswordRequestLimit.
        /// </summary>
        public int PasswordRequestLimit { get; set; }

        /// <summary>
        /// Gets or sets the SupportUrls.
        /// </summary>
        public SupportUrls SupportUrls { get; set; } = new SupportUrls();

        /// <summary>
        /// Gets or sets the SocialMediaSharingUrls.
        /// </summary>
        public SocialMediaSharingUrls SocialMediaSharingUrls { get; set; } = new SocialMediaSharingUrls();

        /// <summary>
        /// Gets or sets the EnableTempDebugging.
        /// </summary>
        public string EnableTempDebugging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Scorm contributions are limited to admin.
        /// </summary>
        public bool LimitScormToAdmin { get; set; }

        /// <summary>
        /// Gets or sets the FileUploadSettings.
        /// </summary>
        public FileUploadSettingsModel FileUploadSettings { get; set; } = new FileUploadSettingsModel();

        /// <summary>
        /// Gets or sets the MediaActivityPlayingEventIntervalSeconds.
        /// </summary>
        public int MediaActivityPlayingEventIntervalSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Google Tag Manager enabled.
        /// </summary>
        public bool GoogleTagManagerEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Google Tag Manager container ID.
        /// </summary>
        public string GoogleTagManagerContainerId { get; set; }

        /// <summary>
        /// Gets or sets the KeepUserSessionAliveIntervalMins.
        /// </summary>
        public int KeepUserSessionAliveIntervalMins { get; set; }

        /// <summary>
        /// Gets or sets the ScormActivityDurationLimitHours.
        /// </summary>
        public int ScormActivityDurationLimitHours { get; set; }

        /// <summary>
        /// Gets or sets the FindwiseSettings.
        /// </summary>
        public FindwiseSettings FindwiseSettings { get; set; } = new FindwiseSettings();

        /// <summary>
        /// Gets or sets the MediaKindSettings.
        /// </summary>
        public MediaKindSettings MediaKindSettings { get; set; } = new MediaKindSettings();

        /// <summary>
        /// Gets or sets AllCataloguePageSize.
        /// </summary>
        public int AllCataloguePageSize { get; set; }

        /// <summary>
        /// Gets or sets the StatMandId.
        /// </summary>
        public int StatMandId { get; set; }
    }
}