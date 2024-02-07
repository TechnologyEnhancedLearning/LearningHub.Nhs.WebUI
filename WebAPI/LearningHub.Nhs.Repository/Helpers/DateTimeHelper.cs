// <copyright file="DateTimeHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Helpers
{
    using System;

    /// <summary>
    /// Extension methods for DateTime class.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Returns the date of the first day of the week, based on the day of week passed in.
        /// </summary>
        /// <param name="dt">The datetime.</param>
        /// <param name="weekStartDay">The first day of the week.</param>
        /// <returns>The date of the first day of the week.</returns>
        public static DateTime FirstDateInWeek(this DateTime dt, DayOfWeek weekStartDay)
        {
            while (dt.DayOfWeek != weekStartDay)
            {
                dt = dt.AddDays(-1);
            }

            return dt;
        }
    }
}
