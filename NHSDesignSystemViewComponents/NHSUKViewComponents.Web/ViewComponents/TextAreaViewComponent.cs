namespace NHSUKViewComponents.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.Helpers;
    using NHSUKViewComponents.Web.ViewModels;

    public class TextAreaViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render TextArea view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="label"></param>
        /// <param name="populateWithCurrentValue"></param>
        /// <param name="rows"></param>
        /// <param name="spellCheck"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="cssClass"></param>
        /// <param name="characterCount"></param>
        /// <param name="maxLengthClientSideErrorMessage"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            int rows,
            bool spellCheck,
            string hintText,
            string cssClass,
            int? characterCount,
            string? maxLengthClientSideErrorMessage = default
            )
        {
            var model = ViewData.Model;

            var valueToSet = ViewComponentValueToSetHelper.DeriveValueToSet(ref aspFor, populateWithCurrentValue, model, ViewData, out var errorMessages);

            var textBoxViewModel = new TextAreaViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                rows,
                spellCheck,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                characterCount,
                maxLengthClientSideErrorMessage
                );
            return View(textBoxViewModel);
        }
    }
}
