﻿namespace LearningHub.Nhs.Repository.Map
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The EntityTypeMap interface.
    /// </summary>
    public interface IEntityTypeMap
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void Map(ModelBuilder builder);
    }
}
