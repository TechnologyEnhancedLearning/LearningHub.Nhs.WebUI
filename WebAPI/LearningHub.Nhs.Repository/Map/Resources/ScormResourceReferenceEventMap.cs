// <copyright file="ScormResourceReferenceEventMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="ScormResourceReferenceEventMap" />.
    /// </summary>
    public class ScormResourceReferenceEventMap : BaseEntityMap<ScormResourceReferenceEvent>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder<see cref="EntityTypeBuilder{ScormResourceReferenceEvent}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<ScormResourceReferenceEvent> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_ScormResource_ReferenceEvent");

            modelBuilder.ToTable("ScormResourceReferenceEvent", "resources");
        }
    }
}
