namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Services.Helpers;
    using LearningHub.Nhs.Services.UnitTests.Builders;
    using Xunit;

    /// <summary>
    /// Tests for the hide assessment details helper.
    /// </summary>
    public class HideAssessmentDetailsHelperTests
    {
        /// <summary>
        /// Tests whether the method NullifyItemsAfterQuestion correctly nullifies all blocks after a given question.
        /// </summary>
        [Fact]
        public void NullifyItemsAfterQuestion_CorrectlyNullifiesTheBlockCollection()
        {
            // Arrange
            BlockViewModel textBlock1 = new BlockBuilder().Build();
            BlockViewModel questionBlock1 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockViewModel pageBreak = new BlockBuilder().AsPageBreak().Build();
            BlockViewModel textBlock2 = new BlockBuilder().Build();
            BlockViewModel questionBlock2 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockViewModel textBlock3 = new BlockBuilder().Build();
            BlockViewModel questionBlock3 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockCollectionViewModel blockCollection =
                new BlockCollectionBuilder().RemoveAllBlocks()
                    .AddBlock(textBlock1)
                    .AddBlock(questionBlock1)
                    .AddBlock(pageBreak)
                    .AddBlock(textBlock2)
                    .AddBlock(questionBlock2)
                    .AddBlock(textBlock3)
                    .AddBlock(questionBlock3)
                    .Build();

            // Act
            BlockCollectionViewModel filteredBlockCollection =
                HideAssessmentDetailsHelper.NullifyItemsAfterQuestion(blockCollection, 1, true);
            BlockViewModel[] blocks = filteredBlockCollection.Blocks.ToArray();

            // Assert
            Assert.Contains(textBlock1, blocks);
            Assert.Contains(textBlock2, blocks);
            Assert.Contains(questionBlock1, blocks);
            Assert.Contains(questionBlock2, blocks);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Text && b.Order == textBlock3.Order && b.TextBlock == null);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Question && b.Order == questionBlock3.Order && b.QuestionBlock == null);
        }

        /// <summary>
        /// Tests whether the method NullifyItemsFromBlock correctly nullifies all blocks after a given block.
        /// </summary>
        [Fact]
        public void NullifyItemsFromBlock_CorrectlyNullifiesTheBlockCollection_NotAnswerInOrder()
        {
            // Arrange
            BlockViewModel textBlock1 = new BlockBuilder().Build();
            BlockViewModel questionBlock1 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockViewModel pageBreak = new BlockBuilder().AsPageBreak().Build();
            BlockViewModel textBlock2 = new BlockBuilder().Build();
            BlockViewModel questionBlock2 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockCollectionViewModel blockCollection =
                new BlockCollectionBuilder().RemoveAllBlocks()
                    .AddBlock(textBlock1)
                    .AddBlock(questionBlock1)
                    .AddBlock(pageBreak)
                    .AddBlock(textBlock2)
                    .AddBlock(questionBlock2)
                    .Build();

            // Act
            BlockCollectionViewModel filteredBlockCollection =
                HideAssessmentDetailsHelper.NullifyItemsFromBlock(blockCollection, 1, false);
            BlockViewModel[] blocks = filteredBlockCollection.Blocks.ToArray();

            // Assert
            Assert.Contains(textBlock1, blocks);
            Assert.Contains(questionBlock1, blocks);
            Assert.Contains(textBlock2, blocks);
            Assert.Contains(questionBlock2, blocks);
        }

        /// <summary>
        /// Tests whether the method NullifyItemsFromBlock correctly nullifies all blocks after a given block.
        /// </summary>
        [Fact]
        public void NullifyItemsFromBlock_CorrectlyNullifiesTheBlockCollection()
        {
            // Arrange
            BlockViewModel textBlock1 = new BlockBuilder().Build();
            BlockViewModel questionBlock1 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockViewModel pageBreak = new BlockBuilder().AsPageBreak().Build();
            BlockViewModel textBlock2 = new BlockBuilder().Build();
            BlockViewModel questionBlock2 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockCollectionViewModel blockCollection =
                new BlockCollectionBuilder().RemoveAllBlocks()
                    .AddBlock(textBlock1)
                    .AddBlock(questionBlock1)
                    .AddBlock(pageBreak)
                    .AddBlock(textBlock2)
                    .AddBlock(questionBlock2)
                    .Build();

            // Act
            BlockCollectionViewModel filteredBlockCollection =
                HideAssessmentDetailsHelper.NullifyItemsFromBlock(blockCollection, 1, true);
            BlockViewModel[] blocks = filteredBlockCollection.Blocks.ToArray();

            // Assert
            Assert.Contains(textBlock1, blocks);
            Assert.Contains(questionBlock1, blocks);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Text && b.Order == textBlock2.Order && b.TextBlock == null);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Question && b.Order == questionBlock2.Order && b.QuestionBlock == null);
        }

        /// <summary>
        /// Tests whether the method NullifyItemsFromBlock hides the answer details.
        /// </summary>
        [Fact]
        public void NullifyItemsFromBlock_HidesAnswerDetails()
        {
            // Arrange
            BlockViewModel textBlock1 = new BlockBuilder().Build();
            BlockViewModel questionBlock1 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().ResetAnswers()
                    .OfQuestionType(QuestionTypeEnum.MultipleChoice)
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Incorrect).WithOrder(2)
                        .Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable)
                        .WithOrder(1).Build())
                    .AddAnswer(new QuestionAnswerBuilder().WithStatus(QuestionAnswerStatus.Reasonable).WithOrder(0)
                        .Build())
                    .WithFeedback(new BlockCollectionBuilder().Build())
                    .Build()).Build();
            BlockViewModel pageBreak = new BlockBuilder().AsPageBreak().Build();
            BlockViewModel textBlock2 = new BlockBuilder().Build();
            BlockViewModel questionBlock2 =
                new BlockBuilder().WithQuestionBlock(new QuestionBlockBuilder().Build()).Build();
            BlockCollectionViewModel blockCollection =
                new BlockCollectionBuilder().RemoveAllBlocks()
                    .AddBlock(textBlock1)
                    .AddBlock(questionBlock1)
                    .AddBlock(pageBreak)
                    .AddBlock(textBlock2)
                    .AddBlock(questionBlock2)
                    .Build();

            // Act
            BlockCollectionViewModel filteredBlockCollection =
                HideAssessmentDetailsHelper.NullifyItemsFromBlock(blockCollection, 1, true);
            BlockViewModel[] blocks = filteredBlockCollection.Blocks.ToArray();

            // Assert
            Assert.Contains(textBlock1, blocks);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Question && b.QuestionBlock.Answers.All(a => a.Status == QuestionAnswerStatus.Best) && b.QuestionBlock.FeedbackBlockCollection == null);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Text && b.Order == textBlock2.Order && b.TextBlock == null);
            Assert.Contains(blocks, b => b.BlockType == BlockType.Question && b.Order == questionBlock2.Order && b.QuestionBlock == null);
        }
    }
}