using Web.Data;
using Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using OfficeOpenXml;
using System.IO;

namespace Web.Controllers
{
    public class ViewController : Controller
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
