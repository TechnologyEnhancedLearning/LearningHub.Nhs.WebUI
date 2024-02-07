// <copyright file="UserController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.UserProfile;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ChangePasswordViewModel = LearningHub.Nhs.WebUI.Models.UserProfile.ChangePasswordViewModel;

    /// <summary>
    /// The UserController.
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public partial class UserController : BaseController
    {
        /// <summary>
        /// multipage form service.
        /// </summary>
        private readonly IMultiPageFormService multiPageFormService;

        /// <summary>
        /// The elfh user service..
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The login wizard service..
        /// </summary>
        private readonly ILoginWizardService loginWizardService;

        /// <summary>
        /// The country service..
        /// </summary>
        private readonly ICountryService countryService;

        /// <summary>
        /// The region service..
        /// </summary>
        private readonly IRegionService regionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="userService">userService.</param>
        /// <param name="loginWizardService">loginWizardService.</param>
        /// <param name="countryService">countryService.</param>
        /// <param name="regionService">regionService.</param>
        /// <param name="multiPageFormService">The multiPageFormService<see cref="IMultiPageFormService"/>.</param>
        public UserController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<ResourceController> logger,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            IUserService userService,
            ILoginWizardService loginWizardService,
            ICountryService countryService,
            IRegionService regionService,
            IMultiPageFormService multiPageFormService)
            : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.userService = userService;
            this.loginWizardService = loginWizardService;
            this.countryService = countryService;
            this.regionService = regionService;
            this.multiPageFormService = multiPageFormService;
        }

        /// <summary>
        /// User profile actions.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> Index()
        {
            var userProfileSummary = await this.userService.GetUserProfileSummaryAsync();
            return this.View("Index", userProfileSummary);
        }

        /// <summary>
        /// Get user personal details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/personaldetails")]
        public async Task<IActionResult> PersonalDetails()
        {
            var model = await this.userService.GetUserPersonalDetailsAsync();
            return this.View("PersonalDetails", model);
        }

        /// <summary>
        /// Post user personal details.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("user/personaldetails")]
        public async Task<IActionResult> PersonalDetails(UserPersonalDetailsViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.userService.UpdateUserPersonalDetailsAsync(this.CurrentUserId, model);
                this.TempData["SuccessMessage"] = "Personal Details Updated.";
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        /// <summary>
        /// Get user email details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/emaildetails")]
        public async Task<IActionResult> EmailDetails()
        {
            var model = await this.userService.GetUserEmailDetailsAsync();
            return this.View("EmailDetails", model);
        }

        /// <summary>
        /// Post user email details.
        /// </summary>
        /// <param name="model">model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("user/emaildetails")]
        public async Task<IActionResult> EmailDetails(UserEmailDetailsViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.userService.UpdateUserEmailDetailsAsync(this.CurrentUserId, model);
                this.TempData["SuccessMessage"] = "Email Details Updated.";
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        /// <summary>
        /// Get user email details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/changepassword")]
        public async Task<IActionResult> ChangePassword()
        {
            var personalDetailModel = await this.userService.GetUserPersonalDetailsAsync();
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Username = personalDetailModel.UserName,
            };

            return this.View("ChangePassword", model);
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="model">The model<see cref="ChangePasswordViewModel"/>Change password model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("user/changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.userService.UpdatePassword(model.NewPassword);
                this.TempData["SuccessMessage"] = "Password Updated.";
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        /// <summary>
        /// Get user additional security details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/additionalsecurity")]
        public async Task<IActionResult> AdditionalSecurity()
        {
            var model = await this.loginWizardService.GetSecurityQuestionsModel(this.CurrentUserId);

            if (model != null && model.UserSecurityQuestions != null)
            {
                foreach (var userSecurityQuestion in model.UserSecurityQuestions)
                {
                    if (!string.IsNullOrEmpty(userSecurityQuestion.SecurityQuestionAnswerHash))
                    {
                        userSecurityQuestion.SecurityQuestionAnswerHash = "********";
                    }
                }
            }

            return this.View("AdditionalSecurity", model);
        }

        /// <summary>
        /// Post additional security details.
        /// </summary>
        /// <param name="securityQuestionsModel">The model<see cref="SecurityQuestionsViewModel"/>security questions model.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("user/additionalsecurity")]
        public async Task<IActionResult> AdditionalSecurity(SecurityQuestionsViewModel securityQuestionsModel)
        {
            bool duplicatesExist = false;
            foreach (var userSecurityQuestion in securityQuestionsModel.UserSecurityQuestions)
            {
                userSecurityQuestion.UserId = this.CurrentUserId;

                if (securityQuestionsModel.UserSecurityQuestions.Count(q => q.SecurityQuestionId == userSecurityQuestion.SecurityQuestionId) > 1)
                {
                    duplicatesExist = true;
                    break;
                }
            }

            if (duplicatesExist)
            {
                this.ModelState.AddModelError("DuplicateQuestion", "Select two different questions.");
            }

            if (this.ModelState.IsValid)
            {
                await this.userService.UpdateUserSecurityQuestions(securityQuestionsModel.UserSecurityQuestions);
                this.TempData["SuccessMessage"] = "Additional Security Updated.";
                return this.RedirectToAction("Index");
            }
            else
            {
                var questions = await this.loginWizardService.GetSecurityQuestions();
                securityQuestionsModel.SecurityQuestions = questions.Select(q => new SelectListItem() { Value = q.Id.ToString(), Text = q.Text }).ToList();
            }

            return this.View("AdditionalSecurity", securityQuestionsModel);
        }

        /// <summary>
        /// Get user location country details.
        /// </summary>
        /// <param name="selectedCountryId">country id.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/locationselectcountry")]
        public async Task<IActionResult> LocationSelectCountry(int? selectedCountryId)
        {
            this.TempData.Clear();
            var model = await this.userService.GetUserLocationDetailsAsync();

            if (selectedCountryId.HasValue)
            {
                model.SelectedCountryId = selectedCountryId;
            }

            await this.multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                this.TempData);

            this.ViewBag.Countries = await this.GetCountriesSelectList();

            return this.View("LocationSelectCountry", model);
        }

        /// <summary>
        /// Post user location country details.
        /// </summary>
        /// <param name="model">user location view model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("user/locationselectcountry")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LocationSelectCountry(UserLocationViewModel model)
        {
            var countries = await this.GetCountriesSelectList();

            if (this.ModelState.IsValid)
            {
                var countryEngland = countries.Where(n => n.Text.ToLower() == "england").FirstOrDefault();
                var selectedCountry = countries.Where(n => n.Value == model.SelectedCountryId.Value.ToString()).FirstOrDefault();
                model.SelectedCountryName = selectedCountry?.Text;

                if (countryEngland != null && countryEngland.Value == model.SelectedCountryId.Value.ToString())
                {
                    await this.multiPageFormService.SetMultiPageFormData(model, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    return this.RedirectToAction("LocationSelectRegion");
                }
                else
                {
                    model.SelectedRegionId = null;
                    await this.multiPageFormService.SetMultiPageFormData(model, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                    return this.RedirectToAction("LocationConfirmation");
                }
             }

            this.ViewBag.Countries = countries;
            return this.View("LocationSelectCountry", model);
        }

        /// <summary>
        /// Get user location region details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/locationselectregion")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> LocationSelectRegion()
        {
            var model = await this.multiPageFormService.GetMultiPageFormData<UserLocationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            this.ViewBag.Regions = await this.GetRegionsSelectList();
            return this.View("LocationSelectRegion", model);
        }

        /// <summary>
        /// Post user location region details.
        /// </summary>
        /// <param name="model">user location view model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("user/locationselectregion")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LocationSelectRegion(UserLocationViewModel model)
        {
            if (model.SelectedRegionId.HasValue && model.SelectedRegionId.Value > 0)
            {
                var region = await this.regionService.GetByIdAsync(model.SelectedRegionId.Value);
                model.SelectedRegionName = region?.Name;
                await this.multiPageFormService.SetMultiPageFormData(model, MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
                return this.RedirectToAction("LocationConfirmation");
            }

            return this.View(model);
        }

        /// <summary>
        /// Get user location confirmation details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("user/locationconfirmation")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> LocationConfirmation()
        {
            var model = await this.multiPageFormService.GetMultiPageFormData<UserLocationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            return this.View("LocationConfirmation", model);
        }

        /// <summary>
        /// Post user location region details.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("user/locationconfirmation")]
        [TypeFilter(typeof(RedirectMissingMultiPageFormData), Arguments = new object[] { nameof(MultiPageFormDataFeature.AddRegistrationPrompt) })]
        public async Task<IActionResult> UpdateLocationConfirmation()
        {
            var model = await this.multiPageFormService.GetMultiPageFormData<UserLocationViewModel>(MultiPageFormDataFeature.AddRegistrationPrompt, this.TempData);
            await this.userService.UpdateUserLocationDetailsAsync(this.CurrentUserId, model);
            return this.RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetCountriesSelectList()
        {
            var countrylist = await this.countryService.GetAllAsync();
            return countrylist.Select(q => new SelectListItem() { Value = q.Id.ToString(), Text = q.Name })
                                                                 .ToList();
        }

        private async Task<List<SelectListItem>> GetRegionsSelectList()
        {
            var regionlist = await this.regionService.GetAllAsync();
            return regionlist.Select(q => new SelectListItem() { Value = q.Id.ToString(), Text = q.Name })
                                                                 .ToList();
        }
    }
}