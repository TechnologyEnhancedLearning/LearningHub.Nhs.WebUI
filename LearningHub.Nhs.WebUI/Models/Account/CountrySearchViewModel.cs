namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System.Collections.Generic;
    using NHSUKFrontendRazor.ViewModels;

    /// <summary>
    /// The CountrySearchViewModel.
    /// </summary>
    public class CountrySearchViewModel
  {
    /// <summary>
    /// Gets or sets the filterText.
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

    /// <summary>
    /// Gets or sets selectedQuestion.
    /// </summary>
    public string SelectedCountry { get; set; }

    /// <summary>
    /// Gets or sets selectedQuestion.
    /// </summary>
    public string SelectedRegion { get; set; }
  }
}
