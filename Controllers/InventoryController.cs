using InventoryManager2.Data;
using InventoryManager2.Data.Migrations;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace InventoryManager2.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly List<string> headers = new List<string>
            {
                "Id", "Titre", "Description", "Statut", "Quantité", "Prix", "Fabricant",
                "Poids", "Dimensions", "Matériau", "Couleur", "Pays d'origine",
                "Date de fabrication", "Date d'expiration", "ID de catégorie",
                "Nom de catégorie", "ID de fournisseur", "Nom du fournisseur", "ID de Utilisateur",
                "Nom complet de l'utilisateur", "Date de création", "Date de mise à jour"
            };

        public IActionResult Index(string search)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var items = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .Where(i => i.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                items = items.Where(c => c.Name.Contains(search));

            return View(items.ToList());
        }

        public IActionResult Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id && i.UserId == userId);

            if (item == null) return NotFound();

            return View(item);
        }

        public IActionResult Create()
        {
            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateUpdateItemVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = new Item
            {
                UserId = userId,
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId,
                CustomFields = model.CustomFields?.Select(field => new CustomField
                {
                    Name = field.Name,
                    Value = field.Value,
                    DataType = field.DataType,
                }).ToList(),
                ItemDetail = new ItemDetail
                {
                    Quantity = model.ItemDetail.Quantity,
                    Price = model.ItemDetail.Price,        
                    Manufacturer = model.ItemDetail.Manufacturer, 
                    Weight = model.ItemDetail.Weight,  
                    Dimensions = model.ItemDetail.Dimensions,
                    Material = model.ItemDetail.Material,
                    Color = model.ItemDetail.Color,
                    CountryOfOrigin = model.ItemDetail.CountryOfOrigin, 
                    ItemId = model.ItemDetail.ItemId,
                    ManufactureDate = model.ItemDetail.ManufactureDate,
                    ExpiryDate = model.ItemDetail.ExpiryDate,
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Items.Add(item);
            _context.SaveChanges();

            this.Flash($"New Item named {item.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .Include(i => i.CustomFields)
                .FirstOrDefault(i => i.Id == id && i.UserId == userId);

            if (item == null) return NotFound();

            var model = new CreateUpdateItemVM
            {
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                CategoryId = item.CategoryId,
                SupplierId = item.SupplierId,
                CustomFields = item.CustomFields?.Select(field => new CreateUpdateCustomFieldVM
                {
                    Name = field.Name,
                    Value = field.Value,
                    DataType = field.DataType,
                }).ToList(),
                ItemDetail = new ItemDetailVM
                {
                    Quantity = item.ItemDetail.Quantity,
                    Price = item.ItemDetail.Price,
                    Manufacturer = item.ItemDetail.Manufacturer,
                    Weight = item.ItemDetail.Weight,
                    Dimensions = item.ItemDetail.Dimensions,
                    Material = item.ItemDetail.Material,
                    Color = item.ItemDetail.Color,
                    CountryOfOrigin = item.ItemDetail.CountryOfOrigin,
                    ItemId = item.ItemDetail.ItemId,
                    ManufactureDate = item.ItemDetail.ManufactureDate,
                    ExpiryDate = item.ItemDetail.ExpiryDate,
                },
            };

            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreateUpdateItemVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .Include(i => i.CustomFields)
                .FirstOrDefault(i => i.Id == id && i.UserId == userId);

            if (item == null) return NotFound();

            item.Name = model.Name;
            item.Description = model.Description;
            item.Status = model.Status;
            item.CategoryId = model.CategoryId;
            item.SupplierId = model.SupplierId;
            item.UpdatedAt = DateTime.UtcNow;
            item.ItemDetail.Quantity = model.ItemDetail.Quantity;
            item.ItemDetail.Price = model.ItemDetail.Price;
            item.ItemDetail.Manufacturer = model.ItemDetail.Manufacturer;
            item.ItemDetail.Weight = model.ItemDetail.Weight;
            item.ItemDetail.Dimensions = model.ItemDetail.Dimensions;
            item.ItemDetail.Material = model.ItemDetail.Material;
            item.ItemDetail.Color = model.ItemDetail.Color;
            item.ItemDetail.CountryOfOrigin = model.ItemDetail.CountryOfOrigin;
            item.ItemDetail.ManufactureDate = model.ItemDetail.ManufactureDate;
            item.ItemDetail.ExpiryDate = model.ItemDetail.ExpiryDate;

            var existingCustomFields = item.CustomFields ?? new List<CustomField>();
            var updatedCustomFields = model.CustomFields ?? new List<CreateUpdateCustomFieldVM>();

            foreach (var field in updatedCustomFields)
            {
                var existingField = existingCustomFields.FirstOrDefault(f => f.Name == field.Name);
                if (existingField != null)
                {
                    existingField.Value = field.Value;
                    existingField.DataType = field.DataType;
                    _context.CustomFields.Update(existingField);
                }
                else
                    item.CustomFields.Add(new CustomField
                    {
                        Name = field.Name,
                        Value = field.Value,
                        DataType = field.DataType,
                        ItemId = item.Id
                    });
            }

            foreach (var field in existingCustomFields)
            {
                if (!updatedCustomFields.Any(f => f.Name == field.Name))
                    _context.CustomFields.Remove(field);
            }

            _context.Items.Update(item);
            _context.SaveChanges();

            this.Flash($"Item named {item.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id && i.UserId == userId);

            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.ItemDetail)
                .Include(i => i.CustomFields)
                .FirstOrDefault(i => i.Id == id && i.UserId == userId);

            if (item == null) return NotFound();

            if (item.ItemDetail != null)
                _context.ItemDetails.Remove(item.ItemDetail);

            if (item.CustomFields != null)
                _context.CustomFields.RemoveRange(item.CustomFields);

            _context.Items.Remove(item);
            _context.SaveChanges();

            this.Flash($"Item named {item.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Export(string exportType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = _context.Items
                .Where(i => i.UserId == userId)
                .Include(i => i.User)
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .Include(i => i.CustomFields)
                .ToList();
            
            if (items.Count == 0) return NotFound();

            switch (exportType.ToLower())
            {
                case "xlsx":
                    return this.ExportToExcel(items);
                case "csv":
                    return this.ExportToCsv(items);
                default:
                    return BadRequest("Invalid export type.");
            }
        }

        public IActionResult ExportToExcel(List<Item> items)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Items");

                for (int i = 0; i < this.headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = this.headers[i];
                }

                int row = 2;
                foreach (var item in items)
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Description;
                    worksheet.Cells[row, 4].Value = item.Status.ToString();
                    worksheet.Cells[row, 5].Value = item.ItemDetail?.Quantity;
                    worksheet.Cells[row, 6].Value = item.ItemDetail?.Price;
                    worksheet.Cells[row, 7].Value = item.ItemDetail?.Manufacturer;
                    worksheet.Cells[row, 8].Value = item.ItemDetail?.Weight;
                    worksheet.Cells[row, 9].Value = item.ItemDetail?.Dimensions;
                    worksheet.Cells[row, 10].Value = item.ItemDetail?.Material;
                    worksheet.Cells[row, 11].Value = item.ItemDetail?.Color;
                    worksheet.Cells[row, 12].Value = item.ItemDetail?.CountryOfOrigin;
                    worksheet.Cells[row, 13].Value = item.ItemDetail?.ManufactureDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 14].Value = item.ItemDetail?.ExpiryDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 15].Value = item.CategoryId;
                    worksheet.Cells[row, 16].Value = item.Category?.Name;
                    worksheet.Cells[row, 17].Value = item.SupplierId;
                    worksheet.Cells[row, 18].Value = item.Supplier?.Name;
                    worksheet.Cells[row, 19].Value = item.UserId;
                    worksheet.Cells[row, 20].Value = item.User.UserName;
                    worksheet.Cells[row, 21].Value = item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 22].Value = item.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Items-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        public IActionResult ExportToCsv(List<Item> items)
        {
            var csv = new StringBuilder();

            csv.AppendLine(string.Join(",", this.headers));

            foreach (var item in items)
            {
                var row = new List<string>
                {
                    item.Id.ToString(),
                    item.Name ?? "",
                    item.Description ?? "",
                    item.Status.ToString(),
                    item.ItemDetail?.Quantity.ToString() ?? "",
                    item.ItemDetail?.Price?.ToString("F2", CultureInfo.InvariantCulture)?? "",
                    item.ItemDetail?.Manufacturer ?? "",
                    item.ItemDetail?.Weight?.ToString("F2", CultureInfo.InvariantCulture) ?? "",
                    item.ItemDetail?.Dimensions ?? "",
                    item.ItemDetail?.Material ?? "",
                    item.ItemDetail?.Color ?? "",
                    item.ItemDetail?.CountryOfOrigin ?? "",
                    item.ItemDetail?.ManufactureDate.ToString("yyyy-MM-dd") ?? "",
                    item.ItemDetail?.ExpiryDate.ToString("yyyy-MM-dd") ?? "",
                    item.CategoryId.ToString() ?? "",
                    item.Category?.Name ?? "",
                    item.SupplierId.ToString() ?? "",
                    item.Supplier?.Name ?? "",
                    item.UserId.ToString() ?? "",
                    item.User?.UserName ?? "",
                    item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                csv.AppendLine(string.Join(",", row));
            }

            var fileName = $"Items-{DateTime.Now:yyyyMMddHHmmss}.csv";
            var contentType = "text/csv";
            var bytes = Encoding.UTF32.GetBytes(csv.ToString());

            return File(bytes, contentType, fileName);
        }

        public SelectList GetStatusSelectList()
        {
            return new SelectList(
                Enum.GetValues(typeof(Item.ItemStatus))
                .Cast<Item.ItemStatus>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }
            ).ToList(), "Value", "Text");
        }
    }
}
