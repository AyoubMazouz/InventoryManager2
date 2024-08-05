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

namespace InventoryManager2.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var suppliers = _context.Supplier.ToList();
            return View(suppliers);
        }

        public IActionResult Details(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
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

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
