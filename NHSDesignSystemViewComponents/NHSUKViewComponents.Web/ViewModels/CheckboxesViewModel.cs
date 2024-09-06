namespace NHSUKViewComponents.Web.ViewModels
{
    using System.Collections.Generic;

    public class CheckboxesViewModel
    {
        public CheckboxesViewModel(
            string label,
            string? hintText,
            string? errormessage,
            IEnumerable<CheckboxItemViewModel> checkboxes,
            bool required = false,
            string? cssClass = default
        )
        {
            Label = label;
            HintText = hintText;
            Checkboxes = checkboxes;
            ErrorMessage = errormessage;
            Required = required;
            Class = cssClass;
        }

        public string Label { get; set; }

        public string? HintText { get; set; }

        public string? Class { get; set; }

        public string? ErrorMessage { get; set; }

        public IEnumerable<CheckboxItemViewModel> Checkboxes { get; set; }

        public bool Required { get; set; }
    }
}
