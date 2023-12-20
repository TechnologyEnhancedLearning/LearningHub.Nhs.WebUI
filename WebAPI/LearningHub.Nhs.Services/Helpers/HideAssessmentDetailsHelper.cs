// <copyright file="HideAssessmentDetailsHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A class containing helper functions for hiding assessment details.
    /// </summary>
    public class HideAssessmentDetailsHelper
    {
        /// <summary>
        /// Returns a new Block Collection with all items after the given Question being with nullified content.
        /// </summary>
        /// <param name="blockCollection">The Block Collection.</param>
        /// <param name="questionNumber">The number of the question.</param>
        /// <param name="answerInOrder">Whether the questions should be answered in order.</param>
        /// <returns>The filtered BlockCollectionViewModel.</returns>
        public static BlockCollectionViewModel NullifyItemsAfterQuestion(BlockCollectionViewModel blockCollection, int questionNumber, bool answerInOrder)
        {
            List<BlockViewModel> questions = blockCollection.Blocks
                .Where(block => block.BlockType == BlockType.Question)
                .ToList();
            questions.Sort((q1, q2) => q1.Order.CompareTo(q2.Order));
            int questionOrder = questions[questionNumber].Order;

            return NullifyItemsFromBlock(blockCollection, questionOrder, answerInOrder);
        }

        /// <summary>
        /// Returns a new Block Collection with all items after the given block being with nullified content.
        /// </summary>
        /// <param name="blockCollection">The Block Collection.</param>
        /// <param name="blockOrder">The order of the block.</param>
        /// <param name="answerInOrder">Whether the questions should be answered in order.</param>
        /// <returns>The filtered BlockCollectionViewModel.</returns>
        public static BlockCollectionViewModel NullifyItemsFromBlock(BlockCollectionViewModel blockCollection, int blockOrder, bool answerInOrder)
        {
            List<BlockViewModel> filteredBlocks = blockCollection.Blocks.Select(block =>
            {
                if (block.Order > blockOrder && answerInOrder)
                {
                    return new BlockViewModel
                    {
                        BlockType = block.BlockType,
                        TextBlock = null,
                        MediaBlock = null,
                        QuestionBlock = null,
                        WholeSlideImageBlock = null,
                        Title = string.Empty,
                        Order = block.Order,
                    };
                }

                if (block.BlockType == BlockType.Question)
                {
                    block.QuestionBlock = new QuestionBlockViewModel
                    {
                        QuestionType = block.QuestionBlock.QuestionType,
                        AllowReveal = block.QuestionBlock.AllowReveal,
                        QuestionBlockCollection = block.QuestionBlock.QuestionBlockCollection,
                        FeedbackBlockCollection = null,
                        Answers = block.QuestionBlock.Answers.Select(HideAnswerDetails).ToList(),
                    };
                }

                return block;
            }).ToList();

            return new BlockCollectionViewModel()
            {
                Blocks = filteredBlocks,
            };
        }

        /// <summary>
        /// Maps the answer to an answer with a hidden type.
        /// </summary>
        /// <param name="answer">The Answer Model.</param>
        /// <returns>The QuestionAnswerViewModel with a status set to Incorrect.</returns>
        private static QuestionAnswerViewModel HideAnswerDetails(QuestionAnswerViewModel answer)
        {
            return new QuestionAnswerViewModel
            {
                BlockCollection = answer.BlockCollection,
                Order = answer.Order,
                Status = QuestionAnswerStatus.Best,
                Id = answer.Id,
                ImageAnnotationOrder = answer.ImageAnnotationOrder,
            };
        }
    }
}