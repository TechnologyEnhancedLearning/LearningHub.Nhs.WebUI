namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using CatalogueViewModel = LearningHub.Nhs.Models.Catalogue.CatalogueViewModel;

    /// <summary>
    /// The resource service.
    /// </summary>
    public class CatalogueService : ICatalogueService
    {
        /// <summary>
        /// The learning hub service.
        /// </summary>
        private readonly ICatalogueRepository catalogueRepository;
        private readonly IMapper mapper;
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private readonly INodeResourceRepository nodeResourceRepository;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly IRoleUserGroupRepository roleUserGroupRepository;

        private readonly IProviderService providerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueService"/> class.
        /// The catalogue service.
        /// </summary>
        /// <param name="catalogueRepository">
        /// The <see cref="ICatalogueRepository"/>.
        /// </param>
        /// <param name="mapper"></param>
        /// <param name="catalogueNodeVersionRepository"></param>
        /// <param name="nodeResourceRepository"></param>
        /// <param name="resourceVersionRepository"></param>
        /// <param name="roleUserGroupRepository"></param>
        /// <param name="providerService"></param>
        public CatalogueService(ICatalogueRepository catalogueRepository, IMapper mapper, ICatalogueNodeVersionRepository catalogueNodeVersionRepository, INodeResourceRepository nodeResourceRepository, IResourceVersionRepository resourceVersionRepository, IRoleUserGroupRepository roleUserGroupRepository, IProviderService providerService)
        {
            this.catalogueRepository = catalogueRepository;
            this.mapper = mapper;
            this.nodeResourceRepository = nodeResourceRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.roleUserGroupRepository = roleUserGroupRepository;
            this.providerService = providerService;
        }

        /// <summary>
        /// Get all catalogues async.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        public async Task<BulkCatalogueViewModel> GetAllCatalogues()
        {
            var catalogueNodeVersions = await this.catalogueRepository.GetAllCatalogues();
            var catalogueViewModels = catalogueNodeVersions.Select(c => new Models.ViewModels.CatalogueViewModel(c)).ToList();
            return new BulkCatalogueViewModel(catalogueViewModels);
        }


        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The catalogue view model.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(int id)
        {
            var catalogue = await this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion).ThenInclude(x => x.Node)
                .Include(x => x.Keywords)
                .Include(x => x.CatalogueNodeVersionProvider).Where(x => !x.Deleted)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (catalogue == null)
            {
                return null;
            }

            var vm = this.mapper.Map<CatalogueViewModel>(catalogue);

            // Used by the admin screen to inform the admin user if they need to add a user group.
            vm.HasUserGroup = this.GetRoleUserGroupsForCatalogue(vm.NodeId).Any();
            vm.Providers = await this.providerService.GetAllAsync();
            return vm;
        }


        /// <summary>
        /// The GetResources.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="id">The catalogueId.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="sortColumn">The sortColumn.</param>
        /// <param name="sortDirection">The sortDirection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The catalogueResourcesViewModel.</returns>
        public async Task<CatalogueResourcesViewModel> GetResourcesAsync(int userId, int id, int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "")
        {
            var catalogueNodeVersion = await this.catalogueNodeVersionRepository.GetAll()
                .Include(x => x.NodeVersion)
                .ThenInclude(x => x.Node)
                .SingleOrDefaultAsync(x => x.Id == id);
            var nodeId = catalogueNodeVersion.NodeVersion.NodeId;
            var nodeResources = this.nodeResourceRepository.GetAll()
                .Include(x => x.Resource)
                .Include(x => x.Node)
                .ThenInclude(x => x.NodePathNodes)
                .ThenInclude(x => x.NodePath)
                .Where(x => x.Node.NodePathNodes.Where(n => n.NodePath.CatalogueNodeId == nodeId).Count() > 0);

            var resourceVersions = this.resourceVersionRepository.GetAllAdminSearch(userId);

            var catalogueResourceVersions = resourceVersions.Join(
                nodeResources,
                rv => rv.ResourceId,
                nr => nr.ResourceId,
                (rv, nr) => rv);

            var filterCriteria = JsonConvert.DeserializeObject<List<PagingColumnFilter>>(filter);

            PagedResultSet<ResourceAdminSearchResultViewModel> result = new PagedResultSet<ResourceAdminSearchResultViewModel>();

            var items = catalogueResourceVersions;

            items = this.FilterItems(items, filterCriteria);

            result.TotalItemCount = items.Count();

            items = this.OrderItems(items, sortColumn, sortDirection);

            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            result.Items = this.mapper.ProjectTo<ResourceAdminSearchResultViewModel>(items).ToList();

            return new CatalogueResourcesViewModel
            {
                CatalogueName = catalogueNodeVersion.Name,
                Resources = result,
            };
        }

        /// <summary>
        /// The GetResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<CatalogueResourceResponseViewModel> GetResourcesAsync(CatalogueResourceRequestViewModel requestViewModel)
        {
            return await this.nodeResourceRepository.GetResourcesAsync(requestViewModel.NodeId, requestViewModel.CatalogueOrder, requestViewModel.Offset);
        }

        /// <summary>
        /// The GetRolesForCatalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="includeUser">The includeUser.</param>
        /// <returns>The roleUserGroups.</returns>
        public List<RoleUserGroup> GetRoleUserGroupsForCatalogue(int catalogueNodeId, bool includeUser = false)
        {
            IQueryable<RoleUserGroup> query = this.roleUserGroupRepository.GetAll()
                .Include(x => x.Scope);
            if (includeUser)
            {
                query = query.Include(x => x.UserGroup).ThenInclude(x => x.UserUserGroup);
            }

            return query.Where(x => x.Scope.CatalogueNodeId == catalogueNodeId).ToList();
        }


        /// <summary>
        /// Filter the items for resource version search.
        /// </summary>
        /// <param name="items">The items<see cref="IQueryable{ResourceVersion}"/>.</param>
        /// <param name="filterCriteria">The filterCriteria<see cref="List{PagingColumnFilter}"/>.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersion}"/>.</returns>
        private IQueryable<ResourceVersion> FilterItems(IQueryable<ResourceVersion> items, List<PagingColumnFilter> filterCriteria)
        {
            if (filterCriteria == null || filterCriteria.Count == 0)
            {
                return items;
            }

            foreach (var filter in filterCriteria)
            {
                switch (filter.Column.ToLower())
                {
                    case "resourceversionid":
                        items = items.Where(x => x.Id == int.Parse(filter.Value));
                        break;
                    case "resourcereference":
                        items = items.Where(x => x.Resource.ResourceReference.Any(resRef => resRef.OriginalResourceReferenceId == int.Parse(filter.Value)));
                        break;
                    case "title":
                        items = items.Where(x => x.Title.Contains(filter.Value));
                        break;
                    case "createuser":
                        items = items.Where(x => x.CreateUser.UserName.Contains(filter.Value));
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Order the items for resource version search.
        /// </summary>
        /// <param name="items">The items<see cref="IQueryable{ResourceVersion}"/>.</param>
        /// <param name="sortColumn">The sortColumn<see cref="string"/>.</param>
        /// <param name="sortDirection">The sortDirection<see cref="string"/>.</param>
        /// <returns>The <see cref="IQueryable{ResourceVersion}"/>.</returns>
        private IQueryable<ResourceVersion> OrderItems(IQueryable<ResourceVersion> items, string sortColumn, string sortDirection)
        {
            switch (sortColumn.ToLower())
            {
                case "createuser":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.CreateUser.UserName);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.CreateUser.UserName);
                    }

                    break;
                case "type":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Resource.ResourceTypeEnum);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Resource.ResourceTypeEnum);
                    }

                    break;
                case "status":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.VersionStatusEnum);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.VersionStatusEnum);
                    }

                    break;
                case "title":
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Title);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Title);
                    }

                    break;
                default:
                    if (sortDirection == "D")
                    {
                        items = items.OrderByDescending(x => x.Resource.Id);
                    }
                    else
                    {
                        items = items.OrderBy(x => x.Resource.Id);
                    }

                    break;
            }

            return items;
        }
    }
}
