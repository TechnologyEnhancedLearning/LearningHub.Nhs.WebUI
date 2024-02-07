// <copyright file="IResourceVersionFlagRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The ResourceVersionFlagRepository interface.
    /// </summary>
    public interface IResourceVersionFlagRepository : IGenericRepository<ResourceVersionFlag>
    {
        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<ResourceVersionFlag> GetByResourceVersionIdAsync(int resourceVersionId);

        /// <summary>
        /// The update resource version flag async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionFlag">The resource version flag.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateResourceVersionFlagAsync(int userId, ResourceVersionFlag resourceVersionFlag);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionFlagId">The resource version flag id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int resourceVersionFlagId);
    }
}
