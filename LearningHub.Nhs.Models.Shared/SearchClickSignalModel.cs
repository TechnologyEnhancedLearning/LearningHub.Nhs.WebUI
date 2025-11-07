using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class SearchClickSignalModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchClickSignalModel"/> class.
        /// </summary>
        public SearchClickSignalModel()
        {
            this.Stats = new SearchClickStatsModel();
            this.ProfileSignature = new SearchClickProfileSignatureModel();
        }

        /// <summary>
        /// Gets or sets the search stats.
        /// </summary>
        [JsonProperty(PropertyName = "stats")]
        public SearchClickStatsModel Stats { get; set; }

        /// <summary>
        /// Gets or sets the search id.
        /// </summary>
        [JsonProperty(PropertyName = "searchId")]
        public string SearchId { get; set; }

        /// <summary>
        /// Gets or sets the profile signature.
        /// </summary>
        [JsonProperty(PropertyName = "profileSignature")]
        public SearchClickProfileSignatureModel ProfileSignature { get; set; }

        /// <summary>
        /// Gets or sets the user query.
        /// </summary>
        [JsonProperty(PropertyName = "userQuery")]
        public string UserQuery { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the time of search.
        /// </summary>
        [JsonProperty(PropertyName = "timeOfSearch")]
        public long TimeOfSearch { get; set; }
    }
}
