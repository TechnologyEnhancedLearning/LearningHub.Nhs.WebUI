namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Entities;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.OpenAthens;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.UserProfile;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Login = elfhHub.Nhs.Models.Common.Login;

    /// <summary>
    /// Defines the <see cref="IUserService" />.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// The get active content async.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        /// <exception cref="Exception">Access Denied.
        /// </exception>
        Task<List<ActiveContentViewModel>> GetActiveContentAsync();

        /// <summary>
        /// The CheckUserCredentialsAsync.
        /// </summary>
        /// <param name="login">Login.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<LoginResultInternal> CheckUserCredentialsAsync(Login login);

        /// <summary>
        /// The CreateElfhAccountWithLinkedOpenAthensAsync.
        /// </summary>
        /// <param name="newUserDetails">New user detail.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> CreateElfhAccountWithLinkedOpenAthensAsync(CreateOpenAthensLinkToLhUser newUserDetails);

        /// <summary>
        /// GetCurrentUserPersonalDetailsAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PersonalDetailsViewModel> GetCurrentUserPersonalDetailsAsync();

        /// <summary>
        /// The CreateOpenAthensLinkToUserAsync.
        /// </summary>
        /// <param name="lhUserId">LH User id.</param>
        /// <param name="openAthensUserId">OA user id.</param>
        /// <param name="openAthensOrgId">OA og id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CreateOpenAthensLinkToUserAsync(int lhUserId, string openAthensUserId, string openAthensOrgId);

        /// <summary>
        /// The DoesEmailAlreadyExist.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DoesEmailAlreadyExist(string emailAddress);

        /// <summary>
        /// The ForgotPasswordAsync.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ForgotPasswordAsync(string email);

        /// <summary>
        /// The GetCurrentUserAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserViewModel> GetCurrentUserAsync();

        /// <summary>
        /// The get current user profile async.
        /// </summary>
        /// <returns>The <see cref="Task{UserProfile}"/>.</returns>
        Task<UserProfile> GetCurrentUserProfileAsync();

        /// <summary>
        /// The GetCurrentUserBasicDetailsAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserBasicViewModel> GetCurrentUserBasicDetailsAsync();

        /// <summary>
        /// The GetEmailAddressRegistrationStatusAsync.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="ipAddress">The user ip address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<EmailRegistrationStatus> GetEmailAddressRegistrationStatusAsync(string emailAddress, string ipAddress);

        /// <summary>
        /// To validate user role upgrade or not.
        /// </summary>
        /// <param name="currentPrimaryEmail">Current email address.</param>
        /// <param name="newPrimaryEmail">New email address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> ValidateUserRoleUpgradeAsync(string currentPrimaryEmail, string newPrimaryEmail);

        /// <summary>
        /// To validate primary email address.
        /// </summary>
        /// <param name="primaryEmailAddress">email address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> GetEmailAddressStatusAsync(string primaryEmailAddress);

        /// <summary>
        /// To check same primary email is pending to validate.
        /// </summary>
        /// <param name="secondaryEmail">secondary email address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CheckSamePrimaryemailIsPendingToValidate(string secondaryEmail);

        /// <summary>
        /// The GetTenantDescriptionByUserId.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<TenantDescription> GetTenantDescriptionByUserId(int userId);

        /// <summary>
        /// The GetUserByUserIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserViewModel> GetUserByUserIdAsync(int id);

        /// <summary>
        /// The GetLHUserByUserIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserLHBasicViewModel> GetLHUserByUserIdAsync(int id);

        /// <summary>
        /// The GetUserEmploymentByIdAsync.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserEmployment> GetUserEmploymentByIdAsync(int id);

        /// <summary>
        /// The GetPrimaryUserEmploymentForUser.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserEmploymentViewModel> GetPrimaryUserEmploymentForUser(int userId);

        /// <summary>
        /// The GetUserRoleName.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetUserRoleName(int userId);

        /// <summary>
        /// The HasMultipleUsersForEmailAsync.
        /// </summary>
        /// <param name="emailAddress">Email Address.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> HasMultipleUsersForEmailAsync(string emailAddress);

        /// <summary>
        /// The RegisterNewUser.
        /// </summary>
        /// <param name="registrationRequest">Registration request.</param>
        /// <returns>A <see cref="LearningHubValidationResult"/> representing the result of the asynchronous operation.</returns>
        Task<LearningHubValidationResult> RegisterNewUser(RegistrationRequestViewModel registrationRequest);

        /// <summary>
        /// The SetUserInitialPasswordAsync.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="loctoken">Loc token.</param>
        /// <param name="password">Password.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> SetUserInitialPasswordAsync(string token, string loctoken, string password);

        /// <summary>
        /// The StoreUserHistory.
        /// </summary>
        /// <param name="userHistory">User history.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task StoreUserHistory(UserHistoryViewModel userHistory);

        /// <summary>
        /// The UpdateLoginWizardFlag.
        /// </summary>
        /// <param name="loginWizardInProgress">Login wizard progress.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateLoginWizardFlag(bool loginWizardInProgress);

        /// <summary>
        /// The UpdatePassword.
        /// </summary>
        /// <param name="newPassword">New password.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdatePassword(string newPassword);

        /// <summary>
        /// The UpdatePersonalDetails.
        /// </summary>
        /// <param name="personalDetailsViewModel">Personal details.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdatePersonalDetails(PersonalDetailsViewModel personalDetailsViewModel);

        /// <summary>
        /// The UpdateUserEmployment.
        /// </summary>
        /// <param name="userEmployment">User employment.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateUserEmployment(UserEmployment userEmployment);

        /// <summary>
        /// The UpdateUserSecurityQuestions.
        /// </summary>
        /// <param name="userSecurityQuestions">Security questions.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateUserSecurityQuestions(List<UserSecurityQuestionViewModel> userSecurityQuestions);

        /// <summary>
        /// The ValidateUserPasswordTokenAsync.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="loctoken">Loc token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PasswordValidationTokenResult> ValidateUserPasswordTokenAsync(string token, string loctoken);

        /// <summary>
        /// The SyncLHUserAsync method.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task SyncLHUserAsync(int userId);

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="newLhUser">The newLhUser.</param>
        /// <returns>The <see cref="T:Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateUserAsync(UserCreateViewModel newLhUser);

        /// <summary>
        /// Update a user.
        /// </summary>
        /// <param name="userUpdateViewModel">The lhUser update model.</param>
        /// <returns>The <see cref="T:Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserAsync(UserUpdateViewModel userUpdateViewModel);

        /// <summary>
        /// The get user profile async.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>The <see cref="Task{UserProfile}"/>.</returns>
        Task<UserProfile> GetUserProfileAsync(int userId);

        /// <summary>
        /// The create user profile async.
        /// </summary>
        /// <param name="userProfile">The userProfile.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> CreateUserProfileAsync(UserProfile userProfile);

        /// <summary>
        /// The update user profile async.
        /// </summary>
        /// <param name="userProfile">The userProfile.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserProfileAsync(UserProfile userProfile);

        /// <summary>
        /// The get user profile summary async.
        /// </summary>
        /// <returns>The <see cref="Task{GetUserProfileSummaryAsync}"/>.</returns>
        Task<UserProfileSummaryViewModel> GetUserProfileSummaryAsync();

        /// <summary>
        /// The get user personal details async.
        /// </summary>
        /// <returns>The <see cref="Task{GetUserPersonalDetailsAsync}"/>.</returns>
        Task<UserPersonalDetailsViewModel> GetUserPersonalDetailsAsync();

        /// <summary>
        /// The update user personal details async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user personal details model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserPersonalDetailsAsync(int userId, UserPersonalDetailsViewModel model);

        /// <summary>
        /// The get user email details async.
        /// </summary>
        /// <returns>The <see cref="Task{GetUserEmailDetailsAsync}"/>.</returns>
        Task<UserEmailDetailsViewModel> GetUserEmailDetailsAsync();

        /// <summary>
        /// The update user email details async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user email details model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserEmailDetailsAsync(int userId, UserEmailDetailsViewModel model);

        /// <summary>
        /// the get user location details.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserLocationViewModel> GetUserLocationDetailsAsync();

        /// <summary>
        /// the update user location details.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user location model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserLocationDetailsAsync(int userId, UserLocationViewModel model);

        /// <summary>
        /// The update user personal details async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user first name model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserFirstNameAsync(int userId, UserPersonalDetailsViewModel model);

        /// <summary>
        /// The update user last name async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user last name model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserLastNameAsync(int userId, UserPersonalDetailsViewModel model);

        /// <summary>
        /// The update user preferred name async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user prefeered name details model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserPreferredNameAsync(int userId, UserPersonalDetailsViewModel model);

        /// <summary>
        /// The update user email details async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user primary email details model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserPrimaryEmailAsync(int userId, UserEmailDetailsViewModel model);

        /// <summary>
        /// The update user email details async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user secondary email details model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserSecondaryEmailAsync(int userId, UserEmailDetailsViewModel model);

        /// <summary>
        /// the update user location details.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user country model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserCountryDetailsAsync(int userId, UserLocationViewModel model);

        /// <summary>
        /// the update user location details.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="model">The user region model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserRegionDetailsAsync(int userId, UserLocationViewModel model);

        /// <summary>
        /// Get user role upgarde details.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserRoleUpgrade> GetUserRoleUpgradeAsync(int userId);

        /// <summary>
        /// Create user role upgarde details.
        /// </summary>
        /// <param name="userRoleUpgradeModel">The userRoleUpgradeModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> CreateUserRoleUpgradeAsync(UserRoleUpgrade userRoleUpgradeModel);

        /// <summary>
        /// the update user role upgrade details.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserRoleUpgradeAsync();

        /// <summary>
        /// the update user role upgrade and email.
        /// </summary>
        /// <param name="email">The email Address.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserPrimaryEmailAsync(string email);

        /// <summary>
        /// RegenerateEmailChangeValidationToken.
        /// </summary>
        /// <param name="newPrimaryEmail">The email Address.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<EmailChangeValidationTokenViewModel> RegenerateEmailChangeValidationTokenAsync(string newPrimaryEmail, bool isUserRoleUpgrade);

        /// <summary>
        /// User Can request for password reset.
        /// </summary>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CanRequestPasswordResetAsync(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit);

        /// <summary>
        /// GenerateEmailChangeValidationTokenAndSendEmail.
        /// </summary>
        /// <param name="emailAddress">The email Address.</param>
        /// <param name="isUserRoleUpgrade">The isUserRoleUpgrade.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<EmailChangeValidationTokenViewModel> GenerateEmailChangeValidationTokenAndSendEmailAsync(string emailAddress, bool isUserRoleUpgrade);

        /// <summary>
        /// The ValidateEmailChangeTokenAsync.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="loctoken">Loc token.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<EmailChangeValidationTokenResult> ValidateEmailChangeTokenAsync(string token, string loctoken, bool isUserRoleUpgrade);

        /// <summary>
        /// CancelEmailChangeValidationToken.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task CancelEmailChangeValidationTokenAsync();

        /// <summary>
        /// uprade user role as general to full access user.
        /// </summary>
        /// <param name="userId">the userId.</param>
        /// <param name="email">the email.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpgradeAsFullAccessUserAsync(int userId, string email);

        /// <summary>
        /// Get providers by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>providers.</returns>
        Task<List<ProviderViewModel>> GetProvidersByUserIdAsync(int userId);

        /// <summary>
        /// To get the Base64MD5HashDigest value.
        /// </summary>
        /// <param name="szString">the string.</param>
        /// <returns>base64 string.</returns>
        string Base64MD5HashDigest(string szString);
    }
}
