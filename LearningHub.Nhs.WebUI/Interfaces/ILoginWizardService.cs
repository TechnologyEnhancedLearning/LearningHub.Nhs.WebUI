// <copyright file="ILoginWizardService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;

    /// <summary>
    /// Defines the <see cref="ILoginWizardService" />.
    /// </summary>
    public interface ILoginWizardService
    {
        /// <summary>
        /// The CompleteLoginWizard.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CompleteLoginWizard(int userId);

        /// <summary>
        /// The GetLoginWizard.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LoginWizardStagesViewModel> GetLoginWizard(int userId);

        /// <summary>
        /// The GetSecurityQuestions.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<SecurityQuestion>> GetSecurityQuestions();

        /// <summary>
        /// The GetSecurityQuestionsModel.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<SecurityQuestionsViewModel> GetSecurityQuestionsModel(int userId);

        /// <summary>
        /// The SaveLoginWizardStageActivity.
        /// </summary>
        /// <param name="loginWizardStageEnum">Login wizard stage.</param>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task SaveLoginWizardStageActivity(LoginWizardStageEnum loginWizardStageEnum, int userId);

        /// <summary>
        /// The StartLoginWizard.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task StartLoginWizard(int userId);
    }
}
