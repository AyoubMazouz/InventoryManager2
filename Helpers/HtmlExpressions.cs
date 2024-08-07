using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InventoryManager2.Helpers
{
    public static class HtmlExtensions
    {
        public static string IsActive(this HtmlHelper html, string controller = null, string action = null)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = routeData.Values["action"] as string;
            var routeController = routeData.Values["controller"] as string;

            // If the controller is specified and does not match, return an empty string
            if (controller != null && !controller.Equals(routeController, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            // If the action is specified and does not match, return an empty string
            if (action != null && !action.Equals(routeAction, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            // If both match or are not specified, return the "active" class
            return "active";
        }
    }
}
