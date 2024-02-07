// <copyright file="CatalogueNameExistsValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// This class checks whether a catalogue name exists in the Learning Hub.
    /// </summary>
    public class CatalogueNameExistsValidator
    {
        private readonly ICatalogueNodeVersionRepository catalogueNodeVersionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueNameExistsValidator"/> class.
        /// </summary>
        /// <param name="catalogueNodeVersionRepository">The catalogueNodeVersionRepository.</param>
        public CatalogueNameExistsValidator(ICatalogueNodeVersionRepository catalogueNodeVersionRepository)
        {
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string catalogueName, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(catalogueName) && !this.catalogueNodeVersionRepository.ExistsAsync(catalogueName).Result)
            {
                result.AddError(modelPropertyName, $"Catalogue '{catalogueName}' not found in Learning Hub database.");
            }
        }
    }
}
