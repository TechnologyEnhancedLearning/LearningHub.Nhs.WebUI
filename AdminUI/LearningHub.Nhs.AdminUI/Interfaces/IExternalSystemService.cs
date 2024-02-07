// <copyright file="IExternalSystemService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.External;
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="IExternalSystemService" />.
    /// </summary>
    public interface IExternalSystemService
    {
        /// <summary>
        /// The GetIdAsync.
        /// </summary>
        /// <param name="id">The externalSystem id.</param>
        /// <returns>The <see cref="Task{ExternalSystem}"/>.</returns>
        Task<ExternalSystem> GetIdAsync(int id);

        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ExternalSystem}"/>.</returns>
        Task<PagedResultSet<ExternalSystem>> GetExternalSystems(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// Deletes a external system.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="externalSystem">The externalSystem<see cref="ExternalSystem"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        Task<string> Create(ExternalSystem externalSystem);

        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="externalSystem">The externalSystem<see cref="ExternalSystem"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<string> Edit(ExternalSystem externalSystem);
    }
}
