namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.WebUI.Models;

    /// <summary>
    /// IMoodleApiService.
    /// </summary>
    public interface IMoodleApiService
    {
        /// <summary>
        /// GetEnrolledCoursesAsync.
        /// </summary>
        /// <param name="currentUserId">Moodle user id.</param>
        /// <param name="pageNumber">pageNumber.</param>
        /// <returns> List of MoodleCourseResponseViewModel.</returns>
        Task<List<MoodleCourseResponseViewModel>> GetEnrolledCoursesAsync(int currentUserId, int pageNumber);
    }
}
