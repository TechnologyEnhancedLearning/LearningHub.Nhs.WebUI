// <copyright file="IDetectJsLogService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// IDetectJsLogService.
    /// </summary>
    public interface IDetectJsLogService
    {
        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="jsEnabled">Js enabled request count.</param>
        /// <param name="jsDisabled">Js disabled request count.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(long jsEnabled, long jsDisabled);
    }
}