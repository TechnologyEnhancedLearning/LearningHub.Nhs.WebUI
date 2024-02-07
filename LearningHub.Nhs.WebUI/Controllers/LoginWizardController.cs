// <copyright file="LoginWizardController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

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
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Account;
    using LearningHub.Nhs.WebUI.Models.UserProfile;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The LoginWizardController.
    /// </summary>
    public class LoginWizardController : BaseController
    {
        private const int UserRegistrationContentPageSize = 10;
        private readonly IJobRoleService jobRoleService;
        private readonly ILoginWizardService loginWizardService;
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
        /// Initializes a new instance of the <see cref="LoginWizardController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The httpClientFactory<see cref="IHttpClientFactory"/>.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{AccountController}"/>.</param>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="termsAndConditionService">The termsAndConditionService<see cref="ITermsAndConditionsService"/>.</param>
        /// <param name="loginWizardService">The loginWizardService<see cref="ILoginWizardService"/>.</param>
        /// <param name="jobRoleService">The jobRoleService<see cref="IJobRoleService"/>.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        /// <param name="cacheService">The cacheService<see cref="ICacheService"/>.</param>
        /// <param name="countryService">The countryService.</param>
        /// <param name="regionService">The regionService.</param>
        /// <param name="multiPageFormService">The multiPageFormService<see cref="IMultiPageFormService"/>.</param>
        /// <param name="specialtyService">The specialtyService.</param>
        /// <param name="locationService">The locationService.</param>
        /// <param name="gradeService">The gradeService.</param>
        public LoginWizardController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            ILogger<AccountController> logger,
            IUserService userService,
            ITermsAndConditionsService termsAndConditionService,
            ILoginWizardService loginWizardService,
            IJobRoleService jobRoleService,
            IOptions<Settings> settings,
            ICacheService cacheService,
            ICountryService countryService,
            IRegionService regionService,
            IMultiPageFormService multiPageFormService,
            ISpecialtyService specialtyService,
            ILocationService locationService,
            IGradeService gradeService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.userService = userService;
            this.termsAndConditionService = termsAndConditionService;
            this.loginWizardService = loginWizardService;
            this.jobRoleService = jobRoleService;
            this.cacheService = cacheService;
            this.multiPageFormService = multiPageFormService;
            this.countryService = countryService;
            this.regionService = regionService;
            this.gradeService = gradeService;
            this.specialtyService = specialtyService;
            this.locationService = locationService;
        }

        private string LoginWizardCacheKey => $"{this.CurrentUserId}:LoginWizard";

        /// <summary>
        /// Display the next step of the login wizard.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await this.GetWizardModelAsync();

            if (model == null || model.LoginWizard.LoginWizardStages.Count == 0)
            {
                var currentUser = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
                bool flag = currentUser.LoginWizardInProgress;
                await this.userService.UpdateLoginWizardFlag(flag);
                await this.cacheService.SetAsync(this.LoginWizardCacheKey, "start");
                return this.RedirectToAction("Index", "Home");
            }

            if (model.LoginWizard.IsWizardComplete())
            {
                await this.loginWizardService.CompleteLoginWizard(this.CurrentUserId);

                await this.cacheService.RemoveAsync(this.LoginWizardCacheKey);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
                else
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }

            var currentStage = model.LoginWizard.LoginWizardStagesRemaining.FirstOrDefault();

            if (currentStage?.Id == (int)LoginWizardStageEnum.TermsAndConditions)
            {
                TermsAndConditions termsAndConditionsObj = await this.termsAndConditionService.LatestVersionAsync(this.Settings.LearningHubTenantId);
                model.TermsAndConditions = termsAndConditionsObj;
            }

            if (currentStage?.Id == (int)LoginWizardStageEnum.PasswordReset)
            {
                var userPersonalDetail = await this.userService.GetCurrentUserPersonalDetailsAsync();
                model.ChangePasswordViewModel = new Models.Account.ChangePasswordViewModel();
                model.ChangePasswordViewModel.Username = userPersonalDetail.UserName;
            }

            if (currentStage?.Id == (int)LoginWizardStageEnum.SecurityQuestions)
            {
                SecurityQuestionsViewModel securityQuestions = await this.loginWizardService.GetSecurityQuestionsModel(this.CurrentUserId);

                while (securityQuestions.UserSecurityQuestions.Count < this.Settings.SecurityQuestionsToAsk)
                {
                    securityQuestions.UserSecurityQuestions.Add(new UserSecurityQuestionViewModel());
                }

                foreach (var answer in securityQuestions.UserSecurityQuestions)
                {
                    if (!string.IsNullOrEmpty(answer.SecurityQuestionAnswerHash))
                    {
                        answer.SecurityQuestionAnswerHash = "********";
                    }
                }

                this.TempData.Clear();
                var securityViewModel = new SecurityViewModel()
                {
                    SecurityQuestions = securityQuestions.SecurityQuestions,
                    UserSecurityQuestions = securityQuestions.UserSecurityQuestions,
                };

                await this.multiPageFormService.SetMultiPageFormData(securityViewModel, MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);

                return this.RedirectToAction("SelectSecurityQuestion", new RouteValueDictionary { { "questionIndex", 0 }, { "returnUrl", returnUrl } });
            }

            if (currentStage?.Id == (int)LoginWizardStageEnum.JobRole || currentStage?.Id == (int)LoginWizardStageEnum.PlaceOfWork || currentStage?.Id == (int)LoginWizardStageEnum.PersonalDetails)
            {
                if (model.LoginWizard.IsFirstLogin)
                {
                    /* Only the OpenAthens registration method will get here. LH registration automatically completes the JobRole,
                     * PlaceOfWork & PersonalDetails stages so they are not shown in the wizard at first login.
                     * The SSO registration process sets a user account flag so that isFirstLogin is false, taking them to the
                     * My Account screen instead (SSO has its own registration screen that already asks for this data). */

                    var user = await this.userService.GetCurrentUserAsync();
                    var userEmployment = await this.userService.GetPrimaryUserEmploymentForUser(this.CurrentUserId);

                    this.TempData.Clear();
                    var account = new AccountCreationViewModel
                    {
                        IsLoginWizard = true,
                        WizardStages = model.LoginWizard.LoginWizardStages,
                        WizardReturnUrl = returnUrl,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress,
                        SecondaryEmailAddress = user.AltEmailAddress,
                        CountryId = user.CountryId.HasValue ? user.CountryId.Value.ToString() : null,
                        RegionId = user.RegionId.HasValue ? user.RegionId.Value.ToString() : null,
                        PrimaryUserEmploymentId = userEmployment.Id.ToString(),
                        CurrentRole = userEmployment.JobRoleId.HasValue ? userEmployment.JobRoleId.ToString() : null,
                        GradeId = userEmployment.GradeId.HasValue ? userEmployment.GradeId.ToString() : null,
                        PrimarySpecialtyId = userEmployment.SpecialtyId.HasValue ? userEmployment.SpecialtyId.ToString() : null,
                        RegistrationNumber = userEmployment.MedicalCouncilNo,
                        StartDate = userEmployment.StartDate.HasValue ? userEmployment.StartDate.Value.DateTime : null,
                    };

                    await this.multiPageFormService.SetMultiPageFormData(
                        account,
                        MultiPageFormDataFeature.AddRegistrationPrompt,
                        this.TempData);

                    var userGroup = await this.userService.GetUserRoleName(this.CurrentUserId);
                    if (userGroup != "BasicUser")
                    {
                        return this.RedirectToAction("AccountInformationNeeded");
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "MyAccount", new { returnUrl, checkDetails = true });
                    }
                }
                else
                {
                    return this.RedirectToAction("Index", "MyAccount", new { returnUrl, checkDetails = true });
                }
            }

            if (currentStage?.Id == (int)LoginWizardStageEnum.None)
            {
                var currentUser = await this.userService.GetUserByUserIdAsync(this.CurrentUserId);
                bool flag = currentUser.LoginWizardInProgress;
                await this.userService.UpdateLoginWizardFlag(flag);
                return this.RedirectToAction("Index", "Home");
            }

            this.ViewBag.IsFirstLogin = model.LoginWizard.IsFirstLogin;
            model.ReturnUrl = returnUrl;
            return this.View("LoginWizard", model);
        }

        /// <summary>
        /// The accept terms and conditions.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<IActionResult> AcceptTermsAndConditions(int id, string returnUrl)
        {
            await this.termsAndConditionService.AcceptByUser(id, this.CurrentUserId);

            var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);

            if (cacheExists)
            {
                await this.CompleteLoginWizardStageAsync(loginWizard, LoginWizardStageEnum.TermsAndConditions);

                return this.RedirectToAction("Index", new RouteValueDictionary { { "returnUrl", returnUrl } });
            }

            return this.Redirect("/");
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="model">The model<see cref="LearningHub.Nhs.WebUI.Models.Account.ChangePasswordViewModel"/>Cange password model.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(LearningHub.Nhs.WebUI.Models.Account.ChangePasswordViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("_ChangePassword", model);
            }
            else
            {
                await this.userService.UpdatePassword(model.NewPassword);

                var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);

                if (cacheExists)
                {
                    await this.CompleteLoginWizardStageAsync(loginWizard, LoginWizardStageEnum.PasswordReset);

                    return this.RedirectToAction("Index", new RouteValueDictionary { { "returnUrl", returnUrl } });
                }
            }

            return this.Redirect("/");
        }

        /// <summary>
        /// Action for starting the security question multiPageForm stage of the wizard.
        /// </summary>
        /// <param name="questionIndex">The question index - i.e. 0 or 1.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.EditRegistrationPrompt) })]
        public async Task<IActionResult> SelectSecurityQuestion(int questionIndex, string returnUrl)
        {
            if (questionIndex < 0 || questionIndex >= this.Settings.SecurityQuestionsToAsk)
            {
                return this.RedirectToAction("Error", "Home");
            }

            SecurityViewModel securityViewModel = await this.multiPageFormService.GetMultiPageFormData<SecurityViewModel>(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
            var userQuestion = securityViewModel.UserSecurityQuestions.ElementAt(questionIndex);

            var viewModel = new SecurityQuestionSelectViewModel
            {
                SecurityQuestions = securityViewModel.SecurityQuestions,
                QuestionIndex = questionIndex,
                SelectedSecurityQuestionId = userQuestion.SecurityQuestionId,
            };

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View("SecurityQuestionSelect", viewModel);
        }

        /// <summary>
        /// Action for choosing a security question.
        /// </summary>
        /// <param name="model">The SecurityQuestionSelectViewModel.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.EditRegistrationPrompt) })]
        public async Task<IActionResult> SelectSecurityQuestionPost(SecurityQuestionSelectViewModel model, string returnUrl)
        {
            if (model.QuestionIndex < 0 || model.QuestionIndex >= this.Settings.SecurityQuestionsToAsk)
            {
                return this.RedirectToAction("Error", "Home");
            }

            SecurityViewModel securityViewModel = await this.multiPageFormService.GetMultiPageFormData<SecurityViewModel>(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
            var userQuestion = securityViewModel.UserSecurityQuestions.ElementAt(model.QuestionIndex);
            userQuestion.SecurityQuestionId = model.SelectedSecurityQuestionId;

            if (userQuestion.SecurityQuestionId > 0)
            {
                bool duplicatesExist = false;
                foreach (var userSecurityQuestion in securityViewModel.UserSecurityQuestions)
                {
                    if (securityViewModel.UserSecurityQuestions.Count(q => q.SecurityQuestionId == userSecurityQuestion.SecurityQuestionId) > 1)
                    {
                        duplicatesExist = true;
                        break;
                    }
                }

                if (duplicatesExist)
                {
                    this.ModelState.AddModelError("DuplicateQuestion", CommonValidationErrorMessages.DuplicateQuestion);
                }
            }

            if (!this.ModelState.IsValid)
            {
                var viewModel = new SecurityQuestionSelectViewModel
                {
                    SecurityQuestions = securityViewModel.SecurityQuestions,
                    QuestionIndex = model.QuestionIndex,
                };

                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("SecurityQuestionSelect", viewModel);
            }

            await this.multiPageFormService.SetMultiPageFormData(securityViewModel, MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);

            return this.RedirectToAction("AnswerSecurityQuestion", new RouteValueDictionary { { "questionIndex", model.QuestionIndex }, { "returnUrl", returnUrl } });
        }

        /// <summary>
        /// Action for displaying the screen to capture the answer to a security question.
        /// </summary>
        /// <param name="questionIndex">The question Id - i.e. 0 or 1.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.EditRegistrationPrompt) })]
        public async Task<IActionResult> AnswerSecurityQuestion(int questionIndex, string returnUrl)
        {
            if (questionIndex < 0 || questionIndex >= this.Settings.SecurityQuestionsToAsk)
            {
                return this.RedirectToAction("Error", "Home");
            }

            SecurityViewModel securityViewModel = await this.multiPageFormService.GetMultiPageFormData<SecurityViewModel>(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
            var userQuestion = securityViewModel.UserSecurityQuestions.ElementAt(questionIndex);
            var selectedSecurityQuestion = securityViewModel.SecurityQuestions.Single(q => q.Value == userQuestion.SecurityQuestionId.ToString());

            var viewModel = new SecurityQuestionAnswerViewModel
            {
                QuestionText = selectedSecurityQuestion.Text,
                QuestionIndex = questionIndex,
            };

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View("SecurityQuestionAnswer", viewModel);
        }

        /// <summary>
        /// Action for displaying the screen to capture the answer to a security question.
        /// </summary>
        /// <param name="model">The SecurityQuestionAnswerViewModel.</param>
        /// <param name="returnUrl">The URL to return to after the login wizard has been completed.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.EditRegistrationPrompt) })]
        public async Task<IActionResult> AnswerSecurityQuestionPost(SecurityQuestionAnswerViewModel model, string returnUrl)
        {
            if (model.QuestionIndex < 0 || model.QuestionIndex >= this.Settings.SecurityQuestionsToAsk)
            {
                return this.RedirectToAction("Error", "Home");
            }

            SecurityViewModel securityViewModel = await this.multiPageFormService.GetMultiPageFormData<SecurityViewModel>(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
            var userQuestion = securityViewModel.UserSecurityQuestions.ElementAt(model.QuestionIndex);
            userQuestion.SecurityQuestionAnswerHash = model.SecurityQuestionAnswer;

            if (!this.ModelState.IsValid)
            {
                var viewModel = new SecurityQuestionSelectViewModel
                {
                    SecurityQuestions = securityViewModel.SecurityQuestions,
                    QuestionIndex = model.QuestionIndex,
                };

                this.ViewBag.ReturnUrl = returnUrl;
                return this.View("SecurityQuestionSelect", viewModel);
            }

            await this.multiPageFormService.SetMultiPageFormData(securityViewModel, MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);

            // If all questions & answers captured, update database.
            if (model.QuestionIndex == securityViewModel.UserSecurityQuestions.Count - 1)
            {
                // Save user's questions & answers via UserApi.
                await this.userService.UpdateUserSecurityQuestions(securityViewModel.UserSecurityQuestions);

                // Mark stage complete.
                var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);

                if (cacheExists)
                {
                    await this.CompleteLoginWizardStageAsync(loginWizard, LoginWizardStageEnum.SecurityQuestions);
                    await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
                    this.TempData.Clear();
                    return this.RedirectToAction("Index", new RouteValueDictionary { { "returnUrl", returnUrl } });
                }

                await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.EditRegistrationPrompt, this.TempData);
                this.TempData.Clear();
                return this.Redirect("/");
            }
            else
            {
                return this.RedirectToAction("SelectSecurityQuestion", new RouteValueDictionary { { "questionIndex", ++model.QuestionIndex }, { "returnUrl", returnUrl } });
            }
        }

        /// <summary>
        /// Show the first of the account related screens - the information screen.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<ActionResult> AccountInformationNeeded()
        {
            var accountModel = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            return this.View(accountModel);
        }

        /// <summary>
        /// Redirects to the correct screen depending on which wizard stages are required. This action is called from the AccountController after
        /// the personal details screens are completed. We either redirect to the first user employment screen if either the JobRole or PlaceOfWork
        /// login wizard stages are required, or to the confirmation screen if they are not. If either the JobRole or PlaceOfWork login wizard
        /// stages are required, all of the user employment screens are displayed, as per the logic in the old JavaScript version of the wizard.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> NextStage()
        {
            var accountModel = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            if (accountModel.WizardStages.Any(x => x.Id == (int)LoginWizardStageEnum.JobRole || x.Id == (int)LoginWizardStageEnum.PlaceOfWork))
            {
                if (string.IsNullOrEmpty(accountModel.CurrentRole))
                {
                    return this.RedirectToAction("CreateAccountSearchRole", "Account");
                }

                return this.RedirectToAction("CreateAccountCurrentRole", "Account", accountModel);
            }
            else
            {
                return this.RedirectToAction("AccountConfirmation");
            }
        }

        /// <summary>
        /// Show the AccountConfirmation screen. This displays all of the data gathered before it is saved to the user's account.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<ActionResult> AccountConfirmation()
        {
            var accountCreation = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            var response = await this.GetAccountConfirmationDetails(accountCreation);

            return this.View(response);
        }

        /// <summary>
        /// Stores all of the data already gathered to the user's account and updates the login wizard activities for the user.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<ActionResult> AccountConfirmationPost()
        {
            var accountModel = await this.multiPageFormService.GetMultiPageFormData<AccountCreationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);

            try
            {
                if (accountModel.WizardStages.Any(x => x.Id == (int)LoginWizardStageEnum.PersonalDetails))
                {
                    var personalDetailsViewModel = new PersonalDetailsViewModel
                    {
                        FirstName = accountModel.FirstName,
                        LastName = accountModel.LastName,
                        SecondaryEmailAddress = accountModel.SecondaryEmailAddress,
                        CountryId = int.TryParse(accountModel.CountryId, out int countryId) ? countryId : 0,
                        RegionId = int.TryParse(accountModel.RegionId, out int regionId) ? regionId : null,
                        UserId = this.CurrentUserId,
                    };
                    await this.userService.UpdatePersonalDetails(personalDetailsViewModel);

                    var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);
                    if (cacheExists)
                    {
                        await this.CompleteLoginWizardStageAsync(loginWizard, LoginWizardStageEnum.PersonalDetails);
                    }
                }

                if (accountModel.WizardStages.Any(x => x.Id == (int)LoginWizardStageEnum.JobRole || x.Id == (int)LoginWizardStageEnum.PlaceOfWork))
                {
                    var userEmploymentViewModel = new UserEmployment
                    {
                        UserId = this.CurrentUserId,
                        Id = int.Parse(accountModel.PrimaryUserEmploymentId),
                        JobRoleId = int.TryParse(accountModel.CurrentRole, out int roleId) ? roleId : 0,
                        GradeId = int.TryParse(accountModel.GradeId, out int gradeId) ? gradeId : 0,
                        MedicalCouncilId = accountModel.MedicalCouncilId,
                        MedicalCouncilNo = accountModel.RegistrationNumber,
                        SpecialtyId = int.TryParse(accountModel.PrimarySpecialtyId, out int specialtyId) ? specialtyId : 0,
                        StartDate = accountModel.StartDate.GetValueOrDefault(),
                        LocationId = int.TryParse(accountModel.LocationId, out int primaryEmploymentId) ? primaryEmploymentId : 0,
                    };
                    await this.userService.UpdateUserEmployment(userEmploymentViewModel);

                    var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);
                    if (cacheExists)
                    {
                        await this.CompleteLoginWizardStageAsync(loginWizard, LoginWizardStageEnum.JobRole);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, "An error has occurred");
                this.Logger.LogError(ex, "Error during completion of Login Wizard.");
                return this.View(accountModel);
            }

            await this.multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.TempData.Clear();
            return this.RedirectToAction("Index", new RouteValueDictionary { { "returnUrl", accountModel.WizardReturnUrl } });
        }

        /// <summary>
        /// The complete login wizard stage.
        /// </summary>
        /// <param name="loginWizard">The loginWizard<see cref="LoginWizardViewModel"/>.</param>
        /// <param name="stage">The stage<see cref="LoginWizardStageEnum"/>.</param>
        private async Task CompleteLoginWizardStageAsync(LoginWizardViewModel loginWizard, LoginWizardStageEnum stage)
        {
            if (stage == LoginWizardStageEnum.PlaceOfWork || stage == LoginWizardStageEnum.JobRole)
            {
                await this.loginWizardService.SaveLoginWizardStageActivity(LoginWizardStageEnum.PlaceOfWork, this.CurrentUserId);
                await this.loginWizardService.SaveLoginWizardStageActivity(LoginWizardStageEnum.JobRole, this.CurrentUserId);
            }
            else
            {
                await this.loginWizardService.SaveLoginWizardStageActivity(stage, this.CurrentUserId);
            }

            loginWizard.CompleteLoginWizardStage(stage);

            var loginWizardViewModel = new LoginWizardStagesViewModel()
            {
                IsFirstLogin = loginWizard.IsFirstLogin,
                LoginWizardStages = loginWizard.LoginWizardStages,
                LoginWizardStagesCompleted = loginWizard.LoginWizardStagesCompleted,
            };

            await this.cacheService.SetAsync(this.LoginWizardCacheKey, JsonConvert.SerializeObject(loginWizardViewModel));
        }

        /// <summary>
        /// The get wizard model.
        /// </summary>
        /// <returns>The <see cref="LoginWizardDisplayViewModel"/>.</returns>
        private async Task<LoginWizardDisplayViewModel> GetWizardModelAsync()
        {
            var (cacheExists, loginWizard) = await this.cacheService.TryGetAsync<LoginWizardViewModel>(this.LoginWizardCacheKey);

            return cacheExists ? new LoginWizardDisplayViewModel(loginWizard) : null;
        }

        private async Task<LoginWizardAccountConfirmation> GetAccountConfirmationDetails(AccountCreationViewModel accountCreationViewModel)
        {
            var country = await this.countryService.GetByIdAsync(int.TryParse(accountCreationViewModel.CountryId, out int countryId) ? countryId : 0);
            var employer = await this.locationService.GetByIdAsync(int.TryParse(accountCreationViewModel.LocationId, out int primaryEmploymentId) ? primaryEmploymentId : 0);
            var region = await this.regionService.GetAllAsync();
            var specialty = await this.specialtyService.GetSpecialtiesAsync();
            var role = await this.jobRoleService.GetPagedFilteredAsync(accountCreationViewModel.CurrentRoleName, accountCreationViewModel.CurrentPageIndex, UserRegistrationContentPageSize);
            if (role.Item1 > 0)
            {
                accountCreationViewModel.CurrentRoleName = role.Item2.FirstOrDefault(x => x.Id == int.Parse(accountCreationViewModel.CurrentRole)).NameWithStaffGroup;
            }

            var grade = await this.gradeService.GetGradesForJobRoleAsync(int.TryParse(accountCreationViewModel.CurrentRole, out int roleId) ? roleId : 0);
            var confirmationPayload = new LoginWizardAccountConfirmation
            {
                AccountCreationViewModel = accountCreationViewModel,
                Country = country?.Name,
                Employer = $"<p>{employer?.Name}</p> <br/> <p>Address:{employer?.Address}</p><br/> <p>Org Code:{employer?.NhsCode}</p>",
                Grade = grade.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.GradeId)?.Name,
                Region = region?.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.RegionId)?.Name,
                Specialty = specialty.FirstOrDefault(x => x.Id.ToString() == accountCreationViewModel.PrimarySpecialtyId)?.Name,
                WizardStages = accountCreationViewModel.WizardStages,
            };
            return confirmationPayload;
        }
    }
}