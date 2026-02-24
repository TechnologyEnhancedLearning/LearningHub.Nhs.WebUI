namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessageQueueing.Repositories;
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.Models.Entities.GovNotifyMessaging;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.GovNotifyMessaging;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Services.Messaging;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// GovNotify Messaging Controller.
    /// </summary>
    [Route("GovNotifyMessage")]
    [Authorize]
    public class GovNotifyMessagingController : OpenApiControllerBase
    {
        private readonly IGovNotifyService messageService;
        private readonly IMessageQueueRepository messageQueueRepository;
        private readonly IGovMessageService govMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyMessagingController"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="messageQueueRepository">The email Queue Repository.</param>
        public GovNotifyMessagingController(IGovNotifyService messageService, IMessageQueueRepository messageQueueRepository, IGovMessageService govMessageService)
        {
            this.messageService = messageService;
            this.messageQueueRepository = messageQueueRepository;
            this.govMessageService = govMessageService;
        }

        /// <summary>
        /// Sends an email using UK Gov.Notify.
        /// </summary>
        /// <param name="request">personalisation.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("SendEmail")]
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequest request)
        {
            try
            {
                var response = await this.govMessageService.SendEmailAsync(request);

                if (!response.IsSuccess)
                {
                    return this.BadRequest(new { error = response.ErrorMessage });
                }

                return this.Ok(response);

            }
            catch (Exception ex)
            {
                return this.Ok(ex.Message);
            }
        }

        /// <summary>
        /// Sends a sms using UK Gov.Notify.
        /// </summary>
        /// <param name="request">SendSmsRequest.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("SendSms")]
        [HttpPost]
        public async Task<IActionResult> SendSmsAsync([FromBody] SmsRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.TemplateId))
                {
                    return this.BadRequest("phoneNumber and template ID are required");
                }

                var response = await this.messageService.SendSmsAsync(request.PhoneNumber, request.TemplateId, request.Personalisation);
                return this.Ok(response);
            }
            catch (Exception ex)
            {
                return this.Ok(ex.Message);
            }
        }

        /// <summary>
        /// To queue the MessageRequests.
        /// </summary>
        /// <param name="request">The QueueRequestList.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("QueueRequests")]
        [HttpPost]
        public async Task<IActionResult> QueueRequests([FromBody] QueueMessageList request)
        {
            ////if (request?.Messages == null || !request.Messages.Any())
            ////{
            ////    return this.BadRequest("At least one email must be provided in the request.");
            ////}

            ////var requests = request.Messages.Select(q => new QueueRequests
            ////{
            ////    Recipient = q.Recipient,
            ////    TemplateId = q.TemplateId,
            ////    Personalisation = q.Personalisation != null ? JsonConvert.SerializeObject(q.Personalisation) : null,
            ////    DeliverAfter = q.DeliverAfter ?? null,
            ////});

            ////await this.messageQueueRepository.QueueMessagesAsync(requests);
            ////return this.Ok(new { Message = $"{requests.Count()} message requests queued successfully." });

            await this.govMessageService.QueueRequestsAsync(request);

            return this.Ok(new { Message = $"{request.Messages.Count} message requests queued successfully." });
        }

        /// <summary>
        /// To fetch the Pending or failed Message Requests.
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("PendingMessageRequests")]
        [HttpGet]
        public async Task<IEnumerable<PendingMessageRequests>> PendingMessageRequests()
        {
            return await this.messageQueueRepository.GetPendingEmailsAsync();
        }

        /// <summary>
        /// Update message request as Success.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("MessageSuccessUpdate")]
        [HttpPost]
        public async Task<IActionResult> MessageSuccessUpdate([FromBody] GovNotifyResponse response)
        {
            await this.messageQueueRepository.MessageDeliverySuccess(response);
            return this.Ok();
        }

        /// <summary>
        /// Update message request as Failed.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("MessageFailedUpdate")]
        [HttpPost]
        public async Task<IActionResult> MessageFailedUpdate([FromBody] GovNotifyResponse response)
        {
            await this.messageQueueRepository.MessageDeliveryFailed(response);
            return this.Ok();
        }
    }
}
