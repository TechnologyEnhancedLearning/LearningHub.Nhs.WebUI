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
    using LearningHub.Nhs.Models.Entities.Activity;
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

            List<ResourceReferenceAndCatalogueTempConstructionDTO> resourceReferenceAndCatalogueTempConstructionDTOs =
                await dbContext.ResourceReferenceAndCatalogueTempConstructionDTO
                .FromSqlRaw(
                    "[resources].[GetResourceReferencesAndCatalogue] @p0, @p1",
                    resourceIdsParameter,
                    originalResourceReferenceIdsParameter)
                .AsNoTracking()
                .ToListAsync<ResourceReferenceAndCatalogueTempConstructionDTO>();

            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs = resourceReferenceAndCatalogueTempConstructionDTOs
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
                    g.Select(r => r.CatalogueNodeId.HasValue ? new CatalogueDTO
                         {
                             CatalogueNodeId = r.CatalogueNodeId.Value,
                             CatalogueNodeName = r.CatalogueNodeName,
                             IsRestricted = r.IsRestricted.Value,
                             OriginalResourceReferenceId = r.OriginalResourceReferenceId.Value,
                         }
                        : new CatalogueDTO
                         {
                             OriginalResourceReferenceId = 0, // qqqqa if we provide empty catalogue maybe this should be nullable, or we should provide the originalResourceId just 
                             CatalogueNodeId = 0, // qqqqa maybe should be null
                             CatalogueNodeName = "No catalogue for resource reference", // NoCatalogueText,
                             IsRestricted = false,
                         })
                    // qqqqa .Distinct() we could get multiple nulled if multiple originalResourceId entries that are external
                        .ToList()))
                        .ToList<ResourceReferenceAndCatalogueDTO>();


            // qqqqc could do this instead but dont think this is right
            // Check for empty catalogues and add the default one if necessary
            //foreach (var resourceReference in resourceReferenceAndCatalogueDTOs)
            //{
            //    if (!resourceReference.Catalogues.Any())
            //    {
            //        resourceReference.Catalogues.Add(new CatalogueDTO
            //        {
            //            OriginalResourceReferenceId = 0, // or make this nullable if preferred
            //            CatalogueNodeId = 0, // or make this nullable if preferred
            //            CatalogueNodeName = "No catalogue for resource reference", // NoCatalogueText
            //            IsRestricted = false,
            //        });
            //    }
            //}


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
    }
}
