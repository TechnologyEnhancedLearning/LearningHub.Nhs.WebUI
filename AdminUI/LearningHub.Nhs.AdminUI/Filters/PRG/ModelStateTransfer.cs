// <copyright file="ModelStateTransfer.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Filters.PRG
{
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Defines the <see cref="ModelStateTransfer" />.
    /// </summary>
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        /// <summary>
        /// Defines the Key.
        /// </summary>
        protected const string Key = nameof(ModelStateTransfer);
    }
}
