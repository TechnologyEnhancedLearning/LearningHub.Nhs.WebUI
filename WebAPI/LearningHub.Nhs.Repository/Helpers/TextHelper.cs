// <copyright file="TextHelper.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Helpers
{
    /// <summary>
    /// The text helper.
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// The Combine Date Components method.
        /// </summary>
        /// <param name="authoredYear">The authoed year.</param>
        /// <param name="authoredMonth">The authored month.</param>
        /// <param name="authoredDayOfMonth">The authored day of month.</param>
        /// <returns>The formatted date.</returns>
        public static string CombineDateComponents(int? authoredYear, int? authoredMonth, int? authoredDayOfMonth)
        {
            string returnText = string.Empty;
            if (authoredYear > 0)
            {
                returnText = authoredYear.ToString();

                if (authoredMonth > 0)
                {
                    string[] monthName = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    returnText = monthName[(int)authoredMonth - 1] + " " + returnText;

                    if (authoredDayOfMonth > 0)
                    {
                        returnText = authoredDayOfMonth.ToString() + " " + returnText;
                    }
                }
            }

            return returnText;
        }
    }
}
