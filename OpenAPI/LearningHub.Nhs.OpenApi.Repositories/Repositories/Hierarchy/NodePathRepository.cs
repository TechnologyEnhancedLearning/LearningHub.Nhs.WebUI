namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Hierarchy
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The NodePathRepository.
    /// </summary>
    public class NodePathRepository : GenericRepository<NodePath>, INodePathRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePathRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodePathRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Gets the root catalogue nodeId for any given nodeId (i.e. folder/course).
        /// </summary>
        /// <param name="folderNodeId">The folder/course nodeId.</param>
        /// <returns>The catalogue nodeId.</returns>
        public async Task<int> GetCatalogueRootNodeId(int folderNodeId)
        {
            return await DbContext.NodePath.AsNoTracking()
                            .Where(np => np.NodeId == folderNodeId
                                        && np.Deleted == false)
                            .Select(np => np.CatalogueNodeId)
                            .FirstAsync();
        }

        /// <summary>
        /// Gets the node paths to the supplied node id.
        /// </summary>
        /// <param name="nodeId">The nodeId.</param>
        /// <returns>The list of NodePaths.</returns>
        public async Task<List<NodePath>> GetNodePathsForNodeId(int nodeId)
        {
            return await DbContext.NodePath.AsNoTracking()
                            .Where(np => np.NodeId == nodeId)
                            .ToListAsync();
        }

        /// <summary>
        /// Gets the basic details of all Nodes in a particular NodePath.
        /// </summary>
        /// <param name="nodePathId">The NodePath id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodePathNodeViewModel>> GetNodePathNodes(int nodePathId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathId };

            var retVal = await DbContext.NodePathNodeViewModel.FromSqlRaw("hierarchy.GetNodePathNodes @p0", param0).AsNoTracking().ToListAsync();
            return retVal;
        }
    }
}
