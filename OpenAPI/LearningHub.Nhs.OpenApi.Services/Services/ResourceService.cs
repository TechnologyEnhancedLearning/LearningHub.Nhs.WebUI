namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.AzureMediaAsset;
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
            var list = new List<int>() { originalResourceReferenceId };
            var resourceReferences = currentUserId == null ?
                 await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(list) :
                 await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(list, currentUserId.Value);

            var resourceReferencesList = resourceReferences.ToList();

            try
            {
                var resourceReference = resourceReferencesList.SingleOrDefault();

                if (resourceReference == null)
                {
                    throw new HttpResponseException("No matching resource reference", HttpStatusCode.NotFound);
                }

                return this.GetResourceReferenceWithResourceDetailsViewModel(resourceReference);
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
            var resourceIdList = new List<int>() { resourceId };

            var resource = (currentUserId == null ?
                            await this.resourceRepository.GetResourcesFromIds(resourceIdList)
                            : await this.resourceRepository.GetResourcesFromIds(resourceIdList, (int)currentUserId))
                        .SingleOrDefault();

            if (resource == null)
            {
                throw new Exception($"Resource with ID {resourceId} not found. Provided userId was {(currentUserId.HasValue ? currentUserId.Value.ToString() : "null")}");
            }

            return new ResourceMetadataViewModel(
                resource.Id,
                resource.CurrentResourceVersion?.Title ?? ResourceHelpers.NoResourceVersionText,
                resource.CurrentResourceVersion?.Description ?? string.Empty,
                resource.ResourceReference.Select(rr => new ResourceReferenceViewModel(
                                                        rr.OriginalResourceReferenceId,
                                                        rr.GetCatalogue(),
                                                        this.learningHubService.GetResourceLaunchUrl(rr.OriginalResourceReferenceId)))
                                                        .ToList(),
                resource.GetResourceTypeNameOrEmpty(),
                resource.ResourceVersion.FirstOrDefault()?.MajorVersion,/*qqqq wont stay first or default because of logic in stored procedure*/
                resource.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0.0m,
                new Dictionary<int, string>() {
                    {2, "Downloaded" },
                    {3, "Passed" },
                    {5, "Failed"  },
                    {6, "Completed" },
                    {9, "In progress" },
                    {10,"Launched" }
                }); //qqqq
                //ActivityStatusHelper.UserSummaryActvityStatus((ActivityStatusEnum?)resource?.ResourceActivity?.FirstOrDefault()?.ActivityStatusId) ?? string.Empty);
        }

        /// <summary>
        /// bulk get by ids async.
        /// </summary>
        /// <param name="originalResourceReferenceIds">the resource reference ids.</param>
        /// <param name="currentUserId">.</param>
        /// <returns>the resource.</returns>
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId)
        {
            var resourceReferences = currentUserId == null ?
                await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceReferenceIds)
                : await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceReferenceIds, currentUserId.Value);

            var resourceReferencesList = resourceReferences.ToList();
            var matchedIds = resourceReferencesList.Select(r => r.OriginalResourceReferenceId).ToList();
            var unmatchedIds = originalResourceReferenceIds.Except(matchedIds).ToList();

            if (unmatchedIds.Any())
            {
                this.logger.LogInformation("Some resource ids not matched");
            }

            foreach (var duplicateIds in matchedIds.GroupBy(id => id).Where(groupOfIds => groupOfIds.Count() > 1))
            {
                this.logger.LogWarning($"Multiple resource references found with OriginalResourceReferenceId {duplicateIds.First()}");
            }

            var matchedResources = resourceReferencesList
                .Select(this.GetResourceReferenceWithResourceDetailsViewModel)
                .ToList();

            return new BulkResourceReferenceViewModel(matchedResources, unmatchedIds);
        }

        private ResourceReferenceWithResourceDetailsViewModel GetResourceReferenceWithResourceDetailsViewModel(ResourceReference resourceReference)
        {
            var hasCurrentResourceVersion = resourceReference.Resource.CurrentResourceVersion != null;
            var hasRating = resourceReference.Resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            if (resourceReference.Resource == null)
            {
                throw new Exception("No matching resource");
            }

            if (!hasCurrentResourceVersion)
            {
                this.logger.LogInformation($"Resource with OriginalResourceReferenceId {resourceReference.Id} is missing a current resource version");
            }

            if (!hasRating)
            {
                this.logger.LogInformation($"Resource with Id: {resourceReference.ResourceId} is missing a ResourceVersionRatingSummary");
            }

            var resourceTypeNameOrEmpty = resourceReference.Resource.GetResourceTypeNameOrEmpty();
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resourceReference.Resource.ResourceTypeEnum}");
            }

            return new ResourceReferenceWithResourceDetailsViewModel(
                resourceReference.ResourceId,
                resourceReference.OriginalResourceReferenceId,
                resourceReference.Resource.CurrentResourceVersion?.Title ?? ResourceHelpers.NoResourceVersionText,
                resourceReference.Resource.CurrentResourceVersion?.Description ?? string.Empty,
                resourceReference.GetCatalogue(),
                resourceTypeNameOrEmpty,
                resourceReference.Resource.ResourceVersion.FirstOrDefault()?.MajorVersion, /*qqqq will be replace by procedure*/
                resourceReference.Resource?.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0,
                this.learningHubService.GetResourceLaunchUrl(resourceReference.OriginalResourceReferenceId),
                //qqqq
                new Dictionary<int, string>() {
                    {2, "Downloaded" },
                    {3, "Passed" },
                    {5, "Failed"  },
                    {6, "Completed" },
                    {9, "In progress" },
                    {10,"Launched" }
                    }
                );
            //ActivityStatusHelper.UserSummaryActvityStatus((ActivityStatusEnum?)resourceReference.Resource?.ResourceActivity?.FirstOrDefault()?.ActivityStatusId) ?? string.Empty);
        }
    }
}
