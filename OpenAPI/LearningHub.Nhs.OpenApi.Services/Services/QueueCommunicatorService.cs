﻿namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Threading.Tasks;
    using Azure.Storage.Queues;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The QueueCommunicatorService class.
    /// </summary>
    public class QueueCommunicatorService : IQueueCommunicatorService
    {
        private AzureConfig azureConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueCommunicatorService"/> class.
        /// </summary>
        /// <param name="settings">config settings.</param>
        public QueueCommunicatorService(IOptions<AzureConfig> azureConfig)
        {
            this.azureConfig = azureConfig.Value;
        }

        /// <summary>
        /// Read queue message.
        /// </summary>
        /// <typeparam name="T">Object type T.</typeparam>
        /// <param name="message">queue message.</param>
        /// <returns>Generic object.</returns>
        public T Read<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        /// <summary>
        /// Send queue message.
        /// </summary>
        /// <param name="queueName">The name of the queue to send to.</param>
        /// <typeparam name="T">Object type T.</typeparam>
        /// <param name="obj">queue message object.</param>
        /// <returns>Task.</returns>
        public async Task SendAsync<T>(string queueName, T obj)
        {
            var queue = new QueueClient(
                azureConfig.AzureStorageQueueConnectionString,
                queueName.ToLower(),
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            await queue.CreateIfNotExistsAsync();

            await queue.SendMessageAsync(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Transfers messages from source to destination queue.
        /// </summary>
        /// <param name="sourceQueueName">The sourceQueueName.</param>
        /// <param name="destinationQueueName">The destinationQueueName.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task TransferMessagesAsync(string sourceQueueName, string destinationQueueName)
        {
            var sourceQueue = new QueueClient(
               azureConfig.AzureStorageQueueConnectionString,
               sourceQueueName.ToLower(),
               new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            var destinationQueue = new QueueClient(
               azureConfig.AzureStorageQueueConnectionString,
               destinationQueueName.ToLower(),
               new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            if (sourceQueue.Exists() && destinationQueue.Exists())
            {
                while (true)
                {
                    var message = await sourceQueue.ReceiveMessageAsync();

                    if (message == null || message?.Value == null)
                    {
                        break;
                    }

                    await destinationQueue.SendMessageAsync(message.Value.Body.ToString());
                    await sourceQueue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                }
            }
        }
    }
}
