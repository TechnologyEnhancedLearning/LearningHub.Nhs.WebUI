// <copyright file="UserBookmarkMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="UserBookmarkMap" />.
    /// </summary>
    public class UserBookmarkMap : BaseEntityMap<UserBookmark>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder<see cref="EntityTypeBuilder{UserBookmark}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<UserBookmark> modelBuilder)
        {
            modelBuilder.ToTable("UserBookmark", "hub");
        }
    }
}
