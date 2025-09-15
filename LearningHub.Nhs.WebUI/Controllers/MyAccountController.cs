namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.WebUtilitiesInterfaces;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.Account;
    using LearningHub.Nhs.WebUI.Models.UserProfile;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NHSUKViewComponents.Web.ViewModels;
    using ChangePasswordViewModel = LearningHub.Nhs.WebUI.Models.UserProfile.ChangePasswordViewModel;
    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

    /// <summary>
    /// The UserController.
    /// </summary>
    [Authorize]
    public partial class MyAccountController : BaseController
    {
        private const int UserDetailContentPageSize = 10;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IUserService userService;
        private readonly ILoginWizardService loginWizardService;
        private readonly ICountryService countryService;
        private readonly IRegionService regionService;
        private readonly IJobRoleService jobRoleService;
        private readonly IGradeService gradeService;
        private readonly ISpecialtyService specialtyService;
        private readonly ILocationService locationService;
        private readonly ICacheService cacheService;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="userService">userService.</param>
        /// <param name="loginWizardService">loginWizardService.</param>
        /// <param name="countryService">countryService.</param>
        /// <param name="regionService">regionService.</param>
        /// <param name="jobRoleService">The jobRoleService<see cref="IJobRoleService"/>.</param>
        /// <param name="gradeService">The gradeService.</param>
        /// <param name="specialtyService">The specialtyService.</param>
        /// <param name="locationService">The locationService.</param>
        /// <param name="multiPageFormService">The multiPageFormService<see cref="IMultiPageFormService"/>.</param>
        /// <param name="cacheService">The cacheService<see cref="ICacheService"/>.</param>
        /// <param name="configuration">The cacheService<see cref="IConfiguration"/>.</param>
        public MyAccountController(
                IWebHostEnvironment hostingEnvironment,
                ILogger<ResourceController> logger,
                IOptions<Settings> settings,
                IHttpClientFactory httpClientFactory,
                IUserService userService,
                ILoginWizardService loginWizardService,
                ICountryService countryService,
                IRegionService regionService,
                IJobRoleService jobRoleService,
                IGradeService gradeService,
                ISpecialtyService specialtyService,
                ILocationService locationService,
                IMultiPageFormService multiPageFormService,
                ICacheService cacheService,
                IConfiguration configuration)
                : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.userService = userService;
            this.loginWizardService = loginWizardService;
            this.countryService = countryService;
            this.regionService = regionService;
            this.jobRoleService = jobRoleService;
            this.gradeService = gradeService;
            this.specialtyService = specialtyService;
            this.locationService = locationService;
            this.multiPageFormService = multiPageFormService;
            this.cacheService = cacheService;
            this.configuration = configuration;
        }

        private string LoginWizardCacheKey => $"{this.CurrentUserId}:LoginWizard";

        /// <summary>
        /// User profile actions.
        /// </summary>
        /// <param name="returnUrl">The redirect back url.</param>
        /// <param name="checkDetails">Whether to check account details.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("myaccount")]
        public async Task<IActionResult> Index(string returnUrl = null, bool? checkDetails = false)
        {
            string loginWizardCacheKey = $"{this.CurrentUserId}:LoginWizard";
            var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<Models.Account.LoginWizardViewModel>(loginWizardCacheKey);

            if (checkDetails == true || cacheExists)
            {
                this.ViewBag.CheckDetails = true;

                var rules = loginWizard.LoginWizardStagesRemaining.SelectMany(l => l.LoginWizardRules.Where(r => r.Required));
                foreach (var rule in rules)
                {
                    this.ModelState.AddModelError(string.Empty, rule.Description);
                }

                if (this.TempData.ContainsKey("IsJobRoleRequired"))
                {
                    if (this.TempData["IsJobRoleRequired"] != null && (bool)this.TempData["IsJobRoleRequired"] == true)
                    {
                        this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.RoleRequired);
                        this.TempData["IsJobRoleRequired"] = null;
                    }
                }
            }

            var userProfileSummary = await this.userService.GetUserProfileSummaryAsync();
            return this.View("Index", userProfileSummary);
        }

        /// <summary>
        /// ChangeFirstName.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangeFirstname")]
        public async Task<IActionResult> ChangeFirstName()
        {
            var userPersonalDetails = await this.userService.GetUserPersonalDetailsAsync();
            return this.View("ChangeFirstname", userPersonalDetails);
        }

        /// <summary>
        /// ChangeLastName.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangeLastName")]
        public async Task<IActionResult> ChangeLastName()
        {
            var userPersonalDetails = await this.userService.GetUserPersonalDetailsAsync();
            return this.View("ChangeLastName", userPersonalDetails);
        }

        /// <summary>
        /// ChangePreferredName.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangePreferredName")]
        public async Task<IActionResult> ChangePreferredName()
        {
            var userPersonalDetails = await this.userService.GetUserPersonalDetailsAsync();
            return this.View("ChangePreferredName", userPersonalDetails);
        }

        /// <summary>
        /// ChangeCountry.
        /// </summary>
        /// <param name="selectedCountryId">country id.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangeCountry")]
        public async Task<IActionResult> ChangeCountry(int? selectedCountryId)
        {
            this.TempData.Clear();
            var userLocationViewModel = await this.userService.GetUserLocationDetailsAsync();
            var userProfileSummary = await this.userService.GetUserProfileSummaryAsync();
            if (selectedCountryId.HasValue)
            {
                userLocationViewModel.SelectedCountryId = selectedCountryId;
            }

            await this.multiPageFormService.SetMultiPageFormData(
                userLocationViewModel,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                this.TempData);
            return this.View("ChangeCountry", new Tuple<UserProfileSummaryViewModel, UserLocationViewModel>(userProfileSummary, userLocationViewModel));
        }

        /// <summary>
        /// ChangeRegion.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangeRegion")]
        public async Task<IActionResult> ChangeRegion()
        {
            var userLocationViewModel = await this.userService.GetUserLocationDetailsAsync();
            UserLocationViewModel model = new UserLocationViewModel();
            var regions = await this.regionService.GetAllAsync();
            List<RadiosItemViewModel> radio = new List<RadiosItemViewModel>();
            foreach (var region in regions)
            {
                var newradio = new RadiosItemViewModel(region.Id.ToString(), region.Name, false, region.Name);
                radio.Add(newradio);
            }

            model.Region = radio;
            model.SelectedRegionId = userLocationViewModel.SelectedRegionId;
            return this.View("ChangeRegion", model);
        }

        /// <summary>
        /// ChangePrimaryEmail.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangePrimaryEmail")]
        public async Task<IActionResult> ChangePrimaryEmail()
        {
            var userEmailDetailsViewModel = await this.userService.GetUserEmailDetailsAsync();
            return this.View("ChangePrimaryEmail", userEmailDetailsViewModel);
        }

        /// <summary>
        /// ChangeSecondaryEmail.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ChangeSecondaryEmail")]
        public async Task<IActionResult> ChangeSecondaryEmail()
        {
            var userEmailDetailsViewModel = await this.userService.GetUserEmailDetailsAsync();
            return this.View("ChangeSecondaryEmail", userEmailDetailsViewModel);
        }

        /// <summary>
        /// Confirm password.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("myaccount/ConfirmPassword")]
        public async Task<IActionResult> ConfirmPassword()
        {
            var personalDetail = await this.userService.GetCurrentUserPersonalDetailsAsync();
            CurrentPasswordViewModel currentPasswordViewModel = new CurrentPasswordViewModel();
            currentPasswordViewModel.CurrentPassword = personalDetail.PasswordHash;
            return this.View("ConfirmPassword", currentPasswordViewModel);
        }

        /// <summary>
        /// Change Password.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [Route("myaccount/ChangePassword")]
        public async Task<IActionResult> ChangePassword(CurrentPasswordViewModel model)
        {
            var personalDetail = await this.userService.GetCurrentUserPersonalDetailsAsync();
            if (this.ModelState.IsValid)
            {
                string passwordHash = this.userService.Base64MD5HashDigest(model.CurrentPassword);
                if (passwordHash == personalDetail.PasswordHash)
                {
                    ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
                    changePasswordViewModel.Username = personalDetail.UserName;
                    return this.View("ChangePassword", changePasswordViewModel);
                }
                else
                {
                    this.ModelState.AddModelError(
                   nameof(model.CurrentPassword),
                   CommonValidationErrorMessages.ValidPasswordRequired);
                }
            }

            CurrentPasswordViewModel currentPasswordViewModel = new CurrentPasswordViewModel();
            currentPasswordViewModel.CurrentPassword = model.CurrentPassword;
            this.ViewBag.Errormessage = CommonValidationErrorMessages.PasswordNotRecognised;
            return this.View("ConfirmPassword", currentPasswordViewModel);
        }

        /// <summary>
        /// SecurityQuestionsDetails.
        /// </summary>
        /// <param name="questionIndex">questionIndex.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> SecurityQuestionsDetails(int questionIndex)
        {
            SecurityQuestionSelectViewModel securityViewModel = new SecurityQuestionSelectViewModel();
            var model = await this.loginWizardService.GetSecurityQuestionsModel(this.CurrentUserId);

            securityViewModel.SecurityQuestions = model.SecurityQuestions.Select(q => new SelectListItem { Value = q.Value, Text = q.Text }).ToList();
            securityViewModel.QuestionIndex = questionIndex;
            securityViewModel.SelectedSecurityQuestionId = model.UserSecurityQuestions.ElementAt(questionIndex).SecurityQuestionId;
            return this.View("SecurityQuestionsDetails", securityViewModel);
        }

        /// <summary>
        /// To Update first name.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateFirstName(UserPersonalDetailsViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ChangeFirstname", model);
            }

            await this.userService.UpdateUserFirstNameAsync(this.CurrentUserId, model);
            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.FirstNameSuccessMessage;
            return this.View("SuccessMessage");
        }

        /// <summary>
        /// To Update last name.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateLastName(UserPersonalDetailsViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ChangeLastname", model);
            }

            await this.userService.UpdateUserLastNameAsync(this.CurrentUserId, model);
            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.LastNameSuccessMessage;
            return this.View("SuccessMessage");
        }

        /// <summary>
        /// To Update preferred name.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePreferredName(UserPersonalDetailsViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ChangePreferredName", model);
            }

            await this.userService.UpdateUserPreferredNameAsync(this.CurrentUserId, model);
            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.PreferredNameSuccessMessage;
            return this.View("SuccessMessage");
        }

        /// <summary>
        /// To Update primary email.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePrimaryEmail(UserEmailDetailsViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("ChangePrimaryEmail", model);
            }

            bool userPrimaryEmailAddressChanged = false;
            var user = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(model.PrimaryEmailAddress) && user.EmailAddress.ToLower() != model.PrimaryEmailAddress.ToLower())
                {
                    userPrimaryEmailAddressChanged = true;
                }
            }

            if (userPrimaryEmailAddressChanged)
            {
                if (await this.userService.DoesEmailAlreadyExist(model.PrimaryEmailAddress))
                {
                    this.ModelState.AddModelError(
                               nameof(model.PrimaryEmailAddress),
                               CommonValidationErrorMessages.DuplicateEmailAddress);
                    return this.View("ChangePrimaryEmail", model);
                }
                else
                {
                    var isUserRoleUpgrade = await this.userService.ValidateUserRoleUpgradeAsync(user.EmailAddress, model.PrimaryEmailAddress);
                    UserRoleUpgrade userRoleUpgradeModel = new UserRoleUpgrade()
                    {
                        UserId = this.CurrentUserId,
                        EmailAddress = model.PrimaryEmailAddress,
                    };
                    if (isUserRoleUpgrade)
                    {
                        userRoleUpgradeModel.UserHistoryTypeId = (int)UserHistoryType.UserRoleUpgarde;
                    }
                    else
                    {
                        userRoleUpgradeModel.UserHistoryTypeId = (int)UserHistoryType.UserDetails;
                    }

                    await this.userService.GenerateEmailChangeValidationTokenAndSendEmailAsync(model.PrimaryEmailAddress, isUserRoleUpgrade);
                    await this.userService.UpdateUserRoleUpgradeAsync();
                    await this.userService.CreateUserRoleUpgradeAsync(userRoleUpgradeModel);
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailChangeRequestedSucessMessage;
                    this.ViewBag.Status = "Valid";
                    return this.View("ConfirmEmailSuccessMessage");
                }
            }

            this.ModelState.AddModelError(nameof(model.PrimaryEmailAddress), CommonValidationErrorMessages.DuplicateEmailAddress);
            return this.View("ChangePrimaryEmail", model);
        }

        /// <summary>
        /// To Update secondary email.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSecondaryEmail(UserEmailDetailsViewModel model)
        {
            bool valid = this.TryValidateModel(model);
            bool isSamePrimaryEmailPendingforValidate = await this.userService.CheckSamePrimaryemailIsPendingToValidate(model.SecondaryEmailAddress);
            if (!valid)
            {
                return this.View("ChangeSecondaryEmail", model);
            }

            if (isSamePrimaryEmailPendingforValidate)
            {
                this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.SecondaryEmailShouldNotBeSame);
                return this.View("ChangeSecondaryEmail", model);
            }

            await this.userService.UpdateUserSecondaryEmailAsync(this.CurrentUserId, model);
            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.SecondaryEmailSuccessMessage;
            return this.View("SuccessMessage");
        }

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="model">The model<see cref="ChangePasswordViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.userService.UpdatePassword(model.NewPassword);
                var redirectUri = $"{this.configuration["LearningHubAuthServiceConfig:Authority"]}/Home/SetIsPasswordUpdate?isPasswordUpdate=true";
                return this.Redirect(redirectUri);
            }
            else
            {
                return this.View("ChangePassword", model);
            }
        }

        /// <summary>
        /// Search Country.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        /// <param name="userLocationViewModel">userLocationViewModel.</param>
        [ResponseCache(CacheProfileName = "Never")]
        ////[TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> SearchCountry(UserLocationViewModel userLocationViewModel)
        {
            var userProfileSummary = await this.userService.GetUserProfileSummaryAsync();
            if (string.IsNullOrWhiteSpace(userLocationViewModel.FilterText))
            {
                this.ModelState.AddModelError(string.Empty, CommonValidationErrorMessages.SearchTextRequired);
                return this.View("ChangeCountry", new Tuple<UserProfileSummaryViewModel, UserLocationViewModel>(userProfileSummary, userLocationViewModel));
            }

            var countries = await this.countryService.GetFilteredAsync(userLocationViewModel.FilterText);

            List<RadiosItemViewModel> radios = new List<RadiosItemViewModel>();
            foreach (var country in countries)
            {
                radios.Add(new RadiosItemViewModel(country.Id.ToString(), country.Name, false, country.Name));
            }

            userLocationViewModel.Country = radios;
            return this.View("ChangeCountry", new Tuple<UserProfileSummaryViewModel, UserLocationViewModel>(userProfileSummary, userLocationViewModel));
        }

        /// <summary>
        /// To list security questions.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> ValidateSecurityQuestion(SecurityQuestionSelectViewModel model)
        {
            bool duplicatesExist = false;
            var securityQuestionsViewModel = await this.loginWizardService.GetSecurityQuestionsModel(this.CurrentUserId);
            model.SecurityQuestions = securityQuestionsViewModel.SecurityQuestions.Select(q => new SelectListItem { Value = q.Value, Text = q.Text }).ToList();

            if (model.QuestionIndex == 0)
            {
                if (model.SelectedSecurityQuestionId == securityQuestionsViewModel.UserSecurityQuestions[1].SecurityQuestionId)
                {
                    duplicatesExist = true;
                }
            }
            else if (model.QuestionIndex == 1)
            {
                if (model.SelectedSecurityQuestionId == securityQuestionsViewModel.UserSecurityQuestions[0].SecurityQuestionId)
                {
                    duplicatesExist = true;
                }
            }

            if (duplicatesExist)
            {
                this.ModelState.AddModelError("DuplicateQuestion", CommonValidationErrorMessages.DuplicateQuestion);
                return this.View("SecurityQuestionsDetails", model);
            }

            var viewModel = new SecurityQuestionAnswerViewModel();
            var selectedSecurityQuestion = securityQuestionsViewModel.SecurityQuestions.SingleOrDefault(q => q.Value == model.SelectedSecurityQuestionId.ToString());
            viewModel.QuestionIndex = model.QuestionIndex;
            viewModel.QuestionText = selectedSecurityQuestion.Text;
            viewModel.SecurityQuestionId = Convert.ToInt32(selectedSecurityQuestion.Value);
            viewModel.UserSecurityQuestionId = securityQuestionsViewModel.UserSecurityQuestions[model.QuestionIndex].Id;
            return this.View("ChangeSecurityQuestions", viewModel);
        }

        /// <summary>
        /// To update security questions.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSecurityQuestions(SecurityQuestionAnswerViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var userSecurityQuestions = new List<UserSecurityQuestionViewModel>
            {
                new UserSecurityQuestionViewModel
                {
                    Id = model.UserSecurityQuestionId,
                    SecurityQuestionId = model.SecurityQuestionId,
                    SecurityQuestionAnswerHash = model.SecurityQuestionAnswer,
                    UserId = this.CurrentUserId,
                },
            };

                await this.userService.UpdateUserSecurityQuestions(userSecurityQuestions);

                if (model.QuestionIndex == 0)
                {
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.FirstQuestionSuccessMessage;
                }
                else
                {
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.SecondQuestionSuccessMessage;
                }

                return this.View("SuccessMessage");
            }
            else
            {
                return this.View("ChangeSecurityQuestions", model);
            }
        }

        /// <summary>
        /// To update country .
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCountry(UserLocationViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.multiPageFormService.SetMultiPageFormData(model, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                if (model.SelectedCountryId == 1)
                {
                    var regions = await this.regionService.GetAllAsync();
                    List<RadiosItemViewModel> radio = new List<RadiosItemViewModel>();
                    foreach (var region in regions)
                    {
                        var newradio = new RadiosItemViewModel(region.Id.ToString(), region.Name, false, region.Name);
                        radio.Add(newradio);
                    }

                    model.Region = radio;
                    return this.View("ChangeRegion", model);
                }
                else
                {
                    var countryDetails = await this.multiPageFormService.GetMultiPageFormData<UserLocationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    await this.userService.UpdateUserCountryDetailsAsync(this.CurrentUserId, countryDetails);
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.CountrySuccessMessage;
                    return this.View("SuccessMessage");
                }
            }
            else
            {
                return this.View("ChangeCountry", model);
            }
        }

        /// <summary>
        /// To update location details.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateLocationDetails(UserLocationViewModel model)
        {
            if (model.SelectedRegionId == null)
            {
                this.ModelState.AddModelError("SelectRegion", CommonValidationErrorMessages.RegionRequired);
                var regions = await this.regionService.GetAllAsync();
                List<RadiosItemViewModel> radio = new List<RadiosItemViewModel>();
                foreach (var region in regions)
                {
                    var newradio = new RadiosItemViewModel(region.Id.ToString(), region.Name, false, region.Name);
                    radio.Add(newradio);
                }

                model.Region = radio;
                return this.View("ChangeRegion", model);
            }
            else if (model.SelectedRegionId != null && model.SelectedCountryId != null)
            {
                await this.userService.UpdateUserLocationDetailsAsync(this.CurrentUserId, model);
                this.ViewBag.SuccessMessage = CommonValidationErrorMessages.LocationSuccessMessage;
                return this.View("SuccessMessage");
            }
            else if (model.SelectedRegionId != null)
            {
                await this.userService.UpdateUserRegionDetailsAsync(this.CurrentUserId, model);
                this.ViewBag.SuccessMessage = CommonValidationErrorMessages.RegionSuccessMessage;
                return this.View("SuccessMessage");
            }
            else
            {
                return this.View("ChangeRegion", model);
            }
        }

        /// <summary>
        /// ChangeCurrentRole.
        /// </summary>
        /// <param name="viewModel">User jobRole update view model.</param>
        /// <param name="searchSubmission">Search form submitted.</param>
        /// <param name="formSubmission">Update role submitted.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-current-role")]
        public async Task<IActionResult> ChangeCurrentRole([FromQuery] UserJobRoleUpdateViewModel viewModel, bool searchSubmission = false, bool formSubmission = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();

            viewModel.CurrentRoleName = profile.JobRole;
            viewModel.PageSize = UserDetailContentPageSize;
            viewModel.CurrentPage = viewModel.CurrentPage == 0 ? 1 : viewModel.CurrentPage;

            if (searchSubmission && string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                this.ModelState.AddModelError(nameof(viewModel.FilterText), CommonValidationErrorMessages.SearchTermRequired);
                return this.View("ChangeCurrentRole", viewModel);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                var jobRoles = await this.jobRoleService.GetPagedFilteredAsync(viewModel.FilterText, viewModel.CurrentPage, viewModel.PageSize);
                viewModel.RoleList = jobRoles.Item2;
                viewModel.TotalItems = jobRoles.Item1;
                viewModel.HasItems = jobRoles.Item1 > 0;
            }

            if (formSubmission)
            {
                if (viewModel.SelectedJobRoleId.HasValue)
                {
                    var newRoleId = viewModel.SelectedJobRoleId.Value;
                    var jobRole = await this.jobRoleService.GetByIdAsync(newRoleId);

                    if (jobRole.MedicalCouncilId > 0 && jobRole.MedicalCouncilId < 4)
                    {
                        return this.RedirectToAction(nameof(this.ChangeMedicalCouncilNo), new UserMedicalCouncilNoUpdateViewModel { SelectedJobRoleId = newRoleId });
                    }
                    else
                    {
                        return this.RedirectToAction(nameof(this.ChangeGrade), new UserGradeUpdateViewModel { SelectedJobRoleId = newRoleId });
                    }
                }
                else
                {
                    this.ModelState.AddModelError(nameof(viewModel.SelectedJobRoleId), CommonValidationErrorMessages.RoleRequired);
                    return this.View("ChangeCurrentRole", viewModel);
                }
            }

            return this.View("ChangeCurrentRole", viewModel);
        }

        /// <summary>
        /// ChangeMedicalCouncilNo.
        /// </summary>
        /// <param name="viewModel">User job medical council number update view model.</param>
        /// <param name="formSubmission">Update medical council number submitted.</param>
        /// <param name="direct">If only need to update medical council number.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-medical-council-number")]
        public async Task<IActionResult> ChangeMedicalCouncilNo([FromQuery] UserMedicalCouncilNoUpdateViewModel viewModel, bool formSubmission = false, bool direct = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();

            if (!string.IsNullOrEmpty(profile.MedicalCouncilNo))
            {
                var jobRole = await this.jobRoleService.GetByIdAsync(profile.JobRoleId.Value);
                viewModel.MedicalCouncil = jobRole.MedicalCouncilName;
                viewModel.MedicalCouncilCode = jobRole.MedicalCouncilCode;
                viewModel.MedicalCouncilNo = profile.MedicalCouncilNo;
            }

            var selectedJobRole = await this.jobRoleService.GetByIdAsync(viewModel.SelectedJobRoleId);
            viewModel.SelectedJobRole = selectedJobRole.NameWithStaffGroup;
            viewModel.SelectedMedicalCouncilId = selectedJobRole.MedicalCouncilId;
            viewModel.SelectedMedicalCouncilCode = selectedJobRole.MedicalCouncilCode;

            if (formSubmission)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.SelectedMedicalCouncilNo))
                {
                    string validateMedicalCouncilNumber = await this.jobRoleService.ValidateMedicalCouncilNumber(
                        profile.LastName, viewModel.SelectedMedicalCouncilId, viewModel.SelectedMedicalCouncilNo);

                    if (!string.IsNullOrWhiteSpace(validateMedicalCouncilNumber))
                    {
                        this.ModelState.AddModelError(nameof(viewModel.SelectedMedicalCouncilNo), validateMedicalCouncilNumber);
                        return this.View("ChangeMedicalCouncilNumber", viewModel);
                    }
                    else if (direct)
                    {
                        await this.userService.UpdateUserEmployment(
                            new elfhHub.Nhs.Models.Entities.UserEmployment
                            {
                                Id = profile.EmploymentId,
                                UserId = profile.Id,
                                JobRoleId = profile.JobRoleId,
                                MedicalCouncilId = viewModel.SelectedMedicalCouncilId,
                                MedicalCouncilNo = viewModel.SelectedMedicalCouncilNo,
                                GradeId = profile.GradeId,
                                SpecialtyId = profile.SpecialtyId,
                                StartDate = profile.JobStartDate,
                                LocationId = profile.LocationId,
                            });

                        this.ViewBag.SuccessMessage = "Your medical council number has been changed";
                        return this.View("SuccessMessage");
                    }
                    else
                    {
                        return this.RedirectToAction(
                            nameof(this.ChangeGrade),
                            new UserGradeUpdateViewModel
                            {
                                SelectedJobRoleId = viewModel.SelectedJobRoleId,
                                SelectedMedicalCouncilNo = viewModel.SelectedMedicalCouncilNo,
                            });
                    }
                }
                else
                {
                    this.ModelState.AddModelError(nameof(viewModel.SelectedMedicalCouncilNo), $"Enter your {viewModel.SelectedMedicalCouncilCode} number");
                    return this.View("ChangeMedicalCouncilNumber", viewModel);
                }
            }

            return this.View("ChangeMedicalCouncilNumber", viewModel);
        }

        /// <summary>
        /// ChangeGrade.
        /// </summary>
        /// <param name="viewModel">User job grade update view model.</param>
        /// <param name="formSubmission">Update grade submitted.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-grade")]
        public async Task<IActionResult> ChangeGrade([FromQuery] UserGradeUpdateViewModel viewModel, bool formSubmission = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();
            var jobRole = await this.jobRoleService.GetByIdAsync(viewModel.SelectedJobRoleId);
            var grades = await this.gradeService.GetGradesForJobRoleAsync(viewModel.SelectedJobRoleId);

            viewModel.GradeList = grades;
            viewModel.Grade = profile.Grade;
            viewModel.SelectedJobRole = jobRole.NameWithStaffGroup;
            viewModel.SelectedMedicalCouncilId = jobRole.MedicalCouncilId;
            if (formSubmission)
            {
                if (this.User.IsInRole("BasicUser") || viewModel.SelectedGradeId != null)
                {
                    var medicalCouncilNoRequired = jobRole.MedicalCouncilId > 0 && jobRole.MedicalCouncilId < 4;
                    await this.userService.UpdateUserEmployment(
                        new elfhHub.Nhs.Models.Entities.UserEmployment
                        {
                            Id = profile.EmploymentId,
                            UserId = profile.Id,
                            JobRoleId = viewModel.SelectedJobRoleId,
                            MedicalCouncilId = medicalCouncilNoRequired ? jobRole.MedicalCouncilId : null,
                            MedicalCouncilNo = medicalCouncilNoRequired ? (viewModel.SelectedMedicalCouncilNo ?? profile.MedicalCouncilNo) : null,
                            GradeId = Convert.ToInt32(viewModel.SelectedGradeId),
                            SpecialtyId = profile.SpecialtyId,
                            StartDate = profile.JobStartDate,
                            LocationId = profile.LocationId,
                        });

                    this.ViewBag.SuccessMessage = "Your job details have been changed";
                    return this.View("SuccessMessage");
                }
                else
                {
                    this.ModelState.AddModelError(nameof(viewModel.SelectedGradeId), CommonValidationErrorMessages.GradeRequired);
                    return this.View("ChangeGrade", viewModel);
                }
            }
            else
            {
                viewModel.SelectedGradeId = profile.GradeId.ToString();
            }

            return this.View("ChangeGrade", viewModel);
        }

        /// <summary>
        /// ChangePrimarySpecialty.
        /// </summary>
        /// <param name="viewModel">User primary specialty update view model.</param>
        /// <param name="searchSubmission">Search form submitted.</param>
        /// <param name="formSubmission">Update primary specialty submitted.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-primary-specialty")]
        public async Task<IActionResult> ChangePrimarySpecialty([FromQuery] UserPrimarySpecialtyUpdateViewModel viewModel, bool searchSubmission = false, bool formSubmission = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();
            var optionalSpecialty = await this.specialtyService.GetSpecialtiesAsync();

            viewModel.PrimarySpecialty = profile.PrimarySpecialty;
            viewModel.PageSize = UserDetailContentPageSize;
            viewModel.CurrentPage = viewModel.CurrentPage == 0 ? 1 : viewModel.CurrentPage;
            viewModel.OptionalSpecialtyItem = optionalSpecialty.FirstOrDefault(x => x.Name.ToLower() == "not applicable");

            if (searchSubmission && string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                this.ModelState.AddModelError(nameof(viewModel.FilterText), CommonValidationErrorMessages.SearchTermRequired);
                return this.View("ChangePrimarySpecialty", viewModel);
            }

            if (formSubmission)
            {
                if (viewModel.SelectedPrimarySpecialtyId.HasValue)
                {
                    await this.userService.UpdateUserEmployment(
                        new elfhHub.Nhs.Models.Entities.UserEmployment
                        {
                            Id = profile.EmploymentId,
                            UserId = profile.Id,
                            JobRoleId = profile.JobRoleId,
                            MedicalCouncilId = profile.MedicalCouncilId,
                            MedicalCouncilNo = profile.MedicalCouncilNo,
                            GradeId = profile.GradeId,
                            SpecialtyId = viewModel.SelectedPrimarySpecialtyId.Value,
                            StartDate = profile.JobStartDate,
                            LocationId = profile.LocationId,
                        });

                    this.ViewBag.SuccessMessage = "Your primary specialty has been changed";
                    return this.View("SuccessMessage");
                }
                else
                {
                    this.ModelState.AddModelError(nameof(viewModel.SelectedPrimarySpecialtyId), CommonValidationErrorMessages.SpecialtyNotApplicable);
                    return this.View("ChangePrimarySpecialty", viewModel);
                }
            }

            if (!string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                var shortlist = await this.specialtyService.GetPagedSpecialtiesAsync(viewModel.FilterText, viewModel.CurrentPage, viewModel.PageSize);
                viewModel.SpecialtyList = shortlist.Item2;
                viewModel.TotalItems = shortlist.Item1;
                viewModel.HasItems = shortlist.Item1 > 0;
            }

            return this.View("ChangePrimarySpecialty", viewModel);
        }

        /// <summary>
        /// ChangeStartDate.
        /// </summary>
        /// <param name="viewModel">User job start date update view model.</param>
        /// <param name="formSubmission">Update start date submitted.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-start-date")]
        public async Task<IActionResult> ChangeStartDate([FromQuery] UserStartDateUpdateViewModel viewModel, bool formSubmission = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();

            viewModel.JobStartDate = profile.JobStartDate;

            if (formSubmission)
            {
                if (this.ModelState.IsValid)
                {
                    await this.userService.UpdateUserEmployment(
                        new elfhHub.Nhs.Models.Entities.UserEmployment
                        {
                            Id = profile.EmploymentId,
                            UserId = profile.Id,
                            JobRoleId = profile.JobRoleId,
                            MedicalCouncilId = profile.MedicalCouncilId,
                            MedicalCouncilNo = profile.MedicalCouncilNo,
                            GradeId = profile.GradeId,
                            SpecialtyId = profile.SpecialtyId,
                            StartDate = viewModel.GetDate(),
                            LocationId = profile.LocationId,
                        });

                    this.ViewBag.SuccessMessage = "Your job start date has been changed";
                    return this.View("SuccessMessage");
                }
            }
            else
            {
                this.ModelState.Remove("Day");
                this.ModelState.Remove("Month");
                this.ModelState.Remove("Year");
            }

            return this.View("ChangeStartDate", viewModel);
        }

        /// <summary>
        /// ChangeWorkPlace.
        /// </summary>
        /// <param name="viewModel">User work place update view model.</param>
        /// <param name="searchSubmission">Search form submitted.</param>
        /// <param name="formSubmission">Update work place submitted.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("my-account/change-work-place")]
        public async Task<IActionResult> ChangeWorkPlace([FromQuery] UserWorkPlaceUpdateViewModel viewModel, bool searchSubmission = false, bool formSubmission = false)
        {
            var profile = await this.userService.GetUserProfileSummaryAsync();

            viewModel.WorkPlace = profile.PlaceOfWork;
            viewModel.PageSize = UserDetailContentPageSize;
            viewModel.CurrentPage = viewModel.CurrentPage == 0 ? 1 : viewModel.CurrentPage;

            if (searchSubmission && string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                this.ModelState.AddModelError(nameof(viewModel.FilterText), CommonValidationErrorMessages.SearchTermRequired);
                return this.View("ChangeWorkPlace", viewModel);
            }

            if (formSubmission && viewModel.SelectedWorkPlaceId.HasValue)
            {
                await this.userService.UpdateUserEmployment(
                    new elfhHub.Nhs.Models.Entities.UserEmployment
                    {
                        Id = profile.EmploymentId,
                        UserId = profile.Id,
                        JobRoleId = profile.JobRoleId,
                        MedicalCouncilId = profile.MedicalCouncilId,
                        MedicalCouncilNo = profile.MedicalCouncilNo,
                        GradeId = profile.GradeId,
                        SpecialtyId = profile.SpecialtyId,
                        StartDate = profile.JobStartDate,
                        LocationId = viewModel.SelectedWorkPlaceId.Value,
                    });

                var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<Models.Account.LoginWizardViewModel>(this.LoginWizardCacheKey);

                if (cacheExists && loginWizard.LoginWizardStagesRemaining.Any(l => l.Id == (int)LoginWizardStageEnum.PlaceOfWork))
                {
                    await this.loginWizardService.SaveLoginWizardStageActivity(LoginWizardStageEnum.PlaceOfWork, this.CurrentUserId);

                    var stagePlaceOfWork = loginWizard.LoginWizardStages.FirstOrDefault(s => s.Id == (int)LoginWizardStageEnum.PlaceOfWork);
                    loginWizard.LoginWizardStagesCompleted.Add(stagePlaceOfWork);

                    var loginWizardViewModel = new LoginWizardStagesViewModel
                    {
                        IsFirstLogin = loginWizard.IsFirstLogin,
                        LoginWizardStages = loginWizard.LoginWizardStages,
                        LoginWizardStagesCompleted = loginWizard.LoginWizardStagesCompleted,
                    };

                    await this.cacheService.SetAsync(this.LoginWizardCacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(loginWizardViewModel));
                }

                this.ViewBag.SuccessMessage = "Your place of work has been changed";
                return this.View("SuccessMessage");
            }

            if (!string.IsNullOrWhiteSpace(viewModel.FilterText))
            {
                var locations = await this.locationService.GetPagedFilteredAsync(viewModel.FilterText, viewModel.CurrentPage, viewModel.PageSize);
                viewModel.WorkPlaceList = locations.Item2;
                viewModel.TotalItems = locations.Item1;
                viewModel.HasItems = locations.Item1 > 0;
            }

            return this.View("ChangeWorkPlace", viewModel);
        }

        /// <summary>
        /// Confirm check details.
        /// </summary>
        /// <param name="returnUrl">Return redirect url.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("my-account/check-details")]
        public async Task<IActionResult> CheckDetails(string returnUrl = null)
        {
            var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<Models.Account.LoginWizardViewModel>(this.LoginWizardCacheKey);

            if (cacheExists)
            {
                var rules = loginWizard.LoginWizardStagesRemaining.SelectMany(l => l.LoginWizardRules.Where(r => r.Required));
                if (rules.Any())
                {
                    return this.RedirectToAction("Index", "MyAccount", new { returnUrl });
                }

                var profile = await this.userService.GetUserProfileSummaryAsync();
                if (profile != null)
                {
                    if (profile.JobRoleId == null && (!this.User.IsInRole("BasicUser")))
                    {
                        this.TempData["IsJobRoleRequired"] = true;
                        return this.RedirectToAction("Index", "MyAccount", new { returnUrl });
                    }
                }

                foreach (var wizardStage in loginWizard.LoginWizardStagesRemaining)
                {
                    var stage = (LoginWizardStageEnum)wizardStage.Id;
                    await this.loginWizardService.SaveLoginWizardStageActivity(stage, this.CurrentUserId);
                    loginWizard.CompleteLoginWizardStage(stage);
                }

                var loginWizardViewModel = new LoginWizardStagesViewModel
                {
                    IsFirstLogin = loginWizard.IsFirstLogin,
                    LoginWizardStages = loginWizard.LoginWizardStages,
                    LoginWizardStagesCompleted = loginWizard.LoginWizardStagesCompleted,
                };

                await this.cacheService.SetAsync(this.LoginWizardCacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(loginWizardViewModel));
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return this.RedirectToAction("Index", "Home");
            }
            else
            {
                return this.Redirect(returnUrl);
            }
        }

        /// <summary>
        /// The confirm email.
        /// </summary>
        /// <param name="token">The token<see cref="string"/>.</param>
        /// <param name="loctoken">The loctoken<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string loctoken)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(loctoken))
            {
                this.ViewBag.Status = "Invalid";
                return this.View("EmailChangeInvalidUrl");
            }

            var user = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
            var userRoleUpgarde = await this.userService.GetUserRoleUpgradeAsync(this.CurrentUserId);
            var isUserRoleUpgrade = await this.userService.ValidateUserRoleUpgradeAsync(user.EmailAddress, userRoleUpgarde.EmailAddress);
            var validationResult = await this.userService.ValidateEmailChangeTokenAsync(token, loctoken, isUserRoleUpgrade);

            EmailChangeValidateViewModel model = new EmailChangeValidateViewModel();

            if (validationResult.Valid)
            {
                if (isUserRoleUpgrade)
                {
                    await this.userService.UpgradeAsFullAccessUserAsync(validationResult.UserId, validationResult.Email);
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailConfirmSucessMessage;
                    model.Token = token;
                    model.Loctoken = loctoken;
                    return this.View("UserUpgradeSuccessMessage", model);
                }
                else
                {
                    await this.userService.UpdateUserPrimaryEmailAsync(validationResult.Email);

                    // Add UserHistory entry
                    UserHistoryViewModel userHistory = new UserHistoryViewModel()
                    {
                        UserId = this.CurrentUserId,
                        Detail = "User primary email address updated.",
                        UserHistoryTypeId = (int)UserHistoryType.UserDetails,
                    };
                    await this.userService.StoreUserHistory(userHistory);
                    this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailConfirmationSuccessMessage;
                    model.Token = token;
                    model.Loctoken = loctoken;
                    return this.View("UserEmailConfirmationSuccess", model);
                }
            }
            else if (validationResult.TokenIssue == "Invalid")
            {
                this.ViewBag.Status = validationResult.TokenIssue;
                this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailChangeValidationTokenInvalidMessage;
                return this.View("EmailChangeInvalidUrl");
            }
            else
            {
                this.ViewBag.Status = validationResult.TokenIssue;
                this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailChangeValidationTokenExpiredMessage;
                return this.View("EmailChangeExpiredToken");
            }
        }

        /// <summary>
        /// The RegenerateEmailChangeValidationToken.
        /// </summary>
        /// <param name="newPrimaryEmail">The majorVersion.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("RegenerateEmailChangeValidationToken/{newPrimaryEmail}")]
        public async Task<ActionResult> RegenerateEmailChangeValidationToken(string newPrimaryEmail)
        {
            var userProfileSummary = await this.userService.GetUserProfileSummaryAsync();
            await this.userService.UpdateUserRoleUpgradeAsync();
            var isUserRoleUpgrade = await this.userService.ValidateUserRoleUpgradeAsync(userProfileSummary.PrimaryEmailAddress, newPrimaryEmail);
            var result = await this.userService.RegenerateEmailChangeValidationTokenAsync(newPrimaryEmail, isUserRoleUpgrade);
            if (result != null)
            {
                UserRoleUpgrade userRoleUpgradeModel = new UserRoleUpgrade()
                {
                    UserId = this.CurrentUserId,
                    EmailAddress = userProfileSummary.NewPrimaryEmailAddress,
                };
                if (isUserRoleUpgrade)
                {
                    userRoleUpgradeModel.UserHistoryTypeId = (int)UserHistoryType.UserRoleUpgarde;
                }
                else
                {
                    userRoleUpgradeModel.UserHistoryTypeId = (int)UserHistoryType.UserDetails;
                }

                await this.userService.CreateUserRoleUpgradeAsync(userRoleUpgradeModel);
            }

            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.RegenearteEmailSuccessMessage;
            return this.View("SuccessMessage");
        }

        /// <summary>
        /// The CancelEmailChangeValidationToken.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("CancelEmailChangeValidationToken")]
        public async Task<ActionResult> CancelEmailChangeValidationToken()
        {
            await this.userService.UpdateUserRoleUpgradeAsync();
            await this.userService.CancelEmailChangeValidationTokenAsync();
            this.ViewBag.SuccessMessage = CommonValidationErrorMessages.EmailCancelMessage;
            return this.View("SuccessMessage");
        }
    }
}