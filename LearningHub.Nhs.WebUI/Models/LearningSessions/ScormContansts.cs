// <copyright file="ScormContansts.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    /// <summary>
    /// Defines the <see cref="ScormContansts" />.
    /// </summary>
    public static class ScormContansts
    {
        /// <summary>
        /// The ScoreChildren.
        /// </summary>
        public const string ScoreChildren = "raw,min,max";

        /// <summary>
        /// The Entry.
        /// </summary>
        public const string Entry = "ab-initio";

        /// <summary>
        /// The Version.
        /// </summary>
        public const string Version = "3.4";

        /// <summary>
        /// The Children.
        /// </summary>
        public const string Children = "student_id,student_name,lesson_location,credit,lesson_status,entry,score,total_time,lesson_mode,exit,session_time";

        /// <summary>
        /// The LaunchData.
        /// </summary>
        public const string LaunchData = "";

        /// <summary>
        /// The LessonMode.
        /// </summary>
        public const string LessonMode = "normal"; // can also be either "browse" or "review"

        /// <summary>
        /// The StudentDataChildren.
        /// </summary>
        public const string StudentDataChildren = "mastery_score,max_time_allowed,time_limit_action";

        /// <summary>
        /// The StudentDataMasteryScore.
        /// </summary>
        public const decimal StudentDataMasteryScore = 100; ////TODO: Get from manifest file // contentObject.MasteryScore.ToString();

        /// <summary>
        /// The StudentDataMaxTimeAllowed.
        /// </summary>
        public const string StudentDataMaxTimeAllowed = ""; //// "00:00:00";       //  TODO: Get from manifest file

        /// <summary>
        /// The StudentDataTimeLimitAction.
        /// </summary>
        public const string StudentDataTimeLimitAction = "";

        /// <summary>
        /// The StudentPreferenceChildren.
        /// </summary>
        public const string StudentPreferenceChildren = "audio,language,speed,text";

        /// <summary>
        /// The StudentPreferenceAudio.
        /// </summary>
        public const string StudentPreferenceAudio = "";

        /// <summary>
        /// The StudentPreferenceAudio.
        /// </summary>
        public const string StudentPreferenceLanguage = "";

        /// <summary>
        /// The StudentPreferenceSpeed.
        /// </summary>
        public const string StudentPreferenceSpeed = "0";

        /// <summary>
        /// The StudentPreferenceText.
        /// </summary>
        public const string StudentPreferenceText = "0";

        /// <summary>
        /// The CommentsFromLMS.
        /// </summary>
        public const string CommentsFromLMS = "";

        /// <summary>
        /// The InteractionsChildren.
        /// </summary>
        public const string InteractionsChildren = "id,objectives,time,type,correct_responses,weighting,student_response,result,latancy";

        /// <summary>
        /// The ObjectivesChildren.
        /// </summary>
        public const string ObjectivesChildren = "id,score,status";

        /// <summary>
        /// The ObjectivesScoreChildren.
        /// </summary>
        public const string ObjectivesScoreChildren = "raw,min,max";
    }
}
