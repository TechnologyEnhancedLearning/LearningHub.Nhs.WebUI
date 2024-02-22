// <copyright file="IResourceVersionEventRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The ResourceVersionEventRepository interface.
    /// </summary>
    public interface IResourceVersionEventRepository : IGenericRepository<ResourceVersionEvent>
    {
        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersionEvent}"/>.</returns>
        IQueryable<ResourceVersionEvent> GetByResourceVersionIdAsync(int resourceVersionId);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="resourceVersionEventType">The resource version event type.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersionEvent}"/>.</returns>
        IQueryable<ResourceVersionEvent> GetByResourceVersionIdAndEventTypeAsync(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType);
    }

    /// <summary>
    /// Defines the <see cref="IResourceReferenceEventRepository" />.
    /// </summary>
    public interface IResourceReferenceEventRepository : IGenericRepository<ResourceReferenceEvent>
    {
    }
}
