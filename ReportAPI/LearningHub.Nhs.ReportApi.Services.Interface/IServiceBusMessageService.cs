// <copyright file="IServiceBusMessageService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services.Interface
{
    using System.Threading.Tasks;

    /// <summary>
    /// The IServiceBusMessageService interface.
    /// </summary>
    public interface IServiceBusMessageService
    {
        /// <summary>
        /// Send Message.
        /// </summary>
        /// <param name="topic">topic.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="message">message.</param>
        /// <returns>Task.</returns>
        Task SendMessageAsync(string topic, string messageId, string message);
    }
}
