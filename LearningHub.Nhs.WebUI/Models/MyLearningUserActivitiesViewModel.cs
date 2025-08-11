namespace LearningHub.Nhs.WebUI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.MyLearning;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Models.Learning;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="MyLearningUserActivitiesViewModel" />.
    /// </summary>
    public class MyLearningUserActivitiesViewModel : MyLearningRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningUserActivitiesViewModel"/> class.
        /// </summary>
        public MyLearningUserActivitiesViewModel()
        {
            this.Activities = new List<MyLearningCombainedActivitiesViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLearningUserActivitiesViewModel"/> class.
        /// </summary>
        /// <param name="requestModel">MyLearningRequestModel.</param>
        public MyLearningUserActivitiesViewModel(MyLearningRequestModel requestModel)
        {
            this.Activities = new List<MyLearningCombainedActivitiesViewModel>();
            foreach (PropertyInfo prop in requestModel.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(requestModel, null), null);
            }
        }

        /// <summary>
        /// Gets or sets the learning form event.
        /// </summary>
        public MyLearningFormActionTypeEnum MyLearningFormActionType { get; set; }

        /// <summary>
        /// Gets or sets the page item index.
        /// </summary>
        public int CurrentPageIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the TotalCount.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the MostRecentResources.
        /// </summary>
        public List<int> MostRecentResources { get; set; }

        /// <summary>
        /// Gets or sets the Activities.
        /// </summary>
        public List<MyLearningCombainedActivitiesViewModel> Activities { get; set; }

        /// <summary>
        /// Gets or sets the learning result paging.
        /// </summary>
        public PagingViewModel MyLearningPaging { get; set; }
    }
}
