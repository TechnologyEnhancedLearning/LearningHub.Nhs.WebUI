// <copyright file="AuthConfiguration.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.NHS.OpenAPI.Auth;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using NLog;

    /// <summary>
    /// Auth section of configuration.
    /// </summary>
    public class AuthConfiguration : IHasPostConfigure
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets clients.
        /// </summary>
        public IEnumerable<ApiKeyClient>? Clients { get; set; }

        /// <inheritdoc />
        public void PostConfigure() => this.DropApiKeyClientsWithMissingNameOrKey();

        private static List<ApiKeyClient> GetClientsWithBothNameAndKey(IEnumerable<ApiKeyClient> clients)
        {
            // We need to check for null client Keys or Name here,
            // because we haven't dropped clients without Keys or Name yet.
            var validApiKeyClients = clients.Where(
                    client => client.Keys != null && !string.IsNullOrWhiteSpace(client.Name))
                .ToList();
            return validApiKeyClients;
        }

        private void DropApiKeyClientsWithMissingNameOrKey()
        {
            if (this.Clients == null)
            {
                return;
            }

            var validApiKeyClients = GetClientsWithBothNameAndKey(this.Clients);

            var invalidApiKeyClients = this.Clients.Except(validApiKeyClients);

            foreach (var client in invalidApiKeyClients)
            {
                Logger.Warn(
                    $"Ignoring invalid API key client with name '{client.Name}'. All clients must have key and non-whitespace name.");
            }

            this.Clients = validApiKeyClients;
        }
    }
}
