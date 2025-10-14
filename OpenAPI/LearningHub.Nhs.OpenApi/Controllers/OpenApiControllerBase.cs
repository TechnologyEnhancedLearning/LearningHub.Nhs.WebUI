namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System;
    using System.Net;
    using System.Security.Claims;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The base class for API controllers.
    /// </summary>
    public abstract class OpenApiControllerBase : ControllerBase, IDisposable
    {
        private const int PortalAdminId = 4;

        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        public int? CurrentUserId
        {
            get
            {
                // This check is to determine between the two ways of authorising, OAuth and APIKey.OAuth provides userId and APIKey does not. For OpenApi we provide the data without specific user info.
                if ((this.User?.Identity?.AuthenticationType ?? null) == "AuthenticationTypes.Federation")
                {
                    int userId;
                    if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out userId))
                    {
                        return userId;
                    }
                    else
                    {
                        // If parsing fails, return PortalAdminId as default userId - for apikey this will be the name
                        return PortalAdminId;
                    }
                }
                else
                {
                    // When authorizing by ApiKey we do not have a user for example
                    return PortalAdminId;
                }
            }
        }

        /// <summary>
        /// Gets the bearer token from OAuth and removes "Bearer " prepend.
        /// </summary>
        public string TokenWithoutBearer
        {
            get
            {
                string accessToken = this.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new HttpResponseException($"No token provided please use OAuth", HttpStatusCode.Unauthorized);
                }

                return accessToken.StartsWith("Bearer ") ? accessToken.Substring("Bearer ".Length) : accessToken;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
