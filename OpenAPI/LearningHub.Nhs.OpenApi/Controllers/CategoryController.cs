namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Catalogue controller.
    /// </summary>
    [Route("Category")]
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [ApiController]
    public class CategoryController : OpenApiControllerBase
    {
        private readonly ICategoryService categoryService;
        private readonly IMoodleApiService moodleApiService;
        private readonly IMoodleBridgeApiService moodleBridgeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service.</param>
        /// <param name="moodleApiService">The moodleApi service.</param>
        /// <param name="moodleBridgeApiService">The moodle Bridge Api service.</param>
        public CategoryController(ICategoryService categoryService, IMoodleApiService moodleApiService, IMoodleBridgeApiService moodleBridgeApiService)
        {
            this.categoryService = categoryService;
            this.moodleApiService = moodleApiService;
            this.moodleBridgeApiService = moodleBridgeApiService;
        }

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogue node version id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("GetCatalogueVersionCategory/{catalogueNodeVersionId}")]
        public async Task<IActionResult> GetCatalogueVersionCategory(int catalogueNodeVersionId)
        {
            var catalogueNodeVersionCategory = await this.categoryService.GetByCatalogueVersionIdAsync(catalogueNodeVersionId);
            return this.Ok(catalogueNodeVersionCategory);
        }

        /// <summary>
        /// The GetCoursesByCategoryId.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("GetCoursesByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetCoursesByCategoryId(string categoryId)
        {
            var courses = await this.moodleBridgeApiService.GetCoursesByCategoryIdAsync(categoryId);
            return this.Ok(courses);
        }

        /// <summary>
        /// The GetSubCategoryByCategoryId.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <returns>The catalogue.</returns>
        [HttpGet]
        [Route("GetSubCategoryByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetSubCategoryByCategoryId(string categoryId)
        {
            var subCategories = await this.moodleBridgeApiService.GetSubCategoryByCategoryIdAsync(categoryId);
            return this.Ok(subCategories);
        }
    }
}
