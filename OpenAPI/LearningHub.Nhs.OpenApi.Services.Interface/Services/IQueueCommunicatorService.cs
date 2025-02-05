namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// The IQueueCommunicatorService interface.
    /// </summary>
    public interface IQueueCommunicatorService
    {
        /// <summary>
        /// Read queue message.
        /// </summary>
        /// <typeparam name="T">Object type T.</typeparam>
        /// <param name="message">queue message.</param>
        /// <returns>Generic object.</returns>
        T Read<T>(string message);

        /// <summary>
        /// Send queue message.
        /// </summary>
        /// <param name="queueName">The name of the queue to send to.</param>
        /// <typeparam name="T">Object type T.</typeparam>
        /// <param name="obj">queue message object.</param>
        /// <returns>Task.</returns>
        Task SendAsync<T>(string queueName, T obj);

        /// <summary>
        /// Transfers messages from source to destination queue.
        /// </summary>
        /// <param name="sourceQueueName">The sourceQueueName.</param>
        /// <param name="destinationQueueName">The destinationQueueName.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task TransferMessagesAsync(string sourceQueueName, string destinationQueueName);
    }
}
