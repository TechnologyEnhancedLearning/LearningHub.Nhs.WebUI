// <copyright file="EsrLinkValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Migration.Validation.Helpers;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// Checks whether an ESR link is valid:
    /// - Does it exist?
    /// - Is it used by any other resources in this migration?
    /// - Is it already used by a resource in LH or in another migration.
    /// </summary>
    public class EsrLinkValidator
    {
        private readonly UrlChecker urlChecker;
        private readonly IUrlRewritingRepository urlRewritingRepository;
        private readonly List<string> esrLinkUrls;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsrLinkValidator"/> class.
        /// </summary>
        /// <param name="urlChecker">The urlChecker.</param>
        /// <param name="urlRewritingRepository">The urlRewritingRepository.</param>
        public EsrLinkValidator(UrlChecker urlChecker, IUrlRewritingRepository urlRewritingRepository)
        {
            this.urlChecker = urlChecker;
            this.urlRewritingRepository = urlRewritingRepository;

            this.esrLinkUrls = new List<string>();
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="esrLinkUrl">The ESR Link Url.</param>
        /// <param name="requestTimeoutInSeconds">The timeout to use when sending requests to the URL.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string esrLinkUrl, int requestTimeoutInSeconds, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(esrLinkUrl))
            {
                if (!this.urlChecker.DoesUrlExist(esrLinkUrl, requestTimeoutInSeconds, out string error))
                {
                    result.AddError(modelPropertyName, $"{modelPropertyName} contains an invalid ESR link - \"{esrLinkUrl}\". Error: {error}");
                }

                if (this.urlRewritingRepository.Exists(esrLinkUrl))
                {
                    result.AddError(modelPropertyName, $"{modelPropertyName} contains a link that is already used by an existing Learning Hub resource. Link: \"{esrLinkUrl}\".");
                }

                if (this.esrLinkUrls.Contains(esrLinkUrl))
                {
                    result.AddError(modelPropertyName, $"{modelPropertyName} contains a link that is already used by another resource in this migration. Link: \"{esrLinkUrl}\".");
                }

                // Keep a note of all ESR link URLs used.
                this.esrLinkUrls.Add(esrLinkUrl);
            }
            else
            {
                result.AddWarning(modelPropertyName, $"SCORM resource does not have an LMS link specified.");
            }
        }
    }
}
