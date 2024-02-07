// <copyright file="IInternalSystemService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.Models.Maintenance;

    /// <summary>
    /// The InternalSystemService.
    /// </summary>
    public interface IInternalSystemService
    {
        /// <summary>
        /// The get all async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<InternalSystemViewModel>> GetAllAsync();

        /// <summary>
        /// Gets InternalSystem by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<InternalSystemViewModel> GetByIdAsync(int id);

        /// <summary>
        /// Toggles the internalSystem offline status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<InternalSystemViewModel> ToggleOfflineStatusAsync(int id, int userId);
    }
}
