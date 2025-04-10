namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Email;
    using LearningHub.Nhs.Models.Email.Models;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services.Messaging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The security service.
    /// </summary>
    public class SecurityService : ISecurityService
    {
        /// <summary>
        /// The user password validation token repository.
        /// </summary>
        private readonly IEmailChangeValidationTokenRepository emailChangeValidationTokenRepository;
        private readonly IEmailSenderService emailSenderService;
        private readonly LearningHubConfig learningHubConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        /// <param name="emailChangeValidationTokenRepository">emailChangeValidationTokenRepository.</param>
        /// <param name="emailSenderService">emailSenderService.</param>
        /// <param name="settings">settings.</param>
        public SecurityService(
            IEmailChangeValidationTokenRepository emailChangeValidationTokenRepository,
            IEmailSenderService emailSenderService,
            IOptions<LearningHubConfig> learningHubConfig)
        {
            this.emailChangeValidationTokenRepository = emailChangeValidationTokenRepository;
            this.emailSenderService = emailSenderService;
            this.learningHubConfig = learningHubConfig.Value;
        }

        /// <summary>
        /// GenerateEmailChangeValidationTokenAndSendEmail.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="email">email.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GenerateEmailChangeValidationTokenAndSendEmail(int userId, string email, bool isUserRoleUpgrade)
        {
            string salt;
            var expiryMinutes = this.learningHubConfig.EmailConfirmationTokenExpiryMinutes;
            var userToken = Convert.ToBase64String(UniqueId().ToByteArray());
            var lookupToken = Convert.ToBase64String(UniqueId().ToByteArray());
            var hashedToken = SecureHash(userToken, out salt);
            var emailChangeValidationToken = new EmailChangeValidationToken()
            {
                HashedToken = hashedToken,
                Salt = salt,
                Lookup = lookupToken,
                Expiry = DateTimeOffset.Now.AddMinutes(expiryMinutes),
                TenantId = this.learningHubConfig.LearningHubTenantId,
                UserId = userId,
                Email = email,
                StatusId = (int)EmailChangeValidationTokenStatusEnum.Issued,
            };

            var validateTokenUrl = $"{this.learningHubConfig.BaseUrl}confirm-email?token={HttpUtility.UrlEncode(userToken)}&loctoken={HttpUtility.UrlEncode(lookupToken)}";
            await SendEmailChangeConfirmationEmail(userId, email, validateTokenUrl, isUserRoleUpgrade);
            await emailChangeValidationTokenRepository.CreateAsync(userId, emailChangeValidationToken);
        }

        /// <summary>
        /// UserDetailsValidateTokenAsync.
        /// </summary>
        /// <param name="token">token.</param>
        /// <param name="loctoken">loctoken.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EmailChangeValidationTokenResult> ValidateEmailChangeTokenAsync(string token, string loctoken, bool isUserRoleUpgrade)
        {
            var tokenResult = new EmailChangeValidationTokenResult { Valid = false, TokenIssue = string.Empty };

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loctoken))
            {
                tokenResult.TokenIssue = "Invalid";
                return tokenResult;
            }

            var emailChangeValidationToken = await emailChangeValidationTokenRepository.GetByToken(loctoken);
            if (emailChangeValidationToken == null || emailChangeValidationToken.StatusId == (int)EmailChangeValidationTokenStatusEnum.Abandoned)
            {
                tokenResult.TokenIssue = "Expired";
            }
            else if (emailChangeValidationToken == null || emailChangeValidationToken.StatusId != (int)EmailChangeValidationTokenStatusEnum.Issued)
            {
                tokenResult.TokenIssue = "Invalid";
            }
            else
            {
                var hashedToken = this.SecureHash(token, emailChangeValidationToken.Salt);

                if (!emailChangeValidationToken.HashedToken.Equals(hashedToken))
                {
                    tokenResult.TokenIssue = "Invalid";
                }
                else if (emailChangeValidationToken.HashedToken.Equals(hashedToken) && emailChangeValidationToken.Expiry < DateTimeOffset.Now)
                {
                    tokenResult.TokenIssue = "Expired";
                    await emailChangeValidationTokenRepository.ExpireEmailChangeValidationToken(emailChangeValidationToken.Lookup);
                }
                else
                {
                    emailChangeValidationToken.StatusId = (int)EmailChangeValidationTokenStatusEnum.Completed;
                    await emailChangeValidationTokenRepository.UpdateAsync(emailChangeValidationToken.UserId, emailChangeValidationToken);
                    tokenResult.UserName = emailChangeValidationToken.User.UserName;
                    tokenResult.Email = emailChangeValidationToken.Email;
                    tokenResult.UserId = emailChangeValidationToken.UserId;
                    tokenResult.Valid = true;
                    await this.SendEmailAfterEmailVerification(emailChangeValidationToken.UserId, emailChangeValidationToken.Email, isUserRoleUpgrade);
                }
            }

            return tokenResult;
        }

        /// <summary>
        /// GetLastIssuedEmailChangeValidationToken.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<EmailChangeValidationTokenViewModel> GetLastIssuedEmailChangeValidationToken(int userId)
        {
            var result = await emailChangeValidationTokenRepository.GetLastIssuedEmailChangeValidationToken(userId);
            return result == null
                ? null
                : new EmailChangeValidationTokenViewModel
                {
                    Email = result.Email,
                    Lookup = result.Lookup,
                    StatusId = result.StatusId,
                };
        }

        /// <inheritdoc/>
        public async Task CancelEmailChangeValidationToken(int userId)
        {
            var emailChangeValidationToken = await emailChangeValidationTokenRepository.GetLastIssuedEmailChangeValidationToken(userId);
            if (emailChangeValidationToken != null)
            {
                emailChangeValidationToken.StatusId = (int)EmailChangeValidationTokenStatusEnum.Cancelled;
                await emailChangeValidationTokenRepository.UpdateAsync(emailChangeValidationToken.UserId, emailChangeValidationToken);
            }
        }

        /// <inheritdoc/>
        public async Task<EmailChangeValidationTokenViewModel> ReGenerateEmailChangeValidationToken(int userId, string newPrimaryEmail, bool isUserRoleUpgrade)
        {
            var emailChangeValidationToken = await emailChangeValidationTokenRepository.GetLastIssuedEmailChangeValidationToken(userId);
            if (emailChangeValidationToken != null)
            {
                emailChangeValidationToken.StatusId = (int)EmailChangeValidationTokenStatusEnum.Abandoned;
                await emailChangeValidationTokenRepository.UpdateAsync(emailChangeValidationToken.UserId, emailChangeValidationToken);
            }

            await GenerateEmailChangeValidationTokenAndSendEmail(userId, newPrimaryEmail, isUserRoleUpgrade);
            return await GetLastIssuedEmailChangeValidationToken(userId);
        }

        /// <summary>
        /// SendEmailChangeConfirmirmationEmail.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="emailAddress">emailAddress.</param>
        /// <param name="validationTokenUrl">validationTokenUrl.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private async Task SendEmailChangeConfirmationEmail(int userId, string emailAddress, string validationTokenUrl, bool isUserRoleUpgrade)
        {
            var expiryMinutes = this.learningHubConfig.EmailConfirmationTokenExpiryMinutes;
            var supportPage = this.learningHubConfig.SupportPages;
            var supportForm = this.learningHubConfig.SupportForm;
            var model = new SendEmailModel<EmailChangeConfirmationEmailModel>(new EmailChangeConfirmationEmailModel
            {
                ValidationTokenUrl = validationTokenUrl,
                TimeLimit = expiryMinutes,
                EmailAddress = emailAddress,
                SupportPages = supportPage,
                SupportForm = supportForm,
            });
            await emailSenderService.SendEmailChangeConfirmationEmail(userId, model, isUserRoleUpgrade);
        }

        /// <summary>
        /// SendEmailafterverification.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="emailAddress">emailAddress.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private async Task SendEmailAfterEmailVerification(int userId, string emailAddress, bool isUserRoleUpgrade)
        {
            var supportForm = this.learningHubConfig.SupportForm;
            var model = new SendEmailModel<EmailChangeConfirmationEmailModel>(new EmailChangeConfirmationEmailModel
            {
                EmailAddress = emailAddress,
                SupportForm = supportForm,
            });
            await emailSenderService.SendEmailVerifiedEmail(userId, model, isUserRoleUpgrade);
        }

        /// <summary>
        /// Generate a PBKDF2 hash and generate a salt.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="salt">
        /// The salt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SecureHash(string value, out string salt)
        {
            salt = GenerateSalt();
            return Hash(value, salt);
        }

        /// <summary>
        /// The generate salt.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateSalt()
        {
            var bytes = Random(16);
            var iterations = 1000;
            return iterations + "." + Convert.ToBase64String(bytes);
        }

        private string SecureHash(string value, string salt)
        {
            return Hash(value, salt);
        }

        private string Hash(string value, string salt)
        {
            var i = salt.IndexOf('.');
            var iterations = int.Parse(salt.Substring(0, i), System.Globalization.NumberStyles.HexNumber);
            salt = salt.Substring(i + 1);

            using (var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(value), Convert.FromBase64String(salt), iterations))
            {
                var key = pbkdf2.GetBytes(24);

                return Convert.ToBase64String(key);
            }
        }

        /// <summary>
        /// Creates a Version 4 random Guid.
        /// https://en.wikipedia.org/wiki/Universally_unique_identifier#Version_4_.28random.29.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        private Guid UniqueId()
        {
            var bytes = Random(16);
            bytes[7] = (byte)(bytes[7] & 0x0F | 0x40);
            bytes[8] = (byte)(bytes[8] & 0x0F | 0x80 + Random(1)[0] % 4);

            return new Guid(bytes);
        }

        /// <summary>
        /// The random.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] Random(int bytes)
        {
            RandomNumberGenerator randomSource = RandomNumberGenerator.Create();

            var ret = new byte[bytes];
            lock (randomSource)
            {
                randomSource.GetBytes(ret);
            }

            return ret;
        }
    }
}
