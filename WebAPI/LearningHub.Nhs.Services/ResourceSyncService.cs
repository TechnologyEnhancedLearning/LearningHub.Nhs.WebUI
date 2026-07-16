namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Helpers;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource sync service.
    /// </summary>
    public class ResourceSyncService : IResourceSyncService
    {
        private readonly IResourceSyncRepository resourceSyncRepository;
        private readonly IResourceVersionRepository resourceVersionRepository;
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private readonly IGenericFileResourceVersionRepository genericFileResourceVersionRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSyncService"/> class.
        /// </summary>
        /// <param name="resourceSyncRepository">The resourceSyncRepository.</param>
        /// <param name="resourceVersionRepository">The resourceVersionRepository.</param>
        /// <param name="catalogueNodeVersionRepository">The catalogueNodeVersionRepository.</param>
        /// <param name="genericFileResourceVersionRepository">The genericFileResourceVersionRepository.</param>
        /// <param name="mapper">The mapper.</param>
        public ResourceSyncService(
            IResourceSyncRepository resourceSyncRepository,
            IResourceVersionRepository resourceVersionRepository,
            ICatalogueNodeVersionRepository catalogueNodeVersionRepository,
            IGenericFileResourceVersionRepository genericFileResourceVersionRepository,
            IMapper mapper)
        {
            this.resourceSyncRepository = resourceSyncRepository;
            this.resourceVersionRepository = resourceVersionRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.genericFileResourceVersionRepository = genericFileResourceVersionRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// The AddToSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        public async Task AddToSyncListAsync(int userId, List<int> resourceIds)
        {
            await this.resourceSyncRepository.AddToSyncListAsync(userId, resourceIds);
        }

        /// <summary>
        /// The RemoveFromSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        public async Task RemoveFromSyncListAsync(int userId, List<int> resourceIds)
        {
            await this.resourceSyncRepository.RemoveFromSyncListAsync(userId, resourceIds);
        }

        /// <summary>
        /// The GetSyncListForUser.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="includeResources">If the resource property should be populated.</param>
        /// <returns>The resource syncs.</returns>
        public List<ResourceSync> GetSyncListForUser(int userId, bool includeResources)
        {
            return this.resourceSyncRepository.GetSyncListForUser(userId, includeResources).ToList();
        }

        /// <summary>
        /// The GetSyncListResourcesForUser.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The list of resources.</returns>
        public List<ResourceAdminSearchResultViewModel> GetSyncListResourcesForUser(int userId)
        {
            var list = this.GetSyncListForUser(userId, true).Select(x => x.Resource);

            return this.mapper.ProjectTo<ResourceAdminSearchResultViewModel>(list.AsQueryable()).ToList();
        }

        /// <summary>
        /// The SyncForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> SyncForUserAsync(int userId)
        {
            await this.resourceSyncRepository.SetSyncedForUserAsync(userId);

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The BuildSearchResourceRequestModelList.
        /// 04-11-20 KD TODO / Note:
        /// This services a temporary mechanism to build the correct data set for Findwise.
        /// This should be refactored to potentially use stored proc for this data retrieval - e.g. using common View/s.
        /// Refactor bulk Findwise Sync operation to Azure Function Activity.
        /// this method processes a Resource Version dataset to only retain active Resource Reference and Catalogue information.
        /// Supplements formatted "authored date string" for Generic file resource type.
        /// Note - require "IsActive" indication on the NodePath ongoing - only publish Resource Reference with an active node path.
        /// (i.e. var activeNodePaths = activeNodeResources.Select(s => s.Node.NodePaths.Where(np => np.IsActive));)
        /// Ongoing - may not wish to return Resource Reference from Findwise - currently only holds one Resource Reference but there may be multiple.
        /// </summary>
        /// <param name="resourceVersionIds">The resource version id list.</param>
        /// <returns>The task.</returns>
        public async Task<List<SearchResourceRequestModel>> BuildSearchResourceRequestModelList(List<int> resourceVersionIds)
        {
            var resourceVersions = await this.resourceVersionRepository.GetResourceVersionsForSearchSubmission(resourceVersionIds);
            var catalogues = await this.catalogueNodeVersionRepository.GetPublishedCatalogues();

            var resourceCatalogueDictionary = new Dictionary<int, List<int>>();
            var resourceReferenceDictionary = new Dictionary<int, List<int>>();
            var resourceAuthoredDateDictionary = new Dictionary<int, string>();

            foreach (var rv in resourceVersions)
            {
                var activeResourceReferences = new List<ResourceReference>();
                var activeNodeResources = rv.Resource.NodeResource.Where(nr => nr.VersionStatusEnum == Models.Enums.VersionStatusEnum.Published);
                var activeNodePaths = activeNodeResources.SelectMany(s => s.Node.NodePaths.Where(np => np.IsActive));
                var activeCatalogues = activeNodePaths.Select(s => s.CatalogueNodeId).Distinct().ToList();

                foreach (var rr in rv.Resource.ResourceReference)
                {
                    if (activeNodePaths.Any(np => np.Id == rr.NodePathId))
                    {
                        activeResourceReferences.Add(rr);
                    }
                }

                // Record additional data for submission to Findwise
                resourceReferenceDictionary.Add(rv.Resource.Id, activeResourceReferences.Select(s => s.OriginalResourceReferenceId).Distinct().ToList());
                resourceCatalogueDictionary.Add(rv.Resource.Id, activeCatalogues);

                if (rv.Resource.ResourceTypeEnum == Models.Enums.ResourceTypeEnum.GenericFile)
                {
                    var genericFileResourceVersion = await this.genericFileResourceVersionRepository.GetByResourceVersionIdAsync(rv.Id);
                    resourceAuthoredDateDictionary.Add(rv.Resource.Id, TextHelper.CombineDateComponents(genericFileResourceVersion.AuthoredYear, genericFileResourceVersion.AuthoredMonth, genericFileResourceVersion.AuthoredDayOfMonth));
                }
            }

            // Automapper for basic properties
            var searchResourceRequestModels = resourceVersions.Select(x => this.mapper.Map<SearchResourceRequestModel>(x)).ToList();

            // Append extra resource reference, catalogue info, formatted author date string.
            // Findwise currently only supports one resource reference.
            foreach (var r in searchResourceRequestModels)
            {
                int resourceId = int.Parse(r.Id);
                List<int> resourceReferenceIds = new List<int>();
                resourceReferenceDictionary.TryGetValue(resourceId, out resourceReferenceIds);
                r.ResourceReferenceId = resourceReferenceDictionary[resourceId].FirstOrDefault();

                List<int> catalogueIds = new List<int>();
                resourceCatalogueDictionary.TryGetValue(resourceId, out catalogueIds);
                r.CatalogueIds = catalogueIds;
                r.LocationPaths = catalogues.Where(c => r.CatalogueIds.Contains(c.NodeVersion.NodeId)).Select(c => c.Name).ToList();

                // Apply formatted authored  date string if applicable.
                string authoredDateString = string.Empty;
                resourceAuthoredDateDictionary.TryGetValue(resourceId, out authoredDateString);
                r.AuthoredDateString = authoredDateString;
            }

            return searchResourceRequestModels;
        }

        /// <summary>
        /// The BuildSearchResourceRequestModel.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The task.</returns>
        public async Task<SearchResourceRequestModel> BuildSearchResourceRequestModel(int resourceVersionId)
        {
            var resourceVersionIds = new List<int>() { resourceVersionId };
            var searchResourceRequestModels = await this.BuildSearchResourceRequestModelList(resourceVersionIds);
            return searchResourceRequestModels.First();
        }

        /// <summary>
        /// The sync single async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The task.</returns>
        public async Task<LearningHubValidationResult> SyncSingleAsync(int resourceVersionId)
        {
            var resourceVersion = this.GetResourcesWithIncludes().SingleOrDefault(x => x.Id == resourceVersionId);
            if (resourceVersion == null)
            {
                throw new Exception($"No resource version found for id '{resourceVersionId}'");
            }          

            return new LearningHubValidationResult(true);
        }

        private bool ResourceIsUnpublished(ResourceVersion r)
        {
            return r.VersionStatusEnum == Models.Enums.VersionStatusEnum.Unpublished
                || r.VersionStatusEnum == Models.Enums.VersionStatusEnum.Draft;
        }

        private IQueryable<ResourceVersion> GetResourcesWithIncludes()
        {
            return this.resourceVersionRepository.GetAll()
                .Include(x => x.CreateUser)
                .Include(x => x.Resource).ThenInclude(x => x.ResourceReference)
                .Include(x => x.ResourceVersionKeyword)
                .Include(x => x.ResourceVersionAuthor)
                .Include(x => x.ResourceVersionRatingSummary)
                .Include(x => x.Publication);
        }
    }
}
