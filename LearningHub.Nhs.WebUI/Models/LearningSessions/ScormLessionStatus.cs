// <copyright file="ScormLessionStatus.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="ScormLessionStatus" />.
    /// </summary>
    public sealed class ScormLessionStatus
    {
        /// <summary>
        /// The Passed.
        /// </summary>
        public const string Passed = "passed";

        /// <summary>
        /// The Failed.
        /// </summary>
        public const string Failed = "failed";

        /// <summary>
        /// The Completed.
        /// </summary>
        public const string Completed = "completed";

        /// <summary>
        /// The Incomplete.
        /// </summary>
        public const string Incomplete = "incomplete";

        /// <summary>
        /// The Browsed.
        /// </summary>
        public const string Browsed = "browsed";

        /// <summary>
        /// The NotAttempted.
        /// </summary>
        public const string NotAttempted = "not attempted";

        private ScormLessionStatus()
        {
        }

        /// <summary>
        /// The CheckForValidStatus.
        /// </summary>
        /// <param name="lessonStatus">lessonStatus.</param>
        /// <returns>bool.</returns>
        public static bool CheckForValidStatus(string lessonStatus)
        {
            switch (lessonStatus)
            {
                case ScormLessionStatus.Browsed:
                    return true;
                case ScormLessionStatus.Completed:
                    return true;
                case ScormLessionStatus.Failed:
                    return true;
                case ScormLessionStatus.Incomplete:
                    return true;
                case ScormLessionStatus.NotAttempted:
                    return true;
                case ScormLessionStatus.Passed:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The ActivityStatusId.
        /// </summary>
        /// <param name="lessonStatus">lessonStatus.</param>
        /// <returns>bool.</returns>
        public static int ActivityStatusId(string lessonStatus)
        {
            switch (lessonStatus)
            {
                case ScormLessionStatus.NotAttempted:
                    return 1;
                case ScormLessionStatus.Incomplete:
                    return 2;
                case ScormLessionStatus.Completed:
                    return 3;
                case ScormLessionStatus.Failed:
                    return 4;
                case ScormLessionStatus.Passed:
                    return 5;
                case ScormLessionStatus.Browsed:
                    return 6;
                default:
                    return 0;
            }
        }
    }
}
