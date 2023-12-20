// <copyright file="IResourceVersionRatingSummaryRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceVersionEventRepository interface.
    /// </summary>
    public interface IResourceVersionRatingSummaryRepository : IGenericRepository<ResourceVersionRatingSummary>
    {
        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersionRatingSummary> GetByResourceVersionIdAsync(int resourceVersionId);
    }
}
