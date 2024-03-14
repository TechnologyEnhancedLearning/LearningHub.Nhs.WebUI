namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// The ScormResourceVersionRepository interface.
    /// </summary>
    public interface IScormResourceVersionRepository : IGenericRepository<ScormResourceVersion>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ScormResourceVersion}"/>.</returns>
        Task<ScormResourceVersion> GetByIdAsync(int id);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="includeDeleted">The includeDeleted<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ScormResourceVersion}"/>.</returns>
        Task<ScormResourceVersion> GetByResourceVersionIdAsync(int id, bool includeDeleted = false);

        /// <summary>
        /// Gets the content details for a particular Learning Hub external reference (guid).
        /// </summary>
        /// <param name="externalReference">The external reference (guid).</param>
        /// <returns>A ContentServerViewModel.</returns>
        ContentServerViewModel GetContentServerDetailsByLHExternalReference(string externalReference);

        /// <summary>
        /// Gets the SCORM content details for a particular historic external URL. These historic URLs have to be supported to
        /// allow historic ESR links on migrated resources to continue to work.
        /// </summary>
        /// <param name="externalUrl">The external Url.</param>
        /// <returns>A ContentServerViewModel.</returns>
        ContentServerViewModel GetScormContentServerDetailsByHistoricExternalUrl(string externalUrl);

        /// <summary>
        /// GetExternalReferenceByResourceId.
        /// </summary>
        /// <param name="resourceId">Resource id.</param>
        /// <returns>External Reference.</returns>
        Task<List<string>> GetExternalReferenceByResourceId(int resourceId);
    }
}
