// <copyright file="DashBoardPagingViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.DashBoard
{
    /// <summary>
    /// The DashBoardPagingViewModel.
    /// </summary>
    public class DashBoardPagingViewModel
    {
        /// <summary>
        /// Gets or sets the dashboard tray type.
        /// </summary>
        public string DashboardTrayType { get; set; }

        /// <summary>
        /// Gets or sets the my learning dashboard.
        /// </summary>
        public string MyLearningDashboard { get; set; }

        /// <summary>
        /// Gets or sets the resource dashboard.
        /// </summary>
        public string ResourceDashboard { get; set; }

        /// <summary>
        /// Gets or sets the catalogue dashboard.
        /// </summary>
        public string CatalogueDashboard { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets the preevious page.
        /// </summary>
        public int PreviousPage => this.CurrentPage > 1 ? this.CurrentPage - 1 : this.CurrentPage;

        /// <summary>
        /// Gets the next page.
        /// </summary>
        public int NextPage => this.CurrentPage < 4 ? this.CurrentPage + 1 : this.CurrentPage;

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the display message.
        /// </summary>
        public string DisplayMessage
        {
            get
            {
                if (this.TotalCount == 0)
                {
                    return string.Empty;
                }

                return this.CurrentPage switch
                {
                    1 => $"{(this.TotalCount >= 3 ? "1 to 3" : $"1 to {this.TotalCount}")} of {this.TotalCount}",
                    2 => $"{(this.TotalCount >= 6 ? "4 to 6" : $"4 to {this.TotalCount}")} of {this.TotalCount}",
                    3 => $"{(this.TotalCount >= 9 ? "7 to 9" : $"7 to {this.TotalCount}")} of {this.TotalCount}",
                    4 => $"{(this.TotalCount >= 12 ? "10 to 12" : $"10 to {this.TotalCount}")} of {this.TotalCount}",
                    _ => string.Empty,
                };
            }
        }
    }
}
