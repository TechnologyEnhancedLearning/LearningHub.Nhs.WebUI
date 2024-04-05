namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The IDetectJsLogRepository interface.
    /// </summary>
    public interface IDetectJsLogRepository : IGenericRepository<DetectJsLog>
    {
        /// <summary>
        /// UpdateAsync.
        /// </summary>
        /// <param name="jsEnabled">Js enabled request count.</param>
        /// <param name="jsDisabled">Js disabled request count.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(long jsEnabled, long jsDisabled);
    }
}