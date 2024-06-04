namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The HierarchyEdit repository.
    /// </summary>
    public class HierarchyEditRepository : GenericRepository<HierarchyEdit>, IHierarchyEditRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyEditRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public HierarchyEditRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HierarchyEdit> GetByIdAsync(int id)
        {
            return await this.DbContext.HierarchyEdit.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by root node path id async.
        /// </summary>
        /// <param name="rootNodePathId">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<HierarchyEdit>> GetByRootNodePathIdAsync(int rootNodePathId)
        {
            return await this.DbContext.HierarchyEdit.AsNoTracking()
                .Include(x => x.Publication)
                .Include(x => x.CreateUser)
                .Where(x => x.RootNodePathId == rootNodePathId && !x.Deleted).ToListAsync();
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="rootNodePathId">The root node path id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The hierarchy edit id.</returns>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> Create(int rootNodePathId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = rootNodePathId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Direction = ParameterDirection.Output };

            string sql = "hierarchy.HierarchyEditCreate @p0, @p1, @p2, @p3 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            return (int)param3.Value;
        }

        /// <summary>
        /// The discard.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Discard(int hierarchyEditId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditDiscard @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public async Task<int> CreateFolder(FolderEditViewModel folderEditViewModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = folderEditViewModel.HierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = folderEditViewModel.Name };
            var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = folderEditViewModel.Description };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = folderEditViewModel.ParentNodePathId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = userId };
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param6 = new SqlParameter("@p6", SqlDbType.Int) { Direction = ParameterDirection.Output };

            string sql = "hierarchy.HierarchyEditFolderCreate @p0, @p1, @p2, @p3, @p4, @p5, @p6 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4, param5, param6 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            return (int)param6.Value;
        }

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="folderEditViewModel">The folderEditViewModel<see cref="FolderEditViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateFolder(FolderEditViewModel folderEditViewModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = folderEditViewModel.HierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = folderEditViewModel.Name };
            var param2 = new SqlParameter("@p2", SqlDbType.NVarChar) { Value = folderEditViewModel.Description };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditFolderUpdate @p0, @p1, @p2, @p3, @p4";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /////// <summary>
        /////// Creates a new folder.
        /////// </summary>
        /////// <param name="nodePathDisplayVersionModel">The nodePathDisplayVersionModel<see cref="NodePathDisplayVersionModel"/>.</param>
        /////// <param name="userId">The user id.</param>
        /////// <returns>The <see cref="int"/>.</returns>
        ////public async Task<int> CreateNodePathDisplayVersionAsync(NodePathDisplayVersionModel nodePathDisplayVersionModel, int userId)
        ////{
        ////    var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathDisplayVersionModel.HierarchyEditDetailId };
        ////    var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = nodePathDisplayVersionModel.Name };
        ////    var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = nodePathDisplayVersionModel.NodePathId };
        ////    var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
        ////    var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
        ////    var param5 = new SqlParameter("@p5", SqlDbType.Int) { Direction = ParameterDirection.Output };

        ////    string sql = "hierarchy.HierarchyEditNodePathDisplayVersionCreate @p0, @p1, @p2, @p3, @p4, @p5 output";
        ////    var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4, param5 };

        ////    await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

        ////    return (int)param5.Value;
        ////}

        /// <summary>
        /// Updates a folder.
        /// </summary>
        /// <param name="nodePathDisplayVersionModel">The nodePathDisplayVersionModel<see cref="NodePathDisplayVersionModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> UpdateNodePathDisplayVersionAsync(NodePathDisplayVersionModel nodePathDisplayVersionModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodePathDisplayVersionModel.HierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = nodePathDisplayVersionModel.Name };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = nodePathDisplayVersionModel.NodePathId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = nodePathDisplayVersionModel.NodePathDisplayVersionId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = userId };
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
            var param6 = new SqlParameter("@p6", SqlDbType.Int) { Direction = ParameterDirection.Output };

            string sql = "hierarchy.HierarchyEditNodePathDisplayVersionUpdate @p0, @p1, @p2, @p3, @p4, @p5, @p6 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4, param5, param6 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);

            return (int)param6.Value;
        }

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteFolder(int hierarchyEditDetailId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditDeleteFolder @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a node up.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MoveNodeUp(int hierarchyEditDetailId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveNodeUp @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a node down.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MoveNodeDown(int hierarchyEditDetailId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveNodeDown @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MoveNode(MoveNodeViewModel moveNodeViewModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = moveNodeViewModel.HierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = moveNodeViewModel.MoveToHierarchyEditDetailId };
            var param3 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveNode @p0, @p1, @p2, @p3";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a node.
        /// </summary>
        /// <param name="moveNodeViewModel">The moveNodeViewModel <see cref="MoveNodeViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ReferenceNode(MoveNodeViewModel moveNodeViewModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = moveNodeViewModel.HierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = moveNodeViewModel.MoveToHierarchyEditDetailId };
            var param3 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditReferenceNode @p0, @p1, @p2, @p3";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource up in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HierarchyEditMoveResourceUp(int hierarchyEditDetailId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveResourceUp @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource down in a hierarchy edit.
        /// </summary>
        /// <param name="hierarchyEditDetailId">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HierarchyEditMoveResourceDown(int hierarchyEditDetailId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveResourceDown @p0, @p1, @p2";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource in a HierarchyEdit.
        /// </summary>
        /// <param name="moveResourceViewModel">The view model <see cref="HierarchyEditMoveResourceViewModel"/>.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HierarchyEditMoveResource(HierarchyEditMoveResourceViewModel moveResourceViewModel, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = moveResourceViewModel.HierarchyEditDetailId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = moveResourceViewModel.MoveToHierarchyEditDetailId };
            var param3 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.HierarchyEditMoveResource @p0, @p1, @p2, @p3";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource up.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MoveResourceUp(int nodeId, int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.MoveResourceUp @p0, @p1, @p2, @p3";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource down.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="nodeId">The id of the node containing the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task MoveResourceDown(int nodeId, int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            string sql = "hierarchy.MoveResourceDown @p0, @p1, @p2, @p3";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3 };

            await this.DbContext.Database.ExecuteSqlRawAsync(sql, sqlParams);
        }

        /// <summary>
        /// Moves a resource.
        /// ITERATION 1 - Not moved within a hierarchy edit, update happens immediately.
        /// </summary>
        /// <param name="sourceNodeId">The id of the node to move the resource from.</param>
        /// <param name="destinationNodeId">The id of the node to move the resource to.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>A list of nodeIds affected by the moved resource. The nodes that will need to be refreshed in the UI.</returns>
        public async Task<List<MoveResourceResultViewModel>> MoveResource(int sourceNodeId, int destinationNodeId, int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = sourceNodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = destinationNodeId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = resourceId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            var result = await this.DbContext.MoveResourceResultViewModel.FromSqlRaw("hierarchy.MoveResource @p0, @p1, @p2, @p3, @p4", param0, param1, param2, param3, param4).AsNoTracking().ToListAsync();
            return result;
        }

        /// <summary>
        /// The publishing.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        public void SubmitForPublishing(int hierarchyEditId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("hierarchy.HierarchyEditSubmitForPublishing @p0, @p1, @p2", param0, param1, param2);
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="isMajorRevision">The is major revision.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The publication id.</returns>
        public int Publish(int hierarchyEditId, bool isMajorRevision, string notes, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.Bit) { Value = isMajorRevision };
            var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = notes };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Direction = ParameterDirection.Output };

            string sql = "hierarchy.HierarchyEditPublish @p0, @p1, @p2, @p3, @p4 output";
            var sqlParams = new List<SqlParameter>() { param0, param1, param2, param3, param4 };

            this.DbContext.Database.ExecuteSqlRaw(sql, sqlParams);

            return (int)param4.Value;
        }

        /// <summary>
        /// Set hierarchy edit to "publishing".
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        public void Publishing(int hierarchyEditId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

            this.DbContext.Database.ExecuteSqlRaw("hierarchy.HierarchyEditPublishing @p0, @p1", param0, param1);
        }

        /// <summary>
        /// Set hierarchy edit to "failed to publish".
        /// </summary>
        /// <param name="hierarchyEditId">The hierarchy edit id.</param>
        /// <param name="userId">The user id.</param>
        public void FailedToPublish(int hierarchyEditId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = hierarchyEditId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = userId };

            this.DbContext.Database.ExecuteSqlRaw("hierarchy.HierarchyEditFailedToPublish @p0, @p1", param0, param1);
        }
    }
}
