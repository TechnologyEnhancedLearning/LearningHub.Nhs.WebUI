namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Extensions;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.SimplifiedRegistration;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// Defines the <see cref="SimplifiedRegistrationController" />.
    /// </summary>
    public class SimplifiedRegistrationController : BaseController
    {
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedRegistrationController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="httpClientFactory">The httpClientFactory<see cref="IHttpClientFactory"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{SimplifiedRegistrationController}"/>.</param>
        /// <param name="moodleBridgeApiService">The moodleBridgeApiService.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        /// <param name="multiPageFormService">The multiPageFormService<see cref="IMultiPageFormService"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        public SimplifiedRegistrationController(
            IWebHostEnvironment hostingEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<AccountController> logger,
            IMoodleBridgeApiService moodleBridgeApiService,
            IOptions<Settings> settings,
            IMultiPageFormService multiPageFormService,
            IUserService userService)
        : base(hostingEnvironment, httpClientFactory, logger, moodleBridgeApiService, settings.Value)
        {
            this.multiPageFormService = multiPageFormService;
            this.userService = userService;
        }

        /// <summary>
        /// Create an account page.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> CreateAnAccount()
        {
            this.TempData.Clear();
            var newAccount = new AccountCreationViewModel
            {
            };

            await this.multiPageFormService.SetMultiPageFormData(
                newAccount,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                this.TempData);

            return this.RedirectToAction("EmailVerification");
        }

        /// <summary>
        /// Create an account page.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult EmailVerification()
        {
            return this.View(new EmailValidateViewModel { });
        }

        /// <summary>
        /// Email verification.
        /// </summary>
        /// <param name="emailValidateViewModel">emailvalidatemodel.</param>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> EmailVerification(EmailValidateViewModel emailValidateViewModel, bool? returnToConfirmation = false)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(emailValidateViewModel);
            }

            var ipAddress = this.Request.GetUserIPAddress();

            var registrationStatus = await this.userService.GetEmailAddressRegistrationStatusAsync(emailValidateViewModel.Email, ipAddress);
            switch (registrationStatus.ToString())
            {
                case "ExistingUserIsEligible":
                    return this.RedirectToAction("CreateAccountValidAccountAlreadyExists");
                case "NewUserNotEligible":
                    return this.RedirectToAction("CreateAccountInvalidAccountDoesntExist");
                case "NewUserIsEligible":
                    var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    accountCreation.EmailAddress = emailValidateViewModel.Email;
                    accountCreation.AccountCreationType = AccountCreationTypeEnum.FullAccess;
                    await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    return this.RedirectToAction("PersonalDetails");

                case "NewGeneralUserIsEligible":
                    var generalAccountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    generalAccountCreation.AccountCreationType = AccountCreationTypeEnum.GeneralAccess;
                    generalAccountCreation.EmailAddress = emailValidateViewModel.Email;
                    await this.multiPageFormService.SetMultiPageFormData(generalAccountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    return this.RedirectToAction("PersonalDetails");
                case "NonUKLocation":
                    return this.RedirectToAction("RestrictedLocation");
                default:
                    return this.RedirectToAction("CreateAccountInvalidAccountAlreadyExists");
            }
        }

        /// <summary>
        /// The create an account restricted location.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("RestrictedLocation")]
        public async Task<IActionResult> RestrictedLocation()
        {
            return this.View();
        }

        /// <summary>
        /// The user doesn't have an account and would not have permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("InvalidAccountDoesntExist")]
        public IActionResult CreateAccountInvalidAccountDoesntExist()
        {
            this.ViewBag.ELfhHubUrl = this.Settings.ELfhHubUrl;
            this.ViewBag.SupportUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// The user already has an account but does not have permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("InvalidAccountAlreadyExists")]
        public IActionResult CreateAccountInvalidAccountAlreadyExists()
        {
            this.ViewBag.ELfhHubUrl = this.Settings.ELfhHubUrl;
            this.ViewBag.SupportUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// Personal Details.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("PersonalDetails")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> PersonalDetails()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
            return this.View(new PersonalDetails
            {
                FirstName = accountCreation.FirstName,
                LastName = accountCreation.LastName,
                PrimaryEmailAddress = accountCreation.EmailAddress,
            });
        }

        /// <summary>
        /// Personal Details.
        /// </summary>
        /// <param name="personalDetailsViewModel">personalDetailsViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("PersonalDetails")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> PersonalDetails(PersonalDetails personalDetailsViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
            bool valid = this.TryValidateModel(personalDetailsViewModel);

            if (!valid)
            {
                return this.View(personalDetailsViewModel);
            }

            accountCreation.FirstName = personalDetailsViewModel.FirstName.Trim();
            accountCreation.LastName = personalDetailsViewModel.LastName.Trim();
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            var request = new SimplifiedRegistrationRequestViewModel
            {
                UserRegistrationType = (UserRegistrationTypeEnum)accountCreation.AccountCreationType,
                EmailAddress = accountCreation.EmailAddress,
                FirstName = accountCreation.FirstName,
                LastName = accountCreation.LastName,
                SelfRegistration = true,
            };
            var response = await this.userService.SimplifiedRegisterNewUser(request);
            if (!response.IsValid)
            {
                var errorMessage = string.Join(", ", response.Details);
                this.ModelState.AddModelError(string.Empty, errorMessage);
                return this.View(personalDetailsViewModel);
            }
            else
            {
                await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                this.TempData.Clear();
            }

            return this.RedirectToAction("CheckYourEmail", new { accountCreation.EmailAddress });
        }

        /// <summary>
        /// The user already has an account with permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("ValidAccountAlreadyExists")]
        public IActionResult CreateAccountValidAccountAlreadyExists()
        {
            this.ViewBag.WhoCanAccessTheLearningHubUrl = this.Settings.SupportUrls.WhoCanAccessTheLearningHub;
            return this.View();
        }

        /// <summary>
        /// Check your email page.
        /// </summary>
        /// <param name="emailAddress">email address.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult CheckYourEmail(string emailAddress)
        {
            return this.View(model: emailAddress);
        }

        /// <summary>
        /// The validate password.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="loctoken">The loctoken.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("validate-email")]
        public async Task<IActionResult> ValidatePassword(string token, string loctoken)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(loctoken))
            {
                return this.View("ValidatePassword");
            }

            var validationResult = await this.userService.ValidateUserPasswordTokenAsync(token, loctoken);
            PasswordValidateViewModel changePasswordViewModel = new PasswordValidateViewModel();

            changePasswordViewModel.Username = validationResult.UserName;
            changePasswordViewModel.Token = token;
            changePasswordViewModel.Loctoken = loctoken;
            if (validationResult.Valid)
            {
                return this.View("SetPassword", changePasswordViewModel);
            }
            else
            {
                return this.View("ValidatePassword");
            }
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="model">The model<see cref="PasswordValidateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> SetPassword(PasswordValidateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("SetPassword", model);
            }
            else
            {
                var result = await this.userService.SetUserInitialPasswordAsync(model.Token, model.Loctoken, model.NewPassword);

                if (result)
                {
                    return this.View("RegistrationSuccess");
                }
                else
                {
                    return this.View("Error");
                }
            }
        }
    }
}
