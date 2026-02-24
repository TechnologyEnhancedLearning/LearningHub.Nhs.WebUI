namespace LearningHub.Nhs.MessagingService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.MessagingService.Model;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using Microsoft.Extensions.Options;
    using Notify.Client;
    using Notify.Exceptions;

    /// <summary>
    /// GovNotify Service class.
    /// </summary>
    public class GovNotifyService : IGovNotifyService
    {
        private readonly NotificationClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyService"/> class.
        /// </summary>
        /// <param name="options">The Messaging Service Options.</param>
        public GovNotifyService(IOptions<MessagingServiceOptions> options)
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
        /// <returns>The <see cref="Task{GovNotifyResponse}"/>.</returns>
        public async Task<GovNotifyResponse> SendEmailAsync(string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            try
            {
                var normalisedPersonlisation = new Dictionary<string, dynamic>();
                if (personalisation != null)
                {
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
                }

                var response = await this.client.SendEmailAsync(email, templateId, normalisedPersonlisation);
                return new GovNotifyResponse
                {
                    IsSuccess = true,
                    NotificationId = response.id,
                };
            }
            catch (NotifyClientException ex)
            {
                return new GovNotifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ////Retry = true,
                };
            }
            catch (Exception ex)
            {
                return new GovNotifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ////Retry = true,
                };
            }
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
        /// <returns>The <see cref="Task{GovNotifyResponse}"/>.</returns>
        public async Task<GovNotifyResponse> SendSmsAsync(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation)
        {
            try
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
                return new GovNotifyResponse
                {
                    IsSuccess = true,
                    NotificationId = response.id,
                };
            }
            catch (NotifyClientException ex)
            {
                return new GovNotifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ////Retry = true,
                };
            }
            catch (Exception ex)
            {
                return new GovNotifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ////Retry = true,
                };
            }
        }
    }
}
