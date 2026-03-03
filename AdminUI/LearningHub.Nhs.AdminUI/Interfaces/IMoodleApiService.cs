namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;

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
        /// <returns> List of MoodleCategory.</returns>
        Task<List<MoodleCategory>> GetAllMoodleCategoriesAsync();
    }
}
