using System;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// <see cref="LearningHubConfig"/>.
    /// </summary>
    public class LearningHubConfig
    {
        /// <summary>
        /// Gets or sets <see cref="BaseUrl"/>.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="LaunchResourceEndpoint"/>.
        /// </summary>
        public string LaunchResourceEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ContentServerUrl"/>.
        /// </summary>
        public string ContentServerUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="EmailConfirmationTokenExpiryMinutes"/>.
        /// </summary>
        public int EmailConfirmationTokenExpiryMinutes { get; set; } = 0;

        /// <summary>
        /// Gets or sets <see cref="LearningHubTenantId"/>.
        /// </summary>
        public int LearningHubTenantId { get; set; } = 0;

        /// <summary>
        /// Gets or sets <see cref="SupportPages"/>.
        /// </summary>
        public string SupportPages { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="SupportForm"/>.
        /// </summary>
        public string SupportForm { get; set; } = null!;

        /// <summary>
        ///  Gets or sets a value indicating whether <see cref="UseRedisCache"/>.
        /// </summary>
        public bool UseRedisCache { get; set; } = false;

        /// <summary>
        /// Gets or sets <see cref="MaxDatabaseRetryAttempts"/>.
        /// </summary>
        public int MaxDatabaseRetryAttempts {  get; set; } = 0;

        /// <summary>
        /// Gets or sets <see cref="HierarchyEditPublishQueueName"/>.
        /// </summary>
        public string HierarchyEditPublishQueueName { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ResourcePublishQueueRouteName"/>.
        /// </summary>
        public string ResourcePublishQueueRouteName { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ContentManagementQueueName"/>.
        /// </summary>
        public string ContentManagementQueueName { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="DetailedMediaActivityRecordingStartDate"/>.
        /// </summary>
        public DateTimeOffset DetailedMediaActivityRecordingStartDate { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Notifications"/>.
        /// </summary>
        public NotificationSetting Notifications { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="MyContributionsUrl"/>.
        /// </summary>
        public string MyContributionsUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="MyLearningUrl"/>.
        /// </summary>
        public string MyLearningUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="MyBookmarksUrl"/>.
        /// </summary>
        public string MyBookmarksUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="SearchUrl"/>.
        /// </summary>
        public string SearchUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="AdminUrl"/>.
        /// </summary>
        public string AdminUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ForumsUrl"/>.
        /// </summary>
        public string ForumsUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="HelpUrl"/>.
        /// </summary>
        public string HelpUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="MyRecordsUrl"/>.
        /// </summary>
        public string MyRecordsUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="NotificationsUrl"/>.
        /// </summary>
        public string NotificationsUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="RegisterUrl"/>.
        /// </summary>
        public string RegisterUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="SignOutUrl"/>.
        /// </summary>
        public string SignOutUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="MyAccountUrl"/>.
        /// </summary>
        public string MyAccountUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="BrowseCataloguesUrl"/>.
        /// </summary>
        public string BrowseCataloguesUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ReportUrl"/>.
        /// </summary>
        public string ReportUrl { get; set; } = null!;


        /// <summary>
        /// Gets or sets <see cref="AuthClientIdentityKey"/>.
        /// </summary>
        public string AuthClientIdentityKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="LHClientIdentityKey"/>.
        /// </summary>
        public string LHClientIdentityKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ContentServerClientIdentityKey"/>.
        /// </summary>
        public string ContentServerClientIdentityKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="ReportApiClientIdentityKey"/>.
        /// </summary>
        public string ReportApiClientIdentityKey { get; set; } = null!;
    }
}
