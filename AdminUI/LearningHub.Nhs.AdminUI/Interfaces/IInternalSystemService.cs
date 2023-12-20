// <copyright file="IInternalSystemService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Maintenance;

    /// <summary>
    /// The IInternalService.
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
        /// Toggles the internalSystem offline status.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<InternalSystemViewModel> ToggleOfflineStatus(int id);
    }
}
