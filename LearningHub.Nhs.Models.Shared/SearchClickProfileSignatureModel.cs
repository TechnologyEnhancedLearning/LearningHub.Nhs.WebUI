using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Models.Shared
{
    public class SearchClickProfileSignatureModel
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        [JsonProperty(PropertyName = "applicationId")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the profile type.
        /// </summary>
        [JsonProperty(PropertyName = "profileType")]
        public string ProfileType { get; set; }

        /// <summary>
        /// Gets or sets the profile id.
        /// </summary>
        [JsonProperty(PropertyName = "profileId")]
        public string ProfileId { get; set; }
    }
}
