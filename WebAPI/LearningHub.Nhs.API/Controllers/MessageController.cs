// <copyright file="MessageController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Services.Interface;
    using LearningHub.Nhs.Services.Interface.Messaging;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The message controller.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ApiControllerBase
    {
        private readonly IMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="userService">The userService.</param>
        /// <param name="messageService">The messageService.</param>
        /// <param name="logger">The logger.</param>
        public MessageController(
            IUserService userService,
            IMessageService messageService,
            ILogger<MessageController> logger)
            : base(userService, logger)
        {
            this.messageService = messageService;
        }

        /// <summary>
        /// Gets the pending messages.
        /// </summary>
        /// <returns>The pending messages.</returns>
        [HttpGet("Pending")]
        public List<MessageViewModel> GetPendingMessages()
        {
            var messages = this.messageService.GetPendingMessages();
            return messages;
        }

        /// <summary>
        /// Marks the message sends as sent.
        /// </summary>
        /// <param name="messageSendIds">The messageSendIds.</param>
        /// <returns>The apiResponse.</returns>
        [HttpPut("MarkAsSent")]
        public async Task<ApiResponse> MarkAsSent(List<int> messageSendIds)
        {
            var vr = await this.messageService.MarkAsSentAsync(4, messageSendIds);
            if (!vr.IsValid)
            {
                return new ApiResponse(false, vr);
            }

            return new ApiResponse(true);
        }

        /// <summary>
        /// Marks the message sends as sent.
        /// </summary>
        /// <param name="messageSendIds">The messageSendIds.</param>
        /// <returns>The apiResponse.</returns>
        [HttpPut("MarkAsSending")]
        public async Task<ApiResponse> MarkAsSending(List<int> messageSendIds)
        {
            var vr = await this.messageService.MarkAsSentAsync(4, messageSendIds);
            if (!vr.IsValid)
            {
                return new ApiResponse(false, vr);
            }

            return new ApiResponse(true);
        }

        /// <summary>
        /// Marks the message sends as sent.
        /// </summary>
        /// <param name="messageFailedIds">The messageFailedIds.</param>
        /// <returns>The apiResponse.</returns>
        [HttpPut("MarkAsFailed")]
        public async Task<ApiResponse> MarkAsFailed(List<int> messageFailedIds)
        {
            var vr = await this.messageService.MarkAsFailedAsync(4, messageFailedIds);
            if (!vr.IsValid)
            {
                return new ApiResponse(false, vr);
            }

            return new ApiResponse(true);
        }
    }
}
