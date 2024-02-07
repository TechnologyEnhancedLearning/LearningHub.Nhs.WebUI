// <copyright file="DisplayConfirmationViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Models
{
    /// <summary>
    /// Defines the <see cref="DisplayConfirmationViewModel" />.
    /// </summary>
    public class DisplayConfirmationViewModel
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Message HTML content to display.
        /// </summary>
        public string ConfirmationMessageHtml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen's "main" button (Button2) should perform a POST request rather than a GET.
        /// </summary>
        public bool IsPost { get; set; } = true;

        /// <summary>
        /// Gets or sets the ViewModelToPost. Only used if IsPost is set to true. This allows a viewmodel to be posted to the URL. Can be left null.
        /// Note: The screen only currently supports flat viewmodels, there is no recursion through child objects. That could be added though.
        /// </summary>
        public dynamic ViewModelToPost { get; set; }

        /// <summary>
        /// Gets or sets the Button1Text. Button1 is intended to be a cancel/back button.
        /// </summary>
        public string Button1Text { get; set; } = "Cancel";

        /// <summary>
        /// Gets or sets the Button1ReturnUrl. Button1 is intended to be a cancel/back button, therefore it will result in a GET request.
        /// </summary>
        public string Button1ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the Button1CssClassNames. Multiple class names can be used, separate with a space.
        /// </summary>
        public string Button1CssClassNames { get; set; } = "btn-outline-custom";

        /// <summary>
        /// Gets or sets the Button2Text. Button2 is intended to be used to confirm the action, e.g. "OK".
        /// </summary>
        public string Button2Text { get; set; } = "OK";

        /// <summary>
        /// Gets or sets the Button2ReturnUrl. Button2 is intended to be used to confirm the action, and therefore by default will make a POST request. Set IsPost to false for GET.
        /// </summary>
        public string Button2ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the Button2CssClassNames. Multiple class names can be used, separate with a space.
        /// </summary>
        public string Button2CssClassNames { get; set; } = "btn-custom";
    }
}
