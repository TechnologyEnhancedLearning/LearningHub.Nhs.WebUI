// <copyright file="QuestionAnswerBuilder.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests.Builders
{
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A Helper class to easily construct QuestionAnswer Objects.
    /// </summary>
    public class QuestionAnswerBuilder
    {
        /// <summary>
        /// The order of this question answer (derives the answer number).
        /// </summary>
        private int order;

        /// <summary>
        /// The status of the answer. Only one answer on a question may have status "Best".
        /// </summary>
        private QuestionAnswerStatus status;

        /// <summary>
        /// The block collection for this answer. The block collection must be checked to ensure it has a single block, and that block is not a question block.
        /// </summary>
        private BlockCollectionViewModel blockCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionAnswerBuilder"/> class.
        /// </summary>
        public QuestionAnswerBuilder()
        {
            this.order = 0;
            this.status = QuestionAnswerStatus.Reasonable;
            this.blockCollection = new BlockCollectionBuilder().Build();
        }

        /// <summary>
        /// Changes the order of the answer.
        /// </summary>
        /// <param name="order"> The new order of the answer.</param>
        /// <returns>The current instance of the question answer builder.</returns>
        public QuestionAnswerBuilder WithOrder(int order)
        {
            this.order = order;
            return this;
        }

        /// <summary>
        /// Changes the status of the answer.
        /// </summary>
        /// <param name="status"> The new status of the answer.</param>
        /// <returns>The current instance of the question answer builder.</returns>
        public QuestionAnswerBuilder WithStatus(QuestionAnswerStatus status)
        {
            this.status = status;
            return this;
        }

        /// <summary>
        /// Builds the question answer.
        /// </summary>
        /// <returns>The created question answer.</returns>
        public QuestionAnswerViewModel Build()
        {
            return new QuestionAnswerViewModel
            {
                Order = this.order,
                Status = this.status,
                BlockCollection = this.blockCollection,
            };
        }
    }
}