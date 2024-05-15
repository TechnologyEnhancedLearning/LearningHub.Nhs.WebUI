namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Net;
    using System.Security.Claims;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The base class for API controllers.
    /// </summary>
    public abstract class OpenApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        public int? CurrentUserId
        {
            get
            {
                if ((this.User?.Identity?.AuthenticationType ?? null) == "AuthenticationTypes.Federation")
                {
                    int userId;
                    if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out userId))
                    {
                        return userId;
                    }
                    else
                    {
                        // If parsing fails, return null - for apikey this will be the name
                        return null;
                    }
                }
                else
                {
                    // When authorizing by ApiKey we do not have a user for example
                    return null;
                }
            }
        }

        public string TokenWithoutBearer
        {
            get
            {
                string accessToken = this.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new HttpResponseException($"No token", HttpStatusCode.Unauthorized);
                    }

                return accessToken.StartsWith("Bearer ") ? accessToken.Substring("Bearer ".Length) : accessToken;
            }
        }
    }
}
