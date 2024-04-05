namespace LearningHub.Nhs.WebUI.Helpers
{
    using System;

    /// <summary>
    /// Defines the <see cref="IClockUtility" />.
    /// </summary>
    public interface IClockUtility
    {
        /// <summary>
        /// Gets UtcNow.
        /// </summary>
        public DateTime UtcNow { get; }

        /// <summary>
        /// Gets UtcToday.
        /// </summary>
        public DateTime UtcToday { get; }
    }

    /// <summary>
    /// Defines the <see cref="ClockUtility" />.
    /// </summary>
    public class ClockUtility : IClockUtility
    {
        /// <inheritdoc/>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc/>
        public DateTime UtcToday => this.UtcNow.Date;
    }
}
