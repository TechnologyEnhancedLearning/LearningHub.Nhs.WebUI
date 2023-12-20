// <copyright file="QuestionValidationHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Helpers
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Helps with validating questions within block collections.
    /// </summary>
    public static class QuestionValidationHelper
    {
        /// <summary>
        /// Validates questions in a given block collection, short-circuiting on the first invalid result.
        /// </summary>
        /// <param name="blockCollection">The block collection to validate.</param>
        /// <returns>Returns null if no validation issues have been found.</returns>
        public static async Task<LearningHubValidationResult> Validate(BlockCollectionViewModel blockCollection)
        {
            var questionValidator = new QuestionBlockValidator();
            var questionBlocks = blockCollection.Blocks.Where(block => block.BlockType == BlockType.Question);

            LearningHubValidationResult validation;
            foreach (var block in questionBlocks)
            {
                validation = new LearningHubValidationResult(await questionValidator.ValidateAsync(block.QuestionBlock));
                if (!validation.IsValid)
                {
                    return validation;
                }
            }

            return new LearningHubValidationResult(true);
        }
    }
}
