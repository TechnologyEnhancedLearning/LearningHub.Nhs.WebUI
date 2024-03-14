// <copyright file = "IEmailTemplateService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface.Messaging
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;

    /// <summary>
    /// The IEmailTemplateService.
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// The GetCatalogueAccessRequest.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>The subject and body.</returns>
        List<EmailDetails> GetCatalogueAccessRequest(List<SendEmailModel<CatalogueAccessRequestEmailModel>> models);

        /// <summary>
        /// The GetCatalogueAccessRequestSuccess.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        EmailDetails GetCatalogueAccessRequestSuccess(SendEmailModel<CatalogueAccessRequestSuccessEmailModel> emailModel);

        /// <summary>
        /// The GetCatalogueAccessRequestFailure.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        EmailDetails GetCatalogueAccessRequestFailure(SendEmailModel<CatalogueAccessRequestFailureEmailModel> emailModel);

        /// <summary>
        /// The GetCatalogueAccessRequestFailure.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        EmailDetails GetCatalogueAccessInvitation(SendEmailModel<CatalogueAccessInviteEmailModel> emailModel);

        /// <summary>
        /// The GetConfirmEmailChangeValidation.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The subject and body.</returns>
        EmailDetails GetEmailChangeConfirmation(SendEmailModel<EmailChangeConfirmationEmailModel> emailModel, bool isUserRoleUpgrade);

        /// <summary>
        /// The GetEmailVerificationEmail.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The subject and body.</returns>
        EmailDetails GetEmailVerificationEmail(SendEmailModel<EmailChangeConfirmationEmailModel> emailModel, bool isUserRoleUpgrade);
    }
}
