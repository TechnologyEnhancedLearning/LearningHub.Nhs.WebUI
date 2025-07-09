namespace LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Models.Validation;
    using Microsoft.AspNetCore.Mvc;

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
    }
}
