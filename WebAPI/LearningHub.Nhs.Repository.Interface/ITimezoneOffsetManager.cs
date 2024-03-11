namespace LearningHub.Nhs.Repository.Interface
{
    using System;

    /// <summary>
    /// The TimezoneOffsetManager interface.
    /// </summary>
    public interface ITimezoneOffsetManager
    {
        /// <summary>
        /// Gets User Timezone Offset.
        /// </summary>
        int? UserTimezoneOffset { get; }

        /// <summary>
        /// Converts a UTC DateTimeOffset to the timezone of the current user.
        /// </summary>
        /// <param name="utcDate">The UTC DateTimeOffset to convert.</param>
        /// <returns><see cref="DateTimeOffset"/>.</returns>
        DateTimeOffset ConvertToUserTimezone(DateTimeOffset utcDate);
    }
}