// <copyright file="IResourceVersionValidationResultRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The IResourceVersionValidationResultRepository interface.
    /// </summary>
    public interface IResourceVersionValidationResultRepository : IGenericRepository<ResourceVersionValidationResult>
    {
        /// <summary>
        /// The GetByResourceVersionIdAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>ResourceVersionValidationResult.</returns>
        Task<ResourceVersionValidationResult> GetByResourceVersionIdAsync(int resourceVersionId);
    }
}
