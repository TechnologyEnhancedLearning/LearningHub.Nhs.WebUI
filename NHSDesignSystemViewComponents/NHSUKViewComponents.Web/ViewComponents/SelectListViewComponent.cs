namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NHSUKViewComponents.Web.ViewModels;

    public class SelectListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string value,
            string? defaultOption,
            IEnumerable<SelectListItem> selectListOptions,
            string? hintText,
            string? cssClass,
            bool required,
            string? requiredClientSideErrorMessage = default
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var selectListViewModel = new SelectListViewModel(
                aspFor,
                aspFor,
                label,
                string.IsNullOrEmpty(value) ? null : value,
                selectListOptions,
                string.IsNullOrEmpty(defaultOption) ? null : defaultOption,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                errorMessage,
                hasError,
                required,
                string.IsNullOrEmpty(requiredClientSideErrorMessage) ? null : requiredClientSideErrorMessage
            );
            return View(selectListViewModel);
        }
    }
}
