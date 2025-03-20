namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// The AccountCreationDateViewModel.
    /// </summary>
    public class AccountCreationDateViewModel : AccountCreationViewModel, IValidatableObject
    {
        /// <summary>
        /// Gets or sets the Day.
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Gets or sets the Day Field.
        /// </summary>
        public int? DayInput { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the Month input.
        /// </summary>
        public int? MonthInput { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Gets or sets YearInput.
        /// </summary>
        public int? YearInput { get; set; }

        /// <summary>
        /// Gets or sets the GetDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime? GetDate()
        {
            return (this.DayInput.HasValue && this.MonthInput.HasValue && this.YearInput.HasValue) ? new DateTime(this.YearInput!.Value, this.MonthInput!.Value, this.DayInput!.Value) : (DateTime?)null;
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            int parsedDay = 0;
            int parsedMonth = 0;
            int parsedYear = 0;

            if (!string.IsNullOrWhiteSpace(this.Day) && !int.TryParse(this.Day, out parsedDay))
            {
                validationResults.Add(new ValidationResult(
                $"The value '{this.Day}' is not valid for Day.", new[] { nameof(this.Day) }));
            }

            if (!string.IsNullOrWhiteSpace(this.Month) && !int.TryParse(this.Month, out parsedMonth))
            {
                validationResults.Add(new ValidationResult(
                $"The value '{this.Month}' is not valid for Month.", new[] { nameof(this.Month) }));
            }

            if (!string.IsNullOrWhiteSpace(this.Year) && !int.TryParse(this.Year, out parsedYear))
            {
                validationResults.Add(new ValidationResult(
                $"The value '{this.Year}' is not valid for Year.", new[] { nameof(this.Year) }));
            }

            if (validationResults.Count > 0)
            {
                return validationResults;
            }

            this.DayInput = parsedDay;
            this.MonthInput = parsedMonth;
            this.YearInput = parsedYear;

            return DateValidator.ValidateDate(this.DayInput, this.MonthInput, this.YearInput, "valid start date")
                    .ToValidationResultList(nameof(this.Day), nameof(this.Month), nameof(this.Year));
        }
    }
}
