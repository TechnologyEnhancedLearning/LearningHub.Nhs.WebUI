namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="SecurityQuestionAnswerViewModel" />.
    /// </summary>
    public class SecurityQuestionAnswerViewModel
    {
        /// <summary>
        /// Gets or sets the UserSecurityQuestion Id, if editing an existing question.
        /// </summary>
        public int UserSecurityQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the SecurityQuestion Id.
        /// </summary>
        public int SecurityQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the question index - i.e. whether the UI is capturing data for the first or second security question.
        /// </summary>
        public int QuestionIndex { get; set; }

        /// <summary>
        /// Gets or sets the user's answer to the security question.
        /// </summary>
        [Required(ErrorMessage = "Please enter an answer to the security question")]
        public string SecurityQuestionAnswer { get; set; }
    }
}
