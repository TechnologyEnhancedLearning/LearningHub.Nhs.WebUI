// <copyright file="AnswerSubmissionValidationHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// Helps with validating the submission of answers.
    /// </summary>
    public static class AnswerSubmissionValidationHelper
    {
        /// <summary>
        /// Validates questions in a given block collection, short-circuiting on the first invalid result.
        /// </summary>
        /// <param name="answerIndices">A list of the indices of the answers submitted.</param>
        /// <param name="question">The question for which the answers are submitted.</param>
        /// <returns>Returns true if the answer indices are valid and unique.</returns>
        public static bool IsAnswerSubmissionValid(List<int> answerIndices, QuestionBlockViewModel question)
        {
            var isValid = answerIndices.All(index => index >= 0 && index < question.Answers.Count)
                          && answerIndices.Distinct().Count() == answerIndices.Count;

            if (question.QuestionType is QuestionTypeEnum.SingleChoice or QuestionTypeEnum.ImageZone)
            {
                isValid = isValid && answerIndices.Count == 1;
            }

            return isValid;
        }
    }
}