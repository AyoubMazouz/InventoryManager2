using InventoryManager2.Data;
using InventoryManager2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using OfficeOpenXml;
using System.IO;

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
