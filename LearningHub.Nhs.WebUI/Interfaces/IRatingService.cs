// <copyright file="IRatingService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
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
        /// Gets the rating summary for the entity.
        /// </summary>
        /// <param name="entityVersionId">The entity version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RatingSummaryViewModel> GetRatingSummaryAsync(int entityVersionId);

        /// <summary>
        /// Create a new rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="T:Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateRatingAsync(RatingViewModel ratingViewModel);

        /// <summary>
        /// Update a rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel.</param>
        /// <returns>The <see cref="T:Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateRatingAsync(RatingViewModel ratingViewModel);
    }
}