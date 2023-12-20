// <copyright file="FileTypeLookup.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Mapping.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// Helper class for looking up LH file types.
    /// </summary>
    public class FileTypeLookup
    {
        private readonly IFileTypeService fileTypeService;
        private Dictionary<string, FileType> fileTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeLookup"/> class.
        /// </summary>
        /// <param name="fileTypeService">The file type service.</param>
        public FileTypeLookup(IFileTypeService fileTypeService)
        {
            this.fileTypeService = fileTypeService;

            this.fileTypes = new Dictionary<string, FileType>();
        }

        /// <summary>
        /// Gets the LH file type for a particular file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The file type.</returns>
        public FileType GetFileTypeByFileName(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            if (fileExtension.StartsWith("."))
            {
                fileExtension = fileExtension.Substring(1);
            }

            if (this.fileTypes.ContainsKey(fileExtension))
            {
                return this.fileTypes[fileExtension];
            }
            else
            {
                var fileType = this.fileTypeService.GetByFilename(fileName).Result;
                this.fileTypes.Add(fileExtension, fileType);
                return fileType;
            }
        }
    }
}
