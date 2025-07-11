namespace LearningHub.Nhs.OpenApi.Services.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessageQueueing.Repositories;
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.Models.Entities.GovNotifyMessaging;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using LearningHub.Nhs.Models.Enums.GovNotifyMessaging;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The GOVMessageService class.
    /// </summary>
    public class GovMessageService : BaseService<Message>, IGovMessageService
    {
        private readonly IMessageQueueRepository messageQueueRepository;
        private readonly IGovNotifyService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovMessageService"/> class.
        /// </summary>
        /// <param name="findwiseClient">The findwiseHttpClient.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageQueueRepository">The message repository.</param>
        /// <param name="messageService">The message Service.</param>
        public GovMessageService(
            IFindwiseClient findwiseClient,
            ILogger<Message> logger,
            IMessageQueueRepository messageQueueRepository,
            IGovNotifyService messageService)
            : base(findwiseClient, logger)
        {
            this.messageQueueRepository = messageQueueRepository;
            this.messageService = messageService;
        }

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request">the request.</param>
        /// <returns>The response.</returns>
        /// <exception cref="ArgumentException">The exception.</exception>
        public async Task<GovNotifyResponse> SendEmailAsync(EmailRequest request)
        {
            var response = new GovNotifyResponse();

            if (string.IsNullOrWhiteSpace(request.Recipient) || string.IsNullOrWhiteSpace(request.TemplateId))
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Email and template ID are required";
                return response;
            }

            response = await this.messageService.SendEmailAsync(request.Recipient, request.TemplateId, request.Personalisation);

            if (response != null)
            {
                if (request.Id == null || request.Id <= 0)
                {
                    var emailRequest = new SingleEmailRequest
                    {
                        Recipient = request.Recipient,
                        TemplateId = request.TemplateId,
                        Personalisation = request.Personalisation != null ? JsonConvert.SerializeObject(request.Personalisation.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())) : null,
                        Status = response.IsSuccess == true ? RequestStatusEnum.Sent : RequestStatusEnum.Failed,
                        ErrorMessage = response.ErrorMessage,
                    };
                    await this.messageQueueRepository.SaveSingleEmailTransactions(emailRequest);
                }
            }

            return response;
        }

        /// <summary>
        /// To queue the MessageRequests.
        /// </summary>
        /// <param name="request">The QueueRequestList.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task QueueRequestsAsync(QueueMessageList request)
        {
            if (request?.Messages == null || !request.Messages.Any())
            {
                throw new ArgumentException("At least one email must be provided in the request.");
            }

            var requests = request.Messages.Select(q => new QueueRequests
            {
                Recipient = q.Recipient,
                TemplateId = q.TemplateId,
                Personalisation = q.Personalisation != null ? JsonConvert.SerializeObject(q.Personalisation) : null,
                DeliverAfter = q.DeliverAfter ?? null,
            });

            await this.messageQueueRepository.QueueMessagesAsync(requests);
        }
    }
}
