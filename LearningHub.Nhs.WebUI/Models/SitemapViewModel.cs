namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="SitemapViewModel" />.
    /// </summary>
    public class SitemapViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
        /// </summary>
        /// <param name="siteUrl">The site url.</param>
        /// <param name="catalogues">List of catalogues urls.</param>
        /// <param name="resources">List of resource reference id.</param>
        public SitemapViewModel(string siteUrl, IEnumerable<string> catalogues, IEnumerable<int> resources)
        {
            this.SiteUrl = siteUrl;

            this.DynamicUrls = catalogues.Select(c => $"/catalogue/{c}").Concat(resources.Select(r => $"/resource/{r}"));
        }

        /// <summary>
        /// Gets or sets the SiteUrl.
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the TotalSteps.
        /// </summary>
        public IEnumerable<string> DynamicUrls { get; set; }
    }
}
