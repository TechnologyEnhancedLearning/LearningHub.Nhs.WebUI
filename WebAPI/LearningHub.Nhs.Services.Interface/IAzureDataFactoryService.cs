// <copyright file="IAzureDataFactoryService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to define methods for interacting with the Azure Data Factory service.
    /// </summary>
    public interface IAzureDataFactoryService
    {
        /// <summary>
        /// Run an ADF pipeline.
        /// </summary>
        /// <param name="resourceGroup">The resourceGroup.</param>
        /// <param name="factoryName">The ADF factoryName.</param>
        /// <param name="pipelineName">The pipeline name.</param>
        /// <param name="pipelineParameters">The pipeline parameters.</param>
        /// <param name="completionPollingInterval">The time interval (in milliseconds) used when polling the pipeline to check if it has completed. Default is 10000 milliseconds.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RunPipeline(string resourceGroup, string factoryName, string pipelineName, Dictionary<string, object> pipelineParameters, int completionPollingInterval = 10000);
    }
}
