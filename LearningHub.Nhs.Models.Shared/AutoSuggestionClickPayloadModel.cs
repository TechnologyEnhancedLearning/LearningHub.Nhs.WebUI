using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class AutoSuggestionClickPayloadModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuggestionClickPayloadModel"/> class.
        /// </summary>
        public AutoSuggestionClickPayloadModel()
        {
            this.SearchSignal = new SearchClickSignalModel();
            this.DocumentFields = new SearchClickDocumentModel();
        }

        /// <summary>
        /// Gets or sets the search action.
        /// </summary>
        [JsonProperty(PropertyName = "searchSignal")]
        public SearchClickSignalModel SearchSignal { get; set; }

        /// <summary>
        /// Gets or sets the hit number.
        /// </summary>
        [JsonProperty(PropertyName = "hitNumber")]
        public int HitNumber { get; set; }

        /// <summary>
        /// Gets or sets the click target url.
        /// </summary>
        [JsonProperty(PropertyName = "clickTargetUrl")]
        public string ClickTargetUrl { get; set; }

        /// <summary>
        /// Gets or sets the search stats.
        /// </summary>
        [JsonProperty(PropertyName = "documentFields")]
        public SearchClickDocumentModel DocumentFields { get; set; }

        /// <summary>
        /// Gets or sets the container Id.
        /// </summary>
        [JsonProperty(PropertyName = "containerId")]
        public string ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the time of click.
        /// </summary>
        [JsonProperty(PropertyName = "timeOfClick")]
        public long? TimeOfClick { get; set; }
    }
}
