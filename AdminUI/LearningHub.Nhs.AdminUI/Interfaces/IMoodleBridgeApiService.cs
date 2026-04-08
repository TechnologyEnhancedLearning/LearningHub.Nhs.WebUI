namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Moodle;
    using LearningHub.Nhs.Models.Moodle.API;

    /// <summary>
    /// IMoodleBridgeApiService.
    /// </summary>
    public interface IMoodleBridgeApiService
    {
        /// <summary>
        /// GetAllMoodleCategoriesAsync.
        /// </summary>
        /// <returns> List of MoodleCategory.</returns>
        Task<List<CategoryResult>> GetAllMoodleCategoriesAsync();
    }
}
