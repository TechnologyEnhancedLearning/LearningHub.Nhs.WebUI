using LearningHub.Nhs.Models.Organisation;
using LearningHub.Nhs.OpenApi.Models.Configuration;
using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// Client for the NHS Organisation Data Service FHIR API.
    /// </summary>
    public class OdsOrganisationClient : IOdsOrganisationClient
    {
        private readonly HttpClient httpClient;
        private readonly OdsApiOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OdsOrganisationClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="options">The ODS API configuration options.</param>
        public OdsOrganisationClient(HttpClient httpClient, IOptions<OdsApiOptions> options)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
        }

        /// <summary>
        /// Retrieves an ODS organisation record by its ODS code.
        /// </summary>
        /// <param name="odsCode">The ODS code to look up.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>
        /// The organisation record if found; otherwise, null.
        /// </returns>
        public async Task<OdsOrganisationRecord?> GetByOdsCodeAsync(
            string odsCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
            {
                return null;
            }

            var normalisedCode = odsCode.Trim().ToUpperInvariant();

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"Organization/{Uri.EscapeDataString(normalisedCode)}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

            if (!string.IsNullOrWhiteSpace(this.options.ApiKey))
            {
                request.Headers.TryAddWithoutValidation("apikey", this.options.ApiKey);
            }

            using var response = await this.httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var root = document.RootElement;

            var name = GetStringProperty(root, "name");
            var active = GetBooleanProperty(root, "active");
            var postCode = GetPostCode(root);

            return new OdsOrganisationRecord
            {
                OdsCode = normalisedCode,
                OrganisationName = name ?? string.Empty,
                PostCode = postCode,
                Active = active,
            };
        }

        private static string? GetStringProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
            {
                return null;
            }

            return property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : null;
        }

        private static bool GetBooleanProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
            {
                return false;
            }

            return property.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => false,
            };
        }


        private static string? GetPostCode(JsonElement root)
        {
            if (!root.TryGetProperty("address", out var addresses))
            {
                return null;
            }

            if (addresses.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var address in addresses.EnumerateArray())
            {
                var postCode = GetStringProperty(address, "postalCode");

                if (!string.IsNullOrWhiteSpace(postCode))
                {
                    return postCode;
                }
            }

            return null;
        }
    }

}
