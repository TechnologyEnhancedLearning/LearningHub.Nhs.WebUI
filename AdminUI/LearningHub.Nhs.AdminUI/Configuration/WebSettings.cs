namespace LearningHub.Nhs.AdminUI.Configuration
{
    using System;

    /// <summary>
    /// The web settings.
    /// </summary>
    public class WebSettings
    {
        /// <summary>
        /// Gets or sets the BuildNumber.
        /// </summary>
        public string BuildNumber { get; set; }

        /// <summary>
        /// Gets or sets the learning hub url.
        /// </summary>
        public string LearningHubUrl { get; set; }

        /// <summary>
        /// Gets or sets the e lfh hub url.
        /// </summary>
        public string ELfhHubUrl { get; set; }

        /// <summary>
        /// Gets or sets the learning hub api url.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the user api url.
        /// </summary>
        public string UserApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the learning hub admin url.
        /// </summary>
        public string LearningHubAdminUrl { get; set; }

        /// <summary>
        /// Gets or sets the log config dir.
        /// </summary>
        public string LogConfigDir { get; set; }

        /// <summary>
        /// Gets or sets the authentication service url.
        /// </summary>
        public string AuthenticationServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the default page size.
        /// </summary>
        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Gets or sets the google analytics id.
        /// </summary>
        public string GoogleAnalyticsId { get; set; }

        /// <summary>
        /// Gets or sets the learning hub secret.
        /// </summary>
        public string LearningHubSecret { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the auth timeout.
        /// </summary>
        public int AuthTimeout { get; set; }

        /// <summary>
        /// Gets or sets the azure file storage connection string.
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
        /// Gets or sets the azure file storage resource share name.
        /// </summary>
        public string AzureFileStorageResourceShareName { get; set; }

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
        /// Gets or sets the AzureMediaJWTPrimaryKeySecret.
        /// </summary>
        public string AzureMediaJWTPrimaryKeySecret { get; set; }

        /// <summary>
        /// Gets or sets the AzureMediaJWTTokenExpiryMinutes.
        /// </summary>
        public int AzureMediaJWTTokenExpiryMinutes { get; set; }

        /// <summary>
        /// Gets or sets the support form url.
        /// </summary>
        public string SupportForm { get; set; }

        /// <summary>
        /// Gets or sets the FileUploadSettings.
        /// </summary>
        public FileUploadSettingsModel FileUploadSettings { get; set; } = new FileUploadSettingsModel();

        /// <summary>
        /// Gets or sets the MediaKindSettings.
        /// </summary>
        public MediaKindSettings MediaKindSettings { get; set; } = new MediaKindSettings();
    }
}