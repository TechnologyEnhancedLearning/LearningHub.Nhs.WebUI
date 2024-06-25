namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.AzureMediaAsset;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The resource service.
    /// </summary>
    public class ResourceService : IResourceService
    {
        /// <summary>
        /// The learning hub service.
        /// </summary>
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly ILogger<ResourceService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="learningHubService">
        /// The <see cref="ILearningHubService"/>.
        /// </param>
        /// <param name="resourceRepository">
        /// The <see cref="IResourceRepository"/>.
        /// </param>
        public ResourceService(ILearningHubService learningHubService, IResourceRepository resourceRepository, ILogger<ResourceService> logger)
        {
            this.learningHubService = learningHubService;
            this.resourceRepository = resourceRepository;
            this.logger = logger;
        }

        /// <summary>
        /// the get by original resource reference id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">the id.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId, int? currentUserId)
        {
            // qqqqq do via constructor on the model

            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs = (await this.resourceRepository.GetResourceReferenceAndCatalogues(new List<int>() { },new List<int>() { originalResourceReferenceId })).ToList();
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };

            try
            {
                ResourceReferenceAndCatalogueDTO? resourceReferenceAndCatalogueDTO = resourceReferenceAndCatalogueDTOs.SingleOrDefault(); // qqqq this could fail i think as it may need to be grouped

                if (resourceReferenceAndCatalogueDTO == null)
                {
                    throw new HttpResponseException("No matching resource reference", HttpStatusCode.NotFound);
                }


                if (currentUserId.HasValue) {
                    List<int> resourceIds = new List<int>() { resourceReferenceAndCatalogueDTO.ResourceId }; // QQQQ the single or default means it has to be single rewrite function logic 
                    List<int> userIds = new List<int>() { currentUserId.Value };

                    // qqqq do i need to null handle with this
                    resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };

                }

                return this.GetResourceReferenceWithResourceDetailsViewModel(resourceReferenceAndCatalogueDTO, resourceActivities);
            }
            catch (InvalidOperationException exception)
            {
                this.logger.LogError(exception, $"Multiple resource references found with OriginalResourceReferenceId {originalResourceReferenceId}");
                throw;
            }
        }

        /// <summary>
        /// the get by  resource reference id async.
        /// </summary>
        /// <param name="resourceId">the id.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<ResourceMetadataViewModel> GetResourceById(int resourceId, int? currentUserId)
        {
            // qqqqq do via constructor on the model

            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            var resourceIdList = new List<int>() { resourceId };

            ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO = (await this.resourceRepository.GetResourceReferenceAndCatalogues(resourceIdList, new List<int>() { })).SingleOrDefault() ?? new ResourceReferenceAndCatalogueDTO();

            if (resourceReferenceAndCatalogueDTO == null)
            {
                throw new Exception($"Resource with ID {resourceId} not found. Provided userId was {(currentUserId.HasValue ? currentUserId.Value.ToString() : "null")}");
            }

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = new List<int>() { resourceId };
                List<int> userIds = new List<int>() { currentUserId.Value };

                // qqqq do i need to null handle with this
                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            majorVersionIdActivityStatusDescription = resourceActivities.Count != 0 ? ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReferenceAndCatalogueDTO, resourceActivities).ToList() : new List<MajorVersionIdActivityStatusDescription>() { } ;

            return new ResourceMetadataViewModel(
                resourceReferenceAndCatalogueDTO.ResourceId,
                resourceReferenceAndCatalogueDTO.Title ?? ResourceHelpers.NoResourceVersionText,
                resourceReferenceAndCatalogueDTO.Description ?? string.Empty,
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Select(rr => new ResourceReferenceViewModel(
                                                        rr.OriginalResourceReferenceId,
                                                        rr.GetCatalogue(),
                                                        this.learningHubService.GetResourceLaunchUrl(rr.OriginalResourceReferenceId)))
                                                        .ToList(),
                resourceReferenceAndCatalogueDTO.GetResourceTypeNameOrEmpty(),
                resourceReferenceAndCatalogueDTO.MajorVersion, /*qqqq wont stay first or default because of logic in stored procedure*/
                resourceReferenceAndCatalogueDTO.Rating ?? 0.0m,
                majorVersionIdActivityStatusDescription); //qqqq
        }

        /// <summary>
        /// bulk get by ids async.
        /// </summary>
        /// <param name="originalResourceReferenceIds">the resource reference ids.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId)
        {
            // qqqqq do via constructor on the model

            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs = (await this.resourceRepository.GetResourceReferenceAndCatalogues(new List<int>() { }, originalResourceReferenceIds)).ToList();

            List<int> matchedOriginalResourceIds = resourceReferenceAndCatalogueDTOs.SelectMany(r => r.CatalogueDTOs.Select(x => x.OriginalResourceReferenceId)).Distinct().ToList<int>();
            List<int> unmatchedOriginalResourceIds = originalResourceReferenceIds.Except(matchedOriginalResourceIds).ToList();

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourceReferenceAndCatalogueDTOs.Select(rrl => rrl.ResourceId).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            if (unmatchedOriginalResourceIds.Any())
            {
                this.logger.LogInformation("Some resource ids not matched");
            }

            foreach (var duplicateIds in unmatchedOriginalResourceIds.GroupBy(id => id).Where(groupOfIds => groupOfIds.Count() > 1))
            {
                // qqqq so this is suggesting by design we shouldnt have multiple originalIds
                this.logger.LogWarning($"Duplicate originalResourceId requested and not found with OriginalResourceReferenceId {duplicateIds.First()}");
            }

            var matchedResources = resourceReferenceAndCatalogueDTOs
                .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities.Where(ra => ra.ResourceId == rr.ResourceId).ToList()))
                .ToList();

            return new BulkResourceReferenceViewModel(matchedResources, unmatchedOriginalResourceIds);
        }

        private ResourceReferenceWithResourceDetailsViewModel GetResourceReferenceWithResourceDetailsViewModel(ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO, List<ResourceActivityDTO> resourceActivities)
        {
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReferenceAndCatalogueDTO, resourceActivities).ToList();
            }

            //qqqq is it possible not to have a currentResourceVersion i dont think so ... check this - but based on proc its not needed
            // var hasCurrentResourceVersion = resourceReferenceAndCatalogueDTO.CurrentResourceVersion != null;
            //var hasCurrentResourceVersion = resourceReferenceAndCatalogueDTO.ResourceReferenceId != null;
            var hasRating = resourceReferenceAndCatalogueDTO.Rating != null;

            if (resourceReferenceAndCatalogueDTO == null)
            {
                throw new Exception("No matching resource");
            }

            //if (!hasCurrentResourceVersion)//qqqq can it not have one??
            //{
            //    this.logger.LogInformation($"Resource with OriginalResourceReferenceId {resourceReferenceAndCatalogueDTO.OriginalResourceReferenceId} is missing a current resource version");
            //}

            if (!hasRating)
            {
                this.logger.LogInformation($"Resource with Id: {resourceReferenceAndCatalogueDTO.ResourceId} is missing a ResourceVersionRatingSummary");
            }

            var resourceTypeNameOrEmpty = ResourceHelpers.GetResourceTypeNameOrEmpty(resourceReferenceAndCatalogueDTO);
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resourceReferenceAndCatalogueDTO.ResourceTypeEnum}");
            }

            return new ResourceReferenceWithResourceDetailsViewModel(
                resourceReferenceAndCatalogueDTO.ResourceId,
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().OriginalResourceReferenceId, // qqqq assumes that when got by originalResourceId there is only one catalogue and it hasnt been nulled because it is external
                resourceReferenceAndCatalogueDTO.Title ?? ResourceHelpers.NoResourceVersionText,
                resourceReferenceAndCatalogueDTO.Description ?? string.Empty,
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().GetCatalogue(), // qqqq assumes that when got by originalResourceId there is only one catalogue and it hasnt been nulled because it is external
                resourceTypeNameOrEmpty,
                resourceReferenceAndCatalogueDTO.MajorVersion,
                resourceReferenceAndCatalogueDTO.Rating ?? 0,
                this.learningHubService.GetResourceLaunchUrl(resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().OriginalResourceReferenceId), // qqqq assumes that when got by originalResourceId there is only one catalogue and it hasnt been nulled because it is external
                majorVersionIdActivityStatusDescription);
        }

        /// <summary>
        /// delete me.
        /// </summary>
        public void QqqqTest() {
            this.resourceRepository.QqqqTest();
        }
    }
}
