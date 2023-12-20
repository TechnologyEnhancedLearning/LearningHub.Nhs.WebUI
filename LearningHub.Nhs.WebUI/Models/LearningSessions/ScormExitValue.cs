// <copyright file="ScormExitValue.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="ScormExitValue" />.
    /// </summary>
    public sealed class ScormExitValue
    {
        /// <summary>
        /// The Timeout.
        /// </summary>
        public const string Timeout = "time-out";

        /// <summary>
        /// The Suspend.
        /// </summary>
        public const string Suspend = "suspend";

        /// <summary>
        /// The Logout.
        /// </summary>
        public const string Logout = "logout";

        /// <summary>
        /// The EmptyString.
        /// </summary>
        public const string EmptyString = "";

        private ScormExitValue()
        {
        }

        /// <summary>
        /// The CheckForValidExitValue.
        /// </summary>
        /// <param name="exitValue">exitValue.</param>
        /// <returns>bool.</returns>
        public static bool CheckForValidExitValue(string exitValue)
        {
            switch (exitValue)
            {
                case ScormExitValue.EmptyString:
                    return true;
                case ScormExitValue.Logout:
                    return true;
                case ScormExitValue.Suspend:
                    return true;
                case ScormExitValue.Timeout:
                    return true;
                default:
                    return false;
            }
        }
    }
}
