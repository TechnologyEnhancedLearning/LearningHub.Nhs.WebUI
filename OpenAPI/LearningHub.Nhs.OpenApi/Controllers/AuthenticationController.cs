namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Authentication;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.NHS.OpenAPI.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Authentication operations.
    /// </summary>
    [Authorize(Policy = "AuthorizeOrCallFromLH")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ISecurityService securityService;
        private readonly IUserService lhUserService;
        private readonly IExternalSystemUserService extSysUserSvc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="lhUserService">The lh user service.</param>
        /// <param name="securityService">The security service.</param>
        /// <param name="extSysUserSvc">The external system service.</param>
        public AuthenticationController(
                    IAuthenticationService authenticationService,
                    ISecurityService securityService,
                    IUserService lhUserService,
                    IExternalSystemUserService extSysUserSvc)
        {
            this.authenticationService = authenticationService;
            this.securityService = securityService;
            this.lhUserService = lhUserService;
            this.extSysUserSvc = extSysUserSvc;
        }

        /// <summary>
        /// Authenticate user.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("Authenticate")]
        public async Task<IActionResult> AuthenticateLhHubAsync([FromBody] Login login)
        {
            var loginResult = await this.authenticationService.AuthenticateAsync(login);
            return this.Ok(loginResult);
        }

        /// <summary>
        /// Authenticate SSO user.
        /// </summary>
        /// <param name="request">External login details.</param>
        /// <returns>The <see cref="LoginResult"/>.</returns>
        [HttpPost("authenticate-sso")]
        public async Task<IActionResult> AuthenticateSsoAsync([FromBody] LoginSso request)
        {
            var extSystemUser = await this.extSysUserSvc.GetByIdAsync(request.UserId, request.ExternalSystemId);
            if (extSystemUser == null)
            {
                var elfhExternalUser = await this.extSysUserSvc.GetLhExternalUserByUserIdAndClientCode(request.UserId, request.ClientCode);
                if (elfhExternalUser == null)
                {
                    return this.Ok(new LoginResultInternal { IsAuthenticated = false, ErrorMessage = "User Id is incorrect or user doest not belongs to external client." });
                }
            }

            var loginResult = await this.authenticationService.CheckUserCredentialsSsoAsync(request.UserId);

            return this.Ok(loginResult);
        }

        /// <summary>
        /// Check credentials without user authentication.
        /// </summary>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [Route("checkcredentials")]
        public async Task<IActionResult> CheckUserCredentialsLhHubAsync([FromBody] Login login)
        {
            var loginResult = await this.authenticationService.CheckUserCredentialsAsync(login);

            return this.Ok(loginResult);
        }

        /// <summary>
        /// Validate user token.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="locToken">
        /// The loc Token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("ValidateToken/{token}/{loctoken}")]
        public async Task<IActionResult> ValidateToken(string token, string locToken)
        {
            var result = await this.securityService.ValidateTokenAsync(token.DecodeParameter(), locToken.DecodeParameter());
            return this.Ok(result);
        }

        /// <summary>
        /// Set user's initial password.
        /// </summary>
        /// <param name="passwordCreateModel">
        /// The password create model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPut("{passwordCreateModel}")]
        public async Task<IActionResult> SetInitialUserPassword([FromBody] PasswordCreateModel passwordCreateModel)
        {
            var result = await this.securityService.SetInitialPasswordAsync(passwordCreateModel);
            return this.Ok(result);
        }
    }
}