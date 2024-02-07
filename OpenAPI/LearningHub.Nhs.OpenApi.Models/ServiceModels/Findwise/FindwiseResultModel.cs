// <copyright file="FindwiseResultModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise
{
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// <see cref="FindwiseResultModel"/>.
    /// </summary>
    public class FindwiseResultModel
    {
        private FindwiseResultModel(SearchResultModel? searchResults)
        {
            this.SearchResults = searchResults;
            this.FindwiseRequestStatus = FindwiseRequestStatus.Success;
        }

        private FindwiseResultModel(FindwiseRequestStatus findwiseRequestStatus)
        {
            this.SearchResults = null;
            this.FindwiseRequestStatus = findwiseRequestStatus;
        }

        /// <summary>
        /// Gets <see cref="SearchResults"/>.
        /// </summary>
        public SearchResultModel? SearchResults { get; }

        /// <summary>
        /// Gets <see cref="FindwiseRequestStatus"/>.
        /// </summary>
        public FindwiseRequestStatus FindwiseRequestStatus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindwiseResultModel"/> class
        /// with successful Findwise request status.
        /// </summary>
        /// <param name="searchResults"><see cref="SearchResults"/>.</param>
        /// <returns><see cref="FindwiseResultModel"/>.</returns>
        public static FindwiseResultModel Success(SearchResultModel? searchResults)
        {
            return new (searchResults);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindwiseResultModel"/> class with empty search results.
        /// </summary>
        /// <param name="findwiseRequestStatus"><see cref="FindwiseRequestStatus"/>.</param>
        /// <returns><see cref="FindwiseResultModel"/>.</returns>
        public static FindwiseResultModel Failure(FindwiseRequestStatus findwiseRequestStatus)
        {
            return new (findwiseRequestStatus);
        }
    }
}
