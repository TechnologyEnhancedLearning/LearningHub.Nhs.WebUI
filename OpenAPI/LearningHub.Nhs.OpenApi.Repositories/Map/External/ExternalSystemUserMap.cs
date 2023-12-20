// <copyright file="ExternalSystemUserMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.External
{
    using LearningHub.Nhs.Models.Entities.External;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ExternalSystemUser map.
    /// </summary>
    public class ExternalSystemUserMap : BaseEntityMap<ExternalSystemUser>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ExternalSystemUser> modelBuilder)
        {
            modelBuilder.ToTable("ExternalSystemUser", "external");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.UserId).IsRequired();

            modelBuilder.Property(e => e.ExternalSystemId).IsRequired();

            modelBuilder.HasOne(d => d.ExternalSystem)
                .WithMany()
                .HasForeignKey(d => d.ExternalSystemId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
