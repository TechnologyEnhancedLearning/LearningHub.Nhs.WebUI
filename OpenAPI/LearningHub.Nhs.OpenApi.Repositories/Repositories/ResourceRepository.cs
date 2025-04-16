namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        {
            var resources = await this.DbContext.Resource
                .Where(r => resourceIds.Contains(r.Id))
                .Where(r => !r.Deleted)
                .Include(r => r.ResourceReference)
                .ThenInclude(rr => rr.NodePath)
                .ThenInclude(np => np.CatalogueNode)
                .ThenInclude(n => n.CurrentNodeVersion)
                .ThenInclude(n => n.CatalogueNodeVersion)
                .Include(r => r.CurrentResourceVersion)
                .ThenInclude(r => r.ResourceVersionRatingSummary)
                .Include(r => r.ResourceReference)
                .ThenInclude(rr => rr.NodePath)
                .ThenInclude(np => np.Node)
                .ToListAsync();

            resources.ForEach(r =>
            {
                var nonExternalReferences = r.ResourceReference
                    .Where(rr => rr?.NodePath?.Node?.NodeTypeEnum != null && (int)rr.NodePath.Node.NodeTypeEnum != 4)
                    .ToList();

                r.ResourceReference = nonExternalReferences;
            });

            return resources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
            IEnumerable<int> originalResourceReferenceIds)
        {
            return await this.DbContext.ResourceReference
                .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId))
                .Where(rr => !rr.Deleted)
                .Where(rr => (int)rr.NodePath.Node.NodeTypeEnum != 4)
                .Include(rr => rr.NodePath)
                .ThenInclude(np => np.CatalogueNode)
                .ThenInclude(n => n.CurrentNodeVersion)
                .ThenInclude(n => n.CatalogueNodeVersion)
                .Include(rr => rr.Resource)
                .ThenInclude(r => r.CurrentResourceVersion)
                .ThenInclude(r => r.ResourceVersionRatingSummary)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetAchievedCertificatedResourceIds(int currentUserId)
        {
            // Use dashboard logic to ensure same resources determined has having achieved certificates
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = currentUserId };
            var param4 = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var result = this.DbContext.DashboardResourceDto.FromSqlRaw("resources.GetAchievedCertificatedResourcesWithOptionalPagination @userId = @userId, @TotalRecords = @TotalRecords output", param0, param4).ToList();
            List<int> achievedCertificatedResourceIds = result.Select(drd => drd.ResourceId).Distinct().ToList<int>();

            return achievedCertificatedResourceIds;
        }

        // </summary>
        // <param name="resourceReferenceIds"></param>
        // <param name="userIds"></param>
        // <param name="originalResourceReferenceIds">.</param>
        // <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceActivityDTO>> GetResourceActivityPerResourceMajorVersion(
          IEnumerable<int>? resourceIds, IEnumerable<int>? userIds)
        {
            var resourceIdsParam = resourceIds != null
                ? string.Join(",", resourceIds)
                : null;

            var userIdsParam = userIds != null
                ? string.Join(",", userIds)
                : null;

            var resourceIdsParameter = new SqlParameter("@p0", resourceIdsParam ?? (object)DBNull.Value);
            var userIdsParameter = new SqlParameter("@p1", userIdsParam ?? (object)DBNull.Value);

            List<ResourceActivityDTO> resourceActivityDTOs = await this.DbContext.ResourceActivityDTO
                .FromSqlRaw(
                    "[activity].[GetResourceActivityPerResourceMajorVersion] @p0, @p1",
                    resourceIdsParameter,
                    userIdsParameter)
                .AsNoTracking()
                .ToListAsync<ResourceActivityDTO>();

            return resourceActivityDTOs;
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
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateResourceAsync(ResourceTypeEnum resourceType, string title, string description, int userId)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = (int)resourceType };
                var param1 = new SqlParameter("@p1", SqlDbType.VarChar) { Value = title };
                var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = description ?? string.Empty };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
                var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
                var param5 = new SqlParameter("@p5", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await this.DbContext.Database.ExecuteSqlRawAsync("resources.ResourceCreate @p0, @p1, @p2, @p3, @p4, @p5 output", param0, param1, param2, param3, param4, param5);

                int resourceVersionId = (int)param5.Value;

                return resourceVersionId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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


    }
}
