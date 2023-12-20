// <copyright file="ContactViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Models
{
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.Models.Binders;

    /// <summary>
    /// Defines the <see cref="ContactViewModel" />.
    /// </summary>
    public class ContactViewModel
    {
        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [SanitizedHtmlModelBinder]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public int UserId { get; set; }
    }
}
