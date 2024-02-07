// <copyright file="ICaseResourceVersionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The CaseResourceVersionRepository interface.
    /// </summary>
    public interface ICaseResourceVersionRepository : IGenericRepository<CaseResourceVersion>
    {
        /// <summary>
        /// The get by Resource Version Id async.
        /// </summary>
        /// <param name="resourceVersionId">The Resource Version Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CaseResourceVersion> GetByResourceVersionIdAsync(int resourceVersionId);
    }
}
