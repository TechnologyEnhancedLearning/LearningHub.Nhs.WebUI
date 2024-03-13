namespace LearningHub.Nhs.Migration.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the parameters required to create a resource via the migration tool. The different implementations of IInputRecordMapper all return an object of this type.
    /// </summary>
    /// <remarks>
    /// All input models, each of which could be specific to a particular migration source, will be mapped to this model class. This allows the implementation of the draft
    /// resource creation and publish stages to be the same for all migration sources. Currently there is only one input model (StandardInputModel.cs), which is used for the
    /// eLR and eWIN migrations, but different input models may be required in the future.
    /// </remarks>
    public class ResourceParamsModel
    {
        /// <summary>
        /// Gets or sets the MigrationInputRecordId.
        /// </summary>
        public int MigrationInputRecordId { get; set; }

        /// <summary>
        /// Gets or sets the DestinationNodeId.
        /// </summary>
        public int DestinationNodeId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceTypeId.
        /// </summary>
        public int ResourceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ResourceLicenceId.
        /// </summary>
        public int ResourceLicenceId { get; set; }

        /// <summary>
        /// Gets or sets the CreateDate.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SensitiveContentFlag is required.
        /// </summary>
        public bool SensitiveContentFlag { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalInformation.
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the YearAuthored.
        /// </summary>
        public int YearAuthored { get; set; }

        /// <summary>
        /// Gets or sets the MonthAuthored.
        /// </summary>
        public int MonthAuthored { get; set; }

        /// <summary>
        /// Gets or sets the DayAuthored.
        /// </summary>
        public int DayAuthored { get; set; }

        /// <summary>
        /// Gets or sets the ArticleBody.
        /// </summary>
        public string ArticleBody { get; set; }

        /// <summary>
        /// Gets or sets the WebLinkUrl.
        /// </summary>
        public string WebLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the WebLinkDisplayText.
        /// </summary>
        public string WebLinkDisplayText { get; set; }

        /// <summary>
        /// Gets or sets the EsrLinkTypeId.
        /// </summary>
        public int EsrLinkTypeId { get; set; }

        /// <summary>
        /// Gets or sets the ResourceFileUrls.
        /// </summary>
        public List<string> ResourceFileUrls { get; set; }

        /// <summary>
        /// Gets or sets the Authors.
        /// </summary>
        public List<AuthorParamsModel> Authors { get; set; }

        /// <summary>
        /// Gets or sets the Keywords.
        /// </summary>
        public List<string> Keywords { get; set; }
    }
}
