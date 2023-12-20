// <copyright file="QuestionValidationHelperTest.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Services.Helpers;
    using Xunit;

    /// <summary>
    /// The question validation helper test.
    /// </summary>
    public class QuestionValidationHelperTest
    {
        /// <summary>
        /// Validates a null return for valid input.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsNullForValidData()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    this.GenerateQuestionBlock_Valid(0),
                    this.GenerateQuestionBlock_Valid(1),
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Validates that the question validation helper short circuits on seeing an invalid question.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ShortCircuits()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    this.GenerateQuestionBlock_Valid(0),
                    this.GenerateQuestionBlock_InvalidOneAnswer(1),
                    this.GenerateQuestionBlock_InvalidOneAnswer(2),
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Single(result.Details);
        }

        /// <summary>
        /// Validates that the question block collection is not empty.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfQuestionBlockCollectionIsEmpty()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>(),
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Question content cannot be empty and may not contain any inner questions"));
        }

        /// <summary>
        /// Validates that the question block collection does not contain inner questions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfQuestionBlockCollectionContainsInnerQuestions()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    new BlockViewModel()
                                    {
                                        Title = "test title",
                                        Order = 1,
                                        BlockType = BlockType.Question,
                                        QuestionBlock = new QuestionBlockViewModel(),
                                    },
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Question content cannot be empty and may not contain any inner questions"));
        }

        /// <summary>
        /// Validates that the question text length is less than 250 characters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfQuestionTextLengthExceeds250Characters()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0, new string('a', 251)),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Question text must not exceed 250 characters in length"));
        }

        /// <summary>
        /// Validates that the feedback block collection is not empty.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfFeedbackBlockCollectionIsEmpty()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>(),
                            },
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Question feedback cannot be empty and may not contain any inner questions"));
        }

        /// <summary>
        /// Validates that the feedback block collection does not contain inner questions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfFeedbackBlockCollectionContainsInnerQuestions()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    new BlockViewModel()
                                    {
                                        Title = "test title",
                                        Order = 1,
                                        BlockType = BlockType.Question,
                                        QuestionBlock = new QuestionBlockViewModel(),
                                    },
                                },
                            },
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Question feedback cannot be empty and may not contain any inner questions"));
        }

        /// <summary>
        /// Validates that the feedback text length is less than 1000 characters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfFeedbackTextLengthExceeds3000Characters()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0, new string('a', 3001)),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Feedback text must not exceed 3000 characters in length"));
        }

        /// <summary>
        /// Validates that there are at least 2 answers.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfThereAreLessThan2Answers()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "There must be between 2 and 20 answers to a question"));
        }

        /// <summary>
        /// Validates that there are less than 20 answers.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfThereAreMoreThan20Answers()
        {
            // Arrange
            var answers = Enumerable.Range(1, 20)
                .Select(i => this.GenerateAnswer(i, QuestionAnswerStatus.Incorrect)).ToList();
            answers.Add(this.GenerateAnswer(0, QuestionAnswerStatus.Best));
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = answers,
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "There must be between 2 and 20 answers to a question"));
        }

        /// <summary>
        /// Validates that the answers are of the same type.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfTheAnswersAreNotOfTheSameType()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(1, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(0, QuestionAnswerStatus.Reasonable, new BlockViewModel()
                                {
                                    Title = "test title",
                                    Order = 1,
                                    BlockType = BlockType.Question,
                                    QuestionBlock = new QuestionBlockViewModel(),
                                }),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Answers must be of the same type and may not be questions"));
        }

        /// <summary>
        /// Validates that the answer order numbers are valid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_GivenInvalidAnswerOrderNumbers()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(3, QuestionAnswerStatus.Reasonable),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Answer order numbers must be unique in [0, 1, ..., n-1], where n is the number of answers."));
        }

        /// <summary>
        /// Validates that there is a single best answer for single choice questions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_GivenMoreThanOneBestAnswerForSingleChoiceQuestion()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Best),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "There must be exactly one Best answer for single choice questions"));
        }

        /// <summary>
        /// Validates that there are no best answers for multiple choice questions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_GivenBestAnswersForMultipleChoiceQuestion()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.MultipleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Reasonable),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "There must be no Best answers for multiple choice questions"));
        }

        /// <summary>
        /// Validates that the answers don't contain questions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfTheAnswersContainQuestions()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best, new BlockViewModel()
                                {
                                    Title = "test title",
                                    Order = 1,
                                    BlockType = BlockType.Question,
                                    QuestionBlock = new QuestionBlockViewModel(),
                                }),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Reasonable, new BlockViewModel()
                                {
                                    Title = "test title",
                                    Order = 1,
                                    BlockType = BlockType.Question,
                                    QuestionBlock = new QuestionBlockViewModel(),
                                }),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Answers must be of the same type and may not be questions"));
        }

        /// <summary>
        /// Validates that the answer text does not exceed 120 characters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task Validate_ReturnsFalse_IfAnswerTextLengthExceeds120Characters()
        {
            // Arrange
            var blockCollection = new BlockCollectionViewModel()
            {
                Blocks = new List<BlockViewModel>()
                {
                    new BlockViewModel()
                    {
                        Title = "Question 1",
                        Order = 0,
                        BlockType = BlockType.Question,
                        QuestionBlock = new QuestionBlockViewModel()
                        {
                            AllowReveal = false,
                            QuestionType = QuestionTypeEnum.SingleChoice,
                            QuestionBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            FeedbackBlockCollection = new BlockCollectionViewModel()
                            {
                                Blocks = new List<BlockViewModel>()
                                {
                                    this.GenerateTextBlock(0),
                                },
                            },
                            Answers = new List<QuestionAnswerViewModel>()
                            {
                                this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                                this.GenerateAnswer(1, QuestionAnswerStatus.Incorrect, this.GenerateTextBlock(0, new string('a', 121))),
                            },
                        },
                    },
                },
            };

            // Act
            var result = await QuestionValidationHelper.Validate(blockCollection);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.True(result.Details.All(details => details == "Text answers must not exceed 120 characters in length."));
        }

        private BlockViewModel GenerateTextBlock(int order, string content = "test content")
        {
            BlockViewModel textBlock = new BlockViewModel()
            {
                Title = "test title",
                Order = order,
                BlockType = BlockType.Text,
                TextBlock = new TextBlockViewModel()
                {
                    Content = content,
                },
            };
            return textBlock;
        }

        private QuestionAnswerViewModel GenerateAnswer(int order, QuestionAnswerStatus status)
        {
            return this.GenerateAnswer(order, status, this.GenerateTextBlock(0));
        }

        private QuestionAnswerViewModel GenerateAnswer(int order, QuestionAnswerStatus status, BlockViewModel block)
        {
            return new QuestionAnswerViewModel()
            {
                Order = order,
                Status = status,
                BlockCollection = new BlockCollectionViewModel()
                {
                    Blocks = new List<BlockViewModel>()
                    {
                        block,
                    },
                },
            };
        }

        private BlockViewModel GenerateQuestionBlock_Valid(int order)
        {
            return new BlockViewModel()
            {
                Title = "Question 1",
                Order = order,
                BlockType = BlockType.Question,
                QuestionBlock = new QuestionBlockViewModel()
                {
                    AllowReveal = false,
                    QuestionType = QuestionTypeEnum.SingleChoice,
                    QuestionBlockCollection = new BlockCollectionViewModel()
                    {
                        Blocks = new List<BlockViewModel>()
                        {
                            this.GenerateTextBlock(0),
                        },
                    },
                    FeedbackBlockCollection = new BlockCollectionViewModel()
                    {
                        Blocks = new List<BlockViewModel>()
                        {
                            this.GenerateTextBlock(0),
                        },
                    },
                    Answers = new List<QuestionAnswerViewModel>()
                    {
                        this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                        this.GenerateAnswer(1, QuestionAnswerStatus.Reasonable),
                        this.GenerateAnswer(2, QuestionAnswerStatus.Incorrect),
                    },
                },
            };
        }

        private BlockViewModel GenerateQuestionBlock_InvalidOneAnswer(int order)
        {
            return new BlockViewModel()
            {
                Title = "Question 1",
                Order = order,
                BlockType = BlockType.Question,
                QuestionBlock = new QuestionBlockViewModel()
                {
                    AllowReveal = false,
                    QuestionType = QuestionTypeEnum.SingleChoice,
                    QuestionBlockCollection = new BlockCollectionViewModel()
                    {
                        Blocks = new List<BlockViewModel>()
                        {
                            this.GenerateTextBlock(0),
                        },
                    },
                    FeedbackBlockCollection = new BlockCollectionViewModel()
                    {
                        Blocks = new List<BlockViewModel>()
                        {
                            this.GenerateTextBlock(0),
                        },
                    },
                    Answers = new List<QuestionAnswerViewModel>()
                    {
                        this.GenerateAnswer(0, QuestionAnswerStatus.Best),
                    },
                },
            };
        }
    }
}
