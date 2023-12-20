// <copyright file="OutputSanitizer.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using Ganss.XSS;

    /// <summary>
    /// Defines the <see cref="OutputSanitizer" />.
    /// </summary>
    public class OutputSanitizer
    {
        /// <summary>
        /// The SanitizeOutputHtml.
        /// </summary>
        /// <param name="html">The html<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SanitizeOutputHtml(string html)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("a");
            return sanitizer.Sanitize(html);
        }
    }
}
