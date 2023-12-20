// <copyright file="AccountCreationListViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// The AccountCreationListViewModel.
    /// </summary>
    public class AccountCreationListViewModel : AccountCreationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountCreationListViewModel"/> class.
        /// </summary>
        public AccountCreationListViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the CountryList.
        /// </summary>
        public List<GenericListViewModel> CountryList { get; set; }

        /// <summary>
        /// Gets or sets the RoleList.
        /// </summary>
        public List<JobRoleBasicViewModel> RoleList { get; set; }

        /// <summary>
        /// Gets or sets the SpecialtyList.
        /// </summary>
        public List<GenericListViewModel> SpecialtyList { get; set; }

        /// <summary>
        /// Gets or sets the optional SpecialtyList.
        /// </summary>
        public GenericListViewModel OptionalSpecialtyItem { get; set; }

        /// <summary>
        /// Gets or sets the WorkPlaceList.
        /// </summary>
        public List<LocationBasicViewModel> WorkPlaceList { get; set; }

        /// <summary>
        /// Gets or sets the GradeList.
        /// </summary>
        public List<GenericListViewModel> GradeList { get; set; }

        /// <summary>
        /// Gets or sets the Region.
        /// </summary>
        public List<GenericListViewModel> Region { get; set; }

        /// <summary>
        /// Gets or sets the list result paging.
        /// </summary>
        public PagingViewModel AccountCreationPaging { get; set; }

        /// <summary>
        /// sets the list of radio region.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> RegionRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.Region.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.Region
                           let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.Name, false, null)
                           select newRadio);
            return radio;
        }

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

        /// <summary>
        /// sets the list of radio country.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> CountryRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.CountryList.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.CountryList
                       let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.Name, false, null)
                       select newRadio);
            return radio;
        }

        /// <summary>
        /// sets the list of radio specialty.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{RadiosItemViewModel}"/>.</returns>
        public IEnumerable<RadiosItemViewModel> SpecialtyRadio()
        {
            var radio = new List<RadiosItemViewModel>();
            if (!this.SpecialtyList.Any())
            {
                return radio;
            }

            radio.AddRange(from k in this.SpecialtyList
                           let newRadio = new RadiosItemViewModel(k.Id.ToString(), k.Name, false, null)
                           select newRadio);

            if (this.OptionalSpecialtyItem != null)
            {
                radio.Add(new RadiosItemViewModel(this.OptionalSpecialtyItem.Id.ToString(), this.OptionalSpecialtyItem.Name, false, null));
            }

            return radio;
        }

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
