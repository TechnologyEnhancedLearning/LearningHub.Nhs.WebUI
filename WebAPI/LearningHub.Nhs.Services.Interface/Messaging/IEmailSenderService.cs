// <copyright file = "IEmailSenderService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.Interface.Messaging
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;

    /// <summary>
    /// The IEmailSenderService.
    /// </summary>
    public interface IEmailSenderService
    {
        /// <summary>
        /// Sends access request success emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        Task SendRequestAccessSuccessEmail(int userId, SendEmailModel<CatalogueAccessRequestSuccessEmailModel> model);

        /// <summary>
        /// Sends access request failure emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        Task SendRequestAccessFailureEmail(int userId, SendEmailModel<CatalogueAccessRequestFailureEmailModel> model);

        /// <summary>
        /// Sends access request failure emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        Task SendAccessRequestInviteEmail(int userId, SendEmailModel<CatalogueAccessInviteEmailModel> model);

        /// <summary>
        /// Sends email change confirmation.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The task.</returns>
        Task SendEmailChangeConfirmationEmail(int userId, SendEmailModel<EmailChangeConfirmationEmailModel> model, bool isUserRoleUpgrade);

        /// <summary>
        /// Sends email verified confirmation.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The task.</returns>
        Task SendEmailVerifiedEmail(int userId, SendEmailModel<EmailChangeConfirmationEmailModel> model, bool isUserRoleUpgrade);
    }
}
