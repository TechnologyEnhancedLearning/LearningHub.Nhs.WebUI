namespace LearningHub.Nhs.WebUI.Helpers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    /// <summary>
    /// Defines the <see cref="AccountCreationFormHelper" />.
    /// </summary>
    public static class AccountCreationFormHelper
    {
        /// <summary>
        /// The PopulateGroupedFormControlMetadata.
        /// </summary>
        /// <param name="viewData">viewData.</param>
        public static void PopulateGroupedFormControlMetadata(ViewDataDictionary viewData)
        {
            viewData["GroupedFormControlMetadata"] = new Dictionary<string, bool>
            {
                { "CountryId", true },
                { "RegionId", true },
                { "CurrentRole", true },
                { "PrimarySpecialtyId", true },
                { "GradeId", true },
                { "LocationId", true },
            };
        }
    }
}
