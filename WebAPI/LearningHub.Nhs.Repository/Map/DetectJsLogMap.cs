// <copyright file="DetectJsLogMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The log map.
    /// </summary>
    public class DetectJsLogMap : BaseEntityMap<DetectJsLog>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<DetectJsLog> modelBuilder)
        {
            modelBuilder.ToTable("DetectJsLog", "hub");
        }
    }
}