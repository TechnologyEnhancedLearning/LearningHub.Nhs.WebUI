using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    public class UserExternalSystemMap : AuditableEntityMap<UserExternalSystem>
    {
        protected override void InternalMap(
            EntityTypeBuilder<UserExternalSystem> modelBuilder)
        {
            modelBuilder.ToTable("UserExternalSystem", "hub");

            modelBuilder.Property(e => e.UserId)
                .HasColumnName("UserId");

            modelBuilder.Property(e => e.ExternalSystemId)
                .HasColumnName("ExternalSystemId");

            modelBuilder.Property(e => e.Active)
                .HasColumnName("Active");
        }
    }
}
