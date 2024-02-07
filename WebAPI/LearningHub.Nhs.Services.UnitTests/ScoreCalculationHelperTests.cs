// <copyright file="ScoreCalculationHelperTests.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Services.Helpers;
    using LearningHub.Nhs.Services.UnitTests.Builders;
    using Microsoft.Rest;
    using Xunit;

    /// <summary>
    /// Tests for the score calculation helper.
    /// </summary>
    public class ScoreCalculationHelperTests
    {
        /// <summary>
        /// Test whether the method CalculateScore correctly calculates the max score.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_ExpectedMaxScore()
        {
            // Arrange
            var questions = new List<BlockViewModel>
            {
                new BlockViewModel(),
                new BlockViewModel(),
                new BlockViewModel(),
            };
            var answers = new List<List<int>>();
            var expectedMaxScore = questions.Count * 2;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedMaxScore, score.MaxScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns null given not all the questions are answered.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_Null_IfNotAllTheQuestionsAreAnswered()
        {
            // Arrange
            var questions = new List<BlockViewModel>
            {
                new BlockViewModel(),
                new BlockViewModel(),
                new BlockViewModel(),
            };
            var answers = new List<List<int>>();
            int? expectedScore = null;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a SingleChoiceQuestion best answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_2_ForASingleChoiceQuestionBestAnswer()
        {
            // Arrange
            var bestAnswerOrder = 2;
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.SingleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(bestAnswerOrder).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { bestAnswerOrder } };
            int expectedScore = 2;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a SingleChoiceQuestion reasonable answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_1_ForASingleChoiceQuestionReasonableAnswer()
        {
            // Arrange
            var reasonableAnswerOrder = 0;
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.SingleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(2).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(reasonableAnswerOrder).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { reasonableAnswerOrder } };
            int expectedScore = 1;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a SingleChoiceQuestion incorrect answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_0_ForASingleChoiceQuestionIncorrectAnswer()
        {
            // Arrange
            var incorrectAnswerOrder = 1;
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.SingleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(2).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(incorrectAnswerOrder).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { incorrectAnswerOrder } };
            int expectedScore = 0;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MultipleChoiceQuestion correct answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_2_ForAMultipleChoiceQuestionCorrectAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 1, 0 } };
            int expectedScore = 2;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MultipleChoiceQuestion incomplete answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_0_ForAMultipleChoiceQuestionIncompleteAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 1 } };
            int expectedScore = 0;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MultipleChoiceQuestion incorrect answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_0_ForAMultipleChoiceQuestionIncorrectAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 1 } };
            int expectedScore = 0;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for an ImageZoneQuestion correct answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_2_ForAnImageZoneQuestionCorrectAnswer()
        {
            // Arrange
            var bestAnswerOrder = 0;
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.ImageZone)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(bestAnswerOrder).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { bestAnswerOrder } };
            int expectedScore = 2;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for an ImageZoneQuestion incorrect answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_0_ForAnImageZoneQuestionIncorrectAnswer()
        {
            // Arrange
            var incorrectAnswerOrder = 0;
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.ImageZone)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(incorrectAnswerOrder).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { incorrectAnswerOrder } };
            int expectedScore = 0;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MatchGameQuestion correct answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_2_ForAMatchGameQuestionCorrectAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.MatchGame)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(0).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(2).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(3).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 0, 1, 2, 3 } };
            int expectedScore = 2;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MatchGameQuestion answer that is half correct.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_1_ForAMatchGameQuestionHalfCorrectAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.MatchGame)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(0).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(2).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(3).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 0, 1, 3, 2 } };
            int expectedScore = 1;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score for a MatchGameQuestion incorrect answer.
        /// </summary>
        [Fact]
        public void CalculateScore_Returns_0_ForAMatchGameQuestionIncorrectAnswer()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.MatchGame)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(0).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(2).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(3).Build())
                    .Build())
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 1, 2, 3, 0 } };
            int expectedScore = 0;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore returns the expected score given mixed question types.
        /// </summary>
        [Fact]
        public void CalculateScore_CalculatesScoreCorrectly_ForMixedQuestionTypes()
        {
            // Arrange
            var question1 = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .Build())
                .WithOrder(0)
                .Build();
            var question2 = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.SingleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(0).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(2).Build())
                    .Build())
                .WithOrder(1)
                .Build();
            var questions = new List<BlockViewModel>
            {
                question1,
                question2,
            };
            int[][] answers = { new[] { 0 }, new[] { 0 } };
            int expectedScore = 4;

            // Act
            var score = ScoreCalculationHelper.CalculateScore(questions, answers);

            // Assert
            Assert.Equal(expectedScore, score.UserScore);
        }

        /// <summary>
        /// Test whether the method CalculateScore checks the submitted answer is valid.
        /// </summary>
        [Fact]
        public void CalculateScore_InvalidSubmittedAnswerError()
        {
            // Arrange
            var question = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers().OfQuestionType(QuestionTypeEnum.SingleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Best).WithOrder(0).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(2).Build())
                    .Build())
                .WithOrder(0)
                .Build();
            var questions = new List<BlockViewModel> { question };
            int[][] answers = { new[] { 0, 1 } };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ScoreCalculationHelper.CalculateScore(questions, answers));
            Assert.Equal("An answer submitted is invalid.", exception.Message);
        }

        /// <summary>
        /// Tests whether the method CalculateScore correctly calculates the score for an assessment.
        /// </summary>
        [Fact]
        public void CalculateScore_CorrectlyCalculatesScore_GivenAnAssessmentBlockCollection()
        {
            // Arrange
            var questionBlock1 = new BlockBuilder()
                .WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .Build())
                .Build();
            BlockViewModel questionBlock2 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder()
                        .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                        .ResetAnswers()
                        .AddAnswer(new QuestionAnswerBuilder().WithOrder(1).WithStatus(QuestionAnswerStatus.Incorrect).Build())
                        .AddAnswer(new QuestionAnswerBuilder().WithOrder(0).Build())
                        .Build())
                    .Build();
            BlockViewModel textBlock =
                new BlockBuilder().WithTextBlock(new TextBlockBuilder().Build())
                    .Build();
            BlockCollectionViewModel assessment =
                new BlockCollectionBuilder().RemoveAllBlocks()
                    .AddBlock(questionBlock1)
                    .AddBlock(questionBlock2)
                    .AddBlock(textBlock)
                    .Build();
            int[][] answerOrders = { new int[] { 0, 1 }, new int[] { 1 } };
            var answers = new Dictionary<int, IEnumerable<int>>()
            {
                { 0, answerOrders[0] },
                { 1, answerOrders[1] },
            };

            // Act
            var score = ScoreCalculationHelper.CalculateScore(assessment, answers);

            // Assert
            Assert.Equal(4, score.MaxScore);
            Assert.Equal(2, score.UserScore);
        }
    }
}