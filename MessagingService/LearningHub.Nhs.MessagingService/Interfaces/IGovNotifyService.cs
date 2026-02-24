namespace LearningHub.Nhs.MessagingService.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.GovNotifyMessaging;

    /// <summary>
    /// IMessageServices.
    /// </summary>
    public interface IGovNotifyService
    {
        /// <summary>
        /// Send EmailAsync.
        /// </summary>
        /// <param name="email">email.</param>
        /// <param name="templateId">templateId.</param>
        /// <param name="personalisation">personalisation.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<GovNotifyResponse> SendEmailAsync(string email, string templateId, Dictionary<string, dynamic> personalisation);

        /// <summary>
        /// Send SmsAsync.
        /// </summary>
        /// <param name="phoneNumber">phoneNumber.</param>
        /// <param name="templateId">templateId.</param>
        /// <param name="personalisation">personalisation.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<GovNotifyResponse> SendSmsAsync(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation);
    }
}
