// <copyright file="UserProviderMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user provider map.
    /// </summary>
    public class UserProviderMap : BaseEntityMap<UserProvider>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<UserProvider> modelBuilder)
        {
            modelBuilder.ToTable("UserProvider", "hub");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id");
            modelBuilder.Property(e => e.UserId).HasColumnName("UserId");
            modelBuilder.Property(e => e.ProviderId).HasColumnName("ProviderId");
            modelBuilder.Property(e => e.RemovalDate).HasColumnName("RemovalDate");

            modelBuilder.HasOne(d => d.User)
                .WithMany(p => p.UserProvider)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userProvider_user");

            modelBuilder.HasOne(d => d.Provider)
                .WithMany(p => p.UserProvider)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userProvider_provider");
        }
    }
}
