// <copyright file="DisplayMessageViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// Defines the <see cref="DisplayMessageViewModel" />.
    /// </summary>
    public class DisplayMessageViewModel
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Message HTML to display. Can include HTML tags/formatting if needed.
        /// </summary>
        public string MessageHtml { get; set; }

        /// <summary>
        /// Gets or sets the ButtonText.
        /// </summary>
        public string ButtonText { get; set; } = "OK";

        /// <summary>
        /// Gets or sets the ButtonCssClasses. Multiple class names can be used, separate with a space.
        /// </summary>
        public string ButtonCssClasses { get; set; } = "btn-custom";

        /// <summary>
        /// Gets or sets the ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
