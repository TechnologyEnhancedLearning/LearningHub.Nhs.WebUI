namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="ICategoryService" />.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// GetCatalogue version category.
        /// </summary>
        /// <param name="catalogueNodeVersionId">catalogueNodeVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetCatalogueVersionCategoryAsync(int catalogueNodeVersionId);

        /// <summary>
        /// GetCoursesByCategoryIdAsync.
        /// </summary>
        /// <param name="categoryId">categoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleCoursesResponseModel> GetCoursesByCategoryIdAsync(int categoryId);

        /// <summary>
        /// GetSubCategoryByCategoryIdAsync.
        /// </summary>
        /// <param name="categoryId">categoryId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<MoodleSubCategoryResponseModel>> GetSubCategoryByCategoryIdAsync(int categoryId);

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<MoodleCategory>> GetAllMoodleCategoriesAsync();
    }
}
