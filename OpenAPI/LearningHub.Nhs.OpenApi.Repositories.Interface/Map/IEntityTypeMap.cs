// <copyright file="IEntityTypeMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Map
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
