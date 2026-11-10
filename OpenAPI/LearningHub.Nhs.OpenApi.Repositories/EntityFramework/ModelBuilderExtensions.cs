using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyRemoveAuditConvention(
            this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IRemoveAudit).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Ignore(nameof(EntityBase.Deleted));
                }
            }
        }
    }
}
