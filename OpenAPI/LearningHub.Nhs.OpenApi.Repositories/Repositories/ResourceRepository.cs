namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class ResourceRepository : IResourceRepository
    {
        private LearningHubDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext"><see cref="dbContext"/>.</param>
        public ResourceRepository(LearningHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        {
            var resources = await this.dbContext.Resource
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
            return await this.dbContext.ResourceReference
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

            var result = this.dbContext.DashboardResourceDto.FromSqlRaw("resources.GetAchievedCertificatedResourcesWithOptionalPagination @userId = @userId, @TotalRecords = @TotalRecords output", param0, param4).ToList();
            List<int> achievedCertificatedResourceIds = result.Select(drd => drd.ResourceId).Distinct().ToList<int>();

            return achievedCertificatedResourceIds;
        }

        /// </summary>
        /// <param name="resourceReferenceIds"></param>
        /// <param name="userIds"></param>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
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

            List<ResourceActivityDTO> resourceActivityDTOs = await dbContext.ResourceActivityDTO
                .FromSqlRaw(
                    "[activity].[GetResourceActivityPerResourceMajorVersion] @p0, @p1",
                    resourceIdsParameter,
                    userIdsParameter)
                .AsNoTracking()
                .ToListAsync<ResourceActivityDTO>();

            return resourceActivityDTOs;
        }
    }
}
