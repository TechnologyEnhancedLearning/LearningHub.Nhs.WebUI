namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceVersionEventRepository interface.
    /// </summary>
    public interface IResourceVersionRatingRepository : IGenericRepository<ResourceVersionRating>
    {
        /// <summary>
        /// Gets a user's previous rating for any minor version of the current major resource version.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceVersionRating> GetUsersPreviousRatingForSameMajorVersionAsync(int resourceVersionId, int userId);

        /// <summary>
        /// Gets the total rating counts for a particular resource version PLUS all other minor versions of the same major version.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>An array of integers, which are the count for each star value, starting at 1 star and ending with 5 stars.</returns>
        Task<int[]> GetRatingCountsForResourceVersionAsync(int resourceVersionId);
    }
}
