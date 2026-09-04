namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Models;
    using MoodleCourseCompletionModel = LearningHub.Nhs.Models.Moodle.API.MoodleCourseCompletionModel;

    /// <summary>
    /// IMoodleApiService.
    /// </summary>
    public interface IMoodleBridgeApiService
    {
        /// <summary>
        /// GetUserInstancesByEmailAsync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleInstanceUserIdsViewModel> GetUserInstancesByEmail(string email);

        /// <summary>
        /// UpdateEmail.
        /// </summary>
        /// <param name="updateEmailaddressViewModel">The updateEmailaddressViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MoodleUpdateEmailResponseModel> UpdateEmail(UpdateEmailaddressViewModel updateEmailaddressViewModel);

        /// <summary>
        /// Gets the configured Moodle instance base URLs keyed by instance short name.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IDictionary<string, string>> GetMoodleInstanceBaseUrlsAsync();

        /// <summary>
        /// Gets a Moodle course URL for the supplied Moodle instance source.
        /// </summary>
        /// <param name="source">The Moodle instance source identifier.</param>
        /// <param name="courseId">The Moodle course id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetCourseUrlAsync(string source, int courseId);
    }
}
