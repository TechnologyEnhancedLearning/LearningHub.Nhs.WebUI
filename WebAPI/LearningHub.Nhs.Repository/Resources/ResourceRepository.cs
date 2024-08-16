namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource repository.
    /// </summary>
    public class ResourceRepository : GenericRepository<Resource>, IResourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Resource> GetByIdAsync(int id)
        {
            return await this.DbContext.Resource.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Resource> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await this.DbContext.ResourceVersion.AsNoTracking()
                            ////.Include(rv => rv.Resource)
                            .Where(rv => rv.Id == resourceVersionId && !rv.Resource.Deleted)
                            .Select(rv => rv.Resource)
                            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns true if the user has any resources published.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has any resources published.</returns>
        public async Task<bool> UserHasPublishedResourcesAsync(int userId)
        {
            return await this.DbContext.Resource.AsNoTracking()
                .Where(r => r.CreateUserId == userId && !r.Deleted)
                .AnyAsync();
        }

        /// <summary>
        /// The create resource async.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="primaryCatalogueNodeId">The primaryCatalogueNodeId.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateResourceAsync(ResourceTypeEnum resourceType, string title, string description, int primaryCatalogueNodeId, int userId)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = (int)resourceType };
                var param1 = new SqlParameter("@p1", SqlDbType.VarChar) { Value = title };
                var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = description ?? string.Empty };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = primaryCatalogueNodeId };
                var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = userId };
                var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
                var param6 = new SqlParameter("@p6", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await this.DbContext.Database.ExecuteSqlRawAsync("resources.ResourceCreate @p0, @p1, @p2, @p3, @p4, @p5,@p6 output", param0, param1, param2, param3, param4, param5, param6);

                int resourceVersionId = (int)param6.Value;

                return resourceVersionId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The transfer resource ownership.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="newOwnerUsername">The new owner username.</param>
        /// <param name="userId">The user id.</param>
        public void TransferResourceOwnership(int resourceId, string newOwnerUsername, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = newOwnerUsername };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            this.DbContext.Database.ExecuteSqlRaw("resources.ResourceReassignOwnership @p0, @p1, @p2, @p3", param0, param1, param2, param3);
        }

        /// <summary>
        /// Returns a bool to indicate if the resourceVersionId corresponds to a current version of a resource.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> IsCurrentVersionAsync(int resourceVersionId)
        {
            return await this.DbContext.Resource.AnyAsync(r => r.CurrentResourceVersionId == resourceVersionId && !r.Deleted);
        }
    }
}
