// <copyright file="DateValidator.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="DateValidator" />.
    /// </summary>
    public static class DateValidator
    {
        private static readonly ClockUtility ClockUtility = new ClockUtility();

        /// <summary>
        /// ValidateDate.
        /// </summary>
        /// <param name="day">day.</param>
        /// <param name="month">month.</param>
        /// <param name="year">year.</param>
        /// <param name="name">name.</param>
        /// <param name="required">required.</param>
        /// <param name="validateNonPast">validateNonPast.</param>
        /// <param name="validateNonFuture">validateNonFuture.</param>
        /// <returns>DateValidationResult .</returns>
        public static DateValidationResult ValidateDate(
            int? day,
            int? month,
            int? year,
            string name = "Date",
            bool required = true,
            bool validateNonPast = false,
            bool validateNonFuture = true)
        {
            if (!day.HasValue && !month.HasValue && !year.HasValue)
            {
                return required
                    ? new DateValidationResult("Enter " + NameWithIndefiniteArticle(name))
                    : new DateValidationResult();
            }

            if (day.HasValue && month.HasValue && year.HasValue)
            {
                return ValidateDate(day.Value, month.Value, year.Value, name, validateNonPast, validateNonFuture);
            }

            var errorMessage = GetMissingValuesErrorMessage(day, month, year, name);
            return new DateValidationResult(!day.HasValue, !month.HasValue, !year.HasValue, errorMessage);
        }

        /// <summary>
        /// IsDateNull.
        /// </summary>
        /// <param name="day">day.</param>
        /// <param name="month">month.</param>
        /// <param name="year">year.</param>
        /// <returns>The .</returns>
        public static bool IsDateNull(int? day, int? month, int? year)
        {
            return day == null && month == null && year == null;
        }

        private static DateValidationResult ValidateDate(
            int day,
            int month,
            int year,
            string name,
            bool dateMustNotBeInPast,
            bool dateMustNotBeInFuture)
        {
            // note: the minimum year the DB can store is 1753
            var invalidDay = day < 1 || day > 31;
            var invalidMonth = month < 1 || month > 12;
            var invalidYear = year < 1900 || year > 9999;

            if (invalidDay || invalidMonth || invalidYear)
            {
                var errorMessage = GetInvalidValuesErrorMessage(invalidDay, invalidMonth, invalidYear, name);
                return new DateValidationResult(invalidDay, invalidMonth, invalidYear, errorMessage);
            }

            try
            {
                var date = new DateTime(year, month, day);
                if (dateMustNotBeInPast && date < ClockUtility.UtcToday)
                {
                    return new DateValidationResult("Enter " + NameWithIndefiniteArticle(name) + " not in the past");
                }

                if (dateMustNotBeInFuture && date > ClockUtility.UtcToday)
                {
                    return new DateValidationResult("Enter " + NameWithIndefiniteArticle(name) + " not in the future");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateValidationResult("Enter a real date for " + name);
            }

            return new DateValidationResult();
        }

        private static string GetMissingValuesErrorMessage(int? day, int? month, int? year, string name)
        {
            var missingValues = new List<string>();

            if (!day.HasValue)
            {
                missingValues.Add("day");
            }

            if (!month.HasValue)
            {
                missingValues.Add("month");
            }

            if (!year.HasValue)
            {
                missingValues.Add("year");
            }

            return "Enter " + NameWithIndefiniteArticle(name) + " containing a " +
                   string.Join(" and a ", missingValues);
        }

        private static string GetInvalidValuesErrorMessage(
            bool invalidDay,
            bool invalidMonth,
            bool invalidYear,
            string name)
        {
            var invalidValues = new List<string>();

            if (invalidDay)
            {
                invalidValues.Add("day");
            }

            if (invalidMonth)
            {
                invalidValues.Add("month");
            }

            if (invalidYear)
            {
                invalidValues.Add("year");
            }

            if (invalidValues.Count == 3)
            {
                return "Enter a real date for " + name;
            }

            return "Enter " + NameWithIndefiniteArticle(name) + " containing a real " +
                   string.Join(" and ", invalidValues);
        }

        private static string NameWithIndefiniteArticle(string name)
        {
            return (StartsWithVowel(name) ? "an " : "a ") + name;
        }

        private static bool StartsWithVowel(string str)
        {
            return "aeiouAEIOU".Contains(str[0]);
        }

        /// <summary>
        /// Defines the <see cref="DateValidationResult" />.
        /// </summary>
        public class DateValidationResult
        {
            /// <summary>
            /// Gets the ErrorMessage.
            /// </summary>
            private readonly string errorMessage;

            /// <summary>
            /// Gets the HasDayError.
            /// </summary>
            private readonly bool hasDayError;

            /// <summary>
            /// Gets the HasMonthError.
            /// </summary>
            private readonly bool hasMonthError;

            /// <summary>
            /// Gets the HasYearError.
            /// </summary>
            private readonly bool hasYearError;

            /// <summary>
            /// Initializes a new instance of the <see cref="DateValidationResult"/> class.
            /// </summary>
            public DateValidationResult()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DateValidationResult"/> class.
            /// </summary>
            /// <param name="errorMessage">The errorMessage.</param>
            public DateValidationResult(string errorMessage)
            {
                this.hasDayError = true;
                this.hasMonthError = true;
                this.hasYearError = true;
                this.errorMessage = errorMessage;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DateValidationResult"/> class.
            /// </summary>
            /// <param name="hasDayError">The hasDayError.</param>
            /// <param name="hasMonthError">The hasMonthError.</param>
            /// <param name="hasYearError">The hasYearError.</param>
            /// <param name="errorMessage">The errorMessage.</param>
            public DateValidationResult(bool hasDayError, bool hasMonthError, bool hasYearError, string errorMessage)
            {
                this.hasDayError = hasDayError;
                this.hasMonthError = hasMonthError;
                this.hasYearError = hasYearError;
                this.errorMessage = errorMessage;
            }

            /// <summary>
            /// The ToValidationResultList.
            /// </summary>
            /// <param name="dayId">The dayId.</param>
            /// <param name="monthId">The monthId.</param>
            /// <param name="yearId">The yearIdt.</param>
            /// <returns>The= ValidationResult.</returns>
            public List<ValidationResult> ToValidationResultList(
                string dayId,
                string monthId,
                string yearId)
            {
                var results = new List<ValidationResult>();
                var errorMessageAdded = false;

                if (this.hasDayError)
                {
                    results.Add(new ValidationResult(this.errorMessage, new[] { dayId }));
                    errorMessageAdded = true;
                }

                if (this.hasMonthError)
                {
                    // Should only add error message once per date to avoid duplicates in error summary component, but still highlight all inputs with errors
                    var errorMessage = !errorMessageAdded ? this.errorMessage : string.Empty;
                    results.Add(new ValidationResult(errorMessage, new[] { monthId }));
                    errorMessageAdded = true;
                }

                if (this.hasYearError)
                {
                    // Should only add error message once per date to avoid duplicates in error summary component, but still highlight all inputs with errors
                    var errorMessage = !errorMessageAdded ? this.errorMessage : string.Empty;
                    results.Add(new ValidationResult(errorMessage, new[] { yearId }));
                }

                return results;
            }
        }
    }
}
