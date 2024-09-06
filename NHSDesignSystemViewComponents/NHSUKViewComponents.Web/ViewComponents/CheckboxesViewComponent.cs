namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.ViewModels;

    public class CheckboxesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string label,
            IEnumerable<CheckboxListItemViewModel> checkboxes,
            bool populateWithCurrentValues,
            string? errormessage,
            string? hintText,
            bool required,
            string cssClass = default
        )
        {
            var checkboxList = checkboxes.Select(
                c => new CheckboxItemViewModel(
                    c.AspFor,
                    c.AspFor,
                    c.Label,
                    GetValueFromModel(c.AspFor, populateWithCurrentValues),
                    c.HintText,
                    null
                )
            );

            var viewModel = new CheckboxesViewModel(
                label,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                errormessage,
                checkboxList,
                required,
                string.IsNullOrEmpty(cssClass) ? null : cssClass
            );

            return View(viewModel);
        }

        private bool GetValueFromModel(string aspFor, bool populateWithCurrentValue)
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            return populateWithCurrentValue && (bool)property?.GetValue(model)!;
        }
    }
}
