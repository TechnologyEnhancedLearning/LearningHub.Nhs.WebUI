namespace LearningHub.Nhs.Services.UnitTests.Builders
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A Helper class to easily construct TextBlockObjects.
    /// </summary>
    public class QuestionBlockBuilder
    {
        /// <summary>
        /// The block collection containing the question. Should not contain any question blocks.
        /// </summary>
        private BlockCollectionViewModel questionBlockCollection;

        /// <summary>
        /// The block collection containing feedback to the question. Should not contain any question blocks.
        /// </summary>
        private BlockCollectionViewModel feedbackBlockCollection;

        /// <summary>
        /// The question type.
        /// </summary>
        private QuestionTypeEnum questionType;

        /// <summary>
        /// The list of answers for the question. Must have length between 2 and 5 inclusive.
        /// </summary>
        private List<QuestionAnswerViewModel> answers;

        /// <summary>
        /// A value indicating whether users can reveal the best answer before answering themselves.
        /// </summary>
        private bool allowReveal;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionBlockBuilder"/> class.
        /// </summary>
        public QuestionBlockBuilder()
        {
            this.questionBlockCollection = new BlockCollectionBuilder().Build();
            this.feedbackBlockCollection = new BlockCollectionBuilder().Build();
            this.questionType = QuestionTypeEnum.SingleChoice;
            this.allowReveal = true;
            this.answers = new List<QuestionAnswerViewModel>();
            this.answers.Add(new QuestionAnswerBuilder().Build());
            this.answers.Add(new QuestionAnswerBuilder().WithOrder(1).WithStatus(QuestionAnswerStatus.Best).Build());
            this.answers.Add(new QuestionAnswerBuilder().WithOrder(2).WithStatus(QuestionAnswerStatus.Incorrect).Build());
        }

        /// <summary>
        /// Changes the question type of the question block.
        /// </summary>
        /// <param name="questionType"> The questionType setting to be set.</param>
        /// <returns>The current instance of the question block builder.</returns>
        public QuestionBlockBuilder OfQuestionType(QuestionTypeEnum questionType)
        {
            this.questionType = questionType;
            return this;
        }

        /// <summary>
        /// Changes the feedback of the question block.
        /// </summary>
        /// <param name="feedbackBlockCollection"> The feedback which will replace the old one.</param>
        /// <returns>The current instance of the question block builder.</returns>
        public QuestionBlockBuilder WithFeedback(BlockCollectionViewModel feedbackBlockCollection)
        {
            this.feedbackBlockCollection = feedbackBlockCollection;
            return this;
        }

        /// <summary>
        /// Adds an answer to the question block.
        /// </summary>
        /// <param name="answer"> The answer which will be added.</param>
        /// <returns>The current instance of the question block builder.</returns>
        public QuestionBlockBuilder AddAnswer(QuestionAnswerViewModel answer)
        {
            this.answers.Add(answer);
            return this;
        }

        /// <summary>
        /// Resets the answer list.
        /// </summary>
        /// <returns>The current instance of the question block builder.</returns>
        public QuestionBlockBuilder ResetAnswers()
        {
            this.answers = new List<QuestionAnswerViewModel>();
            return this;
        }

        /// <summary>
        /// Builds the question block.
        /// </summary>
        /// <returns>The created question block.</returns>
        public QuestionBlockViewModel Build()
        {
            return new QuestionBlockViewModel
            {
                Answers = this.answers,
                AllowReveal = this.allowReveal,
                QuestionType = this.questionType,
                QuestionBlockCollection = this.questionBlockCollection,
                FeedbackBlockCollection = this.feedbackBlockCollection,
            };
        }
    }
}