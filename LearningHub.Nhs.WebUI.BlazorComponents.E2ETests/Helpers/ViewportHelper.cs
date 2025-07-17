using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.Helpers
{
    public static class ViewportHelper
    {
        public enum ViewportType
        {
            Desktop,
            Mobile,
            Tablet
        }

        public static readonly Dictionary<ViewportType, ViewportSize> Viewports = new()
        {
            { ViewportType.Desktop, new ViewportSize { Width = 1920, Height = 1080 } },
            { ViewportType.Mobile, new ViewportSize { Width = 360, Height = 800 } },
            { ViewportType.Tablet, new ViewportSize { Width = 768, Height = 1024 } }
        };
    }
}
