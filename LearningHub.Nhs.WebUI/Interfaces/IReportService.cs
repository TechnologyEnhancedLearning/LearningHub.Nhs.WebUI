namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;

    /// <summary>
    /// Defines the <see cref="IRegionService" />.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// The GetReporterPermission.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> GetReporterPermission();
    }
}
