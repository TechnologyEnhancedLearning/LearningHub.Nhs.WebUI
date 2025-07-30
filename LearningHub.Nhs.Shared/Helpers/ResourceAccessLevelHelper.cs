namespace LearningHub.Nhs.Shared.Helpers
{
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="ResourceAccessLevelHelper" />.
    /// </summary>
    public static class ResourceAccessLevelHelper
    {
        /// <summary>
        /// Get resource access level text.
        /// </summary>
        /// <param name="resourceAccessibilityEnum">The enum value to get display text for.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetResourceAccessLevelText(this ResourceAccessibilityEnum resourceAccessibilityEnum)
        {
            return resourceAccessibilityEnum switch
            {
                ResourceAccessibilityEnum.PublicAccess => "Public user",
                ResourceAccessibilityEnum.FullAccess => "Full user",
                ResourceAccessibilityEnum.GeneralAccess => "General user",
                ResourceAccessibilityEnum.None => "None",
                _ => "Full user",
            };
        }

        /// <summary>
        /// Returns a prettified resource access level name, suitable for display in the UI.
        /// </summary>
        /// <param name="resourceAccessLevel">The resource type.</param>
        /// <returns>The resource access level name, and duration if applicable.</returns>
        public static string GetPrettifiedResourceAccessLevelOptionDisplayName(this ResourceAccessibilityEnum resourceAccessLevel)
        {
            switch (resourceAccessLevel)
            {
                case ResourceAccessibilityEnum.None:
                    return "All resources";
                default:
                    return "only resources I can access";
            }
        }
    }
}
