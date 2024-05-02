namespace LearningHub.Nhs.WebUI.Configuration
{
    /// <summary>
    /// Config AzureMediaSettings.
    /// </summary>
    public class MediaKindSettings
    {
        /// <summary>
        /// Gets or sets subscription name.
        /// </summary>
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Gets or sets token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets storage media account name.
        /// </summary>
        public string StorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets media kind media service issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets media kind media service audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the contentkey policyname.
        /// </summary>
        public string ContentKeyPolicyName { get; set; }

        /// <summary>
        /// Gets or sets media kind media service jwt primary key secret.
        /// </summary>
        public string JWTPrimaryKeySecret { get; set; }
    }
}
