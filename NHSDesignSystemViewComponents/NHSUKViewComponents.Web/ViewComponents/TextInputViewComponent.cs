namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.ViewModels;

    public class TextInputViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render TextInput view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="label"></param>
        /// <param name="populateWithCurrentValue"></param>
        /// <param name="type"></param>
        /// <param name="spellCheck"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="autocomplete">Leave blank to set no autocomplete on the input element.</param>
        /// <param name="cssClass"></param>
        /// <param name="required"></param>
        /// <param name="placeholderText"></param>
        /// <param name="requiredClientErrorMessage"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            string type,
            bool spellCheck,
            string hintText,
            string autocomplete,
            string cssClass,
            bool required,            
            string? placeholderText = null,
            string? requiredClientSideErrorMessage = default
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            var errorMessages = ViewData.ModelState[property?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                                new string[] { };

            var textBoxViewModel = new TextInputViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                type,
                spellCheck,
                string.IsNullOrEmpty(autocomplete) ? null : autocomplete,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                required,
                string.IsNullOrEmpty(placeholderText) ? null : placeholderText,
                string.IsNullOrEmpty(requiredClientSideErrorMessage) ? null : requiredClientSideErrorMessage
            );
            return View(textBoxViewModel);
        }
    }

}
