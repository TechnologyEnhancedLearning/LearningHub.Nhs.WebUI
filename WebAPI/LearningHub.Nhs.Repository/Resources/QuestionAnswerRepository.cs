// <copyright file="QuestionAnswerRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The question answer repository.
    /// </summary>
    public class QuestionAnswerRepository : GenericRepository<QuestionAnswer>, IQuestionAnswerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionAnswerRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public QuestionAnswerRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}