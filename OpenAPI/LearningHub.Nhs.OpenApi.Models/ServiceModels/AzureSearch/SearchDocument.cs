namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.AzureSearch
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// Represents a search document for Azure AI Search integration.
    /// </summary>
    public class SearchDocument
    {
        private string _description = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string PrefixedId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the numeric ID extracted from the PrefixedId.
        /// </summary>
        [JsonIgnore]
        public string Id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PrefixedId))
                    return "0"; 

                var parts = PrefixedId.Split('-');
                if (parts.Length != 2)
                    return "0";

                return int.TryParse(parts[1], out int id) ? id.ToString() : "0";
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set => _description = StripParagraphTags(value);
        }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        [JsonPropertyName("resource_collection")]
        public string ResourceCollection { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the catalogue identifier.
        /// </summary>
        [JsonPropertyName("catalogue_id")]
        public string CatalogueId { get; set; } = string.Empty;

        [JsonPropertyName("resource_reference_id")]
        public string ResourceReferenceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manual tag JSON.
        /// </summary>
        [JsonPropertyName("manual_tag")]
        public string ManualTagJson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manual tags.
        /// </summary>
        [JsonPropertyName("manualTags")]
        public List<string> ManualTags { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonPropertyName("resource_type")]
        public string? ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date authored.
        /// </summary>
        [JsonPropertyName("date_authored")]
        public DateTime? DateAuthored { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        /// <summary>
        /// Gets or sets the provider IDs.
        /// </summary>
        [JsonPropertyName("provider_ids")]
        public string ProviderIds { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this is statutory mandatory.
        /// </summary>
        [JsonPropertyName("statutory_mandatory")]
        public bool? StatutoryMandatory { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Strips paragraph tags from input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The cleaned string.</returns>
        private static string StripParagraphTags(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input, @"&lt;\/?p[^&gt;]*&gt;", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Parses the ManualTagJson into ManualTags list.
        /// </summary>
        public void ParseManualTags()
        {
            if (!string.IsNullOrEmpty(ManualTagJson))
            {
                try
                {
                    ManualTags = System.Text.Json.JsonSerializer.Deserialize<List<string>>(ManualTagJson) ?? new List<string>();
                }
                catch
                {
                    ManualTags = new List<string>();
                }
            }
        }
    }
}
