namespace LearningHub.NHS.OpenAPI.Auth
{
    using AspNetCore.Authentication.ApiKey;

    /// <summary>
    /// Repository for API keys.
    /// </summary>
    public interface IApiKeyRepository
    {
        /// <summary>
        /// Gets the valid ApiKey instance matching the given key if it exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The valid ApiKey instance matching the given key or null if the key is not recognised.</returns>
        IApiKey? GetApiKeyOrNull(string key);
    }
}
