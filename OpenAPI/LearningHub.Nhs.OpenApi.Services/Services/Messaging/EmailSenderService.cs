namespace LearningHub.Nhs.OpenApi.Services.Services.Messaging
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;

    /// <summary>
    /// The EmailSenderService class.
    /// </summary>
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IEmailTemplateService emailTemplateService;
        private readonly IMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSenderService"/> class.
        /// </summary>
        /// <param name="emailTemplateService">The emailTemplateService.</param>
        /// <param name="messageService">The messageService.</param>
        public EmailSenderService(IEmailTemplateService emailTemplateService, IMessageService messageService)
        {
            this.emailTemplateService = emailTemplateService;
            this.messageService = messageService;
        }

        /// <summary>
        /// Sends access request success emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        public async Task SendRequestAccessSuccessEmail(int userId, SendEmailModel<CatalogueAccessRequestSuccessEmailModel> model)
        {
            var template = emailTemplateService.GetCatalogueAccessRequestSuccess(model);
            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }

        /// <summary>
        /// Sends access request failure emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        public async Task SendRequestAccessFailureEmail(int userId, SendEmailModel<CatalogueAccessRequestFailureEmailModel> model)
        {
            var template = emailTemplateService.GetCatalogueAccessRequestFailure(model);
            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }

        /// <summary>
        /// Sends access request failure emails to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        public async Task SendAccessRequestInviteEmail(int userId, SendEmailModel<CatalogueAccessInviteEmailModel> model)
        {
            var template = emailTemplateService.GetCatalogueAccessInvitation(model);
            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }

        /// <summary>
        /// Sends report generation completion email to user.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        public async Task SendReportProcessedEmail(int userId, SendEmailModel<ReportSucessEmailModel> model)
        {
            var template = emailTemplateService.GetReportProcessed(model);
            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }


        /// <summary>
        /// Sends email change confirmation email.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The task.</returns>
        public async Task SendEmailChangeConfirmationEmail(int userId, SendEmailModel<EmailChangeConfirmationEmailModel> model, bool isUserRoleUpgrade)
        {
            var template = emailTemplateService.GetEmailChangeConfirmation(model, isUserRoleUpgrade);
            if (!isUserRoleUpgrade)
            {
                template.Subject = "Thank you for updating your email address";
            }

            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }

        /// <summary>
        /// Sends email change confirmation email.
        /// </summary>
        /// <param name="userId">The userId sending the email.</param>
        /// <param name="model">The model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The task.</returns>
        public async Task SendEmailVerifiedEmail(int userId, SendEmailModel<EmailChangeConfirmationEmailModel> model, bool isUserRoleUpgrade)
        {
            var template = emailTemplateService.GetEmailVerificationEmail(model, isUserRoleUpgrade);
            await messageService.CreateEmailAsync(userId, template.Subject, template.Body, template.EmailAddress);
        }
    }
}
