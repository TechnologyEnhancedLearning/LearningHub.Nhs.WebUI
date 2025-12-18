namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using NHSUKFrontendRazor.ViewModels;

    /// <summary>
    /// Defines the <see cref="UserLocationViewModel" />.
    /// </summary>
    public class UserLocationViewModel
    {
        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [Required(ErrorMessage = "Select a country.")]
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
        /// Gets or sets serach text.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the Region.
        /// </summary>
        public List<RadiosItemViewModel> Region { get; set; }

        /// <summary>
        /// Gets or sets the Region.
        /// </summary>
        public List<RadiosItemViewModel> Country { get; set; }
    }
}
