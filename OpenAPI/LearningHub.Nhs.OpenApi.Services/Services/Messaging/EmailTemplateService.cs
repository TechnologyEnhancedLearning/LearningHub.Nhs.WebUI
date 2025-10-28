namespace LearningHub.Nhs.OpenApi.Services.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;

    /// <summary>
    /// The EmailTemplateService.
    /// </summary>
    public class EmailTemplateService : IEmailTemplateService
    {
        private const string LeftBracket = "[";
        private const string RightBracket = "]";

        private readonly IEmailTemplateRepository emailTemplateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
        /// </summary>
        /// <param name="emailTemplateRepository">The emailTemplateRepository.</param>
        public EmailTemplateService(IEmailTemplateRepository emailTemplateRepository)
        {
            this.emailTemplateRepository = emailTemplateRepository;
        }

        /// <summary>
        /// The GetCatalogueAccessRequest.
        /// </summary>
        /// <param name="emailModels">The email models.</param>
        /// <returns>The subject and body.</returns>
        public List<EmailDetails> GetCatalogueAccessRequest(List<SendEmailModel<CatalogueAccessRequestEmailModel>> emailModels)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.CatalogueAccessRequest);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var emailDetails = new List<EmailDetails>();
            foreach (var emailModel in emailModels)
            {
                var model = emailModel.Model;
                var replacementDict = new Dictionary<string, string>
                {
                    ["UserFullName"] = model.UserFullName,
                    ["UserEmailAddress"] = model.UserEmailAddress,
                    ["AdminFirstName"] = model.AdminFirstName,
                    ["CatalogueName"] = model.CatalogueName,
                    ["ManageAccessUrl"] = model.ManageAccessUrl,
                    ["UserMessage"] = model.UserMessage,
                };

                var subject = this.Replace(emailTemplate.Subject, replacementDict);
                var body = Replace(emailBody, replacementDict);
                emailDetails.Add(new EmailDetails { Body = body, Subject = subject, UserId = emailModel.UserId });
            }

            return emailDetails;
        }

        /// <summary>
        /// The GetCatalogueAccessRequestSuccess.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        public EmailDetails GetCatalogueAccessRequestSuccess(SendEmailModel<CatalogueAccessRequestSuccessEmailModel> emailModel)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.CatalogueAccessRequestSuccess);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var model = emailModel.Model;
            var replacementDict = new Dictionary<string, string>
            {
                ["UserFirstName"] = model.UserFirstName,
                ["CatalogueUrl"] = model.CatalogueUrl,
                ["CatalogueName"] = model.CatalogueName,
            };

            var subject = this.Replace(emailTemplate.Subject, replacementDict);
            var body = Replace(emailBody, replacementDict);
            return new EmailDetails { Body = body, Subject = subject, EmailAddress = emailModel.EmailAddress };
        }

        /// <summary>
        /// The GetCatalogueAccessRequestFailure.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        public EmailDetails GetCatalogueAccessRequestFailure(SendEmailModel<CatalogueAccessRequestFailureEmailModel> emailModel)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.CatalogueAccessRequestFailure);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var model = emailModel.Model;
            var replacementDict = new Dictionary<string, string>
            {
                ["UserFirstName"] = model.UserFirstName,
                ["RejectionReason"] = model.RejectionReason,
                ["CatalogueName"] = model.CatalogueName,
                ["CatalogueUrl"] = model.CatalogueUrl,
            };

            var subject = this.Replace(emailTemplate.Subject, replacementDict);
            var body = Replace(emailBody, replacementDict);
            return new EmailDetails { Body = body, Subject = subject, EmailAddress = emailModel.EmailAddress };
        }

        /// <summary>
        /// The GetCatalogueAccessRequestFailure.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>The subject and body.</returns>
        public EmailDetails GetCatalogueAccessInvitation(SendEmailModel<CatalogueAccessInviteEmailModel> emailModel)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.CatalogueAccessInvitation);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var model = emailModel.Model;
            var replacementDict = new Dictionary<string, string>
            {
                ["AdminFullName"] = model.AdminFullName,
                ["CatalogueUrl"] = model.CatalogueUrl,
                ["CatalogueName"] = model.CatalogueName,
                ["CreateAccountUrl"] = model.CreateAccountUrl,
                ["Greeting"] = model.Greeting,
            };

            var subject = this.Replace(emailTemplate.Subject, replacementDict);
            var body = Replace(emailBody, replacementDict);
            return new EmailDetails { Body = body, Subject = subject, EmailAddress = emailModel.EmailAddress };
        }

        /// <summary>
        /// The GetEmailChangeConfirmationEmail.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The subject and body.</returns>
        public EmailDetails GetEmailChangeConfirmation(SendEmailModel<EmailChangeConfirmationEmailModel> emailModel, bool isUserRoleUpgrade)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.EmailChangeConfirmationEmail);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var model = emailModel.Model;
            var expiraryDate = DateTimeOffset.Now.AddMinutes(model.TimeLimit);
            var replacementDict = new Dictionary<string, string>
            {
                ["UserFirstName"] = model.UserFirstName,
                ["UserName"] = model.UserName,
                ["EmailAddress"] = model.EmailAddress,
                ["ValidationTokenUrl"] = model.ValidationTokenUrl,
                ["ExpiredDate"] = expiraryDate.ToString("dd-MM-yyyy"),
                ["ExpiredTime"] = expiraryDate.ToString("HH:mm"),
                ["SupportForm"] = model.SupportForm,
                ["SupportPages"] = model.SupportPages,
            };

            var subject = this.Replace(emailTemplate.Subject, replacementDict);
            var body = Replace(emailBody, replacementDict);
            if (!isUserRoleUpgrade)
            {
                body = body.Replace("Thanks again for upgrading to a Full user account, and we hope you enjoy your learning experience.", string.Empty);
            }

            return new EmailDetails { Body = body, Subject = subject, EmailAddress = model.EmailAddress };
        }

        /// <summary>
        /// The GetEmailVerificationEmail.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>The subject and body.</returns>
        public EmailDetails GetEmailVerificationEmail(SendEmailModel<EmailChangeConfirmationEmailModel> emailModel, bool isUserRoleUpgrade)
        {
            var emailTemplate = emailTemplateRepository.GetTemplate((int)EmailTemplates.EmailVerified);
            var emailBody = this.Replace(emailTemplate.EmailTemplateLayout.Body, new Dictionary<string, string>
            {
                ["Content"] = emailTemplate.Body,
            });
            var model = emailModel.Model;
            var replacementDict = new Dictionary<string, string>
            {
                ["SupportForm"] = model.SupportForm,
            };

            var subject = this.Replace(emailTemplate.Subject, replacementDict);
            var body = Replace(emailBody, replacementDict);
            if (!isUserRoleUpgrade)
            {
                body = body.Replace("Thanks again for upgrading to a Full user account, and we hope you enjoy your learning experience.", string.Empty);
            }

            return new EmailDetails { Body = body, Subject = subject, EmailAddress = model.EmailAddress };
        }

        private string Replace(string str, Dictionary<string, string> replacements)
        {
            return replacements.Aggregate(
                str,
                (acc, val) =>
                    acc.Replace(LeftBracket + val.Key + RightBracket, val.Value));
        }
    }
}
