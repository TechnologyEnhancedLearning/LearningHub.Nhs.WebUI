namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using elfhHub.Nhs.Models.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NHSUKFrontendRazor.ViewModels;

    /// <summary>
    /// Defines the <see cref="MyAcountSecurityQuestionsViewModel" />.
    /// </summary>
    public class MyAcountSecurityQuestionsViewModel
    {
        /// <summary>
        /// Gets or sets selectedQuestion.
        /// </summary>
        public int SelectedFirstQuestionId { get; set; }

        /// <summary>
        /// Gets or sets selectedQuestion.
        /// </summary>
        public int SelectedSecondQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the security question answer hash.
        /// </summary>
        public string SecurityFirstQuestionAnswerHash { get; set; }

        /// <summary>
        /// Gets or sets the security question answer hash.
        /// </summary>
        public string SecuritySecondQuestionAnswerHash { get; set; }

        /// <summary>
        /// Gets or sets the FirstSecurityQuestions.
        /// </summary>
        public IEnumerable<SelectListItem> FirstSecurityQuestions { get; set; }

        /// <summary>
        /// Gets or sets the SecondSecurityQuestions.
        /// </summary>
        public IEnumerable<SelectListItem> SecondSecurityQuestions { get; set; }

        /// <summary>
        /// Gets or sets the UserSecurityFirstQuestionId.
        /// </summary>
        public int UserSecurityFirstQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the UserSecurityFirstQuestionId.
        /// </summary>
        public int UserSecuritySecondQuestionId { get; set; }
    }
}
