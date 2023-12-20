// <copyright file="ResourceAccessiblityHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Defines the <see cref="ResourceAccessiblityHelper" />.
    /// </summary>
    public static class ResourceAccessiblityHelper
    {
        /// <summary>
        /// Get resource accessiblity access level text.
        /// </summary>
        /// <param name="resourceAccessibilityEnum">The htmlhelper<see cref="IHtmlHelper"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetResourceAccessibilityLevelText(this ResourceAccessibilityEnum resourceAccessibilityEnum)
        {
            return resourceAccessibilityEnum switch
            {
                ResourceAccessibilityEnum.PublicAccess => "Public user",
                ResourceAccessibilityEnum.FullAccess => "Full user",
                ResourceAccessibilityEnum.GeneralAccess => "General user",
                ResourceAccessibilityEnum.None => "None",
                _ => string.Empty,
            };
        }
    }
}
