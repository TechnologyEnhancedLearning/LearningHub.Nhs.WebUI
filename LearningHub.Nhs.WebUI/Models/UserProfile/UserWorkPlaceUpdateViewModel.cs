namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using NHSUKFrontendRazor.ViewModels;

    /// <summary>
    /// Defines the <see cref="UserWorkPlaceUpdateViewModel" />.
    /// </summary>
    public class UserWorkPlaceUpdateViewModel : PagingViewModel
    {
        /// <summary>
        /// Gets or sets work place.
        /// </summary>
        public string WorkPlace { get; set; }

        /// <summary>
        /// Gets or sets filter text.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the selected work place id.
        /// </summary>
        public string SelectedWorkPlaceId { get; set; }

        /// <summary>
        /// Gets or sets the WorkPlaceList.
        /// </summary>
        public List<LocationBasicViewModel> WorkPlaceList { get; set; }

        /// <summary>
        /// sets the list of radio workPlace.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> WorkPlaceRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.WorkPlaceList.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.WorkPlaceList
                           let newRadio = new RadiosItemViewModel(k.Id.ToString(), $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(k.Name.ToLower())} {k.NhsCode}", false, null)
                           select newRadio);
            return radio;
        }
    }
}
