namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using elfhHub.Nhs.Models.Common;

    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Helpers.OpenAthens;
    using LearningHub.Nhs.WebUI.Interfaces;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The open athens controller.
    /// </summary>
    public class OpenAthensController : BaseController
    {
        private readonly IUserService userService;
        private readonly LearningHubAuthServiceConfig authConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAthensController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="env">The env.</param>
        /// <param name="httpClientFactory">The http client factory.</param>
        /// <param name="settings">The settings.</param>
        public OpenAthensController(
            ILogger<OpenAthensController> logger,
            IUserService userService,
            LearningHubAuthServiceConfig authConfig,
            IWebHostEnvironment env,
            IHttpClientFactory httpClientFactory,
            IOptions<Settings> settings)
            : base(env, httpClientFactory, logger, settings.Value)
        {
            this.userService = userService;
            this.authConfig = authConfig;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="authId">The auth id.</param>
        /// <param name="origin">The origin.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        public IActionResult Index(string payload, string authId, string origin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(authId) ||
                    string.IsNullOrWhiteSpace(origin))
                {
                    var ex = new Exception("Payload, authId or origin are empty.");
                    ex.Data.Add("payload", payload);
                    ex.Data.Add("authId", authId);
                    ex.Data.Add("origin", origin);
                    throw ex;
                }

                if (!string.Equals(authId, this.authConfig.AuthSecret, StringComparison.InvariantCultureIgnoreCase))
                {
                    var ex = new Exception("Could not validate auth server.");
                    ex.Data.Add("thereAuthId", authId);
                    ex.Data.Add("ourAuthId", this.authConfig.AuthSecret);
                    throw ex;
                }

                if (!string.Equals(origin.StartsWith("https://") ? origin : $"https://{origin}", this.authConfig.Authority, StringComparison.InvariantCultureIgnoreCase))
                {
                    var ex = new Exception("Auth origins did not match.");
                    ex.Data.Add("ourAuthOrigin", this.authConfig.Authority);
                    ex.Data.Add("thereOrigin", origin);
                    throw ex;
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, e.Message);
                throw;
            }

            var payloadModel = OpenAthensOpenIdConnect.DeserialiseAuthPayload(payload);
            var model = new BeginOpenAthensLinkToLearningHubUser();
            model.ExtractOpenAthensProps(payloadModel);

            if (string.IsNullOrWhiteSpace(model.OaOrganisationId))
            {
                this.Logger.LogError($"OpenAthens user came back from OpenAthens auth without an OA userId, invalid and aborting user log on.");
            }

            this.Logger.LogInformation($"OpenAthens user authenticated by OpenAthens with OA userId: [{model.OaUserId}]");

            return this.View(model);
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> Start(BeginOpenAthensLinkToLearningHubUser model)
        {
            // Check Begin Link form and if success create link to known LH user
            if (this.ModelState.ContainsKey("loginResult"))
            {
                this.ModelState.Remove("loginResult");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View("Index", model);
            }

            var loginInfo = new Login { Username = model.Username, Password = model.Password, IsAuthenticated = false };
            var loginResult = await this.userService.CheckUserCredentialsAsync(loginInfo);

            if (!loginResult.IsAuthenticated && !string.IsNullOrWhiteSpace(loginResult.ErrorMessage))
            {
                this.ModelState.AddModelError("loginResult", loginResult.ErrorMessage);
                return this.View("Index", model);
            }
            else if (!loginResult.IsAuthenticated)
            {
                this.ModelState.AddModelError(
                    "loginResult",
                    "There was a problem trying to check your details. Please try again.");
                if (loginResult.UserId > 0)
                {
                    this.Logger.LogWarning(
                        "Credential check for [{username}] [{lhuserid}] came back from LHAPI as denied with no error message",
                        model.Username,
                        loginResult.UserId);
                }
                else
                {
                    this.Logger.LogWarning(
                        $"Credential check for [{model.Username}] came back from LHAPI as denied with no error message");
                }

                return this.View("Index", model);
            }

            var linkSuccess = await this.userService.CreateOpenAthensLinkToUserAsync(
                                  loginResult.UserId,
                                  model.OaUserId,
                                  model.OaOrganisationId);

            if (linkSuccess)
            {
                // Go through the login process again, the user should be automatically logged on
                return this.Login(this.Url.Action("AuthorisationRequired", "Account", new { originalUrl = "/" }, "https"));
            }

            this.Logger.LogWarning(
                "Failed to link Learning Hub and Open Athens accounts [Username:{username}, lhuserid: {lhuserid}, OaUserId: {OaUserId}]",
                model.Username,
                loginResult.UserId,
                model.OaUserId);
            this.ModelState.AddModelError(
                "loginResult",
                "There was a problem trying to link to your OpenAthens account.");
            return this.View("Index", model);
        }

        /// <summary>
        /// The create link for new user.
        /// </summary>
        /// <param name="oaDetails">The oa details.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        public IActionResult CreateLinkForNewUser(OpenAthensLinkSharedDetails oaDetails)
        {
            var model = new CreateOpenAthensLinkToLhUser
            {
                OaUserId = oaDetails.OaUserId,
                OaOrganisationId = oaDetails.OaOrganisationId,
            };
            return this.View(model);
        }

        /// <summary>
        /// The create link for new user submit.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateLinkForNewUserSubmit(CreateOpenAthensLinkToLhUser model)
        {
            // Make sure the email address doesn't already exist.
            if (this.ModelState.IsValid && await this.userService.DoesEmailAlreadyExist(model.EmailAddress))
            {
                this.ModelState.AddModelError("EmailAddress", "The email address already exists.");
            }

            // Check create user form and if success create user and link OA user to it
            if (!this.ModelState.IsValid)
            {
                return this.View("CreateLinkForNewUser", model);
            }

            var userId = await this.userService.CreateElfhAccountWithLinkedOpenAthensAsync(model);

            if (userId < 1)
            {
                this.Logger.LogError($"Failed to create an ELFH/LH user with OA details. User email: {model.EmailAddress} | OA userId: {model.OaUserId} | OA orgId: {model.OaOrganisationId}");
                throw new Exception("Unable to create user with OA details.");
            }

            // Log user in and forward to Dashboard (Login wizard to take over for additional info)
            return this.Login(this.Url.Action("AuthorisationRequired", "Account", new { originalUrl = "/" }, "https"));
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="returnUrl">The return url.</param>
        /// <param name="invalidScope">The invalid scope.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Login(string returnUrl, bool invalidScope = false)
        {
            if (invalidScope)
            {
                return this.View();
            }

            if (string.IsNullOrWhiteSpace(this.authConfig.ClientId) || string.IsNullOrWhiteSpace(this.Settings.LearningHubWebUiUrl))
            {
                throw new Exception("ClientId or origin are empty.");
            }

            var authUri = OpenAthensOpenIdConnect.GetAuthServerUri(this.authConfig, this.Settings, returnUrl);

            return this.Redirect(authUri.AbsoluteUri);
        }
    }
}