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
            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs = (await this.resourceRepository.GetResourceReferenceAndCatalogues(new List<int>() { }, new List<int>() { originalResourceReferenceId })).ToList();
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };

            try
            {
                //Single because a singular originalResourceId should only return one catalogue and one row from the stored procedure
                ResourceReferenceAndCatalogueDTO? resourceReferenceAndCatalogueDTO = resourceReferenceAndCatalogueDTOs.SingleOrDefault();

                if (resourceReferenceAndCatalogueDTO == null)
                {
                    throw new HttpResponseException("No matching resource reference", HttpStatusCode.NotFound);
                }

                // Check only one CatalogueDTOs for single originalResourceId
                if (resourceReferenceAndCatalogueDTO.CatalogueDTOs.Count != 1)
                {
                    throw new ArgumentException(
                        $"For one originalResourceId only one CatalogueDTOs should be found. Count was {resourceReferenceAndCatalogueDTO.CatalogueDTOs.Count}.");
                }


                if (currentUserId.HasValue) {
                    List<int> resourceIds = new List<int>() { resourceReferenceAndCatalogueDTO.ResourceId };
                    List<int> userIds = new List<int>() { currentUserId.Value };

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
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            List<int> resourceIdList = new List<int>() { resourceId };
            List<int> emptyOriginalResourceIdList = new List<int>() { };

            ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO = null;

            try
            {
                resourceReferenceAndCatalogueDTO = (await this.resourceRepository.GetResourceReferenceAndCatalogues(resourceIdList, emptyOriginalResourceIdList))
                    .SingleOrDefault() // Multiple resourceIds are grouped, multiple CatalogueDTOs with multiple originalResourceId is possible
                    ?? new ResourceReferenceAndCatalogueDTO();
            }
            catch (InvalidOperationException ex) 
            {
                throw new Exception($"Resource with ID {resourceId} had duplicates. Multiple catalogueDTOs permitted but resourceReferences should be groupable. Provided userId was {(currentUserId.HasValue ? currentUserId.Value.ToString() : "null")}", ex);
            }

            if (resourceReferenceAndCatalogueDTO == null)
            {
                throw new Exception($"Resource with ID {resourceId} not found. Provided userId was {(currentUserId.HasValue ? currentUserId.Value.ToString() : "null")}");
            }

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = new List<int>() { resourceId };
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            majorVersionIdActivityStatusDescription = resourceActivities.Count != 0 ? ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReferenceAndCatalogueDTO, resourceActivities).ToList() : new List<MajorVersionIdActivityStatusDescription>() { };

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
                resourceReferenceAndCatalogueDTO.MajorVersion,
                resourceReferenceAndCatalogueDTO.Rating ?? 0.0m,
                majorVersionIdActivityStatusDescription);
        }

        /// <summary>
        /// bulk get by ids async.
        /// </summary>
        /// <param name="originalResourceReferenceIds">the resource reference ids.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId)
        {

            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            List<ResourceReferenceAndCatalogueDTO> resourceReferenceAndCatalogueDTOs = (await this.resourceRepository.GetResourceReferenceAndCatalogues(new List<int>() { }, originalResourceReferenceIds)).ToList();
            resourceReferenceAndCatalogueDTOs = ResourceHelpers.FlattenResourceReferenceAndCatalogueDTOLS(resourceReferenceAndCatalogueDTOs);

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

            List<ResourceReferenceWithResourceDetailsViewModel> matchedResources = resourceReferenceAndCatalogueDTOs
                .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities.Where(ra => ra.ResourceId == rr.ResourceId).ToList()))
                .ToList<ResourceReferenceWithResourceDetailsViewModel>();

            return new BulkResourceReferenceViewModel(matchedResources, unmatchedOriginalResourceIds);
        }

        private ResourceReferenceWithResourceDetailsViewModel GetResourceReferenceWithResourceDetailsViewModel(ResourceReferenceAndCatalogueDTO resourceReferenceAndCatalogueDTO, List<ResourceActivityDTO> resourceActivities)
        {
            /*
                This model requires a flattened ResourceReferenceAndCatalogueDTO
            */

            if (resourceReferenceAndCatalogueDTO.CatalogueDTOs.Count != 1)
            {
                throw new ArgumentException(
                    $"Flat resourceReferenceAndCatalogueDTO with a single CatalogueDTO for one originalResourceId required. Count was {resourceReferenceAndCatalogueDTO.CatalogueDTOs.Count}.");
            }

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReferenceAndCatalogueDTO, resourceActivities).ToList();
            }

            var hasRating = resourceReferenceAndCatalogueDTO.Rating != null;

            if (resourceReferenceAndCatalogueDTO == null)
            {
                throw new Exception("No matching resource");
            }

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
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().OriginalResourceReferenceId,
                resourceReferenceAndCatalogueDTO.Title ?? ResourceHelpers.NoResourceVersionText,
                resourceReferenceAndCatalogueDTO.Description ?? string.Empty,
                resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().GetCatalogue(),
                resourceTypeNameOrEmpty,
                resourceReferenceAndCatalogueDTO.MajorVersion,
                resourceReferenceAndCatalogueDTO.Rating ?? 0,
                this.learningHubService.GetResourceLaunchUrl(resourceReferenceAndCatalogueDTO.CatalogueDTOs.Single().OriginalResourceReferenceId), 
                majorVersionIdActivityStatusDescription);
        }
    }
}
