namespace LearningHub.Nhs.OpenApi.Services.Helpers.Search
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for building Lucene queries for Azure Search.
    /// </summary>
    public static class LuceneQueryBuilder
    {
        /// <summary>
        /// Builds a Lucene query from the search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>The Lucene query string.</returns>
        public static string BuildLuceneQuery(string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return "*";

            var tokens = searchText
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(EscapeLuceneSpecialCharacters)
                .Where(t => !string.IsNullOrWhiteSpace(t));

            return string.Join(" AND ", tokens);
        }

        /// <summary>
        /// Escapes Lucene special characters in the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The escaped string.</returns>
        public static string EscapeLuceneSpecialCharacters(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input ?? string.Empty;

            var pattern = @"([+\-!(){}[\]^\""?~*:\\/])";
            return Regex.Replace(input, pattern, "\\$1");
        }
    }
}
