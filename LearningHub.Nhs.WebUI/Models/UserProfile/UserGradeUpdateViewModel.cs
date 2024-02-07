// <copyright file="UserGradeUpdateViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="UserGradeUpdateViewModel" />.
    /// </summary>
    public class UserGradeUpdateViewModel
    {
        /// <summary>
        /// Gets or sets the selected job role id.
        /// </summary>
        public int SelectedJobRoleId { get; set; }

        /// <summary>
        /// Gets or sets the selected job role.
        /// </summary>
        public string SelectedJobRole { get; set; }

        /// <summary>
        /// Gets or sets the medical council id.
        /// </summary>
        public int? SelectedMedicalCouncilId { get; set; }

        /// <summary>
        /// Gets or sets the selected medical council number.
        /// </summary>
        public string SelectedMedicalCouncilNo { get; set; }

        /// <summary>
        /// Gets or sets grade.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the selected grade id.
        /// </summary>
        public int? SelectedGradeId { get; set; }

        /// <summary>
        /// Gets or sets the grade list.
        /// </summary>
        public List<GenericListViewModel> GradeList { get; set; }

        /// <summary>
        /// sets the list of radio Grade.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> GradeRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.GradeList.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.GradeList
                           let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.Name, false, null)
                           select newRadio);
            return radio;
        }
    }
}
