// <copyright file="IHierarchyEditDetailRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The HierarchyEditDetailRepository interface.
    /// </summary>
    public interface IHierarchyEditDetailRepository : IGenericRepository<HierarchyEditDetail>
    {
        /// <summary>
        /// The get root hierarchy detail by hierarchy edit id async.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HierarchyEditDetail> GetByNodeIdAsync(long hierarchyEditId, int nodeId);
    }
}
