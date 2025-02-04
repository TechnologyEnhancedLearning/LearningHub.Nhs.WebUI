namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The FindwiseSettings.
    /// </summary>
    public class FindwiseConfig
    {
        /// <summary>
        /// Gets or sets the base url for the Findwise Index endpoint.
        /// </summary>
        public string IndexUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the base url for the Findwise search service.
        /// </summary>
        public string SearchBaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the url search component.
        /// </summary>
        public string SearchEndpointPath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Gets or sets the hits per request.
        /// </summary>
        public int DefaultItemLimitForSearch { get; set; }

        /// <summary>
        /// Gets or sets the description limit.
        /// </summary>
        public int DescriptionLengthLimit { get; set; }

        /// <summary>
        /// Gets or sets the description length.
        /// </summary>
        public int MaximumDescriptionLength { get; set; }

        /// <summary>
        /// Gets or sets the collection ids.
        /// </summary>
        public FindwiseCollectionIdSettings CollectionIds { get; set; } = null!;

        /// <summary>
        ///  Gets or sets the special search characters.
        /// </summary>
        public string SpecialSearchCharacters { get; set; } = null!;

        /// <summary>
        ///  Gets or sets the index method.
        /// </summary>
        public string IndexMethod { get; set; } = null!;
    }
}
