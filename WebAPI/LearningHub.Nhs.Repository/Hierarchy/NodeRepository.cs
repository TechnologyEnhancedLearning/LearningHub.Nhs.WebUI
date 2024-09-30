namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dto;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The node repository.
    /// </summary>
    public class NodeRepository : GenericRepository<Node>, INodeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodeRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Node> GetByIdAsync(int id)
        {
            return await this.DbContext.Node.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /////// <summary>
        /////// Gets the basic details of a node. Currently catalogues or folders.
        /////// </summary>
        /////// <param name="nodeId">The node id.</param>
        /////// <returns>The node details.</returns>
        ////public NodeViewModel GetNodeDetails(int nodeId)
        ////{
        ////    var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodeId };

        ////    var retVal = this.DbContext.NodeViewModel.FromSqlRaw("hierarchy.GetNodeDetails @p0", param0).AsEnumerable().FirstOrDefault();
        ////    return retVal;
        ////}

        /// <summary>
        /// Gets the contents of a node path for the catalogue landing page - i.e. published folders and published resources only.
        /// Only returns the items found directly in the specified node path, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="includeEmptyFolder">Include Empty Folder or not.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentBrowseViewModel>> GetNodeContentsForCatalogueBrowse(int nodePathId, bool includeEmptyFolder)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathId };
            if (includeEmptyFolder)
            {
                var retVal = await this.DbContext.NodeContentBrowseViewModel.FromSqlRaw("hierarchy.GetNodeContentsForCatalogueBrowse_withEmptyFolders @p0", param0).AsNoTracking().ToListAsync();
                return retVal;
            }
            else
            {
                var retVal = await this.DbContext.NodeContentBrowseViewModel.FromSqlRaw("hierarchy.GetNodeContentsForCatalogueBrowse @p0", param0).AsNoTracking().ToListAsync();
                return retVal;
            }
        }

        /// <summary>
        /// Gets the contents of a nodepath for the My Contributions page - i.e. published folders only, and all resources (i.e. all statuses).
        /// Only returns the items found directly in the specified nodepath, does not recurse down through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentEditorViewModel>> GetNodeContentsForCatalogueEditor(int nodePathId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathId };

            var retVal = await this.DbContext.NodeContentEditorViewModel.FromSqlRaw("hierarchy.GetNodeContentsForCatalogueEditor @p0", param0).AsNoTracking().ToListAsync();
            return retVal;
        }

        /// <summary>
        /// Check catalogue has external reference.
        /// </summary>
        /// <param name="nodeId">The node path id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> CheckCatalogueHasExternalReference(int nodeId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            string sql = "hierarchy.CheckCatalogueHasExternalReference @p0, @p1 output";
            var sqlParams = new List<SqlParameter>() { param0, param1 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            return (bool)param1.Value;
        }

        /// <summary>
        /// Gets the contents of a node path (catalogue/folder/course) - i.e. returns a list of subfolders and resources. Only returns the
        /// items from the first level down. Doesn't recurse through subfolders.
        /// </summary>
        /// <param name="nodePathId">The node path id.</param>
        /// <param name="readOnly">Set to true if read only data set is required.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeContentAdminDto>> GetNodeContentsAdminAsync(int nodePathId, bool readOnly)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathId };

                if (readOnly)
                {
                    var retVal = await this.DbContext.NodeContentAdminDto.FromSqlRaw("hierarchy.GetNodeContentsAdminReadOnly @p0", param0).AsNoTracking().ToListAsync();
                    return retVal;
                }
                else
                {
                    var retVal = await this.DbContext.NodeContentAdminDto.FromSqlRaw("hierarchy.GetNodeContentsAdmin @p0", param0).AsNoTracking().ToListAsync();
                    return retVal;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
