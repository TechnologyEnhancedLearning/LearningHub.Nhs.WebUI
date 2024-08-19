namespace LearningHub.Nhs.WebUI.Models
{
    using LearningHub.Nhs.Models.Dashboard;

    /// <summary>
    /// Defines the <see cref="DashboardViewModel" />.
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        public DashboardViewModel()
        {
        }

        /// <summary>
        /// Gets or sets a list of my learning items to be displayed in the dashboard.
        /// </summary>
        public DashboardMyLearningResponseViewModel MyLearnings { get; set; }

        /// <summary>
        /// Gets or sets a list of resources to be displayed in the dashboard.
        /// </summary>
        public DashboardResourceResponseViewModel Resources { get; set; }

        /// <summary>
        /// Gets or sets a list of catalogues to be displayed in the dashboard.
        /// </summary>
        public DashboardCatalogueResponseViewModel Catalogues { get; set; }
    }
}
