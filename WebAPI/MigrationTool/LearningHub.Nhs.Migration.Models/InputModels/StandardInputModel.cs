// <copyright file="StandardInputModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Data model class for representing the input record format used by the StandardInputRecordValidator.
    /// This is currently used by the eLR and eWIN migrations, but others may follow in the future.
    /// </summary>
    public class StandardInputModel
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Keywords.
        /// </summary>
        public string[] Keywords { get; set; }

        /// <summary>
        /// Gets or sets the Authors.
        /// </summary>
        public AuthorModel[] Authors { get; set; }

        /// <summary>
        /// Gets or sets the HasCost.
        /// </summary>
        public bool? HasCost { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the CreateUser.
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// Gets or sets the CreateDate.
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the ElfhUserId.
        /// </summary>
        [JsonProperty(PropertyName = "elfhuserid")]
        public string ElfhUserId { get; set; }

        /// <summary>
        /// Gets or sets the WebLinkUrl.
        /// </summary>
        [JsonProperty(PropertyName = "WebLink URL")]
        public string WebLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the ArticleBody.
        /// </summary>
        [JsonProperty(PropertyName = "Article Body")]
        public string ArticleBody { get; set; }

        /// <summary>
        /// Gets or sets the LicenceType.
        /// </summary>
        [JsonProperty(PropertyName = "Licence Type")]
        public string LicenceType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        [JsonProperty(PropertyName = "Resource Type")]
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the ResourceFiles.
        /// </summary>
        [JsonProperty(PropertyName = "Resource Files")]
        public ResourceFileModel[] ResourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the LmsLink.
        /// </summary>
        [JsonProperty(PropertyName = "LMS Link")]
        public string LmsLink { get; set; }
    }
}
