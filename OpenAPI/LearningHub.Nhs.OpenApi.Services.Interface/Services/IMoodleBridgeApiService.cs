
using System.Collections.Generic;
using System.Threading.Tasks;
using LearningHub.Nhs.Models.Moodle;
using LearningHub.Nhs.Models.Moodle.API;
using LearningHub.Nhs.Models.MyLearning;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// IMoodleBridgeApiService.
    /// </summary>
    public interface IMoodleBridgeApiService
    {
        /// <summary>
        /// GetUserInstancesByEmailAsync.
        /// </summary>
        /// <param name="email">The current LH User email.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleInstanceUserIdsViewModel> GetUserInstancesByEmail(string email);

        /// <summary>
        /// GetRecentEnrolledCoursesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">The moodleUserInstanceUserIds.</param>
        /// <param name="requestModel">The requestModel.</param>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        Task<MoodleCompletionsApiResponseModel> GetRecentEnrolledCoursesAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, MyLearningRequestModel requestModel, int? month = null);

        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CategoryResult>> GetAllMoodleCategoriesAsync();

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">Moodle Instances user id.</param>
        /// <param name="requestModel">MyLearningRequestModel requestModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleCompletionsApiResponseModel> GetEnrolledCoursesHistoryAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, MyLearningRequestModel requestModel);

        /// <summary>
        /// GetInProgressEnrolledCoursesAsync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleCompletionsApiResponseModel> GetInProgressEnrolledCoursesAsync(string email);

        /// <summary>
        /// GetUserLearningHistory.
        /// </summary>
        /// <param name="email">user email.</param>
        /// <param name="filterText">The filterText.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleCertificateResponseModel> GetUserCertificateAsync(string email, string filterText = "");

        /// <summary>
        /// GetUserCertificateFromMoodleInstancesAsync.
        /// </summary>
        /// <param name="moodleUserInstanceUserIds">Moodle Instances user id.</param>
        /// <param name="filterText">filterText.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleCertificateResponseModel> GetUserCertificateFromMoodleInstancesAsync(MoodleInstanceUserIdsViewModel moodleUserInstanceUserIds, string filterText = "");

        /// <summary>
        /// GetCoursesByCategoryIdAsync.
        /// </summary>
        /// <param name="categoryId">The categoryId.</param>
        /// <returns></returns>
        Task<MoodleCourseResultsResponseModel> GetCoursesByCategoryIdAsync(string categoryId);

        /// <summary>
        /// GetSubCategoryByCategoryIdAsync.
        /// </summary>
        /// <param name="categoryId">The categoryId.</param>
        /// <returns></returns>
        Task<List<SubCategoryResult>> GetSubCategoryByCategoryIdAsync(string categoryId);
    }
}
