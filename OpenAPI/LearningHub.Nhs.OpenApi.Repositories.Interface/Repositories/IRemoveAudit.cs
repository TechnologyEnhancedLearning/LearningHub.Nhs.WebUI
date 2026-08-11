using System;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    public interface IRemoveAudit
    {
        DateTimeOffset? RemoveDate { get; set; }

        int? RemoveUserId { get; set; }
    }
}
