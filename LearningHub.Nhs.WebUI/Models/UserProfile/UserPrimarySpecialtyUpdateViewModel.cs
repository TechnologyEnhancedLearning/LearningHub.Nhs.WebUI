namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="UserPrimarySpecialtyUpdateViewModel" />.
    /// </summary>
    public class UserPrimarySpecialtyUpdateViewModel : PagingViewModel
    {
        /// <summary>
        /// Gets or sets primary specialty.
        /// </summary>
        public string PrimarySpecialty { get; set; }

        /// <summary>
        /// Gets or sets filter text.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the selected primary specialty id.
        /// </summary>
        public string SelectedPrimarySpecialtyId { get; set; }

        /// <summary>
        /// Gets or sets the SpecialtyList.
        /// </summary>
        public List<GenericListViewModel> SpecialtyList { get; set; }

        /// <summary>
        /// Gets or sets the optional SpecialtyList.
        /// </summary>
        public GenericListViewModel OptionalSpecialtyItem { get; set; }

        /// <summary>
        /// Gets or sets the selected job role id.
        /// </summary>
        public int? SelectedJobRoleId { get; set; }

        /// <summary>
        /// Gets or sets the selected grade id.
        /// </summary>
        public string SelectedGradeId { get; set; }

        /// <summary>
        /// Gets or sets the selected medical council number.
        /// </summary>
        public string SelectedMedicalCouncilNo { get; set; }

        /// <summary>
        /// sets the list of radio specialty.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> SpecialtyRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (this.SpecialtyList != null)
            {
                radio.AddRange(from k in this.SpecialtyList
                               let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.Name, false, null)
                               select newRadio);
            }

            if (this.OptionalSpecialtyItem != null)
            {
                radio.Add(new RadiosItemViewModel(this.OptionalSpecialtyItem.Id.ToString(), this.OptionalSpecialtyItem.Name, false, null));
            }

            return radio;
        }
    }
}
