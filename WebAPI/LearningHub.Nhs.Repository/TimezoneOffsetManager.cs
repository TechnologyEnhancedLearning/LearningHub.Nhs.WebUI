// <copyright file="TimezoneOffsetManager.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// The TimezoneOffsetManager class.
    /// </summary>
    public class TimezoneOffsetManager : ITimezoneOffsetManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private int? userTimezoneOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimezoneOffsetManager"/> class.
        /// </summary>
        /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/>.</param>
        public TimezoneOffsetManager(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets User Timezone Offset.
        /// </summary>
        public int? UserTimezoneOffset
        {
            get
            {
                if (!this.userTimezoneOffset.HasValue)
                {
                    if (this.httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-tz-offset", out StringValues tzOffsetString))
                    {
                        if (int.TryParse(tzOffsetString, out int timezoneOffset))
                        {
                            this.userTimezoneOffset = timezoneOffset;
                        }
                    }
                }

                return this.userTimezoneOffset;
            }
        }

        /// <summary>
        /// Converts a UTC DateTimeOffset to the timezone of the current user.
        /// </summary>
        /// <param name="utcDate">The UTC DateTimeOffset to convert.</param>
        /// <returns><see cref="DateTimeOffset"/>.</returns>
        public DateTimeOffset ConvertToUserTimezone(DateTimeOffset utcDate)
        {
            var tzOffset = this.UserTimezoneOffset;
            return tzOffset.HasValue ? new DateTimeOffset(utcDate.AddMinutes(tzOffset.Value).Ticks, TimeSpan.FromMinutes(tzOffset.Value)) : utcDate;
        }
    }
}