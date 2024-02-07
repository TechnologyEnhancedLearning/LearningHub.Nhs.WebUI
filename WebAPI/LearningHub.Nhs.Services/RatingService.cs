// <copyright file="RatingService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Constants;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The rating service.
    /// </summary>
    public class RatingService : IRatingService
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The resource service.
        /// </summary>
        private readonly IResourceService resourceService;

        /// <summary>
        /// The resource version repository.
        /// </summary>
        private readonly IResourceVersionRepository resourceVersionRepository;

        /// <summary>
        /// The resource repository.
        /// </summary>
        private readonly IResourceRepository resourceRepository;

        /// <summary>
        /// The resource version rating summary repository.
        /// </summary>
        private readonly IResourceVersionRatingSummaryRepository resourceVersionRatingSummaryRepository;

        /// <summary>
        /// The resource version rating repository.
        /// </summary>
        private readonly IResourceVersionRatingRepository resourceVersionRatingRepository;

        /// <summary>
        /// The publication repository.
        /// </summary>
        private readonly IPublicationRepository publicationRepository;

        /// <summary>
        /// The caching service.
        /// </summary>
        private readonly ICachingService cachingService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ActivityService> logger;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingService"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="resourceService">The resource service.</param>
        /// <param name="resourceVersionRepository">The resource version repository.</param>
        /// <param name="resourceRepository">The resource repository.</param>
        /// <param name="resourceVersionRatingSummaryRepository">The resource version rating summary repository.</param>
        /// <param name="resourceVersionRatingRepository">The resource version rating repository.</param>
        /// <param name="publicationRepository">The publicationRepository.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public RatingService(
            IUserService userService,
            IResourceService resourceService,
            IResourceVersionRepository resourceVersionRepository,
            IResourceRepository resourceRepository,
            IResourceVersionRatingSummaryRepository resourceVersionRatingSummaryRepository,
            IResourceVersionRatingRepository resourceVersionRatingRepository,
            ////IUserEmploymentRepository userEmploymentRepository,
            IPublicationRepository publicationRepository,
            ICachingService cachingService,
            ILogger<ActivityService> logger,
            IMapper mapper)
        {
            this.userService = userService;
            this.resourceService = resourceService;
            this.resourceVersionRepository = resourceVersionRepository;
            this.resourceRepository = resourceRepository;
            this.resourceVersionRatingSummaryRepository = resourceVersionRatingSummaryRepository;
            this.resourceVersionRatingRepository = resourceVersionRatingRepository;
            ////this.userEmploymentRepository = userEmploymentRepository;
            this.publicationRepository = publicationRepository;
            this.cachingService = cachingService;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns the rating summary for a given entity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entityVersionId">The entity version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<RatingSummaryViewModel> GetRatingSummary(int userId, int entityVersionId)
        {
            var ratingSummary = await this.resourceVersionRatingSummaryRepository.GetByResourceVersionIdAsync(entityVersionId);

            var ratingSummaryViewModel = this.mapper.Map<RatingSummaryViewModel>(ratingSummary);

            if (ratingSummaryViewModel.RatingCount > 0)
            {
                ratingSummaryViewModel.Rating1StarPercent = (int)((double)ratingSummary.Rating1StarCount / ratingSummary.RatingCount * 100);
                ratingSummaryViewModel.Rating2StarPercent = (int)((double)ratingSummary.Rating2StarCount / ratingSummary.RatingCount * 100);
                ratingSummaryViewModel.Rating3StarPercent = (int)((double)ratingSummary.Rating3StarCount / ratingSummary.RatingCount * 100);
                ratingSummaryViewModel.Rating4StarPercent = (int)((double)ratingSummary.Rating4StarCount / ratingSummary.RatingCount * 100);
                ratingSummaryViewModel.Rating5StarPercent = (int)((double)ratingSummary.Rating5StarCount / ratingSummary.RatingCount * 100);
            }

            ratingSummaryViewModel.UserIsContributor = await this.IsTheUserTheContributor(userId, entityVersionId);
            ratingSummaryViewModel.UserCanRate = await this.HasUserCompletedActivity(userId, entityVersionId);

            var userRating = await this.resourceVersionRatingRepository.GetUsersPreviousRatingForSameMajorVersionAsync(entityVersionId, userId);
            if (userRating != null)
            {
                ratingSummaryViewModel.UserRating = userRating.Rating;
                ratingSummaryViewModel.UserHasAlreadyRated = true;
            }

            return ratingSummaryViewModel;
        }

        /// <summary>
        /// Returns the basic rating summary for a given entity.
        /// Does not include user related details.
        /// </summary>
        /// <param name="entityVersionId">The entity version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<RatingSummaryBasicViewModel> GetRatingSummaryBasic(int entityVersionId)
        {
            string cacheKey = $"{CacheKeys.RatingSummaryBasic}:{entityVersionId}";
            var retVal = await this.cachingService.GetAsync<RatingSummaryBasicViewModel>(cacheKey);
            if (retVal.ResponseEnum == CacheReadResponseEnum.Found)
            {
                return retVal.Item;
            }
            else
            {
                try
                {
                    var ratingSummary = await this.resourceVersionRatingSummaryRepository.GetByResourceVersionIdAsync(entityVersionId);

                    var ratingSummaryBasicViewModel = this.mapper.Map<RatingSummaryBasicViewModel>(ratingSummary);

                    ratingSummaryBasicViewModel.Rating1StarPercent = (int)((double)ratingSummary.Rating1StarCount / ratingSummary.RatingCount * 100);
                    ratingSummaryBasicViewModel.Rating2StarPercent = (int)((double)ratingSummary.Rating2StarCount / ratingSummary.RatingCount * 100);
                    ratingSummaryBasicViewModel.Rating3StarPercent = (int)((double)ratingSummary.Rating3StarCount / ratingSummary.RatingCount * 100);
                    ratingSummaryBasicViewModel.Rating4StarPercent = (int)((double)ratingSummary.Rating4StarCount / ratingSummary.RatingCount * 100);
                    ratingSummaryBasicViewModel.Rating5StarPercent = (int)((double)ratingSummary.Rating5StarCount / ratingSummary.RatingCount * 100);

                    await this.cachingService.SetAsync(cacheKey, ratingSummaryBasicViewModel);

                    return ratingSummaryBasicViewModel;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Create a rating of an entity.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateRating(int userId, RatingViewModel ratingViewModel)
        {
            // Check that this is a valid rating.
            var retVal = await this.ValidateRating(userId, ratingViewModel);

            // Check user has not already rated the resource. Only applies to create, not update.
            if (await this.HasTheUserAlreadyRatedThisMajorVersion(userId, ratingViewModel.EntityVersionId))
            {
                retVal.Add(new LearningHubValidationResult(false, "You have already rated this resource"));
                retVal.IsValid = false;
            }

            if (!retVal.IsValid)
            {
                return retVal;
            }

            // All checks passed. Save a new rating...
            await this.CreateNewRatingAndRecalcAverageAsync(userId, ratingViewModel);

            // Force cache refresh
            string cacheKey = $"{CacheKeys.RatingSummaryBasic}:{ratingViewModel.EntityVersionId}";
            await this.cachingService.RemoveAsync(cacheKey);

            await this.GetRatingSummaryBasic(ratingViewModel.EntityVersionId);

            // TODO: Submit new resource record to findwise.
            return retVal;
        }

        /// <summary>
        /// Update a rating of an entity. Can be performed on ratings for any minor version of a major version of the resource.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateRating(int userId, RatingViewModel ratingViewModel)
        {
            // Check that this is a valid rating.
            var retVal = await this.ValidateRating(userId, ratingViewModel);
            if (!retVal.IsValid)
            {
                return retVal;
            }

            // All checks passed. Mark old rating as deleted and create a new rating...
            var oldRating = await this.resourceVersionRatingRepository.GetUsersPreviousRatingForSameMajorVersionAsync(ratingViewModel.EntityVersionId, userId);
            if (oldRating == null)
            {
                retVal.Add(new LearningHubValidationResult(false, "A previous rating for this resource was not found"));
                retVal.IsValid = false;
                return retVal;
            }

            oldRating.Deleted = true;
            await this.resourceVersionRatingRepository.UpdateAsync(userId, oldRating);

            await this.CreateNewRatingAndRecalcAverageAsync(userId, ratingViewModel);

            // Force cache refresh
            string cacheKey = $"{CacheKeys.RatingSummaryBasic}:{ratingViewModel.EntityVersionId}";
            await this.cachingService.RemoveAsync(cacheKey);

            await this.GetRatingSummaryBasic(ratingViewModel.EntityVersionId);

            return retVal;
        }

        private async Task<LearningHubValidationResult> ValidateRating(int userId, RatingViewModel ratingViewModel)
        {
            // Basic validation
            var ratingValidator = new RatingValidator();
            var validationResults = await ratingValidator.ValidateAsync(ratingViewModel);
            var retVal = new LearningHubValidationResult(validationResults);

            // Check that the resource version is the most recent version. Can't rate old versions. Not possible through UI but check anyway.
            if (!await this.IsTheCurrentVersion(ratingViewModel.EntityVersionId))
            {
                retVal.Add(new LearningHubValidationResult(false, "This version of the resource is not the current version and cannot be rated"));
                retVal.IsValid = false;
            }

            // Check the user is not the contributor. Contributor can't rate own resources.
            if (await this.IsTheUserTheContributor(userId, ratingViewModel.EntityVersionId))
            {
                retVal.Add(new LearningHubValidationResult(false, "A contributor cannot rate their own resources"));
                retVal.IsValid = false;
            }

            // Check the user has performed the activity.
            if (!await this.HasUserCompletedActivity(userId, ratingViewModel.EntityVersionId))
            {
                retVal.Add(new LearningHubValidationResult(false, "You have to access the resource before you can rate it"));
                retVal.IsValid = false;
            }

            return retVal;
        }

        private async Task CreateNewRatingAndRecalcAverageAsync(int userId, RatingViewModel ratingViewModel)
        {
            var resourceVersionRating = new ResourceVersionRating()
            {
                UserId = userId,
                Rating = ratingViewModel.Rating,
                ResourceVersionId = ratingViewModel.EntityVersionId,
                JobRoleId = ratingViewModel.JobRoleId.GetValueOrDefault(),
                LocationId = ratingViewModel.LocationId,
            };

            await this.resourceVersionRatingRepository.CreateAsync(userId, resourceVersionRating);

            // Recalculate the resource's star rating.
            var ratingSummary = await this.resourceVersionRatingSummaryRepository.GetByResourceVersionIdAsync(ratingViewModel.EntityVersionId);

            var allRatings = await this.resourceVersionRatingRepository.GetRatingCountsForResourceVersionAsync(ratingViewModel.EntityVersionId);

            ratingSummary.Rating1StarCount = allRatings[0];
            ratingSummary.Rating2StarCount = allRatings[1];
            ratingSummary.Rating3StarCount = allRatings[2];
            ratingSummary.Rating4StarCount = allRatings[3];
            ratingSummary.Rating5StarCount = allRatings[4];
            ratingSummary.RatingCount = allRatings.Sum();
            ratingSummary.AverageRating = decimal.Round((decimal)((allRatings[0] * 1) + (allRatings[1] * 2) + (allRatings[2] * 3) + (allRatings[3] * 4) + (allRatings[4] * 5)) / ratingSummary.RatingCount, 1);

            await this.resourceVersionRatingSummaryRepository.UpdateAsync(userId, ratingSummary);

            // Submit updated rating to Findwise.
            await this.resourceService.SubmitResourceVersionToSearchAsync(ratingViewModel.EntityVersionId, userId);
        }

        private async Task<bool> IsTheCurrentVersion(int entityVersionId)
        {
            bool isCurrentVersion = await this.resourceRepository.IsCurrentVersionAsync(entityVersionId);
            return isCurrentVersion;
        }

        private async Task<bool> IsTheUserTheContributor(int userId, int entityVersionId)
        {
            var publication = await this.publicationRepository.GetByResourceVersionIdAsync(entityVersionId);
            return publication.CreateUserId == userId;
        }

        private async Task<bool> HasTheUserAlreadyRatedThisMajorVersion(int userId, int entityVersionId)
        {
            var rating = await this.resourceVersionRatingRepository.GetUsersPreviousRatingForSameMajorVersionAsync(entityVersionId, userId);
            return rating != null;
        }

        private async Task<bool> HasUserCompletedActivity(int userId, int entityVersionId)
        {
            var hasCompleted = await this.resourceVersionRepository.HasUserCompletedActivity(userId, entityVersionId);
            return hasCompleted;
        }
    }
}