// <copyright file="ClientMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ClientMap class.
    /// </summary>
    public class ClientMap : BaseEntityMap<Client>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Client> modelBuilder)
        {
            modelBuilder.ToTable("Client", "reports");

            modelBuilder.Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired();
        }
    }
}
