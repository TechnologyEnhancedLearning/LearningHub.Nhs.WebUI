namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The catalogue service.
    /// </summary>
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cacheService.</param>
        public CategoryService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<CategoryService> logger, ICacheService cacheService)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// GetCatalogue version category.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<int> GetCatalogueVersionCategoryAsync(int catalogueNodeVersionId)
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"category/GetCatalogueVersionCategory/{catalogueNodeVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            var categoryId = 0;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                categoryId = Convert.ToInt32(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return categoryId;
        }

        /// <summary>
        /// Get sub categories by category id.
        /// </summary>
        /// <param name="categoryId">The categoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleSubCategoryResponseModel>> GetSubCategoryByCategoryIdAsync(int categoryId)
        {
            List<MoodleSubCategoryResponseModel> viewmodel = new List<MoodleSubCategoryResponseModel> { };

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"category/GetSubCategoryByCategoryId/{categoryId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<MoodleSubCategoryResponseModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Get courses by category id.
        /// </summary>
        /// <param name="categoryId">The categoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<MoodleCoursesResponseModel> GetCoursesByCategoryIdAsync(int categoryId)
        {
            MoodleCoursesResponseModel viewmodel = new MoodleCoursesResponseModel { };

            var client = await this.OpenApiHttpClient.GetClientAsync();

            var request = $"category/GetCoursesByCategoryId/{categoryId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<MoodleCoursesResponseModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<MoodleCategory>> GetAllMoodleCategoriesAsync()
        {
            List<MoodleCategory> viewmodel = new List<MoodleCategory>();

            try
            {
                var client = await this.OpenApiHttpClient.GetClientAsync();

                var request = $"Moodle/GetAllMoodleCategories";
                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<List<MoodleCategory>>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return viewmodel;
            }
            catch (Exception ex)
            {
                return viewmodel;
            }
        }
    }
}
