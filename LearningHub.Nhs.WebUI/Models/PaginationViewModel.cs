namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// Defines the <see cref="PaginationViewModel" />.
    /// </summary>
    public class PaginationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationViewModel"/> class.
        /// </summary>
        public PaginationViewModel()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="PaginationViewModel"/> class.
        /// </summary>
        /// <param name="currentPage">Current page index.</param>
        /// <param name="totalPage">Total page index.</param>
        /// <param name="prevUrl">Previous page url.</param>
        /// <param name="nextUrl">Next page url.</param>
        public PaginationViewModel(int currentPage, int totalPage, string prevUrl, string nextUrl)
        {
            this.CurrentPage = currentPage;
            this.TotalPage = totalPage;
            this.PreviousUrl = prevUrl;
            this.NextUrl = nextUrl;
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total page.
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Gets or sets the previous page url.
        /// </summary>
        public string PreviousUrl { get; set; }

        /// <summary>
        /// Gets or sets the next page url.
        /// </summary>
        public string NextUrl { get; set; }
    }
}
