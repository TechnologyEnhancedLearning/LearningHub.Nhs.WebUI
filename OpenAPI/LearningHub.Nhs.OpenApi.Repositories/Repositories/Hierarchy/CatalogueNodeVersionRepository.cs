﻿namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The CatalogueNodeVersionRepository.
    /// </summary>
    public class CatalogueNodeVersionRepository : GenericRepository<CatalogueNodeVersion>, ICatalogueNodeVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueNodeVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CatalogueNodeVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get basic catalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public CatalogueNodeVersion GetBasicCatalogue(int catalogueNodeId)
        {
            var catalogue = DbContext.CatalogueNodeVersion.AsNoTracking()
                    .Include(cnv => cnv.NodeVersion.Node)
                    .FirstOrDefault(cnv => cnv.NodeVersion.VersionStatusEnum == VersionStatusEnum.Published
                            && cnv.NodeVersion.NodeId == catalogueNodeId);

            return catalogue;
        }

        /// <summary>
        /// The get catalogues for id list.
        /// </summary>
        /// <param name="catalogueIds">The catalogues ids.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<CatalogueNodeVersion>> GetCatalogues(List<int> catalogueIds)
        {
            return await DbContext.CatalogueNodeVersion.AsNoTracking()
                                        .Include(r => r.NodeVersion).AsNoTracking()
                                        .Include(r => r.NodeVersion.Node).AsNoTracking()
                                        .Where(r => !r.Deleted
                                            && catalogueIds.Any(catalogueId => catalogueId == r.NodeVersion.NodeId)
                                            && r.NodeVersion.Node.CurrentNodeVersionId == r.NodeVersionId
                                            && r.NodeVersion.VersionStatusEnum == VersionStatusEnum.Published).ToListAsync();
        }

        /// <summary>
        /// The get catalogues.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<CatalogueNodeVersion>> GetPublishedCatalogues()
        {
            return await DbContext.CatalogueNodeVersion.AsNoTracking()
                                        .Include(r => r.NodeVersion).AsNoTracking()
                                        .Include(r => r.NodeVersion.Node).AsNoTracking()
                                        .Where(r => !r.Deleted
                                                && r.NodeVersion.Node.CurrentNodeVersionId == r.NodeVersionId
                                                && r.NodeVersion.VersionStatusEnum == VersionStatusEnum.Published).ToListAsync();
        }

        /// <summary>
        /// The get published catalogues for user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<CatalogueNodeVersion> GetPublishedCataloguesForUserAsync(int userId)
        {
            var communityCatalogue = DbContext.CatalogueNodeVersion.AsNoTracking()
                    .Include(cnv => cnv.NodeVersion.Node)
                    .Where(cnv => cnv.NodeVersion.VersionStatusEnum == VersionStatusEnum.Published
                            && cnv.NodeVersion.NodeId == 1 /* Community Catalogue */);

            var cataloguesForUser = from cnv in DbContext.CatalogueNodeVersion.Include(cnv => cnv.NodeVersion.Node).AsNoTracking()
                                    join nv in DbContext.NodeVersion.Where(cnv => cnv.VersionStatusEnum == VersionStatusEnum.Published && !cnv.Deleted) // .Include(nv => nv.Node)
                                        on cnv.NodeVersionId equals nv.Id
                                    join s in DbContext.Scope.Where(x => !x.Deleted)
                                        on nv.NodeId equals s.CatalogueNodeId
                                    join rug in DbContext.RoleUserGroup.Where(r => r.RoleId == (int)RoleEnum.Editor && !r.Deleted)
                                        on s.Id equals rug.ScopeId
                                    join uug in DbContext.UserUserGroup.Where(u => u.UserId == userId && !u.Deleted)
                                        on rug.UserGroupId equals uug.UserGroupId
                                    join n in DbContext.Node.Where(x => !x.Deleted)
                                        on nv.Id equals n.CurrentNodeVersionId
                                    select cnv;

            var returnedCatalogues = communityCatalogue.Union(cataloguesForUser).Distinct()
                                                        .OrderBy(cnv => cnv.NodeVersion.NodeId != 1)
                                                        .ThenBy(cnv => cnv.Name);

            return returnedCatalogues;
        }

        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue view model.</param>
        /// <returns>The catalogueNodeVersionId.</returns>
        public async Task<int> CreateCatalogueAsync(int userId, CatalogueViewModel vm)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = (int)vm.Status };
                var param1 = new SqlParameter("@p1", SqlDbType.VarChar) { Value = vm.Name };
                var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = vm.Url };
                var param3 = string.IsNullOrEmpty(vm.BadgeUrl)
                    ? new SqlParameter("@p3", SqlDbType.VarChar) { Value = DBNull.Value }
                    : new SqlParameter("@p3", SqlDbType.VarChar) { Value = vm.BadgeUrl };
                var param4 = string.IsNullOrEmpty(vm.CertificateUrl)
                   ? new SqlParameter("@p4", SqlDbType.VarChar) { Value = DBNull.Value }
                   : new SqlParameter("@p4", SqlDbType.VarChar) { Value = vm.CertificateUrl };
                var param5 = string.IsNullOrEmpty(vm.CardImageUrl)
                    ? new SqlParameter("@p5", SqlDbType.VarChar) { Value = DBNull.Value }
                    : new SqlParameter("@p5", SqlDbType.VarChar) { Value = vm.CardImageUrl };
                var param6 = string.IsNullOrEmpty(vm.BannerUrl)
             ? new SqlParameter("@p6", SqlDbType.VarChar) { Value = DBNull.Value }
             : new SqlParameter("@p6", SqlDbType.VarChar) { Value = vm.BannerUrl };
                var param7 = new SqlParameter("@p7", SqlDbType.Int) { Value = (int)vm.ResourceOrder };
                var param8 = new SqlParameter("@p8", SqlDbType.VarChar) { Value = vm.Description };
                var param9 = new SqlParameter("@p9", SqlDbType.Int) { Value = userId };
                var param10 = new SqlParameter("@p10", SqlDbType.Bit) { Value = vm.Hidden };
                var param11 = new SqlParameter("@p11", SqlDbType.VarChar) { Value = string.Join(",", vm.Keywords) };
                var param12 = new SqlParameter("@p12", SqlDbType.Bit) { Value = vm.RestrictedAccess };
                var param13 = new SqlParameter("@p13", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
                var param14 = new SqlParameter("@p14", SqlDbType.Int) { Value = vm.CatalogueNodeVersionProvider.ProviderId };
                var param15 = new SqlParameter("@p15", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await DbContext.Database.ExecuteSqlRawAsync("hierarchy.CatalogueCreate @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15 output", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15);

                int catalogueNodeVersionId = (int)param15.Value;

                return catalogueNodeVersionId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue view model.</param>
        /// <returns>The task.</returns>
        public async Task UpdateCatalogueAsync(int userId, CatalogueViewModel vm)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = (int)vm.Status };
                var param1 = new SqlParameter("@p1", SqlDbType.VarChar) { Value = vm.Name };
                var param2 = string.IsNullOrEmpty(vm.BadgeUrl)
                    ? new SqlParameter("@p2", SqlDbType.VarChar) { Value = DBNull.Value }
                    : new SqlParameter("@p2", SqlDbType.VarChar) { Value = vm.BadgeUrl };
                var param3 = string.IsNullOrEmpty(vm.CardImageUrl)
                   ? new SqlParameter("@p3", SqlDbType.VarChar) { Value = DBNull.Value }
                   : new SqlParameter("@p3", SqlDbType.VarChar) { Value = vm.CardImageUrl };
                var param4 = string.IsNullOrEmpty(vm.BannerUrl)
                   ? new SqlParameter("@p4", SqlDbType.VarChar) { Value = DBNull.Value }
                   : new SqlParameter("@p4", SqlDbType.VarChar) { Value = vm.BannerUrl };
                var param5 = string.IsNullOrEmpty(vm.CertificateUrl)
                    ? new SqlParameter("@p5", SqlDbType.VarChar) { Value = DBNull.Value }
                    : new SqlParameter("@p5", SqlDbType.VarChar) { Value = vm.CertificateUrl };
                var param6 = new SqlParameter("@p6", SqlDbType.Int) { Value = (int)vm.ResourceOrder };
                var param7 = new SqlParameter("@p7", SqlDbType.VarChar) { Value = vm.Description };
                var param8 = new SqlParameter("@p8", SqlDbType.Int) { Value = userId };
                var param9 = new SqlParameter("@p9", SqlDbType.VarChar) { Value = string.Join(",", vm.Keywords) };
                var param10 = new SqlParameter("@p10", SqlDbType.Int) { Value = vm.CatalogueNodeVersionId };
                var param11 = new SqlParameter("@p11", SqlDbType.Bit) { Value = vm.RestrictedAccess };
                var param12 = new SqlParameter("@p12", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };
                var param13 = new SqlParameter("@p13", SqlDbType.Int) { Value = vm.CatalogueNodeVersionProvider.ProviderId };

                await DbContext.Database.ExecuteSqlRawAsync("hierarchy.CatalogueUpdate @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13", param0, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue owner view model.</param>
        /// <returns>The task.</returns>
        public async Task UpdateCatalogueOwnerAsync(int userId, CatalogueOwnerViewModel vm)
        {
            try
            {
                var param0 = new SqlParameter("@p0", SqlDbType.VarChar) { Value = vm.OwnerName };
                var param1 = new SqlParameter("@p1", SqlDbType.VarChar) { Value = vm.OwnerEmailAddress };
                var param2 = new SqlParameter("@p2", SqlDbType.VarChar) { Value = vm.Notes };
                var param3 = new SqlParameter("@p3", SqlDbType.Int) { Value = userId };
                var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = vm.CatalogueNodeVersionId };
                var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

                await DbContext.Database.ExecuteSqlRawAsync("hierarchy.CatalogueOwnerUpdate @p0, @p1, @p2, @p3, @p4, @p5", param0, param1, param2, param3, param4, param5);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Catlogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The CatalogueViewModel.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(string reference)
        {
            return await (from cnv in DbContext.CatalogueNodeVersion.AsNoTracking()
                          join nv in DbContext.NodeVersion.AsNoTracking() on cnv.NodeVersionId equals nv.Id
                          join n in DbContext.Node.AsNoTracking() on cnv.NodeVersionId equals n.CurrentNodeVersionId
                          where cnv.Url != null && cnv.Url.ToLower() == reference.ToLower() && cnv.Deleted == false && nv.VersionStatusEnum == VersionStatusEnum.Published
                          select new CatalogueViewModel
                          {
                              Id = cnv.Id,
                              NodeVersionId = cnv.NodeVersionId,
                              BadgeUrl = cnv.BadgeUrl,
                              BannerUrl = cnv.BannerUrl,
                              CertificateUrl = cnv.CertificateUrl,
                              Name = cnv.Name,
                              Description = cnv.Description,
                              Notes = cnv.Notes,
                              Url = cnv.Url,
                              NodeId = n.Id,
                              CatalogueNodeVersionId = cnv.NodeVersionId,
                              ResourceOrder = cnv.Order,
                              RestrictedAccess = cnv.RestrictedAccess,
                              Hidden = n.Hidden,
                          }).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get list of Restricted Catalogue AccessRequests for the supplied request.
        /// </summary>
        /// <param name="restrictedCatalogueAccessRequestsRequestViewModel">The restrictedCatalogueAccessRequestsRequestViewModel.</param>
        /// <returns>A RestrictedCatalogueAccessRequestsViewModel.</returns>
        public List<RestrictedCatalogueAccessRequestViewModel> GetRestrictedCatalogueAccessRequests(RestrictedCatalogueAccessRequestsRequestViewModel restrictedCatalogueAccessRequestsRequestViewModel)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = restrictedCatalogueAccessRequestsRequestViewModel.CatalogueNodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.Bit) { Value = restrictedCatalogueAccessRequestsRequestViewModel.IncludeNew };
            var param2 = new SqlParameter("@p2", SqlDbType.Bit) { Value = restrictedCatalogueAccessRequestsRequestViewModel.IncludeApproved };
            var param3 = new SqlParameter("@p3", SqlDbType.Bit) { Value = restrictedCatalogueAccessRequestsRequestViewModel.IncludeDenied };

            var restrictedCatalogueAccessRequests = DbContext.RestrictedCatalogueAccessRequestViewModel.FromSqlRaw("hub.RestrictedCatalogueGetAccessRequests @p0, @p1, @p2, @p3", param0, param1, param2, param3).ToList();

            return restrictedCatalogueAccessRequests;
        }

        /// <summary>
        /// Get list of Restricted Catalogue Users for the supplied request.
        /// </summary>
        /// <param name="restrictedCatalogueUsersRequestViewModel">The restrictedCatalogueUsersRequestViewModel.</param>
        /// <returns>A RestrictedCatalogueUsersViewModel.</returns>
        public RestrictedCatalogueUsersViewModel GetRestrictedCatalogueUsers(RestrictedCatalogueUsersRequestViewModel restrictedCatalogueUsersRequestViewModel)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = restrictedCatalogueUsersRequestViewModel.CatalogueNodeId };
            var param1 = new SqlParameter("@p1", SqlDbType.NVarChar) { Value = string.IsNullOrEmpty(restrictedCatalogueUsersRequestViewModel.EmailAddressFilter) ? string.Empty : restrictedCatalogueUsersRequestViewModel.EmailAddressFilter };
            var param2 = new SqlParameter("@p2", SqlDbType.Bit) { Value = restrictedCatalogueUsersRequestViewModel.IncludeCatalogueAdmins };
            var param3 = new SqlParameter("@p3", SqlDbType.Bit) { Value = restrictedCatalogueUsersRequestViewModel.IncludePlatformAdmins };
            var param4 = new SqlParameter("@p4", SqlDbType.Int) { Value = restrictedCatalogueUsersRequestViewModel.Skip };
            var param5 = new SqlParameter("@p5", SqlDbType.Int) { Value = restrictedCatalogueUsersRequestViewModel.Take };
            var param6 = new SqlParameter("@p6", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var restrictedCatalogueUsers = DbContext.RestrictedCatalogueUserViewModel.FromSqlRaw("hub.RestrictedCatalogueGetUsers @p0, @p1, @p2, @p3, @p4, @p5, @p6 output", param0, param1, param2, param3, param4, param5, param6).ToList();

            var vm = new RestrictedCatalogueUsersViewModel();
            vm.RestrictedCatalogueUsers = restrictedCatalogueUsers;
            vm.UserCount = (int)param6.Value;

            return vm;
        }

        /// <summary>
        /// Get Restricted Catalogue Summary for the supplied catalogue node id.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>A RestrictedCatalogueUsersViewModel.</returns>
        public RestrictedCatalogueSummaryViewModel GetRestrictedCatalogueSummary(int catalogueNodeId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = catalogueNodeId };

            var vm = DbContext.RestrictedCatalogueSummaryViewModel.FromSqlRaw("hub.RestrictedCatalogueGetSummary @p0", param0).AsEnumerable().FirstOrDefault();

            return vm;
        }

        /// <summary>
        /// Gets catalogues for dashboard based on type.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>resources.</returns>
        public (int TotalCount, List<DashboardCatalogueDto> Catalogues) GetCatalogues(string dashboardType, int pageNumber, int userId)
        {
            var param0 = new SqlParameter("@DashboardType", SqlDbType.NVarChar, 30) { Value = dashboardType };
            var param1 = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber };
            var param3 = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var dashboardCatalogues = DbContext.DashboardCatalogueDto.FromSqlRaw("[hierarchy].[GetDashboardCatalogues] @DashboardType, @UserId, @pageNumber, @TotalRecords OUTPUT", param0, param1, param2, param3).ToList();

            return (TotalCount: (int)param3.Value, Catalogues: dashboardCatalogues);
        }

        /// <inheritdoc/>
        public async Task ShowCatalogue(int userId, int nodeId)
        {
            var param0 = new SqlParameter("@NodeId", SqlDbType.Int) { Value = nodeId };
            var param1 = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
            var param2 = new SqlParameter("@UserTimezoneOffset", SqlDbType.Int) { Value = TimezoneOffsetManager.UserTimezoneOffset ?? (object)DBNull.Value };

            await DbContext.Database.ExecuteSqlRawAsync("[hierarchy].[CatalogueShow] @NodeId, @UserId, @UserTimezoneOffset", param0, param1, param2);
        }

        /// <summary>
        /// Check if a Catalogue with a specific name exists or not.
        /// </summary>
        /// <param name="name">The catalogue name.</param>
        /// <returns>True if the catalogue exists, otherwise false.</returns>
        public async Task<bool> ExistsAsync(string name)
        {
            return await DbContext.CatalogueNodeVersion.AnyAsync(x => x.Name.Equals(name) && !x.Deleted);
        }

        /// <summary>
        /// Gets the Node Id for a particular catalogue name.
        /// </summary>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <returns>The catalogue's node id.</returns>
        public async Task<int> GetNodeIdByCatalogueName(string catalogueName)
        {
            return await (from cnv in DbContext.CatalogueNodeVersion.AsNoTracking()
                          join nv in DbContext.NodeVersion.AsNoTracking() on cnv.NodeVersionId equals nv.Id
                          where cnv.Name == catalogueName && cnv.Deleted == false
                          select nv.NodeId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets catalogues count based on alphabets.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>resources.</returns>
        public List<AllCatalogueAlphabetModel> GetAllCataloguesAlphaCount(int userId)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };

            var result = DbContext.AllCatalogueAlphabetModel.FromSqlRaw("[hierarchy].[GetCataloguesCount] @userid", param0)
              .AsNoTracking().ToList();
            return result;
        }

        /// <summary>
        /// Gets catalogues based on filter character.
        /// </summary>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="filterChar">The filterChar.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>resources.</returns>
        public async Task<List<AllCatalogueViewModel>> GetAllCataloguesAsync(int pageSize, string filterChar, int userId)
        {
            var param0 = new SqlParameter("@userId", SqlDbType.Int) { Value = userId };
            var param1 = new SqlParameter("@filterChar", SqlDbType.NVarChar, 10) { Value = filterChar.Trim() };
            var param2 = new SqlParameter("@OffsetRows", SqlDbType.Int) { Value = 0 };
            var param3 = new SqlParameter("@fetchRows", SqlDbType.Int) { Value = pageSize };

            var result = await DbContext.AllCatalogueViewModel.FromSqlRaw("[hierarchy].[GetCatalogues] @userId, @filterChar, @OffsetRows, @fetchRows", param0, param1, param2, param3)
              .AsNoTracking().ToListAsync();
            return result;
        }
    }
}
