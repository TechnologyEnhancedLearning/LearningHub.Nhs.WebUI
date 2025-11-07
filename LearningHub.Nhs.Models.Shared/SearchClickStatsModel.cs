using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class SearchClickStatsModel
    {
        /// <summary>
        /// Gets or sets the total hits.
        /// </summary>
        [JsonProperty(PropertyName = "totalHits")]
        public int TotalHits { get; set; }
    }
}
