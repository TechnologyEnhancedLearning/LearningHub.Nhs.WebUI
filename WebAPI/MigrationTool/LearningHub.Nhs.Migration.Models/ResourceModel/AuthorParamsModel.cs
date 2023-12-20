// <copyright file="AuthorParamsModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    /// <summary>
    /// Provides the standard parameters required for creating ResourceVersionAuthors via the MigrationService.CreateMetadataAsync method.
    /// Referenced by ResourceParamsModel.cs.
    /// </summary>
    public class AuthorParamsModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether to set the AuthorUserId and auto-populate the AuthorName from the user's details.
        /// </summary>
        public bool IAmTheAuthor { get; set; }

        /// <summary>
        /// Gets or sets the Author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the Organisation.
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        public string Role { get; set; }
    }
}
