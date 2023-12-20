// <copyright file="UserJobRoleUpdateViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="UserJobRoleUpdateViewModel" />.
    /// </summary>
    public class UserJobRoleUpdateViewModel : PagingViewModel
    {
        /// <summary>
        /// Gets or sets selected role name.
        /// </summary>
        public string CurrentRoleName { get; set; }

        /// <summary>
        /// Gets or sets filter text.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// Gets or sets the selected job role id.
        /// </summary>
        public int? SelectedJobRoleId { get; set; }

        /// <summary>
        /// Gets or sets the RoleList.
        /// </summary>
        public List<JobRoleBasicViewModel> RoleList { get; set; }

        /// <summary>
        /// sets the list of radio role.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> RoleRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.RoleList.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.RoleList
                           let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.NameWithStaffGroup, false, null)
                           select newRadio);
            return radio;
        }
    }
}
