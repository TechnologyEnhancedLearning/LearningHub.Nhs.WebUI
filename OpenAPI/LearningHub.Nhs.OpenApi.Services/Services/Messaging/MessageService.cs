namespace LearningHub.Nhs.OpenApi.Services.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Messaging;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The MessageService class.
    /// </summary>
    public class MessageService : BaseService<Message>, IMessageService
    {
        private readonly IMessageRepository messageRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="findwiseClient">The findwiseHttpClient.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageRepository">The message repository.</param>
        public MessageService(
            IFindwiseClient findwiseClient,
            ILogger<Message> logger,
            IMessageRepository messageRepository)
            : base(findwiseClient, logger)
        {
            this.messageRepository = messageRepository;
        }

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipient user id.</param>
        /// <returns>The task.</returns>
        public async Task CreateEmailAsync(int userId, string subject, string body, int recipientUserId)
        {
            await messageRepository.CreateEmailAsync(userId, subject, body, recipientUserId);
        }

        /// <summary>
        /// The CreateEmailAsync.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientEmailAddress">The recipientEmailAddress.</param>
        /// <returns>The task.</returns>
        public async Task CreateEmailAsync(int userId, string subject, string body, string recipientEmailAddress)
        {
            await messageRepository.CreateEmailAsync(userId, subject, body, recipientEmailAddress);
        }

        /// <summary>
        /// The CreateNotificationForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="recipientUserId">The recipientUserId.</param>
        /// <param name="notificationStartDate">The notificationStartDate.</param>
        /// <param name="notificationEndDate">The notificationEndDate.</param>
        /// <param name="notificationPriority">The notificationPriority.</param>
        /// <param name="notificationType">The notificationType.</param>
        /// <returns>The task.</returns>
        public async Task CreateNotificationForUserAsync(int userId, string subject, string body, int recipientUserId, DateTimeOffset notificationStartDate, DateTimeOffset notificationEndDate, int notificationPriority, int notificationType)
        {
            await messageRepository.CreateNotificationForUserAsync(userId, subject, body, recipientUserId, notificationStartDate, notificationEndDate, notificationPriority, notificationType);
        }

        /// <summary>
        /// The GetPendingMessages.
        /// </summary>
        /// <returns>The pending messages.</returns>
        public List<MessageViewModel> GetPendingMessages()
        {
            var messages = messageRepository.GetPendingMessages().ToList();
            return this.ResolveMessageViewModels(messages);
        }

        /// <summary>
        /// The MarkAsFailedAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageFailedIds">The messageFailedIds.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> MarkAsFailedAsync(int userId, List<int> messageFailedIds)
        {
            try
            {
                await messageRepository.MessageSendFailure(userId, messageFailedIds);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message);
                return new LearningHubValidationResult(false);
            }

            return new LearningHubValidationResult(true);
        }

        /// <summary>
        /// The MarkAsSentAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="messageSendIds">The messageSendIds.</param>
        /// <returns>The validation result.</returns>
        public async Task<LearningHubValidationResult> MarkAsSentAsync(int userId, List<int> messageSendIds)
        {
            try
            {
                await messageRepository.MessageSendSuccess(userId, messageSendIds);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message);
                return new LearningHubValidationResult(false);
            }

            return new LearningHubValidationResult(true);
        }

        private List<MessageViewModel> ResolveMessageViewModels(List<FullMessageDto> fullMessages)
        {
            var groupedMessages = fullMessages.GroupBy(x => x.MessageId);
            var messageVmDict = new Dictionary<int, MessageViewModel>();
            var messageSendVms = new List<MessageSendViewModel>();
            foreach (var message in fullMessages)
            {
                if (!messageVmDict.ContainsKey(message.MessageId))
                {
                    messageVmDict[message.MessageId] = new MessageViewModel
                    {
                        Subject = message.Subject,
                        Body = message.Body,
                        NotificationPriority = message.NotificationPriority,
                        NotificationType = message.NotificationType,
                        NotificationStartDate = message.NotificationStartDate,
                        NotificationEndDate = message.NotificationEndDate,
                        Id = message.MessageId,
                        Sends = new List<MessageSendViewModel>(),
                    };
                }

                var messageVm = messageVmDict[message.MessageId];
                MessageSendViewModel messageSendVm = messageVm.Sends.SingleOrDefault(x => x.Id == message.MessageSendId);
                if (messageSendVm == null)
                {
                    messageSendVm = new MessageSendViewModel
                    {
                        Id = message.MessageSendId,
                        MessageType = (MessageTypes)message.MessageTypeId,
                        EmailRecipients = new List<string>(),
                        UserIds = new List<int>(),
                    };
                    messageVm.Sends.Add(messageSendVm);
                }

                if (!string.IsNullOrEmpty(message.EmailAddress))
                {
                    messageSendVm.EmailRecipients.Add(message.EmailAddress);
                }

                if (message.UserId.HasValue)
                {
                    messageSendVm.UserIds.Add(message.UserId.Value);
                }

                messageSendVms.Add(messageSendVm);
            }

            return messageVmDict.Values.ToList();
        }
    }
}
