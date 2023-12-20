// <copyright file="QuerystringHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Helpers
{
    /// <summary>
    /// The querystring helper.
    /// </summary>
    public static class QuerystringHelper
    {
        /// <summary>
        /// The decode parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DecodeParameter(this string parameter)
        {
            parameter = parameter.Replace("_*plus*_", "+");
            parameter = parameter.Replace("_*slash*_", "/");

            return parameter;
        }
    }
}
