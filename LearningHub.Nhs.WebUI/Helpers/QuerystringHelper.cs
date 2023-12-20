// <copyright file="QuerystringHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
{
    /// <summary>
    /// Defines the <see cref="QuerystringHelper" />.
    /// </summary>
    public static class QuerystringHelper
    {
        /// <summary>
        /// The EncodeParameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The .</returns>
        public static string EncodeParameter(this string parameter)
        {
            parameter = parameter.Replace("+", "_*plus*_");
            parameter = parameter.Replace("/", "_*slash*_");

            return parameter;
        }
    }
}
