// <copyright file="TrayViewModel.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TrayViewModel" />.
    /// </summary>
    public class TrayViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrayViewModel"/> class.
        /// </summary>
        public TrayViewModel()
        {
            this.TrayCardList = new HashSet<TrayCard>();
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the TrayCardList.
        /// </summary>
        public ICollection<TrayCard> TrayCardList { get; set; }
    }
}
