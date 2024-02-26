namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Roadmap map.
    /// </summary>
    public class RoadmapMap : BaseEntityMap<Roadmap>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Roadmap> builder)
        {
            builder.ToTable("Roadmap", "hub");

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.RoadmapTypeId)
                .HasColumnName("RoadmapTypeId");

            builder.HasOne(x => x.RoadmapType)
                .WithMany()
                .HasForeignKey(x => x.RoadmapTypeId)
                .HasConstraintName("FK_roadmap_roadmapType");

            builder.Property(x => x.Title)
                .HasColumnName("Title")
                .HasMaxLength(400);

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasMaxLength(8000);

            builder.Property(x => x.RoadmapDate)
                .HasColumnName("RoadmapDate");

            builder.Property(x => x.ImageName)
                .HasColumnName("ImageName")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.OrderNumber)
                .HasColumnName("OrderNumber");

            builder.Property(x => x.Published)
                .HasColumnName("Published");
        }
    }
}
