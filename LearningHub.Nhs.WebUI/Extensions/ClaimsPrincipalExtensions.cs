// <copyright file="ClaimsPrincipalExtensions.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Extensions
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Defines the <see cref="ClaimsPrincipalExtensions" />.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get TimezoneOffsetCacheKey.
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal" />.</param>
        /// <returns>Cache key for timezone offset.</returns>
        public static string GetTimezoneOffsetCacheKey(this ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Unable to find logged in user id");
            }

            return $"usr_{userId}_tz";
        }
    }
}