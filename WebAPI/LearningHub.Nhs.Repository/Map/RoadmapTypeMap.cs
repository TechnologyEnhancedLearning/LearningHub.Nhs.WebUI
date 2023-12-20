// <copyright file="RoadmapTypeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The roadmap type map.
    /// </summary>
    public class RoadmapTypeMap : BaseEntityMap<RoadmapType>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void InternalMap(EntityTypeBuilder<RoadmapType> builder)
        {
            builder.Property(x => x.Id)
                .HasColumnName("Id");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasMaxLength(100);
        }
    }
}
