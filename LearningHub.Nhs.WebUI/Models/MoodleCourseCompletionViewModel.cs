namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// MoodleCourseCompletionViewModel.
    /// </summary>
    public class MoodleCourseCompletionViewModel
    {
        /// <summary>
        /// Gets or sets the completion status.
        /// </summary>
        public CompletStatus CompletionStatus { get; set; }

        /// <summary>
        /// Gets or sets the list of warnings.
        /// </summary>
        public List<object> Warnings { get; set; }

        /// <summary>
        /// CompletionStatus.
        /// </summary>
        public class CompletStatus
        {
            /// <summary>
            /// Gets or sets a value indicating whether the course is completed.
            /// </summary>
            public bool Completed { get; set; }

            /// <summary>
            /// Gets or sets the aggregation method.
            /// </summary>
            public int Aggregation { get; set; }

            /// <summary>
            /// Gets or sets the list of completions.
            /// </summary>
            public List<Completion> Completions { get; set; }

            /// <summary>
            /// Completion.
            /// </summary>
            public class Completion
            {
                /// <summary>
                /// Gets or sets the type of completion.
                /// </summary>
                public int Type { get; set; }

                /// <summary>
                /// Gets or sets the title of the completion requirement.
                /// </summary>
                public string Title { get; set; }

                /// <summary>
                /// Gets or sets the status of the completion.
                /// </summary>
                public string Status { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the requirement is complete.
                /// </summary>
                public bool Complete { get; set; }

                /// <summary>
                /// Gets or sets the timestamp when completion was achieved.
                /// </summary>
                public long? TimeCompleted { get; set; }

                /// <summary>
                /// Gets or sets the completion details.
                /// </summary>
                public CompletionDetails Details { get; set; }

                /// <summary>
                /// CompletionDetails.
                /// </summary>
                public class CompletionDetails
                {
                    /// <summary>
                    /// Gets or sets the type of completion requirement.
                    /// </summary>
                    public string Type { get; set; }

                    /// <summary>
                    /// Gets or sets the criteria for completion.
                    /// </summary>
                    public string Criteria { get; set; }

                    /// <summary>
                    /// Gets or sets the requirement for completion.
                    /// </summary>
                    public string Requirement { get; set; }

                    /// <summary>
                    /// Gets or sets the status of the requirement.
                    /// </summary>
                    public string Status { get; set; }
                }
            }
        }
    }
}