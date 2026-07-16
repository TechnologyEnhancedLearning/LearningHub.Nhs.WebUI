namespace LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.GovNotifyMessaging;

    /// <summary>
    /// The IGovMessageService interface.
    /// </summary>
    public interface IGovMessageService
    {
        /// <summary>
        /// SendEmailAsync.
        /// </summary>
        /// <param name="request">the request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="ArgumentException">The exception.</exception>
        Task<GovNotifyResponse> SendEmailAsync(EmailRequest request);

        /// <summary>
        /// To queue the MessageRequests.
        /// </summary>
        /// <param name="request">The QueueRequestList.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task QueueRequestsAsync(QueueMessageList request);

        /// <summary>
        /// Get Message Requests.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<PagedResultSet<MessageRequestViewModel>> GetMessageRequests(int page, int pageSize, string sortColumn, string sortDirection, string filter);

        /// <summary>
        /// Get Message request by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        Task<MessageRequestViewModel> GetMessageRequestById(int id);
    }
}
