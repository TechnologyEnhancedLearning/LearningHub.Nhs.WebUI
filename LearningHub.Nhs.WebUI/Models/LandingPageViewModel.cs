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
        public string NoJSSearchText { get; set; } = "MVC set but really would start blank";
        public string NoJSActionUrl { get; set; } = "/nojsformsubmitstubpage";
        public List<string> NoJSSearchResults { get; set; } = new List<string>() { "!!Set by mvc but really would start blank!!", "In a nojs browser", " the user would first get a blanks components ", " they then send their data", " the Controller would provide", "this static info back", "including original provideds search", "having used the same service", "serverside in its controller" };
        public List<string> NoJSSuggestions { get; set; } = new List<string>() { "!!Set by mvc but really would start blank!!", "Unsure if this would work ", "it might do but we wouldnt", " want to spam a page reload per button press ", " so maybe there would be different suggestions in nojs" };


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

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the DisplayAVFromAMS.
        /// </summary>
        public bool DisplayAudioVideo { get; set; }

        /// <summary>
        /// Gets or sets the media kind media kind MKPlayer licence key.
        /// </summary>
        public string MKPlayerLicence { get; set; }
    }
}