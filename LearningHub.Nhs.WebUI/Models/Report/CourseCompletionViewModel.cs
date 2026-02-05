namespace LearningHub.Nhs.WebUI.Models.Report
{
    using System.Collections.Generic;
    using System.Reflection;
    using LearningHub.Nhs.Models.Databricks;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.WebUI.Models.Learning;

    /// <summary>
    /// CourseCompletionViewModel.
    /// </summary>
    public class CourseCompletionViewModel : DatabricksRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CourseCompletionViewModel"/> class.
        /// </summary>
        public CourseCompletionViewModel()
        {
            this.CourseCompletionRecords = new List<DatabricksDetailedItemViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseCompletionViewModel"/> class.
        /// </summary>
        /// <param name="requestModel">DatabricksRequestModel.</param>
        public CourseCompletionViewModel(DatabricksRequestModel requestModel)
        {
            this.CourseCompletionRecords = new List<DatabricksDetailedItemViewModel>();
            foreach (PropertyInfo prop in requestModel.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(requestModel, null), null);
            }
        }

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
        /// Gets or sets the CourseCompletionRecords.
        /// </summary>
        public List<DatabricksDetailedItemViewModel> CourseCompletionRecords { get; set; }

        /// <summary>
        /// Gets or sets the ReportHistoryModel.
        /// </summary>
        public ReportHistoryModel ReportHistoryModel { get; set; }
    }
}
