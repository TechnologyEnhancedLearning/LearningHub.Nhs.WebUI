namespace NHSUKViewComponents.Web.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class RadiosViewModel
    {
        public RadiosViewModel(
            string aspFor,
            string label,
            string hintText,
            IEnumerable<RadiosItemViewModel> radios,
            RadiosItemViewModel optionalRadio,
            IEnumerable<string> errorMessages,
            bool required,
            string? requiredClientSideErrorMessage = default,
            string? cssClass = default,
            bool? isPageHeading = false
        )
        {
            var errorMessageList = errorMessages.ToList();
            AspFor = aspFor;
            Label = !required && !label.EndsWith("(optional)") ? label + " (optional)" : label;
            HintText = hintText;
            Radios = radios;
            OptionalRadio = optionalRadio;
            ErrorMessages = errorMessageList;
            HasError = errorMessageList.Any();
            Required = required;
            RequiredClientSideErrorMessage = requiredClientSideErrorMessage;
            Class = cssClass;
            IsPageHeading = isPageHeading;
        }

        public string AspFor { get; set; }

        public string Label { get; set; }

        public string HintText { get; set; }
        public string? Class { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
        public readonly bool HasError;

        public IEnumerable<RadiosItemViewModel> Radios { get; set; }
        public RadiosItemViewModel OptionalRadio { get; set; }
        public bool? IsPageHeading { get; set; }
        public bool Required { get; set; }
        public string RequiredClientSideErrorMessage { get; set; }
    }
}
