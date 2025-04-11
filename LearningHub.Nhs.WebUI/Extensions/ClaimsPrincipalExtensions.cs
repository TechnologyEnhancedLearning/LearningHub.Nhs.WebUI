namespace LearningHub.Nhs.WebUI.Extensions
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;

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

        /// <summary>
        /// Get MoodleUserId.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>The System.Int32.</returns>
        public static int GetMoodleUserId(this IIdentity identity)
        {
            Claim claim = (identity as ClaimsIdentity)?.FindFirst("moodle_username");
            if (claim != null)
            {
                return int.Parse(claim.Value);
            }

            return 0;
        }
    }
}