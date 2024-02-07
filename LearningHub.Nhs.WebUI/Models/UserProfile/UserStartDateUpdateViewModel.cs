// <copyright file="UserStartDateUpdateViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using LearningHub.Nhs.WebUI.Helpers;

    /// <summary>
    /// Defines the <see cref="UserStartDateUpdateViewModel" />.
    /// </summary>
    public class UserStartDateUpdateViewModel : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTimeOffset? JobStartDate { get; set; }

        /// <summary>
        /// Gets or sets the Day.
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Gets or sets the GetDate.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime? GetDate()
        {
            return (this.Day.HasValue && this.Month.HasValue && this.Year.HasValue) ? new DateTime(this.Year!.Value, this.Month!.Value, this.Day!.Value) : (DateTime?)null;
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(this.Day, this.Month, this.Year, "valid start date")
                .ToValidationResultList(nameof(this.Day), nameof(this.Month), nameof(this.Year));
        }
    }
}
