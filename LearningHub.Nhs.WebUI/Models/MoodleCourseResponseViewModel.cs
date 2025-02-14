namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// MoodleCourseResponseViewModel.
    /// </summary>
    public class MoodleCourseResponseViewModel
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the enrolled user count.
        /// </summary>
        public int? EnrolledUserCount { get; set; }

        /// <summary>
        /// Gets or sets the ID number.
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// Gets or sets the visibility status.
        /// </summary>
        public int? Visible { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the summary format.
        /// </summary>
        public int? SummaryFormat { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the course image URL.
        /// </summary>
        public string CourseImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grades are shown.
        /// </summary>
        public bool? ShowGrades { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether completion is enabled.
        /// </summary>
        public bool? EnableCompletion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether completion has criteria.
        /// </summary>
        public bool? CompletionHasCriteria { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether completion is user-tracked.
        /// </summary>
        public bool? CompletionUserTracked { get; set; }

        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        public int? Progress { get; set; }

        /// <summary>
        /// Gets or sets the completion status.
        /// </summary>
        public bool? Completed { get; set; }

        /// <summary>
        /// Gets or sets the start date (Unix timestamp).
        /// </summary>
        public long? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public int? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the marker.
        /// </summary>
        public int? Marker { get; set; }

        /// <summary>
        /// Gets or sets the last access timestamp.
        /// </summary>
        public int? LastAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is a favorite.
        /// </summary>
        public bool? IsFavourite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is hidden.
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Gets or sets the overview files.
        /// </summary>
        public List<object> OverviewFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether activity dates are shown.
        /// </summary>
        public bool? ShowActivityDates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether completion conditions are shown.
        /// </summary>
        public bool? ShowCompletionConditions { get; set; }

        /// <summary>
        /// Gets or sets the last modified timestamp (Unix timestamp).
        /// </summary>
        public long? TimeModified { get; set; }

        /// <summary>
        /// Gets or sets the moodle course completion view model.
        /// </summary>
        public MoodleCourseCompletionViewModel CourseCompletionViewModel { get; set; }
    }
}
