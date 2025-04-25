namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.MessagingService.Interfaces;
    using LearningHub.Nhs.MessagingService.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// GovNotify Messaging Controller.
    /// </summary>
    [Route("GovNotifyMessage")]
    [Authorize]
    public class GovNotifyMessagingController : OpenApiControllerBase
    {
        private readonly IGovNotifyService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovNotifyMessagingController"/> class.
        /// </summary>
        /// <param name="messageService">The catalogue service.</param>
        public GovNotifyMessagingController(IGovNotifyService messageService)
        {
            this.messageService = messageService;
        }

        /// <summary>
        /// Sends an email using UK Gov.Notify.
        /// </summary>
        /// <param name="request">personalisation.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("sendemail")]
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync([FromBody] SendEmailRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.TemplateId))
                {
                    return this.BadRequest("Email and template ID are required");
                }

                var response = await this.messageService.SendEmailAsync(request.Email, request.TemplateId, request.Personalisation);
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
        [Route("sendsms")]
        [HttpPost]
        public async Task<IActionResult> SendSmsAsync([FromBody] SendSmsRequest request)
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
    }
}
