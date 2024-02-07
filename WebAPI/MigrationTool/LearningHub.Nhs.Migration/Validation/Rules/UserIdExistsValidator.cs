// <copyright file="UserIdExistsValidator.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Validation.Rules
{
    using LearningHub.Nhs.Migration.Models;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// This class checks whether a user id exists in the Learning Hub.
    /// </summary>
    public class UserIdExistsValidator
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdExistsValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository.</param>
        public UserIdExistsValidator(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Performs the validation check.
        /// </summary>
        /// <param name="userIdString">The user id.</param>
        /// <param name="result">The validation result to add any error to.</param>
        /// <param name="modelPropertyName">The input model property name to use in the validation error.</param>
        public void Validate(string userIdString, MigrationInputRecordValidationResult result, string modelPropertyName)
        {
            if (int.TryParse(userIdString, out int userId))
            {
                if (userId > 0 && this.userRepository.GetByIdAsync(userId).Result == null)
                {
                    result.AddError(modelPropertyName, $"User ID '{userId}' not found in Learning Hub database.");
                }
            }
        }
    }
}
