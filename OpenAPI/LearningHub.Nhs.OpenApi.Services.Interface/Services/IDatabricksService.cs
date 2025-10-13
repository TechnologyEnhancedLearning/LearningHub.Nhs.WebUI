using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// IDatabricks service
    /// </summary>
    public interface IDatabricksService
    {
        /// <summary>
        /// IsUserReporter.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> IsUserReporter(int userId);
    }
}
