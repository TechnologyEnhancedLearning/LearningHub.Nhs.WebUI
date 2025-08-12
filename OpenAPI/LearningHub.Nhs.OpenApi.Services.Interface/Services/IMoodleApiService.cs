using LearningHub.Nhs.Models.Moodle.API;
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
        /// <param name="months">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<List<MoodleEnrolledCourseResponseModel>> GetRecentEnrolledCoursesAsync(int userId, int? months = null);

        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="userId">Moodle user id.</param>
        /// <param name="courseId">Moodle course id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseModel.</returns>
        Task<MoodleCourseCompletionModel> GetCourseCompletionAsync(int userId, int courseId, int pageNumber);
    }
}
