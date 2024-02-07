// <copyright file="IResourceReferenceService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The Resource Reference Service interface.
    /// </summary>
    public interface IResourceReferenceService
    {
        /// <summary>
        /// The get resource reference by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceReference}"/>.</returns>
        Task<ResourceReference> GetByIdAsync(int id);
    }
}