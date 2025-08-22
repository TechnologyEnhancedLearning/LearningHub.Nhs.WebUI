using LearningHub.Nhs.Models.Moodle.API;
using LearningHub.Nhs.Models.MyLearning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// IMoodleApiService.
    /// </summary>
    public interface IMoodleApiService
    {
        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="currentUserId">The current LH User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetMoodleUserIdByUsernameAsync(int currentUserId);

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="currentUserId">Moodle user id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<List<MoodleCourseResponseModel>> GetEnrolledCoursesAsync(int currentUserId, int pageNumber);

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="requestModel">MyLearningRequestModel requestModel.</param>
        /// <param name="months">months.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<List<MoodleEnrolledCourseResponseModel>> GetRecentEnrolledCoursesAsync(int userId, MyLearningRequestModel requestModel, int? months = null);

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="requestModel">MyLearningRequestModel requestModel.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<List<MoodleEnrolledCourseResponseModel>> GetEnrolledCoursesHistoryAsync(int userId, MyLearningRequestModel requestModel);

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="courseId">Moodle course id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<MoodleCourseCompletionModel> GetCourseCompletionAsync(int userId, int courseId, int pageNumber);

        /// <summary>
        /// GetUserLearningHistory.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="filterText">The page Number.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<MoodleUserCertificateResponseModel>> GetUserCertificateAsync(int userId, string filterText = "");
    }
}
