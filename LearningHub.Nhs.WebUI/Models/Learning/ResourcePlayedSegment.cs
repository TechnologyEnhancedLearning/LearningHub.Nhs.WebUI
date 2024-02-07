// <copyright file="ResourcePlayedSegment.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Learning
{
    using System;

    /// <summary>
    /// ResourcePlayedSegment.
    /// </summary>
    public class ResourcePlayedSegment
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Played.
        /// </summary>
        public bool Played { get; set; }

        /// <summary>
        /// Gets or sets SegmentStartTime.
        /// </summary>
        public int SegmentStartTime { get; set; }

        /// <summary>
        /// Gets or sets SegmentEndTime.
        /// </summary>
        public int SegmentEndTime { get; set; }

        /// <summary>
        /// Gets SegmentTime.
        /// </summary>
        public string SegmentTime => $"{GetDurationHhmmss(this.SegmentStartTime)} to {GetDurationHhmmss(this.SegmentEndTime)}";

        /// <summary>
        /// Gets or sets Percentage.
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// GetDurationHhmmss.
        /// </summary>
        /// <param name="seconds">seconds.</param>
        /// <returns>string.</returns>
        public static string GetDurationHhmmss(int seconds)
        {
            var minutes = Math.Floor((decimal)seconds / 60);
            seconds %= 60;

            var hours = Math.Floor(minutes / 60);
            minutes %= 60;

            var duration = string.Empty;
            if (hours > 0)
            {
                duration = hours.ToString().PadLeft(2, '0');
            }

            duration += minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
            return duration;
        }
    }
}