// <copyright file="UserLookup.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Migration.Mapping.Helpers
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// Helper class for looking up a user id from a user name.
    /// </summary>
    public class UserLookup
    {
        private readonly IUserProfileRepository userProfileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLookup"/> class.
        /// </summary>
        /// <param name="userProfileRepository">The user profile repository.</param>
        public UserLookup(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        /// <summary>
        /// Gets the user entity for a particular user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The user id.</returns>
        public UserProfile GetByUserName(string userName)
        {
            var user = this.userProfileRepository.GetByUsernameAsync(userName).Result;
            return user;
        }
    }
}
