namespace LearningHub.Nhs.Shared.Helpers
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;

    /// <summary>
    /// Defines the <see cref="SearchHelper" />.
    /// </summary>
    public static class SearchHelper
    {
        /// <summary>
        /// The GetSearchSortList.
        /// </summary>
        /// <returns>The <see cref="List{SortItemViewModel}"/>.</returns>
        public static List<SortItemViewModel> GetSearchSortList()
        {
            var sortlist = new List<SortItemViewModel>()
            {
                new SortItemViewModel { SearchSortType = SearchSortTypeEnum.Relevance,  Name = "Relevance", Value = string.Empty, SortDirection = string.Empty },
                new SortItemViewModel { SearchSortType = SearchSortTypeEnum.AToZ,  Name = "A to Z", Value = "title", SortDirection = "ascending" },
                new SortItemViewModel { SearchSortType = SearchSortTypeEnum.DateAuthored, Name = "Date authored", Value = "authored_date", SortDirection = "descending" },
                new SortItemViewModel { SearchSortType = SearchSortTypeEnum.Rating, Name = "Rating", Value = "rating", SortDirection = "descending" },
            };

            return sortlist;
        }
    }
}
