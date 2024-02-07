// <copyright file="FileExtensionIsAllowedValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using System.Collections.Generic;
    using System.IO;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// Checks whether a particular resource file extension is allowed by the Learning Hub.
    /// </summary>
    public class FileExtensionIsAllowedValidator
    {
        private readonly List<string> disallowedFileExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionIsAllowedValidator"/> class.
        /// </summary>
        /// <param name="fileTypeService"> The file type service.</param>
        public FileExtensionIsAllowedValidator(IFileTypeService fileTypeService)
        {
            this.disallowedFileExtensions = fileTypeService.GetAllDisallowedFileExtensions();
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string filename, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                // Check that the file type is allowed by LH.
                var fileExtension = Path.GetExtension(filename);
                if (fileExtension.StartsWith("."))
                {
                    fileExtension = fileExtension.Substring(1);
                }

                if (this.disallowedFileExtensions.Contains(fileExtension))
                {
                    result.AddError(modelPropertyName, $"The file extension of resource file '{filename}' is not allowed by the Learning Hub.");
                }
            }
        }
    }
}
