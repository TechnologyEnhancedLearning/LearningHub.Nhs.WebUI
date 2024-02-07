// <copyright file="CurrentPasswordViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="CurrentPasswordViewModel" />.
    /// </summary>
    public class CurrentPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Current Password.
        /// </summary>
        [Required(ErrorMessage = "Enter a valid password.")]
        public string CurrentPassword { get; set; }
    }
}
