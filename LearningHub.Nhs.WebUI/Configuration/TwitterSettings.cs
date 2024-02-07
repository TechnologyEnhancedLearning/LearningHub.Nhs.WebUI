// <copyright file="TwitterSettings.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Configuration
{
    /// <summary>
    /// Defines the <see cref="TwitterSettings" />.
    /// </summary>
    public class TwitterSettings
    {
        /// <summary>
        /// Gets or sets the AccessToken.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the AccessTokenSecret.
        /// </summary>
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerKey.
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerSecret.
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets or sets the ScreenName.
        /// </summary>
        public string ScreenName { get; set; }
    }
}
