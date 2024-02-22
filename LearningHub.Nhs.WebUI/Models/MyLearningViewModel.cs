// <copyright file="MyLearningViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Models.Learning;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="MyLearningViewModel" />.
    /// </summary>
    public class MyLearningViewModel : MyLearningRequestModel, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningViewModel"/> class.
        /// </summary>
        public MyLearningViewModel()
        {
            this.Activities = new List<ActivityDetailedItemViewModel>();
            this.MostRecentResources = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningViewModel"/> class.
        /// </summary>
        /// <param name="requestModel">MyLearningRequestModel.</param>
        public MyLearningViewModel(MyLearningRequestModel requestModel)
        {
            this.Activities = new List<ActivityDetailedItemViewModel>();
            this.MostRecentResources = new List<int>();
            foreach (PropertyInfo prop in requestModel.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(requestModel, null), null);
            }
        }

        /// <summary>
        /// Gets or sets the learning form event.
        /// </summary>
        public MyLearningFormActionTypeEnum MyLearningFormActionType { get; set; }

        /// <summary>
        /// Gets or sets the page item index.
        /// </summary>
        public int CurrentPageIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the TotalCount.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the MostRecentResources.
        /// </summary>
        public List<int> MostRecentResources { get; set; }

        /// <summary>
        /// Gets or sets the Activities.
        /// </summary>
        public List<ActivityDetailedItemViewModel> Activities { get; set; }

        /// <summary>
        /// Gets or sets the learning result paging.
        /// </summary>
        public PagingViewModel MyLearningPaging { get; set; }

        /// <summary>
        /// Gets or sets the Day.
        /// </summary>
        public int? StartDay { get; set; }

        /// <summary>
        /// Gets or sets the end Day.
        /// </summary>
        public int? EndDay { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public int? StartMonth { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public int? EndMonth { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public int? StartYear { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public int? EndYear { get; set; }

        /// <summary>
        /// Gets or sets the GetDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime? GetStartDate()
        {
            return (this.StartDay.HasValue && this.StartMonth.HasValue && this.StartYear.HasValue) ? new DateTime(this.StartYear!.Value, this.StartMonth!.Value, this.StartDay!.Value) : (DateTime?)null;
        }

        /// <summary>
        /// Gets or sets the GetDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime? GetEndDate()
        {
            return (this.EndDay.HasValue && this.EndMonth.HasValue && this.EndYear.HasValue) ? new DateTime(this.EndYear!.Value, this.EndMonth!.Value, this.EndDay!.Value) : (DateTime?)null;
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (this.TimePeriod == "dateRange")
            {
                var startDateValidation = DateValidator.ValidateDate(this.StartDay, this.StartMonth, this.StartYear, "valid start date")
                    .ToValidationResultList(nameof(this.StartDay), nameof(this.StartMonth), nameof(this.StartYear));
                if (startDateValidation.Any())
                {
                    results.AddRange(startDateValidation);
                }

                var endDateValidation = DateValidator.ValidateDate(this.EndDay, this.EndMonth, this.EndYear, "valid end date")
                    .ToValidationResultList(nameof(this.EndDay), nameof(this.EndMonth), nameof(this.EndYear));
                if (endDateValidation.Any())
                {
                    results.AddRange(endDateValidation);
                }
            }

            return results;
        }

        /// <summary>
        /// sets the list of radio region.
        /// </summary>
        /// <returns>The <see cref="List{RadiosItemViewModel}"/>.</returns>
        public List<RadiosItemViewModel> SortByDateRadio()
        {
            var radios = new List<RadiosItemViewModel>()
            {
                new RadiosItemViewModel("allDates", "All dates", true, null),
                new RadiosItemViewModel("thisWeek", "This week", false, null),
                new RadiosItemViewModel("thisMonth", "This month", false, null),
                new RadiosItemViewModel("last12Months", "Last 12 months", false, null),
            };
            if (string.IsNullOrWhiteSpace(this.TimePeriod))
            {
                this.TimePeriod = "allDates";
            }

            return radios;
        }

        /// <summary>
        /// sets the list of certificate checkboxes.
        /// </summary>
        /// <returns>The <see cref="List{CheckboxListItemViewModel}"/>.</returns>
        public List<CheckboxListItemViewModel> CertificateFilterCheckbox()
        {
            var checkboxes = new List<CheckboxListItemViewModel>()
            {
                new CheckboxListItemViewModel("CertificateEnabled", "Certificate", null),
            };
            return checkboxes;
        }

        /// <summary>
        /// sets the list of status checkboxes.
        /// </summary>
        /// <returns>The <see cref="List{CheckboxListItemViewModel}"/>.</returns>
        public List<CheckboxListItemViewModel> StatusFilterCheckbox()
        {
            var checkboxes = new List<CheckboxListItemViewModel>()
            {
                new CheckboxListItemViewModel("Complete", "Complete", null),
                new CheckboxListItemViewModel("Incomplete", "Incomplete", null),
                new CheckboxListItemViewModel("Passed", "Passed", null),
                new CheckboxListItemViewModel("Failed", "Failed", null),
                new CheckboxListItemViewModel("Downloaded", "Downloaded", null),
            };
            return checkboxes;
        }

        /// <summary>
        /// sets the list of type checkboxes.
        /// </summary>
        /// <returns>The <see cref="List{CheckboxListItemViewModel}"/>.</returns>
        public List<CheckboxListItemViewModel> TypeFilterCheckbox()
        {
            var checkboxes = new List<CheckboxListItemViewModel>()
            {
                new CheckboxListItemViewModel("Article", "Article", null),
                new CheckboxListItemViewModel("Assessment", "Assessment", null),
                new CheckboxListItemViewModel("Audio", "Audio", null),
                new CheckboxListItemViewModel("Case", "Case", null),
                new CheckboxListItemViewModel("Elearning", "elearning", null),
                new CheckboxListItemViewModel("File", "File", null),
                new CheckboxListItemViewModel("Html", "HTML", null),
                new CheckboxListItemViewModel("Image", "Image", null),
                new CheckboxListItemViewModel("Video", "Video", null),
                new CheckboxListItemViewModel("Weblink", "Weblink", null),
            };
            return checkboxes;
        }
    }
}
