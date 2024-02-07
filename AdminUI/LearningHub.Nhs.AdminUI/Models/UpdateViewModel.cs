// <copyright file="UpdateViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="UpdateViewModel" />.
    /// </summary>
    public class UpdateViewModel
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [Required]
        [MaxLength(8000)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ImageName.
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the OrderNumber.
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Published.
        /// </summary>
        [Required]
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the RoadmapDate.
        /// </summary>
        [Required]
        [Display(Name = "Date")]
        public DateTimeOffset? RoadmapDate { get; set; }

        /// <summary>
        /// Gets the RoadmapTypeId.
        /// </summary>
        public int RoadmapTypeId => 1;

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [Required]
        [MaxLength(400)]
        public string Title { get; set; }
    }
}
