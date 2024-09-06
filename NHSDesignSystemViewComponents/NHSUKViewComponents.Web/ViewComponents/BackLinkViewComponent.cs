namespace NHSUKViewComponents.Web.ViewComponents
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using NHSUKViewComponents.Web.ViewModels;

    public class BackLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspController,
            string aspAction,
            Dictionary<string, string> aspAllRouteData,
            string linkText
        )
        {
            return View(new LinkViewModel(aspController, aspAction, linkText, aspAllRouteData));
        }
    }
}
