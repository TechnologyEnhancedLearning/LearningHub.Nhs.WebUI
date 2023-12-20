// <copyright file="ServiceBusMessageService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.ReportApi.Services
{
    using System.Text;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using LearningHub.Nhs.ReportApi.Shared.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Tha message service.
    /// </summary>
    public class ServiceBusMessageService : IServiceBusMessageService
    {
        private readonly ILogger<ServiceBusMessageService> logger;
        private readonly Settings settings;
        private readonly ServiceBusClient serviceBusClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusMessageService"/> class.
        /// </summary>
        /// <param name="serviceBusClient">The service bus client.</param>
        /// <param name="logger">The logger<see cref="ILogger{ServiceBusMessageService}"/>.</param>
        /// <param name="settings">The settings.</param>
        public ServiceBusMessageService(
            ServiceBusClient serviceBusClient,
            ILogger<ServiceBusMessageService> logger,
            IOptions<Settings> settings)
        {
            this.serviceBusClient = serviceBusClient;
            this.logger = logger;
            this.settings = settings.Value;
        }

        /// <summary>
        /// Send Message.
        /// </summary>
        /// <param name="topic">topic.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="message">message.</param>
        /// <returns>Task.</returns>
        public async Task SendMessageAsync(string topic, string messageId, string message)
        {
            var serviceBusSender = this.serviceBusClient.CreateSender(topic);

            if (!string.IsNullOrEmpty(message))
            {
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));

                if (!string.IsNullOrEmpty(messageId))
                {
                    serviceBusMessage.MessageId = messageId;
                }

                await serviceBusSender.SendMessageAsync(serviceBusMessage);
            }
        }
    }
}
