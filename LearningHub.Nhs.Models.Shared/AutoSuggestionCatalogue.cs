using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionCatalogue
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the NumberOfHits.
        /// </summary>
        [JsonProperty("numberOfHits")]
        public int TotalHits { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueDocumentList.
        /// </summary>
        [JsonProperty("documents")]
        public List<AutoSuggestionCatalogueDocument> CatalogueDocumentList { get; set; }
    }
}
