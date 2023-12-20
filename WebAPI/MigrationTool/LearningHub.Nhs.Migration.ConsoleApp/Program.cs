// <copyright file="Program.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private static IConfigurationSection settings;

        /// <summary>
        /// The log file path.
        /// </summary>
        private static string logFilePath;

        /// <summary>
        /// The authentication service url.
        /// </summary>
        private static string authenticationServiceUrl;

        /// <summary>
        /// The learning hub api url.
        /// </summary>
        private static string learningHubApiUrl;

        /// <summary>
        /// The api client.
        /// </summary>
        private static HttpClient apiClient;

        private static string username;

        private static string password;

        private static HttpClient ApiClient
        {
            get
            {
                if (apiClient == null)
                {
                    apiClient = GetApiClient().Result;
                }

                return apiClient;
            }
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task Main(string[] args)
        {
            // Get the appsettings.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            settings = builder.Build().GetSection("Settings");
            authenticationServiceUrl = settings["AuthenticationServiceUrl"];
            learningHubApiUrl = settings["LearningHubApiUrl"];

            // Store the path to use for logging.
            Directory.CreateDirectory("./Logs");
            logFilePath = $"./Logs/{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";

            string info = $"Started LearningHub.Nhs.Migration.ConsoleApp. Appsetting - AuthenticationServiceUrl: {authenticationServiceUrl}. Appsetting - LearningHubApiUrl: {learningHubApiUrl}";
            WriteToScreenAndLog(info);

            if (args.Length == 0)
            {
                info = "No command line parameters were entered.";
                WriteToScreenAndLog(info);
                WriteToScreen(GetSupportedCommands());
                return;
            }

            // Process command line args
            switch (args[0])
            {
                case "-createMigration":
                case "-cm":
                    if (args.Length == 1)
                    {
                        info = "A config file path parameter is required when using -createMigration. e.g. '-createMigration config.json'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"CreateMigration parameter passed in. Config file path: {args[1]}");
                        await CreateMigration(args[1]);
                    }

                    break;
                case "-validate":
                case "-v":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -validate. e.g. '-validate 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"Validate parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await ValidateMigration(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "-createResources":
                case "-cr":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -createResources. e.g. '-createResources 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"Create Resources parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await CreateResources(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "-checkCreateResourcesResult":
                case "-ccrr":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -checkCreateResourcesResult. e.g. '-checkCreateResourcesResult 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"CheckCreateResourcesResult parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await CheckCreateResourcesResult(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "-publishResources":
                case "-pr":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -publishResources. e.g. '-publishResources 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"CheckCreateResourcesResult parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await PublishResources(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "-checkPublishResourcesResult":
                case "-cprr":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -publishResources. e.g. '-publishResources 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"checkPublishResourcesResult parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await CheckPublishResourcesResult(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "-unpublishResources":
                case "-ur":
                    if (args.Length == 1)
                    {
                        info = "A migration ID parameter is required when using -unpublishResources. e.g. '-unpublishResources 999'";
                        WriteToScreenAndLog(info);
                    }
                    else
                    {
                        WriteToLog($"unpublishResources parameter passed in. Migration ID: {args[1]}");
                        if (int.TryParse(args[1], out int migrationId) && migrationId > 0)
                        {
                            await UnpublishResources(migrationId);
                        }
                        else
                        {
                            info = "Migration ID parameter was not a positive integer";
                            WriteToScreenAndLog(info);
                        }
                    }

                    break;
                case "/h":
                case "-h":
                case "/?":
                    WriteToScreen(GetSupportedCommands());
                    break;
                default:
                    info = "Unknown command line parameter entered";
                    WriteToScreenAndLog($"{info} - {args[0]}");
                    WriteToScreen(GetSupportedCommands());
                    break;
            }
        }

        /// <summary>
        /// The create migration.
        /// </summary>
        /// <param name="configFilePath">The config file path.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task CreateMigration(string configFilePath)
        {
            // Read in the input config file.
            CreateParamsModel config;
            try
            {
                var configJson = File.ReadAllText(configFilePath);
                config = JsonConvert.DeserializeObject<CreateParamsModel>(configJson);
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"An error occurred when attemping to read the input config file: {ex.ToString()}");
                return;
            }

            if (config.MigrationDataSourceType == null)
            {
                WriteToScreenAndLog($"MigrationDataSourceType config file setting was missing or empty.");
                return;
            }

            if (config.MigrationSourceId == 0)
            {
                WriteToScreenAndLog($"MigrationSourceId config file setting was missing or empty.");
                return;
            }

            if (config.AzureMigrationContainerName == null)
            {
                WriteToScreenAndLog($"AzureMigrationContainerName config file setting was missing or empty.");
                return;
            }

            switch (config.MigrationDataSourceType.ToLower())
            {
                case "metadatafile":
                    await CreateMigrationFromMetadataFile(config);
                    break;
                case "stagingtables":
                    await CreateMigrationFromStagingTables(config);
                    break;
                default:
                    WriteToScreenAndLog($"The MigrationDataSourceType in the config file is not supported. Found value '{config.MigrationDataSourceType}'. Supported values: 'MetadataFile' or 'StagingTables'.");
                    return;
            }
        }

        /// <summary>
        /// The create migration.
        /// </summary>
        /// <param name="config">The config file settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task CreateMigrationFromMetadataFile(CreateParamsModel config)
        {
            if (config.DestinationNodeId == 0)
            {
                WriteToScreenAndLog($"DestinationNodeId config file setting was missing or empty.");
                return;
            }

            // Read in the input metadata file.
            byte[] metadataFileContent = await ReadTheInputMetadataFile(config);
            if (metadataFileContent == null)
            {
                return;
            }

            // Ready to call the migration web service.
            WriteToScreen(new string[]
            {
                "About to create a new migration from a JSON metadata file:",
                $"Metadata File Path: {config.MetadataFilePath}",
                $"Migration Source ID: {config.MigrationSourceId}",
                $"Destination Node ID: {config.DestinationNodeId}",
                $"Azure Migration Container Name: {config.AzureMigrationContainerName}",
                string.Empty,
                "Are you sure? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Creating migration. Metadata File Path: {config.MetadataFilePath}, Migration Source ID: {config.MigrationSourceId}, Destination Node ID: {config.DestinationNodeId}, Azure Migration Container Name: {config.AzureMigrationContainerName}");
            WriteToScreen("Creating new migration....");

            string url = $"{learningHubApiUrl}api/Migration/CreateFromJsonFile/{config.MigrationSourceId}/{config.AzureMigrationContainerName}/{config.DestinationNodeId}";
            await CallMigrationServiceToCreateMigration(config, url, metadataFileContent);
        }

        /// <summary>
        /// The create migration.
        /// </summary>
        /// <param name="config">The config file settings.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task CreateMigrationFromStagingTables(CreateParamsModel config)
        {
            // Read in the input metadata file.
            byte[] metadataFileContent = await ReadTheInputMetadataFile(config);
            if (metadataFileContent == null)
            {
                return;
            }

            // Ready to call the migration web service.
            WriteToScreen(new string[]
            {
                "About to create a new migration via the Staging Tables ADF pipeline:",
                $"Metadata File Path: {config.MetadataFilePath}",
                $"Migration Source ID: {config.MigrationSourceId}",
                $"Azure Migration Container Name: {config.AzureMigrationContainerName}",
                string.Empty,
                "Are you sure? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Creating migration. Using database staging tables as data source. Migration Source ID: {config.MigrationSourceId}, Azure Migration Container Name: {config.AzureMigrationContainerName}");
            WriteToScreen("Creating new migration....");

            // Call the Migration web service.
            string url = $"{learningHubApiUrl}api/Migration/CreateFromStagingTables/{config.MigrationSourceId}/{config.AzureMigrationContainerName}";
            await CallMigrationServiceToCreateMigration(config, url, metadataFileContent);
        }

        private static async Task<byte[]> ReadTheInputMetadataFile(CreateParamsModel config)
        {
            // Check setting was set.
            if (string.IsNullOrEmpty(config.MetadataFilePath))
            {
                WriteToScreenAndLog($"MetadataFilePath config file setting was missing or empty.");
                return null;
            }

            // Check metadata file isn't too big.
            FileInfo fileInfo = new FileInfo(config.MetadataFilePath);
            int fileLimit = Convert.ToInt32(settings["MetadataFileUploadSizeLimit"]);
            if (fileInfo.Length > fileLimit)
            {
                WriteToScreenAndLog($"The input metadata file is too large. File is {fileInfo.Length} bytes, limit is {fileLimit} bytes.");
                return null;
            }

            // Read in the input metadata file.
            byte[] metadataFileContent;
            try
            {
                metadataFileContent = await File.ReadAllBytesAsync(config.MetadataFilePath);
                return metadataFileContent;
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"An error occurred when attemping to read the input metadata file: {ex.ToString()}");
                return null;
            }
        }

        private static async Task CallMigrationServiceToCreateMigration(CreateParamsModel config, string url, byte[] metadataFileContent)
        {
            // Create form object and pass into the Migration web service.
            using (var form = new MultipartFormDataContent())
            using (var fileContent = new ByteArrayContent(metadataFileContent))
            {
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "file", Path.GetFileName(config.MetadataFilePath));
                WriteToScreenAndLog($"Calling URL: {url}");
                var response = await PostAsync(url, form);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    int migrationId = apiResponse.ValidationResult.CreatedId.Value;
                    WriteToScreenAndLog($"New migration created successfully. ID: '{migrationId}'");

                    await ValidateMigration(migrationId);
                }
                else
                {
                    string info = $"Migration creation failed! Error {(int)response.StatusCode} ({response.StatusCode})";
                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                        info += $"{Environment.NewLine}{BuildResponseText(apiResponse)}";
                    }
                    catch (Exception)
                    {
                    }

                    WriteToScreenAndLog(info);
                }
            }
        }

        private static async Task ValidateMigration(int migrationId)
        {
            // Ready to call the migration web service.
            WriteToScreen(new string[]
            {
                $"About to validate migration '{migrationId}'",
                string.Empty,
                "Continue? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Validating migration. Migration ID: {migrationId}");
            WriteToScreen("Validating migration....");

            // Call Migration web service.
            string url = $"{learningHubApiUrl}api/Migration/Validate/{migrationId}";
            WriteToScreenAndLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await PostAsync(url, null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var validationResult = JsonConvert.DeserializeObject<MigrationValidationResult>(responseContent);
                    if (!string.IsNullOrEmpty(validationResult.FundamentalError))
                    {
                        WriteToScreenAndLog($"Migration validation failed. Error: {validationResult.FundamentalError}");
                    }
                    else
                    {
                        WriteToScreenAndLog(BuildValidationOutputText(validationResult));
                    }

                    if (validationResult.AreAnyRecordsValid)
                    {
                        await CreateResources(migrationId);
                    }
                    else
                    {
                        WriteToScreen("Migration contains no valid input records.");
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Migration validation failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"Migration validation failed. Error: {ex.ToString()}");
            }
        }

        private static async Task CreateResources(int migrationId)
        {
            // Ready to call the migration web service.
            WriteToScreen(new string[]
            {
                $"About to create resources for migration '{migrationId}'",
                string.Empty,
                "Continue? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Creating resources. Migration ID: {migrationId}");
            WriteToScreen("Creating resources....");

            // Call Migration web service to initiate the resource creation.
            string url = $"{learningHubApiUrl}api/Migration/BeginCreateMetadata/{migrationId}";
            WriteToScreenAndLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await PostAsync(url, null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    if (!result.Success)
                    {
                        WriteToScreenAndLog($"Resource creation failed. Error: {result.ValidationResult.Details[0]}");
                    }
                    else
                    {
                        WriteToScreenAndLog("Resource creation initiated successfully. Checking status...");
                        await CheckCreateResourcesResult(migrationId);
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Resource creation failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                // Record unhandled error.
                WriteToScreenAndLog($"Resource creation failed. Error: {ex.ToString()}");
            }
        }

        private static async Task CheckCreateResourcesResult(int migrationId)
        {
            bool stillProcessing = true;
            do
            {
                string url = $"{learningHubApiUrl}api/Migration/CheckStatusOfCreateMetadata/{migrationId}";
                HttpResponseMessage response = await GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MigrationResourceCreationResult>(responseContent);
                    if (!string.IsNullOrEmpty(result.FundamentalError))
                    {
                        WriteToScreenAndLog($"Resource creation failed. Error: {result.FundamentalError}");
                        stillProcessing = false;
                    }
                    else
                    {
                        WriteToScreenAndLog($"Total Count: {result.TotalCount} (Not yet processed: {result.NotYetProcessedCount}, Successful: {result.SuccessCount}, Failed: {result.ErrorCount})                    ");
                    }

                    if (result.NotYetProcessedCount > 0)
                    {
                        WriteToScreen("Please wait...");
                        Console.SetCursorPosition(0, Console.CursorTop - 4);
                        await Task.Delay(Convert.ToInt32(settings["PollingIntervalInSeconds"]) * 1000);
                    }
                    else
                    {
                        WriteToScreenAndLog(BuildResourceCreationOutputText(result));

                        if (result.SuccessCount > 0)
                        {
                            await PublishResources(migrationId);
                        }
                        else
                        {
                            WriteToScreen("Migration contains no valid input records.");
                        }

                        stillProcessing = false;
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Call to CheckStatusOfCreateMetadata failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                    stillProcessing = false;
                }
            }
            while (stillProcessing);
        }

        private static async Task PublishResources(int migrationId)
        {
            // Ready to call the migration web service.
            WriteToScreen(new string[]
            {
                $"About to queue resources to be published for migration '{migrationId}'",
                string.Empty,
                "Continue? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Getting IDs of MigrationInputRecords for publishing. Migration ID: {migrationId}");
            WriteToScreen("Retrieving MigrationInputRecord IDs to publish...");

            // Call Migration web service.
            string url = $"{learningHubApiUrl}api/Migration/BeginPublishResources/{migrationId}";
            WriteToScreenAndLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await PostAsync(url, null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var publishResult = JsonConvert.DeserializeObject<MigrationBeginPublishResult>(responseContent);

                    if (!string.IsNullOrEmpty(publishResult.FundamentalError))
                    {
                        WriteToScreenAndLog($"Call to BeginPublishResources failed. Error: {publishResult.FundamentalError}");
                    }
                    else
                    {
                        WriteToScreenAndLog($"Call to BeginPublishResources returned {publishResult.MigrationInputRecordIdsToPublish.Count} MigrationInputRecord ID(s).");
                        WriteToScreenAndLog("Queueing resources for publishing...");

                        if (publishResult.MigrationInputRecordIdsToPublish.Count > 0)
                        {
                            var successfullyQueued = new List<int>();
                            var failedToQueue = new List<int>();

                            foreach (int migrationInputRecordId in publishResult.MigrationInputRecordIdsToPublish)
                            {
                                if (await PublishSingleResource(migrationInputRecordId))
                                {
                                    successfullyQueued.Add(migrationInputRecordId);
                                }
                                else
                                {
                                    failedToQueue.Add(migrationInputRecordId);
                                }
                            }

                            if (successfullyQueued.Any() && !failedToQueue.Any())
                            {
                                WriteToScreenAndLog("All resources were successfully queued for publishing. Now checking publish status...");
                                await CheckPublishResourcesResult(migrationId);
                            }
                            else if (!successfullyQueued.Any() && failedToQueue.Any())
                            {
                                WriteToScreenAndLog("No resources were successfully queued for publish. Unable to continue.");
                            }
                            else
                            {
                                WriteToScreenAndLog($"Some resources failed to be queued for publishing - MigrationInputRecord IDs: {string.Join(", ", failedToQueue)}");
                                WriteToScreenAndLog($"Now checking publish status of those successfully queued...");
                                await CheckPublishResourcesResult(migrationId);
                            }
                        }
                        else
                        {
                            WriteToScreenAndLog($"There are no MigrationInputRecords in the correct state to be published in migration '{migrationId}'.");
                        }
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Call to BeginPublishResources failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"Call to BeginPublishResources failed. Error: {ex.ToString()}");
            }
        }

        private static async Task<bool> PublishSingleResource(int migrationInputRecordId)
        {
            string url = $"{learningHubApiUrl}api/Migration/PublishResourceForSingleInputRecord/{migrationInputRecordId}";
            WriteToLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await PostAsync(url, null);
                var responseContent = await response.Content.ReadAsStringAsync();
                bool succeeded = false;

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    if (result.Success)
                    {
                        WriteToLog($"MigrationInputRecord '{migrationInputRecordId}' was successfully queued for publishing.");
                        succeeded = true;
                    }
                    else
                    {
                        WriteToScreenAndLog($"MigrationInputRecord '{migrationInputRecordId}' failed to be queued for publishing.");
                        WriteToScreenAndLog(BuildResponseText(result));
                    }
                }
                else
                {
                    WriteToLog($"Call to PublishResourceForSingleInputRecord failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }

                return succeeded;
            }
            catch (Exception ex)
            {
                WriteToLog($"Call to PublishResourceForSingleInputRecord failed. Error: {ex.ToString()}");
                return false;
            }
        }

        private static async Task CheckPublishResourcesResult(int migrationId)
        {
            bool stillProcessing = true;
            do
            {
                string url = $"{learningHubApiUrl}api/Migration/CheckStatusOfPublishResources/{migrationId}";
                HttpResponseMessage response = await GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MigrationPublishResult>(responseContent);
                    if (!string.IsNullOrEmpty(result.FundamentalError))
                    {
                        WriteToScreenAndLog($"Call to CheckStatusOfPublishResources failed. Error: {result.FundamentalError}");
                        stillProcessing = false;
                    }
                    else
                    {
                        WriteToScreenAndLog($"Total Count: {result.TotalCount} (Queued for publish: {result.QueuedForPublishCount}, Publish complete: {result.PublishedCount}, Publish failed: {result.PublishFailedCount})                    ");
                    }

                    if (result.QueuedForPublishCount > 0)
                    {
                        WriteToScreen("Please wait...");
                        Console.SetCursorPosition(0, Console.CursorTop - 4);
                        await Task.Delay(Convert.ToInt32(settings["PollingIntervalInSeconds"]) * 1000);
                    }
                    else
                    {
                        WriteToScreenAndLog("Call to CheckPublishResourcesResult shows that the publish operation has finished.");

                        if (result.Errors != null && result.Errors.Count > 0)
                        {
                            WriteToScreenAndLog($"Errors: {string.Join(Environment.NewLine, result.Errors.Select(x => "MigrationInputRecordId: '" + x.Key + "' - Error: " + x.Value).ToList())}");
                        }

                        stillProcessing = false;
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Call to CheckStatusOfPublishResources failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                    stillProcessing = false;
                }
            }
            while (stillProcessing);
        }

        private static async Task UnpublishResources(int migrationId)
        {
            //// Note: This is a quick fix to allow migrated resources to be unpublished so that the migration can be repeated without seeing multiple
            //// resources with the same titles in LH.

            WriteToScreen(new string[]
            {
                $"WARNING! About to unpublish ALL of the resources for migration '{migrationId}'",
                string.Empty,
                "This cannot be undone. Continue? y/n",
            });
            string confirmResponse = Console.ReadLine();
            if (confirmResponse.ToLower() != "y")
            {
                WriteToLog("User did not confirm. Exiting.");
                return;
            }

            WriteToLog($"User confirmed. Getting ResourceVersionIds to be unpublished. Migration ID: {migrationId}");
            WriteToScreen("Retrieving ResourceVersionIds to unpublish...");

            // Call Migration web service.
            string url = $"{learningHubApiUrl}api/Migration/GetPublishedResourceVersionIds/{migrationId}";
            WriteToScreenAndLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var publishResult = JsonConvert.DeserializeObject<MigrationBeginPublishResult>(responseContent);

                    if (!string.IsNullOrEmpty(publishResult.FundamentalError))
                    {
                        WriteToScreenAndLog($"Call to GetPublishedResourceIds failed. Error: {publishResult.FundamentalError}");
                    }
                    else
                    {
                        WriteToScreenAndLog($"Call to GetPublishedResourceIds returned {publishResult.MigrationInputRecordIdsToPublish.Count} ResourceVersionId(s).");
                        WriteToScreenAndLog("Unpublishing the resources...");

                        if (publishResult.MigrationInputRecordIdsToPublish.Count > 0)
                        {
                            foreach (int migrationInputRecordId in publishResult.MigrationInputRecordIdsToPublish)
                            {
                                await UnpublishSingleResource(migrationInputRecordId);
                            }

                            WriteToScreenAndLog("The resources have been unpublished.");
                        }
                        else
                        {
                            WriteToScreenAndLog($"There are no MigrationInputRecords in the correct state to be unpublished for migration '{migrationId}'.");
                        }
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Call to GetPublishedResourceVersionIds failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"Call to GetPublishedResourceVersionIds failed. Error: {ex.ToString()}");
            }
        }

        private static async Task<bool> UnpublishSingleResource(int resourceVersionId)
        {
            var unpublishViewModel = new UnpublishViewModel { ResourceVersionId = resourceVersionId };
            var json = JsonConvert.SerializeObject(unpublishViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            string url = $"{learningHubApiUrl}api/Resource/UnpublishResourceVersion";
            WriteToLog($"Calling URL: {url}");

            try
            {
                HttpResponseMessage response = await PostAsync(url, stringContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                bool succeeded = false;

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    if (result.Success)
                    {
                        WriteToLog($"Resource version '{resourceVersionId}' was successfully unpublished.");
                        succeeded = true;
                    }
                    else
                    {
                        WriteToScreenAndLog($"Resource version '{resourceVersionId}' failed to be unpublished.");
                    }
                }
                else
                {
                    WriteToScreenAndLog($"Call to UnpublishSingleResource failed. Error {(int)response.StatusCode} ({response.StatusCode})");
                }

                return succeeded;
            }
            catch (Exception ex)
            {
                WriteToScreenAndLog($"Call to UnpublishSingleResource failed. Error: {ex.ToString()}");
                return false;
            }
        }

        private static string BuildValidationOutputText(MigrationValidationResult validationResult)
        {
            int validRecordCount = validationResult.InputRecordValidationResults.Where(x => x.IsValid).Count();
            int invalidRecordCount = validationResult.InputRecordValidationResults.Where(x => !x.IsValid).Count();
            int errorCount = validationResult.InputRecordValidationResults.Sum(x => x.Errors.Count());
            int warningCount = validationResult.InputRecordValidationResults.Sum(x => x.Warnings.Count());

            string info = "Migration validation complete." + Environment.NewLine;
            info += $"There were {validRecordCount} valid and {invalidRecordCount} invalid input record(s), with a total of {errorCount} validation error(s) and {warningCount} validation warning(s).";

            foreach (MigrationInputRecordValidationResult inputRecordResult in validationResult.InputRecordValidationResults)
            {
                foreach (string error in inputRecordResult.Errors)
                {
                    info += $"{Environment.NewLine}Error - {GetReferenceText(inputRecordResult)} {error}";
                }

                foreach (string warning in inputRecordResult.Warnings)
                {
                    info += $"{Environment.NewLine}Warning - {GetReferenceText(inputRecordResult)} {warning}";
                }
            }

            return info;
        }

        private static string BuildResourceCreationOutputText(MigrationResourceCreationResult resourceCreationResult)
        {
            string info = "Migration draft resource creation complete." + Environment.NewLine;
            info += $"{resourceCreationResult.SuccessCount} draft resources were created.";

            if (resourceCreationResult.ErrorCount > 0)
            {
                info += $"{resourceCreationResult.ErrorCount} resources failed due to errors:";

                foreach (var error in resourceCreationResult.Errors)
                {
                    info += $"{Environment.NewLine}Error - MigrationInputRecordId {error.Key}. {error.Value}";
                }
            }

            return info;
        }

        private static string GetReferenceText(MigrationInputRecordValidationResult inputRecordResult)
        {
            // If migration type supports unique record references, use them. Otherwise return record index, e.g. #1.
            string recordReference;
            if (!string.IsNullOrEmpty(inputRecordResult.RecordReference))
            {
                recordReference = $"Record ref: '{inputRecordResult.RecordReference}'.";
            }
            else
            {
                recordReference = $"Record #{inputRecordResult.RecordIndex}.";
            }

            if (!string.IsNullOrEmpty(inputRecordResult.RecordTitle))
            {
                recordReference += $" Title: '{inputRecordResult.RecordTitle}'.";
            }

            return recordReference;
        }

        private static async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var response = await ApiClient.PostAsync(requestUri, content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAuthToken();
                response = await PostAsync(requestUri, content);
            }

            return response;
        }

        private static async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var response = await ApiClient.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAuthToken();
                response = await GetAsync(requestUri);
            }

            return response;
        }

        /// <summary>
        /// The get api client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task<HttpClient> GetApiClient()
        {
            // Create a HttpClient to call LH web API.
            var apiClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };
            string authToken = await GetAuthToken();

            if (authToken.Length == 0)
            {
                // Invalid user credentials.
                return null;
            }

            apiClient.SetBearerToken(authToken);

            return apiClient;
        }

        /// <summary>
        /// The get auth token.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private static async Task<string> GetAuthToken()
        {
            // Discover endpoints from metadata.
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(authenticationServiceUrl);
            if (disco.IsError)
            {
                string info = $"Error connecting to authentication web service: {disco.Error}";
                WriteToScreenAndLog(info);
                return string.Empty;
            }

            // Request auth token.
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "password",
                ClientId = "migrationTool",
                ClientSecret = settings["MigrationToolClientKey"],
                Scope = "learninghubapi",
                UserName = AskUserForUsername(),
                Password = AskUserForPassword(),
            });

            if (tokenResponse.IsError)
            {
                string info = $"Invalid username or password: {tokenResponse.Error}";
                WriteToScreenAndLog(info);
                username = string.Empty;
                password = string.Empty;
                return await GetAuthToken();
            }

            WriteToLog("Auth token successfully returned from Auth service.");
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// The ask user for username.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        private static string AskUserForUsername()
        {
            if (string.IsNullOrEmpty(username))
            {
                string usernameTemp;
                do
                {
                    Console.WriteLine("Please enter your Learning Hub user name:");
                    usernameTemp = Console.ReadLine();
                    if (usernameTemp.Trim().Length == 0)
                    {
                        WriteToScreen("A blank username is not valid.");
                    }
                }
                while (usernameTemp.Trim().Length == 0);
                WriteToLog($"Username entered by user: {usernameTemp}");
                username = usernameTemp;
            }

            return username;
        }

        /// <summary>
        /// The ask user for password.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        private static string AskUserForPassword()
        {
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"Please enter your Learning Hub password:");
                string passwordTemp = string.Empty;
                do
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    //// Backspace Should Not Work
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        passwordTemp += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && passwordTemp.Length > 0)
                        {
                            passwordTemp = passwordTemp.Substring(0, passwordTemp.Length - 1);
                            Console.Write("\b \b");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                }
                while (true);

                password = passwordTemp;
                Console.WriteLine();
                Console.WriteLine();
            }

            return password;
        }

        private static async Task RefreshAuthToken()
        {
            string authToken = await GetAuthToken();
            apiClient.SetBearerToken(authToken);
        }

        /// <summary>
        /// The write to log.
        /// </summary>
        /// <param name="info">The info.</param>
        private static void WriteToLog(string info)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now.ToString()}: {info}{Environment.NewLine}");
        }

        private static void WriteToScreen(string info)
        {
            Console.WriteLine(info);
            Console.WriteLine();
        }

        private static void WriteToScreen(string[] lines)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine();
        }

        private static void WriteToScreenAndLog(string info)
        {
            WriteToLog(info);
            WriteToScreen(info);
        }

        /// <summary>
        /// The get supported commands.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetSupportedCommands()
        {
            string supportedCommands =
            @"Supported commands are:

            -createMigration <pathToConfigFile.json>
            -validate <migrationId>
            -createResource <migrationId>
            -checkCreateResourcesResult <migrationId>";

            return supportedCommands;
        }

        private static string BuildResponseText(ApiResponse response)
        {
            string info = string.Empty;
            if (response.ValidationResult != null)
            {
                response.ValidationResult.Details.ForEach(x => info = info += x + Environment.NewLine);
            }

            return info;
        }
    }
}
