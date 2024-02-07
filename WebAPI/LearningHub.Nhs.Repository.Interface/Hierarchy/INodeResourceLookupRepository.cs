// <copyright file="INodeResourceLookupRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The INodeResourceLookupRepository interface.
    /// </summary>
    public interface INodeResourceLookupRepository : IGenericRepository<NodeResourceLookup>
    {
        /// <summary>
        /// The get by node id async.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeResourceLookup>> GetByNodeIdAsync(int nodeId);
    }
}
