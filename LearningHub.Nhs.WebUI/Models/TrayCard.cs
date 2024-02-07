// <copyright file="TrayCard.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="TrayCard" />.
    /// </summary>
    public class TrayCard
    {
        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the HierarchyId.
        /// </summary>
        public int HierarchyId { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Publisher.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the ResourceSubType.
        /// </summary>
        public string ResourceSubType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the Time.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        public ResourceTypeEnum Type { get; set; }
    }
}
