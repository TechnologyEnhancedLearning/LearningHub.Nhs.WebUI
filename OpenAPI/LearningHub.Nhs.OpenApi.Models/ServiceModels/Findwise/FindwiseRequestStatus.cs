// <copyright file="FindwiseRequestStatus.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise
{
    /// <summary>
    /// <see cref="FindwiseRequestStatus"/>.
    /// </summary>
    public enum FindwiseRequestStatus
    {
        /// <summary>
        /// <see cref="Success"/>
        /// </summary>
        Success,

        /// <summary>
        /// <see cref="AccessDenied"/>
        /// </summary>
        AccessDenied,

        /// <summary>
        /// <see cref="Timeout"/>
        /// </summary>
        Timeout,

        /// <summary>
        /// <see cref="RequestException"/>
        /// </summary>
        RequestException,

        /// <summary>
        /// <see cref="BadRequest"/>
        /// </summary>
        BadRequest,
    }
}
