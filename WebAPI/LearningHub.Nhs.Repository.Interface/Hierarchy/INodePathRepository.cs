// <copyright file="INodePathRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The INodePathRepository.
    /// </summary>
    public interface INodePathRepository : IGenericRepository<NodePath>
    {
        /// <summary>
        /// Gets the root catalogue nodeId for any given nodeId (i.e. folder/course).
        /// </summary>
        /// <param name="folderNodeId">The folder/course nodeId.</param>
        /// <returns>The catalogue nodeId.</returns>
        Task<int> GetCatalogueRootNodeId(int folderNodeId);

        /// <summary>
        /// Gets the node paths to the supplied node id.
        /// </summary>
        /// <param name="nodeId">The nodeId.</param>
        /// <returns>The list of NodePaths.</returns>
        Task<List<NodePath>> GetNodePathsForNodeId(int nodeId);

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The NodePath id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodePathNodeViewModel>> GetNodePathNodes(int nodePathId);
    }
}
