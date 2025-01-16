namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// The FileTypeService interface.
    /// </summary>
    public interface IFileTypeService
    {
        /// <summary>
        /// The get all async.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<FileTypeViewModel>> GetAllAsync();

        /// <summary>
        /// Gets the FileType corresponding to a filename.
        /// </summary>
        /// <param name="filename">The filename to return the FileType for.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<FileType> GetByFilename(string filename);

        /// <summary>
        /// Gets a list of all disallowed file extensions.
        /// </summary>
        /// <returns>The <see cref="List{T}"/>.</returns>
        List<string> GetAllDisallowedFileExtensions();
    }
}
