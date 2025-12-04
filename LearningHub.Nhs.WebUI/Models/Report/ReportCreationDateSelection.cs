namespace LearningHub.Nhs.WebUI.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using LearningHub.Nhs.WebUI.Helpers;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// ReportCreationDateSelection.
    /// </summary>
    public class ReportCreationDateSelection : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the start date to define on the search.
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// Gets or sets the start date to define on the search.
        /// </summary>
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
        /// Gets or sets the Year.
        /// </summary>
        public DateTime? DataStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the EndDate.
        /// </summary>
        public bool EndDate { get; set; }

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
            var validationResults = new List<ValidationResult>();
            if (this.TimePeriod == "Custom")
            {
                this.ValidateStartDate(validationResults);

                if (this.EndDate)
                {
                    this.ValidateEndDate(validationResults);
                }
            }

            return validationResults;
        }

        /// <summary>
        /// Gets or sets the GetValidatedStartDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime GetValidatedStartDate()
        {
            return new DateTime(this.StartYear!.Value, this.StartMonth!.Value, this.StartDay!.Value);
        }

        /// <summary>
        /// Gets or sets the GetValidatedEndDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime? GetValidatedEndDate()
        {
            return this.EndDate
                ? new DateTime(this.EndYear!.Value, this.EndMonth!.Value, this.EndDay!.Value)
                : (DateTime?)null;
        }

        /// <summary>
        /// sets the list of radio region.
        /// </summary>
        /// <returns>The <see cref="List{RadiosItemViewModel}"/>.</returns>
        public List<RadiosItemViewModel> PopulateDateRange()
        {
            var radios = new List<RadiosItemViewModel>()
            {
                new RadiosItemViewModel("7", "7 days", false, null),
                new RadiosItemViewModel("30", "30 days", false, null),
                new RadiosItemViewModel("90", "90 days", false, null),
            };

            // if (string.IsNullOrWhiteSpace(this.TimePeriod))
            // {
            //    this.TimePeriod = "Custom";
            // }
            return radios;
        }

        /// <inheritdoc/>
        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //    var results = new List<ValidationResult>();
        //    if (this.TimePeriod == "dateRange")
        //    {
        //        var startDateValidation = DateValidator.ValidateDate(this.StartDay, this.StartMonth, this.StartYear, "valid start date")
        //            .ToValidationResultList(nameof(this.StartDay), nameof(this.StartMonth), nameof(this.StartYear));
        //        if (startDateValidation.Any())
        //        {
        //            results.AddRange(startDateValidation);
        //        }
        //        var endDateValidation = DateValidator.ValidateDate(this.EndDay, this.EndMonth, this.EndYear, "valid end date")
        //            .ToValidationResultList(nameof(this.EndDay), nameof(this.EndMonth), nameof(this.EndYear));
        //        if (endDateValidation.Any())
        //        {
        //            results.AddRange(endDateValidation);
        //        }
        //    }
        //    return results;
        // }
        private void ValidateStartDate(List<ValidationResult> validationResults)
        {
            var startDateValidationResults = DateValidator.ValidateDate(
                    this.StartDay,
                    this.StartMonth,
                    this.StartYear,
                    "Start date",
                    true,
                    false,
                    true)
                .ToValidationResultList(nameof(this.StartDay), nameof(this.StartMonth), nameof(this.StartYear));

            if (!startDateValidationResults.Any())
            {
                this.ValidateStartDateIsAfterDataStart(startDateValidationResults);
            }

            validationResults.AddRange(startDateValidationResults);
        }

        private void ValidateStartDateIsAfterDataStart(List<ValidationResult> startDateValidationResults)
        {
            var startDate = this.GetValidatedStartDate();

            if (startDate.AddDays(1) < this.DataStart)
            {
                startDateValidationResults.Add(
                    new ValidationResult(
                        "Enter a start date after the start of data in the platform", new[] { nameof(this.StartDay), }));
                startDateValidationResults.Add(
                    new ValidationResult(
                        string.Empty,
                        new[] { nameof(this.StartMonth), nameof(this.StartYear), }));
            }
        }

        private void ValidateEndDate(List<ValidationResult> validationResults)
        {
            var endDateValidationResults = DateValidator.ValidateDate(
                    this.EndDay,
                    this.EndMonth,
                    this.EndYear,
                    "End date",
                    true,
                    false,
                    true)
                .ToValidationResultList(nameof(this.EndDay), nameof(this.EndMonth), nameof(this.EndYear));

            this.ValidateEndDateIsAfterStartDate(endDateValidationResults);

            validationResults.AddRange(endDateValidationResults);
        }

        private void ValidateEndDateIsAfterStartDate(List<ValidationResult> endDateValidationResults)
        {
            if (this.StartYear > this.EndYear
                || (this.StartYear == this.EndYear && this.StartMonth > this.EndMonth)
                || (this.StartYear == this.EndYear && this.StartMonth == this.EndMonth && this.StartDay > this.EndDay))
            {
                endDateValidationResults.Add(
                    new ValidationResult("Enter an end date after the start date", new[] { nameof(this.EndDay), }));
                endDateValidationResults.Add(
                    new ValidationResult(string.Empty, new[] { nameof(this.EndMonth), nameof(this.EndYear), }));
            }
        }
    }
}
