using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using OfficeOpenXml;
using System.Text;

namespace InventoryManager2.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly List<string> headers = new List<string>
        {
            "Id", "Nom", "Parent", "Date de création", "Date de mise à jour"
        };

        public IActionResult Index(string search)
        {
            var categories = _context.Category
                .Include(c => c.Parent)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                categories = categories.Where(c => c.Name.Contains(search));

            var models = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentName = c.Parent.Name ?? ""
                })
                .ToList();

            return View(models);
        }

        public IActionResult Details(int id)
        {
            var category = _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(m => m.Id == id);

            if (category == null) return NotFound();

            var model = new CategoryVM
            { 
                Name = category.Name,
                Parent = category.Parent,
                Children = category.Children,
            };

            return View(model);
        }

        public IActionResult Create()
        {
            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateUpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", model.ParentId);
                return View(model);
            }

            var parent = _context.Category.Find(model.ParentId); 

            var category = new Category
            {
                Name = model.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ParentId = model.ParentId
            };

            _context.Add(category);
            _context.SaveChanges();

            this.Flash($"New category named {category.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(m => m.Id == id);

            if (category == null) return NotFound();

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreateUpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Parent"] = new SelectList(
                    _context.Category.ToList(),
                    "Id", "Name", model.ParentId);
                return View(model);
            }

            var category = _context.Category.Find(id);

            if (category == null) return NotFound();

            var parent = _context.Category.Find(model.ParentId);

            category.Name = model.Name;
            category.UpdatedAt = DateTime.Now;
            category.ParentId = model.ParentId;
            if (parent != null) category.Parent = parent;

            _context.Update(category);
            _context.SaveChanges();

            this.Flash($"category named {model.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _context.Category.Find(id);

            if (category == null) return NotFound();

            if (this.HasDependencies(category))
            {
                this.Flash(
                    $"category named {category.Name} has one or more Subcategory that depends on it!", 
                    AlertType.Warn
                );
                return RedirectToAction(nameof(Delete));
            }

            _context.Category.Remove(category);
            _context.SaveChanges();

            this.Flash($"category named {category.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Export(string exportType)
        {
            var categories = _context.Category
                .Include(c => c.Parent)
                .ToList();

            if (categories.Count == 0) return NotFound();

            switch (exportType.ToLower())
            {
                case "xlsx":
                    return this.ExportToExcel(categories);
                case "csv":
                    return this.ExportToCsv(categories);
                default:
                    return BadRequest("Invalid export type.");
            }
        }

        public IActionResult ExportToExcel(List<Category> categories)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Categories");

                for (int i = 0; i < this.headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = this.headers[i];
                }

                int row = 2;
                foreach (var category in categories)
                {
                    worksheet.Cells[row, 1].Value = category.Id;
                    worksheet.Cells[row, 2].Value = category.Name;
                    worksheet.Cells[row, 3].Value = category.Parent?.Name ?? "N/A";
                    worksheet.Cells[row, 4].Value = category.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 5].Value = category.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Categories-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        public IActionResult ExportToCsv(List<Category> categories)
        {
            var csv = new StringBuilder();

            csv.AppendLine(string.Join(",", this.headers));

            foreach (var category in categories)
            {
                var row = new List<string>
                {
                    category.Id.ToString(),
                    category.Name ?? "",
                    category.Parent?.Name ?? "N/A",
                    category.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    category.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                csv.AppendLine(string.Join(",", row));
            }

            var fileName = $"Categories-{DateTime.Now:yyyyMMddHHmmss}.csv";
            var contentType = "text/csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, contentType, fileName);
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        private bool HasDependencies(Category category)
        {
            return _context.Category.Any(p => p.ParentId == category.Id);
        }
    }
}
