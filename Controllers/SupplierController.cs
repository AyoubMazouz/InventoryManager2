using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SupplierController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly List<string> headers = new List<string>
        {
            "Identiant", "Nom", "Informations de contact", "Date de création", "Date de mise à jour"
        };

        public IActionResult Index(string search, int page = 1, int pageSize = 1)
        {
            var suppliersQuery = _context.Supplier.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                suppliersQuery = suppliersQuery.Where(c => c.Name.Contains(search));
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 250) pageSize = 10;

            var suppliers = suppliersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SupplierVM
            {
                Id = s.Id,
                Name = s.Name,
                ContactInfo = s.ContactInfo ?? ""
            })
            .ToList();

            var paginatedModel = new PaginationViewModel<SupplierVM>
            {
                Items = suppliers,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = suppliersQuery.Count()
            };

            ViewBag.search = search;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            return View(paginatedModel);
        }

        public IActionResult Details(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            var model = new SupplierVM
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactInfo = supplier.ContactInfo ?? ""
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateUpdateSupplierVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var supplier = new Supplier
            {
                Name = model.Name,
                ContactInfo = model.ContactInfo,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Add(supplier);
            _context.SaveChanges();

            this.Flash($"New Category named {model.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreateUpdateSupplierVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            supplier.Name = model.Name;
            supplier.ContactInfo = model.ContactInfo;

            _context.Update(supplier);
            _context.SaveChanges();

            this.Flash($"Supplier named {supplier.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Delete(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            _context.Supplier.Remove(supplier);
            _context.SaveChanges();

            this.Flash($"supplier named {supplier.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Export(string exportType)
        {
            var suppliers = _context.Supplier.ToList();

            if (suppliers.Count == 0) return NotFound();

            switch (exportType.ToLower())
            {
                case "xlsx":
                    return this.ExportToExcel(suppliers);
                case "csv":
                    return this.ExportToCsv(suppliers);
                default:
                    return BadRequest("Invalid export type.");
            }
        }

        public IActionResult ExportToExcel(List<Supplier> suppliers)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Suppliers");

                for (int i = 0; i < this.headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = this.headers[i];
                }

                int row = 2;
                foreach (var supplier in suppliers)
                {
                    worksheet.Cells[row, 1].Value = supplier.Id;
                    worksheet.Cells[row, 2].Value = supplier.Name;
                    worksheet.Cells[row, 3].Value = supplier.ContactInfo;
                    worksheet.Cells[row, 4].Value = supplier.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 5].Value = supplier.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Suppliers-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        public IActionResult ExportToCsv(List<Supplier> suppliers)
        {
            var csv = new StringBuilder();

            csv.AppendLine(string.Join(",", this.headers));

            foreach (var supplier in suppliers)
            {
                var row = new List<string>
                {
                    supplier.Id.ToString(),
                    supplier.Name ?? "",
                    supplier.ContactInfo ?? "",
                    supplier.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    supplier.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                csv.AppendLine(string.Join(",", row));
            }

            var fileName = $"Suppliers-{DateTime.Now:yyyyMMddHHmmss}.csv";
            var contentType = "text/csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, contentType, fileName);
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
