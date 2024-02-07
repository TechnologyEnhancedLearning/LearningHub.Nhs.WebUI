// <copyright file="LoginWizardViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="LoginWizardViewModel" />.
    /// </summary>
    public class LoginWizardViewModel : LoginWizardStagesViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWizardViewModel"/> class.
        /// </summary>
        public LoginWizardViewModel()
        {
        }

        /// <summary>
        /// Gets the CurrentLoginWizardStage.
        /// </summary>
        public LoginWizardStage CurrentLoginWizardStage
        {
            get
            {
                return this.LoginWizardStagesRemaining.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the LoginWizardStagesRemaining.
        /// </summary>
        public List<LoginWizardStage> LoginWizardStagesRemaining
        {
            get
            {
                return this.LoginWizardStages
                        .Where(s => !this.LoginWizardStagesCompleted.Select(c => c.Id).Contains(s.Id))
                        .OrderBy(s => s.Id).ToList();
            }
        }

        /// <summary>
        /// The CompleteLoginWizardStage.
        /// </summary>
        /// <param name="loginWizardStageEnum">The loginWizardStageEnum<see cref="LoginWizardStageEnum"/>.</param>
        public void CompleteLoginWizardStage(LoginWizardStageEnum loginWizardStageEnum)
        {
            if (loginWizardStageEnum == LoginWizardStageEnum.PlaceOfWork || loginWizardStageEnum == LoginWizardStageEnum.JobRole)
            {
                var stagePlaceOfWork = this.LoginWizardStages.Where(s => s.Id == (int)LoginWizardStageEnum.PlaceOfWork).FirstOrDefault();
                var stageJobRole = this.LoginWizardStages.Where(s => s.Id == (int)LoginWizardStageEnum.JobRole).FirstOrDefault();
                if (stagePlaceOfWork != null)
                {
                    this.LoginWizardStagesCompleted.Add(stagePlaceOfWork);
                }

                if (stageJobRole != null)
                {
                    this.LoginWizardStagesCompleted.Add(stageJobRole);
                }
            }
            else
            {
                var stage = this.LoginWizardStages.Where(s => s.Id == (int)loginWizardStageEnum).FirstOrDefault();
                if (stage == null)
                {
                    throw new ApplicationException("Supplied Login Wizard Stage is not valid");
                }

                this.LoginWizardStagesCompleted.Add(stage);
            }
        }

        /// <summary>
        /// The GetWizardStageDescription.
        /// </summary>
        /// <param name="stageId">The stageId<see cref="int"/>.</param>
        /// <returns>The .</returns>
        public string GetWizardStageDescription(int stageId)
        {
            LoginWizardStageEnum loginWizardStageEnum = (LoginWizardStageEnum)stageId;
            string retVal = string.Empty;
            switch (loginWizardStageEnum)
            {
                case LoginWizardStageEnum.TermsAndConditions:
                    retVal = "Terms and conditions";
                    break;
                case LoginWizardStageEnum.PasswordReset:
                    retVal = "Change your password";
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// The GetWizardStageText.
        /// </summary>
        /// <param name="stageId">The stageId<see cref="int"/>.</param>
        /// <param name="isFirstLogin">The isFirstLogin<see cref="bool"/>.</param>
        /// <param name="jobRoleId">The jobRoleId<see cref="T:int?"/>.</param>
        /// <param name="location">The location<see cref="Location"/>.</param>
        /// <returns>The .</returns>
        public string GetWizardStageText(int stageId, bool isFirstLogin, int? jobRoleId = 0, Location location = null)
        {
            LoginWizardStageEnum loginWizardStageEnum = (LoginWizardStageEnum)stageId;
            string retVal = string.Empty;

            switch (loginWizardStageEnum)
            {
                case LoginWizardStageEnum.TermsAndConditions:
                    if (isFirstLogin)
                    {
                        retVal = "You need to accept the Learning Hub terms and conditions before you can use the Hub.";
                    }
                    else
                    {
                        retVal = "You need to accept the latest Learning Hub terms and conditions before you can continue to use the Hub.";
                    }

                    break;
                case LoginWizardStageEnum.PasswordReset:
                    retVal = string.Empty;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// The IsWizardComplete.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsWizardComplete()
        {
            return this.LoginWizardStages.Count() > 0 && this.LoginWizardStagesRemaining.Count() == 0;
        }
    }
}
