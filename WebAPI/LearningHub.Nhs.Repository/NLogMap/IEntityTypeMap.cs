// <copyright file="IEntityTypeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.NLogMap
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
        /// <param name="builder">
        /// The builder.
        /// </param>
        void Map(ModelBuilder builder);
    }
}
