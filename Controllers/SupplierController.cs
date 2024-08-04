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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Supplier.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUpdateSupplierVM model)
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
            await _context.SaveChangesAsync();

            this.Flash($"New Category named {model.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CreateUpdateSupplierVM model)
        {
            if (id == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            supplier.Name = model.Name;
            supplier.ContactInfo = model.ContactInfo;

            _context.Update(supplier);
            await _context.SaveChangesAsync();

            this.Flash($"Supplier named {supplier.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();

            this.Flash($"supplier named {supplier.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
