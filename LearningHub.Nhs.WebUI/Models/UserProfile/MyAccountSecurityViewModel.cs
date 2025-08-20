namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System;

    /// <summary>
    /// Defines the <see cref="UserProfileSummaryViewModel" />.
    /// </summary>
    public class MyAccountSecurityViewModel
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the SecondaryEmailAddress.
        /// </summary>
        public string SecondaryEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the SecurityFirstQuestion.
        /// </summary>
        public string SecurityFirstQuestion { get; set; }

        /// <summary>
        /// Gets or sets the SecuritySecondQuestion.
        /// </summary>
        public string SecuritySecondQuestion { get; set; }

        /////// <summary>
        /////// Gets or sets the LastUpdated.
        /////// </summary>
        ////public DateTimeOffset LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the PasswordHash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the SecurityQuestionLastUpdated.
        /// </summary>
        public string SecurityQuestionLastUpdated { get; set; }
    }
}
