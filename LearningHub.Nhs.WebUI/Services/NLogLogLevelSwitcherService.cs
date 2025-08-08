namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazored.LocalStorage;
    using Microsoft.Extensions.Logging;
    using TELBlazor.Components.Core.Models.Logging;
    using TELBlazor.Components.Core.Services.HelperServices;

    /// <summary>
    /// Provides functionality for managing log levels in applications using NLog.
    /// </summary>
    /// <remarks>This service implements the <see cref="ILogLevelSwitcherService"/> interface to provide log
    /// level management. Note that NLog does not support runtime log level switching in WASM environments. As a result,
    /// methods in this service primarily serve to fulfill the interface contract and provide default
    /// behaviors.</remarks>
    public class NLogLogLevelSwitcherService : ILogLevelSwitcherService
    {
        private const string LogLevelKey = "logLevel";
        private readonly ILogger<NLogLogLevelSwitcherService> logger;
        private readonly ILocalStorageService localStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogLevelSwitcherService"/> class.
        /// </summary>
        /// <param name="localStorage">The local storage service.</param>
        /// <param name="logger">The logger instance.</param>
        public NLogLogLevelSwitcherService(ILocalStorageService localStorage, ILogger<NLogLogLevelSwitcherService> logger)
        {
            this.localStorage = localStorage;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; set; } = false;

        /// <inheritdoc/>
        public async Task InitializeLogLevelFromAsyncSourceIfAvailable()
        {
            // NLog does not support runtime level switching in WASM
            // Here only to mirror the interface
            this.logger.LogInformation("NLog does not support dynamic runtime log level switching.");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public List<string> GetAvailableLogLevels() =>
            Enum.GetNames(typeof(LogLevel)).ToList();

        /// <inheritdoc/>
        public string GetCurrentLogLevel()
        {
            this.logger.LogInformation("Returning default log level (NLog does not support querying runtime level).");
            return "Information";
        }

        /// <inheritdoc/>
        public string SetLogLevel(string level)
        {
            this.logger.LogInformation("Requested to change log level to {Level}, but NLog does not support runtime changes in WASM.", level);
            this.LogAllLevels("After 'Change'");

            _ = this.StoreLogLevelWithTimestamp(level); // Fire and forget
            return this.GetCurrentLogLevel();
        }

        private void LogAllLevels(string phase)
        {
            this.logger.LogTrace("[{Phase}] TRACE log", phase);
            this.logger.LogDebug("[{Phase}] DEBUG log", phase);
            this.logger.LogInformation("[{Phase}] INFO log", phase);
            this.logger.LogWarning("[{Phase}] WARN log", phase);
            this.logger.LogError("[{Phase}] ERROR log", phase);
            this.logger.LogCritical("[{Phase}] CRITICAL log", phase);
        }

        private async Task StoreLogLevelWithTimestamp(string level)
        {
            try
            {
                var newItem = new LocalStorageLogLevel
                {
                    Level = level,
                    Expires = DateTime.UtcNow.AddHours(24),
                };

                await this.localStorage.SetItemAsync(LogLevelKey, newItem);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error storing log level to local storage.");
            }
        }
    }
}
