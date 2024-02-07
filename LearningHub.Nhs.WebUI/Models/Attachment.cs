// <copyright file="Attachment.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// Defines the <see cref="Attachment" />.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the FileSize.
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// Gets or sets the FileType.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets the IconUrl.
        /// </summary>
        public string IconUrl
        {
            get
            {
                switch (this.FileType)
                {
                    case "PDF":
                        return @"\images\fileicons\pdf.svg";
                    case "Image":
                        return @"\images\fileicons\jpeg.svg";
                    case "Word":
                        return @"\images\fileicons\ms-word-doc.svg";
                    default:
                        return @"\images\fileicons\jpeg.svg";
                }
            }
        }
    }
}
