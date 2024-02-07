// <copyright file="NodeResourceRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Repository.Helpers;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The node resource repository.
    /// </summary>
    public class NodeResourceRepository : GenericRepository<NodeResource>, INodeResourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeResourceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodeResourceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by resource id async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeResource>> GetByResourceIdAsync(int resourceId)
        {
            return await this.DbContext.NodeResource.AsNoTracking().Where(r => r.ResourceId == resourceId && !r.Deleted)
                                                                .Include(n => n.Node)
                                                                .ThenInclude(n => n.NodePaths)
                                                                .ThenInclude(n => n.NodePathNode)
                                                                .ThenInclude(n => n.Node)
                                                                .ToListAsync();
        }

        /// <summary>
        /// Gets the Id of the node where a resource is currently located (IT1 - one resource is located in one node).
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The node id.</returns>
        public async Task<int> GetNodeIdByResourceId(int resourceId)
        {
            return await this.DbContext.NodeResource.AsNoTracking().Where(r => r.ResourceId == resourceId && !r.Deleted)
                .Select(x => x.NodeId).SingleAsync();
        }

        /// <summary>
        /// The get catalogue locations for resource.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The CatalogueLocationViewModel list.</returns>
        public List<CatalogueLocationViewModel> GetCatalogueLocationsForResource(int resourceId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = resourceId };

            var vms = this.DbContext.CatalogueLocationViewModel.FromSqlRaw("hierarchy.CatalogueLocationsForResource @p0", param0).ToList();

            return vms;
        }

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="nodeId">nodeId.</param>
        /// <param name="catalogueOrder">catalogueOrder.</param>
        /// <param name="offset">offset.</param>
        /// <returns>CatalogueResourceViewModel.</returns>
        public async Task<CatalogueResourceResponseViewModel> GetResourcesAsync(int nodeId, CatalogueOrder catalogueOrder, int offset)
        {
            var resourceViewModels = (from nr in this.DbContext.NodeResource.AsNoTracking()
                                      join rr in this.DbContext.ResourceReference.AsNoTracking() on nr.ResourceId equals rr.ResourceId
                                      join np in this.DbContext.NodePath.AsNoTracking() on rr.NodePathId equals np.Id
                                      join r in this.DbContext.Resource.AsNoTracking() on nr.ResourceId equals r.Id
                                      join rv in this.DbContext.ResourceVersion.AsNoTracking() on r.CurrentResourceVersionId equals rv.Id
                                      join grv in this.DbContext.GenericFileResourceVersion.AsNoTracking() on rv.Id equals grv.ResourceVersionId into genericjoin
                                      from grv in genericjoin.DefaultIfEmpty()
                                      where nr.NodeId == nodeId && np.NodeId == nodeId && rv.VersionStatusEnum == VersionStatusEnum.Published && rv.Deleted == false && nr.VersionStatusEnum == VersionStatusEnum.Published && nr.Deleted == false
                                      select new CatalogueResourceViewModel
                                      {
                                          Type = r.ResourceTypeEnum.ToString(),
                                          ResourceId = nr.ResourceId.ToString(),
                                          ResourceVersionId = r.CurrentResourceVersionId.ToString(),
                                          ResourceReferenceId = rr.Id.ToString(),
                                          Title = rv.Title,
                                          Description = rv.Description,
                                          AuthoredBy = rv.ResourceVersionAuthor.FirstOrDefault().AuthorName,
                                          Organisation = rv.ResourceVersionAuthor.FirstOrDefault().Organisation,
                                          CreatedOn = rv.CreateDate.Date,
                                          AuthoredDateText = r.ResourceTypeEnum != ResourceTypeEnum.GenericFile ? string.Empty : TextHelper.CombineDateComponents(grv.AuthoredYear, grv.AuthoredMonth, grv.AuthoredDayOfMonth),
                                      }).AsQueryable();

            if (resourceViewModels != null)
            {
                var totalResources = await resourceViewModels.CountAsync();
                if (catalogueOrder == CatalogueOrder.AlphabeticalAscending)
                {
                    resourceViewModels = resourceViewModels.OrderBy(r => r.Title).Take(offset + 10);
                }
                else
                {
                    resourceViewModels = resourceViewModels.OrderByDescending(r => r.CreatedOn).Take(offset + 10);
                }

                return new CatalogueResourceResponseViewModel
                {
                    NodeId = nodeId,
                    TotalResources = totalResources,
                    CatalogueResources = resourceViewModels?.ToList<CatalogueResourceViewModel>(),
                };
            }

            return null;
        }

        /// <summary>
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        public async Task<IEnumerable<int>> GetAllPublishedResourceAsync()
        {
            return await (from nr in this.DbContext.NodeResource.AsNoTracking()
                          join rr in this.DbContext.ResourceReference.AsNoTracking() on nr.ResourceId equals rr.ResourceId
                          join np in this.DbContext.NodePath.AsNoTracking() on rr.NodePathId equals np.Id
                          join r in this.DbContext.Resource.AsNoTracking() on nr.ResourceId equals r.Id
                          join rv in this.DbContext.ResourceVersion.AsNoTracking() on r.CurrentResourceVersionId equals rv.Id
                          where nr.NodeId == np.NodeId && rv.VersionStatusEnum == VersionStatusEnum.Published && nr.VersionStatusEnum == VersionStatusEnum.Published
                          select rr.Id)
                          .ToListAsync();
        }

        /// <summary>
        /// Creates or updates the NodeResource record for a draft resource in a node.
        /// </summary>
        /// <param name="nodeId">The nodeId.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        public async Task<int> CreateOrUpdateAsync(int nodeId, int resourceId, int userId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = nodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Int) { Value = resourceId };
            var param2 = new SqlParameter("@p2", SqlDbType.Int) { Value = userId };
            var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = this.TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            return await this.DbContext.Database.ExecuteSqlRawAsync("hierarchy.NodeResourceCreateOrUpdate @p0, @p1, @p2, @p3", param0, param1, param2, param3);
        }
    }
}
