namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle.API;
    using MoodleCourseCompletionModel = LearningHub.Nhs.Models.Moodle.API.MoodleCourseCompletionModel;

    /// <summary>
    /// IMoodleApiService.
    /// </summary>
    public interface IMoodleApiService
    {
        /// <summary>
        /// GetMoodleUserIdByUsernameAsync.
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
    }
}
