namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="MyAccountLocationViewModel" />.
    /// </summary>
    public class MyAccountLocationViewModel
    {
        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [DisplayName("Country")]
        public int? SelectedCountryId { get; set; }

        /// <summary>
        /// Gets or sets the region id.
        /// </summary>
        [DisplayName("Region")]
        public int? SelectedRegionId { get; set; }

        /// <summary>
        /// Gets or sets selected country name.
        /// </summary>
        public string SelectedCountryName { get; set; }

        /// <summary>
        /// Gets or sets selected region name.
        /// </summary>
        public string SelectedRegionName { get; set; }

        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        public int? SelectedOtherCountryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SelectedOtherCountry.
        /// </summary>
        public bool HasSelectedOtherCountry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SelectedRegion.
        /// </summary>
        public bool HasSelectedRegion { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public List<RadiosItemViewModel> Country { get; set; }

        /// <summary>
        /// Gets or sets the OtherCountryOptions.
        /// </summary>
        public IEnumerable<SelectListItem> OtherCountryOptions { get; set; }

        /// <summary>
        /// Gets or sets the RegionOptions.
        /// </summary>
        public IEnumerable<SelectListItem> RegionOptions { get; set; }
    }
}
