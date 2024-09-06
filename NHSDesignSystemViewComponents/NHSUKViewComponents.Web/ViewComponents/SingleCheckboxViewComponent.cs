namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.ViewModels;

    public class SingleCheckboxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string? hintText
        )
        {
            var model = ViewData.Model;
            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = (bool)(property?.GetValue(model) ?? false);
            var errorMessage = ViewData.ModelState[property?.Name]?.Errors?.FirstOrDefault()?.ErrorMessage;

            var viewModel = new CheckboxItemViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                hintText,
                errorMessage
            );
            return View(viewModel);
        }
    }
}
