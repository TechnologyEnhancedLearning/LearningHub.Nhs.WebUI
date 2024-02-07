// <copyright file="UserRoleHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Helpers
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Defines the <see cref="UserRoleHelper" />.
    /// </summary>
    public static class UserRoleHelper
    {
        /// <summary>
        /// Get role label.
        /// </summary>
        /// <param name="roleName">The rolename<see cref="string"/>..</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetRoleLabel(string roleName)
        {
            return roleName.ToLower() switch
            {
                "administrator" => "Administrator",
                "blueuser" => "FullAccess",
                "basicuser" => "General",
                "readonly" => "ReadOnly",
                _ => string.Empty,
            };
        }
    }
}
