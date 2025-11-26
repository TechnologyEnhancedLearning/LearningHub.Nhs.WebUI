using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// Gets or sets NotificationSetting.
    /// </summary>
    public class NotificationSetting
    {
        /// <summary>
        /// Gets or sets the PublishResourceTimeToProcessInSec.
        /// </summary>
        public int PublishResourceTimeToProcessInSec { get; set; }

        /// <summary>
        /// Gets or sets the ResourcePublishedTitle.
        /// </summary>
        public string ResourcePublishedTitle { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourcePublished.
        /// </summary>
        public string ResourcePublished { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourcePublishFailedTitle.
        /// </summary>
        public string ResourcePublishFailedTitle { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourcePublishFailed.
        /// </summary>
        public string ResourcePublishFailed { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourcePublishFailedWithReason.
        /// </summary>
        public string ResourcePublishFailedWithReason { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourceAccessTitle.
        /// </summary>
        public string ResourceAccessTitle { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourceReadonlyAccess.
        /// </summary>
        public string ResourceReadonlyAccess { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ResourceContributeAccess.
        /// </summary>
        public string ResourceContributeAccess { get; set; } = null!;

        /// <summary>
        /// Gets or sets the report title notification content.
        /// </summary>
        public string ReportTitle { get; set; } = null!;

        /// <summary>
        /// Gets or sets the report notification content.
        /// </summary>
        public string Report { get; set; } = null!;
    }
}
