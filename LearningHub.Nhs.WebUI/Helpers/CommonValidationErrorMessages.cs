// <copyright file="CommonValidationErrorMessages.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.WebUI.Helpers
{
    /// <summary>
    /// Defines the <see cref="CommonValidationErrorMessages" />.
    /// </summary>
    public static class CommonValidationErrorMessages
    {
        /// <summary>
        /// Secondary email should not be same.
        /// </summary>
        public const string SecondaryEmailShouldNotBeSame = "Secondary email address should not be same as primary email address";

        /// <summary>
        /// Primary email Required.
        /// </summary>
        public const string ValidPasswordRequired = "Enter a valid password";

        /// <summary>
        /// Primary email Required.
        /// </summary>
        public const string PasswordNotRecognised = "Your password isn't recognised";

        /// <summary>
        /// Diffrent security question Required.
        /// </summary>
        public const string DuplicateQuestion = "Your chosen security question cannot be the same as your other security question. Please select a different question.";

        /// <summary>
        /// Diffrent search text Required.
        /// </summary>
        public const string SearchTextRequired = "Field cannot be empty";

        /// <summary>
        /// Diffrent search term Required.
        /// </summary>
        public const string SearchTermRequired = "Please enter a search term";

        /// <summary>
        /// Country Required.
        /// </summary>
        public const string CountryRequired = "Select a country";

        /// <summary>
        /// Region Required.
        /// </summary>
        public const string RegionRequired = "Select a region";

        /// <summary>
        /// Region Required summary.
        /// </summary>
        public const string RegionRequiredSummary = "A region is required";

        /// <summary>
        /// Role Required.
        /// </summary>
        public const string RoleRequired = "Select a role";

        /// <summary>
        /// Grade Required.
        /// </summary>
        public const string GradeRequired = "Select a grade";

        /// <summary>
        /// Primary specialty Not Applicable.
        /// </summary>
        public const string SpecialtyNotApplicable = "Select not applicable if there is no search result available";

        /// <summary>
        /// Primary specialty Required.
        /// </summary>
        public const string SpecialtyRequired = "Select a primary specialty or not applicable";

        /// <summary>
        /// Start date Required.
        /// </summary>
        public const string StartDate = "Enter a start date containing a day, month and a year";

        /// <summary>
        /// Workplace Required.
        /// </summary>
        public const string WorkPlace = "Select a place of work";

        /// <summary>
        /// First Name Success Message.
        /// </summary>
        public const string FirstNameSuccessMessage = "Your first name has been changed";

        /// <summary>
        /// Last Name Success Message.
        /// </summary>
        public const string LastNameSuccessMessage = "Your last name has been changed";

        /// <summary>
        /// Preferred Name Success Message.
        /// </summary>
        public const string PreferredNameSuccessMessage = "Your preferred name has been changed";

        /// <summary>
        /// Location Success Message.
        /// </summary>
        public const string LocationSuccessMessage = "Your country and region have been changed";

        /// <summary>
        /// Country Success Message.
        /// </summary>
        public const string CountrySuccessMessage = "Your country has been changed";

        /// <summary>
        /// Region Success Message.
        /// </summary>
        public const string RegionSuccessMessage = "Your region has been changed";

        /// <summary>
        /// primary email Success Message.
        /// </summary>
        public const string PrimaryEmailSuccessMessage = "Your primary email has been changed";

        /// <summary>
        /// Secondary email Success Message.
        /// </summary>
        public const string SecondaryEmailSuccessMessage = "Your secondary email has been changed";

        /// <summary>
        /// primary email Success Message.
        /// </summary>
        public const string EmailConfirmationSuccessMessage = "Your email address has been successfully changed";

        /// <summary>
        /// Password Success Message.
        /// </summary>
        public const string PasswordSuccessMessage = "Your password has been changed";

        /// <summary>
        /// first security question Success Message.
        /// </summary>
        public const string FirstQuestionSuccessMessage = "Your first security question has been changed";

        /// <summary>
        /// second security question Success Message.
        /// </summary>
        public const string SecondQuestionSuccessMessage = "Your second security question has been changed";

        /// <summary>
        /// enter new password Message.
        /// </summary>
        public const string PasswordRequired = "Enter your new password";

        /// <summary>
        /// password length error Message.
        /// </summary>
        public const string PasswordLengthErrorMessage = "Password must be minimum 8 characters and less than equal to 50 characters";

        /// <summary>
        /// passwords mismatch error Message.
        /// </summary>
        public const string PasswordMisMatchErrorMessage = "Password does not match the rules";

        /// <summary>
        /// Choose a less common password Message.
        /// </summary>
        public const string PasswordTooCommon = "Choose a less common password";

        /// <summary>
        /// Choose a valid password Message.
        /// </summary>
        public const string PasswordNotValid = "Enter a valid password";

        /// <summary>
        /// password matches the username Message.
        /// </summary>
        public const string PasswordMatchesUsername = "Password should not match to Username";

        /// <summary>
        /// password and confirmation do not match Message.
        /// </summary>
        public const string PasswordConfirmationMismatch = "Enter the same password twice";

        /// <summary>
        /// Too long email message.
        /// </summary>
        public const string TooLongEmail = "Email must be 100 characters or fewer";

        /// <summary>
        /// invalid email message.
        /// </summary>
        public const string InvalidEmail = "Enter an email in the correct format, like name@example.com";

        /// <summary>
        /// white space in email message.
        /// </summary>
        public const string WhitespaceInEmail = "Email must not contain any whitespace characters";

        /// <summary>
        /// Message after cancelling the email update.
        /// </summary>
        public const string EmailCancelMessage = "Your email change has been cancelled";

        /// <summary>
        /// Message after validating the email update.
        /// </summary>
        public const string EmailConfirmSucessMessage = "Success, you have upgraded to a Full access Learning Hub account";

        /// <summary>
        /// Message for duplicate Email address.
        /// </summary>
        public const string DuplicateEmailAddress = "The email address already exists";

        /// <summary>
        /// Message for Downgrade UserAccess .
        /// </summary>
        public const string DowngradeUserAccess = "Check your primary email address";

        /// <summary>
        /// Message after regenerating email confirmation mail.
        /// </summary>
        public const string RegenearteEmailSuccessMessage = "A verification email was resent to the updated email address";

        /// <summary>
        /// Message after the email update request.
        /// </summary>
        public const string EmailChangeRequestedSucessMessage = "Success, you have requested your primary email address change";

        /// <summary>
        /// Message if the validation token expired.
        /// </summary>
        public const string EmailChangeValidationTokenExpiredMessage = "There is a problem with validation or the link has expired";

        /// <summary>
        /// Message if the validation token expired.
        /// </summary>
        public const string EmailChangeValidationTokenInvalidMessage = "We cannot find the page you are looking for";
    }
}
