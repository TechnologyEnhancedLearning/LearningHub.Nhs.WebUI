namespace LearningHub.Nhs.MessagingService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.MessagingService.Model;
    using Microsoft.Extensions.Options;
    using Notify.Client;

    /// <summary>
    /// GovNotify Service class.
    /// </summary>
    public class GovNotifyService : IGovNotifyService
    {
        private readonly NotificationClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyService"/> class.
        /// GovNotifyService.
        /// </summary>
        /// <param name="options">The Messaging Service Model.</param>
        public GovNotifyService(IOptions<MessagingServiceModel> options)
        {
            this.client = new NotificationClient(options.Value.GovNotifyApiKey);
        }

        /// <summary>
        /// Sends an email via the UK Gov.Notify service.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="templateId">The ID of the Gov.Notify template to be used for the email.</param>
        /// <param name="personalisation">
        /// A dictionary containing key-value pairs for personalising the email template.
        /// Keys should match the placeholders in the Gov.Notify template.
        /// </param>
        /// <returns>
        /// A unique message ID representing the queued email.
        /// </returns>
        public async Task<string> SendEmailAsync(string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            var normalisedPersonlisation = new Dictionary<string, dynamic>();
            foreach (var item in personalisation)
            {
                if (item.Value is JsonElement element)
                {
                    normalisedPersonlisation[item.Key] = element.ToString();
                }
                else
                {
                    normalisedPersonlisation[item.Key] = item.Value;
                }
            }

            var response = await this.client.SendEmailAsync(email, templateId, normalisedPersonlisation);
            return response.id;
        }

        /// <summary>
        /// Sends an SMS via the UK Gov.Notify service.
        /// </summary>
        /// <param name="phoneNumber">The recipient's phone number.</param>
        /// <param name="templateId">The ID of the Gov.Notify template to be used for the SMS.</param>
        /// <param name="personalisation">
        /// A dictionary containing key-value pairs for personalising the SMS template.
        /// Keys should match the placeholders in the Gov.Notify template.
        /// </param>
        /// <returns>
        /// A unique message ID representing the queued SMS.
        /// </returns>
        public async Task<string> SendSmsAsync(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation)
        {
            var normalisedPersonlisation = new Dictionary<string, dynamic>();
            foreach (var item in personalisation)
            {
                if (item.Value is JsonElement element)
                {
                    normalisedPersonlisation[item.Key] = element.ToString();
                }
                else
                {
                    normalisedPersonlisation[item.Key] = item.Value;
                }
            }

            var response = await this.client.SendSmsAsync(phoneNumber, templateId, normalisedPersonlisation);
            return response.id;
        }
    }
}
