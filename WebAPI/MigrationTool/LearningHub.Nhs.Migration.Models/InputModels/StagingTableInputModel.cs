// <copyright file="StagingTableInputModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// Data model class for representing the input data format used by the staging tables migration method.
    /// Originally written for the South Central Content Server migration.
    /// </summary>
    public class StagingTableInputModel
    {
        /// <summary>
        /// Gets or sets the Id. This is here just to keep EntityFramework happy. The SP returns a unique Id, starting at 1.
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ResourceUniqueRef.
        /// </summary>
        public string ResourceUniqueRef { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to record the user id against the first author.
        /// </summary>
        public string IAmTheAuthorFlag { get; set; }

        /// <summary>
        /// Gets or sets the ResourceTitle.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource contains sensitive content.
        /// </summary>
        public string SensitiveContentFlag { get; set; }

        /// <summary>
        /// Gets or sets the Keywords.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueName.
        /// </summary>
        public string CatalogueName { get; set; }

        /// <summary>
        /// Gets or sets the Licence.
        /// </summary>
        public string Licence { get; set; }

        /// <summary>
        /// Gets or sets the PublishedDate.
        /// </summary>
        public DateTime? PublishedDate { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalInformation.
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the AuthorName1.
        /// </summary>
        public string AuthorName1 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorRole1.
        /// </summary>
        public string AuthorRole1 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorOrganisation1.
        /// </summary>
        public string AuthorOrganisation1 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorName2.
        /// </summary>
        public string AuthorName2 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorRole2.
        /// </summary>
        public string AuthorRole2 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorOrganisation2.
        /// </summary>
        public string AuthorOrganisation2 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorName3.
        /// </summary>
        public string AuthorName3 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorRole3.
        /// </summary>
        public string AuthorRole3 { get; set; }

        /// <summary>
        /// Gets or sets the AuthorOrganisation3.
        /// </summary>
        public string AuthorOrganisation3 { get; set; }

        /* Article Resource Type */

        /// <summary>
        /// Gets or sets the ArticleContentFilename. This points to a file in the migration's staging blob container in Azure.
        /// </summary>
        public string ArticleContentFilename { get; set; }

        /// <summary>
        /// Gets or sets the ArticleBodyText. This is the contents of the file referred to by the ArticleBodyFilename property.
        /// This text is read from the file during the migration creation stage.
        /// NotMapped because this value does not come back from the stored procedure that returns the data. It is read from the
        /// article body file in azure.
        /// </summary>
        [NotMapped]
        public string ArticleBodyText { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile1.
        /// </summary>
        public string ArticleFile1 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile2.
        /// </summary>
        public string ArticleFile2 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile3.
        /// </summary>
        public string ArticleFile3 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile4.
        /// </summary>
        public string ArticleFile4 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile5.
        /// </summary>
        public string ArticleFile5 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile6.
        /// </summary>
        public string ArticleFile6 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile7.
        /// </summary>
        public string ArticleFile7 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile8.
        /// </summary>
        public string ArticleFile8 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile9.
        /// </summary>
        public string ArticleFile9 { get; set; }

        /// <summary>
        /// Gets or sets the ArticleFile10.
        /// </summary>
        public string ArticleFile10 { get; set; }

        /* File Resource Type */

        /// <summary>
        /// Gets or sets the ServerFileName.
        /// </summary>
        public string ServerFileName { get; set; }

        /// <summary>
        /// Gets or sets the YearAuthored.
        /// </summary>
        public int? YearAuthored { get; set; }

        /// <summary>
        /// Gets or sets the MonthAuthored.
        /// </summary>
        public int? MonthAuthored { get; set; }

        /// <summary>
        /// Gets or sets the DayAuthored.
        /// </summary>
        public int? DayAuthored { get; set; }

        /* SCORM Resource Type */

        /// <summary>
        /// Gets or sets the LMSLink.
        /// </summary>
        public string LMSLink { get; set; }

        /// <summary>
        /// Gets or sets the ESRLinkType.
        /// </summary>
        public string LMSLinkVisibility { get; set; }

        /* Weblink Resource Type */

        /// <summary>
        /// Gets or sets the WeblinkUrl.
        /// </summary>
        public string WeblinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the WeblinkText.
        /// </summary>
        public string WeblinkText { get; set; }

        /* Contributor */

        /// <summary>
        /// Gets or sets the ContributorOrgName.
        /// </summary>
        public string ContributorOrgName { get; set; }

        /// <summary>
        /// Gets or sets the ContributorDate.
        /// </summary>
        public string ContributorDate { get; set; }

        /// <summary>
        /// Gets or sets the ContributorLearningHubUserName.
        /// </summary>
        public string ContributorLearningHubUserName { get; set; }

        /// <summary>
        /// Gets or sets the ContributorFirstName.
        /// </summary>
        public string ContributorFirstName { get; set; }

        /// <summary>
        /// Gets or sets the ContributorLastName.
        /// </summary>
        public string ContributorLastName { get; set; }

        /// <summary>
        /// Gets or sets the ContributorJobRole.
        /// </summary>
        public string ContributorJobRole { get; set; }

        /// <summary>
        /// Gets or sets the ContributorProfessionalBody.
        /// </summary>
        public string ContributorProfessionalBody { get; set; }

        /// <summary>
        /// Gets or sets the ContributorRegistrationNumber.
        /// </summary>
        public string ContributorRegistrationNumber { get; set; }
    }
}