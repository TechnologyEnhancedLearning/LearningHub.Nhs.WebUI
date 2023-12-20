// <copyright file="EmailTemplateMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Email Template Map.
    /// </summary>
    public class EmailTemplateMap : BaseEntityMap<EmailTemplate>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<EmailTemplate> modelBuilder)
        {
            modelBuilder.ToTable("EmailTemplate", "messaging");

            modelBuilder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired(false);

            modelBuilder.Property(x => x.Subject)
                .IsRequired();

            modelBuilder.Property(x => x.Body)
                .IsRequired();

            modelBuilder.HasOne(x => x.EmailTemplateLayout)
                .WithMany()
                .HasForeignKey(x => x.LayoutId)
                .IsRequired();
        }
    }
}
