// <copyright file="IDetectJsLogService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;

    /// <summary>
    /// The IDetectJsLogService interface.
    /// </summary>
    public interface IDetectJsLogService
    {
        /// <summary>
        /// UpdateAsync.
        /// </summary>
        /// <param name="jsEnabled">Js enabled request count.</param>
        /// <param name="jsDisabled">Js disabled request count.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(long jsEnabled, long jsDisabled);
    }
}
