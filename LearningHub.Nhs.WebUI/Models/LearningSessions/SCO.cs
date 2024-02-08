namespace LearningHub.Nhs.WebUI.Models.LearningSessions
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Resource.Activity;

    /// <summary>
    /// Defines the <see cref="SCO" />.
    /// </summary>
    public class SCO : ScormActivityViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SCO"/> class.
        /// </summary>
        public SCO()
        {
            this.ScormActivityInteraction = new List<ScormActivityInteractionViewModel>();
            this.ScormActivityObjective = new List<ScormActivityObjectiveViewModel>();
        }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        public string Children { get; set; }

        /// <summary>
        /// Gets or sets the StudentId.
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// Gets or sets the StudentName.
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// Gets or sets the LessonStatus.
        /// </summary>
        public string LessonStatus { get; set; }

        /// <summary>
        /// Gets or sets the Credit.
        /// </summary>
        public string Credit { get; set; }

        /// <summary>
        /// Gets or sets the Entry.
        /// </summary>
        public string Entry { get; set; }

        /// <summary>
        /// Gets or sets the LaunchData.
        /// </summary>
        public string LaunchData { get; set; }

        /// <summary>
        /// Gets or sets the LessonMode.
        /// </summary>
        public string LessonMode { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the CommentsFromLms.
        /// </summary>
        public string CommentsFromLms { get; set; }

        /// <summary>
        /// Gets or sets the ScoreChildren.
        /// </summary>
        public string ScoreChildren { get; set; }

        /// <summary>
        /// Gets or sets the StudentDataChildren.
        /// </summary>
        public string StudentDataChildren { get; set; }

        /// <summary>
        /// Gets or sets the StudentDataMasteryScore.
        /// </summary>
        public decimal? StudentDataMasteryScore { get; set; }

        /// <summary>
        /// Gets or sets the StudentDataMaxTimeAllowed.
        /// </summary>
        public string StudentDataMaxTimeAllowed { get; set; }

        /// <summary>
        /// Gets or sets the StudentDataTimeLimitAction.
        /// </summary>
        public string StudentDataTimeLimitAction { get; set; }

        /// <summary>
        /// Gets or sets the StudentPreferenceChildren.
        /// </summary>
        public string StudentPreferenceChildren { get; set; }

        /// <summary>
        /// Gets or sets the StudentPreferenceAudio.
        /// </summary>
        public string StudentPreferenceAudio { get; set; }

        /// <summary>
        /// Gets or sets the StudentPreferenceLanguage.
        /// </summary>
        public string StudentPreferenceLanguage { get; set; }

        /// <summary>
        /// Gets or sets the StudentPreferenceSpeed.
        /// </summary>
        public string StudentPreferenceSpeed { get; set; }

        /// <summary>
        /// Gets or sets the StudentPreferenceText.
        /// </summary>
        public string StudentPreferenceText { get; set; }

        /// <summary>
        /// Gets or sets the InteractionsChildren.
        /// </summary>
        public string InteractionsChildren { get; set; }

        /// <summary>
        /// Gets or sets the ObjectivesChildren.
        /// </summary>
        public string ObjectivesChildren { get; set; }

        /// <summary>
        /// Gets or sets the ObjectivesScoreChildren.
        /// </summary>
        public string ObjectivesScoreChildren { get; set; }
    }
}
