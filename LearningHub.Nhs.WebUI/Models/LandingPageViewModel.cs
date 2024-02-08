namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Content;

    /// <summary>
    /// Defines the <see cref="LandingPageViewModel" />.
    /// </summary>
    public class LandingPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LandingPageViewModel"/> class.
        /// </summary>
        public LandingPageViewModel()
        {
            this.LoginInput = new LoginInputModel();
        }

        /// <summary>
        /// Gets or sets a value indicating whether AuthenticatedNotAuthorised.
        /// </summary>
        public bool AuthenticatedNotAuthorised { get; set; }

        /// <summary>
        /// Gets the LoginInput.
        /// </summary>
        public LoginInputModel LoginInput { get; }

        /// <summary>
        /// Gets or sets the TenantDescription.
        /// </summary>
        public string TenantDescription { get; set; }

        /// <summary>
        /// Gets or sets the TenantUrl.
        /// </summary>
        public string TenantUrl { get; set; }

        /// <summary>
        /// Gets or sets the PageViewModel.
        /// </summary>
        public PageViewModel PageViewModel { get; set; }

        /// <summary>
        /// Gets or sets get or sets the pageSectionDetailViewModels.
        /// </summary>
        public List<PageSectionDetailViewModel> PageSectionDetailViewModels { get; set; }
    }
}