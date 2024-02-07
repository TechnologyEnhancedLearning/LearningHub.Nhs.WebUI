// <copyright file="UserNameExistsValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// This class checks whether a user name exists in the Learning Hub.
    /// </summary>
    public class UserNameExistsValidator
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameExistsValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository.</param>
        public UserNameExistsValidator(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string userName, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (!string.IsNullOrEmpty(userName) && this.userRepository.GetByUsernameAsync(userName, false).Result == null)
            {
                result.AddError(modelPropertyName, $"Username '{userName}' not found in Learning Hub database.");
            }
        }
    }
}
