namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// MoodleUserResponseViewModel.
    /// </summary>
    public class MoodleUserResponseViewModel
    {
        /// <summary>
        /// Gets or sets the list of users.
        /// </summary>
        public List<MoodleUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        public List<object> Warnings { get; set; }

        /// <summary>
        /// MoodleUser.
        /// </summary>
        public class MoodleUser
        {
            /// <summary>
            /// Gets or sets the user ID.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the username.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// Gets or sets the full name.
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets the department.
            /// </summary>
            public string Department { get; set; }

            /// <summary>
            /// Gets or sets the first access timestamp.
            /// </summary>
            public long FirstAccess { get; set; }

            /// <summary>
            /// Gets or sets the last access timestamp.
            /// </summary>
            public long LastAccess { get; set; }

            /// <summary>
            /// Gets or sets the authentication method.
            /// </summary>
            public string Auth { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the user is suspended.
            /// </summary>
            public bool Suspended { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the user is confirmed.
            /// </summary>
            public bool Confirmed { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Lang { get; set; }

            /// <summary>
            /// Gets or sets the theme.
            /// </summary>
            public string Theme { get; set; }

            /// <summary>
            /// Gets or sets the timezone.
            /// </summary>
            public string Timezone { get; set; }

            /// <summary>
            /// Gets or sets the mail format.
            /// </summary>
            public int MailFormat { get; set; }

            /// <summary>
            /// Gets or sets the forum tracking preference.
            /// </summary>
            public int TrackForums { get; set; }

            /// <summary>
            /// Gets or sets the small profile image URL.
            /// </summary>
            public string ProfileImageUrlSmall { get; set; }

            /// <summary>
            /// Gets or sets the profile image URL.
            /// </summary>
            public string ProfileImageUrl { get; set; }
        }
    }
}
