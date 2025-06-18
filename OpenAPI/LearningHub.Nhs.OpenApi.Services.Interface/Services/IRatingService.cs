namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The RatingService interface.
    /// </summary>
    public interface IRatingService
    {
        /// <summary>
        /// Returns the rating summary for a given entity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="entityVersionId">The entity version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RatingSummaryViewModel> GetRatingSummary(int userId, int entityVersionId);

        /// <summary>
        /// Returns the basic rating summary for a given entity.
        /// Does not include user related details.
        /// </summary>
        /// <param name="entityVersionId">The entity version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RatingSummaryBasicViewModel> GetRatingSummaryBasic(int entityVersionId);

        /// <summary>
        /// Create a rating of an entity. Can only be performed once per major version of an entity.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateRating(int userId, RatingViewModel ratingViewModel);

        /// <summary>
        /// Update a rating of an entity. Can be performed on ratings for any minor version of a major version of the resource.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateRating(int userId, RatingViewModel ratingViewModel);
    }
}
