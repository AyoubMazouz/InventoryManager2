using Microsoft.AspNetCore.Mvc;

namespace InventoryManager2.Controllers
{
    public class BaseController : Controller
    {

        public enum AlertType
        {
            Success,
            Info,
            Warn,
            Danger,
        }

        private readonly Dictionary<AlertType, string> AlertTypeMappings = new Dictionary<AlertType, string>
        {
            { AlertType.Success, "alert-success" },
            { AlertType.Info, "alert-info" },
            { AlertType.Warn, "alert-warning" },
            { AlertType.Danger, "alert-danger" }
        };

        public void Flash(string message, AlertType type = AlertType.Success)
      {
            TempData["Alert.Type"] = this.AlertTypeMappings[type];
            TempData["Alert.Message"] = message;
      }
    }
}
