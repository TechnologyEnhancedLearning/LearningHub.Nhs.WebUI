// <copyright file="AzureBlobFileValidator.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// Checks whether a resource filename exists in the specified Azure blob container, and also checks that the file size is
    /// within the supplied limit. Operations done together to avoid calling azureBlobService.GetBlobMetadata twice for each.
    /// </summary>
    public class AzureBlobFileValidator
    {
        private readonly IAzureBlobService azureBlobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobFileValidator"/> class.
        /// </summary>
        /// <param name="azureBlobService">The azure blob service.</param>
        public AzureBlobFileValidator(IAzureBlobService azureBlobService)
        {
            this.azureBlobService = azureBlobService;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="azureBlobContainerConnectionString">The connection string to the Azure storage account hosting the blob container.</param>
        /// <param name="azureBlobContainerName">The name of the Azure blob container to search within.</param>
        /// <param name="blobFilename">The filename to search for.</param>
        /// <param name="fileSizeLimitInBytes">The maximum allowed file size. </param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Validate(string azureBlobContainerConnectionString, string azureBlobContainerName, string blobFilename, long fileSizeLimitInBytes, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(blobFilename))
            {
                // Check that the file exists in the migration blob container and the size is within limit.
                string blobName = blobFilename.StartsWith(@"public://") ? blobFilename.Substring(9) : blobFilename;
                try
                {
                    var blobMetadata = await this.azureBlobService.GetBlobMetadata(azureBlobContainerConnectionString, azureBlobContainerName, blobName);

                    if (blobMetadata == null)
                    {
                        result.AddError(modelPropertyName, $"Resource File '{blobName}' does not exist in the Azure migration blob container.");
                    }
                    else if (blobMetadata.SizeInBytes > fileSizeLimitInBytes)
                    {
                        result.AddError(modelPropertyName, $"Resource File '{blobName}' is too large. Size limit is {fileSizeLimitInBytes} bytes. Size of file is {blobMetadata.SizeInBytes} bytes.");
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(modelPropertyName, $"An error occurred when retrieving the details of resource file '{blobName}' from the Azure migration blob container. Details: {Environment.NewLine}{ex.ToString()}");
                }
            }
        }
    }
}
