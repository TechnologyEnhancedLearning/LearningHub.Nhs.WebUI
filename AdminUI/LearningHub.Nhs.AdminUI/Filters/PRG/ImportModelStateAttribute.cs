// <copyright file="ImportModelStateAttribute.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Filters.PRG
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Defines the <see cref="ImportModelStateAttribute" />.
    /// </summary>
    public class ImportModelStateAttribute : ModelStateTransfer
    {
        /// <summary>
        /// The OnActionExecuted.
        /// </summary>
        /// <param name="filterContext">The filterContext<see cref="ActionExecutedContext"/>.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var serialisedModelState = controller?.TempData[Key] as string;

            if (serialisedModelState != null)
            {
                if (filterContext.Result is ViewResult)
                {
                    var modelState = ModelStateHelpers.DeserialiseModelState(serialisedModelState);
                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
