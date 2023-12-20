// <copyright file="DashboardEventViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="DashboardEventViewModel" />.
    /// </summary>
    public class DashboardEventViewModel
    {
        /// <summary>
        /// Gets or sets the EventType.
        /// </summary>
        public EventTypeEnum EventType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceReference.
        /// </summary>
        public int? ResourceReference { get; set; }
    }
}
