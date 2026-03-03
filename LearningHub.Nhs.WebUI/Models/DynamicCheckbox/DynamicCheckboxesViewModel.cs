namespace LearningHub.Nhs.WebUI.Models.DynamicCheckbox
{
    using System.Collections.Generic;

    /// <summary>
    /// DynamicCheckboxesViewModel.
    /// </summary>
    public class DynamicCheckboxesViewModel
    {
        /// <summary>
        /// Gets or sets a Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a HintText.
        /// </summary>
        public string HintText { get; set; }

        /// <summary>
        /// Gets or sets a ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a Required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a CssClass.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets SelectedValues.
        /// </summary>
        public List<string> SelectedValues { get; set; } = [];

        /// <summary>
        /// Gets or sets a Checkboxes.
        /// </summary>
        public List<DynamicCheckboxItemViewModel> Checkboxes { get; set; } = [];
    }
}
