namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.ViewModels;

    public class NumericInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            string type,
            string hintText,
            string cssClass,
            bool required,
            string? requiredClientSideErrorMessage = default,
            string? regularExClientSideErrorMessage = default
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            var errorMessages = ViewData.ModelState[property?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                                new string[] { };

            var numericInputViewModel = new NumericInputViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                type,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                required,
                string.IsNullOrEmpty(requiredClientSideErrorMessage) ? null : requiredClientSideErrorMessage,
                string.IsNullOrEmpty(regularExClientSideErrorMessage) ? null : regularExClientSideErrorMessage                
            );
            return View(numericInputViewModel);
        }
    }
}
