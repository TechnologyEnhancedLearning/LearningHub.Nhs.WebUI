// <copyright file="TrayCardExtended.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TrayCardExtended" />.
    /// </summary>
    public class TrayCardExtended : TrayCard
    {
        private List<string> authorlist = new List<string>();

        /// <summary>
        /// Gets or sets the AdditionalInformation.
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the AuthoredDate.
        /// </summary>
        public DateTimeOffset AuthoredDate { get; set; }

        /// <summary>
        /// Gets or sets the AuthorList.
        /// </summary>
        public List<string> AuthorList
        {
            get
            {
                return this.authorlist;
            }

            set
            {
                this.authorlist = value;
            }
        }

        /// <summary>
        /// Gets or sets the Cost.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the GenericFile.
        /// </summary>
        public Attachment GenericFile { get; set; }

        /// <summary>
        /// Gets or sets the LastVersionUpdateDate.
        /// </summary>
        public DateTimeOffset LastVersionUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the PublishedDate.
        /// </summary>
        public DateTimeOffset PublishedDate { get; set; }

        /// <summary>
        /// Gets or sets the ResourceFileName.
        /// </summary>
        public string ResourceFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ResourceFree.
        /// </summary>
        public bool ResourceFree { get; set; }

        /// <summary>
        /// Gets or sets the ResourcePath.
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Gets a value indicating whether ShowActionPanel.
        /// </summary>
        public bool ShowActionPanel => false;

        /// <summary>
        /// Gets a value indicating whether ShowFlagResourceLink.
        /// </summary>
        public bool ShowFlagResourceLink => false;

        /// <summary>
        /// Gets or sets the SourcedBy.
        /// </summary>
        public string SourcedBy { get; set; }

        /// <summary>
        /// Gets or sets the SourcedByBadge.
        /// </summary>
        public string SourcedByBadge { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the WebLinkDisplayText.
        /// </summary>
        public string WebLinkDisplayText { get; set; }

        /// <summary>
        /// Gets or sets the WebLinkUrl.
        /// </summary>
        public string WebLinkUrl { get; set; }
    }
}
