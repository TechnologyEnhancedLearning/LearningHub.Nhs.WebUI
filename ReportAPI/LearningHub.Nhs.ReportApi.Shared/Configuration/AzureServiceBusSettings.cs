// <copyright file="AzureServiceBusSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Shared.Configuration
{
    /// <summary>
    /// Config AzureServiceBusSettings.
    /// </summary>
    public class AzureServiceBusSettings
    {
        /// <summary>
        /// Gets or sets pdf report topic name.
        /// </summary>
        public string PdfReportCreateTopicName { get; set; }
    }
}