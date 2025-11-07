using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionConceptDocument
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty("_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Concept.
        /// </summary>
        [JsonProperty("concept")]
        public string Concept { get; set; }

        /// <summary>
        /// Gets or sets the click details.
        /// </summary>
        [JsonProperty("_click")]
        public AutoSuggestionClickModel Click { get; set; }
    }
}
