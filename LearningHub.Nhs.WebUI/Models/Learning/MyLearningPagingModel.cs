// <copyright file="MyLearningPagingModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Learning
{
    using LearningHub.Nhs.Models.Paging;

    /// <summary>
    /// Defines the <see cref="MyLearningPagingModel" />.
    /// </summary>
    public class MyLearningPagingModel : PagingViewModel
    {
        /// <summary>
        /// Gets or sets the page previous action value.
        /// </summary>
        public int PreviousActionValue { get; set; }

        /// <summary>
        /// Gets or sets the page next action value.
        /// </summary>
        public int NextActionValue { get; set; }
    }
}
