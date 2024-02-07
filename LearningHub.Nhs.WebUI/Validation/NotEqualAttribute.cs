// <copyright file="NotEqualAttribute.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Validation
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="NotEqualAttribute" />.
    /// </summary>
    public class NotEqualAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEqualAttribute"/> class.
        /// </summary>
        /// <param name="otherProperty">other property.</param>
        public NotEqualAttribute(string otherProperty)
        {
            this.OtherProperty = otherProperty;
        }

        private string OtherProperty { get; set; }

        /// <summary>
        /// Check if is valid.
        /// </summary>
        /// <param name="value">value.</param>
        /// <param name="validationContext">validation context.</param>
        /// <returns>validation result.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get other property value
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.OtherProperty);
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // verify values
            if (value != null && value.ToString().Equals(otherValue.ToString()))
            {
                return new ValidationResult(this.ErrorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
 }
