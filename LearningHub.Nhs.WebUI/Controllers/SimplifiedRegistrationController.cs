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
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    ////using LearningHub.Nhs.WebUI.Models.Account;
    using LearningHub.Nhs.WebUI.Models.SimplifiedRegistration;
    using LearningHub.Nhs.WebUI.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using NHSUKViewComponents.Web.ViewModels;

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
                    if (returnToConfirmation == true && accountCreation.AccountCreationType == AccountCreationTypeEnum.GeneralAccess && accountCreation.EmailAddress != emailValidateViewModel.Email)
                    {
                        accountCreation.EmailAddress = emailValidateViewModel.Email;
                        accountCreation.AccountCreationType = AccountCreationTypeEnum.FullAccess;
                        await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                        return this.RedirectToAction("CreateAccountAccessChangeConfirmation");
                    }
                    else
                    {
                        return this.RedirectToAction("CreateAccountAccessChangeConfirmation");
                        ////accountCreation.EmailAddress = emailValidateViewModel.Email;
                        ////accountCreation.AccountCreationType = AccountCreationTypeEnum.FullAccess;
                        ////await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                        ////return this.CheckConfirmationUpdate() ? this.RedirectToAction("CreateAccountConfirmation", new AccountCreationViewModel { LocationId = accountCreation.LocationId }) : this.RedirectToAction("CreateAccountFullUserConfirmation");
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
        [Route("RestrictedLocation")]
        public async Task<IActionResult> RestrictedLocation()
        {
            return this.View();
        }

        /// <summary>
        /// CreateAccountGeneralUserConfirmation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CreateAccountGeneralUserConfirmation")]
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
        [Route("CreateAccountGeneralUserConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountGeneralUserConfirmation(bool? returnToConfirmation = false)
        {
            return this.RedirectToAction("CreateAccountPersonalDetails");
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
        /// CreateAccountAccessChangeConfirmation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Route("CreateAccountAccessChangeConfirmation")]
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
        [Route("CreateAccountAccessChangeConfirmation")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> CreateAccountAccessChangeConfirmation(bool? returnToConfirmation = false)
        {
            return this.RedirectToAction("CreateAccountFullUserConfirmation");
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

        /////// <summary>
        /////// Account confirmation view.
        /////// </summary>
        /////// <returns>The <see cref="IActionResult"/>.</returns>
        /////// <param name="accountCreationViewModel">accountCreationViewModel.</param>
        ////[Route("Registration/CreateAccountConfirmation")]
        ////[ResponseCache(CacheProfileName = "Never")]
        ////[TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        ////public async Task<IActionResult> CreateAccountConfirmation(AccountCreationViewModel accountCreationViewModel)
        ////{
        ////    var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
        ////    AccountCreationFormHelper.PopulateGroupedFormControlMetadata(this.ViewData);
        ////    if (accountCreation.AccountCreationType == AccountCreationTypeEnum.FullAccess)
        ////    {
        ////        var placeOfWorkCheck = int.TryParse(accountCreationViewModel.LocationId, out int locationId);
        ////        if (!placeOfWorkCheck || locationId == 0)
        ////        {
        ////            this.ModelState.AddModelError("LocationId", CommonValidationErrorMessages.WorkPlace);
        ////            var location = await this.locationService.GetPagedFilteredAsync(accountCreationViewModel.FilterText, 1, UserRegistrationContentPageSize);
        ////            return this.View("CreateAccountWorkPlace", new AccountCreationListViewModel { FilterText = accountCreationViewModel.FilterText, WorkPlaceList = location.Item2, AccountCreationPaging = new AccountCreationPagingModel { TotalItems = location.Item1, PageSize = UserRegistrationContentPageSize, HasItems = location.Item1 > 0, CurrentPage = 1 }, LocationId = accountCreation.LocationId });
        ////        }

        ////        accountCreation.LocationId = accountCreationViewModel.LocationId;
        ////        await this.multiPageFormService.SetMultiPageFormData(accountCreation, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
        ////    }

        ////    this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
        ////    if (accountCreation.IsLoginWizard)
        ////    {
        ////        return this.RedirectToAction("AccountConfirmation", "LoginWizard");
        ////    }

        ////    var response = await this.GetAccountConfirmationDetails(accountCreation);
        ////    return this.View(response);
        ////}

        /////// <summary>
        /////// Account confirmation and Create account API request.
        /////// </summary>
        /////// <returns>The <see cref="IActionResult"/>.</returns>
        ////[HttpPost]
        ////[Route("Registration/CreateAccountConfirmation")]
        ////[ResponseCache(CacheProfileName = "Never")]
        ////[TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        ////public async Task<IActionResult> CreateAccountConfirmation()
        ////{
        ////    var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
        ////    this.ViewBag.AccountCreationType = accountCreation.AccountCreationType;
        ////    AccountCreationFormHelper.PopulateGroupedFormControlMetadata(this.ViewData);
        ////    ////if (accountCreation.CountryId == "1" && (string.IsNullOrWhiteSpace(accountCreation.RegionId) || accountCreation.RegionId == "0"))
        ////    ////{
        ////    ////    this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.RegionRequiredSummary);
        ////    ////    var accountDetails = await this.GetAccountConfirmationDetails(accountCreation);
        ////    ////    return this.View(accountDetails);
        ////    ////}
        ////    ////else
        ////    ////{
        ////    var request = new RegistrationRequestViewModel
        ////    {
        ////        UserRegistrationType = (UserRegistrationTypeEnum)accountCreation.AccountCreationType,
        ////        CountryId = 0,
        ////        RegionId = null,
        ////        EmailAddress = accountCreation.EmailAddress,
        ////        SecondaryEmailAddress = null,
        ////        JobRoleId = 0,
        ////        FirstName = accountCreation.FirstName,
        ////        LastName = accountCreation.LastName,
        ////        SelfRegistration = true,
        ////        GradeId = 0,
        ////        LocationId = 0,
        ////        LocationStartDate = accountCreation.StartDate.GetValueOrDefault(),
        ////        MedicalCouncilNumber = accountCreation.RegistrationNumber,
        ////        SpecialtyId = int.TryParse(accountCreation.PrimarySpecialtyId, out int specialtyId) ? specialtyId : 0,
        ////    };
        ////    var response = await this.userService.RegisterNewUser(request);
        ////    if (!response.IsValid)
        ////    {
        ////        var errorMessage = string.Join(", ", response.Details);
        ////        this.ModelState.AddModelError(string.Empty, errorMessage);
        ////        var accountDetails = await this.GetAccountConfirmationDetails(accountCreation);
        ////        return this.View(accountDetails);
        ////    }
        ////    else
        ////    {
        ////        await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
        ////        this.TempData.Clear();
        ////    }
        ////    ////}

        ////    return this.View("CreateAccountSuccess");
        ////}

        /// <summary>
        /// Check your email page.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult CheckYourEmail()
        {
            return this.View();
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
