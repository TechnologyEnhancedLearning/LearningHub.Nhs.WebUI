namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The FindwiseSettings.
    /// </summary>
    public class FindwiseSettings
    {
        /// <summary>
        /// Gets or sets the index url.
        /// </summary>
        public string IndexUrl { get; set; }

        /// <summary>
        /// Gets or sets the search url.
        /// </summary>
        public string SearchUrl { get; set; }

        /// <summary>
        /// Gets or sets the url search component.
        /// </summary>
        public string UrlSearchComponent { get; set; }

        /// <summary>
        /// Gets or sets the url click component.
        /// </summary>
        public string UrlClickComponent { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the maximum description length.
        /// </summary>
        public int MaximumDescriptionLength { get; set; }

        /// <summary>
        /// Gets or sets the index method.
        /// </summary>
        public string IndexMethod { get; set; }

        /// <summary>
        /// Gets or sets the collection ids.
        /// </summary>
        public FindwiseCollectionIdSettings CollectionIds { get; set; }

        /// <summary>
        /// Gets or sets the Description length limit.
        /// </summary>
        public int DescriptionLengthLimit { get; set; }
    }
}
