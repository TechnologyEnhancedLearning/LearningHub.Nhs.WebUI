namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
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
        /// the get by id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">the id.</param>
        /// <param name="currentUserId"></param>
        /// <returns>the resource.</returns>
        public async Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            var list = new List<int>() { originalResourceReferenceId };

            var resourceReferences = await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(list);
            var resourceReferencesList = resourceReferences.ToList();

            try
            {
                var resourceReference = resourceReferencesList.SingleOrDefault();

                if (resourceReference == null)
                {
                    throw new HttpResponseException("No matching resource reference", HttpStatusCode.NotFound);
                }

                if (currentUserId.HasValue)
                {
                    List<int> resourceIds = new List<int>() { resourceReference.ResourceId };
                    List<int> userIds = new List<int>() { currentUserId.Value };

                    resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
                }

                return this.GetResourceReferenceWithResourceDetailsViewModel(resourceReference, resourceActivities);
            }
            catch (InvalidOperationException exception)
            {
                this.logger.LogError(exception, $"Multiple resource references found with OriginalResourceReferenceId {originalResourceReferenceId}");
                throw;
            }
        }

        /// <summary>
        /// bulk get by ids async.
        /// </summary>
        /// <param name="originalResourceReferenceIds">the resource reference ids.</param>
        /// <returns>the resource.</returns>
        public async Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            var resourceReferences = await this.resourceRepository.GetResourceReferencesByOriginalResourceReferenceIds(originalResourceReferenceIds);
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

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourceReferencesList.Select(rrl => rrl.ResourceId).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            List<ResourceReferenceWithResourceDetailsViewModel> matchedResources = resourceReferencesList
            .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities.Where(ra => ra.ResourceId == rr.ResourceId).ToList()))
            .ToList<ResourceReferenceWithResourceDetailsViewModel>();

            return new BulkResourceReferenceViewModel(matchedResources, unmatchedIds);
        }


        /// <summary>
        /// the get by id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">the id.</param>
        /// <param name="currentUserId"></param>
        /// <returns>list resource ViewModel.</returns>
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferenceByActivityStatus(List<int> activityStatusIds, int currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceReferenceWithResourceDetailsViewModel> ResourceReferenceWithResourceDetailsViewModelLS = new List<ResourceReferenceWithResourceDetailsViewModel>() { };

            resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(new List<int>(){ }, new List<int>(){ currentUserId }))?.ToList() ?? new List<ResourceActivityDTO>() { };

            // Removing resources that have no major versions with the required activitystatus
            List<int> resourceIds = resourceActivities
                .GroupBy(ra => ra.ResourceId)
                .Where(group => group.Any(g => activityStatusIds.Contains(g.ActivityStatusId)))
                .Select(group => group.Key)
                .Distinct()
                .ToList();

            var resourceReferencesList = (await this.resourceRepository.GetResourcesFromIds(resourceIds)).SelectMany(r => r.ResourceReference).ToList();

            ResourceReferenceWithResourceDetailsViewModelLS = resourceReferencesList.Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities)).ToList();

            return ResourceReferenceWithResourceDetailsViewModelLS;
        }

        /// <summary>
        /// Gets ResourceReferences ForCertificates using the ResourceReferenceWithResourceDetailsViewModel .
        /// </summary>
        /// <param name="currentUserId">user Id.</param>
        /// <returns>list resource reference ViewModel.</returns>
        public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesForCertificates(int currentUserId)
        {

            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceReferenceWithResourceDetailsViewModel> ResourceReferenceWithResourceDetailsViewModelLS = new List<ResourceReferenceWithResourceDetailsViewModel>() { };
            List<int> acheivedCertificatedResourceIds = (await this.resourceRepository.GetAcheivedCertificatedResourceIds(currentUserId));

            resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(acheivedCertificatedResourceIds, new List<int>() { currentUserId }))?.ToList() ?? new List<ResourceActivityDTO>() { };

            var resourceList = (await this.resourceRepository.GetResourcesFromIds(acheivedCertificatedResourceIds)).ToList();

            //qqqq check this can return empty list of resourceActivity where there isnt any
            //because we return resourceActivity but things are certified on other things could have strange looking results
            ResourceReferenceWithResourceDetailsViewModelLS = resourceList.SelectMany(r => r.ResourceReference)
                .Distinct()
                .Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities)).ToList();

            return ResourceReferenceWithResourceDetailsViewModelLS;
        }
        // qqqq original
        //public async Task<List<ResourceReferenceWithResourceDetailsViewModel>> GetResourceReferencesForCertificates(int currentUserId)
        //{
        //    // GetAcheivedCertificatedResourceIds
        //    //qqqq can some of this go into a helper
        //    List<int> activityStatusesForCertificates = new List<int>() { (int)ActivityStatusEnum.Completed, (int)ActivityStatusEnum.Passed }; // qqqq maybe drop completed
        //    List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
        //    List<ResourceReferenceWithResourceDetailsViewModel> ResourceReferenceWithResourceDetailsViewModelLS = new List<ResourceReferenceWithResourceDetailsViewModel>() { };

        //    resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(new List<int>() { }, new List<int>() { currentUserId }))?.ToList() ?? new List<ResourceActivityDTO>() { };

        //    // Removing resources that have no major versions with the required activitystatus
        //    List<int> resourceIds = resourceActivities
        //        .GroupBy(ra => ra.ResourceId)
        //        .Where(group => group.Any(g => activityStatusesForCertificates.Contains(g.ActivityStatusId)))
        //        .Select(group => group.Key)
        //        .Distinct()
        //        .ToList();

        //    var resourceReferencesList = (await this.resourceRepository.GetResourceReferencesForAssessments(resourceIds)).ToList();

        //    ResourceReferenceWithResourceDetailsViewModelLS = resourceReferencesList.Select(rr => this.GetResourceReferenceWithResourceDetailsViewModel(rr, resourceActivities)).ToList();

        //    return ResourceReferenceWithResourceDetailsViewModelLS;
        //}

        private ResourceReferenceWithResourceDetailsViewModel GetResourceReferenceWithResourceDetailsViewModel(ResourceReference resourceReference, List<ResourceActivityDTO> resourceActivities)
        {
            var hasCurrentResourceVersion = resourceReference.Resource.CurrentResourceVersion != null;
            var hasRating = resourceReference.Resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resourceReference.Resource, resourceActivities).ToList();
            }

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
                resourceReference.Resource?.CurrentResourceVersion?.MajorVersion ?? 0,
                resourceReference.Resource?.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0,
                this.learningHubService.GetResourceLaunchUrl(resourceReference.OriginalResourceReferenceId),
                majorVersionIdActivityStatusDescription);
        }
    }
}
