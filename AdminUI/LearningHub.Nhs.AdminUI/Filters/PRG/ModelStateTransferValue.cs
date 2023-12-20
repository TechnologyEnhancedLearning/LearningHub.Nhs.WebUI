// <copyright file="ModelStateTransferValue.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Filters.PRG
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ModelStateTransferValue" />.
    /// </summary>
    public class ModelStateTransferValue
    {
        /// <summary>
        /// Gets or sets the AttemptedValue.
        /// </summary>
        public string AttemptedValue { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessages.
        /// </summary>
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the RawValue.
        /// </summary>
        public object RawValue { get; set; }
    }
}
