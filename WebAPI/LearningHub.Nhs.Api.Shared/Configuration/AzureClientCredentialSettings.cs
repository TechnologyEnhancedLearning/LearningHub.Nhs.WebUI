// <copyright file="AzureClientCredentialSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The AzureClientCredentialSettings.
    /// </summary>
    public class AzureClientCredentialSettings
    {
        /// <summary>
        /// Gets or sets the AadClientId.
        /// </summary>
        public string AadClientId { get; set; }

        /// <summary>
        /// Gets or sets the AadSecret.
        /// </summary>
        public string AadSecret { get; set; }

        /// <summary>
        /// Gets or sets the TenantId.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets SubscriptionId.
        /// </summary>
        public string SubscriptionId { get; set; }
    }
}
