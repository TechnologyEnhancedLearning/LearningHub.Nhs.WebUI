namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Maintenance;

    /// <summary>
    /// The InternalSystemService.
    /// </summary>
    public interface IInternalSystemService
    {
        /// <summary>
        /// Gets InternalSystem by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<InternalSystemViewModel> GetByIdAsync(int id);
    }
}
