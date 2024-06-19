namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The HierarchyEditDetailRepository interface.
    /// </summary>
    public interface IHierarchyEditDetailRepository : IGenericRepository<HierarchyEditDetail>
    {
        /// <summary>
        /// The get root hierarchy detail by hierarchy edit id async.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HierarchyEditDetail> GetByNodePathIdAsync(long hierarchyEditId, int nodePathId);

        /// <summary>
        /// The get node path breakdown for child nodes.
        /// </summary>
        /// <param name="nodePathId">The parent node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodePathBreakdownItemDto>> GetChildNodePathBreakdownAsync(int nodePathId);
    }
}
