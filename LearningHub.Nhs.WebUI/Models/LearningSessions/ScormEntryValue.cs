// <copyright file="ScormEntryValue.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="ScormEntryValue" />.
    /// </summary>
    public sealed class ScormEntryValue
    {
        /// <summary>
        /// The AbInitio.
        /// </summary>
        public const string AbInitio = "ab-initio";

        /// <summary>
        /// The Resume.
        /// </summary>
        public const string Resume = "resume";

        /// <summary>
        /// The EmptyString.
        /// </summary>
        public const string EmptyString = "";

        private ScormEntryValue()
        {
        }

        /// <summary>
        /// The CheckForValidEntryValue.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>bool.</returns>
        public static bool CheckForValidEntryValue(string value)
        {
            switch (value)
            {
                case ScormEntryValue.AbInitio:
                    return true;
                case ScormEntryValue.EmptyString:
                    return true;
                case ScormEntryValue.Resume:
                    return true;
                default:
                    return false;
            }
        }
    }
}
