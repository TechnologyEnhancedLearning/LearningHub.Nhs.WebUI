// <copyright file="UserBasicViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System;

    /// <summary>
    /// The UserBasicViewModel.
    /// </summary>
    public class UserBasicViewModel
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }
    }
}
