using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using TELBlazor.Components.Core.Models.Logging;
using TELBlazor.Components.Core.Services.HelperServices;

namespace LearningHub.Nhs.WebUI.BlazorComponents.Services.HelperServices;

public class NLogLogLevelSwitcherService : ILogLevelSwitcherService
{
    private readonly ILogger<NLogLogLevelSwitcherService> _logger;
    private readonly ILocalStorageService _localStorage;
    private const string LogLevelKey = "logLevel";

    public bool IsInitialized { get; set; } = false;

    public NLogLogLevelSwitcherService(
        ILogger<NLogLogLevelSwitcherService> logger,
        ILocalStorageService localStorage)
    {
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task InitializeLogLevelFromAsyncSourceIfAvailable()
    {
        // NLog does not support runtime level switching in WASM
        // Here only to mirror the interface
        _logger.LogInformation("NLog does not support dynamic runtime log level switching.");
        await Task.CompletedTask;
    }

    public List<string> GetAvailableLogLevels() =>
        Enum.GetNames(typeof(LogLevel)).ToList();

    public string GetCurrentLogLevel()
    {
        _logger.LogInformation("Returning default log level (NLog does not support querying runtime level).");
        return "Information";
    }

    public string SetLogLevel(string level)
    {
        //if (string.IsNullOrWhiteSpace(level))
        //{
        //    _logger.LogWarning("Log level was null or empty.");
        //    return GetCurrentLogLevel();
        //}

        //if (!Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
        //{
        //    _logger.LogWarning("Invalid log level: {Level}", level);
        //    return GetCurrentLogLevel();
        //}

        _logger.LogInformation("Requested to change log level to {Level}, but NLog does not support runtime changes in WASM.", level);
        LogAllLevels("After 'Change'");

        _ = StoreLogLevelWithTimestamp(level); // Fire and forget
        return GetCurrentLogLevel();
    }

    private void LogAllLevels(string phase)
    {
        _logger.LogTrace("[{Phase}] TRACE log", phase);
        _logger.LogDebug("[{Phase}] DEBUG log", phase);
        _logger.LogInformation("[{Phase}] INFO log", phase);
        _logger.LogWarning("[{Phase}] WARN log", phase);
        _logger.LogError("[{Phase}] ERROR log", phase);
        _logger.LogCritical("[{Phase}] CRITICAL log", phase);
    }

    private async Task StoreLogLevelWithTimestamp(string level)
    {
        try
        {
            var newItem = new LocalStorageLogLevel
            {
                Level = level,
                Expires = DateTime.UtcNow.AddHours(24)
            };

            await _localStorage.SetItemAsync(LogLevelKey, newItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing log level to local storage.");
        }
    }

    //private async Task<string?> GetStoredLogLevelWithExpiration()
    //{
    //    try
    //    {
    //        var storedItem = await _localStorage.GetItemAsync<LocalStorageLogLevel>(LogLevelKey);
    //        if (storedItem == null || DateTime.UtcNow > storedItem.Expires)
    //        {
    //            await _localStorage.RemoveItemAsync(LogLevelKey);
    //            return null;
    //        }

    //        return storedItem.Level;
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //}
}