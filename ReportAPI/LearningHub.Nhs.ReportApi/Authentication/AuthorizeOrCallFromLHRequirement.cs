// <copyright file="AuthorizeOrCallFromLHRequirement.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Authentication
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class AuthorizeOrCallFromLHRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeOrCallFromLHRequirement"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        public AuthorizeOrCallFromLHRequirement()
        {
        }
    }
}
