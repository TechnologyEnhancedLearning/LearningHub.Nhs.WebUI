namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Extensions;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Account;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;

    /// <summary>
    /// The account controller.
    /// </summary>
    public class AccountController : BaseController
    {
        private const int UserRegistrationContentPageSize = 10;
        private readonly LearningHubAuthServiceConfig authConfig;
        private readonly IJobRoleService jobRoleService;
        private readonly ITermsAndConditionsService termsAndConditionService;
        private readonly IUserService userService;
        private readonly ICountryService countryService;
        private readonly IRegionService regionService;
        private readonly ICacheService cacheService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IGradeService gradeService;
        private readonly ISpecialtyService specialtyService;
        private readonly ILocationService locationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The httpClientFactory<see cref="IHttpClientFactory"/>.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{AccountController}"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="termsAndConditionService">The termsAndConditionService<see cref="ITermsAndConditionsService"/>.</param>
        /// <param name="jobRoleService">The jobRoleService<see cref="IJobRoleService"/>.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        /// <param name="authConfig">The authConfig<see cref="LearningHubAuthServiceConfig"/>.</param>
        /// <param name="cacheService">The cacheService<see cref="ICacheService"/>.</param>
        /// <param name="countryService">The countryService.</param>
        /// <param name="regionService">The regionService.</param>
        /// <param name="multiPageFormService">The multiPageFormService<see cref="IMultiPageFormService"/>.</param>
        /// <param name="specialtyService">The specialtyService.</param>
        /// <param name="locationService">The locationService.</param>
        /// <param name="gradeService">The gradeService.</param>
        public AccountController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<AccountController> logger,
            IUserService userService,
            ITermsAndConditionsService termsAndConditionService,
            IJobRoleService jobRoleService,
            IOptions<Settings> settings,
            LearningHubAuthServiceConfig authConfig,
            ICacheService cacheService,
            ICountryService countryService,
            IRegionService regionService,
            IMultiPageFormService multiPageFormService,
            ISpecialtyService specialtyService,
            ILocationService locationService,
            IGradeService gradeService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.authConfig = authConfig;
            this.userService = userService;
            this.termsAndConditionService = termsAndConditionService;
            this.jobRoleService = jobRoleService;
            this.cacheService = cacheService;
            this.multiPageFormService = multiPageFormService;
            this.countryService = countryService;
            this.regionService = regionService;
            this.gradeService = gradeService;
            this.specialtyService = specialtyService;
            this.locationService = locationService;
        }

        /// <summary>
        /// Gets the TwitterFeedKey
        /// The twitter feed key.
        /// </summary>
        public static string TwitterFeedKey => "TwitterFeeds";

        /// <summary>
        /// This method acts as an endpoint that unauthorised pages can navigate to in order to trigger authentication.
        /// e.g. the Resource page shows a sign in button to unauthorised users, that button calls this method.
        /// After authentication has taken place via the index screen the Auth service will automatically return to this URL
        /// afterwards, and we can pull out the originalUrl parameter from the query string and redirect to it.
        /// </summary>
        /// <param name="originalUrl">The URL of the page to redirect to after successful authentication.</param>
        /// <returns>Action result.</returns>
        [Authorize]
        public IActionResult AuthorisationRequired(string originalUrl)
        {
            if (!string.IsNullOrEmpty(originalUrl))
            {
                return this.Redirect(originalUrl);
            }
            else
            {
                return this.BadRequest("No originalUrl parameter was supplied.");
            }
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="model">The model<see cref="PasswordValidateViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordValidateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ChangePassword", model);
            }
            else
            {
                var result = await this.userService.SetUserInitialPasswordAsync(model.Token, model.Loctoken, model.NewPassword);

                if (result)
                {
                    return this.View("ChangePasswordAcknowledgement");
                }
                else
                {
                    return this.View("Error");
                }
            }
        }

        /// <summary>
        /// Redirect to create an account.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            return this.RedirectToAction("CreateAnAccount");
        }

        /// <summary>
        /// The create an account.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Registration/create-an-account")]
        [ResponseCache(CacheProfileName = "Never")]
        public async Task<IActionResult> CreateAnAccount()
        {
            this.TempData.Clear();
            var newAccooount = new AccountCreationViewModel
            {
            };

            await this.multiPageFormService.SetMultiPageFormData(
                newAccooount,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                this.TempData);

            return this.RedirectToAction("CreateAccountRegistrationInformation");
        }

        /// <summary>
        /// The create an account registration information.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Registration/CreateAccountRegInfo")]
        public IActionResult CreateAccountRegistrationInformation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountEmailVerification.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Registration/CreateAccountEmailVerification")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountEmailVerification()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            return this.View(new EmailValidateViewModel { Email = accountCreation.EmailAddress, ComfirmEmail = accountCreation.EmailAddress });
        }

        /// <summary>
        /// Verify User Email Account.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="emailValidateViewModel">emailValidateViewModel.</param>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        [HttpPost]
        [Route("Registration/CreateAccountEmailVerification")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountEmailVerification(EmailValidateViewModel emailValidateViewModel, bool? returnToConfirmation = false)
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
                    if (returnToConfirmation == true && accountCreation.AccountCreationType == AccountCreationTypeEnum.GeneralAccess && accountCreation.EmailAddress != emailValidateViewModel.Email)
                    {
                        accountCreation.EmailAddress = emailValidateViewModel.Email;
                        accountCreation.AccountCreationType = AccountCreationTypeEnum.FullAccess;
                        await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                        return this.RedirectToAction("CreateAccountAccessChangeConfirmation");
                    }
                    else
                    {
                        accountCreation.EmailAddress = emailValidateViewModel.Email;
                        accountCreation.AccountCreationType = AccountCreationTypeEnum.FullAccess;
                        await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                        return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountFullUserConfirmation");
                    }

                case "NewGeneralUserIsEligible":
                    var generalAccountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    generalAccountCreation.AccountCreationType = AccountCreationTypeEnum.GeneralAccess;
                    generalAccountCreation.EmailAddress = emailValidateViewModel.Email;
                    await this.multiPageFormService.SetMultiPageFormData(generalAccountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    return this.RedirectToAction("CreateAccountGeneralUserConfirmation");
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
        [Route("Registration/RestrictedLocation")]
        public async Task<IActionResult> RestrictedLocation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountGeneralUserConfirmation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Registration/CreateAccountGeneralUserConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountGeneralUserConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountGeneralUserConfirmation.
        /// </summary>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("Registration/CreateAccountGeneralUserConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountGeneralUserConfirmation(bool? returnToConfirmation = false)
        {
            return this.RedirectToAction("CreateAccountPersonalDetails");
        }

        /// <summary>
        /// CreateAccountFullUserConfirmation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Registration/CreateAccountFullUserConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountFullUserConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountFullUserConfirmation.
        /// </summary>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("Registration/CreateAccountFullUserConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountFullUserConfirmation(bool? returnToConfirmation = false)
        {
            return this.RedirectToAction("CreateAccountPersonalDetails");
        }

        /// <summary>
        /// CreateAccountAccessChangeConfirmation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("Registration/CreateAccountAccessChangeConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountAccessChangeConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountAccessChangeConfirmation.
        /// </summary>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("Registration/CreateAccountAccessChangeConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountAccessChangeConfirmation(bool? returnToConfirmation = false)
        {
            return this.RedirectToAction("CreateAccountFullUserConfirmation");
        }

        /// <summary>
        /// Create Personal Details.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Registration/CreateAccountPersonalDetails")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<ActionResult> CreateAccountPersonalDetails()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
            return this.View(new PersonalDetails
            {
                FirstName = accountCreation.FirstName,
                LastName = accountCreation.LastName,
                PrimaryEmailAddress = accountCreation.EmailAddress,
                SecondaryEmailAddress = accountCreation.SecondaryEmailAddress,
                IsLoginWizard = accountCreation.IsLoginWizard,
            });
        }

        /// <summary>
        /// Create Personal Details.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="personalDetailsViewModel">personalDetailsViewModel.</param>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        [HttpPost]
        [Route("Registration/CreateAccountPersonalDetails")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountPersonalDetails(PersonalDetails personalDetailsViewModel, bool? returnToConfirmation = false)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
            bool valid = this.TryValidateModel(personalDetailsViewModel);

            if (!valid)
            {
                return this.View(personalDetailsViewModel);
            }

            var personalDetails = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            personalDetails.FirstName = personalDetailsViewModel.FirstName.Trim();
            personalDetails.LastName = personalDetailsViewModel.LastName.Trim();
            personalDetails.SecondaryEmailAddress = personalDetailsViewModel.SecondaryEmailAddress?.Trim();
            await this.multiPageFormService.SetMultiPageFormData(personalDetails, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = personalDetails.LocationId }) : this.RedirectToAction("CreateAccountCountrySearch");
        }

        /// <summary>
        /// Country Search.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Registration/CreateAccountCountrySearch")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountCountrySearch()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (!string.IsNullOrWhiteSpace(accountCreation.CountryId) && !this.CheckConfirmationUpdate())
            {
                return this.RedirectToAction("CreateAccountCountrySelection", new AccountCreationViewModel { CountryId = accountCreation.CountryId });
            }

            return this.View(new AccountCreationViewModel());
        }

        /// <summary>
        /// Select country.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountCountrySelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountCountrySelection(AccountCreationViewModel accountCreationViewModel)
        {
            var accountDetails = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (!string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                string filterText = Regex.Replace(accountCreationViewModel.FilterText, "[:!@#$%^&*()}{|\":?><\\[\\]\\;'/.,~\\\"\"\\'\\\\/]", " ");
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                    return this.View("CreateAccountCountrySearch", accountCreationViewModel);
                }
            }

            if (string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                if (!string.IsNullOrWhiteSpace(accountDetails.CountryId))
                {
                    var country = await this.countryService.GetByIdAsync(int.Parse(accountDetails.CountryId));
                    accountCreationViewModel.FilterText = country.Name;
                }
                else
                {
                    this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                    return this.View("CreateAccountCountrySearch", accountCreationViewModel);
                }
            }

            var countries = await this.countryService.GetFilteredAsync(accountCreationViewModel.FilterText);
            return this.View(new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, CountryList = countries.ToList(), CountryId = accountDetails.CountryId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Select Region.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountRegionSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountRegionSelection(AccountCreationViewModel accountCreationViewModel)
        {
            var countryCheck = int.TryParse(accountCreationViewModel.CountryId, out int countryId);
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (string.IsNullOrWhiteSpace(accountCreationViewModel.CountryId) || !countryCheck)
            {
                this.ModelState.AddModelError("CountryId", CommonValidationErrorMessages.CountryRequired);
                var countries = await this.countryService.GetFilteredAsync(accountCreationViewModel.FilterText);
                return this.View("CreateAccountCountrySelection", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, CountryList = countries.ToList(), ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
            }

            accountCreation.CountryId = accountCreationViewModel.CountryId;
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (countryId != 1)
            {
                accountCreationViewModel.RegionId = string.Empty;
                accountCreation.RegionId = string.Empty;
                await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

                if (accountCreation.IsLoginWizard)
                {
                    return this.CheckConfirmationUpdate() ? this.RedirectToAction("AccountConfirmation", "LoginWizard") : this.RedirectToAction("NextStage", "LoginWizard");
                }

                return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountSearchRole", accountCreationViewModel);
            }

            switch (accountCreationViewModel.AccountCreationPagingEnum)
            {
                case AccountCreationPagingEnum.NextPageChange:
                    accountCreationViewModel.CurrentPageIndex += 1;
                    break;

                case AccountCreationPagingEnum.PreviousPageChange:
                    accountCreationViewModel.CurrentPageIndex -= 1;
                    break;
                case AccountCreationPagingEnum.Default:
                    accountCreationViewModel.CurrentPageIndex = 1;
                    break;
            }

            var region = await this.regionService.GetAllPagedAsync(accountCreationViewModel.CurrentPageIndex, UserRegistrationContentPageSize);
            return this.View(new AccountCreationListViewModel { Region = region.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = region.Item1, PageSize = UserRegistrationContentPageSize, HasItems = region.Item1 > 0, CurrentPage = accountCreationViewModel.CurrentPageIndex }, RegionId = accountCreation.RegionId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Submit Region selection.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [HttpPost]
        [Route("Registration/CreateAccountSubmitRegionSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountSubmitRegionSelection(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (string.IsNullOrWhiteSpace(accountCreationViewModel.RegionId))
            {
                if (accountCreation.CountryId == "1")
                {
                    this.ModelState.AddModelError("RegionId", CommonValidationErrorMessages.RegionRequired);
                    var region = await this.regionService.GetAllPagedAsync(1, UserRegistrationContentPageSize);
                    return this.View("CreateAccountRegionSelection", new AccountCreationListViewModel { CountryId = "1", Region = region.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = region.Item1, PageSize = UserRegistrationContentPageSize, HasItems = region.Item1 > 0, CurrentPage = 1 } });
                }
            }

            accountCreation.RegionId = accountCreationViewModel.RegionId;
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (!string.IsNullOrWhiteSpace(accountCreation.CurrentRole) && !this.CheckConfirmationUpdate())
            {
                return this.RedirectToAction("CreateAccountCurrentRole", new AccountCreationViewModel { CurrentRole = accountCreation.CurrentRole, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
            }

            if (accountCreation.IsLoginWizard)
            {
                return this.CheckConfirmationUpdate() ? this.RedirectToAction("AccountConfirmation", "LoginWizard") : this.RedirectToAction("NextStage", "LoginWizard");
            }

            return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountSearchRole", new AccountCreationViewModel() { CountryId = accountCreation.CountryId });
        }

        /// <summary>
        /// Search User Role.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountSearchRole")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountSearchRole(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            return this.View("CreateAccountSearchRole", new AccountCreationViewModel { CountryId = accountCreationViewModel.CountryId });
        }

        /// <summary>
        /// Select Current Role.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountCurrentRole")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountCurrentRole(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (!string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                string filterText = Regex.Replace(accountCreationViewModel.FilterText, "[:!@#$%^&*()}{|\":?><\\[\\]\\;'/.,~\\\"\"\\'\\\\/]", " ");
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                    return this.View("CreateAccountSearchRole", new AccountCreationViewModel { RegionId = accountCreation.RegionId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
                }
            }

            if (string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                var currentJobRole = int.TryParse(accountCreation.CurrentRole, out int currentRole);
                if (currentJobRole && currentRole > 0)
                {
                    var filterText = await this.jobRoleService.GetByIdAsync(currentRole);
                    accountCreationViewModel.FilterText = filterText.Name;
                    var jobrole = await this.jobRoleService.GetByIdAsync(currentRole);
                    return this.View("CreateAccountCurrentRole", new AccountCreationListViewModel { RoleList = new List<JobRoleBasicViewModel> { jobrole }, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = 1, PageSize = UserRegistrationContentPageSize, HasItems = jobrole != null, CurrentPage = 1 }, CurrentRole = accountCreation.CurrentRole, CountryId = accountCreation.CountryId, RegionId = accountCreation.RegionId, FilterText = accountCreationViewModel.FilterText, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
                }
                else
                {
                    this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                    return this.View("CreateAccountSearchRole", new AccountCreationViewModel { RegionId = accountCreation.RegionId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
                }
            }

            switch (accountCreationViewModel.AccountCreationPagingEnum)
            {
                case AccountCreationPagingEnum.NextPageChange:
                    accountCreationViewModel.CurrentPageIndex += 1;
                    break;

                case AccountCreationPagingEnum.PreviousPageChange:
                    accountCreationViewModel.CurrentPageIndex -= 1;
                    break;
                case AccountCreationPagingEnum.Default:
                    accountCreationViewModel.CurrentPageIndex = 1;
                    break;
            }

            var jobroles = await this.jobRoleService.GetPagedFilteredAsync(accountCreationViewModel.FilterText, accountCreationViewModel.CurrentPageIndex, UserRegistrationContentPageSize);
            return this.View("CreateAccountCurrentRole", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, RoleList = jobroles.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = jobroles.Item1, PageSize = UserRegistrationContentPageSize, HasItems = jobroles.Item1 > 0, CurrentPage = accountCreationViewModel.CurrentPageIndex }, CurrentRole = accountCreation.CurrentRole, CountryId = accountCreation.CountryId, RegionId = accountCreation.RegionId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Submit Professional Registration Number.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountProfessionalRegNumber")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountProfessionalRegNumber(AccountCreationViewModel accountCreationViewModel)
        {
            var roleCheck = int.TryParse(accountCreationViewModel.CurrentRole, out int roleId);
            if (string.IsNullOrWhiteSpace(accountCreationViewModel.CurrentRole) || !roleCheck)
            {
                this.ModelState.AddModelError("CurrentRole", CommonValidationErrorMessages.RoleRequired);
                var jobroles = await this.jobRoleService.GetPagedFilteredAsync(accountCreationViewModel.FilterText, 1, UserRegistrationContentPageSize);
                return this.View("CreateAccountCurrentRole", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, RoleList = jobroles.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = jobroles.Item1, PageSize = UserRegistrationContentPageSize, HasItems = jobroles.Item1 > 0, CurrentPage = 1 }, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
            }

            var jobrole = await this.jobRoleService.GetByIdAsync(roleId);
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            accountCreation.CurrentRole = jobrole.Id.ToString();
            accountCreation.CurrentRoleName = jobrole.Name;
            accountCreation.MedicalCouncilId = jobrole.MedicalCouncilId;
            accountCreation.MedicalCouncilName = jobrole.MedicalCouncilName;
            accountCreation.MedicalCouncilCode = jobrole.MedicalCouncilCode;

            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.Job = jobrole;
            return jobrole.MedicalCouncilId > 0 ? this.View(new AccountCreationViewModel { RegistrationNumber = accountCreation.RegistrationNumber, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation }) : this.RedirectToAction("CreateAccountGradeSelection", new AccountCreationViewModel { ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Select Grade.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountGradeSelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountGradeSelection(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            int gradePageSize = UserRegistrationContentPageSize + 5;
            var roleCheck = int.TryParse(accountCreation.CurrentRole, out int roleId);
            if (!roleCheck || roleId == 0)
            {
                this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                return this.View("CreateAccountSearchRole", new AccountCreationViewModel { RegionId = accountCreation.RegionId });
            }

            if (string.IsNullOrWhiteSpace(accountCreationViewModel.RegistrationNumber) && accountCreation.MedicalCouncilId.HasValue && (int)accountCreation.MedicalCouncilId > 0)
            {
                this.ModelState.AddModelError("RegistrationNumber", $"You must provide a {accountCreation.MedicalCouncilCode} Number");
                this.ViewBag.Job = await this.jobRoleService.GetByIdAsync(roleId);
                accountCreationViewModel.CurrentRole = roleId.ToString();
                return this.View("CreateAccountProfessionalRegNumber", accountCreationViewModel);
            }

            if (accountCreation.MedicalCouncilId.HasValue && (int)accountCreation.MedicalCouncilId > 0 && (int)accountCreation.MedicalCouncilId < 4)
            {
                string validateMedicalCouncilNumber = await this.jobRoleService.ValidateMedicalCouncilNumber(accountCreation.LastName, (int)accountCreation.MedicalCouncilId, accountCreationViewModel.RegistrationNumber);
                if (!string.IsNullOrWhiteSpace(validateMedicalCouncilNumber))
                {
                    this.ModelState.AddModelError("RegistrationNumber", validateMedicalCouncilNumber);
                    this.ViewBag.Job = await this.jobRoleService.GetByIdAsync(roleId);
                    return this.View("CreateAccountProfessionalRegNumber", accountCreationViewModel);
                }
            }

            accountCreation.RegistrationNumber = accountCreationViewModel.RegistrationNumber;
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (accountCreation.AccountCreationType == AccountCreationTypeEnum.GeneralAccess)
            {
                return this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { PrimaryUserEmploymentId = accountCreation.PrimaryUserEmploymentId });
            }

            switch (accountCreationViewModel.AccountCreationPagingEnum)
            {
                case AccountCreationPagingEnum.NextPageChange:
                    accountCreationViewModel.CurrentPageIndex += 1;
                    break;

                case AccountCreationPagingEnum.PreviousPageChange:
                    accountCreationViewModel.CurrentPageIndex -= 1;
                    break;
                case AccountCreationPagingEnum.Default:
                    accountCreationViewModel.CurrentPageIndex = 1;
                    break;
            }

            var gradeLevel = await this.gradeService.GetPagedGradesForJobRoleAsync(roleId, accountCreationViewModel.CurrentPageIndex, gradePageSize);
            return this.View(new AccountCreationListViewModel { GradeList = gradeLevel.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = gradeLevel.Item1, PageSize = gradePageSize, HasItems = gradeLevel.Item1 > 0, CurrentPage = accountCreationViewModel.CurrentPageIndex }, CurrentRole = accountCreation.CurrentRole, GradeId = accountCreation.GradeId, MedicalCouncilCode = accountCreation.MedicalCouncilCode, MedicalCouncilId = accountCreation.MedicalCouncilId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Search Primary Specialty.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountPrimarySpecialty")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountPrimarySpecialty(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            var gradeCheck = int.TryParse(accountCreationViewModel.GradeId, out int gradeId);
            if (string.IsNullOrWhiteSpace(accountCreationViewModel.GradeId) || !gradeCheck)
            {
                int gradePageSize = UserRegistrationContentPageSize + 5;
                this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.GradeRequired);
                var gradeLevel = await this.gradeService.GetPagedGradesForJobRoleAsync(int.Parse(accountCreation.CurrentRole), 1, gradePageSize);
                return this.View("CreateAccountGradeSelection", new AccountCreationListViewModel { GradeList = gradeLevel.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = gradeLevel.Item1, PageSize = gradePageSize, HasItems = gradeLevel.Item1 > 0, CurrentPage = 1 }, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
            }

            accountCreation.GradeId = accountCreationViewModel.GradeId;
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (!string.IsNullOrWhiteSpace(accountCreation.PrimarySpecialtyId) && !this.CheckConfirmationUpdate())
            {
                return this.RedirectToAction("CreateAccountPrimarySpecialtySelection", new AccountCreationViewModel { PrimarySpecialtyId = accountCreation.PrimarySpecialtyId });
            }

            return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.View(new AccountCreationViewModel() { ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation, GradeId = accountCreation.GradeId, CurrentRole = accountCreation.CurrentRole, RegistrationNumber = accountCreation.RegistrationNumber });
        }

        /// <summary>
        /// Primary Specialty Selection.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [HttpGet]
        [Route("Registration/CreateAccountPrimarySpecialtySelection")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountPrimarySpecialtySelection(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (accountCreationViewModel.PrimarySpecialtyId?.ToLower() == "not applicable")
            {
                var specialties = await this.specialtyService.GetSpecialtiesAsync();
                accountCreation.PrimarySpecialtyId = specialties.FirstOrDefault(x => x.Name.ToLower() == "not applicable").Id.ToString();
                await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

                // do a return to confirmation check here
                return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountWorkStartDate", new AccountCreationViewModel { PrimarySpecialtyId = accountCreation.PrimarySpecialtyId, FilterText = accountCreationViewModel.FilterText });
            }

            var optionalSpecialty = await this.specialtyService.GetSpecialtiesAsync();
            if (!string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                string filterText = Regex.Replace(accountCreationViewModel.FilterText, "[:!@#$%^&*()}{|\":?><\\[\\]\\;'/.,~\\\"\"\\'\\\\/]", " ");
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    this.ModelState.AddModelError("PrimarySpecialtyId", CommonValidationErrorMessages.SpecialtyNotApplicable);
                    accountCreationViewModel.RegistrationNumber = accountCreation.RegistrationNumber;
                    accountCreationViewModel.CurrentRole = accountCreation.CurrentRole;
                    return this.View("CreateAccountPrimarySpecialty", accountCreationViewModel);
                }
            }

            if (string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                if (!string.IsNullOrWhiteSpace(accountCreation.PrimarySpecialtyId))
                {
                    var specialties = await this.specialtyService.GetSpecialtiesAsync();
                    var selectedPrimarySpecialty = specialties.FirstOrDefault(x => x.Id.ToString() == accountCreation.PrimarySpecialtyId);
                    accountCreationViewModel.FilterText = selectedPrimarySpecialty.Name;
                    return this.View(new AccountCreationListViewModel { GradeId = accountCreation.GradeId, RegistrationNumber = accountCreation.RegistrationNumber, FilterText = accountCreationViewModel.FilterText, SpecialtyList = new List<GenericListViewModel> { selectedPrimarySpecialty }, OptionalSpecialtyItem = optionalSpecialty.FirstOrDefault(x => x.Name.ToLower() == "not applicable"), AccountCreationPaging = new AccountCreationPagingModel { TotalItems = 1, PageSize = UserRegistrationContentPageSize, HasItems = selectedPrimarySpecialty != null, CurrentPage = 1 }, PrimarySpecialtyId = accountCreation.PrimarySpecialtyId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
                }

                this.ModelState.AddModelError("PrimarySpecialtyId", CommonValidationErrorMessages.SpecialtyNotApplicable);
                accountCreationViewModel.RegistrationNumber = accountCreation.RegistrationNumber;
                accountCreationViewModel.CurrentRole = accountCreation.CurrentRole;
                return this.View("CreateAccountPrimarySpecialty", accountCreationViewModel);
            }

            switch (accountCreationViewModel.AccountCreationPagingEnum)
            {
                case AccountCreationPagingEnum.NextPageChange:
                    accountCreationViewModel.CurrentPageIndex += 1;
                    break;

                case AccountCreationPagingEnum.PreviousPageChange:
                    accountCreationViewModel.CurrentPageIndex -= 1;
                    break;
                case AccountCreationPagingEnum.Default:
                    accountCreationViewModel.CurrentPageIndex = 1;
                    break;
            }

            var shortlist = await this.specialtyService.GetPagedSpecialtiesAsync(accountCreationViewModel.FilterText, accountCreationViewModel.CurrentPageIndex, UserRegistrationContentPageSize);
            if (shortlist.Item1 < 1)
            {
                this.ModelState.AddModelError("PrimarySpecialtyId", CommonValidationErrorMessages.SpecialtyNotApplicable);
            }

            return this.View(new AccountCreationListViewModel { GradeId = accountCreation.GradeId, RegistrationNumber = accountCreation.RegistrationNumber, FilterText = accountCreationViewModel.FilterText, SpecialtyList = shortlist.Item2, OptionalSpecialtyItem = optionalSpecialty.FirstOrDefault(x => x.Name.ToLower() == "not applicable"), AccountCreationPaging = new AccountCreationPagingModel { TotalItems = shortlist.Item1, PageSize = UserRegistrationContentPageSize, HasItems = shortlist.Item1 > 0, CurrentPage = accountCreationViewModel.CurrentPageIndex }, PrimarySpecialtyId = accountCreation.PrimarySpecialtyId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Submit Work Start Date.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [HttpGet]
        [Route("Registration/CreateAccountWorkStartDate")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountWorkStartDate(AccountCreationViewModel accountCreationViewModel)
        {
            int specialtyId;
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (accountCreationViewModel.PrimarySpecialtyId?.ToLower() == "not applicable")
            {
                var specialties = await this.specialtyService.GetSpecialtiesAsync();
                specialtyId = specialties.FirstOrDefault(x => x.Name.ToLower() == "not applicable").Id;
                accountCreation.PrimarySpecialtyId = specialtyId.ToString();
            }
            else
            {
                var specialtyCheck = int.TryParse(accountCreationViewModel.PrimarySpecialtyId, out specialtyId);
                if (!specialtyCheck || specialtyId == 0)
                {
                    this.ModelState.AddModelError("PrimarySpecialtyId", CommonValidationErrorMessages.SpecialtyRequired);
                    var shortlist = await this.specialtyService.GetPagedSpecialtiesAsync(accountCreationViewModel.FilterText, 1, UserRegistrationContentPageSize);
                    var optionalSpecialty = await this.specialtyService.GetSpecialtiesAsync();
                    if (shortlist.Item1 < 1)
                    {
                        this.ModelState.AddModelError("PrimarySpecialtyId", CommonValidationErrorMessages.SpecialtyNotApplicable);
                    }

                    return this.View("CreateAccountPrimarySpecialtySelection", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation, SpecialtyList = shortlist.Item2, OptionalSpecialtyItem = optionalSpecialty.FirstOrDefault(x => x.Name.ToLower() == "not applicable"), AccountCreationPaging = new AccountCreationPagingModel { TotalItems = shortlist.Item1, PageSize = UserRegistrationContentPageSize, HasItems = shortlist.Item1 > 0, CurrentPage = 1 } });
                }

                accountCreation.PrimarySpecialtyId = accountCreationViewModel.PrimarySpecialtyId;
            }

            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            var dateVM = accountCreation.StartDate.HasValue ? new AccountCreationDateViewModel() { Day = accountCreation.StartDate.Value.Day, Month = accountCreation.StartDate.GetValueOrDefault().Month, Year = accountCreation.StartDate.Value.Year, FilterText = accountCreationViewModel.FilterText, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation } : new AccountCreationDateViewModel() { FilterText = accountCreationViewModel.FilterText, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation };
            if (!string.IsNullOrWhiteSpace(accountCreationViewModel.PrimarySpecialtyId) && string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                var specialty = this.specialtyService.GetSpecialtiesAsync().Result.FirstOrDefault(x => x.Id == specialtyId);
                this.ViewBag.FilterText = specialty.Name;
            }

            return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.View(dateVM);
        }

        /// <summary>
        /// Submit  Work Start Date.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationDateViewModel">accountCreationDateViewModel.</param>
        [HttpPost]
        [Route("Registration/CreateAccountStartDate")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountStartDate(AccountCreationDateViewModel accountCreationDateViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("CreateAccountWorkStartDate", accountCreationDateViewModel);
            }

            if (accountCreationDateViewModel.Day == null || accountCreationDateViewModel.Month == null || accountCreationDateViewModel.Year == null || !accountCreationDateViewModel.GetDate().HasValue)
            {
                this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.StartDate);
                return this.View("CreateAccountWorkStartDate", accountCreationDateViewModel);
            }

            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            accountCreation.StartDate = accountCreationDateViewModel.GetDate().Value;
            await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountWorkPlaceSearch", new AccountCreationViewModel { PrimarySpecialtyId = accountCreationDateViewModel.PrimarySpecialtyId, FilterText = accountCreationDateViewModel.FilterText, });
        }

        /// <summary>
        ///  Work Start Date.
        /// </summary>
        /// <param name="returnToConfirmation">returnToConfirmation.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("Registration/CreateAccountStartDate")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountStartDate(bool? returnToConfirmation = false)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (!string.IsNullOrWhiteSpace(accountCreation.PrimarySpecialtyId))
            {
                int specialtyId = 0;
                var specialtyCheck = int.TryParse(accountCreation.PrimarySpecialtyId, out specialtyId);
                if (specialtyId > 0)
                {
                    var specialty = this.specialtyService.GetSpecialtiesAsync().Result.FirstOrDefault(x => x.Id == specialtyId);
                    this.ViewBag.FilterText = specialty.Name;
                }
            }

            var dateVM = accountCreation.StartDate.HasValue ? new AccountCreationDateViewModel() { Day = accountCreation.StartDate.Value.Day, Month = accountCreation.StartDate.GetValueOrDefault().Month, Year = accountCreation.StartDate.Value.Year, ReturnToConfirmation = returnToConfirmation } : new AccountCreationDateViewModel() { ReturnToConfirmation = returnToConfirmation };
            return this.View("CreateAccountWorkStartDate", dateVM);
        }

        /// <summary>
        /// Search work place.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Registration/CreateAccountWorkPlaceSearch")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountWorkPlaceSearch()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (!string.IsNullOrWhiteSpace(accountCreation.LocationId) && !this.CheckConfirmationUpdate())
            {
                return this.RedirectToAction("CreateAccountWorkPlace", new AccountCreationViewModel { LocationId = accountCreation.LocationId });
            }

            return this.View(new AccountCreationViewModel { PrimarySpecialtyId = accountCreation.PrimarySpecialtyId });
        }

        /// <summary>
        /// Select workplace.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountWorkPlace")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountWorkPlace(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            if (string.IsNullOrWhiteSpace(accountCreationViewModel.FilterText))
            {
                if (!string.IsNullOrWhiteSpace(accountCreation.LocationId))
                {
                    var selectedLocation = await this.locationService.GetByIdAsync(int.Parse(accountCreation.LocationId));
                    return this.View(new AccountCreationListViewModel { WorkPlaceList = new List<LocationBasicViewModel> { selectedLocation }, FilterText = selectedLocation.Name, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = 1, PageSize = UserRegistrationContentPageSize, HasItems = selectedLocation != null, CurrentPage = 1 }, LocationId = accountCreation.LocationId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
                }

                accountCreationViewModel.PrimarySpecialtyId = accountCreation.PrimarySpecialtyId;
                this.ModelState.AddModelError("FilterText", CommonValidationErrorMessages.SearchTermRequired);
                return this.View("CreateAccountWorkPlaceSearch", accountCreationViewModel);
            }

            switch (accountCreationViewModel.AccountCreationPagingEnum)
            {
                case AccountCreationPagingEnum.NextPageChange:
                    accountCreationViewModel.CurrentPageIndex += 1;
                    break;

                case AccountCreationPagingEnum.PreviousPageChange:
                    accountCreationViewModel.CurrentPageIndex -= 1;
                    break;
                case AccountCreationPagingEnum.Default:
                    accountCreationViewModel.CurrentPageIndex = 1;
                    break;
            }

            var location = await this.locationService.GetPagedFilteredAsync(accountCreationViewModel.FilterText, accountCreationViewModel.CurrentPageIndex, UserRegistrationContentPageSize);
            return this.View(new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, WorkPlaceList = location?.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = location.Item1, PageSize = UserRegistrationContentPageSize, HasItems = location.Item1 > 0, CurrentPage = accountCreationViewModel.CurrentPageIndex }, LocationId = accountCreation.LocationId, ReturnToConfirmation = accountCreationViewModel.ReturnToConfirmation });
        }

        /// <summary>
        /// Account confirmation view.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        [Route("Registration/CreateAccountConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountConfirmation(AccountCreationViewModel accountCreationViewModel)
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (accountCreation.AccountCreationType == AccountCreationTypeEnum.FullAccess)
            {
                var placeOfWorkCheck = int.TryParse(accountCreationViewModel.LocationId, out int locationId);
                if (!placeOfWorkCheck || locationId == 0)
                {
                    this.ModelState.AddModelError("LocationId", CommonValidationErrorMessages.WorkPlace);
                    var location = await this.locationService.GetPagedFilteredAsync(accountCreationViewModel.FilterText, 1, UserRegistrationContentPageSize);
                    return this.View("CreateAccountWorkPlace", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, WorkPlaceList = location.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = location.Item1, PageSize = UserRegistrationContentPageSize, HasItems = location.Item1 > 0, CurrentPage = 1 }, LocationId = accountCreation.LocationId });
                }

                accountCreation.LocationId = accountCreationViewModel.LocationId;
                await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            }

            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
            if (accountCreation.IsLoginWizard)
            {
                return this.RedirectToAction("AccountConfirmation", "LoginWizard");
            }

            var response = await this.GetAccountConfirmationDetails(accountCreation);
            return this.View(response);
        }

        /// <summary>
        /// Account confirmation and Create account API request.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("Registration/CreateAccountConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountConfirmation()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;

            if (accountCreation.CountryId == "1" && (string.IsNullOrWhiteSpace(accountCreation.RegionId) || accountCreation.RegionId == "0"))
            {
                this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.RegionRequiredSummary);
                var accountDetails = await this.GetAccountConfirmationDetails(accountCreation);
                return this.View(accountDetails);
            }
            else
            {
                var request = new RegistrationRequestViewModel
                {
                    UserRegistrationType = (UserRegistrationTypeEnum)accountCreation.AccountCreationType,
                    CountryId = int.TryParse(accountCreation.CountryId, out int countryId) ? countryId : 0,
                    RegionId = int.TryParse(accountCreation.RegionId, out int regionId) ? regionId : null,
                    EmailAddress = accountCreation.EmailAddress,
                    SecondaryEmailAddress = accountCreation.SecondaryEmailAddress,
                    JobRoleId = int.TryParse(accountCreation.CurrentRole, out int roleId) ? roleId : 0,
                    FirstName = accountCreation.FirstName,
                    LastName = accountCreation.LastName,
                    SelfRegistration = true,
                    GradeId = int.TryParse(accountCreation.GradeId, out int gradeId) ? gradeId : 0,
                    LocationId = int.TryParse(accountCreation.LocationId, out int primaryEmploymentId) ? primaryEmploymentId : 0,
                    LocationStartDate = accountCreation.StartDate.GetValueOrDefault(),
                    MedicalCouncilNumber = accountCreation.RegistrationNumber,
                    SpecialtyId = int.TryParse(accountCreation.PrimarySpecialtyId, out int specialtyId) ? specialtyId : 0,
                };
                var response = await this.userService.RegisterNewUser(request);
                if (!response.IsValid)
                {
                    var errorMessage = string.Join(", ", response.Details);
                    this.ModelState.AddModelError(string.Empty, errorMessage);
                    var accountDetails = await this.GetAccountConfirmationDetails(accountCreation);
                    return this.View(accountDetails);
                }
                else
                {
                    await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    this.TempData.Clear();
                }
            }

            return this.View("CreateAccountSuccess");
        }

        /// <summary>
        /// The user already has an account with permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("Registration/ValidAccountAlreadyExists")]
        public IActionResult CreateAccountValidAccountAlreadyExists()
        {
            this.ViewBag.WhoCanAccessTheLearningHubUrl = this.Settings.SupportUrls.WhoCanAccessTheLearningHub;
            return this.View();
        }

        /// <summary>
        /// The user already has an account but does not have permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("Registration/InvalidAccountAlreadyExists")]
        public IActionResult CreateAccountInvalidAccountAlreadyExists()
        {
            this.ViewBag.ELfhHubUrl = this.Settings.ELfhHubUrl;
            this.ViewBag.SupportUrl = this.Settings.SupportUrls.SupportForm;
            return this.View();
        }

        /// <summary>
        /// The user doesn't have an account and would not have permissions to access the Learning Hub.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("Registration/InvalidAccountDoesntExist")]
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
        public IActionResult InvalidUserAccount()
        {
            this.ViewBag.ELfhHubUrl = this.Settings.ELfhHubUrl.TrimEnd('/') + "/Dashboard";
            this.ViewBag.WhoCanAccessTheLearningHub = this.Settings.SupportUrls.WhoCanAccessTheLearningHub;
            return this.View();
        }

        /// <summary>
        /// The ForgotPassword.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("forgotten-password")]
        public IActionResult ForgotUserPassword()
        {
            return this.View(new Models.Account.ForgotPasswordViewModel());
        }

        /// <summary>
        /// The ForgotPassword.
        /// </summary>
        /// <param name="model">Forgotten password view model.</param>
        /// <returns>A <see cref="Task{TResult}"/>.</returns>
        [HttpPost]
        [Route("Account/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(Models.Account.ForgotPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ForgotUserPassword", model);
            }

            var hasMultipleUsers = await this.userService.HasMultipleUsersForEmailAsync(model.EmailAddress);
            if (hasMultipleUsers)
            {
                return this.Ok(new { duplicate = true });
            }

            await this.userService.ForgotPasswordAsync(model.EmailAddress);
            return this.View("ForgotPasswordAcknowledgement");
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        // [Route("Registration/{*path}")]
        public IActionResult Registration()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.Redirect("/");
            }
            else
            {
                this.ViewBag.ELfhHubUrl = this.Settings.ELfhHubUrl;
                this.ViewBag.AuthService = this.authConfig.Authority;
                this.ViewBag.WhoCanAccessUrl = this.Settings.SupportUrls.WhoCanAccessTheLearningHub;
                this.ViewBag.SupportFormUrl = this.Settings.SupportUrls.SupportForm;
                return this.View();
            }
        }

        /// <summary>
        /// The registration logon.
        /// </summary>
        /// <param name="returnUrl">The returnUrl.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="errMsg">The errMsg.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult RegistrationLogon(string returnUrl = "/", string origin = "", string errMsg = "")
        {
            var authOrigin = WebUtility.UrlDecode(origin);
            if (!authOrigin.Contains(this.authConfig.Authority))
            {
                return this.Content("Access Denied");
            }

            // model.LoginInput.ReturnUrl = _settings.LearningHubWebUiUrl;
            if (string.IsNullOrWhiteSpace(errMsg) && this.ModelState.ErrorCount > 0)
            {
                this.ModelState.Clear();
                this.ViewBag.Errors = false;
            }

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                this.ModelState.AddModelError(string.Empty, errMsg);
                this.ViewBag.Errors = true;
            }

            this.ViewBag.AuthUrl = $"{this.authConfig.Authority}/account/login";

            LoginInputModel model = new LoginInputModel();
            return this.View(model);
        }

        /// <summary>
        /// The registration not required.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            return this.View();
        }

        /// <summary>
        /// The validate password.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="loctoken">The loctoken.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("validate-password")]
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
                return this.View("ChangePassword", changePasswordViewModel);
            }
            else
            {
                return this.View("ValidatePassword");
            }
        }

        private async Task<AccountCreationConfirmation> GetAccountConfirmationDetails(AccountCreationViewModel accountCreationViewModel)
        {
            var country = await this.countryService.GetByIdAsync(int.TryParse(accountCreationViewModel.CountryId, out int countryId) ? countryId : 0);
            var employer = await this.locationService.GetByIdAsync(int.TryParse(accountCreationViewModel.LocationId, out int primaryEmploymentId) ? primaryEmploymentId : 0);
            var region = await this.regionService.GetAllAsync();
            var specialty = await this.specialtyService.GetSpecialtiesAsync();

            var role = await this.jobRoleService.GetFilteredAsync(accountCreationViewModel.CurrentRoleName);
            if (role.Count > 0)
            {
                accountCreationViewModel.CurrentRoleName = role.FirstOrDefault(x => x.Id == int.Parse(accountCreationViewModel.CurrentRole)).NameWithStaffGroup;
            }

            var grade = await this.gradeService.GetGradesForJobRoleAsync(int.TryParse(accountCreationViewModel.CurrentRole, out int roleId) ? roleId : 0);
            var confirmationPayload = new AccountCreationConfirmation
            {
                AccountCreationViewModel = accountCreationViewModel,
                Country = country?.Name,
                Employer = $"<p>{employer?.Name}</p> <br/> <p>Address:{employer?.Address}</p><br/> <p>Org Code:{employer?.NhsCode}</p>",
                Grade = grade.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.GradeId)?.Name,
                Region = region?.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.RegionId)?.Name,
                Specialty = specialty.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.PrimarySpecialtyId)?.Name,
            };
            return confirmationPayload;
        }

        private bool CheckConfirmationUpdate()
        {
            bool returnToConfirmation = false;
            StringValues rtc;
            bool formSubmission;
            if (this.HttpContext.Request.HasFormContentType)
            {
                formSubmission = this.HttpContext.Request.Form.Keys.Contains("formSubmission");
                rtc = this.HttpContext.Request.Form["returnToConfirmation"];
                if (!string.IsNullOrWhiteSpace(rtc))
                {
                    returnToConfirmation = bool.Parse(rtc);
                }
            }
            else
            {
                formSubmission = this.HttpContext.Request.Query.Keys.Contains("formSubmission");
                this.HttpContext.Request.Query.TryGetValue("returnToConfirmation", out rtc);
                if (!string.IsNullOrWhiteSpace(rtc))
                {
                    returnToConfirmation = bool.Parse(rtc);
                }
            }

            if (returnToConfirmation && formSubmission)
            {
                return true;
            }

            return false;
        }
    }
}