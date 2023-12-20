// <copyright file="ElfhCacheSettings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The ELFH cache settings.
    /// </summary>
    public class ElfhCacheSettings
    {
        /// <summary>
        /// Gets or sets the ELFH Redis Key Prefix.
        /// </summary>
        public string ElfhRedisKeyPrefix { get; set; }

        /// <summary>
        /// Gets or sets the ELFH user load by user Id key.
        /// </summary>
        public string ElfhUserLoadByUserIdKey { get; set; }

        /// <summary>
        /// Gets or sets the ELFH user load by user name key.
        /// </summary>
        public string ElfhUserLoadByUserNameKey { get; set; }
    }
}
