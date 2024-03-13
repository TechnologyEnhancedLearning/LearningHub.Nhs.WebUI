namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Services.Helpers;
    using Xunit;

    /// <summary>
    /// Tests for the AnswerSubmissionValidationHelper.
    /// </summary>
    public class AnswerSubmissionValidationHelperTests
    {
        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates a negative answer index.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsFalse_GivenANegativeAnswerIndex()
        {
            // Arrange
            var indices = new List<int>() { -1 };
            var question = new QuestionBlockViewModel();

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates an invalid answer index.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsFalse_GivenAnInvalidAnswerIndex()
        {
            // Arrange
            var indices = new List<int>() { 2 };
            var question = new QuestionBlockViewModel
            {
                Answers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel(),
                    new QuestionAnswerViewModel(),
                },
            };

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates duplicated answer indices.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsFalse_GivenDuplicatedAnswerIndices()
        {
            // Arrange
            var indices = new List<int>() { 0, 0 };
            var question = new QuestionBlockViewModel
            {
                Answers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel(),
                    new QuestionAnswerViewModel(),
                },
            };

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates the answer for single choice question.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsFalse_GivenMoreThanAnAnswerForSingleChoiceQuestion()
        {
            // Arrange
            var indices = new List<int>() { 0, 1 };
            var question = new QuestionBlockViewModel
            {
                QuestionType = QuestionTypeEnum.SingleChoice,
                Answers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel(),
                    new QuestionAnswerViewModel(),
                },
            };

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates the answer for single choice question.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsTrue_GivenAValidAnswerForSingleChoiceQuestion()
        {
            // Arrange
            var indices = new List<int>() { 0 };
            var question = new QuestionBlockViewModel
            {
                QuestionType = QuestionTypeEnum.SingleChoice,
                Answers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel(),
                    new QuestionAnswerViewModel(),
                },
            };

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests whether the method IsAnswerSubmissionValid correctly validates the answer for multiple choice question.
        /// </summary>
        [Fact]
        public void IsAnswerSubmissionValid_ReturnsTrue_GivenAValidAnswerForMultipleChoiceQuestion()
        {
            // Arrange
            var indices = new List<int>() { 0, 1 };
            var question = new QuestionBlockViewModel
            {
                QuestionType = QuestionTypeEnum.MultipleChoice,
                Answers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel(),
                    new QuestionAnswerViewModel(),
                },
            };

            // Act
            var result = AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(indices, question);

            // Assert
            Assert.True(result);
        }
    }
}