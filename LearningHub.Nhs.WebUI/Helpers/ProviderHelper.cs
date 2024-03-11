namespace LearningHub.Nhs.WebUI.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ProviderHelper" />.
    /// </summary>
    public class ProviderHelper
    {
        /// <summary>
        /// The GetProviders string.
        /// </summary>
        /// <param name="providerNames">list of provider name.</param>
        /// <returns>string.</returns>
        public static string GetProvidersString(List<string> providerNames)
        {
            var providerstring = string.Empty;

            if (providerNames != null && providerNames.Count > 0)
            {
                providerstring += "Developed with ";
                providerstring += string.Join(", ", providerNames.ToArray());
            }

            return providerstring;
        }

        /// <summary>
        /// The GetProvider string.
        /// </summary>
        /// <param name="providerName">provider name.</param>
        /// <returns>string.</returns>
        public static string GetProviderString(string providerName)
        {
            return GetProvidersString(new List<string> { providerName });
        }
    }
}
