using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Diagnostics;
using System.Security.Claims;

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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var items = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .Where(i => i.UserId == userId)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null || item.UserId != userId) return NotFound();

            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
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
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId,
                UserId = userId,
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
                    ManufactureDate = (DateTime)model.ItemDetail.ManufactureDate,
                    ExpiryDate = (DateTime)model.ItemDetail.ExpiryDate,
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            this.Flash($"New Item named {item.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null || item.UserId != userId) return NotFound();

            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ItemViewModel model)
        {
            if (id == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.Items.Include(i => i.ItemDetail).FirstOrDefaultAsync(i => i.Id == id);

            if (item == null || item.UserId != userId) return NotFound();

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

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            this.Flash($"Item named {item.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null || item.UserId != userId) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _context.Items
                .Include(i => i.Category)
                .FirstOrDefault(i => i.Id == id);

            if (item == null || item.UserId != userId) return NotFound();

            if (item.ItemDetail != null)
                _context.ItemDetails.Remove(item.ItemDetail);

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            this.Flash($"Item named {item.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Export(string exportType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _context.Items
                .Where(i => i.UserId == userId)
                .Include(i => i.User)
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .ToListAsync();
            
            if (items.Count == 0) return NotFound();

            switch (exportType.ToLower())
            {
                case "xlsx":
                    return await this.ExportToExcel(items, "items");
                case "csv":
                    return await this.ExportToCsv(items, "items");
                default:
                    return BadRequest("Invalid export type.");
            }
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

        public async Task<IActionResult> ExportToExcel(List<Item> items, string label)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(label);
                var headers = new List<string>
                {
                    "Id", "Titre", "Description", "Statut", "Quantité", "Prix", "Fabricant",
                    "Poids", "Dimensions", "Matériau", "Couleur", "Pays d'origine",
                    "Date de fabrication", "Date d'expiration", "ID de catégorie",
                    "Nom de catégorie", "ID de fournisseur", "Nom du fournisseur", "ID de Utilisateur", 
                    "Nom complet de l'utilisateur", "Date de création", "Date de mise à jour"
                };

                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
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
                var fileName = $"{label}-{DateTime.Now.ToString()}.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        public async Task<IActionResult> ExportToCsv(List<Item> items, string label)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(label);
                worksheet.Cells["A1"].LoadFromCollection(items, true);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"{label}-{DateTime.Now.ToString()}.xlsx";
                return File(stream, contentType, fileName);
            }
        }
    }
}
