namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Hierarchy
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The HierarchyEditDetail repository.
    /// </summary>
    public class HierarchyEditDetailRepository : GenericRepository<HierarchyEditDetail>, IHierarchyEditDetailRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyEditDetailRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public HierarchyEditDetailRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get root hierarchy detail by hierarchy edit id async.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HierarchyEditDetail> GetByNodeIdAsync(long hierarchyEditId, int nodeId)
        {
            try
            {
                var retVal = await DbContext.HierarchyEditDetail.AsNoTracking().FirstOrDefaultAsync(r => r.HierarchyEditId == hierarchyEditId && r.NodeId == nodeId && !r.Deleted);
                return retVal;
            }
            catch
            {
                throw;
            }
        }
    }
}
