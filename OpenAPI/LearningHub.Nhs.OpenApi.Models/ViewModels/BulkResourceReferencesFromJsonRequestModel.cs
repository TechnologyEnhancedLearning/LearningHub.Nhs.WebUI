namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    ///     Class.
    /// </summary>
    public class BulkResourceReferencesFromJsonRequestModel
    {
        /// <summary>
        ///     Gets or sets <see cref="ResourceReferenceIds" />.
        /// </summary>
        [JsonProperty("referenceIds")]
        public List<int> ResourceReferenceIds { get; set; }
    }
}