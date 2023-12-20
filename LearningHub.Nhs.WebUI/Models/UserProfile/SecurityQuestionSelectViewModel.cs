// <copyright file="SecurityQuestionSelectViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Defines the <see cref="SecurityQuestionSelectViewModel" />.
    /// </summary>
    public class SecurityQuestionSelectViewModel
    {
        /// <summary>
        /// Gets or sets the list of possible security questions.
        /// </summary>
        public List<SelectListItem> SecurityQuestions { get; set; }

        /// <summary>
        /// Gets or sets the question index - i.e. whether the UI is capturing data for the first or second security question.
        /// </summary>
        public int QuestionIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected SecurityQuestionId.
        /// </summary>
        [Range(1, 13, ErrorMessage = "Please select a security question")]
        public int SelectedSecurityQuestionId { get; set; }
    }
}
