namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Resources;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Activity;//qqqq move to activity repository
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
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

        /// <summary>
        /// GetResourceReferencesByOriginalResourceReferenceIds
        /// </summary>
        /// <param name="resourceReferenceIds">.</param>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReferenceAndCatalogueDTO}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceReferenceAndCatalogueDTO>> GetResourceReferenceAndCatalogues(
           IEnumerable<int> resourceReferenceIds, IEnumerable<int> originalResourceReferenceIds)
        {
            var resourceIdsParam = resourceReferenceIds != null
                ? string.Join(",", resourceReferenceIds)
                : null;

            var originalResourceReferenceIdsParam = originalResourceReferenceIds != null
                ? string.Join(",", originalResourceReferenceIds)
                : null;

            var resourceIdsParameter = new SqlParameter("@p0", resourceIdsParam ?? (object)DBNull.Value);
            var originalResourceReferenceIdsParameter = new SqlParameter("@p1", originalResourceReferenceIdsParam ?? (object)DBNull.Value);

            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs =
                dbContext.ResourceReferenceAndCatalogueTempConstructionDTO
                .FromSqlRaw(
                    "[resources].[GetResourceReferencesAndCatalogue] @p0, @p1",
                    resourceIdsParameter,
                    originalResourceReferenceIdsParameter)
                .AsNoTracking()
                .ToList<ResourceReferenceAndCatalogueTempConstructionDTO>()
                .GroupBy(r => new
                {
                    r.ResourceId,
                    r.Title,
                    r.Description,
                    r.ResourceTypeId,
                    r.MajorVersion,
                    r.Rating,
                })
                .Select(
                    g => new ResourceReferenceAndCatalogueDTO(
                    g.Key.ResourceId,
                    g.Key.Title,
                    g.Key.Description,
                    g.Key.ResourceTypeId,
                    g.Key.MajorVersion,
                    g.Key.Rating,
                    g.Select(r => new CatalogueDTO
                    {
                        CatalogueNodeId = r.CatalogueNodeId,
                        CatalogueNodeName = r.CatalogueNodeName,
                        IsRestricted = r.IsRestricted,
                        OriginalResourceReferenceId = r.OriginalResourceReferenceId,
                    })
                        .Where(c => c.CatalogueNodeId.HasValue).ToList())
                ) // Filter out null catalogue entries (the external ones)
                        .ToList<ResourceReferenceAndCatalogueDTO>();

            return resourceReferenceAndCatalogueDTOs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceReferenceIds"></param>
        /// <param name="userIds"></param>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<ResourceActivityDTO>> GetResourceActivityPerResourceMajorVersion(
          IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds)
        {
            var resourceIdsParam = resourceReferenceIds != null
                ? string.Join(",", resourceReferenceIds)
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

        ///// <inheritdoc/>
        public void QqqqTest()
        {
            //List<int> originalResourceReferenceIds = new List<int>() { 302 };
            //List<int> resourceIds = new List<int>() { 303 };
            //int userId = 57541;

            //var getResourceReferencesByOriginalResourceReferenceIdsQuery = this.dbContext.ResourceReference //has
            //                                                                                                //.AsNoTracking()
            //     .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
            //              !rr.Deleted &&
            //              (int)rr.NodePath.Node.NodeTypeEnum != 4)
            //     .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion) //has
            //     .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary) //has
            //     ;

            //var GetResourcesFromIdsQuery =  this.dbContext.Resource //has
            //                                        //.AsNoTracking()
            //                                        .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
            //                                        .Include(r => r.ResourceVersion) //not shared    -- is it used
            //                                        .Include(r => r.ResourceReference) //has 
            //                                            .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion) //has
            //                                        .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary) //has
            //                                        .Include(r => r.ResourceReference)
            //                                            .ThenInclude(rr => rr.NodePath.Node) //hasnt except for the where
            //                                        .Include(r => r.ResourceActivity.Where(ra => ra.UserId == userId && !ra.Deleted))
            //                                        ;


            //var query = getResourceReferencesByOriginalResourceReferenceIdsQuery;
            //// Retrieve and print SQL before execution
            //string sql = query.ToQueryString();
            //Console.WriteLine(sql);

            //// Execute the query asynchronously
            //var resources = query.ToListAsync();

            var qqqq = GetResourceReferenceAndCatalogues(new List<int>() { 303 }, new List<int>() { });
            var qqqq1 = qqqq;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="resourceIds">.</param>
        ///// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        //public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds)
        //{
        //    try
        //    {
        //        var resources = await this.dbContext.Resource // qqqqqq
        //                                                .AsNoTracking()
        //                                                .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
        //                                                .Include(r => r.ResourceReference)
        //                                                    .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //                                                .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
        //                                                .Include(r => r.ResourceReference)
        //                                                    .ThenInclude(rr => rr.NodePath.Node)
        //                                                .ToListAsync();

        //        resources.ForEach(r =>
        //        {
        //            var nonExternalReferences = r.ResourceReference
        //                .Where(rr => rr?.NodePath?.Node?.NodeTypeEnum != null && (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //                .ToList();

        //            r.ResourceReference = nonExternalReferences;
        //        });

        //        return resources;
        //    }
        //    catch (Exception e)
        //    {
        //        // qqqq
        //        var z = e;
        //        return null;
        //    }

        //}

        ///// <inheritdoc />
        /////qqqq
        //public async Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds/*, int userId*/)
        //{

        //    var resources = await this.dbContext.Resource
        //                                            .AsNoTracking()
        //                                            .Where(r => resourceIds.Contains(r.Id) && !r.Deleted)
        //                                            .Include(r => r.ResourceVersion)
        //                                            .Include(r => r.ResourceReference)
        //                                                .ThenInclude(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //                                            .Include(r => r.CurrentResourceVersion.ResourceVersionRatingSummary)
        //                                            .Include(r => r.ResourceReference)
        //                                                .ThenInclude(rr => rr.NodePath.Node)
        //                                            //.Include(r => r.ResourceActivity.Where(ra => ra.UserId == userId && !ra.Deleted))
        //                                            .ToListAsync();

        //    // nodepath just for catalogues
        //    // has to have catalogue, but one catalogue community contribution, whether this is null or a specific catalogueid to catch it
        //    // qqqq what is this for - in proc can i just right join and NodeTypeEnum != 4 in where
        //    resources.ForEach(r =>
        //    {
        //        var nonExternalReferences = r.ResourceReference
        //            .Where(rr =>
        //            rr?.NodePath?.Node?.NodeTypeEnum != null &&
        //            (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //            .ToList();

        //        r.ResourceReference = nonExternalReferences;
        //    });

        //    return resources;
        //}

        ///// <inheritdoc/>
        /////qqqq
        //public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
        //    IEnumerable<int> originalResourceReferenceIds, int userId)
        //{
        //        return await this.dbContext.ResourceReference
        //    .AsNoTracking()
        //    .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
        //                 !rr.Deleted &&
        //                 (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //    .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //    .Include(rr => rr.Resource.ResourceActivity
        //        .Where(ra => ra.UserId == userId && !ra.Deleted))
        //        .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
        //        .ToListAsync();
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="originalResourceReferenceIds">.</param>
        ///// <returns>A <see cref="Task{ResourceReference}"/> representing the result of the asynchronous operation.</returns>
        //public async Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
        //  IEnumerable<int> originalResourceReferenceIds)
        //{
        //    return await this.dbContext.ResourceReference
        //        .AsNoTracking()
        //        .Where(rr => originalResourceReferenceIds.Contains(rr.OriginalResourceReferenceId) &&
        //                 !rr.Deleted &&
        //                 (int)rr.NodePath.Node.NodeTypeEnum != 4)
        //        .Include(rr => rr.NodePath.CatalogueNode.CurrentNodeVersion.CatalogueNodeVersion)
        //        .Include(rr => rr.Resource.CurrentResourceVersion.ResourceVersionRatingSummary)
        //        .ToListAsync();
        //}
    }
}
