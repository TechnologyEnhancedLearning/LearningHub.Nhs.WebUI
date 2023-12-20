// <copyright file="ResourceActivityMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource activity map.
    /// </summary>
    public class ResourceActivityMap : BaseEntityMap<ResourceActivity>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceActivity> modelBuilder)
        {
            modelBuilder.ToTable("ResourceActivity", "activity");
            modelBuilder.Property(e => e.LaunchResourceActivityId).HasColumnName("LaunchResourceActivityId");
        }
    }
}
