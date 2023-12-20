// <copyright file="RatingService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="RatingService" />.
    /// </summary>
    public class RatingService : BaseService<RatingService>, IRatingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RatingService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        public RatingService(ILearningHubHttpClient learningHubHttpClient, ILogger<RatingService> logger)
        : base(learningHubHttpClient, logger)
        {
        }

        /// <summary>
        /// Create a new rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel<see cref="RatingViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateRatingAsync(RatingViewModel ratingViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(ratingViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Rating/CreateRating";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }

        /// <summary>
        /// Gets the rating summary for the entity.
        /// </summary>
        /// <param name="entityVersionId">The entityVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{RatingSummaryViewModel}"/>.</returns>
        public async Task<RatingSummaryViewModel> GetRatingSummaryAsync(int entityVersionId)
        {
            RatingSummaryViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Rating/GetRatingSummary/{entityVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<RatingSummaryViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Update a rating.
        /// </summary>
        /// <param name="ratingViewModel">The ratingViewModel<see cref="RatingViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateRatingAsync(RatingViewModel ratingViewModel)
        {
            ApiResponse apiResponse = null;

            var json = JsonConvert.SerializeObject(ratingViewModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Rating/UpdateRating";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }

                return apiResponse.ValidationResult;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else
            {
                throw new Exception("save failed!");
            }
        }
    }
}
