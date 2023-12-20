// <copyright file="NotificationSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The Notification Settings.
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Gets or sets the publish resource time to process in seconds.
        /// </summary>
        public int PublishResourceTimeToProcessInSec { get; set; }

        /// <summary>
        /// Gets or sets the resource published notification title.
        /// </summary>
        public string ResourcePublishedTitle { get; set; }

        /// <summary>
        /// Gets or sets the resource published notification content.
        /// </summary>
        public string ResourcePublished { get; set; }

        /// <summary>
        /// Gets or sets the resource publish failed notification title.
        /// </summary>
        public string ResourcePublishFailedTitle { get; set; }

        /// <summary>
        /// Gets or sets the resource publish failed notification content.
        /// </summary>
        public string ResourcePublishFailed { get; set; }

        /// <summary>
        /// Gets or sets the resource publish failed notification content with reason.
        /// </summary>
        public string ResourcePublishFailedWithReason { get; set; }

        /// <summary>
        /// Gets or sets the resource access notification title.
        /// </summary>
        public string ResourceAccessTitle { get; set; }

        /// <summary>
        /// Gets or sets the resource readonly access notification content.
        /// </summary>
        public string ResourceReadonlyAccess { get; set; }

        /// <summary>
        /// Gets or sets the resource contribute access notification content.
        /// </summary>
        public string ResourceContributeAccess { get; set; }
    }
}