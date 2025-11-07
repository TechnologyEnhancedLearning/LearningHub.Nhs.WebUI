using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionResource
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
        /// Gets or sets the ResourceDocumentList.
        /// </summary>
        [JsonProperty("documents")]
        public List<AutoSuggestionResourceDocument> ResourceDocumentList { get; set; }
    }
}
