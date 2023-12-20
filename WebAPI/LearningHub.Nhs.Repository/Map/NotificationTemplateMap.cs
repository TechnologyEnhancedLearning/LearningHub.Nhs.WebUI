// <copyright file="NotificationTemplateMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The NotificationTemplateMap class.
    /// </summary>
    public class NotificationTemplateMap : BaseEntityMap<NotificationTemplate>, IEntityTypeMap
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.ToTable("NotificationTemplate", "hub");

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.Subject)
                .IsRequired();

            builder.Property(x => x.Body)
                .IsRequired();

            builder.Property(x => x.AvailableTags)
                .IsRequired();
        }
    }
}
