namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;

    /// <summary>
    /// Defines the <see cref="InMemoryTicketStore" />.
    /// </summary>
    public class InMemoryTicketStore : ITicketStore
    {
        private readonly ConcurrentDictionary<string, AuthenticationTicket> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTicketStore"/> class.
        /// The InMemoryTicketStore.
        /// </summary>
        /// <param name="cache">the cache.</param>
        public InMemoryTicketStore(ConcurrentDictionary<string, AuthenticationTicket> cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// The StoreAsync.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns>The key.</returns>
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var ticketUserId = ticket.Principal.Claims.Where(c => c.Type == "sub")
                .FirstOrDefault()
                .Value;
            var matchingAuthTicket = this.cache.Values.FirstOrDefault(
                t => t.Principal.Claims.FirstOrDefault(
                    c => c.Type == "sub"
                    && c.Value == ticketUserId) != null);
            if (matchingAuthTicket != null)
            {
                var cacheKey = this.cache.Where(
                    entry => entry.Value == matchingAuthTicket)
                    .Select(entry => entry.Key)
                    .FirstOrDefault();
                this.cache.TryRemove(
                    cacheKey,
                    out _);
            }

            var key = Guid
                .NewGuid()
                .ToString();
            await this.RenewAsync(
                key,
                ticket);
            return key;
        }

        /// <summary>
        /// The RenewAsync.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ticket">The ticket.</param>
        /// <returns>The Task.</returns>
        public Task RenewAsync(
            string key,
            AuthenticationTicket ticket)
        {
            this.cache.AddOrUpdate(
                key,
                ticket,
                (_, _) => ticket);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The RetrieveAsync.
        /// </summary>
        /// <param name="key">The Key.</param>
        /// <returns>The Task.</returns>
        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            this.cache.TryGetValue(
                key,
                out var ticket);
            return Task.FromResult(ticket);
        }

        /// <summary>
        /// The RemoveAsync.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The Task.</returns>
        public Task RemoveAsync(string key)
        {
            this.cache.TryRemove(
                key,
                out _);
            return Task.CompletedTask;
        }
    }
}