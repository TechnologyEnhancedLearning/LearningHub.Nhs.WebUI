using LearningHub.Nhs.Caching;

namespace LearningHub.Nhs.WebUI.BlazorClient.Services
{
    /// <summary>
    /// We may use storage, we may just stub it and throw an error, we cant directly use redis we may access it via an api
    /// </summary>
    public class WasmCacheServiceStub : ICacheService
    {
        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(default(T));
        }

        public Task<(bool Success, T Value)> TryGetAsync<T>(string key)
        {
            return Task.FromResult((false, default(T)));
        }

        public Task<T> SetAsync<T>(string key, T value)
        {
            return Task.FromResult(value);
        }

        public Task RemoveAsync(string key)
        {
            return Task.CompletedTask;
        }

        public Task<T> SetAsync<T>(string key, T value, int? expiryInMinutes, bool slidingExpiration = true)
        {
            return Task.FromResult(value);
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue)
        {
            return Task.FromResult(getValue());
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue, int? expiryInMinutes, bool slidingExpiration = true)
        {
            return Task.FromResult(getValue());
        }

        public Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue)
        {
            return getValue();
        }

        public Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue, int? expiryInMinutes, bool slidingExpiration = true)
        {
            return getValue();
        }

        public Task FlushAll()
        {
            return Task.CompletedTask;
        }

    }
}
