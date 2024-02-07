// <copyright file="AzureDataFactoryService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Azure.Management.DataFactory;
    using Microsoft.Azure.Management.DataFactory.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure.Authentication;

    /// <summary>
    /// The Azure Data Factory service. Use this to run ADF pipelines.
    /// </summary>
    public class AzureDataFactoryService : IAzureDataFactoryService, IDisposable
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AzureDataFactoryService> logger;

        /// <summary>
        /// The data factory client.
        /// </summary>
        private IDataFactoryManagementClient azureDataFactoryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDataFactoryService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public AzureDataFactoryService(IOptions<Settings> settings, ILogger<AzureDataFactoryService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Run an ADF pipeline.
        /// </summary>
        /// <param name="resourceGroup">The resourceGroup.</param>
        /// <param name="factoryName">The ADF factoryName.</param>
        /// <param name="pipelineName">The pipeline name.</param>
        /// <param name="pipelineParameters">The pipeline parameters.</param>
        /// <param name="completionPollingInterval">The time interval (in milliseconds) used when polling the pipeline to check if it has completed. Default is 10000 milliseconds.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RunPipeline(string resourceGroup, string factoryName, string pipelineName, Dictionary<string, object> pipelineParameters, int completionPollingInterval = 10000)
        {
            this.logger.LogInformation($"About to run ADF pipeline. Factory: {factoryName}, Pipeline: {pipelineName}");

            IDataFactoryManagementClient client = await this.CreateDataFactoryClientAsync();

            // Run pipeline
            CreateRunResponse runResponse;
            PipelineRun pipelineRun;

            if (pipelineParameters == null || pipelineParameters.Count() == 0)
            {
                this.logger.LogInformation("Called pipeline without parameters.");
                runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(resourceGroup, factoryName, pipelineName).Result.Body;
            }
            else
            {
                this.logger.LogInformation($"Called pipeline with {pipelineParameters.Count()} parameters: {string.Join(";", pipelineParameters.Select(x => x.Key + "=" + x.Value))}");

                runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(resourceGroup, factoryName, pipelineName, parameters: pipelineParameters).Result.Body;
            }

            this.logger.LogInformation("Pipeline run ID: " + runResponse.RunId);

            // Wait and check for pipeline result
            this.logger.LogInformation("Checking pipeline run status...");
            while (true)
            {
                pipelineRun = client.PipelineRuns.Get(resourceGroup, factoryName, runResponse.RunId);

                if (pipelineRun.Status == "InProgress" || pipelineRun.Status == "Queued")
                {
                    System.Threading.Thread.Sleep(completionPollingInterval);
                }
                else
                {
                    break;
                }
            }

            // Throw an exception if the pipeline indicates that it failed.
            if (pipelineRun.Status == "Failed")
            {
                string error = $"Azure Data Factory pipeline run failed. Factory: {factoryName}, Pipeline: {pipelineName}, Pipeline run ID: {runResponse.RunId}, Error: {pipelineRun.Message}";
                this.logger.LogInformation(error);
                throw new Exception(error);
            }
            else
            {
                this.logger.LogInformation("Azure Data Factory pipeline run succeeded.");
            }
        }

        /// <summary>
        /// Creates the DataFactoryManagementClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IDataFactoryManagementClient> CreateDataFactoryClientAsync()
        {
            if (string.IsNullOrEmpty(this.settings.AzureClientCredentials.AadClientId) ||
                string.IsNullOrEmpty(this.settings.AzureClientCredentials.AadSecret) ||
                string.IsNullOrEmpty(this.settings.AzureClientCredentials.TenantId) ||
                string.IsNullOrEmpty(this.settings.AzureClientCredentials.SubscriptionId))
            {
                throw new ConfigurationErrorsException("Incomplete AzureClientCredentials configuration in appsettings. Check that AadClientId, AadSecret, TenantId and SubscriptionId are all set.");
            }

            if (this.azureDataFactoryClient != null)
            {
                return this.azureDataFactoryClient;
            }

            var credentials = await this.GetCredentialsAsync();

            this.azureDataFactoryClient = new DataFactoryManagementClient(credentials)
            {
                SubscriptionId = this.settings.AzureClientCredentials.SubscriptionId,
            };

            return this.azureDataFactoryClient;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispoase.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.azureDataFactoryClient.Dispose();
            }
        }

        /// <summary>
        /// Get AzureMedia Credentials.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<ServiceClientCredentials> GetCredentialsAsync()
        {
            ClientCredential clientCredential = new ClientCredential(this.settings.AzureClientCredentials.AadClientId, this.settings.AzureClientCredentials.AadSecret);
            return await ApplicationTokenProvider.LoginSilentAsync(this.settings.AzureClientCredentials.TenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }
    }
}
