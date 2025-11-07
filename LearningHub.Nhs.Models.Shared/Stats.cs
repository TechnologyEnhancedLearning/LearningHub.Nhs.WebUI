using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class Stats
    {
        /// <summary>
        /// Gets or sets the total hits.
        /// </summary>
        [JsonProperty("totalHits")]
        public int TotalHits { get; set; }

        /// <summary>
        /// Gets or sets the search engine time in millis.
        /// </summary>
        [JsonProperty("searchEngineTimeInMillis")]
        public int SearchEngineTimeInMillis { get; set; }

        /// <summary>
        /// Gets or sets the search engine round trip time in millis.
        /// </summary>
        [JsonProperty("searchEngineRoundTripTimeInMillis")]
        public int SearchEngineRoundTripTimeInMillis { get; set; }

        /// <summary>
        /// Gets or sets the total hits.
        /// </summary>
        [JsonProperty("searchProcessingTimeInMillis")]
        public int SearchProcessingTimeInMillis { get; set; }
    }
}
