namespace LearningHub.Nhs.WebUI.Models.DynamicCheckbox
{
    /// <summary>
    /// DynamicCheckboxItemViewModel.
    /// </summary>
    public class DynamicCheckboxItemViewModel
    {
        /// <summary>
        /// Gets or sets a value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a HintText.
        /// </summary>
        public string? HintText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a selected.
        /// </summary>
        public bool Selected { get; set; }
    }
}
