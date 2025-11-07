using Newtonsoft.Json;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionModel
    {
        /// <summary>
        /// Gets or sets the stats.
        /// </summary>
        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        /// <summary>
        /// Gets or sets the Catalogue documents.
        /// </summary>
        [JsonProperty("catalogues_documents")]
        public AutoSuggestionCatalogue CatalogueDocument { get; set; }

        /// <summary>
        /// Gets or sets the Concept documents.
        /// </summary>
        [JsonProperty("concepts_documents")]
        public AutoSuggestionConcept ConceptDocument { get; set; }

        /// <summary>
        /// Gets or sets the Resource Documents.
        /// </summary>
        [JsonProperty("resources_documents")]
        public AutoSuggestionResource ResourceDocument { get; set; }
    }
}
