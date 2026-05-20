namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using LearningHub.Nhs.Models.Paging;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IGovNotifyDashboardService" />.
    /// </summary>
    public interface IGovNotifyDashboardService
    {
        /// <summary>
        /// The GetPagedAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{MessageRequestViewModel}"/>.</returns>
        Task<PagedResultSet<MessageRequestViewModel>> GetPagedAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// Get Message Request By Id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<MessageRequestViewModel> GetMessageRequestById(int id);
    }
}
