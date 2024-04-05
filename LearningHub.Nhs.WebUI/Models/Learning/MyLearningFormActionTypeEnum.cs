namespace LearningHub.Nhs.WebUI.Models.Learning
{
    /// <summary>
    /// Defines the MyLearningFormActionTypeEnum.
    /// </summary>
    public enum MyLearningFormActionTypeEnum
    {
        /// <summary>
        /// Defines the basic search for mylearning
        /// </summary>
        Default = 0,

        /// <summary>
        /// Defines the basic search for mylearning
        /// </summary>
        BasicSearch = 1,

        /// <summary>
        /// Defines the weekly mylearning filter.
        /// </summary>
        ApplyWeekFilter = 2,

        /// <summary>
        /// Defines the monthly mylearning filter.
        /// </summary>
        ApplyMonthFilter = 3,

        /// <summary>
        /// Defines the 12 months mylearning filter.
        /// </summary>
        ApplyTwelveMonthFilter = 4,

        /// <summary>
        /// Defines the major mylearning filter.
        /// </summary>
        ApplyMajorFilters = 5,

        /// <summary>
        /// Defines the clearall mylearning filter.
        /// </summary>
        ClearAllFilters = 6,

        /// <summary>
        /// Previoous page change.
        /// </summary>
        PreviousPageChange = 7,

        /// <summary>
        /// Next page change.
        /// </summary>
        NextPageChange = 8,
    }
}
