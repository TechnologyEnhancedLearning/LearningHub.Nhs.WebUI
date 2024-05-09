namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
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
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HierarchyEditDetail> GetByNodePathIdAsync(long hierarchyEditId, int nodePathId)
        {
            try
            {
                var retVal = await this.DbContext.HierarchyEditDetail.AsNoTracking().FirstOrDefaultAsync(r => r.HierarchyEditId == hierarchyEditId && r.NodePathId == nodePathId && !r.Deleted);
                return retVal;
            }
            catch
            {
                throw;
            }
        }
    }
}
