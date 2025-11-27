namespace LearningHub.Nhs.WebUI.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using LearningHub.Nhs.WebUI.Models.DynamicCheckbox;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// CourseSelection.
    /// </summary>
    public class ReportCreationCourseSelection
    {
        /// <summary>
        /// Gets or sets the list of courses.
        /// </summary>
        public List<string> Courses { get; set; }

        /// <summary>
        /// Gets or sets the list of all courses.
        /// </summary>
        public List<DynamicCheckboxItemViewModel> AllCources { get; set; }

        /// <summary>
        /// Gets or sets the list of SearchText.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// BuildCourses.
        /// </summary>
        /// <param name="allCourses">The all Courses.</param>
        /// <returns>The <see cref="List{RadiosItemViewModel}"/>.</returns>
        public List<DynamicCheckboxItemViewModel> BuildCourses(List<KeyValuePair<string, string>> allCourses)
        {
            this.AllCources = allCourses.Select(r => new DynamicCheckboxItemViewModel
            {
                Value = r.Key.ToString(),
                Label = r.Value,
            }).ToList();
            this.AllCources.Insert(0, new DynamicCheckboxItemViewModel { Value = "all", Label = "All Courses", });
            return this.AllCources;
        }
    }
}
