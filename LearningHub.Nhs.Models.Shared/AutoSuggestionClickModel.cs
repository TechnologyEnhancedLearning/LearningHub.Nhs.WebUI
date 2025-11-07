using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionClickModel
    {
        /// <summary>
        /// Gets or sets the search feedback action payload.
        /// </summary>
        [JsonProperty(PropertyName = "payload")]
        public AutoSuggestionClickPayloadModel Payload { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
