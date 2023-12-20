// <copyright file="SecurityViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models.UserProfile
{
    using System.Collections.Generic;
    using elfhHub.Nhs.Models.Common;
    using NHSUKViewComponents.Web.ViewModels;

    /// <summary>
    /// Defines the <see cref="SecurityViewModel" />.
    /// </summary>
    public class SecurityViewModel : SecurityQuestionsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityViewModel"/> class.
        /// </summary>
        public SecurityViewModel()
        {
        }

        /// <summary>
        /// Gets or sets radio.
        /// </summary>
        public List<RadiosItemViewModel> Radios { get; set; }

        /// <summary>
        /// Gets or sets selectedQuestion.
        /// </summary>
        public int SelectedQuestion { get; set; }
  }
}
