// <copyright file="ReadWriteRequirement.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Authentication
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Provide Authentication policy for Auth Service.
    /// </summary>
    public class ReadWriteRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteRequirement"/> class.
        /// Provide Authentication policy for Auth Service.
        /// </summary>
        public ReadWriteRequirement()
        {
        }

        /// <summary>
        /// The can read write.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool HasReadWriteRole(ClaimsPrincipal user)
        {
            bool retVal = false;
            foreach (var role in this.ReadWriteRoles())
            {
                if (user.IsInRole(role))
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The read write roles.
        /// </summary>
        /// <returns>The Read Write Roles.</returns>
        private List<string> ReadWriteRoles() => new List<string>() { "Administrator", "BlueUser" };
    }
}
