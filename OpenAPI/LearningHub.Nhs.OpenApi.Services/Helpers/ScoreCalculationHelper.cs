namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// Helper for the calculation of assessment scores on submission.
    /// </summary>
    public static class ScoreCalculationHelper
    {
        /// <summary>
        /// A method to calculate the score of the given answer.
        /// </summary>
        /// <param name="assessmentContent">The assessment content.</param>
        /// <param name="answers">The answers submitted.</param>
        /// <returns>The score corresponding to the submitted answers.</returns>
        public static (int? UserScore, int MaxScore) CalculateScore(BlockCollectionViewModel assessmentContent, Dictionary<int, IEnumerable<int>> answers)
        {
            var questionAnswersKeyValuePairs = answers.ToList();
            questionAnswersKeyValuePairs.Sort((x1, x2) => x1.Key.CompareTo(x2.Key));
            var answerOrders = questionAnswersKeyValuePairs.Select(x => x.Value.ToList()).ToList();

            return CalculateScore(
                assessmentContent.Blocks
                    .Where(x => x.BlockType == BlockType.Question)
                    .ToList(),
                answerOrders);
        }

        /// <summary>
        /// A method to calculate the score of the given answer.
        /// </summary>
        /// <param name="questionList">The list of blocks that are question blocks.</param>
        /// <param name="answerOrders">The answers submitted.</param>
        /// <returns>The score corresponding to the submitted answers.</returns>
        public static (int? UserScore, int MaxScore) CalculateScore(List<BlockViewModel> questionList, IEnumerable<IEnumerable<int>> answerOrders)
        {
            questionList.Sort((q1, q2) => q1.Order.CompareTo(q2.Order));
            var maxScore = 2 * questionList.Count;

            if (questionList.Count != answerOrders.Count())
            {
                return (
                    UserScore: null,
                    MaxScore: maxScore);
            }

            var results = questionList.Select((question, index) => GetScoreForQuestion(
                question,
                answerOrders.ToList()[index])).ToList();

            return (
                UserScore: results.Sum(),
                MaxScore: maxScore);
        }

        private static int GetScoreForQuestion(BlockViewModel question, IEnumerable<int> answerOrders)
        {
            var answerOrdersList = answerOrders.ToList();
            if (!AnswerSubmissionValidationHelper.IsAnswerSubmissionValid(
                    answerOrdersList.ToList(),
                    question.QuestionBlock))
            {
                throw new ValidationException("An answer submitted is invalid.");
            }

            return question.QuestionBlock.QuestionType switch
            {
                QuestionTypeEnum.SingleChoice => GetScoreForSingleChoiceQuestion(question, answerOrdersList),
                QuestionTypeEnum.MultipleChoice => GetScoreForMultipleChoiceQuestion(question, answerOrdersList),
                QuestionTypeEnum.ImageZone => GetScoreForImageZoneQuestion(question, answerOrdersList),
                _ => GetScoreForMatchQuestion(answerOrdersList)
            };
        }

        private static int GetScoreForMatchQuestion(List<int> answerOrders)
        {
            if (answerOrders != null)
            {
                var correctQuestionAnswers = answerOrders.Where((secondAnswerOrder, firstAnswerOrder) => secondAnswerOrder == firstAnswerOrder).Count();
                var incorrectQuestionAnswers = answerOrders.Count - correctQuestionAnswers;

                if (incorrectQuestionAnswers == 0)
                {
                    return 2;
                }

                if (correctQuestionAnswers >= incorrectQuestionAnswers)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static int GetScoreForSingleChoiceQuestion(BlockViewModel question, List<int> answerOrders)
        {
            if (answerOrders != null)
            {
                var questionAnswers = question.QuestionBlock.Answers.ToList();
                return questionAnswers.Find(answer => answer.Order == answerOrders[0]).Status switch
                {
                    QuestionAnswerStatus.Best => 2,
                    QuestionAnswerStatus.Reasonable => 1,
                    _ => 0
                };
            }

            return 0;
        }

        private static int GetScoreForMultipleChoiceQuestion(BlockViewModel question, List<int> answerOrders)
        {
            if (answerOrders != null)
            {
                var correctQuestionAnswers = question.QuestionBlock.Answers.Where(answer => answer.Status == QuestionAnswerStatus.Reasonable).ToList();
                if (correctQuestionAnswers.All(answer => answerOrders.Any(x => x == answer.Order)) &&
                    correctQuestionAnswers.Count == answerOrders.Count)
                {
                    return 2;
                }
            }

            return 0;
        }

        private static int GetScoreForImageZoneQuestion(BlockViewModel question, List<int> answerOrders)
        {
            if (answerOrders != null)
            {
                var questionAnswers = question.QuestionBlock.Answers.ToList();
                if (questionAnswers.Find(answer => answer.Order == answerOrders[0]).Status == QuestionAnswerStatus.Best)
                {
                    return 2;
                }
            }

            return 0;
        }
    }
}