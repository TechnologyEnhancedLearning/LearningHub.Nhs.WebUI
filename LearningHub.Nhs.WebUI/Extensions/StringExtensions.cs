// <copyright file="StringExtensions.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Extensions
{
    /// <summary>
    /// Defines the <see cref="StringExtensions" />.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// String Truncate.
        /// </summary>
        /// <param name="value">string value.</param>
        /// <param name="maxLength">Length to truncate.</param>
        /// <param name="addEllipsis">Add ellipsis.</param>
        /// <returns>new truncated string.</returns>
        public static string Truncate(this string value, int maxLength, bool addEllipsis = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + (addEllipsis ? (char)0x2026 : string.Empty);
        }
    }
}
