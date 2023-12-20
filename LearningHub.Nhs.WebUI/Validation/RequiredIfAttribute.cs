// <copyright file="RequiredIfAttribute.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Validation
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// DataAnnotation extension for a conditionally Required property.
    /// </summary>
    public class RequiredIfAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">The propertyName.</param>
        /// <param name="value">The value.</param>
        /// <param name="errorMessage">The errorMessage.</param>
        public RequiredIfAttribute(string propertyName, object value, string errorMessage = "")
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the PropertyName.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc/>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var propertyvalue = type.GetProperty(this.PropertyName).GetValue(instance, null);
            if (propertyvalue != null && (propertyvalue.ToString() == this.Value.ToString() && value == null))
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
