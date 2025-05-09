﻿namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Map;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The password reset requests map.
    /// </summary>
    public class PasswordResetRequestsMap : BaseEntityMap<PasswordResetRequests>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<PasswordResetRequests> modelBuilder)
        {
            modelBuilder.ToTable("PasswordResetRequests", "hub");
        }
    }
}