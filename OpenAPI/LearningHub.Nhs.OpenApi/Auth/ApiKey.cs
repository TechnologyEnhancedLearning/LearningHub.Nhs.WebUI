// <copyright file="ApiKey.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Auth
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using AspNetCore.Authentication.ApiKey;

    /// <inheritdoc />
    public class ApiKey : IApiKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKey"/> class.
        /// </summary>
        /// <param name="key">The valid API Key.</param>
        /// <param name="ownerName">The owner name.</param>
        /// <param name="claims">The claims.</param>
        public ApiKey(string key, string ownerName, IReadOnlyCollection<Claim>? claims = null)
        {
            this.Key = key;
            this.OwnerName = ownerName;
            this.Claims = claims;
        }

        /// <inheritdoc/>
        public string Key { get; }

        /// <inheritdoc/>
        public string OwnerName { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Claim>? Claims { get; }
    }
}
