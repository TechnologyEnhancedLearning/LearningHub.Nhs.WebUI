// <copyright file="ResourceFileModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data model class for representing a resource file, used by the StandardInputModel class.
    /// </summary>
    public class ResourceFileModel
    {
        /// <summary>
        /// Gets or sets the ResourceIndex.
        /// </summary>
        [JsonProperty(PropertyName = "Resource Index")]
        public int? ResourceIndex { get; set; }

        /// <summary>
        /// Gets or sets the ResourceUrl.
        /// </summary>
        [JsonProperty(PropertyName = "Resource URL")]
        public string ResourceUrl { get; set; }
    }
}
