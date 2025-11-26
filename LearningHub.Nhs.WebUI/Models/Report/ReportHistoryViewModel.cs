namespace LearningHub.Nhs.WebUI.Models.Report
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Models.Learning;

    /// <summary>
    /// ReportHistoryViewModel.
    /// </summary>
    public class ReportHistoryViewModel
    {
        /// <summary>
        /// Gets or sets the CurrentPageIndex.
        /// </summary>
        public int CurrentPageIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the TotalCount.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the report result paging.
        /// </summary>
        public PagingViewModel ReportPaging { get; set; }

        /// <summary>
        /// Gets or sets the ReportFormActionTypeEnum.
        /// </summary>
        public ReportFormActionTypeEnum ReportFormActionType { get; set; }

        /// <summary>
        /// Gets or sets the ReportHistoryModels.
        /// </summary>
        public List<ReportHistoryModel> ReportHistoryModels { get; set; }
    }
}
