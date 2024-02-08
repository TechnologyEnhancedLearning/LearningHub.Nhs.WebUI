namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version event map.
    /// </summary>
    public class ResourceVersionEventMap : BaseEntityMap<ResourceVersionEvent>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionEvent> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionEvent", "resources");

            modelBuilder.Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Property(e => e.Details).HasMaxLength(1024);

            modelBuilder.Property(e => e.ResourceVersionEventType).HasColumnName("ResourceVersionEventTypeId")
               .HasConversion<int>();

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithMany(p => p.ResourceVersionEvent)
                        .HasForeignKey(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ResourceVersionEvent_ResourceVersion");

            modelBuilder.HasOne(d => d.CreateUser)
                .WithMany(p => p.ResourceVersionEvent)
                .HasForeignKey(d => d.CreateUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionEvent_CreateUser");
        }
    }
}
